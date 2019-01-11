using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.ObjectModel
{
    [DataContract]
    public class CommonEntityWithSiteDTO : CommonEntityDTO
    {
        [DataMember]
        public Guid? SiteId { get; set; }

        [DataMember]
        public string SiteName { get; set; }
    }
}