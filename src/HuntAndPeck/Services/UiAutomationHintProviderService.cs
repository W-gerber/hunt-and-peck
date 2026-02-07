using HuntAndPeck.Extensions;
using HuntAndPeck.Models;
using HuntAndPeck.NativeMethods;
using HuntAndPeck.Services.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UIAutomationClient;

namespace HuntAndPeck.Services
{
    internal class UiAutomationHintProviderService : IHintProviderService, IDebugHintProviderService
    {
        private readonly IUIAutomation _automation = new CUIAutomation();
        private static readonly ConcurrentDictionary<IntPtr, CachedWindowElements> _elementCache = new ConcurrentDictionary<IntPtr, CachedWindowElements>();
        private static readonly object _cacheLock = new object();

        private class CachedWindowElements
        {
            public List<IUIAutomationElement> Elements { get; set; }
            public DateTime LastAccessed { get; set; }
            public int ProcessId { get; set; }
        }

        public HintSession EnumHints()
        {
            var foregroundWindow = User32.GetForegroundWindow();
            if (foregroundWindow == IntPtr.Zero)
            {
                return null;
            }
            return EnumHints(foregroundWindow);
        }

        public HintSession EnumHints(IntPtr hWnd)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var session = EnumWindowHints(hWnd, CreateHint);
            sw.Stop();

            Debug.WriteLine("Enumeration of hints took {0} ms", sw.ElapsedMilliseconds);
            return session;
        }

        public HintSession EnumDebugHints()
        {
            var foregroundWindow = User32.GetForegroundWindow();
            if (foregroundWindow == IntPtr.Zero)
            {
                return null;
            }
            return EnumDebugHints(foregroundWindow);
        }

        public HintSession EnumDebugHints(IntPtr hWnd)
        {
            return EnumWindowHints(hWnd, CreateDebugHint);
        }

        /// <summary>
        /// Enumerates all the hints from the given window
        /// </summary>
        /// <param name="hWnd">The window to get hints from</param>
        /// <param name="hintFactory">The factory to use to create each hint in the session</param>
        /// <returns>A hint session</returns>
        private HintSession EnumWindowHints(IntPtr hWnd, Func<IntPtr, Rect, IUIAutomationElement, Hint> hintFactory)
        {
            var result = new ConcurrentBag<Hint>();
            var elements = GetCachedOrEnumElements(hWnd);

            // Window bounds
            var rawWindowBounds = new RECT();
            User32.GetWindowRect(hWnd, ref rawWindowBounds);
            Rect windowBounds = rawWindowBounds;

            // Use parallel processing for better performance
            Parallel.ForEach(elements, (element) =>
            {
                try
                {
                    var boundingRectObject = element.CurrentBoundingRectangle;
                    if ((boundingRectObject.right > boundingRectObject.left) && (boundingRectObject.bottom > boundingRectObject.top))
                    {
                        var niceRect = new Rect(new Point(boundingRectObject.left, boundingRectObject.top), new Point(boundingRectObject.right, boundingRectObject.bottom));
                        // Convert the bounding rect to logical coords
                        var logicalRect = niceRect.PhysicalToLogicalRect(hWnd);
                        if (!logicalRect.IsEmpty)
                        {
                            var windowCoords = niceRect.ScreenToWindowCoordinates(windowBounds);
                            var hint = hintFactory(hWnd, windowCoords, element);
                            if (hint != null)
                            {
                                result.Add(hint);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Element may have been destroyed during enumeration
                }
            });

            return new HintSession
            {
                Hints = result.ToList(),
                OwningWindow = hWnd,
                OwningWindowBounds = windowBounds,
            };
        }

        /// <summary>
        /// Gets cached elements or enumerates new ones if cache is invalid
        /// </summary>
        /// <param name="hWnd">The window handle</param>
        /// <returns>List of automation elements</returns>
        private List<IUIAutomationElement> GetCachedOrEnumElements(IntPtr hWnd)
        {
            // Check if window still exists
            if (!User32.IsWindow(hWnd))
            {
                CleanupCacheForWindow(hWnd);
                return new List<IUIAutomationElement>();
            }

            // Get process ID for the window
            User32.GetWindowThreadProcessId(hWnd, out int processId);

            // Try to get from cache
            if (_elementCache.TryGetValue(hWnd, out var cachedData))
            {
                // Verify the process is still alive
                try
                {
                    var process = Process.GetProcessById(cachedData.ProcessId);
                    if (!process.HasExited)
                    {
                        cachedData.LastAccessed = DateTime.UtcNow;
                        return cachedData.Elements;
                    }
                }
                catch
                {
                    // Process no longer exists
                }

                // Cache is invalid, remove it
                CleanupCacheForWindow(hWnd);
            }

            // Enumerate new elements and cache them
            var elements = EnumElements(hWnd);
            
            var newCache = new CachedWindowElements
            {
                Elements = elements,
                LastAccessed = DateTime.UtcNow,
                ProcessId = processId
            };

            _elementCache.TryAdd(hWnd, newCache);

            // Clean up old cache entries (older than 5 minutes or for closed windows)
            CleanupStaleCache();

            return elements;
        }

        /// <summary>
        /// Removes cache entry for a specific window
        /// </summary>
        /// <param name="hWnd">The window handle</param>
        private void CleanupCacheForWindow(IntPtr hWnd)
        {
            _elementCache.TryRemove(hWnd, out _);
        }

        /// <summary>
        /// Cleans up stale cache entries
        /// </summary>
        private void CleanupStaleCache()
        {
            var staleThreshold = DateTime.UtcNow.AddMinutes(-5);
            var keysToRemove = new List<IntPtr>();

            foreach (var kvp in _elementCache)
            {
                // Remove if last accessed more than 5 minutes ago or window no longer exists
                if (kvp.Value.LastAccessed < staleThreshold || !User32.IsWindow(kvp.Key))
                {
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
            {
                _elementCache.TryRemove(key, out _);
            }
        }

        /// <summary>
        /// Enumerates the automation elements from the given window
        /// </summary>
        /// <param name="hWnd">The window handle</param>
        /// <returns>All of the automation elements found</returns>
        private List<IUIAutomationElement> EnumElements(IntPtr hWnd)
        {
            var result = new List<IUIAutomationElement>();
            var automationElement = _automation.ElementFromHandle(hWnd);

            var conditionControlView = _automation.ControlViewCondition;
            var conditionEnabled = _automation.CreatePropertyCondition(UIA_PropertyIds.UIA_IsEnabledPropertyId, true);
            var enabledControlCondition = _automation.CreateAndCondition(conditionControlView, conditionEnabled);

            var conditionOnScreen = _automation.CreatePropertyCondition(UIA_PropertyIds.UIA_IsOffscreenPropertyId, false);
            var condition = _automation.CreateAndCondition(enabledControlCondition, conditionOnScreen);

            var elementArray = automationElement.FindAll(TreeScope.TreeScope_Descendants, condition);
            if (elementArray != null)
            {
                for (var i = 0; i < elementArray.Length; ++i)
                {
                    result.Add(elementArray.GetElement(i));
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a UI Automation element from the given automation element
        /// </summary>
        /// <param name="owningWindow">The owning window</param>
        /// <param name="hintBounds">The hint bounds</param>
        /// <param name="automationElement">The associated automation element</param>
        /// <returns>The created hint, else null if the hint could not be created</returns>
        private Hint CreateHint(IntPtr owningWindow, Rect hintBounds, IUIAutomationElement automationElement)
        {
            try
            {
                var invokePattern = (IUIAutomationInvokePattern)automationElement.GetCurrentPattern(UIA_PatternIds.UIA_InvokePatternId);
                if (invokePattern != null)
                {
                    return new UiAutomationInvokeHint(owningWindow, invokePattern, hintBounds);
                }

                var togglePattern = (IUIAutomationTogglePattern)automationElement.GetCurrentPattern(UIA_PatternIds.UIA_TogglePatternId);
                if (togglePattern != null)
                {
                    return new UiAutomationToggleHint(owningWindow, togglePattern, hintBounds);
                }
                
                var selectPattern = (IUIAutomationSelectionItemPattern) automationElement.GetCurrentPattern(UIA_PatternIds.UIA_SelectionItemPatternId);
                if (selectPattern != null)
                {
                    return new UiAutomationSelectHint(owningWindow, selectPattern, hintBounds);
                }

                var expandCollapsePattern = (IUIAutomationExpandCollapsePattern) automationElement.GetCurrentPattern(UIA_PatternIds.UIA_ExpandCollapsePatternId);
                if (expandCollapsePattern != null)
                {
                    return new UiAutomationExpandCollapseHint(owningWindow, expandCollapsePattern, hintBounds);
                }

                var valuePattern = (IUIAutomationValuePattern)automationElement.GetCurrentPattern(UIA_PatternIds.UIA_ValuePatternId);
                if (valuePattern != null && valuePattern.CurrentIsReadOnly == 0)
                {
                    return new UiAutomationFocusHint(owningWindow, automationElement, hintBounds);
                }

                var rangeValuePattern = (IUIAutomationRangeValuePattern) automationElement.GetCurrentPattern(UIA_PatternIds.UIA_RangeValuePatternId);
                if (rangeValuePattern != null && rangeValuePattern.CurrentIsReadOnly == 0)
                {
                    return new UiAutomationFocusHint(owningWindow, automationElement, hintBounds);
                }
                
                return null;
            }
            catch (Exception)
            {
                // May have gone
                return null;
            }
        }

        /// <summary>
        /// Creates a debug hint
        /// </summary>
        /// <param name="owningWindow">The window that owns the hint</param>
        /// <param name="hintBounds">The hint bounds</param>
        /// <param name="automationElement">The automation element</param>
        /// <returns>A debug hint</returns>
        private DebugHint CreateDebugHint(IntPtr owningWindow, Rect hintBounds, IUIAutomationElement automationElement)
        {
            // Enumerate all possible patterns. Note that the performance of this is *very* bad -- hence debug only.
            var programmaticNames = new List<string>();

            foreach (var pn in UiAutomationPatternIds.PatternNames)
            {
                try
                {
                    var pattern = automationElement.GetCurrentPattern(pn.Key);
                    if(pattern != null)
                    {
                        programmaticNames.Add(pn.Value);
                    }
                }
                catch (Exception)
                {
                }
            }

            if (programmaticNames.Any())
            {
                return new DebugHint(owningWindow, hintBounds, programmaticNames.ToList());
            }

            return null;
        }
    }
}
