using HuntAndPeck.Models;
using HuntAndPeck.Properties;

namespace HuntAndPeck.ViewModels
{
    public class HintViewModel : NotifyPropertyChanged
    {
        private string _label;
        private bool _active;
        private string _fontSizeReadValue;
        private string _fontFamilyReadValue;
        private string _backgroundColorReadValue;
        private string _inactiveBackgroundColorReadValue;
        private string _textColorReadValue;

        public HintViewModel(Hint hint)
        {
            Hint = hint;
            FontSizeReadValue = Settings.Default.FontSize;
            FontFamilyReadValue = Settings.Default.FontFamily;
            BackgroundColorReadValue = Settings.Default.BackgroundColor;
            InactiveBackgroundColorReadValue = Settings.Default.InactiveBackgroundColor;
            TextColorReadValue = Settings.Default.TextColor;
        }

        public Hint Hint { get; set; }

        public bool Active
        {
            get { return _active; }
            set { _active = value; NotifyOfPropertyChange(); }
        }

        public string Label
        {
            get { return _label; }
            set { _label = value; NotifyOfPropertyChange(); }
        }

        public string FontSizeReadValue
        {
            get { return _fontSizeReadValue; }
            set { _fontSizeReadValue = value; NotifyOfPropertyChange(); }
        }

        public string FontFamilyReadValue
        {
            get { return _fontFamilyReadValue; }
            set { _fontFamilyReadValue = value; NotifyOfPropertyChange(); }
        }

        public string BackgroundColorReadValue
        {
            get { return _backgroundColorReadValue; }
            set { _backgroundColorReadValue = value; NotifyOfPropertyChange(); }
        }

        public string InactiveBackgroundColorReadValue
        {
            get { return _inactiveBackgroundColorReadValue; }
            set { _inactiveBackgroundColorReadValue = value; NotifyOfPropertyChange(); }
        }

        public string TextColorReadValue
        {
            get { return _textColorReadValue; }
            set { _textColorReadValue = value; NotifyOfPropertyChange(); }
        }
    }
}
