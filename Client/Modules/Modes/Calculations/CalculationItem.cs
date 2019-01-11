using System.Text.RegularExpressions;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Calculation;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.Modes.Calculations
{
    public class CalculationItem : PropertyChangedBase
    {
        private string _expressionOriginal;

        public CalculationItem(CalculationDTO dto)
        {
            Dto = dto;
            ExpressionBackup = dto.Expression;
            ExpressionOriginal = dto.Expression;
        }

        public CalculationDTO Dto { get; set; }

        

        public bool IsExpressionChanged => (ExpressionOriginal ?? string.Empty) != (ExpressionBackup ?? string.Empty);

        public string ExpressionBackup { get; set; }

        public string ExpressionOriginal
        {
            get { return _expressionOriginal; }
            set
            {
                if (value != _expressionOriginal)
                {
                    _expressionOriginal = value ?? string.Empty;
                    var input =
                        _expressionOriginal.Replace(@"\r\n", string.Empty)
                            .Replace(@"\n", string.Empty)
                            .Replace(@"\r", string.Empty);
                    Dto.Expression = Regex.Replace(input, @"(/\*(.*?)\*/)", string.Empty);
                    OnPropertyChanged(() => ExpressionOriginal);
                }
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Dto != null ? Dto.GetHashCode() : 0)*397) ^
                       (ExpressionBackup != null ? ExpressionBackup.GetHashCode() : 0);
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((CalculationItem) obj);
        }

        protected bool Equals(CalculationItem other)
        {
            return Equals(Dto, other.Dto) && string.Equals(ExpressionBackup, other.ExpressionBackup);
        }
    }
}