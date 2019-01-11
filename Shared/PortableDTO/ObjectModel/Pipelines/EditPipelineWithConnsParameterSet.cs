using GazRouter.DTO.ObjectModel.PipelineConns;

namespace GazRouter.DTO.ObjectModel.Pipelines
{
    public class EditPipelineWithConnsParameterSet
    {
        public EditPipelineParameterSet PipelineParameters { get; set; }

        public AddPipelineConnParameterSet StartConnParameters { get; set; }

        public AddPipelineConnParameterSet EndConnParameters { get; set; }
    }
}
