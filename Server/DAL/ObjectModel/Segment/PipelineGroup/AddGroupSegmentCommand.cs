using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Segment;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Segment.PipelineGroup
{
    public class AddGroupSegmentCommand : CommandScalar<AddGroupSegmentParameterSet, int>
    {
        public AddGroupSegmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddGroupSegmentParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_entity_id");
            command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            command.AddInputParameter("p_pipeline_group_id", parameters.PipelineGroupId);
            command.AddInputParameter("p_kilometer_start", parameters.KilometerOfStartPoint);
            command.AddInputParameter("p_kilometer_end", parameters.KilometerOfEndPoint);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(AddGroupSegmentParameterSet parameters)
        {
            return "rd.P_SEGMENT_BY_GROUP.AddF";
        }
    }
}
