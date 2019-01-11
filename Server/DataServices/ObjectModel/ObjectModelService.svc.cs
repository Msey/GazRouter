using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.Dictionaries.Enterprises;
using GazRouter.DAL.EntitySelector;
using GazRouter.DAL.ObjectModel;
using GazRouter.DAL.ObjectModel.BoilerPlants;
using GazRouter.DAL.ObjectModel.Boilers;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompStationCoolingRecomended;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.CompUnits;
using GazRouter.DAL.ObjectModel.CoolingStations;
using GazRouter.DAL.ObjectModel.CoolingUnit;
using GazRouter.DAL.ObjectModel.DistrStationOutlets;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DAL.ObjectModel.Entities;
using GazRouter.DAL.ObjectModel.Inconsistency;
using GazRouter.DAL.ObjectModel.MeasLine;
using GazRouter.DAL.ObjectModel.MeasPoint;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DAL.ObjectModel.PipelineConns;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DAL.ObjectModel.PowerPlants;
using GazRouter.DAL.ObjectModel.PowerUnits;
using GazRouter.DAL.ObjectModel.ReducingStations;
using GazRouter.DAL.ObjectModel.Segment.Diameter;
using GazRouter.DAL.ObjectModel.Segment.PipelineGroup;
using GazRouter.DAL.ObjectModel.Segment.Pressure;
using GazRouter.DAL.ObjectModel.Segment.Site;
using GazRouter.DAL.ObjectModel.Sites;
using GazRouter.DAL.ObjectModel.Valves;
using GazRouter.DataServices.BL;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DAL.Dictionaries.EntityTypes;
using GazRouter.DAL.ObjectModel.Aggregators;
using GazRouter.DAL.ObjectModel.Entities.Attachments;
using GazRouter.DAL.ObjectModel.Entities.Urls;
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
using GazRouter.DTO.ObjectModel.CoolingUnit;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.Entities;
using GazRouter.DTO.ObjectModel.Entities.Urls;
using GazRouter.DTO.ObjectModel.Inconsistency;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.PipelineConns;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.PowerPlants;
using GazRouter.DTO.ObjectModel.PowerUnits;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Segment;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.DTO.Dictionaries.PowerUnitTypes;
using GazRouter.DAL.Dictionaries.PowerUnitTypes;
using GazRouter.DTO.Dictionaries.BoilerTypes;
using GazRouter.DAL.Dictionaries.BoilerTypes;
using GazRouter.DTO.Dictionaries.RegulatorTypes;
using GazRouter.DTO.ObjectModel.Regulators;
using GazRouter.DAL.Dictionaries.RegulatorTypes;
using GazRouter.DTO.Dictionaries.EmergencyValveTypes;
using GazRouter.DTO.Dictionaries.HeaterTypes;
using GazRouter.DTO.ObjectModel.EmergencyValves;
using GazRouter.DTO.ObjectModel.Heaters;
using GazRouter.DAL.Dictionaries.HeaterTypes;
using GazRouter.DAL.Dictionaries.EmergencyValveTypes;
using GazRouter.DAL.ObjectModel.Consumers;
using GazRouter.DAL.ObjectModel.Heaters;
using GazRouter.DAL.ObjectModel.EmergencyValves;
using GazRouter.DAL.ObjectModel.Regulators;
using GazRouter.DTO.ObjectModel.ChangeLogs;
using GazRouter.DAL.ObjectModel.DeviceConfig;
using GazRouter.DAL.ObjectModel.Entities.IsInputOff;
using GazRouter.DAL.ObjectModel.OperConsumers;
using GazRouter.DAL.ObjectModel.Segment.Regions;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.Entities.IsInputOff;
using GazRouter.DTO.ObjectModel.OperConsumers;

namespace GazRouter.DataServices.ObjectModel
{
    [Authorization]
    [ErrorHandlerLogger("mainLogger")]
    public class ObjectModelService : ServiceBase, IObjectModelService
    {
        #region Common

        // Получение страницы сущностей
        public EntitiesPageDTO GetEntitiesPage(GetEntitesPageParameterSet parameters)
        {
            if (parameters.OnlyCurrentEnterpriseEntities && !parameters.SiteId.HasValue)
                parameters.EnterpriseId = AppSettingsManager.CurrentEnterpriseId;

            var result = ExecuteRead<GetEntitiesPageQuery, EntitiesPageDTO, GetEntitesPageParameterSet>(parameters);
            result.Token = parameters.Token;
            return result;
        }

        // Получение списка сущностей
        public List<CommonEntityDTO> GetEntityList(GetEntityListParameterSet parameters)
        {
            return ExecuteRead<GetEntityListQuery, List<CommonEntityDTO>, GetEntityListParameterSet>(parameters);
        }

        // Получение сущности по идентификатору
        public CommonEntityDTO GetEntityById(Guid parameters)
        {
            using (var context = OpenDbContext())
            {
                var entity = new GetEntityQuery(context).Execute(parameters);

                switch (entity?.EntityType)
                {
                    case EntityType.DistrStation:
                        return new GetDistrStationByIdQuery(context).Execute(parameters);
                    case EntityType.Valve:
                        return new GetValveByIdQuery(context).Execute(parameters);
                    case EntityType.MeasLine:
                        return new GetMeasLineByIdQuery(context).Execute(parameters);
                    case EntityType.Site:
                        return new GetSiteByIdQuery(context).Execute(parameters);
                    case EntityType.CompStation:
                        return new GetCompStationByIdQuery(context).Execute(parameters);
                    case EntityType.CompShop:
                        return new GetCompShopByIdQuery(context).Execute(parameters);
                    case EntityType.CompUnit:
                        return new GetCompUnitByIdQuery(context).Execute(parameters);
                    case EntityType.ReducingStation:
                        return new GetReducingStationByIdQuery(context).Execute(parameters);
                    case EntityType.Boiler:
                        return new GetBoilerByIdQuery(context).Execute(parameters);
                    case EntityType.MeasStation:
                        return new GetMeasStationByIdQuery(context).Execute(parameters);
                    case EntityType.PowerUnit:
                        return new GetPowerUnitByIdQuery(context).Execute(parameters);
                    case EntityType.DistrStationOutlet:
                        return new GetDistrStationOutletByIdQuery(context).Execute(parameters);
                    case EntityType.Pipeline:
                        return new GetPipelineByIdQuery(context).Execute(parameters);
                    case EntityType.MeasPoint:
                        return new GetMeasPointByIdQuery(context).Execute(parameters);
                    case EntityType.CoolingStation:
                        return new GetCoolingStationByIdQuery(context).Execute(parameters);
                    case EntityType.BoilerPlant:
                        return new GetBoilerPlantByIdQuery(context).Execute(parameters);
                    case EntityType.CoolingUnit:
                        return new GetCoolingUnitByIdQuery(context).Execute(parameters);
                    case EntityType.PowerPlant:
                        return new GetPowerPlantByIdQuery(context).Execute(parameters);
                    case EntityType.Consumer:
                        return new GetConsumerByIdQuery(context).Execute(parameters);
                    case EntityType.OperConsumer:
                        return new GetOperConsumerByIdQuery(context).Execute(parameters);
                }

                return entity;
            }
        }

        // Удаление сущности
        public void DeleteEntity(DeleteEntityParameterSet parameters)
        {
            switch (parameters.EntityType)
            {
                case EntityType.Site:
                    ExecuteNonQuery<DeleteSiteCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.CompStation:
                    ExecuteNonQuery<DeleteCompStationCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.DistrStation:
                    ExecuteNonQuery<DeleteMeasStationCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.MeasStation:
                    ExecuteNonQuery<DeleteMeasStationCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.CompShop:
                    ExecuteNonQuery<DeleteCompShopCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.CompUnit:
                    ExecuteNonQuery<DeleteCompUnitCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.Consumer:
                    ExecuteNonQuery<DeleteConsumerCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.DistrStationOutlet:
                    ExecuteNonQuery<DeleteDistrStationOutletCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.Pipeline:
                    using (var context = OpenDbContext())
                    {
                        var connections =
                            new GetPipelineConnListQuery(context).Execute(new GetPipelineConnListParameterSet
                            {
                                PipelineId = parameters.Id
                            });

                        foreach (var pipelineConnDTO in connections)
                        {
                            new DeletePipelineConnCommand(context).Execute(pipelineConnDTO.Id);
                        }
                        new DeletePipelineCommand(context).Execute(parameters);
                    }
                    break;
                case EntityType.MeasLine:
                    ExecuteNonQuery<DeleteMeasLineCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.Valve:
                    ExecuteNonQuery<DeleteValveCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.ReducingStation:
                    ExecuteNonQuery<DeleteReducingStationCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.MeasPoint:
                    ExecuteNonQuery<DeleteMeasPointCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.CoolingStation:
                    ExecuteNonQuery<DeleteCoolingStationCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.PowerUnit:
                    ExecuteNonQuery<DeletePowerUnitCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.Boiler:
                    ExecuteNonQuery<DeleteBoilerCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.BoilerPlant:
                    ExecuteNonQuery<DeleteBoilerPlantCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.CoolingUnit:
                    ExecuteNonQuery<DeleteCoolingUnitCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.PowerPlant:
                    ExecuteNonQuery<DeletePowerPlantCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.Aggregator:
                    ExecuteNonQuery<DeleteAggregatorCommand, DeleteEntityParameterSet>(parameters);
                    break;
                case EntityType.OperConsumer:
                    ExecuteNonQuery<DeleteOperConsumerCommand, DeleteEntityParameterSet>(parameters);
                    break;


                default:
                    throw new ArgumentOutOfRangeException(nameof(parameters), "incorrect EntityType");
            }
        }

        // Установление порядка сортировки сущности
        public void SetSortOrder(SetSortOrderParameterSet parameters)
        {
            ExecuteNonQuery<SetSortOrderCommand, SetSortOrderParameterSet>(parameters);
        }

        // Добавление примечания к сущности
        public void AddDescription(AddDescriptionParameterSet parameters)
        {
            ExecuteNonQuery<AddDescriptionCommand, AddDescriptionParameterSet>(parameters);
        }

        // Получение лога изменения сущности
        public List<EntityChangeDTO> GetEntityChangeList(Guid parameters)
        {
            return ExecuteRead<GetEntityChangeListQuery, List<EntityChangeDTO>, Guid>(parameters);
        }

        // Получить список прикрепленных к сущности документов
        public List<AttachmentDTO<int, Guid>> GetEntityAttachmentList(Guid? parameters)
        {
            return ExecuteRead<GetEntityAttachmentListQuery, List<AttachmentDTO<int, Guid>>, Guid?>(parameters);
        }

        // Прикрепить документ к сущности
        public int AddEntityAttachment(AddAttachmentParameterSet<Guid> parameters)
        {
            return ExecuteRead<AddEntityAttachmentCommand, int, AddAttachmentParameterSet<Guid>>(parameters);
        }

        // Удалить прикрепленный к сущности документ
        public void RemoveEntityAttachment(int parameters)
        {
            ExecuteNonQuery<RemoveEntityAttachmentCommand, int>(parameters);
        }

        // Получение списка внешних ссылок сущности
        public List<EntityUrlDTO> GetEntityUrlList(Guid? parameters)
        {
            return ExecuteRead<GetEntityUrlListQuery, List<EntityUrlDTO>, Guid?>(parameters);
        }

        //Добавить внешнюю ссылку сущности
        public int AddEntityUrl(AddEntityUrlParameterSet parameters)
        {
            return ExecuteRead<AddEntityUrlCommand, int, AddEntityUrlParameterSet>(parameters);
        }

        // Изменить внешнюю ссылку сущности
        public void EditEntityUrl(EditEntityUrlParameterSet parameters)
        {
            ExecuteNonQuery<EditEntityUrlCommand, EditEntityUrlParameterSet>(parameters);
        }

        // Удалить внешнюю ссылку сущности
        public void RemoveEntityUrl(int parameters)
        {
            ExecuteNonQuery<RemoveEntityUrlCommand, int>(parameters);
        }

        #endregion

        #region Full Tree

        // Получение дерева объектной модели
        public TreeDataDTO GetFullTree(EntityTreeGetParameterSet parameters)
        {
            var result = new TreeDataDTO();
            using (var context = OpenDbContext())
            {
                if (parameters.Filter.HasFlag(EntityFilter.Sites))
                {
                    result.Sites = new GetSiteListQuery(context).Execute(new GetSiteListParameterSet
                    {
                        SystemId = parameters.SystemId,
                        SiteId = parameters.SiteId
                    });
                }

                if (parameters.Filter.HasFlag(EntityFilter.CompStations))
                {
                    result.CompStations =
                        new GetCompStationListQuery(context).Execute(new GetCompStationListParameterSet
                        {
                            SystemId = parameters.SystemId,
                            SiteId = parameters.SiteId
                        });
                }

                if (parameters.Filter.HasFlag(EntityFilter.CompShops))
                {
                    result.CompShops = new GetCompShopListQuery(context).Execute(
                        new GetCompShopListParameterSet
                        {
                            SystemId = parameters.SystemId
                        });
                }

                if (parameters.Filter.HasFlag(EntityFilter.CompUnits))
                {
                    result.CompUnits = new GetCompUnitListQuery(context).Execute(new GetCompUnitListParameterSet
                    {
                        SystemId = parameters.SystemId
                    });
                }

                if (parameters.Filter.HasFlag(EntityFilter.DistrStations))
                {
                    result.DistrStations =
                        new GetDistrStationListQuery(context).Execute(new GetDistrStationListParameterSet
                        {
                            SystemId = parameters.SystemId,
                            SiteId = parameters.SiteId
                        });
                }

                if (parameters.Filter.HasFlag(EntityFilter.DistrStationOutlets))
                {
                    result.DistrStationOutlets =
                        new GetDistrStationOutletListQuery(context).Execute(new GetDistrStationOutletListParameterSet
                        {
                            SystemId = parameters.SystemId
                        });
                }

                if (parameters.Filter.HasFlag(EntityFilter.Consumers))
                {
                    result.Consumers = new GetConsumerListQuery(context).Execute(new GetConsumerListParameterSet
                    {
                        SystemId = parameters.SystemId
                    });
                }

                if (parameters.Filter.HasFlag(EntityFilter.MeasStations))
                {
                    result.MeasStations =
                        new GetMeasStationListQuery(context).Execute(new GetMeasStationListParameterSet
                        {
                            SystemId = parameters.SystemId,
                            SiteId = parameters.SiteId
                        });
                }

                if (parameters.Filter.HasFlag(EntityFilter.MeasLines))
                {
                    result.MeasLines = new GetMeasLineListQuery(context).Execute(new GetMeasLineListParameterSet
                    {
                        SystemId = parameters.SystemId
                    });
                }

                if (parameters.Filter.HasFlag(EntityFilter.ReducingStations))
                {
                    result.ReducingStations =
                        new GetReducingStationListQuery(context).Execute(new GetReducingStationListParameterSet
                        {
                            SystemId = parameters.SystemId,
                            SiteId = parameters.SiteId
                        });
                }

                if (parameters.Filter.HasFlag(EntityFilter.MeasPoints))
                {
                    result.MeasPoints = new GetMeasPointListQuery(context).Execute(new GetMeasPointListParameterSet
                    {
                        SystemId = parameters.SystemId
                    });
                }

                if (parameters.Filter.HasFlag(EntityFilter.PowerPlants))
                {
                    result.PowerPlants = new GetPowerPlantListQuery(context).Execute(parameters.SystemId);
                }

                if (parameters.Filter.HasFlag(EntityFilter.PowerUnits))
                {
                    result.PowerUnits = new GetPowerUnitListQuery(context).Execute(new GetPowerUnitListParameterSet {SystemId = parameters.SystemId });
                }

                if (parameters.Filter.HasFlag(EntityFilter.BoilerPlants))
                {
                    result.BoilerPlants = new GetBoilerPlantListQuery(context).Execute(parameters.SystemId);
                }

                if (parameters.Filter.HasFlag(EntityFilter.Boilers))
                {
                    result.Boilers = new GetBoilerListQuery(context).Execute(new GetBoilerListParameterSet {SystemId = parameters.SystemId });
                }

                if (parameters.Filter.HasFlag(EntityFilter.CoolingStations))
                {
                    result.CoolingStations =
                        new GetCoolingStationsListQuery(context).Execute(new GetCoolingStationListParameterSet
                        {
                            SystemId = parameters.SystemId,
                            SiteId = parameters.SiteId
                        });
                }

                if (parameters.Filter.HasFlag(EntityFilter.CoolingUnits))
                {
                    result.CoolingUnits = new GetCoolingUnitListQuery(context).Execute(new GetCoolingUnitListParameterSet());
                }

                if (parameters.Filter.HasFlag(EntityFilter.Pipelines))
                {
                    result.Pipelines = new GetPipelineListQuery(context).Execute(new GetPipelineListParameterSet
                    {
                        SystemId = parameters.SystemId,
                        SiteId = parameters.SiteId
                    });
                }

                if (parameters.Filter.HasFlag(EntityFilter.LinearValves))
                {
                    result.LinearValves =
                        new GetValveListQuery(context).Execute(new GetValveListParameterSet
                        {
                            SystemId = parameters.SystemId,
                            SiteId = parameters.SiteId
                        });
                }

                if (parameters.Filter.HasFlag(EntityFilter.OperConsumers))
                {
                    result.OperConsumers = new GetOperConsumerListQuery(context).Execute(
                        new GetOperConsumerListParameterSet
                        {
                            SystemId = parameters.SystemId,
                            SiteId = parameters.SiteId
                        });
                }
            }
            return result;
        }
        #endregion

        public List<EnterpriseDTO> GetEnterpriseList()
        {
            return
                ExecuteRead<GetEnterpriseListQuery, List<EnterpriseDTO>, Guid?>(AppSettingsManager.CurrentEnterpriseId);
        }

        #region Site

        // Получение списка предприятий и ЛПУ
        public List<CommonEntityDTO> GetCurrentEnterpriseAndSites()
        {
            return
                ExecuteRead<GetEnterpriseAndSitesQuery, List<CommonEntityDTO>, Guid>(
                    AppSettingsManager.CurrentEnterpriseId);
        }

        // Получение списка ЛПУ
        public List<SiteDTO> GetSiteList(GetSiteListParameterSet parameters)
        {
            return ExecuteRead<GetSiteListQuery, List<SiteDTO>, GetSiteListParameterSet>(parameters);
        }

        // Поиск ЛПУ
        public Guid? FindSite(Guid parameters)
        {
            var result = ExecuteRead<GetEntitySiteIdQuery, Guid?, Guid>(parameters);
            if (result == Guid.Empty)
            {
                result = ExecuteRead<GetEntitySegmentSiteIdQuery, Guid?, Guid>(parameters);
            }
            return result;
        }

        // Добавление ЛПУ
        public Guid AddSite(AddSiteParameterSet parameters)
        {
            return ExecuteRead<AddSiteCommand, Guid, AddSiteParameterSet>(parameters);
        }

        // Редактирование ЛПУ
        public void EditSite(EditSiteParameterSet parameters)
        {
            ExecuteNonQuery<EditSiteCommand, EditSiteParameterSet>(parameters);
        }

        #endregion

        #region CompStation

        // Получение списка КС
        public List<CompStationDTO> GetCompStationList(GetCompStationListParameterSet parameters)
        {
            return ExecuteRead<GetCompStationListQuery, List<CompStationDTO>, GetCompStationListParameterSet>(parameters);
        }

        // Получение дерева КС по идентификатору ЛПУ
        public TreeDataDTO GetCompStationTree(Guid? parameters)
        {
            var result = new TreeDataDTO();
            using (var context = OpenDbContext())
            {
                var stations =
                    new GetCompStationListQuery(context).Execute(parameters.HasValue
                        ? new GetCompStationListParameterSet {SiteId = parameters}
                        : null);
                result.CompStations = stations;

                var compShops =
                    new GetCompShopListQuery(context).Execute(null)
                        .Where(shop => stations.Any(station => station.Id == shop.ParentId))
                        .ToList();
                result.CompShops = compShops;

                result.CompUnits =
                    new GetCompUnitListQuery(context).Execute(null)
                        .Where(compUnit => compShops.Any(compShop => compShop.Id == compUnit.ParentId))
                        .ToList();

                var powerPlants =
                    new GetPowerPlantListQuery(context).Execute(null)
                        .Where(pp => stations.Any(station => station.Id == pp.ParentId))
                        .ToList();
                result.PowerPlants = powerPlants;

                result.PowerUnits =
                    new GetPowerUnitListQuery(context).Execute(null)
                        .Where(pu => powerPlants.Any(pp => pp.Id == pu.ParentId))
                        .ToList();

                var boilerPlants =
                    new GetBoilerPlantListQuery(context).Execute(null)
                        .Where(b => stations.Any(station => station.Id == b.ParentId))
                        .ToList();
                result.BoilerPlants = boilerPlants;

                result.Boilers =
                    new GetBoilerListQuery(context).Execute(null)
                        .Where(b => boilerPlants.Any(station => station.Id == b.ParentId))
                        .ToList();

            
                var coolingStations =
                    new GetCoolingStationsListQuery(context).Execute(new GetCoolingStationListParameterSet
                    {
                        SiteId = parameters
                    }).Where(c => stations.Any(station => station.Id == c.ParentId)).ToList();
                result.CoolingStations = coolingStations;

                result.CoolingUnits =
                    new GetCoolingUnitListQuery(context).Execute(new GetCoolingUnitListParameterSet())
                        .Where(cu => coolingStations.Any(station => station.Id == cu.ParentId))
                        .ToList();

                result.MeasPoints =
                    new GetMeasPointListQuery(context).Execute(null)
                        .Where(mp => compShops.Any(s => s.Id == mp.ParentId))
                        .ToList();
            }
            return result;
        }

        // Добавление КС
        public Guid AddCompStation(AddCompStationParameterSet parameters)
        {
            return ExecuteRead<AddCompStationCommand, Guid, AddCompStationParameterSet>(parameters);
        }

        // Редактирование КС
        public void EditCompStation(EditCompStationParameterSet parameters)
        {
            ExecuteNonQuery<EditCompStationCommand, EditCompStationParameterSet>(parameters);
        }

        #endregion

        #region CompStationCoolingRecomended

        // Получение списка рекомендуемой температуры на выходе КС
        public List<CompStationCoolingRecomendedDTO> GetCompStationCoolingRecomendedList(Guid parameters)
        {
            return
                ExecuteRead<GetCompStationCoolingRecomendedListQuery, List<CompStationCoolingRecomendedDTO>, Guid>(
                    parameters);
        }

        // Редактирование рекомендуемой температуры на выходе КС
        public void SetCompStationCoolingRecomended(SetCompStationCoolingRecomendedParameterSet parameters)
        {
            ExecuteNonQuery<SetCompStationCoolingRecomendedCommand, SetCompStationCoolingRecomendedParameterSet>(
                parameters);
        }

        #endregion

        #region CompShop

        // Получение списка КЦ
        public List<CompShopDTO> GetCompShopList(GetCompShopListParameterSet parameters)
        {
            return ExecuteRead<GetCompShopListQuery, List<CompShopDTO>, GetCompShopListParameterSet>(parameters);
        }



        // Добавление КЦ
        public Guid AddCompShop(AddCompShopParameterSet parameters)
        {
            return ExecuteRead<AddCompShopCommand, Guid, AddCompShopParameterSet>(parameters);
        }

        // Редактирование КЦ
        public void EditCompShop(EditCompShopParameterSet parameters)
        {
            ExecuteNonQuery<EditCompShopCommand, EditCompShopParameterSet>(parameters);
        }

        #endregion

        #region CompUnit

        // Получение параметров ГПА по идентификатору
        public CompUnitDTO GetCompUnitById(Guid parameters)
        {
            return ExecuteRead<GetCompUnitByIdQuery, CompUnitDTO, Guid>(parameters);
        }

        // Получение списка ГПА
        public List<CompUnitDTO> GetCompUnitList(GetCompUnitListParameterSet parameters)
        {
            return ExecuteRead<GetCompUnitListQuery, List<CompUnitDTO>, GetCompUnitListParameterSet>(parameters);
        }

        // Добавление ГПА
        public Guid AddCompUnit(AddCompUnitParameterSet parameters)
        {
            return ExecuteRead<AddCompUnitCommand, Guid, AddCompUnitParameterSet>(parameters);
        }

        // Редактирование ГПА
        public void EditCompUnit(EditCompUnitParameterSet parameters)
        {
            ExecuteNonQuery<EditCompUnitCommand, EditCompUnitParameterSet>(parameters);
        }

        #endregion

        #region BoilerPlant

        // Добавление Котельной
        public Guid AddBoilerPlant(AddBoilerPlantParameterSet parameters)
        {
            return ExecuteRead<AddBoilerPlantCommand, Guid, AddBoilerPlantParameterSet>(parameters);
        }

        // Редактирование Котельной
        public void EditBoilerPlant(EditBoilerPlantParameterSet parameters)
        {
            ExecuteNonQuery<EditBoilerPlantCommand, EditBoilerPlantParameterSet>(parameters);
        }

        #endregion

        #region Boiler

        // Получение котла по идентификатору
        public BoilerDTO GetBoilerById(Guid parameters)
        {
            return ExecuteRead<GetBoilerByIdQuery, BoilerDTO, Guid>(parameters);
        }

        // Добавление котла
        public Guid AddBoiler(AddBoilerParameterSet parameters)
        {
            return ExecuteRead<AddBoilerCommand, Guid, AddBoilerParameterSet>(parameters);
        }

        // Редактирование котла
        public void EditBoiler(EditBoilerParameterSet parameters)
        {
            ExecuteNonQuery<EditBoilerCommand, EditBoilerParameterSet>(parameters);
        }

        #endregion

        #region PowerPlant

        //Добавление ЭСН
        public Guid AddPowerPlant(AddPowerPlantParameterSet parameters)
        {
            return ExecuteRead<AddPowerPlantCommand, Guid, AddPowerPlantParameterSet>(parameters);
        }

        //Редактирование ЭСН
        public void EditPowerPlant(EditPowerPlantParameterSet parameters)
        {
            ExecuteNonQuery<EditPowerPlantCommand, EditPowerPlantParameterSet>(parameters);
        }

        #endregion

        #region PowerUnit

        // Получение электроагрегата по идентификатору
        public PowerUnitDTO GetPowerUnitById(Guid parameters)
        {
            return ExecuteRead<GetPowerUnitByIdQuery, PowerUnitDTO, Guid>(parameters);
        }

        // Добавление электроагрегата
        public Guid AddPowerUnit(AddPowerUnitParameterSet parameters)
        {
            return ExecuteRead<AddPowerUnitCommand, Guid, AddPowerUnitParameterSet>(parameters);
        }

        // Редактирование электроагрегата
        public void EditPowerUnit(EditPowerUnitParameterSet parameters)
        {
            ExecuteNonQuery<EditPowerUnitCommand, EditPowerUnitParameterSet>(parameters);
        }

        #endregion

        #region CoolingStation

        // Добавление СОГ
        public Guid AddCoolingStation(AddCoolingStationParameterSet parameters)
        {
            return ExecuteRead<AddCoolingStationCommand, Guid, AddCoolingStationParameterSet>(parameters);
        }

        // Редактирование СОГ
        public void EditCoolingStation(EditCoolingStationParameterSet parameters)
        {
            ExecuteNonQuery<EditCoolingStationCommand, EditCoolingStationParameterSet>(parameters);
        }

        #endregion

        #region CoolingUnit

        // Получение списка установок охлаждения газа
        public List<CoolingUnitDTO> GetCoolingUnitList(GetCoolingUnitListParameterSet parameters)
        {
            return ExecuteRead<GetCoolingUnitListQuery, 
                List<CoolingUnitDTO>,GetCoolingUnitListParameterSet>(parameters);
        }

        // Получение установки охлаждения газа по идентификатору
        public CoolingUnitDTO GetCoolingUnitById(Guid parameters)
        {
            return ExecuteRead<GetCoolingUnitByIdQuery, CoolingUnitDTO, Guid>(parameters);
        }

        // Добавление установки охлаждения газа
        public Guid AddCoolingUnit(AddCoolingUnitParameterSet parameters)
        {
            return ExecuteRead<AddCoolingUnitCommand, Guid, AddCoolingUnitParameterSet>(parameters);
        }

        // Редактирование установки охлаждения газа
        public void EditCoolingUnit(EditCoolingUnitParameterSet parameters)
        {
            ExecuteNonQuery<EditCoolingUnitCommand, EditCoolingUnitParameterSet>(parameters);
        }

        #endregion

        #region DistrStation

        // Получение ветки дерева по ГРС
        public TreeDataDTO GetDistrStationTree(GetDistrStationListParameterSet parameters)
        {
            if (parameters.ThisEnterprise)
                parameters.EnterpriseId = AppSettingsManager.CurrentEnterpriseId;

            var result = new TreeDataDTO();
            using (var context = OpenDbContext())
            {
                var stations = new GetDistrStationListQuery(context).Execute(parameters).ToList();
                result.DistrStations = stations;

                result.DistrStationOutlets =
                    new GetDistrStationOutletListQuery(context).Execute(null)
                        .Where(o => stations.Any(station => station.Id == o.ParentId))
                        .ToList();

                result.Boilers =
                    new GetBoilerListQuery(context).Execute(null)
                        .Where(b => stations.Any(station => station.Id == b.ParentId))
                        .ToList();

                result.MeasPoints =
                    new GetMeasPointListQuery(context).Execute(null)
                        .Where(mp => stations.Any(s => s.Id == mp.ParentId))
                        .ToList();

                result.Consumers =
                    new GetConsumerListQuery(context).Execute(null)
                        .Where(c => stations.Any(s => s.Id == c.DistrStationId))
                        .ToList();
            }
            return result;
        }

        // Добавление ГРС
        public Guid AddDistrStation(AddDistrStationParameterSet parameters)
        {
            return ExecuteRead<AddDistrStationCommand, Guid, AddDistrStationParameterSet>(parameters);
        }

        // Редактирование ГРС
        public void EditDistrStation(EditDistrStationParameterSet parameters)
        {
            ExecuteNonQuery<EditDistrStationCommand, EditDistrStationParameterSet>(parameters);
        }

        #endregion

        #region DistrStationOutlet

        // Получение списка выходов ГРС
        public List<DistrStationOutletDTO> GetDistrStationOutletList(GetDistrStationOutletListParameterSet parameters)
        {
            return
                ExecuteRead
                    <GetDistrStationOutletListQuery, List<DistrStationOutletDTO>, GetDistrStationOutletListParameterSet>
                    (parameters);
        }

        // Добавление выхода ГРС
        public Guid AddDistrStationOutlet(AddDistrStationOutletParameterSet parameters)
        {
            return ExecuteRead<AddDistrStationOutletCommand, Guid, AddDistrStationOutletParameterSet>(parameters);
        }

        // Редактирование выхода ГРС
        public void EditDistrStationOutlet(EditDistrStationOutletParameterSet parameters)
        {
            ExecuteNonQuery<EditDistrStationOutletCommand, EditDistrStationOutletParameterSet>(parameters);
        }

        #endregion

        #region Consumer

        // Получение списка потребителей
        public List<ConsumerDTO> GetConsumerList(GetConsumerListParameterSet parameters)
        {
            return ExecuteRead<GetConsumerListQuery, List<ConsumerDTO>, GetConsumerListParameterSet>(parameters);
        }

        // Добавление потребителя
        public Guid AddConsumer(AddConsumerParameterSet parameters)
        {
            return ExecuteRead<AddConsumerCommand, Guid, AddConsumerParameterSet>(parameters);
        }

        // Редактирование потребителя
        public void EditConsumer(EditConsumerParameterSet parameters)
        {
            ExecuteNonQuery<EditConsumerCommand, EditConsumerParameterSet>(parameters);
        }

        #endregion

        #region OperConsumers

        public List<OperConsumerDTO> GetOperConsumers(GetOperConsumerListParameterSet parameters)
        {
            return ExecuteRead<GetOperConsumerListQuery, List<OperConsumerDTO>, GetOperConsumerListParameterSet>(parameters);
        }

        public Guid AddOperConsumer(AddEditOperConsumerParameterSet parameters)
        {
            return ExecuteRead<AddOperConsumerCommand, Guid, AddEditOperConsumerParameterSet>(parameters);
        }

        public void EditOperConsumer(AddEditOperConsumerParameterSet parameters)
        {
            ExecuteNonQuery<EditOperConsumerCommand, AddEditOperConsumerParameterSet>(parameters);
        }

        #endregion

        #region MeasStation

        // Получение списка ГИС
        public List<MeasStationDTO> GetMeasStationList(GetMeasStationListParameterSet parameters)
        {
            if (parameters.ThisEnterprise)
                parameters.EnterpriseId = AppSettingsManager.CurrentEnterpriseId;

            return ExecuteRead<GetMeasStationListQuery, List<MeasStationDTO>, GetMeasStationListParameterSet>(parameters);
        }

        // Получение ветки дерева ГИС
        public TreeDataDTO GetMeasStationTree(GetMeasStationListParameterSet parameters)
        {
            var result = new TreeDataDTO();
            using (var context = OpenDbContext())
            {
                if (parameters.ThisEnterprise)
                    parameters.EnterpriseId = AppSettingsManager.CurrentEnterpriseId;

                var stations = new GetMeasStationListQuery(context).Execute(parameters);
                result.MeasStations = stations;
                result.MeasLines = new GetMeasLineListQuery(context).Execute(
                    new GetMeasLineListParameterSet
                    {
                        MeasStationList = stations.Select(s => s.Id).ToList()
                    });
                result.MeasPoints =
                    new GetMeasPointListQuery(context).Execute(null)
                        .Where(mp => result.MeasLines.Any(s => s.Id == mp.ParentId))
                        .ToList();
                result.Boilers =
                    new GetBoilerListQuery(context).Execute(null)
                        .Where(b => stations.Any(station => station.Id == b.ParentId))
                        .ToList();
            }
            return result;
        }

        // Добавление ГИС
        public Guid AddMeasStation(AddMeasStationParameterSet parameters)
        {
            return ExecuteRead<AddMeasStationCommand, Guid, AddMeasStationParameterSet>(parameters);
        }

        // Редактирование ГИС
        public void EditMeasStation(EditMeasStationParameterSet parameters)
        {
            ExecuteNonQuery<EditMeasStationCommand, EditMeasStationParameterSet>(parameters);
        }

        #endregion

        #region MeasLine

        // Получение списка замерных линий ГИС
        public List<MeasLineDTO> GetMeasLineList(GetMeasLineListParameterSet parameters)
        {
            return ExecuteRead<GetMeasLineListQuery, List<MeasLineDTO>, GetMeasLineListParameterSet>(parameters);
        }

        // Добавление замерной линии ГИС
        public Guid AddMeasLine(AddMeasLineParameterSet parameters)
        {
            return ExecuteRead<AddMeasLineCommand, Guid, AddMeasLineParameterSet>(parameters);
        }

        // Редактирование замерной линии ГИС
        public void EditMeasLine(EditMeasLineParameterSet parameters)
        {
            ExecuteNonQuery<EditMeasLineCommand, EditMeasLineParameterSet>(parameters);
        }

        #endregion

        #region ReducingStation

        // Получение списка ПРГ
        public List<ReducingStationDTO> GetReducingStationList(GetReducingStationListParameterSet parameters)
        {
            return
                ExecuteRead<GetReducingStationListQuery, List<ReducingStationDTO>, GetReducingStationListParameterSet>(
                    parameters);
        }

        // Добавление ПРГ
        public Guid AddReducingStation(AddReducingStationParameterSet parameters)
        {
            return ExecuteRead<AddReducingStationCommand, Guid, AddReducingStationParameterSet>(parameters);
        }

        // Редактирование ПРГ
        public void EditReducingStation(EditReducingStationParameterSet parameters)
        {
            ExecuteNonQuery<EditReducingStationCommand, EditReducingStationParameterSet>(parameters);
        }

        #endregion

        #region MeasPoint

        // Получение точки измерения параметров газа по идентификатору родительского объекта
        public MeasPointDTO GetMeasPointByParent(Guid parameters)
        {
            using (var context = OpenDbContext())
            {
                var mpl =
                    new GetMeasPointListQuery(context).Execute(new GetMeasPointListParameterSet {ParentId = parameters});
                return mpl.FirstOrDefault();
            }
        }

        // Добавление точки измерения параметров газа
        public Guid AddMeasPoint(AddMeasPointParameterSet parameters)
        {
            return ExecuteRead<AddMeasPointCommand, Guid, AddMeasPointParameterSet>(parameters);
        }

        // Редактирование точки измерения параметров газа
        public void EditMeasPoint(EditMeasPointParameterSet parameters)
        {
            ExecuteNonQuery<EditMeasPointCommand, EditMeasPointParameterSet>(parameters);
        }

        // Поиск точки измерения параметров газа для указанного объекта
        public MeasPointDTO FindMeasPoint(Guid parameters)
        {
            // Задача нетривиальная. Поэтому поиск реализован только для некоторых очевидных случаев, 
            // для более сложных случаев работать не будет, т.к. нужно решать задачу по расчету смешивания газа
            using (var context = OpenDbContext())
            {
                var entity = GetEntityById(parameters);
                if (entity == null)
                {
                    return null;
                }

                // ГРС
                if (entity.EntityType == EntityType.DistrStation)
                {
                    return
                        new GetMeasPointListQuery(context).Execute(
                            new GetMeasPointListParameterSet {ParentId = entity.Id}).FirstOrDefault();
                }

                // ГПА
                if (entity.EntityType == EntityType.CompUnit)
                {
                    // Ищем точку для родительской КЦ
                    var unit = entity as CompUnitDTO;
                    if (unit?.ParentId == null)
                    {
                        return null;
                    }
                    return FindMeasPoint(unit.ParentId.Value);
                }

                // КЦ
                if (entity.EntityType == EntityType.CompShop)
                {
                    // Ищем точку для КЦ
                    var mp =
                        new GetMeasPointListQuery(context).Execute(
                            new GetMeasPointListParameterSet {ParentId = entity.Id}).FirstOrDefault();
                    if (mp != null)
                    {
                        return mp;
                    }

                    // Если нет, то ищем у смежных КЦ, той же станции - запускаем поиск для КС
                    var shop = entity as CompShopDTO;
                    if (shop?.ParentId != null)
                    {
                        return FindMeasPoint(shop.ParentId.Value);
                    }
                }

                // КС
                if (entity.EntityType == EntityType.CompStation)
                {
                    // Ищем первую точку у дочерних КЦ
                    var shopList = new GetCompShopListQuery(context).Execute(
                        new GetCompShopListParameterSet {StationIdList = new List<Guid> {entity.Id}});
                    foreach (var s in shopList)
                    {
                        var mp =
                            new GetMeasPointListQuery(context).Execute(
                                new GetMeasPointListParameterSet {ParentId = s.Id}).FirstOrDefault();
                        if (mp != null)
                        {
                            return mp;
                        }
                    }
                }

                // ГИС
                if (entity.EntityType == EntityType.MeasStation)
                {
                    // Ищем первую точку по замерным линиям
                    var lineList = new GetMeasLineListQuery(context).Execute(
                        new GetMeasLineListParameterSet
                        {
                            MeasStationId = entity.Id
                        });
                    foreach (var l in lineList)
                    {
                        var mp = new GetMeasPointListQuery(context).Execute(
                            new GetMeasPointListParameterSet {ParentId = l.Id}).FirstOrDefault();
                        if (mp != null)
                        {
                            return mp;
                        }
                    }
                }

                // Замерная линия ГИС
                if (entity.EntityType == EntityType.MeasLine)
                {
                    // Ищем точку для линии
                    var mp = new GetMeasPointListQuery(context).Execute(
                        new GetMeasPointListParameterSet {ParentId = entity.Id}).FirstOrDefault();
                    if (mp != null)
                    {
                        return mp;
                    }

                    // если не находим, то ищем по соседним линиям - запускаем поиск для станции (ГИС)
                    var line = entity as MeasLineDTO;
                    if (line?.PipelineId == null)
                    {
                        return null;
                    }
                    FindMeasPoint(line.PipelineId);
                }
            }

            return null;
        }

        #endregion

        #region Pipeline

        // Получение дерева газопроводов по идентификатору ЛПУ
        public TreeDataDTO GetPipelineTree(Guid? parameters)
        {
            var result = new TreeDataDTO();
            using (var context = OpenDbContext())
            {
                var pipelines =
                    new GetPipelineListQuery(context).Execute(parameters.HasValue
                        ? new GetPipelineListParameterSet {SiteId = parameters}
                        : null);
                result.Pipelines = pipelines;

                var valves = new GetValveListQuery(context).Execute(new GetValveListParameterSet {SiteId = parameters});
                result.LinearValves = valves;
            }
            return result;
        }

        // Получение списка газопроводов
        public List<PipelineDTO> GetPipelineList(GetPipelineListParameterSet parameters)
        {
            return ExecuteRead<GetPipelineListQuery, List<PipelineDTO>, GetPipelineListParameterSet>(parameters);
        }

        // Получение газопровода по идентификатору
        public PipelineDTO GetPipelineById(Guid parameters)
        {
            return ExecuteRead<GetPipelineByIdQuery, PipelineDTO, Guid>(parameters);
        }

        // Добавление газопровода
        public Guid AddPipeline(AddPipelineWithConnsParameterSet parameters)
        {
            Guid pipelineId;
            using (var context = OpenDbContext())
            {
                pipelineId = new AddPipelineCommand(context).Execute(parameters.PipelineParameters);
                if (parameters.StartConnParameters != null)
                {
                    parameters.StartConnParameters.PipelineId = pipelineId;
                    new AddPipelineConnCommand(context).Execute(parameters.StartConnParameters);
                }
                if (parameters.EndConnParameters != null)
                {
                    parameters.EndConnParameters.PipelineId = pipelineId;
                    new AddPipelineConnCommand(context).Execute(parameters.EndConnParameters);
                }
            }
            return pipelineId;
        }

        // Редактирование газопровода
        public void EditPipeline(EditPipelineWithConnsParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var conns = new GetPipelineConnListQuery(context).Execute(new GetPipelineConnListParameterSet
                {
                    PipelineId = parameters.PipelineParameters.Id,
                    GasTrasportSystemId = parameters.PipelineParameters.GasTransportSystemId
                });

                foreach (var pipelineConnDTO in conns)
                {
                    new DeletePipelineConnCommand(context).Execute(pipelineConnDTO.Id);
                }
                new EditPipelinesCommand(context).Execute(parameters.PipelineParameters);
                if (parameters.StartConnParameters != null)
                {
                    parameters.StartConnParameters.PipelineId = parameters.PipelineParameters.Id;
                    new AddPipelineConnCommand(context).Execute(parameters.StartConnParameters);
                }
                if (parameters.EndConnParameters != null)
                {
                    parameters.EndConnParameters.PipelineId = parameters.PipelineParameters.Id;
                    new AddPipelineConnCommand(context).Execute(parameters.EndConnParameters);
                }
            }
        }

        #endregion

        #region PipelineSegmentBySite

        // Получение списка сегментов по ЛПУ по идентификатору газопровода
        public List<SiteSegmentDTO> GetSiteSegmentList(Guid? parameters)
        {
            return ExecuteRead<GetSiteSegmentListQuery, List<SiteSegmentDTO>, Guid?>(parameters);
        }

        // Добавление сегмента по ЛПУ
        public int AddSiteSegment(AddSiteSegmentParameterSet parameters)
        {
            return ExecuteRead<AddSiteSegmentCommand, int, AddSiteSegmentParameterSet>(parameters);
        }

        // Редактирование сегмента по ЛПУ
        public void EditSiteSegment(EditSiteSegmentParameterSet parameters)
        {
            ExecuteNonQuery<EditSiteSegmentCommand, EditSiteSegmentParameterSet>(parameters);
        }

        // Удаление сегмента по ЛПУ
        public void DeleteSiteSegment(int parameters)
        {
            ExecuteNonQuery<DeleteSiteSegmentCommand, int>(parameters);
        }

        #endregion

        #region PipelineSegmentByRegions

        // Получение списка сегментов по ЛПУ по идентификатору газопровода
        public List<RegionSegmentDTO> GetRegionSegmentList(Guid? parameters)
        {
            return ExecuteRead<GetRegionSegmentListQuery, List<RegionSegmentDTO>, Guid?>(parameters);
        }

        // Добавление сегмента по ЛПУ
        public int AddRegionSegment(AddRegionSegmentParameterSet parameters)
        {
            return ExecuteRead<AddRegionSegmentCommand, int, AddRegionSegmentParameterSet>(parameters);
        }

        // Редактирование сегмента по ЛПУ
        public void EditRegionSegment(EditRegionSegmentParameterSet parameters)
        {
            ExecuteNonQuery<EditRegionSegmentCommand, EditRegionSegmentParameterSet>(parameters);
        }

        // Удаление сегмента по ЛПУ
        public void DeleteRegionSegment(int parameters)
        {
            ExecuteNonQuery<DeleteRegionSegmentCommand, int>(parameters);
        }

        #endregion

        #region PipelineSegmentByGroup

        // Получение списка сегментов по группам газапроводов
        public List<GroupSegmentDTO> GetGroupSegmentList(Guid? parameters)
        {
            return ExecuteRead<GetGroupSegmentListQuery, List<GroupSegmentDTO>, Guid?>(parameters);
        }

        // Добавление сегмента по группе газопроводов
        public int AddGroupSegment(AddGroupSegmentParameterSet parameters)
        {
            return ExecuteRead<AddGroupSegmentCommand, int, AddGroupSegmentParameterSet>(parameters);
        }

        // Редактирование сегмента по группе газопроводов
        public void EditGroupSegment(EditGroupSegmentParameterSet parameters)
        {
            ExecuteNonQuery<EditGroupSegmentCommand, EditGroupSegmentParameterSet>(parameters);
        }

        // Удаление сегмента по группе газопроводов
        public void DeleteGroupSegment(int parameters)
        {
            ExecuteNonQuery<DeleteGroupSegmentCommand, int>(parameters);
        }

        #endregion

        #region PipelineSegmentByDiameter

        // Получение списка сегментов по диаметру
        public List<DiameterSegmentDTO> GetDiameterSegmentList(Guid? parameters)
        {
            return ExecuteRead<GetDiameterSegmentListQuery, List<DiameterSegmentDTO>, Guid?>(parameters);
        }

        // Добавление сегмента по диаметру
        public int AddDiameterSegment(AddDiameterSegmentParameterSet parameters)
        {
            return ExecuteRead<AddDiameterSegmentCommand, int, AddDiameterSegmentParameterSet>(parameters);
        }

        // Редактирование сегмента по диаметру
        public void EditDiameterSegment(EditDiameterSegmentParameterSet parameters)
        {
            ExecuteNonQuery<EditDiameterSegmentCommand, EditDiameterSegmentParameterSet>(parameters);
        }

        // Удаление сегмента по диаметру
        public void DeleteDiameterSegment(int parameters)
        {
            ExecuteNonQuery<DeleteDiameterSegmentCommand, int>(parameters);
        }

        #endregion

        #region PipelineSegmentByPressure

        // Получение списка сегментов по давлению
        public List<PressureSegmentDTO> GetPressureSegmentList(Guid? parameters)
        {
            return ExecuteRead<GetPressureSegmentListQuery, List<PressureSegmentDTO>, Guid?>(parameters);
        }

        // Добавление сегмента по давлению
        public int AddPressureSegment(AddPressureSegmentParameterSet parameters)
        {
            return ExecuteRead<AddPressureSegmentCommand, int, AddPressureSegmentParameterSet>(parameters);
        }

        // Редактирование сегмента по давлению
        public void EditPressureSegment(EditPressureSegmentParameterSet parameters)
        {
            ExecuteNonQuery<EditPressureSegmentCommand, EditPressureSegmentParameterSet>(parameters);
        }

        // Удаление сегмента по давлению
        public void DeletePressureSegment(int parameters)
        {
            ExecuteNonQuery<DeletePressureSegmentCommand, int>(parameters);
        }

        #endregion

        #region Valve

        // Получение списка кранов газопровода
        public List<ValveDTO> GetValveList(GetValveListParameterSet parameters)
        {
            return ExecuteRead<GetValveListQuery, List<ValveDTO>, GetValveListParameterSet>(parameters);
        }

        // Получение крана по идентификатору
        public ValveDTO GetValveById(Guid parameters)
        {
            return ExecuteRead<GetValveByIdQuery, ValveDTO, Guid>(parameters);
        }

        // Добавление кранового узла
        public Guid AddValve(AddValveParameterSet parameters)
        {
            return ExecuteRead<AddValveCommand, Guid, AddValveParameterSet>(parameters);
        }

        // Редактирование кранового узла
        public void EditValve(EditValveParameterSet parameters)
        {
            ExecuteNonQuery<EditValveCommand, EditValveParameterSet>(parameters);
        }

        #endregion

        #region Validation

        // Валидация объектной модели
        public void Validate()
        {
            using (var context = OpenDbContext())
            {
                new AfterValidateCommand(context).Execute();
                ObjectModelValidator.Validate(context);
            }
        }

        // Получение ошибок валидации
        public Dictionary<Guid, List<InconsistencyDTO>> GetInconsistencies(Guid? parameters)
        {
            var incList = ExecuteRead<GetInconsistencyListQuery, List<InconsistencyDTO>, Guid?>(parameters);
            return incList.GroupBy(i => i.EntityId).ToDictionary(gr => gr.Key, gr => gr.ToList());
        }

        #endregion

        public List<EntityTypePropertyDTO> GetEntityTypeProperties(EntityType? parameters)
        {
            return ExecuteRead<GetEntityTypePropertyListQuery, List<EntityTypePropertyDTO>, EntityType?>(parameters);
        }

        public int AddPowerUnitType(AddPowerUnitTypeParameterSet parameters)
        {
            return ExecuteRead<AddPowerUnitTypeCommand, int, AddPowerUnitTypeParameterSet>(parameters);
        }

        public void EditPowerUnitType(EditPowerUnitTypeParameterSet parameters)
        {
            ExecuteNonQuery<EditPowerUnitTypeCommand, EditPowerUnitTypeParameterSet>(parameters);
        }

        public void RemovePowerUnitType(int parameters)
        {
            ExecuteNonQuery<RemovePowerUnitTypeCommand, int>(parameters);
        }

        public List<PowerUnitTypeDTO> GetPowerUnitTypes()
        {
            return ExecuteRead<GetPowerUnitTypeListQuery, List<PowerUnitTypeDTO>>();
        }

        public int AddBoilerType(AddBoilerTypeParameterSet parameters)
        {
            return ExecuteRead<AddBoilerTypeCommand, int, AddBoilerTypeParameterSet>(parameters);
        }

        public void EditBoilerType(EditBoilerTypeParameterSet parameters)
        {
            ExecuteNonQuery<EditBoilerTypeCommand, EditBoilerTypeParameterSet>(parameters);
        }

        public void RemoveBoilerType(int parameters)
        {
            ExecuteNonQuery<RemoveBoilerTypeCommand, int>(parameters);
        }

        public List<BoilerTypeDTO> GetBoilerTypes()
        {
            return ExecuteRead<GetBoilerTypeListQuery, List<BoilerTypeDTO>>();
        }


        #region Aggregator

        public List<AggregatorDTO> GetAggregatorList(GetAggregatorListParameterSet parameters)
        {
            return ExecuteRead<GetAggregatorListQuery, List<AggregatorDTO>, GetAggregatorListParameterSet>(parameters);
        }

        public AggregatorDTO GetAggregatorById(Guid parameters)
        {
            return ExecuteRead<GetAggregatorByIdQuery, AggregatorDTO, Guid>(parameters);
        }

        public Guid AddAggregator(AddAggregatorParameterSet parameters)
        {
            return ExecuteRead<AddAggregatorCommand, Guid, AddAggregatorParameterSet>(parameters);
        }

        public void EditAggregator(EditAggregatorParameterSet parameters)
        {
            ExecuteNonQuery<EditAggregatorCommand, EditAggregatorParameterSet>(parameters);
        }

        #endregion

        public int AddRegulatorType(AddRegulatorTypeParameterSet parameters)
        {
            return ExecuteRead<AddRegulatorTypeCommand, int, AddRegulatorTypeParameterSet>(parameters);
        }

        public void EditRegulatorType(EditRegulatorTypeParameterSet parameters)
        {
            ExecuteNonQuery<EditRegulatorTypeCommand, EditRegulatorTypeParameterSet>(parameters);
        }

        public void RemoveRegulatorType(int parameters)
        {
            ExecuteNonQuery<RemoveRegulatorTypeCommand, int>(parameters);
        }

        public List<RegulatorTypeDTO> GetRegulatorTypes()
        {
            return ExecuteRead<GetRegulatorTypeListQuery, List<RegulatorTypeDTO>>();
        }

        public int AddHeaterType(AddHeaterTypeParameterSet parameters)
        {
            return ExecuteRead<AddHeaterTypeCommand, int, AddHeaterTypeParameterSet>(parameters);
        }

        public void EditHeaterType(EditHeaterTypeParameterSet parameters)
        {
            ExecuteNonQuery<EditHeaterTypeCommand, EditHeaterTypeParameterSet>(parameters);
        }

        public void RemoveHeaterType(int parameters)
        {
            ExecuteNonQuery<RemoveHeaterTypeCommand, int>(parameters);
        }

        public List<HeaterTypeDTO> GetHeaterTypes()
        {
            return ExecuteRead<GetHeaterTypesListQuery, List<HeaterTypeDTO>>();
        }

        public int AddEmergencyValveType(AddEmergencyValveTypeParameterSet parameters)
        {
            return ExecuteRead<AddEmergencyValveTypeCommand, int, AddEmergencyValveTypeParameterSet>(parameters);
        }

        public void EditEmergencyValveType(EditEmergencyValveTypeParameterSet parameters)
        {
            ExecuteNonQuery<EditEmergencyValveTypeCommand, EditEmergencyValveTypeParameterSet>(parameters);
        }

        public void RemoveEmergencyValveType(int parameters)
        {
            ExecuteNonQuery<RemoveEmergencyValveTypeCommand, int>(parameters);
        }

        public List<EmergencyValveTypeDTO> GetEmergencyValveTypes()
        {
            return ExecuteRead<GetEmergencyValveTypeListQuery, List<EmergencyValveTypeDTO>>();
        }

        public List<ChangeDTO> GetChangeLog(GetChangeLogParameterSet parameters)
        {
            return ExecuteRead<GetChangesListQuery, List<ChangeDTO>, GetChangeLogParameterSet>(parameters);
        }
        public void SetIsInputOff(SetIsInputOffParameterSet parameters)
        {
            ExecuteNonQuery<SetIsInputOffCommand, SetIsInputOffParameterSet>(parameters);
        }
#region GasCosts
        public List<DistrStationDTO> GetDistrStationList(GetDistrStationListParameterSet parameters)
        {
            return ExecuteRead<GetDistrStationListQuery, List<DistrStationDTO>, GetDistrStationListParameterSet>(parameters);
        }
        public List<BoilerDTO> GetBoilerList(GetBoilerListParameterSet parameters)
        {
            return ExecuteRead<GetBoilerListQuery, List<BoilerDTO>, GetBoilerListParameterSet>(parameters);
        }
        public List<PowerUnitDTO> GetPowerUnitList(GetPowerUnitListParameterSet parameters)
        {
            return ExecuteRead<GetPowerUnitListQuery, List<PowerUnitDTO>, GetPowerUnitListParameterSet>(parameters);
        }

        public List<BoilerPlantDTO> GetBoilerPlantList(int? parameters)
        {
            return ExecuteRead<GetBoilerPlantListQuery, List<BoilerPlantDTO>, int?>(parameters);
        }
#endregion
    }
}


