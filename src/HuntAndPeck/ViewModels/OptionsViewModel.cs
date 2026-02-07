using HuntAndPeck.Properties;
using System;
using System.ComponentModel;
using System.Windows;

namespace HuntAndPeck.ViewModels
{
    internal class OptionsViewModel : INotifyPropertyChanged
    {
        public OptionsViewModel()
        {
            DisplayName = "Options";
            FontSize = Settings.Default.FontSize;
            FontFamily = Settings.Default.FontFamily;
            BackgroundColor = Settings.Default.BackgroundColor;
            InactiveBackgroundColor = Settings.Default.InactiveBackgroundColor;
            TextColor = Settings.Default.TextColor;
            MainHotKeyKey = Settings.Default.MainHotKeyKey;
            MainHotKeyModifier = Settings.Default.MainHotKeyModifier;
            TaskbarHotKeyKey = Settings.Default.TaskbarHotKeyKey;
            TaskbarHotKeyModifier = Settings.Default.TaskbarHotKeyModifier;
            Settings.Default.PropertyChanged += OnSettingsPropertyChanged;
        }

        public string DisplayName { get; set; }

        private string _fontSize;
        public string FontSize
        // Assign the font size value to a variable and update it every time user 
        // changes the option in tray menu
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    OnPropertyChanged("FontSize");
                    Settings.Default.FontSize = value;
                    Settings.Default.Save();
                }
            }
        }

        private string _fontFamily;
        public string FontFamily
        {
            get { return _fontFamily; }
            set
            {
                if (_fontFamily != value)
                {
                    _fontFamily = value;
                    OnPropertyChanged("FontFamily");
                    Settings.Default.FontFamily = value;
                    Settings.Default.Save();
                }
            }
        }

        private string _backgroundColor;
        public string BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                if (_backgroundColor != value)
                {
                    _backgroundColor = value;
                    OnPropertyChanged("BackgroundColor");
                    Settings.Default.BackgroundColor = value;
                    Settings.Default.Save();
                }
            }
        }

        private string _inactiveBackgroundColor;
        public string InactiveBackgroundColor
        {
            get { return _inactiveBackgroundColor; }
            set
            {
                if (_inactiveBackgroundColor != value)
                {
                    _inactiveBackgroundColor = value;
                    OnPropertyChanged("InactiveBackgroundColor");
                    Settings.Default.InactiveBackgroundColor = value;
                    Settings.Default.Save();
                }
            }
        }

        private string _textColor;
        public string TextColor
        {
            get { return _textColor; }
            set
            {
                if (_textColor != value)
                {
                    _textColor = value;
                    OnPropertyChanged("TextColor");
                    Settings.Default.TextColor = value;
                    Settings.Default.Save();
                }
            }
        }

        private string _mainHotKeyKey;
        public string MainHotKeyKey
        {
            get { return _mainHotKeyKey; }
            set
            {
                if (_mainHotKeyKey != value)
                {
                    _mainHotKeyKey = value;
                    OnPropertyChanged("MainHotKeyKey");
                    Settings.Default.MainHotKeyKey = value;
                    Settings.Default.Save();
                }
            }
        }

        private string _mainHotKeyModifier;
        public string MainHotKeyModifier
        {
            get { return _mainHotKeyModifier; }
            set
            {
                if (_mainHotKeyModifier != value)
                {
                    _mainHotKeyModifier = value;
                    OnPropertyChanged("MainHotKeyModifier");
                    Settings.Default.MainHotKeyModifier = value;
                    Settings.Default.Save();
                }
            }
        }

        private string _taskbarHotKeyKey;
        public string TaskbarHotKeyKey
        {
            get { return _taskbarHotKeyKey; }
            set
            {
                if (_taskbarHotKeyKey != value)
                {
                    _taskbarHotKeyKey = value;
                    OnPropertyChanged("TaskbarHotKeyKey");
                    Settings.Default.TaskbarHotKeyKey = value;
                    Settings.Default.Save();
                }
            }
        }

        private string _taskbarHotKeyModifier;
        public string TaskbarHotKeyModifier
        {
            get { return _taskbarHotKeyModifier; }
            set
            {
                if (_taskbarHotKeyModifier != value)
                {
                    _taskbarHotKeyModifier = value;
                    OnPropertyChanged("TaskbarHotKeyModifier");
                    Settings.Default.TaskbarHotKeyModifier = value;
                    Settings.Default.Save();
                }
            }
        }


        private void OnSettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FontSize")
            {
                FontSize = Settings.Default.FontSize;
            }
            else if (e.PropertyName == "FontFamily")
            {
                FontFamily = Settings.Default.FontFamily;
            }
            else if (e.PropertyName == "BackgroundColor")
            {
                BackgroundColor = Settings.Default.BackgroundColor;
            }
            else if (e.PropertyName == "InactiveBackgroundColor")
            {
                InactiveBackgroundColor = Settings.Default.InactiveBackgroundColor;
            }
            else if (e.PropertyName == "TextColor")
            {
                TextColor = Settings.Default.TextColor;
            }
            else if (e.PropertyName == "MainHotKeyKey")
            {
                MainHotKeyKey = Settings.Default.MainHotKeyKey;
            }
            else if (e.PropertyName == "MainHotKeyModifier")
            {
                MainHotKeyModifier = Settings.Default.MainHotKeyModifier;
            }
            else if (e.PropertyName == "TaskbarHotKeyKey")
            {
                TaskbarHotKeyKey = Settings.Default.TaskbarHotKeyKey;
            }
            else if (e.PropertyName == "TaskbarHotKeyModifier")
            {
                TaskbarHotKeyModifier = Settings.Default.TaskbarHotKeyModifier;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}