using System;

namespace GazRouter.DTO.ObjectModel.Sites
{
    public class GetSiteListParameterSet
    {
        public int? SystemId { get; set; } 
        public Guid? EnterpriseId { get; set; }
        public Guid? SiteId { get; set; }
    }
}