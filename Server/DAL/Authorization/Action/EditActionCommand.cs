using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;
namespace GazRouter.DAL.Authorization.Action
{
	public class EditActionCommand : CommandNonQuery<EditActionParameterSet>
	{
		public EditActionCommand(ExecutionContext context) : base(context)
		{
		    IsStoredProcedure = true;
		}
		
	    protected override void BindParameters(OracleCommand command, EditActionParameterSet parameters)
	    {
            command.AddInputParameter("p_action_id", parameters.Id);
            command.AddInputParameter("p_action_path", parameters.Path);
            command.AddInputParameter("p_app_host_name", Context.AppHostName);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_service_description", parameters.ServiceDescription);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
	    }
		
		protected override string GetCommandText(EditActionParameterSet parameters)
	    {
            return "P_ACTION.Edit";
	    }
	}
}