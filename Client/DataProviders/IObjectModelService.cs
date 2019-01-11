using System;
using System.ServiceModel;
using GazRouter.DTO.Infrastructure.Faults;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.ObjectModel.CoolingUnit;
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
      
// ReSharper disable once CheckNamespace
namespace GazRouter.DataProviders.ObjectModel  
{
    [ServiceContract]
    public interface IObjectModelService
    {                   
        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetEntitiesPage(GetEntitesPageParameterSet parameters, AsyncCallback callback, object state);
        EntitiesPageDTO EndGetEntitiesPage(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetEntityList(GetEntityListParameterSet parameters, AsyncCallback callback, object state);
        List<CommonEntityDTO> EndGetEntityList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
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
        IAsyncResult BeginGetEntityById(Guid parameters, AsyncCallback callback, object state);
        CommonEntityDTO EndGetEntityById(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteEntity(DeleteEntityParameterSet parameters, AsyncCallback callback, object state);
        void EndDeleteEntity(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetSortOrder(SetSortOrderParameterSet parameters, AsyncCallback callback, object state);
        void EndSetSortOrder(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddDescription(AddDescriptionParameterSet parameters, AsyncCallback callback, object state);
        void EndAddDescription(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetEntityChangeList(Guid parameters, AsyncCallback callback, object state);
        List<EntityChangeDTO> EndGetEntityChangeList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetEntityAttachmentList(Guid? parameters, AsyncCallback callback, object state);
        List<AttachmentDTO<int, Guid>> EndGetEntityAttachmentList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddEntityAttachment(AddAttachmentParameterSet<Guid> parameters, AsyncCallback callback, object state);
        int EndAddEntityAttachment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveEntityAttachment(int parameters, AsyncCallback callback, object state);
        void EndRemoveEntityAttachment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetEntityUrlList(Guid? parameters, AsyncCallback callback, object state);
        List<EntityUrlDTO> EndGetEntityUrlList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddEntityUrl(AddEntityUrlParameterSet parameters, AsyncCallback callback, object state);
        int EndAddEntityUrl(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditEntityUrl(EditEntityUrlParameterSet parameters, AsyncCallback callback, object state);
        void EndEditEntityUrl(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveEntityUrl(int parameters, AsyncCallback callback, object state);
        void EndRemoveEntityUrl(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetFullTree(EntityTreeGetParameterSet parameters, AsyncCallback callback, object state);
        TreeDataDTO EndGetFullTree(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetEnterpriseList(object parameters, AsyncCallback callback, object state);
        List<EnterpriseDTO> EndGetEnterpriseList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCurrentEnterpriseAndSites(object parameters, AsyncCallback callback, object state);
        List<CommonEntityDTO> EndGetCurrentEnterpriseAndSites(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetSiteList(GetSiteListParameterSet parameters, AsyncCallback callback, object state);
        List<SiteDTO> EndGetSiteList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginFindSite(Guid parameters, AsyncCallback callback, object state);
        Guid? EndFindSite(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddSite(AddSiteParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddSite(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditSite(EditSiteParameterSet parameters, AsyncCallback callback, object state);
        void EndEditSite(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCompStationList(GetCompStationListParameterSet parameters, AsyncCallback callback, object state);
        List<CompStationDTO> EndGetCompStationList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCompStationTree(Guid? parameters, AsyncCallback callback, object state);
        TreeDataDTO EndGetCompStationTree(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddCompStation(AddCompStationParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddCompStation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditCompStation(EditCompStationParameterSet parameters, AsyncCallback callback, object state);
        void EndEditCompStation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCompStationCoolingRecomendedList(Guid parameters, AsyncCallback callback, object state);
        List<CompStationCoolingRecomendedDTO> EndGetCompStationCoolingRecomendedList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetCompStationCoolingRecomended(SetCompStationCoolingRecomendedParameterSet parameters, AsyncCallback callback, object state);
        void EndSetCompStationCoolingRecomended(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCompShopList(GetCompShopListParameterSet parameters, AsyncCallback callback, object state);
        List<CompShopDTO> EndGetCompShopList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddCompShop(AddCompShopParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddCompShop(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditCompShop(EditCompShopParameterSet parameters, AsyncCallback callback, object state);
        void EndEditCompShop(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCompUnitById(Guid parameters, AsyncCallback callback, object state);
        CompUnitDTO EndGetCompUnitById(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCompUnitList(GetCompUnitListParameterSet parameters, AsyncCallback callback, object state);
        List<CompUnitDTO> EndGetCompUnitList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddCompUnit(AddCompUnitParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddCompUnit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditCompUnit(EditCompUnitParameterSet parameters, AsyncCallback callback, object state);
        void EndEditCompUnit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddBoilerPlant(AddBoilerPlantParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddBoilerPlant(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditBoilerPlant(EditBoilerPlantParameterSet parameters, AsyncCallback callback, object state);
        void EndEditBoilerPlant(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetBoilerById(Guid parameters, AsyncCallback callback, object state);
        BoilerDTO EndGetBoilerById(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddBoiler(AddBoilerParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddBoiler(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditBoiler(EditBoilerParameterSet parameters, AsyncCallback callback, object state);
        void EndEditBoiler(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddPowerPlant(AddPowerPlantParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddPowerPlant(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditPowerPlant(EditPowerPlantParameterSet parameters, AsyncCallback callback, object state);
        void EndEditPowerPlant(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetPowerUnitById(Guid parameters, AsyncCallback callback, object state);
        PowerUnitDTO EndGetPowerUnitById(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddPowerUnit(AddPowerUnitParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddPowerUnit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditPowerUnit(EditPowerUnitParameterSet parameters, AsyncCallback callback, object state);
        void EndEditPowerUnit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddCoolingStation(AddCoolingStationParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddCoolingStation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditCoolingStation(EditCoolingStationParameterSet parameters, AsyncCallback callback, object state);
        void EndEditCoolingStation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCoolingUnitList(GetCoolingUnitListParameterSet parameters, AsyncCallback callback, object state);
        List<CoolingUnitDTO> EndGetCoolingUnitList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetCoolingUnitById(Guid parameters, AsyncCallback callback, object state);
        CoolingUnitDTO EndGetCoolingUnitById(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddCoolingUnit(AddCoolingUnitParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddCoolingUnit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditCoolingUnit(EditCoolingUnitParameterSet parameters, AsyncCallback callback, object state);
        void EndEditCoolingUnit(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        [ServiceKnownType(typeof(DistrStationDTO))] 
        [ServiceKnownType(typeof(BoilerDTO))] 
        IAsyncResult BeginGetDistrStationTree(GetDistrStationListParameterSet parameters, AsyncCallback callback, object state);
        TreeDataDTO EndGetDistrStationTree(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddDistrStation(AddDistrStationParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddDistrStation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditDistrStation(EditDistrStationParameterSet parameters, AsyncCallback callback, object state);
        void EndEditDistrStation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDistrStationOutletList(GetDistrStationOutletListParameterSet parameters, AsyncCallback callback, object state);
        List<DistrStationOutletDTO> EndGetDistrStationOutletList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddDistrStationOutlet(AddDistrStationOutletParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddDistrStationOutlet(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditDistrStationOutlet(EditDistrStationOutletParameterSet parameters, AsyncCallback callback, object state);
        void EndEditDistrStationOutlet(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetConsumerList(GetConsumerListParameterSet parameters, AsyncCallback callback, object state);
        List<ConsumerDTO> EndGetConsumerList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddConsumer(AddConsumerParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddConsumer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditConsumer(EditConsumerParameterSet parameters, AsyncCallback callback, object state);
        void EndEditConsumer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetOperConsumers(GetOperConsumerListParameterSet parameters, AsyncCallback callback, object state);
        List<OperConsumerDTO> EndGetOperConsumers(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddOperConsumer(AddEditOperConsumerParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddOperConsumer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditOperConsumer(AddEditOperConsumerParameterSet parameters, AsyncCallback callback, object state);
        void EndEditOperConsumer(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetMeasStationList(GetMeasStationListParameterSet parameters, AsyncCallback callback, object state);
        List<MeasStationDTO> EndGetMeasStationList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetMeasStationTree(GetMeasStationListParameterSet parameters, AsyncCallback callback, object state);
        TreeDataDTO EndGetMeasStationTree(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddMeasStation(AddMeasStationParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddMeasStation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditMeasStation(EditMeasStationParameterSet parameters, AsyncCallback callback, object state);
        void EndEditMeasStation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetMeasLineList(GetMeasLineListParameterSet parameters, AsyncCallback callback, object state);
        List<MeasLineDTO> EndGetMeasLineList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddMeasLine(AddMeasLineParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddMeasLine(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditMeasLine(EditMeasLineParameterSet parameters, AsyncCallback callback, object state);
        void EndEditMeasLine(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetReducingStationList(GetReducingStationListParameterSet parameters, AsyncCallback callback, object state);
        List<ReducingStationDTO> EndGetReducingStationList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddReducingStation(AddReducingStationParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddReducingStation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditReducingStation(EditReducingStationParameterSet parameters, AsyncCallback callback, object state);
        void EndEditReducingStation(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetMeasPointByParent(Guid parameters, AsyncCallback callback, object state);
        MeasPointDTO EndGetMeasPointByParent(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddMeasPoint(AddMeasPointParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddMeasPoint(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditMeasPoint(EditMeasPointParameterSet parameters, AsyncCallback callback, object state);
        void EndEditMeasPoint(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginFindMeasPoint(Guid parameters, AsyncCallback callback, object state);
        MeasPointDTO EndFindMeasPoint(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetPipelineTree(Guid? parameters, AsyncCallback callback, object state);
        TreeDataDTO EndGetPipelineTree(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetPipelineList(GetPipelineListParameterSet parameters, AsyncCallback callback, object state);
        List<PipelineDTO> EndGetPipelineList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetPipelineById(Guid parameters, AsyncCallback callback, object state);
        PipelineDTO EndGetPipelineById(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddPipeline(AddPipelineWithConnsParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddPipeline(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditPipeline(EditPipelineWithConnsParameterSet parameters, AsyncCallback callback, object state);
        void EndEditPipeline(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetSiteSegmentList(Guid? parameters, AsyncCallback callback, object state);
        List<SiteSegmentDTO> EndGetSiteSegmentList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddSiteSegment(AddSiteSegmentParameterSet parameters, AsyncCallback callback, object state);
        int EndAddSiteSegment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditSiteSegment(EditSiteSegmentParameterSet parameters, AsyncCallback callback, object state);
        void EndEditSiteSegment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteSiteSegment(int parameters, AsyncCallback callback, object state);
        void EndDeleteSiteSegment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetRegionSegmentList(Guid? parameters, AsyncCallback callback, object state);
        List<RegionSegmentDTO> EndGetRegionSegmentList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddRegionSegment(AddRegionSegmentParameterSet parameters, AsyncCallback callback, object state);
        int EndAddRegionSegment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditRegionSegment(EditRegionSegmentParameterSet parameters, AsyncCallback callback, object state);
        void EndEditRegionSegment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteRegionSegment(int parameters, AsyncCallback callback, object state);
        void EndDeleteRegionSegment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetGroupSegmentList(Guid? parameters, AsyncCallback callback, object state);
        List<GroupSegmentDTO> EndGetGroupSegmentList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddGroupSegment(AddGroupSegmentParameterSet parameters, AsyncCallback callback, object state);
        int EndAddGroupSegment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditGroupSegment(EditGroupSegmentParameterSet parameters, AsyncCallback callback, object state);
        void EndEditGroupSegment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteGroupSegment(int parameters, AsyncCallback callback, object state);
        void EndDeleteGroupSegment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDiameterSegmentList(Guid? parameters, AsyncCallback callback, object state);
        List<DiameterSegmentDTO> EndGetDiameterSegmentList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddDiameterSegment(AddDiameterSegmentParameterSet parameters, AsyncCallback callback, object state);
        int EndAddDiameterSegment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditDiameterSegment(EditDiameterSegmentParameterSet parameters, AsyncCallback callback, object state);
        void EndEditDiameterSegment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeleteDiameterSegment(int parameters, AsyncCallback callback, object state);
        void EndDeleteDiameterSegment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetPressureSegmentList(Guid? parameters, AsyncCallback callback, object state);
        List<PressureSegmentDTO> EndGetPressureSegmentList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddPressureSegment(AddPressureSegmentParameterSet parameters, AsyncCallback callback, object state);
        int EndAddPressureSegment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditPressureSegment(EditPressureSegmentParameterSet parameters, AsyncCallback callback, object state);
        void EndEditPressureSegment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginDeletePressureSegment(int parameters, AsyncCallback callback, object state);
        void EndDeletePressureSegment(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetValveList(GetValveListParameterSet parameters, AsyncCallback callback, object state);
        List<ValveDTO> EndGetValveList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetValveById(Guid parameters, AsyncCallback callback, object state);
        ValveDTO EndGetValveById(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddValve(AddValveParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddValve(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditValve(EditValveParameterSet parameters, AsyncCallback callback, object state);
        void EndEditValve(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginValidate(object parameters, AsyncCallback callback, object state);
        void EndValidate(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetInconsistencies(Guid? parameters, AsyncCallback callback, object state);
        Dictionary<Guid, List<InconsistencyDTO>> EndGetInconsistencies(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetEntityTypeProperties(EntityType? parameters, AsyncCallback callback, object state);
        List<EntityTypePropertyDTO> EndGetEntityTypeProperties(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddPowerUnitType(AddPowerUnitTypeParameterSet parameters, AsyncCallback callback, object state);
        int EndAddPowerUnitType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditPowerUnitType(EditPowerUnitTypeParameterSet parameters, AsyncCallback callback, object state);
        void EndEditPowerUnitType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemovePowerUnitType(int parameters, AsyncCallback callback, object state);
        void EndRemovePowerUnitType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetPowerUnitTypes(object parameters, AsyncCallback callback, object state);
        List<PowerUnitTypeDTO> EndGetPowerUnitTypes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddBoilerType(AddBoilerTypeParameterSet parameters, AsyncCallback callback, object state);
        int EndAddBoilerType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditBoilerType(EditBoilerTypeParameterSet parameters, AsyncCallback callback, object state);
        void EndEditBoilerType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveBoilerType(int parameters, AsyncCallback callback, object state);
        void EndRemoveBoilerType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetBoilerTypes(object parameters, AsyncCallback callback, object state);
        List<BoilerTypeDTO> EndGetBoilerTypes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetAggregatorList(GetAggregatorListParameterSet parameters, AsyncCallback callback, object state);
        List<AggregatorDTO> EndGetAggregatorList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetAggregatorById(Guid parameters, AsyncCallback callback, object state);
        AggregatorDTO EndGetAggregatorById(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddAggregator(AddAggregatorParameterSet parameters, AsyncCallback callback, object state);
        Guid EndAddAggregator(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditAggregator(EditAggregatorParameterSet parameters, AsyncCallback callback, object state);
        void EndEditAggregator(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddRegulatorType(AddRegulatorTypeParameterSet parameters, AsyncCallback callback, object state);
        int EndAddRegulatorType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditRegulatorType(EditRegulatorTypeParameterSet parameters, AsyncCallback callback, object state);
        void EndEditRegulatorType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveRegulatorType(int parameters, AsyncCallback callback, object state);
        void EndRemoveRegulatorType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetRegulatorTypes(object parameters, AsyncCallback callback, object state);
        List<RegulatorTypeDTO> EndGetRegulatorTypes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddHeaterType(AddHeaterTypeParameterSet parameters, AsyncCallback callback, object state);
        int EndAddHeaterType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditHeaterType(EditHeaterTypeParameterSet parameters, AsyncCallback callback, object state);
        void EndEditHeaterType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveHeaterType(int parameters, AsyncCallback callback, object state);
        void EndRemoveHeaterType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetHeaterTypes(object parameters, AsyncCallback callback, object state);
        List<HeaterTypeDTO> EndGetHeaterTypes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginAddEmergencyValveType(AddEmergencyValveTypeParameterSet parameters, AsyncCallback callback, object state);
        int EndAddEmergencyValveType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginEditEmergencyValveType(EditEmergencyValveTypeParameterSet parameters, AsyncCallback callback, object state);
        void EndEditEmergencyValveType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginRemoveEmergencyValveType(int parameters, AsyncCallback callback, object state);
        void EndRemoveEmergencyValveType(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetEmergencyValveTypes(object parameters, AsyncCallback callback, object state);
        List<EmergencyValveTypeDTO> EndGetEmergencyValveTypes(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetChangeLog(GetChangeLogParameterSet parameters, AsyncCallback callback, object state);
        List<ChangeDTO> EndGetChangeLog(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginSetIsInputOff(SetIsInputOffParameterSet parameters, AsyncCallback callback, object state);
        void EndSetIsInputOff(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetDistrStationList(GetDistrStationListParameterSet parameters, AsyncCallback callback, object state);
        List<DistrStationDTO> EndGetDistrStationList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetBoilerList(GetBoilerListParameterSet parameters, AsyncCallback callback, object state);
        List<BoilerDTO> EndGetBoilerList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetPowerUnitList(GetPowerUnitListParameterSet parameters, AsyncCallback callback, object state);
        List<PowerUnitDTO> EndGetPowerUnitList(IAsyncResult result);

        [OperationContract(AsyncPattern = true)]
        [FaultContract(typeof(FaultDetail))]
        IAsyncResult BeginGetBoilerPlantList(int? parameters, AsyncCallback callback, object state);
        List<BoilerPlantDTO> EndGetBoilerPlantList(IAsyncResult result);
    }

	public interface IObjectModelServiceProxy
	{

        Task<EntitiesPageDTO> GetEntitiesPageAsync(GetEntitesPageParameterSet parameters);

        Task<List<CommonEntityDTO>> GetEntityListAsync(GetEntityListParameterSet parameters);

        Task<CommonEntityDTO> GetEntityByIdAsync(Guid parameters);

        Task DeleteEntityAsync(DeleteEntityParameterSet parameters);

        Task SetSortOrderAsync(SetSortOrderParameterSet parameters);

        Task AddDescriptionAsync(AddDescriptionParameterSet parameters);

        Task<List<EntityChangeDTO>> GetEntityChangeListAsync(Guid parameters);

        Task<List<AttachmentDTO<int, Guid>>> GetEntityAttachmentListAsync(Guid? parameters);

        Task<int> AddEntityAttachmentAsync(AddAttachmentParameterSet<Guid> parameters);

        Task RemoveEntityAttachmentAsync(int parameters);

        Task<List<EntityUrlDTO>> GetEntityUrlListAsync(Guid? parameters);

        Task<int> AddEntityUrlAsync(AddEntityUrlParameterSet parameters);

        Task EditEntityUrlAsync(EditEntityUrlParameterSet parameters);

        Task RemoveEntityUrlAsync(int parameters);

        Task<TreeDataDTO> GetFullTreeAsync(EntityTreeGetParameterSet parameters);

        Task<List<EnterpriseDTO>> GetEnterpriseListAsync();

        Task<List<CommonEntityDTO>> GetCurrentEnterpriseAndSitesAsync();

        Task<List<SiteDTO>> GetSiteListAsync(GetSiteListParameterSet parameters);

        Task<Guid?> FindSiteAsync(Guid parameters);

        Task<Guid> AddSiteAsync(AddSiteParameterSet parameters);

        Task EditSiteAsync(EditSiteParameterSet parameters);

        Task<List<CompStationDTO>> GetCompStationListAsync(GetCompStationListParameterSet parameters);

        Task<TreeDataDTO> GetCompStationTreeAsync(Guid? parameters);

        Task<Guid> AddCompStationAsync(AddCompStationParameterSet parameters);

        Task EditCompStationAsync(EditCompStationParameterSet parameters);

        Task<List<CompStationCoolingRecomendedDTO>> GetCompStationCoolingRecomendedListAsync(Guid parameters);

        Task SetCompStationCoolingRecomendedAsync(SetCompStationCoolingRecomendedParameterSet parameters);

        Task<List<CompShopDTO>> GetCompShopListAsync(GetCompShopListParameterSet parameters);

        Task<Guid> AddCompShopAsync(AddCompShopParameterSet parameters);

        Task EditCompShopAsync(EditCompShopParameterSet parameters);

        Task<CompUnitDTO> GetCompUnitByIdAsync(Guid parameters);

        Task<List<CompUnitDTO>> GetCompUnitListAsync(GetCompUnitListParameterSet parameters);

        Task<Guid> AddCompUnitAsync(AddCompUnitParameterSet parameters);

        Task EditCompUnitAsync(EditCompUnitParameterSet parameters);

        Task<Guid> AddBoilerPlantAsync(AddBoilerPlantParameterSet parameters);

        Task EditBoilerPlantAsync(EditBoilerPlantParameterSet parameters);

        Task<BoilerDTO> GetBoilerByIdAsync(Guid parameters);

        Task<Guid> AddBoilerAsync(AddBoilerParameterSet parameters);

        Task EditBoilerAsync(EditBoilerParameterSet parameters);

        Task<Guid> AddPowerPlantAsync(AddPowerPlantParameterSet parameters);

        Task EditPowerPlantAsync(EditPowerPlantParameterSet parameters);

        Task<PowerUnitDTO> GetPowerUnitByIdAsync(Guid parameters);

        Task<Guid> AddPowerUnitAsync(AddPowerUnitParameterSet parameters);

        Task EditPowerUnitAsync(EditPowerUnitParameterSet parameters);

        Task<Guid> AddCoolingStationAsync(AddCoolingStationParameterSet parameters);

        Task EditCoolingStationAsync(EditCoolingStationParameterSet parameters);

        Task<List<CoolingUnitDTO>> GetCoolingUnitListAsync(GetCoolingUnitListParameterSet parameters);

        Task<CoolingUnitDTO> GetCoolingUnitByIdAsync(Guid parameters);

        Task<Guid> AddCoolingUnitAsync(AddCoolingUnitParameterSet parameters);

        Task EditCoolingUnitAsync(EditCoolingUnitParameterSet parameters);

        Task<TreeDataDTO> GetDistrStationTreeAsync(GetDistrStationListParameterSet parameters);

        Task<Guid> AddDistrStationAsync(AddDistrStationParameterSet parameters);

        Task EditDistrStationAsync(EditDistrStationParameterSet parameters);

        Task<List<DistrStationOutletDTO>> GetDistrStationOutletListAsync(GetDistrStationOutletListParameterSet parameters);

        Task<Guid> AddDistrStationOutletAsync(AddDistrStationOutletParameterSet parameters);

        Task EditDistrStationOutletAsync(EditDistrStationOutletParameterSet parameters);

        Task<List<ConsumerDTO>> GetConsumerListAsync(GetConsumerListParameterSet parameters);

        Task<Guid> AddConsumerAsync(AddConsumerParameterSet parameters);

        Task EditConsumerAsync(EditConsumerParameterSet parameters);

        Task<List<OperConsumerDTO>> GetOperConsumersAsync(GetOperConsumerListParameterSet parameters);

        Task<Guid> AddOperConsumerAsync(AddEditOperConsumerParameterSet parameters);

        Task EditOperConsumerAsync(AddEditOperConsumerParameterSet parameters);

        Task<List<MeasStationDTO>> GetMeasStationListAsync(GetMeasStationListParameterSet parameters);

        Task<TreeDataDTO> GetMeasStationTreeAsync(GetMeasStationListParameterSet parameters);

        Task<Guid> AddMeasStationAsync(AddMeasStationParameterSet parameters);

        Task EditMeasStationAsync(EditMeasStationParameterSet parameters);

        Task<List<MeasLineDTO>> GetMeasLineListAsync(GetMeasLineListParameterSet parameters);

        Task<Guid> AddMeasLineAsync(AddMeasLineParameterSet parameters);

        Task EditMeasLineAsync(EditMeasLineParameterSet parameters);

        Task<List<ReducingStationDTO>> GetReducingStationListAsync(GetReducingStationListParameterSet parameters);

        Task<Guid> AddReducingStationAsync(AddReducingStationParameterSet parameters);

        Task EditReducingStationAsync(EditReducingStationParameterSet parameters);

        Task<MeasPointDTO> GetMeasPointByParentAsync(Guid parameters);

        Task<Guid> AddMeasPointAsync(AddMeasPointParameterSet parameters);

        Task EditMeasPointAsync(EditMeasPointParameterSet parameters);

        Task<MeasPointDTO> FindMeasPointAsync(Guid parameters);

        Task<TreeDataDTO> GetPipelineTreeAsync(Guid? parameters);

        Task<List<PipelineDTO>> GetPipelineListAsync(GetPipelineListParameterSet parameters);

        Task<PipelineDTO> GetPipelineByIdAsync(Guid parameters);

        Task<Guid> AddPipelineAsync(AddPipelineWithConnsParameterSet parameters);

        Task EditPipelineAsync(EditPipelineWithConnsParameterSet parameters);

        Task<List<SiteSegmentDTO>> GetSiteSegmentListAsync(Guid? parameters);

        Task<int> AddSiteSegmentAsync(AddSiteSegmentParameterSet parameters);

        Task EditSiteSegmentAsync(EditSiteSegmentParameterSet parameters);

        Task DeleteSiteSegmentAsync(int parameters);

        Task<List<RegionSegmentDTO>> GetRegionSegmentListAsync(Guid? parameters);

        Task<int> AddRegionSegmentAsync(AddRegionSegmentParameterSet parameters);

        Task EditRegionSegmentAsync(EditRegionSegmentParameterSet parameters);

        Task DeleteRegionSegmentAsync(int parameters);

        Task<List<GroupSegmentDTO>> GetGroupSegmentListAsync(Guid? parameters);

        Task<int> AddGroupSegmentAsync(AddGroupSegmentParameterSet parameters);

        Task EditGroupSegmentAsync(EditGroupSegmentParameterSet parameters);

        Task DeleteGroupSegmentAsync(int parameters);

        Task<List<DiameterSegmentDTO>> GetDiameterSegmentListAsync(Guid? parameters);

        Task<int> AddDiameterSegmentAsync(AddDiameterSegmentParameterSet parameters);

        Task EditDiameterSegmentAsync(EditDiameterSegmentParameterSet parameters);

        Task DeleteDiameterSegmentAsync(int parameters);

        Task<List<PressureSegmentDTO>> GetPressureSegmentListAsync(Guid? parameters);

        Task<int> AddPressureSegmentAsync(AddPressureSegmentParameterSet parameters);

        Task EditPressureSegmentAsync(EditPressureSegmentParameterSet parameters);

        Task DeletePressureSegmentAsync(int parameters);

        Task<List<ValveDTO>> GetValveListAsync(GetValveListParameterSet parameters);

        Task<ValveDTO> GetValveByIdAsync(Guid parameters);

        Task<Guid> AddValveAsync(AddValveParameterSet parameters);

        Task EditValveAsync(EditValveParameterSet parameters);

        Task ValidateAsync();

        Task<Dictionary<Guid, List<InconsistencyDTO>>> GetInconsistenciesAsync(Guid? parameters);

        Task<List<EntityTypePropertyDTO>> GetEntityTypePropertiesAsync(EntityType? parameters);

        Task<int> AddPowerUnitTypeAsync(AddPowerUnitTypeParameterSet parameters);

        Task EditPowerUnitTypeAsync(EditPowerUnitTypeParameterSet parameters);

        Task RemovePowerUnitTypeAsync(int parameters);

        Task<List<PowerUnitTypeDTO>> GetPowerUnitTypesAsync();

        Task<int> AddBoilerTypeAsync(AddBoilerTypeParameterSet parameters);

        Task EditBoilerTypeAsync(EditBoilerTypeParameterSet parameters);

        Task RemoveBoilerTypeAsync(int parameters);

        Task<List<BoilerTypeDTO>> GetBoilerTypesAsync();

        Task<List<AggregatorDTO>> GetAggregatorListAsync(GetAggregatorListParameterSet parameters);

        Task<AggregatorDTO> GetAggregatorByIdAsync(Guid parameters);

        Task<Guid> AddAggregatorAsync(AddAggregatorParameterSet parameters);

        Task EditAggregatorAsync(EditAggregatorParameterSet parameters);

        Task<int> AddRegulatorTypeAsync(AddRegulatorTypeParameterSet parameters);

        Task EditRegulatorTypeAsync(EditRegulatorTypeParameterSet parameters);

        Task RemoveRegulatorTypeAsync(int parameters);

        Task<List<RegulatorTypeDTO>> GetRegulatorTypesAsync();

        Task<int> AddHeaterTypeAsync(AddHeaterTypeParameterSet parameters);

        Task EditHeaterTypeAsync(EditHeaterTypeParameterSet parameters);

        Task RemoveHeaterTypeAsync(int parameters);

        Task<List<HeaterTypeDTO>> GetHeaterTypesAsync();

        Task<int> AddEmergencyValveTypeAsync(AddEmergencyValveTypeParameterSet parameters);

        Task EditEmergencyValveTypeAsync(EditEmergencyValveTypeParameterSet parameters);

        Task RemoveEmergencyValveTypeAsync(int parameters);

        Task<List<EmergencyValveTypeDTO>> GetEmergencyValveTypesAsync();

        Task<List<ChangeDTO>> GetChangeLogAsync(GetChangeLogParameterSet parameters);

        Task SetIsInputOffAsync(SetIsInputOffParameterSet parameters);

        Task<List<DistrStationDTO>> GetDistrStationListAsync(GetDistrStationListParameterSet parameters);

        Task<List<BoilerDTO>> GetBoilerListAsync(GetBoilerListParameterSet parameters);

        Task<List<PowerUnitDTO>> GetPowerUnitListAsync(GetPowerUnitListParameterSet parameters);

        Task<List<BoilerPlantDTO>> GetBoilerPlantListAsync(int? parameters);

    }

    public sealed class ObjectModelServiceProxy : DataProviderBase<IObjectModelService>, IObjectModelServiceProxy
	{
        protected override string ServiceUri => "/ObjectModel/ObjectModelService.svc";
      


        public Task<EntitiesPageDTO> GetEntitiesPageAsync(GetEntitesPageParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<EntitiesPageDTO,GetEntitesPageParameterSet>(channel, channel.BeginGetEntitiesPage, channel.EndGetEntitiesPage, parameters);
        }

        public Task<List<CommonEntityDTO>> GetEntityListAsync(GetEntityListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<CommonEntityDTO>,GetEntityListParameterSet>(channel, channel.BeginGetEntityList, channel.EndGetEntityList, parameters);
        }

        public Task<CommonEntityDTO> GetEntityByIdAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<CommonEntityDTO,Guid>(channel, channel.BeginGetEntityById, channel.EndGetEntityById, parameters);
        }

        public Task DeleteEntityAsync(DeleteEntityParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteEntity, channel.EndDeleteEntity, parameters);
        }

        public Task SetSortOrderAsync(SetSortOrderParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetSortOrder, channel.EndSetSortOrder, parameters);
        }

        public Task AddDescriptionAsync(AddDescriptionParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginAddDescription, channel.EndAddDescription, parameters);
        }

        public Task<List<EntityChangeDTO>> GetEntityChangeListAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<EntityChangeDTO>,Guid>(channel, channel.BeginGetEntityChangeList, channel.EndGetEntityChangeList, parameters);
        }

        public Task<List<AttachmentDTO<int, Guid>>> GetEntityAttachmentListAsync(Guid? parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<AttachmentDTO<int, Guid>>,Guid?>(channel, channel.BeginGetEntityAttachmentList, channel.EndGetEntityAttachmentList, parameters);
        }

        public Task<int> AddEntityAttachmentAsync(AddAttachmentParameterSet<Guid> parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddAttachmentParameterSet<Guid>>(channel, channel.BeginAddEntityAttachment, channel.EndAddEntityAttachment, parameters);
        }

        public Task RemoveEntityAttachmentAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveEntityAttachment, channel.EndRemoveEntityAttachment, parameters);
        }

        public Task<List<EntityUrlDTO>> GetEntityUrlListAsync(Guid? parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<EntityUrlDTO>,Guid?>(channel, channel.BeginGetEntityUrlList, channel.EndGetEntityUrlList, parameters);
        }

        public Task<int> AddEntityUrlAsync(AddEntityUrlParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddEntityUrlParameterSet>(channel, channel.BeginAddEntityUrl, channel.EndAddEntityUrl, parameters);
        }

        public Task EditEntityUrlAsync(EditEntityUrlParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditEntityUrl, channel.EndEditEntityUrl, parameters);
        }

        public Task RemoveEntityUrlAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveEntityUrl, channel.EndRemoveEntityUrl, parameters);
        }

        public Task<TreeDataDTO> GetFullTreeAsync(EntityTreeGetParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<TreeDataDTO,EntityTreeGetParameterSet>(channel, channel.BeginGetFullTree, channel.EndGetFullTree, parameters);
        }

        public Task<List<EnterpriseDTO>> GetEnterpriseListAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<EnterpriseDTO>>(channel, channel.BeginGetEnterpriseList, channel.EndGetEnterpriseList);
        }

        public Task<List<CommonEntityDTO>> GetCurrentEnterpriseAndSitesAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<CommonEntityDTO>>(channel, channel.BeginGetCurrentEnterpriseAndSites, channel.EndGetCurrentEnterpriseAndSites);
        }

        public Task<List<SiteDTO>> GetSiteListAsync(GetSiteListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<SiteDTO>,GetSiteListParameterSet>(channel, channel.BeginGetSiteList, channel.EndGetSiteList, parameters);
        }

        public Task<Guid?> FindSiteAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid?,Guid>(channel, channel.BeginFindSite, channel.EndFindSite, parameters);
        }

        public Task<Guid> AddSiteAsync(AddSiteParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddSiteParameterSet>(channel, channel.BeginAddSite, channel.EndAddSite, parameters);
        }

        public Task EditSiteAsync(EditSiteParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditSite, channel.EndEditSite, parameters);
        }

        public Task<List<CompStationDTO>> GetCompStationListAsync(GetCompStationListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<CompStationDTO>,GetCompStationListParameterSet>(channel, channel.BeginGetCompStationList, channel.EndGetCompStationList, parameters);
        }

        public Task<TreeDataDTO> GetCompStationTreeAsync(Guid? parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<TreeDataDTO,Guid?>(channel, channel.BeginGetCompStationTree, channel.EndGetCompStationTree, parameters);
        }

        public Task<Guid> AddCompStationAsync(AddCompStationParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddCompStationParameterSet>(channel, channel.BeginAddCompStation, channel.EndAddCompStation, parameters);
        }

        public Task EditCompStationAsync(EditCompStationParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditCompStation, channel.EndEditCompStation, parameters);
        }

        public Task<List<CompStationCoolingRecomendedDTO>> GetCompStationCoolingRecomendedListAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<CompStationCoolingRecomendedDTO>,Guid>(channel, channel.BeginGetCompStationCoolingRecomendedList, channel.EndGetCompStationCoolingRecomendedList, parameters);
        }

        public Task SetCompStationCoolingRecomendedAsync(SetCompStationCoolingRecomendedParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetCompStationCoolingRecomended, channel.EndSetCompStationCoolingRecomended, parameters);
        }

        public Task<List<CompShopDTO>> GetCompShopListAsync(GetCompShopListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<CompShopDTO>,GetCompShopListParameterSet>(channel, channel.BeginGetCompShopList, channel.EndGetCompShopList, parameters);
        }

        public Task<Guid> AddCompShopAsync(AddCompShopParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddCompShopParameterSet>(channel, channel.BeginAddCompShop, channel.EndAddCompShop, parameters);
        }

        public Task EditCompShopAsync(EditCompShopParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditCompShop, channel.EndEditCompShop, parameters);
        }

        public Task<CompUnitDTO> GetCompUnitByIdAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<CompUnitDTO,Guid>(channel, channel.BeginGetCompUnitById, channel.EndGetCompUnitById, parameters);
        }

        public Task<List<CompUnitDTO>> GetCompUnitListAsync(GetCompUnitListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<CompUnitDTO>,GetCompUnitListParameterSet>(channel, channel.BeginGetCompUnitList, channel.EndGetCompUnitList, parameters);
        }

        public Task<Guid> AddCompUnitAsync(AddCompUnitParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddCompUnitParameterSet>(channel, channel.BeginAddCompUnit, channel.EndAddCompUnit, parameters);
        }

        public Task EditCompUnitAsync(EditCompUnitParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditCompUnit, channel.EndEditCompUnit, parameters);
        }

        public Task<Guid> AddBoilerPlantAsync(AddBoilerPlantParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddBoilerPlantParameterSet>(channel, channel.BeginAddBoilerPlant, channel.EndAddBoilerPlant, parameters);
        }

        public Task EditBoilerPlantAsync(EditBoilerPlantParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditBoilerPlant, channel.EndEditBoilerPlant, parameters);
        }

        public Task<BoilerDTO> GetBoilerByIdAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<BoilerDTO,Guid>(channel, channel.BeginGetBoilerById, channel.EndGetBoilerById, parameters);
        }

        public Task<Guid> AddBoilerAsync(AddBoilerParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddBoilerParameterSet>(channel, channel.BeginAddBoiler, channel.EndAddBoiler, parameters);
        }

        public Task EditBoilerAsync(EditBoilerParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditBoiler, channel.EndEditBoiler, parameters);
        }

        public Task<Guid> AddPowerPlantAsync(AddPowerPlantParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddPowerPlantParameterSet>(channel, channel.BeginAddPowerPlant, channel.EndAddPowerPlant, parameters);
        }

        public Task EditPowerPlantAsync(EditPowerPlantParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditPowerPlant, channel.EndEditPowerPlant, parameters);
        }

        public Task<PowerUnitDTO> GetPowerUnitByIdAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<PowerUnitDTO,Guid>(channel, channel.BeginGetPowerUnitById, channel.EndGetPowerUnitById, parameters);
        }

        public Task<Guid> AddPowerUnitAsync(AddPowerUnitParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddPowerUnitParameterSet>(channel, channel.BeginAddPowerUnit, channel.EndAddPowerUnit, parameters);
        }

        public Task EditPowerUnitAsync(EditPowerUnitParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditPowerUnit, channel.EndEditPowerUnit, parameters);
        }

        public Task<Guid> AddCoolingStationAsync(AddCoolingStationParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddCoolingStationParameterSet>(channel, channel.BeginAddCoolingStation, channel.EndAddCoolingStation, parameters);
        }

        public Task EditCoolingStationAsync(EditCoolingStationParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditCoolingStation, channel.EndEditCoolingStation, parameters);
        }

        public Task<List<CoolingUnitDTO>> GetCoolingUnitListAsync(GetCoolingUnitListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<CoolingUnitDTO>,GetCoolingUnitListParameterSet>(channel, channel.BeginGetCoolingUnitList, channel.EndGetCoolingUnitList, parameters);
        }

        public Task<CoolingUnitDTO> GetCoolingUnitByIdAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<CoolingUnitDTO,Guid>(channel, channel.BeginGetCoolingUnitById, channel.EndGetCoolingUnitById, parameters);
        }

        public Task<Guid> AddCoolingUnitAsync(AddCoolingUnitParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddCoolingUnitParameterSet>(channel, channel.BeginAddCoolingUnit, channel.EndAddCoolingUnit, parameters);
        }

        public Task EditCoolingUnitAsync(EditCoolingUnitParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditCoolingUnit, channel.EndEditCoolingUnit, parameters);
        }

        public Task<TreeDataDTO> GetDistrStationTreeAsync(GetDistrStationListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<TreeDataDTO,GetDistrStationListParameterSet>(channel, channel.BeginGetDistrStationTree, channel.EndGetDistrStationTree, parameters);
        }

        public Task<Guid> AddDistrStationAsync(AddDistrStationParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddDistrStationParameterSet>(channel, channel.BeginAddDistrStation, channel.EndAddDistrStation, parameters);
        }

        public Task EditDistrStationAsync(EditDistrStationParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditDistrStation, channel.EndEditDistrStation, parameters);
        }

        public Task<List<DistrStationOutletDTO>> GetDistrStationOutletListAsync(GetDistrStationOutletListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DistrStationOutletDTO>,GetDistrStationOutletListParameterSet>(channel, channel.BeginGetDistrStationOutletList, channel.EndGetDistrStationOutletList, parameters);
        }

        public Task<Guid> AddDistrStationOutletAsync(AddDistrStationOutletParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddDistrStationOutletParameterSet>(channel, channel.BeginAddDistrStationOutlet, channel.EndAddDistrStationOutlet, parameters);
        }

        public Task EditDistrStationOutletAsync(EditDistrStationOutletParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditDistrStationOutlet, channel.EndEditDistrStationOutlet, parameters);
        }

        public Task<List<ConsumerDTO>> GetConsumerListAsync(GetConsumerListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<ConsumerDTO>,GetConsumerListParameterSet>(channel, channel.BeginGetConsumerList, channel.EndGetConsumerList, parameters);
        }

        public Task<Guid> AddConsumerAsync(AddConsumerParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddConsumerParameterSet>(channel, channel.BeginAddConsumer, channel.EndAddConsumer, parameters);
        }

        public Task EditConsumerAsync(EditConsumerParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditConsumer, channel.EndEditConsumer, parameters);
        }

        public Task<List<OperConsumerDTO>> GetOperConsumersAsync(GetOperConsumerListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<OperConsumerDTO>,GetOperConsumerListParameterSet>(channel, channel.BeginGetOperConsumers, channel.EndGetOperConsumers, parameters);
        }

        public Task<Guid> AddOperConsumerAsync(AddEditOperConsumerParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddEditOperConsumerParameterSet>(channel, channel.BeginAddOperConsumer, channel.EndAddOperConsumer, parameters);
        }

        public Task EditOperConsumerAsync(AddEditOperConsumerParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditOperConsumer, channel.EndEditOperConsumer, parameters);
        }

        public Task<List<MeasStationDTO>> GetMeasStationListAsync(GetMeasStationListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<MeasStationDTO>,GetMeasStationListParameterSet>(channel, channel.BeginGetMeasStationList, channel.EndGetMeasStationList, parameters);
        }

        public Task<TreeDataDTO> GetMeasStationTreeAsync(GetMeasStationListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<TreeDataDTO,GetMeasStationListParameterSet>(channel, channel.BeginGetMeasStationTree, channel.EndGetMeasStationTree, parameters);
        }

        public Task<Guid> AddMeasStationAsync(AddMeasStationParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddMeasStationParameterSet>(channel, channel.BeginAddMeasStation, channel.EndAddMeasStation, parameters);
        }

        public Task EditMeasStationAsync(EditMeasStationParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditMeasStation, channel.EndEditMeasStation, parameters);
        }

        public Task<List<MeasLineDTO>> GetMeasLineListAsync(GetMeasLineListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<MeasLineDTO>,GetMeasLineListParameterSet>(channel, channel.BeginGetMeasLineList, channel.EndGetMeasLineList, parameters);
        }

        public Task<Guid> AddMeasLineAsync(AddMeasLineParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddMeasLineParameterSet>(channel, channel.BeginAddMeasLine, channel.EndAddMeasLine, parameters);
        }

        public Task EditMeasLineAsync(EditMeasLineParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditMeasLine, channel.EndEditMeasLine, parameters);
        }

        public Task<List<ReducingStationDTO>> GetReducingStationListAsync(GetReducingStationListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<ReducingStationDTO>,GetReducingStationListParameterSet>(channel, channel.BeginGetReducingStationList, channel.EndGetReducingStationList, parameters);
        }

        public Task<Guid> AddReducingStationAsync(AddReducingStationParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddReducingStationParameterSet>(channel, channel.BeginAddReducingStation, channel.EndAddReducingStation, parameters);
        }

        public Task EditReducingStationAsync(EditReducingStationParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditReducingStation, channel.EndEditReducingStation, parameters);
        }

        public Task<MeasPointDTO> GetMeasPointByParentAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<MeasPointDTO,Guid>(channel, channel.BeginGetMeasPointByParent, channel.EndGetMeasPointByParent, parameters);
        }

        public Task<Guid> AddMeasPointAsync(AddMeasPointParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddMeasPointParameterSet>(channel, channel.BeginAddMeasPoint, channel.EndAddMeasPoint, parameters);
        }

        public Task EditMeasPointAsync(EditMeasPointParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditMeasPoint, channel.EndEditMeasPoint, parameters);
        }

        public Task<MeasPointDTO> FindMeasPointAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<MeasPointDTO,Guid>(channel, channel.BeginFindMeasPoint, channel.EndFindMeasPoint, parameters);
        }

        public Task<TreeDataDTO> GetPipelineTreeAsync(Guid? parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<TreeDataDTO,Guid?>(channel, channel.BeginGetPipelineTree, channel.EndGetPipelineTree, parameters);
        }

        public Task<List<PipelineDTO>> GetPipelineListAsync(GetPipelineListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<PipelineDTO>,GetPipelineListParameterSet>(channel, channel.BeginGetPipelineList, channel.EndGetPipelineList, parameters);
        }

        public Task<PipelineDTO> GetPipelineByIdAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<PipelineDTO,Guid>(channel, channel.BeginGetPipelineById, channel.EndGetPipelineById, parameters);
        }

        public Task<Guid> AddPipelineAsync(AddPipelineWithConnsParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddPipelineWithConnsParameterSet>(channel, channel.BeginAddPipeline, channel.EndAddPipeline, parameters);
        }

        public Task EditPipelineAsync(EditPipelineWithConnsParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditPipeline, channel.EndEditPipeline, parameters);
        }

        public Task<List<SiteSegmentDTO>> GetSiteSegmentListAsync(Guid? parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<SiteSegmentDTO>,Guid?>(channel, channel.BeginGetSiteSegmentList, channel.EndGetSiteSegmentList, parameters);
        }

        public Task<int> AddSiteSegmentAsync(AddSiteSegmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddSiteSegmentParameterSet>(channel, channel.BeginAddSiteSegment, channel.EndAddSiteSegment, parameters);
        }

        public Task EditSiteSegmentAsync(EditSiteSegmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditSiteSegment, channel.EndEditSiteSegment, parameters);
        }

        public Task DeleteSiteSegmentAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteSiteSegment, channel.EndDeleteSiteSegment, parameters);
        }

        public Task<List<RegionSegmentDTO>> GetRegionSegmentListAsync(Guid? parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<RegionSegmentDTO>,Guid?>(channel, channel.BeginGetRegionSegmentList, channel.EndGetRegionSegmentList, parameters);
        }

        public Task<int> AddRegionSegmentAsync(AddRegionSegmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddRegionSegmentParameterSet>(channel, channel.BeginAddRegionSegment, channel.EndAddRegionSegment, parameters);
        }

        public Task EditRegionSegmentAsync(EditRegionSegmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditRegionSegment, channel.EndEditRegionSegment, parameters);
        }

        public Task DeleteRegionSegmentAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteRegionSegment, channel.EndDeleteRegionSegment, parameters);
        }

        public Task<List<GroupSegmentDTO>> GetGroupSegmentListAsync(Guid? parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<GroupSegmentDTO>,Guid?>(channel, channel.BeginGetGroupSegmentList, channel.EndGetGroupSegmentList, parameters);
        }

        public Task<int> AddGroupSegmentAsync(AddGroupSegmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddGroupSegmentParameterSet>(channel, channel.BeginAddGroupSegment, channel.EndAddGroupSegment, parameters);
        }

        public Task EditGroupSegmentAsync(EditGroupSegmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditGroupSegment, channel.EndEditGroupSegment, parameters);
        }

        public Task DeleteGroupSegmentAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteGroupSegment, channel.EndDeleteGroupSegment, parameters);
        }

        public Task<List<DiameterSegmentDTO>> GetDiameterSegmentListAsync(Guid? parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DiameterSegmentDTO>,Guid?>(channel, channel.BeginGetDiameterSegmentList, channel.EndGetDiameterSegmentList, parameters);
        }

        public Task<int> AddDiameterSegmentAsync(AddDiameterSegmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddDiameterSegmentParameterSet>(channel, channel.BeginAddDiameterSegment, channel.EndAddDiameterSegment, parameters);
        }

        public Task EditDiameterSegmentAsync(EditDiameterSegmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditDiameterSegment, channel.EndEditDiameterSegment, parameters);
        }

        public Task DeleteDiameterSegmentAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeleteDiameterSegment, channel.EndDeleteDiameterSegment, parameters);
        }

        public Task<List<PressureSegmentDTO>> GetPressureSegmentListAsync(Guid? parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<PressureSegmentDTO>,Guid?>(channel, channel.BeginGetPressureSegmentList, channel.EndGetPressureSegmentList, parameters);
        }

        public Task<int> AddPressureSegmentAsync(AddPressureSegmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddPressureSegmentParameterSet>(channel, channel.BeginAddPressureSegment, channel.EndAddPressureSegment, parameters);
        }

        public Task EditPressureSegmentAsync(EditPressureSegmentParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditPressureSegment, channel.EndEditPressureSegment, parameters);
        }

        public Task DeletePressureSegmentAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginDeletePressureSegment, channel.EndDeletePressureSegment, parameters);
        }

        public Task<List<ValveDTO>> GetValveListAsync(GetValveListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<ValveDTO>,GetValveListParameterSet>(channel, channel.BeginGetValveList, channel.EndGetValveList, parameters);
        }

        public Task<ValveDTO> GetValveByIdAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<ValveDTO,Guid>(channel, channel.BeginGetValveById, channel.EndGetValveById, parameters);
        }

        public Task<Guid> AddValveAsync(AddValveParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddValveParameterSet>(channel, channel.BeginAddValve, channel.EndAddValve, parameters);
        }

        public Task EditValveAsync(EditValveParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditValve, channel.EndEditValve, parameters);
        }

        public Task ValidateAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginValidate, channel.EndValidate);
        }

        public Task<Dictionary<Guid, List<InconsistencyDTO>>> GetInconsistenciesAsync(Guid? parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Dictionary<Guid, List<InconsistencyDTO>>,Guid?>(channel, channel.BeginGetInconsistencies, channel.EndGetInconsistencies, parameters);
        }

        public Task<List<EntityTypePropertyDTO>> GetEntityTypePropertiesAsync(EntityType? parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<EntityTypePropertyDTO>,EntityType?>(channel, channel.BeginGetEntityTypeProperties, channel.EndGetEntityTypeProperties, parameters);
        }

        public Task<int> AddPowerUnitTypeAsync(AddPowerUnitTypeParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddPowerUnitTypeParameterSet>(channel, channel.BeginAddPowerUnitType, channel.EndAddPowerUnitType, parameters);
        }

        public Task EditPowerUnitTypeAsync(EditPowerUnitTypeParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditPowerUnitType, channel.EndEditPowerUnitType, parameters);
        }

        public Task RemovePowerUnitTypeAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemovePowerUnitType, channel.EndRemovePowerUnitType, parameters);
        }

        public Task<List<PowerUnitTypeDTO>> GetPowerUnitTypesAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<PowerUnitTypeDTO>>(channel, channel.BeginGetPowerUnitTypes, channel.EndGetPowerUnitTypes);
        }

        public Task<int> AddBoilerTypeAsync(AddBoilerTypeParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddBoilerTypeParameterSet>(channel, channel.BeginAddBoilerType, channel.EndAddBoilerType, parameters);
        }

        public Task EditBoilerTypeAsync(EditBoilerTypeParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditBoilerType, channel.EndEditBoilerType, parameters);
        }

        public Task RemoveBoilerTypeAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveBoilerType, channel.EndRemoveBoilerType, parameters);
        }

        public Task<List<BoilerTypeDTO>> GetBoilerTypesAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<BoilerTypeDTO>>(channel, channel.BeginGetBoilerTypes, channel.EndGetBoilerTypes);
        }

        public Task<List<AggregatorDTO>> GetAggregatorListAsync(GetAggregatorListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<AggregatorDTO>,GetAggregatorListParameterSet>(channel, channel.BeginGetAggregatorList, channel.EndGetAggregatorList, parameters);
        }

        public Task<AggregatorDTO> GetAggregatorByIdAsync(Guid parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<AggregatorDTO,Guid>(channel, channel.BeginGetAggregatorById, channel.EndGetAggregatorById, parameters);
        }

        public Task<Guid> AddAggregatorAsync(AddAggregatorParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<Guid,AddAggregatorParameterSet>(channel, channel.BeginAddAggregator, channel.EndAddAggregator, parameters);
        }

        public Task EditAggregatorAsync(EditAggregatorParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditAggregator, channel.EndEditAggregator, parameters);
        }

        public Task<int> AddRegulatorTypeAsync(AddRegulatorTypeParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddRegulatorTypeParameterSet>(channel, channel.BeginAddRegulatorType, channel.EndAddRegulatorType, parameters);
        }

        public Task EditRegulatorTypeAsync(EditRegulatorTypeParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditRegulatorType, channel.EndEditRegulatorType, parameters);
        }

        public Task RemoveRegulatorTypeAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveRegulatorType, channel.EndRemoveRegulatorType, parameters);
        }

        public Task<List<RegulatorTypeDTO>> GetRegulatorTypesAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<RegulatorTypeDTO>>(channel, channel.BeginGetRegulatorTypes, channel.EndGetRegulatorTypes);
        }

        public Task<int> AddHeaterTypeAsync(AddHeaterTypeParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddHeaterTypeParameterSet>(channel, channel.BeginAddHeaterType, channel.EndAddHeaterType, parameters);
        }

        public Task EditHeaterTypeAsync(EditHeaterTypeParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditHeaterType, channel.EndEditHeaterType, parameters);
        }

        public Task RemoveHeaterTypeAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveHeaterType, channel.EndRemoveHeaterType, parameters);
        }

        public Task<List<HeaterTypeDTO>> GetHeaterTypesAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<HeaterTypeDTO>>(channel, channel.BeginGetHeaterTypes, channel.EndGetHeaterTypes);
        }

        public Task<int> AddEmergencyValveTypeAsync(AddEmergencyValveTypeParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<int,AddEmergencyValveTypeParameterSet>(channel, channel.BeginAddEmergencyValveType, channel.EndAddEmergencyValveType, parameters);
        }

        public Task EditEmergencyValveTypeAsync(EditEmergencyValveTypeParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginEditEmergencyValveType, channel.EndEditEmergencyValveType, parameters);
        }

        public Task RemoveEmergencyValveTypeAsync(int parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginRemoveEmergencyValveType, channel.EndRemoveEmergencyValveType, parameters);
        }

        public Task<List<EmergencyValveTypeDTO>> GetEmergencyValveTypesAsync()
        {
            var channel = GetChannel();
            return ExecuteAsync<List<EmergencyValveTypeDTO>>(channel, channel.BeginGetEmergencyValveTypes, channel.EndGetEmergencyValveTypes);
        }

        public Task<List<ChangeDTO>> GetChangeLogAsync(GetChangeLogParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<ChangeDTO>,GetChangeLogParameterSet>(channel, channel.BeginGetChangeLog, channel.EndGetChangeLog, parameters);
        }

        public Task SetIsInputOffAsync(SetIsInputOffParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync(channel, channel.BeginSetIsInputOff, channel.EndSetIsInputOff, parameters);
        }

        public Task<List<DistrStationDTO>> GetDistrStationListAsync(GetDistrStationListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<DistrStationDTO>,GetDistrStationListParameterSet>(channel, channel.BeginGetDistrStationList, channel.EndGetDistrStationList, parameters);
        }

        public Task<List<BoilerDTO>> GetBoilerListAsync(GetBoilerListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<BoilerDTO>,GetBoilerListParameterSet>(channel, channel.BeginGetBoilerList, channel.EndGetBoilerList, parameters);
        }

        public Task<List<PowerUnitDTO>> GetPowerUnitListAsync(GetPowerUnitListParameterSet parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<PowerUnitDTO>,GetPowerUnitListParameterSet>(channel, channel.BeginGetPowerUnitList, channel.EndGetPowerUnitList, parameters);
        }

        public Task<List<BoilerPlantDTO>> GetBoilerPlantListAsync(int? parameters)
        {
            var channel = GetChannel();
            return ExecuteAsync<List<BoilerPlantDTO>,int?>(channel, channel.BeginGetBoilerPlantList, channel.EndGetBoilerPlantList, parameters);
        }

    }
}
