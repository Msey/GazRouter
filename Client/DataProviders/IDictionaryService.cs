using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
      
// ReSharper disable once CheckNamespace
namespace GazRouter.DataProviders.Dictionaries  
{
    [ServiceContract]
    public interface IDictionaryService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        [ServiceKnownType(typeof(PropertyTypeDictDTO))] 
        [ServiceKnownType(typeof(StateSetDTO<StateDTO<ValveState>>))] 
        [ServiceKnownType(typeof(StateSetDTO<StateDTO<CompUnitState>>))] 
        [ServiceKnownType(typeof(StateSetDTO<StateDTO<CompShopState>>))] 
        IAsyncResult BeginGetDictionaryRepository(bool force, AsyncCallback callback, object state);
        DictionaryRepositoryDTO EndGetDictionaryRepository(IAsyncResult result);
    }

	public interface IDictionaryServiceProxy
	{

        Task<DictionaryRepositoryDTO> GetDictionaryRepositoryAsync(bool force);

    }

    public sealed class DictionaryServiceProxy : DataProviderBase<IDictionaryService>, IDictionaryServiceProxy
	{
        protected override string ServiceUri => "/Dictionaries/DictionaryService.svc";
      


        public Task<DictionaryRepositoryDTO> GetDictionaryRepositoryAsync(bool force)
        {
            var channel = GetChannel();
            return ExecuteAsync<DictionaryRepositoryDTO,bool>(channel, channel.BeginGetDictionaryRepository, channel.EndGetDictionaryRepository, force);
        }

    }
}
