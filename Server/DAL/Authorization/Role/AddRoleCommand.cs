using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.Role;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.Role
{

	public class AddRoleCommand : CommandScalar<AddRoleParameterSet, int>
	{
		public AddRoleCommand(ExecutionContext context) : base(context)
		{
		    IsStoredProcedure = true;
			IntegrityConstraints.Add("ORA-20104", "Роль с таким именем уже существует");
		}
		
	    protected override void BindParameters(OracleCommand command, AddRoleParameterSet parameters)
	    {
            OutputParameter = command.AddReturnParameter<int>("role_id");
            command.AddInputParameter("p_sys_name", parameters.Name);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
	    }
		
		protected override string GetCommandText(AddRoleParameterSet parameters)
	    {
            return "P_ROLE.AddF";
		}
	}

}

