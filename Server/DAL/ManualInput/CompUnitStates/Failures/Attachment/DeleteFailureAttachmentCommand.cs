using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitStates.Failures.Attachment
{
    public class DeleteFailureAttachmentCommand: CommandNonQuery<int>
    {
        public DeleteFailureAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
		{
            command.AddInputParameter("p_failure_act_id", parameters);
        }

        protected override string GetCommandText(int parameters)
        {
            return "rd.P_FAILURE_ACT.Remove";
        }
    }
}