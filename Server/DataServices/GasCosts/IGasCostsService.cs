using System;
using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.GasCosts.Import;
namespace GazRouter.DataServices.GasCosts
{
    [Service("Расход газа на ПТН")]
    [ServiceContract]
    public interface IGasCostsService
    {
        [ServiceAction("Получение списка расходов")]
        [OperationContract]
        List<GasCostDTO> GetGasCostList(GetGasCostListParameterSet parameter);


        [ServiceAction("Получение значений показателей по умолчанию")]
        [OperationContract]
        List<DefaultParamValuesDTO> GetDefaultParamValues(GetGasCostListParameterSet parameter);

        [ServiceAction("Сохранение значений показателей по умолчанию")]
        [OperationContract]
        void SetDefaultParamValues(List<DefaultParamValuesDTO> parameter);


        [ServiceAction("Получение списка возможных типов расходов")]
        [OperationContract]
        List<GasCostTypeDTO> GetCostTypeList();

        [ServiceAction("Добавление расхода")]
        [OperationContract]
        int AddGasCost(AddGasCostParameterSet parameters);

        [ServiceAction("Удаление расхода")]
        [OperationContract]
        void DeleteGasCost(int parameters);

        [ServiceAction("Редактирование расхода")]
        [OperationContract]
        void EditGasCost(EditGasCostParameterSet parameters);

        [ServiceAction("Получение списка разрешений на редактирование")]
        [OperationContract]
        List<GasCostAccessDTO> GetGasCostAccessList(GetGasCostAccessListParameterSet parameter);

        [ServiceAction("Изменение списка разрешений на редактирование")]
        [OperationContract]
        void UpdateGasCostAccessList(List<GasCostAccessDTO> parameters);


        [ServiceAction("Добавить информацию в журнал импорта")]
        [OperationContract]
        int AddGasCostImportInfo(AddGasCostImportInfoParameterSet parameters);


        [ServiceAction("Удалить информацию из журнала импорта")]
        [OperationContract]
        void DeleteGasCostImportInfo(int parameters);


        [ServiceAction("Обновить видимость статей для ЛПУ")]
        [OperationContract]
        void UpdateGasCostsVisibility(UpdateGasCostsVisibilityParameterSet parameters);

        [ServiceAction("Обновить видимость статьи для ЛПУ")]
        [OperationContract]
        void UpdateGasCostVisibility(AddGasCostVisibilityParameterSet parameters);


        [ServiceAction("Получить список доступа статей по ЛПУ")]
        [OperationContract]
        List<GasCostVisibilityDTO> GetGasCostsVisibility(Guid? siteId);
    }
}
