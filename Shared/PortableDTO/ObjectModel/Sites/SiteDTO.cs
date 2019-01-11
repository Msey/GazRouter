using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.ObjectModel.Sites
{
    [DataContract]
    public class SiteDTO : EntityDTO
	{
        public SiteDTO()
        {
            NeighbourSiteIdList = new List<Guid>();
            DependantSiteIdList = new List<Guid>();
        }

        public override EntityType EntityType
        {
            get { return EntityType.Site; }
        }

        [DataMember]
        public EntityStatus? Status { get; set; }

        [DataMember]
        public List<Guid> NeighbourSiteIdList { get; set; }

        [DataMember]
        public List<Guid> DependantSiteIdList { get; set; }

        [DataMember]
        public string BalanceGroupName { get; set; }
    }
}