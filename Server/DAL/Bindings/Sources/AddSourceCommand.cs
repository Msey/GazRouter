using GazRouter.DAL.Core;
using GazRouter.DTO.Bindings.Sources;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.Bindings.Sources 
{
    public class AddSourceCommand : CommandScalar<AddEditSourceParameterSet, int>
    {
        public AddSourceCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddEditSourceParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_source_id");
            command.AddInputParameter("p_source_name", parameters.SourceName);
            command.AddInputParameter("p_system_name", parameters.SystemName);
            command.AddInputParameter("p_is_hidden", parameters.IsHidden);
            command.AddInputParameter("p_is_readonly", parameters.IsReadonly);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }


        protected override string GetCommandText(AddEditSourceParameterSet parameters)
        {
            return "rd.P_SOURCE.AddF";

        }

    }
}