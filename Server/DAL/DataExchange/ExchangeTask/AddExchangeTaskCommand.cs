using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.ExchangeTask;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.ExchangeTask
{
    public class AddExchangeTaskCommand : CommandScalar<AddExchangeTaskParameterSet, int>
    {
        public AddExchangeTaskCommand(ExecutionContext context)
            : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddExchangeTaskParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<int>("p_exchange_task_id");
            command.AddInputParameter("p_name", parameters.Name);
            command.AddInputParameter("p_exchange_type_id", parameters.ExchangeTypeId);
            command.AddInputParameter("p_exchange_status", parameters.ExchangeStatus);
            command.AddInputParameter("p_exchange_lag", parameters.Lag);
            command.AddInputParameter("p_source_id", parameters.DataSourceId);
            command.AddInputParameter("p_period_type_id", parameters.PeriodTypeId);
            command.AddInputParameter("p_is_critical", parameters.IsCritical);
            command.AddInputParameter("p_file_name_mask", parameters.FileNameMask);
            command.AddInputParameter("p_is_plsql", parameters.IsSql);
            command.AddInputParameter("p_plsql_procedure", parameters.SqlProcedureName);
            command.AddInputParameter("p_is_transform", parameters.IsTransform);
            command.AddInputParameter("p_transformation", parameters.Transformation);
            command.AddInputParameter("p_transport_type_id", parameters.TransportTypeId);
            command.AddInputParameter("p_transport_address", parameters.TransportAddress);
            command.AddInputParameter("p_transport_login", parameters.TransportLogin);
            command.AddInputParameter("p_transport_password", parameters.TransportPassword);
            command.AddInputParameter("p_send_as_attachment", parameters.SendAsAttachment);
            command.AddInputParameter("p_secure_key", parameters.HostKey);
            command.AddInputParameter("p_exclude_hours", parameters.ExcludeHours); 

            command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }


        protected override string GetCommandText(AddExchangeTaskParameterSet parameters)
        {
            return "rd.P_EXCHANGE_TASK.AddF";

        }

    }
}