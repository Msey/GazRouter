using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.Role;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.Role
{

	public class EditRoleCommand : CommandNonQuery<EditRoleParameterSet>
	{
		public EditRoleCommand(ExecutionContext context) : base(context)
		{
		    IsStoredProcedure = true;
            IntegrityConstraints.Add("ORA-20104", "Роль с таким именем уже существует"); 
		}
		
	    protected override void BindParameters(OracleCommand command, EditRoleParameterSet parameters)
	    {
            command.AddInputParameter("p_role_id", parameters.Id);
            command.AddInputParameter("p_sys_name", parameters.Name);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
		
		protected override string GetCommandText(EditRoleParameterSet parameters)
	    {
            return "P_ROLE.Edit";
	    }
	}

}