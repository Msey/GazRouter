using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Authorization.Permission
{
    public class RemovePermissionsCommand : CommandNonQuery
    {
        public RemovePermissionsCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }
        protected override void BindParameters(OracleCommand command)
        {
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
        protected override string GetCommandText()
        {
            return "P_PERMISSION.Remove";
        }
    }
}