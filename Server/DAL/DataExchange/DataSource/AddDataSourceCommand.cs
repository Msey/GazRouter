using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.DataSource;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.DataSource
{
    public class AddDataSourceCommand : CommandScalar<AddDataSourceParameterSet, int>
    {
        public AddDataSourceCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddDataSourceParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_source_id");
            command.AddInputParameter("p_source_name", parameters.Name);
            command.AddInputParameter("p_system_name", parameters.SysName);
            command.AddInputParameter("p_is_hidden", false);
            command.AddInputParameter("p_is_readonly", false);
            command.AddInputParameter("p_description", parameters.Description);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }


        protected override string GetCommandText(AddDataSourceParameterSet parameters)
        {
            return "rd.P_SOURCE.AddF";

        }

    }
}