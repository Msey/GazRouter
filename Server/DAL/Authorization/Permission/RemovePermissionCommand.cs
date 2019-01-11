using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Authorization.Permission
{
    public class RemovePermissionCommand : CommandNonQuery<int>
    {
        public RemovePermissionCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }
        protected override void BindParameters(OracleCommand command, int parameters)
        {
            command.AddInputParameter("p_permission_id", parameters);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
        protected override string GetCommandText(int parameters)
        {
            return "P_PERMISSION.Remove";
        }
    }
}
