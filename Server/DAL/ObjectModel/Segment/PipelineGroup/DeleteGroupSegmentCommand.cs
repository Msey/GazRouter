using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Segment.PipelineGroup
{
    public class DeleteGroupSegmentCommand : CommandNonQuery<int>
    {
        public DeleteGroupSegmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_segments_by_group_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }


        protected override string GetCommandText(int parameters)
        {
            return "rd.P_SEGMENT_BY_GROUP.Remove";
        }
    }
}
