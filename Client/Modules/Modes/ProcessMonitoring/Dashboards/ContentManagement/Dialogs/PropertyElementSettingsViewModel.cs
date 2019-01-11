using System.Windows.Media;
using GazRouter.Common.ViewModel;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Dialogs
{
    public class PropertyElementSettingsViewModel : DialogViewModel
    {
        public DelegateCommand SaveCommand { get; private set; }
        private readonly PropertyElementModel _model;

        private string _comment;
        public string Comment
        {
            get { return _comment; }
            set
            {
                _comment = value;
                OnPropertyChanged(() => Comment);
            }
        }

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

        private bool _showTitle;
        public bool ShowTitle
        {
            get { return _showTitle; }
            set
            {
                _showTitle = value;
                OnPropertyChanged(() => ShowTitle);
            }
        }



        public PropertyElementSettingsViewModel(PropertyElementModel model)
            : base(() => { })
        {
            _model = model;

            _comment = model.Comment;
            _fontColor = model.FontColor;
            _fontSize = model.FontSize;
            _isBoxVisible = model.IsBoxVisible;
            _showTitle = model.ShowTitle;

            SaveCommand = new DelegateCommand(OnSave);
        }



        private void OnSave()
        {
            _model.Comment = _comment;
            _model.FontSize = _fontSize;
            _model.FontColor = _fontColor;
            _model.IsBoxVisible = _isBoxVisible;
            _model.ShowTitle = _showTitle;
            DialogResult = true;
        }
        
        
    }
}
