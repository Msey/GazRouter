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
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DAL.SeriesData.PropertyValues;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.MonthAlgorithms;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;
using Utils.ValueExtrators;

namespace GazRouter.DataServices.Balances.Algorithms.MoveGasSupply
{
    public class MoveGasSupplyAlgorithm
    {
        public static void Run(ExecutionContextReal context, int contractId)
        {
            var contract = (new GetContractListQuery(context).Execute(
                new GetContractListParameterSet
                {
                    ContractId = contractId
                })).FirstOrDefault();

            if (contract == null) return;

            var prevMonth = contract.ContractDate.AddMonths(-1);

            var prevContract = (new GetContractListQuery(context).Execute(
                new GetContractListParameterSet
                {
                    ContractDate = prevMonth,
                    SystemId = contract.SystemId,
                    PeriodTypeId = PeriodType.Month,
                    TargetId = Target.Fact,
                    IsFinal = true
                })).FirstOrDefault();

            if (prevContract == null) return;

            var curValues =
                new GetBalanceValueListQuery(context).Execute(new GetBalanceValueListParameterSet {ContractId = contract.Id})
                    .Where(v => v.BalanceItem == BalanceItem.GasSupply)
                    .ToList();
            var prevValues =
                new GetBalanceValueListQuery(context).Execute(new GetBalanceValueListParameterSet { ContractId = prevContract.Id })
                    .Where(v => v.BalanceItem == BalanceItem.GasSupply)
                    .ToList();

            // Обнулить текущие значения по запасу
            foreach (var val in curValues)
            {
                var saveSet = new SetBalanceValueParameterSet
                {
                    ContractId = contract.Id,
                    EntityId = val.EntityId,
                    GasOwnerId = val.GasOwnerId,
                    BalanceItem = BalanceItem.GasSupply,
                    BaseValue = null,
                    Correction = null
                };
                new SetBalanceValueCommand(context).Execute(saveSet);
            }


            foreach (var val in prevValues)
            {
                var saveSet = new SetBalanceValueParameterSet
                {
                    ContractId = contract.Id,
                    EntityId = val.EntityId,
                    GasOwnerId = val.GasOwnerId,
                    BalanceItem = BalanceItem.GasSupply,
                    BaseValue = val.Correction
                };
                new SetBalanceValueCommand(context).Execute(saveSet);
            }
        }
    }
}

