using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DataServices.Balances.Algorithms.DivideVolume;
using GazRouter.DataServices.Balances.Algorithms.MoveGasSupply;
using GazRouter.DataServices.Balances.Algorithms.OwnersDaySum;
using GazRouter.DataServices.Balances.Algorithms.Transport;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DAL.Balances.Contracts;
using GazRouter.DAL.Balances.Docs;
using GazRouter.DAL.Balances.GasOwners;
using GazRouter.DAL.Balances.Routes;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DataServices.Infrastructure.Attributes.Behaviors;
using GazRouter.DataServices.Infrastructure.Services;
using GazRouter.DAL.Balances.BalanceGroups;
using GazRouter.DAL.Balances.Corrections;
using GazRouter.DAL.Balances.DistrNetworks;
using GazRouter.DAL.Balances.InputStates;
using GazRouter.DAL.Balances.Irregularity;
using GazRouter.DAL.Balances.MiscTab;
using GazRouter.DAL.Balances.Routes.Exceptions;
using GazRouter.DAL.Balances.SortOrder;
using GazRouter.DAL.Balances.Swaps;
using GazRouter.DAL.Balances.Transport;
using GazRouter.DAL.Balances.Values;
using GazRouter.DAL.Dictionaries.Enterprises;
using GazRouter.DAL.GasCosts;
using GazRouter.DAL.ObjectModel.Aggregators;
using GazRouter.DAL.ObjectModel.DistrStationOutlets;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DAL.ObjectModel.MeasLine;
using GazRouter.DAL.ObjectModel.OperConsumers;
using GazRouter.DAL.ObjectModel.Sites;
using GazRouter.DAL.SeriesData.PropertyValues;
using GazRouter.DAL.SeriesData.Series;
using GazRouter.DTO.Balances.BalanceGroups;
using GazRouter.DTO.Balances.BalanceMeasurings;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.DayBalance;
using GazRouter.DTO.Balances.DistrNetworks;
using GazRouter.DTO.Balances.Docs;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Balances.InputStates;
using GazRouter.DTO.Balances.Irregularity;
using GazRouter.DTO.Balances.MiscTab;
using GazRouter.DTO.Balances.MonthAlgorithms;
using GazRouter.DTO.Balances.Routes;
using GazRouter.DTO.Balances.Routes.Exceptions;
using GazRouter.DTO.Balances.SortOrder;
using GazRouter.DTO.Balances.Swaps;
using GazRouter.DTO.Balances.Transport;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Aggregators;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.OperConsumers;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using Utils.Extensions;
using SetBalSortOrderParameterSet = GazRouter.DTO.Balances.SortOrder.SetBalSortOrderParameterSet;

namespace GazRouter.DataServices.Balances
{
    [ErrorHandlerLogger("mainLogger")]
    [Authorization]
    public class BalancesService : ServiceBase, IBalancesService
    {
		#region GasOwners

        public List<GasOwnerDTO> GetGasOwnerList(int? parameters)
		{
            return ExecuteRead<GetGasOwnerListQuery, List<GasOwnerDTO>, int?>(parameters);
		}

        public int AddGasOwner(AddGasOwnerParameterSet parameters)
		{
			return ExecuteRead<AddGasOwnerCommand, int, AddGasOwnerParameterSet>(parameters);
		}

		public void EditGasOwner(EditGasOwnerParameterSet parameters)
		{
			ExecuteNonQuery<EditGasOwnerCommand, EditGasOwnerParameterSet>(parameters);
		}

        public void DeleteGasOwner(int parameters)
		{
			ExecuteNonQuery<DeleteGasOwnerCommand, int>(parameters);
		}

		public void SetGasOwnerSortOrder(SetGasOwnerSortOrderParameterSet parameters)
		{
			ExecuteNonQuery<SetGasOwnerSortOrderCommand, SetGasOwnerSortOrderParameterSet>(parameters);
		}

        public List<GasOwnerDisableDTO> GetGasOwnerDisableList()
        {
            return ExecuteRead<GetGasOwnerDisableListQuery, List<GasOwnerDisableDTO>>();
        }

        public void SetGasOwnerDisable(SetGasOwnerDisableParameterSet parameters)
        {
            ExecuteNonQuery<SetGasOwnerDisableCommand, SetGasOwnerDisableParameterSet>(parameters);
        }


        public void SetGasOwnerSystem(SetGasOwnerSystemParameterSet parameters)
        {
            ExecuteNonQuery<SetGasOwnerSystemCommand, SetGasOwnerSystemParameterSet>(parameters);
        }

        #endregion




        

        


        public ContractDTO GetContract(GetContractListParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                if (!parameters.ContractDate.HasValue && parameters.Year.HasValue && parameters.Month.HasValue &&
                    parameters.Day.HasValue)
                {
                    parameters.ContractDate = new DateTime(parameters.Year.Value, parameters.Month.Value, parameters.Day.Value);
                }

                var contracts = new GetContractListQuery(context).Execute(parameters);

                // Если в параметрах запроса указан идентификатор, то ищем по id 
                // и возвращаем контракт, либо NULL если такого контракта не существует
                if (parameters.ContractId.HasValue)
                    return contracts.FirstOrDefault();

                if (contracts.Any())
                    return contracts.First();

                // Если контракта не существует, то нужно его сначала создать, а потом вернуть
                // Но только в том случае, если все необходимые для этого параметры переданы
                if (parameters.ContractDate.HasValue && parameters.SystemId.HasValue &&
                    parameters.PeriodTypeId.HasValue && parameters.TargetId.HasValue)
                {
                    var id = new AddContractCommand(context).Execute(
                        new AddContractParameterSet
                        {
                            ContractDate = parameters.ContractDate.Value,
                            GasTransportSystemId = parameters.SystemId.Value,
                            PeriodTypeId = parameters.PeriodTypeId.Value,
                            TargetId = parameters.TargetId.Value,
                            IsFinal = parameters.IsFinal ?? false
                        });

                    var newContact = new GetContractListQuery(context).Execute(
                        new GetContractListParameterSet {ContractId = id});

                    return newContact.FirstOrDefault();
                }

                return null;
            }
        }


        #region VALUES

        public List<BalanceValueDTO> GetBalanceValues(int parameters)
        {
            using (var context = OpenDbContext())
            {
                var valueList = new GetBalanceValueListQuery(context).Execute(new GetBalanceValueListParameterSet { ContractId = parameters });
                var irregularityList = new GetIrregularityListQuery(context).Execute(parameters);
                var correctionList = new GetCorrectionListQuery(context).Execute(parameters);
                
                valueList.ForEach(v => v.IrregularityList = irregularityList.Where(i => i.BalanceValueId == v.Id).ToList());
                valueList.ForEach(v => v.CorrectionList = correctionList.Where(i => i.BalanceValueId == v.Id).ToList());
                return valueList;
            }
        }


        public void SaveBalanceValues(SaveBalanceValuesParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                // Сначала нужно удалить все сохраненные значения
                new CleanContractCommand(context).Execute(parameters.ContractId);

                foreach (var value in parameters.ValueList)
                {
                    var valueId = new SetBalanceValueCommand(context).Execute(value);

                    if (value.IrregularityList != null)
                        foreach (var irrty in value.IrregularityList)
                        {
                            irrty.BalanceValueId = valueId;
                            new SetIrregularityCommand(context).Execute(irrty);
                        }

                    if (value.CorrectionList != null)
                        foreach (var corr in value.CorrectionList)
                        {
                            corr.BalanceValueId = valueId;
                            new SetCorrectionCommand(context).Execute(corr);
                        }
                }
                
                parameters.SwapList.ForEach(s => new AddSwapCommand(context).Execute(s));
            }
        }

        public void ClearBalanceValues(ClearBalanceValuesParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var values = new GetBalanceValueListQuery(context).Execute(
                    new GetBalanceValueListParameterSet
                    {
                        ContractId = parameters.ContractId,
                        SiteId = parameters.SiteId
                    });

                var selectedValues =
                    values.Where(
                        v =>
                            parameters.BalanceItemList.Contains(v.BalanceItem) &&
                            parameters.OwnerIdList.Contains(v.GasOwnerId));

                foreach (var value in selectedValues)
                {
                    new SetBalanceValueCommand(context).Execute(
                        new SetBalanceValueParameterSet
                        {
                            ContractId = value.ContractId,
                            BalanceItem = value.BalanceItem,
                            GasOwnerId = value.GasOwnerId,
                            EntityId = value.EntityId,
                            BaseValue = null
                        });
                }
            }
        }


        public void SetBalanceValue(SetBalanceValueParameterSet parameters)
        {
            ExecuteRead<SetBalanceValueCommand, int, SetBalanceValueParameterSet>(parameters);
        }


        #endregion



        #region VALUE SWAPS

        public List<SwapDTO> GetValueSwaps(int parameters)
        {
            return ExecuteRead<GetSwapListQuery, List<SwapDTO>, int>(parameters);
        }

        #endregion



        #region МАРШРУТЫ

        public int SetRoute(SetRouteParameterSet parameters)
        {
            return ExecuteRead<SetRouteCommand, int, SetRouteParameterSet>(parameters);


            //using (var context = OpenDbContext())
            //{
            //    var systemId = new GetMeasStationByIdQuery(context).Execute(parameters.InletId).SystemId;
            //    var graph = (new GraphBuilder(systemId, AppSettingsManager.CurrentEnterpriseId, context)).BuildGraph();

            //    var route = new MinRoute(graph, parameters.InletId).GetMinRoute(parameters.OutletId);

            //    var routeId = new int();

            //    if (route.Any())
            //    {
            //        routeId = new SetRouteCommand(context).Execute(new SetRouteParameterSet
            //        {
            //            InletId = parameters.InletId,
            //            OutletId = parameters.OutletId,
            //            Length = parameters.Length
            //        });

            //        foreach (var routeSection in route)
            //        {
            //            new AddRouteSectionCommand(context).Execute(new AddRouteSectionParameterSet
            //            {
            //                RouteId = routeId,
            //                PipelineId = routeSection.PipelineId,
            //                KilometerStart = routeSection.KilometerStart,
            //                KilometerEnd = routeSection.KilometerEnd
            //            });
            //        }
            //    }

            //    return routeId;
            //}
        }
        

        public List<RouteDTO> GetRoutesList(GetRouteListParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var routes = new GetRouteListQuery(context).Execute(parameters);
                //if (routes.Any(r => r.RouteId.HasValue))
                //{
                //    var sections =
                //        new GetSectionListQuery(context).Execute(
                //            routes.Where(r => r.RouteId.HasValue).Select(r => r.RouteId.Value).ToList());
                //    routes.ForEach(r => r.SectionList = sections.Where(s => s.RouteId == r.RouteId).ToList());
                //}
                return routes;
            }
        }
        

        public int AddRouteSection(AddRouteSectionParameterSet parameters)
        {
            return ExecuteRead<AddRouteSectionCommand, int, AddRouteSectionParameterSet>(parameters);
        }

        public void AddAllRoutes(GetRouteListParameterSet parameters)
        {
            //using (var context = OpenDbContext())
            //{
            //    var graph =
            //        (new GraphBuilder(parameters.SystemId, AppSettingsManager.CurrentEnterpriseId, context)).BuildGraph();

            //    if (!graph.EdgeList.Any() || !graph.VertexList.Any()) return;

            //    var entitiesTo = new List<EntityDTO>();

            //    if (parameters.OutletEntityType == EntityType.DistrStation)
            //    {
            //        entitiesTo.AddRange(
            //            new GetDistrStationListQuery(context).Execute(new GetDistrStationListParameterSet
            //            {
            //                SystemId = parameters.SystemId
            //            }));
            //    }

            //    if (parameters.OutletEntityType == EntityType.CompStation)
            //    {
            //        entitiesTo.AddRange(new GetCompStationListQuery(context).Execute(new GetCompStationListParameterSet
            //        {
            //            SystemId =
            //                parameters
            //                    .SystemId
            //        }));
            //    }

            //    if (parameters.OutletEntityType == EntityType.MeasStation)
            //    {
            //        entitiesTo.AddRange(new GetMeasStationListQuery(context).Execute(new GetMeasStationListParameterSet
            //        {
            //            SystemId =
            //                parameters.SystemId
            //        })
            //            .Where(st => st.BalanceSignId == Sign.Out));
            //    }

            //    var entitiesIdTo = entitiesTo.Select(e => e.Id).ToList();

            //    var routes = new MinRoute(graph, parameters.InletMeasStationId).GetMinRoute(entitiesIdTo).ToList();

            //    foreach (var route in routes)
            //    {
            //        if (route != null)
            //        {
            //            var routeSave = new GetRouteListQuery(context).Execute(parameters).SingleOrDefault(r => r.OutletId == entitiesIdTo[routes.IndexOf(route)]);

            //            if (routeSave != null && routeSave.RouteId.HasValue)
            //            {
            //                new DeleteRouteCommand(context).Execute(routeSave.RouteId.Value);
            //            }

            //            var routeId = new SetRouteCommand(context).Execute(new SetRouteParameterSet
            //            {
            //                InletId = parameters.InletMeasStationId,
            //                OutletId = entitiesIdTo[routes.IndexOf(route)]
            //            });

            //            foreach (var routeSection in route)
            //            {
            //                var sectionId = new AddRouteSectionCommand(context).Execute(
            //                    new AddRouteSectionParameterSet
            //                    {
            //                        RouteId = routeId,
            //                        PipelineId = routeSection.PipelineId,
            //                        KilometerStart = routeSection.KilometerStart,
            //                        KilometerEnd = routeSection.KilometerEnd,
            //                        SortOrder = routeSection.SortOrder
            //                    });
            //            }
            //        }
            //    }
            //}
        }

        public int AddRouteException(AddRouteExceptionParameterSet parameters)
        {
            return ExecuteRead<AddRouteExceptionCommand, int, AddRouteExceptionParameterSet>(parameters);
        }

        public void EditRouteException(EditRouteExceptionParameterSet parameters)
        {
            ExecuteNonQuery<EditRouteExceptionCommand, EditRouteExceptionParameterSet>(parameters);
        }

        public void DeleteRouteException(int parameters)
        {
            ExecuteNonQuery<DeleteRouteExceptionCommand, int>(parameters);
        }
        #endregion



        #region ДОКУМЕНТЫ В ПЛАНЕ ТРАНСПОРТА

        public List<DocDTO> GetDocList(int? parameters)
        {
            return ExecuteRead<GetDocListQuery, List<DocDTO>, int?>(parameters);
        }

        public int AddDoc(AddDocParameterSet parameters)
        {
            return ExecuteRead<AddDocCommand, int, AddDocParameterSet>(parameters);
        }

        public void EditDoc(EditDocParameterSet parameters)
        {
            ExecuteNonQuery<EditDocCommand, EditDocParameterSet>(parameters);
        }

        public void DeleteDoc(int parameters)
        {
            ExecuteNonQuery<DeleteDocCommand, int>(parameters);
        }

        #endregion

		

        #region ТТР

        public List<TransportDTO> GetTransportList(HandleTransportListParameterSet parameters)
        {
            return ExecuteRead<GetTransportListQuery, List<TransportDTO>, HandleTransportListParameterSet>(parameters);
        }

        public void CalculateTransportList(HandleTransportListParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                new ClearTransportListCommand(context).Execute(parameters);

                TransportAlgorithm.Run(context, parameters.ContractId);
            }
        }

        // Удаляет все результаты расчета ТТР (ТТР и запас на конец месяца)
        public void CLearTransportResults(HandleTransportListParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                new ClearTransportListCommand(context).Execute(parameters);

                var values = new GetBalanceValueListQuery(context).Execute(
                    new GetBalanceValueListParameterSet { ContractId = parameters.ContractId });

                // ОЧИСИТЬ ЗАПАС
                foreach (var value in values.Where(v => v.BalanceItem == BalanceItem.GasSupply))
                {
                    new SetBalanceValueCommand(context).Execute(
                        new SetBalanceValueParameterSet
                        {
                            ContractId = value.ContractId,
                            BalanceItem = value.BalanceItem,
                            GasOwnerId = value.GasOwnerId,
                            EntityId = value.EntityId,
                            BaseValue = value.BaseValue,
                            Correction = 0
                        });
                }
            }
        }

        #endregion

        #region SORT ORDER

        public List<BalSortOrderDTO> GetSortOrderList()
        {
            return ExecuteRead<GetSortOrderListQuery, List<BalSortOrderDTO>>();
        }


        public void SetSortOrder(SetBalSortOrderParameterSet parameters)
        {
            ExecuteNonQuery<SetSortOrderCommand, SetBalSortOrderParameterSet>(parameters);
        }

        #endregion


        #region ВКЛАДКА СПРАВОЧНЫХ ОБЪЕКТОВ

        public List<CommonEntityDTO> GetMiscTabEntityList(int parameters)
        {
            return ExecuteRead<GetMiscTabEntityListQuery, List<CommonEntityDTO>, int>(parameters);
        }

        public void AddMiscTabEntity(AddRemoveMiscTabEntityParameterSet parameters)
        {
            ExecuteNonQuery<AddMiscTabEntityCommand, AddRemoveMiscTabEntityParameterSet>(parameters);
        }

        public void RemoveMiscTabEntity(AddRemoveMiscTabEntityParameterSet parameters)
        {
            ExecuteNonQuery<RemoveMiscTabEntityCommand, AddRemoveMiscTabEntityParameterSet>(parameters);
        }

        #endregion


        #region BALANCE GROUPS

        public List<BalanceGroupDTO> GetBalanceGroupList(int parameters)
        {
            return ExecuteRead<GetBalanceGroupListQuery, List<BalanceGroupDTO>, int>(parameters);
        }

        public int AddBalanceGroup(AddBalanceGroupParameterSet parameters)
        {
            return ExecuteRead<AddBalanceGroupCommand, int, AddBalanceGroupParameterSet>(parameters);
        }

        public void EditBalanceGroup(EditBalanceGroupParameterSet parameters)
        {
            ExecuteNonQuery<EditBalanceGroupCommand, EditBalanceGroupParameterSet>(parameters);
        }

        public void RemoveBalanceGroup(int parameters)
        {
            ExecuteNonQuery<DeleteBalanceGroupCommand, int>(parameters);
        }

        #endregion


        #region ГАЗОРАСПРЕДЕЛИТЕЛЬНЫЕ ОРГАНИЗАЦИИ

        public List<DistrNetworkDTO> GetDistrNetworkList()
        {
            return ExecuteRead<GetDistrNetworkListQuery, List<DistrNetworkDTO>>();
        }

        public int AddDistrNetwork(AddDistrNetworkParameterSet parameters)
        {
            return ExecuteRead<AddDistrNetworkCommand, int, AddDistrNetworkParameterSet>(parameters);
        }

        public void EditDistrNetwork(EditDistrNetworkParameterSet parameters)
        {
            ExecuteNonQuery<EditDistrNetworkCommand, EditDistrNetworkParameterSet>(parameters);
        }

        public void RemoveDistrNetwork(int parameters)
        {
            ExecuteNonQuery<DeleteDistrNetworkCommand, int>(parameters);
        }

        #endregion


        #region INPUT STATES
        public List<InputStateDTO> GetInputStateList(GetInputStateListParameterSet parameters)
        {
            return ExecuteRead<GetInputStateListQuery, List<InputStateDTO>, GetInputStateListParameterSet>(parameters);
        }

        public void SetInputState(SetInputStateParameterSet parameters)
        {
            ExecuteNonQuery<SetInputStateCommand, SetInputStateParameterSet>(parameters);
        }
        #endregion


        #region ДАННЫЕ ДЛЯ СУТОЧНОГО БАЛАНСА

        public DayBalanceDataDTO GetDayBalanceData(GetDayBalanceDataParameterSet parameters)
        {
            var data = new DayBalanceDataDTO();
            var day = new DateTime(parameters.Year, parameters.Month, parameters.Day);

            using (var context = OpenDbContext())
            {
                data.Serie = new GetSeriesQuery(context).Execute(
                    new GetSeriesParameterSet
                    {
                        PeriodType = PeriodType.Day,
                        TimeStamp = day
                    });

                data.BalanceGroups = new GetBalanceGroupListQuery(context).Execute(parameters.SystemId);

                data.SortOrderList = new GetSortOrderListQuery(context).Execute();

                data.Sites = new GetSiteListQuery(context).Execute(
                    new GetSiteListParameterSet
                    {
                        SystemId = parameters.SystemId,
                        EnterpriseId = AppSettingsManager.CurrentEnterpriseId
                    });

                data.Enterprises = new GetEnterpriseListQuery(context).Execute(null);

                data.MeasStations = new GetMeasStationListQuery(context).Execute(
                    new GetMeasStationListParameterSet
                    {
                        SystemId = parameters.SystemId,
                        EnterpriseId = AppSettingsManager.CurrentEnterpriseId
                    });

                data.MeasLines = new GetMeasLineListQuery(context).Execute(
                    new GetMeasLineListParameterSet
                    {
                        MeasStationList = data.MeasStations.Select(s => s.Id).ToList()
                    });

                data.DistrStations = new GetDistrStationListQuery(context).Execute(
                    new GetDistrStationListParameterSet
                    {
                        SystemId = parameters.SystemId,
                        EnterpriseId = AppSettingsManager.CurrentEnterpriseId,
                        UseInBalance = true
                    });

                data.DistrStationOutlets = new GetDistrStationOutletListQuery(context).Execute(
                    new GetDistrStationOutletListParameterSet
                    {
                        SystemId = parameters.SystemId
                    });

                data.OperConsumers = new GetOperConsumerListQuery(context).Execute(
                    new GetOperConsumerListParameterSet
                    {
                        SystemId = parameters.SystemId
                    });
                
                data.AuxCosts = new GetGasCostListQuery(context).Execute(
                    new GetGasCostListParameterSet
                    {
                        StartDate = day == day.MonthStart() ? day.AddDays(-1) : day.MonthStart(),
                        EndDate = day,
                        Target = Target.Fact,
                        SystemId = parameters.SystemId
                    });

                data.Aggregators = new GetAggregatorListQuery(context).Execute(
                    new GetAggregatorListParameterSet
                    {
                        SystemId = parameters.SystemId
                    });

                
                data.MiscTabEntities = new GetMiscTabEntityListQuery(context).Execute(parameters.SystemId);


                data.FactValues = new GetEntityPropertyValueListQuery(context).Execute(
                new GetEntityPropertyValueListParameterSet
                {
                    StartDate = day == day.MonthStart() ? day.AddDays(-1) : day.MonthStart(),
                    EndDate = day,
                    PeriodType = PeriodType.Day,
                    CreateEmpty = false,
                    PropertyList = { PropertyType.Flow }
                });

                var contract = (new GetContractListQuery(context).Execute(
                    new GetContractListParameterSet
                    {
                        ContractDate = day.Date.MonthStart(),
                        SystemId = parameters.SystemId,
                        PeriodTypeId = PeriodType.Month,
                        TargetId = Target.Plan
                    })).FirstOrDefault();

                if (contract != null)
                    data.PlanValues = GetBalanceValues(contract.Id);
            }

            return data;
        }

        // Суточное значение для балансов и прочих форм по учету газа
        public Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> GetBalanceMeasurings(
            GetBalanceMeasuringsParameterSet parameters)
        {
            var date = new DateTime(parameters.Year, parameters.Month, parameters.Day);
            var pSet = new GetEntityPropertyValueListParameterSet
            {
                StartDate = parameters.Day == 1 ? date.AddDays(-1) : date.MonthStart(),
                EndDate = date,
                PeriodType = PeriodType.Day,
                CreateEmpty = false,
                PropertyList = { PropertyType.Flow }
            };

            return
                ExecuteRead
                    <GetEntityPropertyValueListQuery,
                        Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>>,
                        GetEntityPropertyValueListParameterSet>(pSet);
        }

        #endregion


        #region ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ ДЛЯ ФОРМИРОВАНИЯ МЕСЯЧНОГО БАЛАНСА

        public void RunDivideVolumeAlgorithm(DivideVolumeAlgorithmParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                DivideVolumeAlgorithm.Run(context, parameters);
            }
        }

        public void RunOwnersDaySumAlgorithm(OwnersDaySumAlgorithmParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                OwnersDaySumAlgorithm.Run(context, parameters);
            }
        }

        public void RunMoveGasSupplyAlgorithm(int parameters)
        {
            using (var context = OpenDbContext())
            {
                MoveGasSupplyAlgorithm.Run(context, parameters);
            }
        }

        public void MoveAuxCosts(MoveAuxCostsParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var contract = new GetContractListQuery(context).Execute(
                    new GetContractListParameterSet {ContractId = parameters.ContractId}).FirstOrDefault();

                if (contract == null) return;

                var costs = new GetGasCostListQuery(context).Execute(
                    new GetGasCostListParameterSet
                    {
                        SystemId = contract.SystemId,
                        Target = contract.TargetId,
                        StartDate = contract.ContractDate,
                        EndDate = contract.ContractDate.MonthEnd()
                    }).Sum(c => c.Volume);

                new SetBalanceValueCommand(context).Execute(
                    new SetBalanceValueParameterSet
                    {
                        ContractId = contract.Id,
                        GasOwnerId = parameters.OwnerId,
                        EntityId = AppSettingsManager.CurrentEnterpriseId,
                        BalanceItem = BalanceItem.AuxCosts,
                        BaseValue = costs
                    });
            }
        }
        #endregion


        #region СОХРАНЕНИЕ ФИНАЛЬНОЙ ВЕРСИИ

        // Копирует значения из одной версии контракта в другую
        public void MoveValuesToOtherVersion(MoveValuesToOtherVersionParameterSet parameters)
        {
            using (var context = OpenDbContext())
            {
                var draftContract = GetContract(new GetContractListParameterSet {ContractId = parameters.ContractId});
                if (draftContract == null) return;

                var finalContract = GetContract(
                    new GetContractListParameterSet
                    {
                        ContractDate = draftContract.ContractDate,
                        PeriodTypeId = draftContract.PeriodTypeId,
                        SystemId = draftContract.SystemId,
                        TargetId = draftContract.TargetId,
                        IsFinal = !draftContract.IsFinal
                    });

                if (finalContract == null) return;

                // Копирование договоров
                var docList = GetDocList(parameters.ToFinal ? draftContract.Id : finalContract.Id);
                foreach (var doc in docList)
                {
                }

                // Копирование значений
                var values = GetBalanceValues(parameters.ToFinal ? draftContract.Id : finalContract.Id);
                var saveSet = new SaveBalanceValuesParameterSet
                {
                    ContractId = parameters.ToFinal ? finalContract.Id : draftContract.Id
                };

                foreach (var value in values)
                {
                    var pSet = new SetBalanceValueParameterSet
                    {
                        ContractId = parameters.ToFinal ? finalContract.Id : draftContract.Id,
                        EntityId = value.EntityId,
                        GasOwnerId = value.GasOwnerId,
                        BalanceItem = value.BalanceItem,
                        BaseValue = value.BaseValue,
                        Correction = value.Correction
                    };

                    pSet.IrregularityList = value.IrregularityList.Select(i =>
                        new SetIrregularityParameterSet
                        {
                            StartDayNum = i.StartDayNum,
                            EndDayNum = i.EndDayNum,
                            Value = i.Value
                        }).ToList();

                    //pSet.CorrectionList = value.CorrectionList.Select(c =>
                    //    new SetCorrectionParameterSet
                    //    {
                    //        DocId = c.DocId,
                    //        Value = c.Value
                    //    }).ToList();

                    saveSet.ValueList.Add(pSet);
                }
                SaveBalanceValues(saveSet);


                // Копирование ТТР
                var transports = GetTransportList(
                    new HandleTransportListParameterSet
                    {
                        ContractId = parameters.ToFinal ? draftContract.Id : finalContract.Id
                    });


                new ClearTransportListCommand(context).Execute(
                    new HandleTransportListParameterSet
                    {
                        ContractId = parameters.ToFinal ? finalContract.Id : draftContract.Id
                    });

                foreach (var transport in transports)
                {
                    new AddTransportCommand(context).Execute(
                        new AddTransportParameterSet
                        {
                            ContractId = parameters.ToFinal ? finalContract.Id : draftContract.Id,
                            OwnerId = transport.OwnerId,
                            InletId = transport.InletId,
                            OutletId = transport.OutletId,
                            BalanceItem = transport.BalanceItem,
                            Volume = transport.Volume,
                            Length = transport.Length,
                            RouteId = transport.RouteId
                        });
                }
            }
        }
        

        #endregion
    }
}
