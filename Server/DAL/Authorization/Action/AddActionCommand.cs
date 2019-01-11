using GazRouter.DAL.Core;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Authorization.Action
{

	public class AddActionCommand : CommandScalar<AddActionParameterSet, int>
	{
		public AddActionCommand(ExecutionContext context) : base(context)
		{
		    IsStoredProcedure = true;
		}
		
	    protected override void BindParameters(OracleCommand command, AddActionParameterSet parameters)
	    {
            OutputParameter = command.AddReturnParameter<int>("action_id");
            command.AddInputParameter("p_action_path", parameters.Path);
            command.AddInputParameter("p_app_host_name", Context.AppHostName);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_service_description", parameters.ServiceDescription);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
	    }
		
		protected override string GetCommandText(AddActionParameterSet parameters)
	    {
            return "P_ACTION.AddF";
		}
	}

}

