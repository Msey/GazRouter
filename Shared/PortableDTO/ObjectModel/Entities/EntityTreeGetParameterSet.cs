using System;

namespace GazRouter.DTO.ObjectModel.Entities
{
    public class EntityTreeGetParameterSet
    {
        public bool ShowHidden { get; set; }
        public bool ShowDeleted { get; set; }
        public EntityFilter Filter { get; set; }
        public int? SystemId { get; set; }

        public Guid? SiteId { get; set; }
    }

}