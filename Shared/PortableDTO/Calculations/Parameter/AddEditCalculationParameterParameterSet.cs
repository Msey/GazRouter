using System;
using GazRouter.DTO.Dictionaries.ParameterTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.Calculations.Parameter
{
    public class AddEditCalculationParameterParameterSet : BaseDto<int>
    {

        public string Alias { get; set; }
        public ParameterType? ParameterTypeId { get; set; }
        public PropertyType? PropertyTypeId { get; set; }
        public Guid EntityId { get; set; }
        public int CalculationId { get; set; }
        public int SortOrder { get; set; }
        public TimeShiftUnit? TimeShiftUnit { get; set; }
        public int TimeShiftValue { get; set; }
        public string Value { get; set; }
        public bool IsNumeric { get; set; }
    }
}