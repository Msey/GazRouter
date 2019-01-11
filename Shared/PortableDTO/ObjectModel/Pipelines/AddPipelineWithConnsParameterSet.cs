using GazRouter.DTO.ObjectModel.PipelineConns;

namespace GazRouter.DTO.ObjectModel.Pipelines
{
    public class AddPipelineWithConnsParameterSet
    {
        public AddPipelineParameterSet PipelineParameters { get; set; }

        public AddPipelineConnParameterSet StartConnParameters { get; set; }

        public AddPipelineConnParameterSet EndConnParameters { get; set; }
    }
}
