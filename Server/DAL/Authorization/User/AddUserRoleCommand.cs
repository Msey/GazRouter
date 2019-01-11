using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.User;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.User
{

	public class AddUserRoleCommand : CommandNonQuery<UserRoleParameterSet>
	{
        public AddUserRoleCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, UserRoleParameterSet parameters)
	    {
            command.AddInputParameter("p_role_id", parameters.RoleId);
            command.AddInputParameter("p_user_id", parameters.UserId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }

        protected override string GetCommandText(UserRoleParameterSet parameters)
	    {
            return @"P_ROLE.Add_USERROLE";
	    }
	}
}