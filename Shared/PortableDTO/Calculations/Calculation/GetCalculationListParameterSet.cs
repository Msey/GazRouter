using System;
using GazRouter.DTO.Dictionaries.ParameterTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.Calculations.Calculation
{
    public class GetCalculationListParameterSet
    {
        public PeriodType? PeriodType { get; set; }

        public int? CalculationId { get; set; }


        public Guid? EntityId { get; set; }

        public PropertyType? PropertyType { get; set; }

        public ParameterType? ParameterType { get; set; }
    }
}