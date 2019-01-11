using System;

namespace GazRouter.DTO.ManualInput.DependantSites
{
    public class AddRemoveDependantSiteParameterSet
    {
        public Guid SiteId { get; set; }
        public Guid DependantSiteId { get; set; }
    }
}