using System;
using System.Windows;
using System.Windows.Media;
using GazRouter.Common.ViewModel;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Flobus.Dialogs
{
    public interface ITextWidget
    {
        double FontSize { get; set; }
        FontStyle FontStyle { get; set; }
        Brush Foreground { get; set; }
        bool IsBoxVisible { get; set; }
        FontWeight FontWeight { get; set; }
        string Text { get; set; }
    }

    public class TextEditDialogViewModel : DialogViewModel<Action<TextEditDialogViewModel>>
    {
        private readonly ITextWidget _model;
        private Color _fontColor;
        private int _fontSize;
        private int _fontStyle;
        private string _text;
        private bool _isBoxVisible;
        private bool _fontBold;
        private bool _fontItalic;

/*
        public TextEditDialogViewModel(Action<TextEditDialogViewModel> closeCallback) : base(closeCallback)
        {
        }
*/

        public TextEditDialogViewModel(ITextWidget model) : base(vm=> {} )
        {
            _model = model;
            _fontSize = (int)model.FontSize;
            _fontColor = (model.Foreground as SolidColorBrush)?.Color ?? Colors.Black;
            _fontBold = model.FontWeight == FontWeights.Bold;
            _fontItalic = model.FontStyle == FontStyles.Italic;
            _isBoxVisible = model.IsBoxVisible;
            _text = model.Text;

            SaveCommand = new DelegateCommand(OnSave);
        }

        public int FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                OnPropertyChanged(() => FontSize);
            }
        }

        public int FontStyle
        {
            get { return _fontStyle; }
            set
            {
                _fontStyle = value;
                OnPropertyChanged(() => FontStyle);
            }
        }

        public DelegateCommand SaveCommand { get; private set; }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(() => Text);
            }
        }

        public Color FontColor
        {
            get { return _fontColor; }
            set
            {
                _fontColor = value;
                OnPropertyChanged(() => FontColor);
            }
        }

        public bool FontBold
        {
            get { return _fontBold; }
            set { SetProperty(ref _fontBold, value); }
        }

        public bool FontItalic
        {
            get { return _fontItalic; }
            set { SetProperty(ref _fontItalic, value); }
        }

        public bool IsBoxVisible
        {
            get { return _isBoxVisible; }
            set
            {
                _isBoxVisible = value;
                OnPropertyChanged(() => IsBoxVisible);
            }
        }

        protected override void InvokeCallback(Action<TextEditDialogViewModel> closeCallback)
        {
            closeCallback(this);
        }

        private void OnSave()
        {
            _model.FontSize = _fontSize;
           _model.FontStyle = _fontItalic ? FontStyles.Italic : FontStyles.Normal;
            _model.Foreground = new SolidColorBrush(_fontColor); 
            _model.IsBoxVisible = _isBoxVisible;
            _model.FontWeight = _fontBold ? FontWeights.Bold : FontWeights.Normal;
            _model.Text = _text;

            DialogResult = true;
        }
    }
}