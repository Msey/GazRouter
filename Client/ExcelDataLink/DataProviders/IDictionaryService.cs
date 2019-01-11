using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
      
// ReSharper disable once CheckNamespace
namespace DataProviders.Dictionaries  
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
        IAsyncResult BeginGetDictionaryRepository(object parameters, AsyncCallback callback, object state);
        DictionaryRepositoryDTO EndGetDictionaryRepository(IAsyncResult result);
    }


    public class DictionaryServiceProxy : DataProviderBase<IDictionaryService>
	{
        protected override string ServiceUri
        {
            get { return "/Dictionaries/DictionaryService.svc"; }
        }

        public Task<DictionaryRepositoryDTO> GetDictionaryRepositoryAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<DictionaryRepositoryDTO>(channel, channel.BeginGetDictionaryRepository, channel.EndGetDictionaryRepository);
        }

    }
}
