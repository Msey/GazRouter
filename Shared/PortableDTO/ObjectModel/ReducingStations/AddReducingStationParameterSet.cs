using System;

namespace GazRouter.DTO.ObjectModel.ReducingStations
{
    public class AddReducingStationParameterSet
    {
        public string Name { get; set; }
        public bool Status { get; set; }
        public int? SortOrder { get; set; }
        public bool Hidden { get; set; }
        public bool IsVirtual { get; set; }
        public Guid SiteId { get; set; }
        public Guid MainPipelineId { get; set; }
        public double Kilometer { get; set; }
        public Guid? Id { get; set; }
    }
}
