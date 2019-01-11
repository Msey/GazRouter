using System;

namespace GazRouter.DTO.ObjectModel.ReducingStations
{
    public class GetReducingStationListParameterSet
    {
        public Guid? Id { get; set; }

        public Guid? SiteId { get; set; }

        public int? SystemId { get; set; }

        public Guid? PipelineId { get; set; }
    }
}