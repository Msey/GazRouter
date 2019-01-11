using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.PipelineTypes;

namespace GazRouter.DTO.ObjectModel.Pipelines
{
    public class GetPipelineListParameterSet
    {
        public int? SystemId { get; set; }
        public Guid? SiteId { get; set; }
        public List<PipelineType> PipelineTypes { get; set; }
    }
}