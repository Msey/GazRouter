using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.ObjectModel.Entities.Urls
{
    [DataContract]
    public class EntityUrlDTO
    {
        [DataMember]
        public int UrlId { get; set; }

        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Url { get; set; }
    }
}
