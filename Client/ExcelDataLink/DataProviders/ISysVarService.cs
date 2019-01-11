using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.SystemVariables;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.SystemVariables  
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
    }


    public class SysVarServiceProxy : DataProviderBase<ISysVarService>
	{
        protected override string ServiceUri
        {
            get { return "/SystemVariables/SysVarService.svc"; }
        }

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

    }
}
