using System.Windows.Media;
using GazRouter.Common.ViewModel;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Dialogs
{
    public class ShapeElementSettingsViewModel : DialogViewModel
    {
        public DelegateCommand SaveCommand { get; private set; }
        private readonly ShapeElementModel _model;

        private double _strokeThickness;
        public double StrokeThickness
        {
            get { return _strokeThickness; }
            set
            {
                _strokeThickness = value;
                OnPropertyChanged(() => StrokeThickness);
            }
        }

        private Color _strokeColor;
        public Color StrokeColor
        {
            get { return _strokeColor; }
            set
            {
                _strokeColor = value;
                OnPropertyChanged(() => StrokeColor);
            }
        }

        private Color _fillColor;
        public Color FillColor
        {
            get { return _fillColor; }
            set
            {
                _fillColor = value;
                OnPropertyChanged(() => FillColor);
            }
        }


        private int _shapeType;
        public int ShapeType
        {
            get { return _shapeType; }
            set
            {
                _shapeType = value;
                OnPropertyChanged(() => ShapeType);
            }
        }

        private bool _isStrokeDotted;
        public bool IsStrokeDotted
        {
            get { return _isStrokeDotted; }
            set
            {
                _isStrokeDotted = value;
                OnPropertyChanged(() => IsStrokeDotted);
            }
        }

        private bool _isAdd;
        public string ButtonContent
        {
            get { return _isAdd? "Добавить": "Изменить"; }
        }

        private double _rotateAngle;
        public double RotateAngle
        {
            get { return _rotateAngle; }
            set
            {
                _rotateAngle = value;
                OnPropertyChanged(() => RotateAngle);
            }
        }


        public ShapeElementSettingsViewModel(ShapeElementModel model, bool IsAdd)
            : base(() => { })
        {
            this._isAdd = IsAdd;
            _model = model;
            _fillColor = model.FillColor;
            _isStrokeDotted = model.IsStrokeDotted;
            _rotateAngle = model.RotateAngle;
            _shapeType = (int)model.Type;
            _strokeColor = model.StrokeColor;
            _strokeThickness = model.StrokeThickness;

            SaveCommand = new DelegateCommand(OnSave);
        }



        private void OnSave()
        {
            _model.FillColor = _fillColor;
            _model.IsStrokeDotted = _isStrokeDotted;
            _model.RotateAngle = _rotateAngle;
            _model.Type = (ShapeType) _shapeType;
            _model.StrokeColor = _strokeColor;
            _model.StrokeThickness = _strokeThickness;
            
            DialogResult = true;
        }
        
        
    }
}
