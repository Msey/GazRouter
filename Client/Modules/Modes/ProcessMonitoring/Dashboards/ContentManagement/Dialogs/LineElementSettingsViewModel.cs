using System.Windows.Media;
using GazRouter.Common.ViewModel;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Dialogs
{
    public class LineElementSettingsViewModel : DialogViewModel
    {
        public DelegateCommand SaveCommand { get; private set; }
        private readonly LineElementModel _model;


        private int _thickness;
        public int Thickness
        {
            get { return _thickness; }
            set
            {
                _thickness = value;
                OnPropertyChanged(()=>Thickness);
            }
        }

        private Color _color;
        public Color Color
        {
            get { return _color; }
            set
            {
                _color = value;
                OnPropertyChanged(() => Color);
            }
        }

        private bool _isDotted;
        public bool IsDotted
        {
            get { return _isDotted; }
            set
            {
                _isDotted = value;
                OnPropertyChanged(() => IsDotted);
            }
        }


        private bool _isJoinRounded;
        public bool IsJoinRounded
        {
            get { return _isJoinRounded; }
            set
            {
                _isJoinRounded = value;
                OnPropertyChanged(() => IsJoinRounded);
            }
        }


        public LineElementSettingsViewModel(LineElementModel model)
            : base(() => { })
        {
            _model = model;

            _color = model.Color;
            _isDotted = model.IsDotted;
            _isJoinRounded = model.IsJoinRounded;
            _thickness = model.Thickness;

            SaveCommand = new DelegateCommand(OnSave);
        }



        private void OnSave()
        {
            _model.Color = _color;
            _model.IsDotted = _isDotted;
            _model.IsJoinRounded = _isJoinRounded;
            _model.Thickness = _thickness;

            DialogResult = true;
        }
        
        
    }
}
