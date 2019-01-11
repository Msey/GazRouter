using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.Sources
{
    public class DeleteSourceCommand : CommandNonQuery<int>
    {
        public DeleteSourceCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, int id)
        {
            command.AddInputParameter("p_source_id", id);
            command.AddInputParameter("p_is_hidden", 1);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(int id)
        {
            return "P_SOURCE.Edit";
        }
    }
}