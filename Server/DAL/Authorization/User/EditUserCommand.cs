using GazRouter.DAL.Core;
using GazRouter.DTO.Authorization.User;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.User
{

	public class EditUserCommand : CommandNonQuery<EditUserParameterSet>
	{
		public EditUserCommand(ExecutionContext context) : base(context)
		{
		    IsStoredProcedure = true;
			IntegrityConstraints.Add("ORA-20104", "Пользователь с таким именем и/или логином уже существует");
		}
		
	    protected override void BindParameters(OracleCommand command, EditUserParameterSet parameters)
	    {
            command.AddInputParameter("p_user_id", parameters.Id);
            command.AddInputParameter("P_name", parameters.UserName);
            command.AddInputParameter("p_phone", parameters.Phone);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_SITE_ID", parameters.SiteId);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }
		
		protected override string GetCommandText(EditUserParameterSet parameters)
	    {
            return "P_USER.Edit";
	    }
	}

}