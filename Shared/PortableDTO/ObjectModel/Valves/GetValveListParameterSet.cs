using System;

namespace GazRouter.DTO.ObjectModel.Valves
{
    public class GetValveListParameterSet
    {
        public Guid? PipelineId { get; set; }

        public Guid? SiteId { get; set; }

        public int? SystemId { get; set; }

        public Guid? ValveId { get; set; }

        public bool? IsControlPoint { get; set; }

    }
}
