using System;
using GazRouter.DAL.Core;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeLog;
using Oracle.ManagedDataAccess.Client;

namespace GazRouter.DAL.DataExchange.ExchangeLog
{
    public class AddExchangeLogCommand : CommandScalar<AddEditExchangeLogParameterSet, DateTime?>
    {
        public AddExchangeLogCommand(ExecutionContext context) : base(context)
        {
            IsStoredProcedure = true;
        }

        protected override void BindParameters(OracleCommand command, AddEditExchangeLogParameterSet parameters)
        {
            command.AddInputParameter("p_exchange_task_id", parameters.ExchangeTaskId);
            command.AddInputParameter("p_series_id", parameters.SeriesId);
            command.AddInputParameter("p_is_ok", parameters.IsOk);
            command.AddInputParameter("p_data_content", GetBytes(parameters.Content));
            command.AddInputParameter("p_processing_error", parameters.Error);
            command.AddInputParameter("p_user_name", Context.UserIdentifier);
            OutputParameter = command.AddReturnParameter<DateTime?>("start_time");
        }

        protected override string GetCommandText(AddEditExchangeLogParameterSet parameters)
        {
            return "rd.P_EXCHANGE_LOG.AddF";
        }


        static byte[] GetBytes(string str)
        {
            var bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}