using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.Calculations.Calculation
{
    public class EditCalculationParameterSet : AddCalculationParameterSet
    {
        public int CalculationId { get; set; }
        public string ExpressionOriginal { get; set; }
    }
}