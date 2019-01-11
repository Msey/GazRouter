using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.DataStorage;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.ExchangeTask
{
    public class RunProcCommand : CommandScalar<RunProcParameterSet, string>
    {
        public RunProcCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, RunProcParameterSet parameters)
        {
            OutputParameter = command.AddReturnParameter<string>("p_clob", OracleDbType.Clob);
            //command.AddOutputParameter<string>("p_clob");
            command.AddInputParameter("p_exchange_task_id", parameters.TaskId);
            command.AddInputParameter("p_series_date", parameters.TimeStamp);
            //command.AddInputParameter("p_user_name", Context.UserIdentifier);
        }


        protected override string GetCommandText(RunProcParameterSet parameters)
        {
            return parameters.ProcedureName;
        }
    }
}