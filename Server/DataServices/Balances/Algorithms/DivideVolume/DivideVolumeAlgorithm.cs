using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DAL.Balances.Contracts;
using GazRouter.DAL.Balances.GasOwners;
using GazRouter.DAL.Balances.Values;
using GazRouter.DAL.Core;
using GazRouter.DAL.GasCosts;
using GazRouter.DAL.ObjectModel.Consumers;
using GazRouter.DAL.ObjectModel.DistrStationOutlets;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DAL.ObjectModel.OperConsumers;
using GazRouter.DAL.SeriesData.PropertyValues;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.MonthAlgorithms;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.OperConsumers;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;
using Utils.ValueExtrators;

namespace GazRouter.DataServices.Balances.Algorithms.DivideVolume
{
    public class DivideVolumeAlgorithm
    {
        public static void Run(ExecutionContextReal context, DivideVolumeAlgorithmParameterSet pSet)
        {
            var contract = (new GetContractListQuery(context).Execute(
                new GetContractListParameterSet
                {
                    ContractId = pSet.ContractId
                })).FirstOrDefault();

            if (contract == null) return;

            var balValues = new GetBalanceValueListQuery(context).Execute(
                new GetBalanceValueListParameterSet {ContractId = contract.Id});

            var startDate = contract.ContractDate;
            var endDate = contract.PeriodTypeId == PeriodType.Month ? startDate.MonthEnd() : startDate;

            var values = new GetEntityPropertyValueListQuery(context).Execute(
                new GetEntityPropertyValueListParameterSet
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    PeriodType = PeriodType.Day,
                    CreateEmpty = false,
                    PropertyList = {PropertyType.Flow}
                });

            var valueExtractor = new EntityPropertyValueExtractor(values);

            var owners = new GetGasOwnerListQuery(context).Execute(contract.SystemId);


#region ПОСТУПЛЕНИЕ + ТРАНЗИТ 

            if (pSet.IntakeTransitFilter)
            {
                var measStations = new GetMeasStationListQuery(context).Execute(
                    new GetMeasStationListParameterSet
                    {
                        SystemId = contract.SystemId,
                        EnterpriseId = AppSettingsManager.CurrentEnterpriseId
                    });

                if (pSet.SiteId.HasValue)
                    measStations = measStations.Where(s => s.ParentId == pSet.SiteId).ToList();

                foreach (var station in measStations.Where(s => s.BalanceSignId != Sign.NotUse))
                {
                    var balItem = station.BalanceSignId == Sign.In ? BalanceItem.Intake : BalanceItem.Transit;
                    var oList =
                        owners.Where(o => !o.DisableList.Any(d => d.EntityId == station.Id && d.BalanceItem == balItem))
                            .ToList();
                    var ownerId = oList.Count == 1 ? oList.First().Id : pSet.DefaultOwnerId;

                    var balVal =
                        balValues.SingleOrDefault(
                            v => v.EntityId == station.Id && v.GasOwnerId == ownerId && v.BalanceItem == balItem);
                    var saveSet = new SetBalanceValueParameterSet
                    {
                        ContractId = contract.Id,
                        EntityId = station.Id,
                        GasOwnerId = ownerId,
                        BalanceItem = balItem,
                        BaseValue = valueExtractor.GetSum(station.Id, PropertyType.Flow),
                        Correction = balVal?.Correction
                    };
                    new SetBalanceValueCommand(context).Execute(saveSet);
                }
            }

#endregion


#region ПОТРЕБИТЕЛИ

            if (pSet.ConsumersFilter)
            {
                var distrStations = new GetDistrStationListQuery(context).Execute(
                    new GetDistrStationListParameterSet
                    {
                        EnterpriseId = AppSettingsManager.CurrentEnterpriseId,
                        UseInBalance = true,
                        SystemId = contract.SystemId
                    });

                if (pSet.SiteId.HasValue)
                    distrStations = distrStations.Where(s => s.ParentId == pSet.SiteId).ToList();

                var outlets = new GetDistrStationOutletListQuery(context).Execute(
                    new GetDistrStationOutletListParameterSet
                    {
                        SystemId = contract.SystemId
                    });

                var gasConsumers = new GetConsumerListQuery(context).Execute(
                    new GetConsumerListParameterSet
                    {
                        SystemId = contract.SystemId
                    });

                var balItem = BalanceItem.Consumers;

                foreach (var station in distrStations)
                {
                    var dsConsumers = gasConsumers.Where(c => c.ParentId == station.Id).ToList();
                    var dsOutlets = outlets.Where(o => o.ParentId == station.Id).ToList();

                    // Если потребитель (подключение) только одно на данной ГРС, то скинуть весь объем на это подключение
                    if (dsConsumers.Count == 1)
                    {
                        var consumer = dsConsumers.FirstOrDefault(c => c.DistrStationId == station.Id);
                        if (consumer == null) continue;
                        var ownerList =
                            owners.Where(
                                o => !o.DisableList.Any(d => d.EntityId == consumer.Id && d.BalanceItem == balItem))
                                .ToList();
                        var ownerId = ownerList.Count == 1 ? ownerList.First().Id : pSet.DefaultOwnerId;

                        var balVal =
                            balValues.SingleOrDefault(
                                v => v.EntityId == consumer.Id && v.GasOwnerId == ownerId && v.BalanceItem == balItem);

                        var saveSet = new SetBalanceValueParameterSet
                        {
                            ContractId = contract.Id,
                            EntityId = consumer.Id,
                            GasOwnerId = ownerId,
                            BalanceItem = balItem,
                            BaseValue = valueExtractor.GetSum(station.Id, PropertyType.Flow),
                            Correction = balVal?.Correction
                        };
                        new SetBalanceValueCommand(context).Execute(saveSet);
                        continue;
                    }

                    // В другом случае перебрасываем объемы с выходов на привязанные к ним потребители
                    foreach (var consumer in dsConsumers)
                    {
                        var outletSum =
                            dsOutlets.Where(o => o.ConsumerId == consumer.Id)
                                .Sum(o => valueExtractor.GetSum(o.Id, PropertyType.Flow));

                        var ownerList =
                            owners.Where(
                                o => !o.DisableList.Any(d => d.EntityId == consumer.Id && d.BalanceItem == balItem))
                                .ToList();
                        var ownerId = ownerList.Count == 1 ? ownerList.First().Id : pSet.DefaultOwnerId;

                        var balVal =
                            balValues.SingleOrDefault(
                                v => v.EntityId == consumer.Id && v.GasOwnerId == ownerId && v.BalanceItem == balItem);

                        var saveSet = new SetBalanceValueParameterSet
                        {
                            ContractId = contract.Id,
                            EntityId = consumer.Id,
                            GasOwnerId = ownerId,
                            BalanceItem = balItem,
                            BaseValue = outletSum,
                            Correction = balVal?.Correction
                        };
                        new SetBalanceValueCommand(context).Execute(saveSet);
                    }
                }
            }

#endregion


#region ПЭН

            if (pSet.OperConsumersFilter)
            {
                var operConsumers = new GetOperConsumerListQuery(context).Execute(
                    new GetOperConsumerListParameterSet
                    {
                        SystemId = contract.SystemId
                    });

                if (pSet.SiteId.HasValue)
                    operConsumers = operConsumers.Where(s => s.ParentId == pSet.SiteId).ToList();

                foreach (var consumer in operConsumers)
                {
                    var balItem = BalanceItem.OperConsumers;
                    var oList =
                        owners.Where(o => !o.DisableList.Any(d => d.EntityId == consumer.Id && d.BalanceItem == balItem))
                            .ToList();
                    var ownerId = oList.Count == 1 ? oList.First().Id : pSet.DefaultOwnerId;

                    var balVal =
                        balValues.SingleOrDefault(
                            v => v.EntityId == consumer.Id && v.GasOwnerId == ownerId && v.BalanceItem == balItem);

                    var saveSet = new SetBalanceValueParameterSet
                    {
                        ContractId = contract.Id,
                        EntityId = consumer.Id,
                        GasOwnerId = ownerId,
                        BalanceItem = balItem,
                        BaseValue = valueExtractor.GetSum(consumer.Id, PropertyType.Flow),
                        Correction = balVal?.Correction
                    };
                    new SetBalanceValueCommand(context).Execute(saveSet);
                }
            }

#endregion

        }
    }
}

