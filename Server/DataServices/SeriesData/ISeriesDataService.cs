using System;
using System.Collections.Generic;
using System.ServiceModel;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData;
using GazRouter.DTO.SeriesData.CompUnitPropertyValues;
using GazRouter.DTO.SeriesData.GasInPipes;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.SerieChecks;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.DTO.SeriesData.Trends;
using GazRouter.DTO.SeriesData.ValueMessages;

namespace GazRouter.DataServices.SeriesData
{
    [Service("Значения свойств")]
    [ServiceContract] 
    public interface ISeriesDataService
    {
        
        [ServiceAction("Получение списка серий")]
        [OperationContract]
        List<SeriesDTO> GetSeriesList(GetSeriesListParameterSet parameters);
        

        [ServiceAction("Получение серии")]
        [OperationContract]
        SeriesDTO GetSeries(GetSeriesParameterSet parameters);
        
        
        [ServiceAction("Добавление новой серии")]
        [OperationContract]
        SeriesDTO AddSerie(AddSeriesParameterSet parameters);




        [ServiceAction("Получение списка ошибок и тревог для значений свойств за интервал")]
        [OperationContract]
        Dictionary<Guid, Dictionary<PropertyType, List<PropertyValueMessageDTO>>> GetPropertyValueMessageList(
            GetPropertyValueMessageListParameterSet parameters);

        [ServiceAction("Выполнение проверки объекта")]
        [OperationContract]
        void PerformChecking(List<PerformCheckingParameterSet> parameters);

        [ServiceAction("Квитирование тревоги для значения свойства")]
        [OperationContract]
        void AcceptMessage(Guid parameters);
        


        [ServiceAction("Получение значения свойства")]
        [OperationContract]
        [ServiceKnownType(typeof(PropertyValueDoubleDTO))]
        [ServiceKnownType(typeof(PropertyValueStringDTO))]
        [ServiceKnownType(typeof(PropertyValueDateDTO))]
        [ServiceKnownType(typeof(PropertyValueEmptyDTO))]
        BasePropertyValueDTO GetPropertyValue(GetPropertyValueParameterSet parameters);


		[ServiceAction("Получение тренда")]
		[OperationContract]
		[ServiceKnownType(typeof(PropertyValueDoubleDTO))]
		[ServiceKnownType(typeof(PropertyValueStringDTO))]
        [ServiceKnownType(typeof(PropertyValueDateDTO))]
        [ServiceKnownType(typeof(PropertyValueEmptyDTO))]
        TrendDTO GetTrend(GetTrendParameterSet parameters);

        
        [ServiceAction("Получение значений свойства")]
        [OperationContract]
        [ServiceKnownType(typeof(PropertyValueDoubleDTO))]
        [ServiceKnownType(typeof(PropertyValueStringDTO))]
        [ServiceKnownType(typeof(PropertyValueDateDTO))]
        [ServiceKnownType(typeof(PropertyValueEmptyDTO))]
        List<BasePropertyValueDTO> GetPropertyValueList(GetPropertyValueListParameterSet parameters);

        
        
        [ServiceAction("Получение значений всех свойств для указанных сущностей (или всех) за интервал")]
        [OperationContract]
        [ServiceKnownType(typeof(PropertyValueDoubleDTO))]
        [ServiceKnownType(typeof(PropertyValueStringDTO))]
        [ServiceKnownType(typeof(PropertyValueDateDTO))]
        [ServiceKnownType(typeof(PropertyValueEmptyDTO))]
        Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> GetEntityPropertyValueList(GetEntityPropertyValueListParameterSet parameters);



        [ServiceAction("Запись параметра в серии")]
        [OperationContract]
        void SetPropertyValue(List<SetPropertyValueParameterSet> parameters);



        [ServiceAction("Получение списка проверок для серии")]
        [OperationContract]
        List<SerieCheckDTO> GetSerieCheckList();


        [ServiceAction("Обновление параметров проверки")]
        [OperationContract]
        void UpdateSerieCheck(UpdateSerieCheckParameterSet parameters);


        [ServiceAction("Изменение признака обязательности для свойств")]
        [OperationContract]
        void UpdateEntityTypeProperty(UpdateEntityPropertyTypeParameterSet parameters);



        [ServiceAction("Получение значений свойств ГПА")]
        [OperationContract]
        Dictionary<PropertyType, PropertyValueDoubleDTO> GetCompUnitPropertyValueList(GetCompUnitPropertyValuesParameterSet parameters);


        [ServiceAction("Получение данных для наработки ГПА")]
        [OperationContract]
		[ServiceKnownType(typeof(SiteDTO))]
        [ServiceKnownType(typeof(CompStationDTO))]
        [ServiceKnownType(typeof(CompShopDTO))]
        [ServiceKnownType(typeof(CompUnitDTO))]
        [ServiceKnownType(typeof(DateIntervalDTO))]
        CompUnitsOperatingTimeDto GetOperatingTimeCompUnitList(DateIntervalParameterSet parameters);

        
        
        [ServiceAction("Получение данных по запасу газа")]
        [OperationContract]
        List<GasInPipeDTO> GetGasInPipeList(GetGasInPipeListParameterSet parameters);


        [ServiceAction("Получение изменения запаса газа за сутки")]
        [OperationContract]
        double GetGasInPipeChange(DateTime parameters);


    }
}