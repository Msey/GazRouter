using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.Role;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.Role
{

    public class RemoveRoleActionCommand : CommandNonQuery<RoleActionParameterSet>
	{
        public RemoveRoleActionCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, RoleActionParameterSet parameters)
	    {
            command.AddInputParameter("p_action_id", parameters.ActionId);
            command.AddInputParameter("p_role_id", parameters.RoleId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(RoleActionParameterSet parameters)
	    {
            return "P_ACTION.Remove_ROLEACTION";
	    }
	}

}