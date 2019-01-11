using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.ParameterTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.Calculations.Parameter
{
    [DataContract]
    public class CalculationParameterDTO : BaseDto<int>
    {
        [DataMember]
        public string Alias { get; set; }

        [DataMember]
        public ParameterType ParameterTypeId { get; set; }

        [DataMember]
        public PropertyType PropertyTypeId { get; set; }

        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public int CalculationId { get; set; }

        
        [DataMember]
        public string TimeShiftUnit { get; set; }

        [DataMember]
        public int TimeShiftValue { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public bool IsNumeric { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public EntityType EntityTypeId { get; set; }

        [DataMember]
        public int UseCount { get; set; }
    }

}