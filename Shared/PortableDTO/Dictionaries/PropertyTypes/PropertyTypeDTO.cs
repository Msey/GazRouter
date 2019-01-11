using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PhisicalTypes;


namespace GazRouter.DTO.Dictionaries.PropertyTypes
{
    [DataContract]
    [KnownType(typeof(PropertyTypeDictDTO))]
    public class PropertyTypeDTO : BaseDictionaryDTO
    {
        [DataMember]
        public PhysicalType PhysicalTypeId { get; set; }

        [DataMember]
        public PhysicalTypeDTO PhysicalType { get; set; }

        [DataMember]
        public string SysName { get; set; }

        [DataMember]
        public string ShortName { get; set; }
        
        [DataMember]
        public string Description { get; set; }


        public PropertyType PropertyType => (PropertyType)Id;
    }
}