using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Balances.Docs
{
    public class DeleteDocCommand : CommandNonQuery<int>
    {
        public DeleteDocCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_doc_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(int parameters)
        {
            return "P_BL_DOC.Remove";
        }
    }
}
