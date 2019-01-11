using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.DTO.Dictionaries.Enterprises
{
    [DataContract]
    public class EnterpriseDTO : EntityDTO
    {
        public override EntityType EntityType => EntityType.Enterprise; 

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public bool? IsGr { get; set; }

        [DataMember]
        public string AsduCode { get; set; }
    }
}