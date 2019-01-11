using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.GasLeaks;

namespace GazRouter.DataServices.GasLeaks
{
    [Service("Утечки")]
    [ServiceContract]
    public interface IGasLeaksService
    {
        
        [ServiceAction("Получение списка утечек")]
        [OperationContract]
        List<LeakDTO> GetLeaks(GetLeaksParameterSet parameters);

        [ServiceAction("Удаление утечки")]
        [OperationContract]
        void DeleteLeak(int parameters);

        [ServiceAction("Добавление утечки")]
        [OperationContract]
        int AddLeak(AddLeakParameterSet parameters);

        [ServiceAction("Редактирование утечки")]
        [OperationContract]
        void EditLeak(EditLeakParameterSet parameters);

    }
}
