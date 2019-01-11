using System;
using GazRouter.DTO.Dictionaries.EntityTypes;


namespace GazRouter.DTO.SeriesData.EntityValidationStatus
{
    public class GetEntityValidationStatusListParameterSet
    {
        public Guid? SiteId { get; set; }

        public int? SerieId { get; set; }

        public EntityType? EntityType { get; set; }

    }
}
