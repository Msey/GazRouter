using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.StatesModel;

namespace GazRouter.DataServices.Dictionaries
{
    [Service("Неизменяемые справочники")]
    [ServiceContract]
    public interface IDictionaryService
    {
        [ServiceAction("Получение репозитория справочников", true)]
        [OperationContract]
        [ServiceKnownType(typeof(PropertyTypeDictDTO))]
        [ServiceKnownType(typeof(StateSetDTO<StateDTO<ValveState>>))]
        [ServiceKnownType(typeof(StateSetDTO<StateDTO<CompUnitState>>))]
        [ServiceKnownType(typeof(StateSetDTO<StateDTO<CompShopState>>))]
        DictionaryRepositoryDTO GetDictionaryRepository(bool force);
    }
}
