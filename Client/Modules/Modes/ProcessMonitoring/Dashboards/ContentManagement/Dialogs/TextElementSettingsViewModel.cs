using System.Windows.Media;
using GazRouter.Common.ViewModel;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Dialogs
{
    public class TextElementSettingsViewModel : DialogViewModel
    {
        public DelegateCommand SaveCommand { get; private set; }
        private readonly TextElementModel _model;

        private int _fontSize;
        public int FontSize
        {
            get { return _fontSize; }
            set
            {
                _fontSize = value;
                OnPropertyChanged(() => FontSize);
            }
        }

        private int _fontStyle;
        public int FontStyle
        {
            get { return _fontStyle; }
            set
            {
                _fontStyle = value;
                OnPropertyChanged(() => FontStyle);
            }
        }


        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged(() => Text);
            }
        }

        private Color _fontColor;
        public Color FontColor
        {
            get { return _fontColor; }
            set
            {
                _fontColor = value;
                OnPropertyChanged(() => FontColor);
            }
        }

        private bool _isBoxVisible;
        public bool IsBoxVisible
        {
            get { return _isBoxVisible; }
            set
            {
                _isBoxVisible = value;
                OnPropertyChanged(() => IsBoxVisible);
            }
        }


        
        public TextElementSettingsViewModel(TextElementModel model)
            : base(() => { })
        {
            _model = model;
            _fontSize = model.FontSize;
            _fontStyle = (int)model.FontStyle;
            _fontColor = model.FontColor;
            _isBoxVisible = model.IsBoxVisible;
            _text = model.Text;

            SaveCommand = new DelegateCommand(OnSave);
        }



        private void OnSave()
        {
            _model.FontSize = _fontSize;
            _model.FontStyle = (MyFontStyle)_fontStyle;
            _model.FontColor = _fontColor;
            _model.IsBoxVisible = _isBoxVisible;
            _model.Text = _text;

            DialogResult = true;
        }
        
        
    }
}
