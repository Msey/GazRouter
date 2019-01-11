using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GazRouter.DTO.DataExchange.Asdu
{
    [DataContract]
    public class AsduPropertyDTO
    {
        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public Guid? ParameterGid { get; set; }

        [DataMember]
        public PropertyType PropertyTypeId { get; set; }

        [DataMember]
        public string UnitName { get; set; }

        [DataMember]
        public Guid? EntityGid { get; set; }
    }
}