using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.Calculations.Calculation
{
    public class AddCalculationParameterSet
    {
        //public int EventTypeId { get; set; }
        public PeriodType PeriodTypeId { get; set; }
        public int SortOrder { get; set; }
        public string Description { get; set; }
        public string SysName { get; set; }
        public string Expression { get; set; }
        public string Sql { get; set; }
        public CalculationStage CalcStage { get; set; }
    }
}