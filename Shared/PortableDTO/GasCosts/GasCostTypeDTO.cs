using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
namespace GazRouter.DTO.GasCosts
{
    public class GasCostTypeDTO
    {
        [DataMember]
        public CostType CostType { get; set; }

        [DataMember]
        public string CostTypeName { get; set; }

        [DataMember]
        public EntityType EntityType { get; set; }

        [DataMember]
        public int GroupId  { get; set; }
        
        [DataMember]
        public int TubNum { get; set; }

        [DataMember]
        public int? IsRegular { get; set; }
    }
}