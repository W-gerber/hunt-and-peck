using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using HuntAndPeck.Models;
using HuntAndPeck.Services.Interfaces;

namespace HuntAndPeck.ViewModels
{
    internal class OverlayViewModel : NotifyPropertyChanged
    {
        private Rect _bounds;
        private ObservableCollection<HintViewModel> _hints = new ObservableCollection<HintViewModel>();
        private Dictionary<string, List<HintViewModel>> _hintLookup = new Dictionary<string, List<HintViewModel>>(StringComparer.OrdinalIgnoreCase);

        public OverlayViewModel(
            HintSession session,
            IHintLabelService hintLabelService)
        {
            _bounds = session.OwningWindowBounds;

            var labels = hintLabelService.GetHintStrings(session.Hints.Count());
            for (int i = 0; i < labels.Count; ++i)
            {
                var hint = session.Hints[i];
                var hintViewModel = new HintViewModel(hint)
                {
                    Label = labels[i],
                    Active = false
                };
                _hints.Add(hintViewModel);

                // Build lookup dictionary for faster matching
                for (int j = 1; j <= labels[i].Length; j++)
                {
                    var prefix = labels[i].Substring(0, j).ToUpperInvariant();
                    if (!_hintLookup.ContainsKey(prefix))
                    {
                        _hintLookup[prefix] = new List<HintViewModel>();
                    }
                    _hintLookup[prefix].Add(hintViewModel);
                }
            }
        }

        /// <summary>
        /// Bounds in logical screen coordiantes
        /// </summary>
        public Rect Bounds
        {
            get
            {
                return _bounds;
            }
            set
            {
                _bounds = value;
                NotifyOfPropertyChange();
            }
        }

        public ObservableCollection<HintViewModel> Hints
        {
            get
            {
                return _hints;
            }
            set
            {
                _hints = value;
                NotifyOfPropertyChange();
            }
        }

        public Action CloseOverlay { get; set; }

        public string MatchString
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Reset all hints to inactive
                    foreach (var x in Hints)
                    {
                        x.Active = false;
                    }
                    return;
                }

                var upperValue = value.ToUpperInvariant();
                
                // Use lookup dictionary for O(1) access instead of O(n) search
                if (_hintLookup.TryGetValue(upperValue, out var matching))
                {
                    // Deactivate all hints first
                    foreach (var x in Hints)
                    {
                        x.Active = false;
                    }

                    // Activate only matching hints
                    foreach (var x in matching)
                    {
                        x.Active = true;
                    }

                    // If exactly one match, invoke it
                    if (matching.Count == 1)
                    {
                        matching[0].Hint.Invoke();
                        CloseOverlay?.Invoke();
                    }
                }
                else
                {
                    // No matches, deactivate all
                    foreach (var x in Hints)
                    {
                        x.Active = false;
                    }
                }
            }
        }
    }
}
