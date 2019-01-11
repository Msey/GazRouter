using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.SystemVariables;
      
// ReSharper disable once CheckNamespace
namespace GazRouter.DataProviders.SystemVariables  
{
    [ServiceContract]
    public interface ISysVarService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetIusVariableList(object parameters, AsyncCallback callback, object state);
        List<IusVariableDTO> EndGetIusVariableList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditIusVariableValue(IusVariableParameterSet newValue, AsyncCallback callback, object state);
        void EndEditIusVariableValue(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetHelpFileName(object parameters, AsyncCallback callback, object state);
        string EndGetHelpFileName(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetLastChangesFileName(object parameters, AsyncCallback callback, object state);
        string EndGetLastChangesFileName(IAsyncResult result);
    }

	public interface ISysVarServiceProxy
	{

        Task<List<IusVariableDTO>> GetIusVariableListAsync();

        Task EditIusVariableValueAsync(IusVariableParameterSet newValue);

        Task<string> GetHelpFileNameAsync();

        Task<string> GetLastChangesFileNameAsync();

    }

    public sealed class SysVarServiceProxy : DataProviderBase<ISysVarService>, ISysVarServiceProxy
	{
        protected override string ServiceUri => "/SystemVariables/SysVarService.svc";
      


        public Task<List<IusVariableDTO>> GetIusVariableListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<IusVariableDTO>>(channel, channel.BeginGetIusVariableList, channel.EndGetIusVariableList);
        }

        public Task EditIusVariableValueAsync(IusVariableParameterSet newValue)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditIusVariableValue, channel.EndEditIusVariableValue, newValue);
        }

        public Task<string> GetHelpFileNameAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<string>(channel, channel.BeginGetHelpFileName, channel.EndGetHelpFileName);
        }

        public Task<string> GetLastChangesFileNameAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<string>(channel, channel.BeginGetLastChangesFileName, channel.EndGetLastChangesFileName);
        }

    }
}
