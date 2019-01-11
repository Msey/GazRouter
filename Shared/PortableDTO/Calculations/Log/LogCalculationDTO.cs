using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.Calculations.Log
{
    public class LogCalculationDTO : BaseDto<Guid>
    {
        public string CalculationSysName { get; set; }
        public string CalculationDescription { get; set; }
        public string ErrorNumber { get; set; }
        public DateTime? KeyDate { get; set; }
        public string ErrorMessage { get; set; }
        public PeriodType PerionTypeId { get; set; }
        public int SortOrder { get; set; }
        //public string EventTypeName { get; set; }

        public int CalculationId { get; set; }
        public DateTime ErrorDate { get; set; }

        public PeriodType? PeriodTypeId { get; set; }

        public string SrcCode { get; set; }
    }
}