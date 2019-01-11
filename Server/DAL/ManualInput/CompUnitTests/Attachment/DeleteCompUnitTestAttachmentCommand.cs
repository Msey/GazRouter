using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.ManualInput.CompUnitTests.Attachment
{
    public class DeleteCompUnitTestAttachmentCommand : CommandNonQuery<int>
    {
        public DeleteCompUnitTestAttachmentCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_comp_unit_test_att_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }


        protected override string GetCommandText(int parameters)
        {
            return "rd.P_COMP_UNITS_TEST.Remove_ATT";
        }
    }
}
