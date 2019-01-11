using GazRouter.DAL.Core;
using GazRouter.DTO.ObjectModel.Segment;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Segment.PipelineGroup
{
    public class EditGroupSegmentCommand : CommandNonQuery<EditGroupSegmentParameterSet>
    {
        public EditGroupSegmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, EditGroupSegmentParameterSet parameters)
        {
            command.AddInputParameter("p_segments_by_group_id", parameters.SegmentId);
            command.AddInputParameter("p_pipeline_id", parameters.PipelineId);
            command.AddInputParameter("p_kilometer_start", parameters.KilometerOfStartPoint);
            command.AddInputParameter("p_kilometer_end", parameters.KilometerOfEndPoint);
            command.AddInputParameter("p_pipeline_group_id", parameters.PipelineGroupId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(EditGroupSegmentParameterSet parameters)
        {
            return "rd.P_SEGMENT_BY_GROUP.Edit";
        }
    }
}
