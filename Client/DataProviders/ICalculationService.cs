using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Calculation;
using GazRouter.DTO.Calculations.Log;
using GazRouter.DTO.Calculations.Parameter;
      
// ReSharper disable once CheckNamespace
namespace GazRouter.DataProviders.Calculations  
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
        IAsyncResult BeginAddCalculationParameter(AddEditCalculationParameterParameterSet parameters, AsyncCallback callback, object state);
        int EndAddCalculationParameter(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditCalculationParameter(AddEditCalculationParameterParameterSet parameters, AsyncCallback callback, object state);
        void EndEditCalculationParameter(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteCalculationParameter(int parameters, AsyncCallback callback, object state);
        void EndDeleteCalculationParameter(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCalculationParameter(GetCalculationParameterParameterSet parameters, AsyncCallback callback, object state);
        int EndGetCalculationParameter(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCalculationParameterById(int parameters, AsyncCallback callback, object state);
        List<CalculationParameterDTO> EndGetCalculationParameterById(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginTestExecute(TestCalculationParameterSet parameters, AsyncCallback callback, object state);
        TestCalcResultDTO EndTestExecute(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRunCalc(RunCalcParameterSet parameters, AsyncCallback callback, object state);
        List<SerializableTuple<DateTime, string>> EndRunCalc(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetLogs(GetLogListParameterSet parameters, AsyncCallback callback, object state);
        List<LogCalculationDTO> EndGetLogs(IAsyncResult result);
    }

	public interface ICalculationServiceProxy
	{

        Task<List<CalculationDTO>> GetCalculationListAsync(GetCalculationListParameterSet parameters);

        Task<int> AddCalculationAsync(AddCalculationParameterSet parameters);

        Task EditCalculationAsync(EditCalculationParameterSet parameters);

        Task DeleteCalculationAsync(int parameters);

        Task<int> AddCalculationParameterAsync(AddEditCalculationParameterParameterSet parameters);

        Task EditCalculationParameterAsync(AddEditCalculationParameterParameterSet parameters);

        Task DeleteCalculationParameterAsync(int parameters);

        Task<int> GetCalculationParameterAsync(GetCalculationParameterParameterSet parameters);

        Task<List<CalculationParameterDTO>> GetCalculationParameterByIdAsync(int parameters);

        Task<TestCalcResultDTO> TestExecuteAsync(TestCalculationParameterSet parameters);

        Task<List<SerializableTuple<DateTime, string>>> RunCalcAsync(RunCalcParameterSet parameters);

        Task<List<LogCalculationDTO>> GetLogsAsync(GetLogListParameterSet parameters);

    }

    public sealed class CalculationServiceProxy : DataProviderBase<ICalculationService>, ICalculationServiceProxy
	{
        protected override string ServiceUri => "/Calculations/CalculationService.svc";
      


        public Task<List<CalculationDTO>> GetCalculationListAsync(GetCalculationListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<CalculationDTO>,GetCalculationListParameterSet>(channel, channel.BeginGetCalculationList, channel.EndGetCalculationList, parameters);
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

        public Task<int> AddCalculationParameterAsync(AddEditCalculationParameterParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddEditCalculationParameterParameterSet>(channel, channel.BeginAddCalculationParameter, channel.EndAddCalculationParameter, parameters);
        }

        public Task EditCalculationParameterAsync(AddEditCalculationParameterParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditCalculationParameter, channel.EndEditCalculationParameter, parameters);
        }

        public Task DeleteCalculationParameterAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteCalculationParameter, channel.EndDeleteCalculationParameter, parameters);
        }

        public Task<int> GetCalculationParameterAsync(GetCalculationParameterParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,GetCalculationParameterParameterSet>(channel, channel.BeginGetCalculationParameter, channel.EndGetCalculationParameter, parameters);
        }

        public Task<List<CalculationParameterDTO>> GetCalculationParameterByIdAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<CalculationParameterDTO>,int>(channel, channel.BeginGetCalculationParameterById, channel.EndGetCalculationParameterById, parameters);
        }

        public Task<TestCalcResultDTO> TestExecuteAsync(TestCalculationParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<TestCalcResultDTO,TestCalculationParameterSet>(channel, channel.BeginTestExecute, channel.EndTestExecute, parameters);
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

    }
}
