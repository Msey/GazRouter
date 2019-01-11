using System;
using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.DataLoadMonitoring;

namespace GazRouter.DataServices.DataLoadMonitoring
{
    [Service("Мониторинг загрузки данных") ]
    [ServiceContract]
    public interface IDataLoadService
    {
        [ServiceAction("Получение статистики загрузки данных по всем ЛПУ (режимные данные)")]
        [OperationContract]
        List<SiteDataLoadStatistics> GetDataLoadSiteStatisticsTechData(DateTime dt);
        
        [ServiceAction("Получение получение значений по ЛПУ за выбранную серию данных (режимные данные)")]
        [OperationContract]
        [ServiceKnownType(typeof(EntityPropertyValueStringDTO))]
        [ServiceKnownType(typeof(EntityPropertyValueDateDTO))]
        [ServiceKnownType(typeof(EntityPropertyValueDoubleDTO))]
        List<BaseEntityProperty> GetSiteTechData(EntityPropertyValueParameterSet parameters);

        [ServiceAction("Получение данных по изменению режима за выбранные метки времени")]
        [OperationContract]
        [ServiceKnownType(typeof(CompressorShopValuesChangeDTO))]
        [ServiceKnownType(typeof(MeasureLineGasFlowChangeDTO))]
        [ServiceKnownType(typeof(ConsumerGasFlowChangeDTO))]
        [ServiceKnownType(typeof(ChangeModeValueDouble))]
        [ServiceKnownType(typeof(ChangeModeValue<double?>))]
        GasModeChangeData GetGasModeChangeData(GasModeChangeParameterSet parameters);

        [ServiceAction("Получение данных по изменению режима за последние метки времени")]
        [OperationContract]
        [ServiceKnownType(typeof(CompressorShopValuesChangeDTO))]
        [ServiceKnownType(typeof(MeasureLineGasFlowChangeDTO))]
        [ServiceKnownType(typeof(ConsumerGasFlowChangeDTO))]
        [ServiceKnownType(typeof(ChangeModeValueDouble))]
        [ServiceKnownType(typeof(ChangeModeValue<double?>))]
        GasModeChangeData GetGasModeChangeDataLastSerie();


        [ServiceAction("Получение данных НСИ + последние значения за сутки по запасу газа")]
        [OperationContract]
        GasSupplyDataSetDTO GetGasSupplyDataSet(int systemId);

        [ServiceAction("Получение данных по запасу газа за метку времени")]
        [OperationContract]
        List<GasSupplyValue> GetGasSupplyValues(DateTime dt);

        [ServiceAction("Запас газа по предприятю за период")]
        [OperationContract]
        List<GasSupplySumValueDTO> GetSumGasSupplyValuesByEnterprise(GasSupplySumParameterSet paramSet);

    }
}
