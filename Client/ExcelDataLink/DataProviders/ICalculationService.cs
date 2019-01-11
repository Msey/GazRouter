using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Log;
using GazRouter.DTO.Calculations.Parameter;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.Calculations  
{
    [ServiceContract]
    public interface ICalculationService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCalculationList(GetCalculationListParameterSet parameters, AsyncCallback callback, object state);
        List<CalculationDTO> EndGetCalculationList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCalculationListById(int parameters, AsyncCallback callback, object state);
        CalculationDTO EndGetCalculationListById(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddCalculation(AddCalculationParameterSet parameters, AsyncCallback callback, object state);
        int EndAddCalculation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditCalculation(EditCalculationParameterSet parameters, AsyncCallback callback, object state);
        void EndEditCalculation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteCalculation(int parameters, AsyncCallback callback, object state);
        void EndDeleteCalculation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddCalculationParameter(AddEditCalculationParParameterSet parameters, AsyncCallback callback, object state);
        int EndAddCalculationParameter(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditCalculationParameter(AddEditCalculationParParameterSet parameters, AsyncCallback callback, object state);
        void EndEditCalculationParameter(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteCalculationParameter(int parameters, AsyncCallback callback, object state);
        void EndDeleteCalculationParameter(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCalculationParameter(GetCalculationParParameterSet parameters, AsyncCallback callback, object state);
        int EndGetCalculationParameter(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCalculationParameterById(int parameters, AsyncCallback callback, object state);
        List<CalculationParameterDTO> EndGetCalculationParameterById(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTestExecute(GetCalcExecuteSqlParameterSet parameters, AsyncCallback callback, object state);
        CalcExecOutput EndTestExecute(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRunCalc(RunCalcParameterSet parameters, AsyncCallback callback, object state);
        List<SerializableTuple<DateTime, string>> EndRunCalc(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetLogs(GetLogListParameterSet parameters, AsyncCallback callback, object state);
        List<LogCalculationDTO> EndGetLogs(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCalculationListByVar(GetCalculationListByVarParameterSet parameters, AsyncCallback callback, object state);
        List<CalculationsByParameterDTO> EndGetCalculationListByVar(IAsyncResult result);
    }


    public class CalculationServiceProxy : DataProviderBase<ICalculationService>
	{
        protected override string ServiceUri
        {
            get { return "/Calculations/CalculationService.svc"; }
        }

        public Task<List<CalculationDTO>> GetCalculationListAsync(GetCalculationListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<CalculationDTO>,GetCalculationListParameterSet>(channel, channel.BeginGetCalculationList, channel.EndGetCalculationList, parameters);
        }

        public Task<CalculationDTO> GetCalculationListByIdAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<CalculationDTO,int>(channel, channel.BeginGetCalculationListById, channel.EndGetCalculationListById, parameters);
        }

        public Task<int> AddCalculationAsync(AddCalculationParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddCalculationParameterSet>(channel, channel.BeginAddCalculation, channel.EndAddCalculation, parameters);
        }

        public Task EditCalculationAsync(EditCalculationParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditCalculation, channel.EndEditCalculation, parameters);
        }

        public Task DeleteCalculationAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteCalculation, channel.EndDeleteCalculation, parameters);
        }

        public Task<int> AddCalculationParameterAsync(AddEditCalculationParParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddEditCalculationParParameterSet>(channel, channel.BeginAddCalculationParameter, channel.EndAddCalculationParameter, parameters);
        }

        public Task EditCalculationParameterAsync(AddEditCalculationParParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditCalculationParameter, channel.EndEditCalculationParameter, parameters);
        }

        public Task DeleteCalculationParameterAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteCalculationParameter, channel.EndDeleteCalculationParameter, parameters);
        }

        public Task<int> GetCalculationParameterAsync(GetCalculationParParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,GetCalculationParParameterSet>(channel, channel.BeginGetCalculationParameter, channel.EndGetCalculationParameter, parameters);
        }

        public Task<List<CalculationParameterDTO>> GetCalculationParameterByIdAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<CalculationParameterDTO>,int>(channel, channel.BeginGetCalculationParameterById, channel.EndGetCalculationParameterById, parameters);
        }

        public Task<CalcExecOutput> TestExecuteAsync(GetCalcExecuteSqlParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<CalcExecOutput,GetCalcExecuteSqlParameterSet>(channel, channel.BeginTestExecute, channel.EndTestExecute, parameters);
        }

        public Task<List<SerializableTuple<DateTime, string>>> RunCalcAsync(RunCalcParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<SerializableTuple<DateTime, string>>,RunCalcParameterSet>(channel, channel.BeginRunCalc, channel.EndRunCalc, parameters);
        }

        public Task<List<LogCalculationDTO>> GetLogsAsync(GetLogListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<LogCalculationDTO>,GetLogListParameterSet>(channel, channel.BeginGetLogs, channel.EndGetLogs, parameters);
        }

        public Task<List<CalculationsByParameterDTO>> GetCalculationListByVarAsync(GetCalculationListByVarParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<CalculationsByParameterDTO>,GetCalculationListByVarParameterSet>(channel, channel.BeginGetCalculationListByVar, channel.EndGetCalculationListByVar, parameters);
        }

    }
}
