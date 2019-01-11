using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ObjectModel.Segment.Diameter
{
    public class DeleteDiameterSegmentCommand : CommandNonQuery<int>
    {
		public DeleteDiameterSegmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("P_SEGMENTS_BY_DIAMETR_ID", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }


        protected override string GetCommandText(int parameters)
        {
            return "rd.P_SEGMENT_BY_DIAMETR.Remove";
        }
    }
}
