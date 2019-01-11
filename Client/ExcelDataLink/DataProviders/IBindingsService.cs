using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using GazRouter.DTO.Bindings.EntityBindings;
using GazRouter.DTO.Bindings.EntityPropertyBindings;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.Bindings  
{
    [ServiceContract]
    public interface IBindingsService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetEntityBindingsList(GetEntityBindingsPageParameterSet parameters, AsyncCallback callback, object state);
        List<BindingDTO> EndGetEntityBindingsList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddEntityBindings(EntityBindingParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddEntityBindings(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditEntityBindings(EditEntityBindingParameterSet parameters, AsyncCallback callback, object state);
        void EndEditEntityBindings(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteEntityBindings(Guid parameters, AsyncCallback callback, object state);
        void EndDeleteEntityBindings(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetEntityPropertyBindingsList(GetEntityPropertyBindingsParameterSet parameters, AsyncCallback callback, object state);
        List<EntityPropertyBindingDTO> EndGetEntityPropertyBindingsList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddEntityPropertyBindings(AddEntityPropertyBindingParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddEntityPropertyBindings(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditEntityPropertyBindings(EditEntityPropertyBindingParameterSet parameters, AsyncCallback callback, object state);
        void EndEditEntityPropertyBindings(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteEntityPropertyBindings(Guid parameters, AsyncCallback callback, object state);
        void EndDeleteEntityPropertyBindings(IAsyncResult result);
    }


    public class BindingsServiceProxy : DataProviderBase<IBindingsService>
	{
        protected override string ServiceUri
        {
            get { return "/Bindings/BindingsService.svc"; }
        }

        public Task<List<BindingDTO>> GetEntityBindingsListAsync(GetEntityBindingsPageParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<BindingDTO>,GetEntityBindingsPageParameterSet>(channel, channel.BeginGetEntityBindingsList, channel.EndGetEntityBindingsList, parameters);
        }

        public Task<Guid> AddEntityBindingsAsync(EntityBindingParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,EntityBindingParameterSet>(channel, channel.BeginAddEntityBindings, channel.EndAddEntityBindings, parameters);
        }

        public Task EditEntityBindingsAsync(EditEntityBindingParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditEntityBindings, channel.EndEditEntityBindings, parameters);
        }

        public Task DeleteEntityBindingsAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteEntityBindings, channel.EndDeleteEntityBindings, parameters);
        }

        public Task<List<EntityPropertyBindingDTO>> GetEntityPropertyBindingsListAsync(GetEntityPropertyBindingsParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<EntityPropertyBindingDTO>,GetEntityPropertyBindingsParameterSet>(channel, channel.BeginGetEntityPropertyBindingsList, channel.EndGetEntityPropertyBindingsList, parameters);
        }

        public Task<Guid> AddEntityPropertyBindingsAsync(AddEntityPropertyBindingParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddEntityPropertyBindingParameterSet>(channel, channel.BeginAddEntityPropertyBindings, channel.EndAddEntityPropertyBindings, parameters);
        }

        public Task EditEntityPropertyBindingsAsync(EditEntityPropertyBindingParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditEntityPropertyBindings, channel.EndEditEntityPropertyBindings, parameters);
        }

        public Task DeleteEntityPropertyBindingsAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteEntityPropertyBindings, channel.EndDeleteEntityPropertyBindings, parameters);
        }

    }
}
