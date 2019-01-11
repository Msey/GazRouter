using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.DTO.EntitySelector
{
    [DataContract]
    public class EntitiesPageDTO
    {
        [DataMember]
        public int TotalCount { get; set; }

        [DataMember]
        public Collection<CommonEntityWithSiteDTO> Entities { get; set; }

        [DataMember]
        public Guid Token { get; set; }
    }
}
