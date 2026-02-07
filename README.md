# ⚡ Hunt-and-Peck Enhanced

Hunt-and-Peck Enhanced is a performance-oriented fork of the original Hunt-and-Peck project, focused on improving responsiveness, expanding configurability, and modernizing the user interface while preserving the lightweight, accessibility-driven navigation model.  

This work builds directly on the original project created by Zach Sims. Full credit belongs to the original author for the concept and foundation.  

**Original Repository:**  
👉 https://github.com/zsims/hunt-and-peck  


## Overview

The original Hunt-and-Peck application provides Vim-style keyboard navigation for Windows applications using the UI Automation framework. This fork extends that capability with architectural improvements, faster hint generation, configurable controls, and a refined visual presentation suitable for modern workflows.

![Explorer Screenshot](assets/explorer.png)

<details>
<summary>**🔧 KEY ENHANCEMENTS**</summary>

### 🔧 Performance Optimization

- Implemented a concurrent caching system to store UI elements per window, reducing repeated enumeration.  
- Added cache validation to ensure associated windows and processes remain active.  
- Introduced automatic cleanup of stale cache entries.  
- Enabled parallel hint generation to improve responsiveness in complex interfaces.  
- Strengthened error handling to safely manage UI elements that are destroyed during enumeration.  

**Outcome:** Faster overlay rendering and more reliable hint activation.

---

### 🔎 Improved Hint Matching 

- Replaced linear searches with O(1) dictionary-based lookup for constant-time prefix matching.  
- Enabled case-insensitive comparisons for more forgiving input.  
- Added automatic state reset when no matches are detected.  

**Outcome:** Immediate and predictable hint execution.

---

### 🔑 Configurable Hotkeys

Hotkeys are now fully configurable rather than hardcoded.  

Users can define:

- Main overlay shortcut  
- Taskbar shortcut  
- Modifier keys (Alt, Control, Shift)  

Settings are loaded dynamically and persist automatically.

---

### ⚙️ Expanded Appearance and Settings System

A redesigned configuration infrastructure allows comprehensive personalization without code changes.  

**Configurable options include:**

- Font family and size  
- Active and inactive background colors  
- Text color  
- Gradient-based hint styling  
- Refined borders and improved text clarity  

The Options window has been resized and reorganized to support these additions with a clearer layout.

---

### ⚙️ Architectural Improvements

- Expanded settings model with automatic persistence.  
- Two-way bound view models to ensure synchronization between UI and configuration.  
- Added Win32 validation methods to confirm window handle integrity.  
- Refactored components for improved maintainability and long-term extensibility.  

</details>

## 📌 Usage

Launch the executable and trigger hints using the configured hotkeys.  


### Command-line options:

```bash
hap.exe /hint
hap.exe /tray
```

The application supports UI Automation elements that implement the Invoke pattern.


## 🙌 Credit

Full credit to Zach Sims for creating Hunt-and-Peck and the powerful idea behind it.  

This fork exists to extend, modernize, and optimize his work while preserving its lightweight philosophy.  

👉 https://github.com/zsims/hunt-and-peck
