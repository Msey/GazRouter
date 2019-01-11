using System;
using GazRouter.Common.ViewModel;
namespace GazRouter.Common.Ui.Templates
{
    public enum TemplateType
    {
        Default = 0,
        Formula2,
        Formula3,
    }
    public class InputData : PropertyChangedBase
    {
        public InputData(string parameterName,
                 string designation,
                 string unit,
                 TemplateType template = TemplateType.Default,
                 double inputValue = 0)
        {
            ParameterName = parameterName;
            Designation = designation;
            Unit = unit;
            _inputValue = inputValue;
            Template = template;
        }
        public string ParameterName { get; private set; }
        public string Designation { get; private set; }
        public string Unit { get; private set; }
        private double _inputValue;
        public double InputValue
        {
            get { return _inputValue; }
            set
            {
//                if (value <= 0) throw new Exception("asdasd");
                _inputValue = value;
                OnPropertyChanged(() => InputValue);
            }
        }
        public TemplateType Template { get; set; }

    }
}
