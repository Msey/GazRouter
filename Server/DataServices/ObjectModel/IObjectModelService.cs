using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using GazRouter.DTO.ObjectModel.CoolingUnit;
using GazRouter.DataServices.Infrastructure.Attributes;
using GazRouter.DTO.Attachments;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.Dictionaries.EntityTypeProperties;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.EntitySelector;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Aggregators;
using GazRouter.DTO.ObjectModel.BoilerPlants;
using GazRouter.DTO.ObjectModel.Boilers;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStationCoolingRecomended;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.CoolingStations;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.Entities;
using GazRouter.DTO.ObjectModel.Entities.Urls;
using GazRouter.DTO.ObjectModel.Inconsistency;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.PowerPlants;
using GazRouter.DTO.ObjectModel.PowerUnits;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Segment;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.DTO.Dictionaries.BoilerTypes;
using GazRouter.DTO.Dictionaries.PowerUnitTypes;
using GazRouter.DTO.Dictionaries.RegulatorTypes;
using GazRouter.DTO.ObjectModel.Regulators;
using GazRouter.DTO.Dictionaries.HeaterTypes;
using GazRouter.DTO.ObjectModel.Heaters;
using GazRouter.DTO.Dictionaries.EmergencyValveTypes;
using GazRouter.DTO.ObjectModel.EmergencyValves;
using GazRouter.DTO.ObjectModel.ChangeLogs;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.Entities.IsInputOff;
using GazRouter.DTO.ObjectModel.OperConsumers;

namespace GazRouter.DataServices.ObjectModel
{
	[Service("Объектная модель")]
	[ServiceContract]
	public interface IObjectModelService
    {
        #region Common
        
        [ServiceAction("Получение страницы сущностей")]
		[OperationContract]
		EntitiesPageDTO GetEntitiesPage(GetEntitesPageParameterSet parameters);

        [ServiceAction("Получение списка сущностей")]
        [OperationContract]
        List<CommonEntityDTO> GetEntityList(GetEntityListParameterSet parameters);
        
		[ServiceAction("Получение сущности по идентификатору")]
		[OperationContract]
        [ServiceKnownType(typeof(DistrStationDTO))]
        [ServiceKnownType(typeof(ValveDTO))]
        [ServiceKnownType(typeof(MeasLineDTO))]
        [ServiceKnownType(typeof(SiteDTO))]
        [ServiceKnownType(typeof(CompStationDTO))]
        [ServiceKnownType(typeof(CompShopDTO))]
        [ServiceKnownType(typeof(CompUnitDTO))]
        [ServiceKnownType(typeof(ReducingStationDTO))]
        [ServiceKnownType(typeof(BoilerDTO))]
        [ServiceKnownType(typeof(MeasStationDTO))]
        [ServiceKnownType(typeof(PowerUnitDTO))]
        [ServiceKnownType(typeof(DistrStationOutletDTO))]
        [ServiceKnownType(typeof(PipelineDTO))]
        [ServiceKnownType(typeof(MeasPointDTO))]
        [ServiceKnownType(typeof(CoolingStationDTO))]
        [ServiceKnownType(typeof(BoilerPlantDTO))]
        [ServiceKnownType(typeof(PowerPlantDTO))]
        [ServiceKnownType(typeof(CoolingUnitDTO))]
        [ServiceKnownType(typeof(AggregatorDTO))]
        [ServiceKnownType(typeof(ChangeDTO))]
        [ServiceKnownType(typeof(GetChangeLogParameterSet))]
        CommonEntityDTO GetEntityById(Guid parameters);




        [ServiceAction("Удаление сущности")]
        [OperationContract]
        void DeleteEntity(DeleteEntityParameterSet parameters);

        [ServiceAction("Установление порядка сортировки сущности")]
        [OperationContract]
        void SetSortOrder(SetSortOrderParameterSet parameters);

        [ServiceAction("Добавление примечания к сущности")]
        [OperationContract]
        void AddDescription(AddDescriptionParameterSet parameters);

        [ServiceAction("Получение лога изменения сущности")]
        [OperationContract]
        List<EntityChangeDTO> GetEntityChangeList(Guid parameters);




        [ServiceAction("Получение списка прикрепленных документов сущности")]
        [OperationContract]
        List<AttachmentDTO<int, Guid>> GetEntityAttachmentList(Guid? parameters);

        [ServiceAction("Прикрепить документ к сущности")]
        [OperationContract]
        int AddEntityAttachment(AddAttachmentParameterSet<Guid> parameters);

        [ServiceAction("Удалить прикрепленный к сущности документ")]
        [OperationContract]
        void RemoveEntityAttachment(int parameters);




        [ServiceAction("Получение списка внешних ссылок сущности")]
        [OperationContract]
        List<EntityUrlDTO> GetEntityUrlList(Guid? parameters);

        [ServiceAction("Добавить внешнюю ссылку сущности")]
        [OperationContract]
        int AddEntityUrl(AddEntityUrlParameterSet parameters);

        [ServiceAction("Изменить внешнюю ссылку сущности")]
        [OperationContract]
        void EditEntityUrl(EditEntityUrlParameterSet parameters);

        [ServiceAction("Удалить внешнюю ссылку сущности")]
        [OperationContract]
        void RemoveEntityUrl(int parameters);


        #endregion


        [ServiceAction("Получение дерева объектной модели")]
        [OperationContract]
        TreeDataDTO GetFullTree(EntityTreeGetParameterSet parameters);


        [ServiceAction("Получение списка предприятий")]
        [OperationContract]
        List<EnterpriseDTO> GetEnterpriseList();



        #region Sites

        [ServiceAction("Получение списка предприятий и ЛПУ")]
        [OperationContract]
        List<CommonEntityDTO> GetCurrentEnterpriseAndSites();

        [ServiceAction("Получение списка ЛПУ")]
        [OperationContract]
        List<SiteDTO> GetSiteList(GetSiteListParameterSet parameters);

        [ServiceAction("Поиск ЛПУ")]
        [OperationContract]
        Guid? FindSite(Guid parameters);

        [ServiceAction("Добавление ЛПУ")]
        [OperationContract]
        Guid AddSite(AddSiteParameterSet parameters);

        [ServiceAction("Редактирование ЛПУ")]
        [OperationContract]
        void EditSite(EditSiteParameterSet parameters);
        
        #endregion
        
        
        #region CompStation

        [ServiceAction("Получение списка КС")]
        [OperationContract]
        List<CompStationDTO> GetCompStationList(GetCompStationListParameterSet parameters);

        [ServiceAction("Получение дерева КС по идентификатору ЛПУ")]
        [OperationContract]
        TreeDataDTO GetCompStationTree(Guid? parameters);

        [ServiceAction("Добавление КС")]
        [OperationContract]
        Guid AddCompStation(AddCompStationParameterSet parameters);

        [ServiceAction("Редактирование КС")]
        [OperationContract]
        void EditCompStation(EditCompStationParameterSet parameters);
        
        #endregion


        #region CompStationCoolingRecomended

        [ServiceAction("Получение списка рекомендуемой температуры на выходе КС")]
        [OperationContract]
        List<CompStationCoolingRecomendedDTO> GetCompStationCoolingRecomendedList(Guid parameters);

        [ServiceAction("Редактирование рекомендуемой температуры на выходе КС")]
        [OperationContract]
        void SetCompStationCoolingRecomended(SetCompStationCoolingRecomendedParameterSet parameters);
        
        #endregion


        #region CompShop

        [ServiceAction("Получение списка КЦ")]
        [OperationContract]
        List<CompShopDTO> GetCompShopList(GetCompShopListParameterSet parameters);

        [ServiceAction("Добавление КЦ")]
        [OperationContract]
        Guid AddCompShop(AddCompShopParameterSet parameters);

        [ServiceAction("Редактирование КЦ")]
        [OperationContract]
        void EditCompShop(EditCompShopParameterSet parameters);
#endregion

        #region CompUnit

        [ServiceAction("Получение параметров ГПА по идентификатору")]
        [OperationContract]
        CompUnitDTO GetCompUnitById(Guid parameters);

        [ServiceAction("Получение списка ГПА")]
        [OperationContract]
        List<CompUnitDTO> GetCompUnitList(GetCompUnitListParameterSet parameters);

        [ServiceAction("Добавление ГПА")]
        [OperationContract]
        Guid AddCompUnit(AddCompUnitParameterSet parameters);

        [ServiceAction("Редактирование ГПА")]
        [OperationContract]
        void EditCompUnit(EditCompUnitParameterSet parameters);

        #endregion


        #region BoilerPlant

        [ServiceAction("Добавление Котельной")]
        [OperationContract]
        Guid AddBoilerPlant(AddBoilerPlantParameterSet parameters);

        [ServiceAction("Редактирование Котельной")]
        [OperationContract]
        void EditBoilerPlant(EditBoilerPlantParameterSet parameters);

        #endregion


        #region Boiler

        [ServiceAction("Получение котла по идентификатору")]
        [OperationContract]
        BoilerDTO GetBoilerById(Guid parameters);

        [ServiceAction("Добавление котла")]
        [OperationContract]
        Guid AddBoiler(AddBoilerParameterSet parameters);

        [ServiceAction("Редактирование котла")]
        [OperationContract]
        void EditBoiler(EditBoilerParameterSet parameters);

        #endregion


        #region PowerPlant

        [ServiceAction("Добавление ЭСН")]
        [OperationContract]
        Guid AddPowerPlant(AddPowerPlantParameterSet parameters);

        [ServiceAction("Редактирование ЭСН")]
        [OperationContract]
        void EditPowerPlant(EditPowerPlantParameterSet parameters);

        #endregion


        #region PowerUnit

        [ServiceAction("Получение электроагрегата по идентификатору")]
        [OperationContract]
        PowerUnitDTO GetPowerUnitById(Guid parameters);

        [ServiceAction("Добавление электроагрегата")]
        [OperationContract]
        Guid AddPowerUnit(AddPowerUnitParameterSet parameters);

        [ServiceAction("Редактирование электроагрегата")]
        [OperationContract]
        void EditPowerUnit(EditPowerUnitParameterSet parameters);

        #endregion

        #region CoolingStation

        [ServiceAction("Добавление СОГ")]
        [OperationContract]
        Guid AddCoolingStation(AddCoolingStationParameterSet parameters);

        [ServiceAction("Редактирование СОГ")]
        [OperationContract]
        void EditCoolingStation(EditCoolingStationParameterSet parameters);

        #endregion


        #region CoolingUnit

        [ServiceAction("Получение списка установок охлаждения газа")]
        [OperationContract]
        List<CoolingUnitDTO> GetCoolingUnitList(GetCoolingUnitListParameterSet parameters);

        [ServiceAction("Получение установки охлаждения газа по идентификатору")]
        [OperationContract]
        CoolingUnitDTO GetCoolingUnitById(Guid parameters);

        [ServiceAction("Добавление установки охлаждения газа")]
        [OperationContract]
        Guid AddCoolingUnit(AddCoolingUnitParameterSet parameters);

        [ServiceAction("Редактирование установки охлаждения газа")]
        [OperationContract]
        void EditCoolingUnit(EditCoolingUnitParameterSet parameters);

        #endregion
        
        
        #region DistrStation

        [ServiceAction("Получение ветки дерева по ГРС")]
        [OperationContract]
        [ServiceKnownType(typeof(DistrStationDTO))]
        [ServiceKnownType(typeof(BoilerDTO))]
        TreeDataDTO GetDistrStationTree(GetDistrStationListParameterSet parameters);

        [ServiceAction("Добавление ГРС")]
        [OperationContract]
        Guid AddDistrStation(AddDistrStationParameterSet parameters);

        [ServiceAction("Редактирование ГРС")]
        [OperationContract]
        void EditDistrStation(EditDistrStationParameterSet parameters);

        #endregion


        #region DistrStationOutlet

        [ServiceAction("Получение списка выходов ГРС")]
        [OperationContract]
        List<DistrStationOutletDTO> GetDistrStationOutletList(GetDistrStationOutletListParameterSet parameters);
        
        [ServiceAction("Добавление выхода ГРС")]
        [OperationContract]
        Guid AddDistrStationOutlet(AddDistrStationOutletParameterSet parameters);

        [ServiceAction("Редактирование выхода ГРС")]
        [OperationContract]
        void EditDistrStationOutlet(EditDistrStationOutletParameterSet parameters);

        #endregion

        #region Consumers

        [ServiceAction("Получение списка потребителей (подключений ГРС)")]
        [OperationContract]
        List<ConsumerDTO> GetConsumerList(GetConsumerListParameterSet parameters);

        [ServiceAction("Добавление потребителя (подключения ГРС)")]
        [OperationContract]
        Guid AddConsumer(AddConsumerParameterSet parameters);

        [ServiceAction("Редактирование потребителя (подключения ГРС)")]
        [OperationContract]
        void EditConsumer(EditConsumerParameterSet parameters);

        #endregion


        #region OperConsumers

        [ServiceAction("Получение списка ПЭН")]
        [OperationContract]
        List<OperConsumerDTO> GetOperConsumers(GetOperConsumerListParameterSet parameters);

        [ServiceAction("Добавление ПЭН")]
        [OperationContract]
        Guid AddOperConsumer(AddEditOperConsumerParameterSet parameters);

        [ServiceAction("Редактирование ПЭН")]
        [OperationContract]
        void EditOperConsumer(AddEditOperConsumerParameterSet parameters);

        #endregion


        #region MeasStation

        [ServiceAction("Получение списка ГИС")]
        [OperationContract]
        List<MeasStationDTO> GetMeasStationList(GetMeasStationListParameterSet parameters);

        [ServiceAction("Получение ветки дерева по ГИС")]
        [OperationContract]
        TreeDataDTO GetMeasStationTree(GetMeasStationListParameterSet parameters);

        [ServiceAction("Добавление ГИС")]
        [OperationContract]
        Guid AddMeasStation(AddMeasStationParameterSet parameters);

        [ServiceAction("Редактирование ГИС")]
        [OperationContract]
        void EditMeasStation(EditMeasStationParameterSet parameters);

        #endregion


        #region MeasLine

        [ServiceAction("Получение списка замерных линий ГИС")]
        [OperationContract]
        List<MeasLineDTO> GetMeasLineList(GetMeasLineListParameterSet parameters);

        [ServiceAction("Добавление замерной линии ГИС")]
        [OperationContract]
        Guid AddMeasLine(AddMeasLineParameterSet parameters);

        [ServiceAction("Редактирование замерной линии ГИС")]
        [OperationContract]
        void EditMeasLine(EditMeasLineParameterSet parameters);

        #endregion


        #region ReducingStation

	    [ServiceAction("Получение списка ПРГ")]
	    [OperationContract]
	    List<ReducingStationDTO> GetReducingStationList(GetReducingStationListParameterSet parameters);

        [ServiceAction("Добавление ПРГ")]
        [OperationContract]
        Guid AddReducingStation(AddReducingStationParameterSet parameters);

        [ServiceAction("Редактирование ПРГ")]
        [OperationContract]
        void EditReducingStation(EditReducingStationParameterSet parameters);
        
        #endregion


        #region MeasPoint

        [ServiceAction("Получение точки измерения параметров газа по идентификатору родительского объекта")]
        [OperationContract]
        MeasPointDTO GetMeasPointByParent(Guid parameters);


        [ServiceAction("Добавление точки измерения параметров газа")]
        [OperationContract]
        Guid AddMeasPoint(AddMeasPointParameterSet parameters);

        [ServiceAction("Редактирование точки измерения параметров газа")]
        [OperationContract]
        void EditMeasPoint(EditMeasPointParameterSet parameters);


        [ServiceAction("Поиск подходящей точки измерения параметров газа по идентификатору объекта")]
        [OperationContract]
        MeasPointDTO FindMeasPoint(Guid parameters);


        #endregion


        #region Pipeline

        [ServiceAction("Получение дерева газопроводов по идентификатору ЛПУ")]
        [OperationContract]
        TreeDataDTO GetPipelineTree(Guid? parameters);


        [ServiceAction("Получение списка газопроводов")]
        [OperationContract]
        List<PipelineDTO> GetPipelineList(GetPipelineListParameterSet parameters);

        [ServiceAction("Получение газопровода по идентификатору")]
        [OperationContract]
        PipelineDTO GetPipelineById(Guid parameters);

        [ServiceAction("Добавление газопровода")]
        [OperationContract]
        Guid AddPipeline(AddPipelineWithConnsParameterSet parameters);

        [ServiceAction("Редактирование газопровода")]
        [OperationContract]
        void EditPipeline(EditPipelineWithConnsParameterSet parameters);

        #endregion


        #region PipelineSegmentBySite

        [ServiceAction("Получение списка сегментов по ЛПУ по идентификатору газопровода")]
        [OperationContract]
        List<SiteSegmentDTO> GetSiteSegmentList(Guid? parameters);

        [ServiceAction("Добавление сегмента по ЛПУ")]
        [OperationContract]
        int AddSiteSegment(AddSiteSegmentParameterSet parameters);

        [ServiceAction("Редактирование сегмента по ЛПУ")]
        [OperationContract]
        void EditSiteSegment(EditSiteSegmentParameterSet parameters);

        [ServiceAction("Удаление сегмента по ЛПУ")]
        [OperationContract]
        void DeleteSiteSegment(int parameters);

        #endregion

        #region PipelineSegmentByRegion

        [ServiceAction("Получение списка сегментов по региону")]
        [OperationContract]
        List<RegionSegmentDTO> GetRegionSegmentList(Guid? parameters);

        [ServiceAction("Добавление сегмента по региону")]
        [OperationContract]
        int AddRegionSegment(AddRegionSegmentParameterSet parameters);

        [ServiceAction("Редактирование сегмента по региону")]
        [OperationContract]
        void EditRegionSegment(EditRegionSegmentParameterSet parameters);

        [ServiceAction("Удаление сегмента по региону")]
        [OperationContract]
        void DeleteRegionSegment(int parameters);

        #endregion


        #region PipelineSegmentByGroup

        [ServiceAction("Получение списка сегментов по группам газапроводов")]
        [OperationContract]
        List<GroupSegmentDTO> GetGroupSegmentList(Guid? parameters);

        [ServiceAction("Добавление сегмента по группе газопроводов")]
        [OperationContract]
        int AddGroupSegment(AddGroupSegmentParameterSet parameters);

        [ServiceAction("Редактирование сегмента по группе газопроводов")]
        [OperationContract]
        void EditGroupSegment(EditGroupSegmentParameterSet parameters);

        [ServiceAction("Удаление сегмента по группе газопроводов")]
        [OperationContract]
        void DeleteGroupSegment(int parameters);

        #endregion


        #region PipelineSegmentByDiameter

        [ServiceAction("Получение списка сегментов по диаметру")]
        [OperationContract]
        List<DiameterSegmentDTO> GetDiameterSegmentList(Guid? parameters);

        [ServiceAction("Добавление сегмента по диаметру")]
        [OperationContract]
        int AddDiameterSegment(AddDiameterSegmentParameterSet parameters);

        [ServiceAction("Редактирование сегмента по диаметру")]
        [OperationContract]
        void EditDiameterSegment(EditDiameterSegmentParameterSet parameters);

        [ServiceAction("Удаление сегмента по диаметру")]
        [OperationContract]
        void DeleteDiameterSegment(int parameters);

        #endregion


        #region PipelineSegmentByPressure

        [ServiceAction("Получение списка сегментов по давлению")]
        [OperationContract]
        List<PressureSegmentDTO> GetPressureSegmentList(Guid? parameters);

        [ServiceAction("Добавление сегмента по давлению")]
        [OperationContract]
        int AddPressureSegment(AddPressureSegmentParameterSet parameters);

        [ServiceAction("Редактирование сегмента по давлению")]
        [OperationContract]
        void EditPressureSegment(EditPressureSegmentParameterSet parameters);

        [ServiceAction("Удаление сегмента по давлению")]
        [OperationContract]
        void DeletePressureSegment(int parameters);

        #endregion


        #region Valve

        [ServiceAction("Получение списка кранов газопровода")]
        [OperationContract]
        List<ValveDTO> GetValveList(GetValveListParameterSet parameters);

        [ServiceAction("Получение крана по идентификатору")]
        [OperationContract]
        ValveDTO GetValveById(Guid parameters);

        [ServiceAction("Добавление кранового узла")]
        [OperationContract]
        Guid AddValve(AddValveParameterSet parameters);

        [ServiceAction("Редактирование кранового узла")]
        [OperationContract]
        void EditValve(EditValveParameterSet parameters);
        
        #endregion

        
        #region Validation

        [ServiceAction("Валидация объектной модели")]
        [OperationContract]
        void Validate();

        [ServiceAction("Получение ошибок валидации")]
        [OperationContract]
        Dictionary<Guid, List<InconsistencyDTO>> GetInconsistencies(Guid? parameters);

        #endregion


        #region EntityProperties

        [ServiceAction("Получение списка параметров (и их свойств) для сущностей")]
        [OperationContract]
        List<EntityTypePropertyDTO> GetEntityTypeProperties(EntityType? parameters);
        
        #endregion

        #region PowerUnitType

        [ServiceAction("Добавление типа электроагрегата")]
        [OperationContract]
        int AddPowerUnitType(AddPowerUnitTypeParameterSet parameters);

        [ServiceAction("Редактирование типа электроагрегата")]
        [OperationContract]
        void EditPowerUnitType(EditPowerUnitTypeParameterSet parameters);

        [ServiceAction("Удалить тип электроагрегата")]
        [OperationContract]
        void RemovePowerUnitType(int parameters);

        [ServiceAction("Получение списка типов электроагрегатов")]
        [OperationContract]
        List<PowerUnitTypeDTO> GetPowerUnitTypes();

        #endregion

        #region BoilerType

        [ServiceAction("Добавление типа котла")]
        [OperationContract]
        int AddBoilerType(AddBoilerTypeParameterSet parameters);

        [ServiceAction("Редактирование типа котла")]
        [OperationContract]
        void EditBoilerType(EditBoilerTypeParameterSet parameters);

        [ServiceAction("Удалить тип котла")]
        [OperationContract]
        void RemoveBoilerType(int parameters);

        [ServiceAction("Получение списка типов котлов")]
        [OperationContract]
        List<BoilerTypeDTO> GetBoilerTypes();


        #endregion


        #region Aggregators

        [ServiceAction("Получение списка агрегаторов")]
        [OperationContract]
        List<AggregatorDTO> GetAggregatorList(GetAggregatorListParameterSet parameters);

        [ServiceAction("Получение агрегатора по идентификатору")]
        [OperationContract]
        AggregatorDTO GetAggregatorById(Guid parameters);

        [ServiceAction("Добавление агрегатора")]
        [OperationContract]
        Guid AddAggregator(AddAggregatorParameterSet parameters);

        [ServiceAction("Редактирование агрегатора")]
        [OperationContract]
        void EditAggregator(EditAggregatorParameterSet parameters);

        #endregion

        #region RegulatorType

        [ServiceAction("Добавление типа крана-регулятора")]
        [OperationContract]
        int AddRegulatorType(AddRegulatorTypeParameterSet parameters);

        [ServiceAction("Редактирование типа крана-регулятора")]
        [OperationContract]
        void EditRegulatorType(EditRegulatorTypeParameterSet parameters);

        [ServiceAction("Удалить тип крана-регулятора")]
        [OperationContract]
        void RemoveRegulatorType(int parameters);

        [ServiceAction("Получение списка типов кранов-регуляторов")]
        [OperationContract]
        List<RegulatorTypeDTO> GetRegulatorTypes();

        #endregion

        #region HeaterType

        [ServiceAction("Добавление типа подогревателя газа")]
        [OperationContract]
        int AddHeaterType(AddHeaterTypeParameterSet parameters);

        [ServiceAction("Редактирование типа подогревателя газа")]
        [OperationContract]
        void EditHeaterType(EditHeaterTypeParameterSet parameters);

        [ServiceAction("Удалить тип подогревателя газа")]
        [OperationContract]
        void RemoveHeaterType(int parameters);

        [ServiceAction("Получение списка типов подогревателя газа")]
        [OperationContract]
        List<HeaterTypeDTO> GetHeaterTypes();

        #endregion

        #region EmergencyValve

        [ServiceAction("Добавление типа предохранительного клапана")]
        [OperationContract]
        int AddEmergencyValveType(AddEmergencyValveTypeParameterSet parameters);

        [ServiceAction("Редактирование типа предохранительного клапана")]
        [OperationContract]
        void EditEmergencyValveType(EditEmergencyValveTypeParameterSet parameters);

        [ServiceAction("Удалить тип предохранительного клапана")]
        [OperationContract]
        void RemoveEmergencyValveType(int parameters);

        [ServiceAction("Получение списка типов предохранительного клапана")]
        [OperationContract]
        List<EmergencyValveTypeDTO> GetEmergencyValveTypes();

        #endregion

        [ServiceAction("Получение лога")]
        [OperationContract]
        List<ChangeDTO> GetChangeLog(GetChangeLogParameterSet parameters);


        [ServiceAction("Установка признака исключения объекта из ручного ввода")]
        [OperationContract]
        void SetIsInputOff(SetIsInputOffParameterSet parameters);

#region GasCosts2
        [ServiceAction("GetDistrStationList")]
        [OperationContract]
        List<DistrStationDTO> GetDistrStationList(GetDistrStationListParameterSet parameters);
        [ServiceAction("GetBoilerListList")]
        [OperationContract]
        List<BoilerDTO> GetBoilerList(GetBoilerListParameterSet parameters);

        [ServiceAction("GetPowerUnitList")]
        [OperationContract]
        List<PowerUnitDTO> GetPowerUnitList(GetPowerUnitListParameterSet parameters);
        
	    [ServiceAction("GetBoilerPlantList")]
	    [OperationContract]
	    List<BoilerPlantDTO> GetBoilerPlantList(int? parameters);
#endregion
    }
}

