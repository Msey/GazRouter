using System;
using System.Runtime.Serialization;
namespace GazRouter.DTO.GasCosts
{
    public class GasCostVisibilityDTO
    {
        [DataMember]
        public Guid SiteId { get; set; }
        [DataMember]
        public int CostType { get; set; }
        [DataMember]
        public int? Visibility { get; set; }
    }
}
