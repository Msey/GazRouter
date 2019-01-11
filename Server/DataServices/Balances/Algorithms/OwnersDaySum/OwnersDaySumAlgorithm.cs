using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.Balances.Contracts;
using GazRouter.DAL.Balances.Values;
using GazRouter.DAL.Core;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.MonthAlgorithms;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Targets;
using Utils.Extensions;

namespace GazRouter.DataServices.Balances.Algorithms.OwnersDaySum
{
    public class OwnersDaySumAlgorithm
    {
        public static void Run(ExecutionContextReal context, OwnersDaySumAlgorithmParameterSet parameters)
        {
            var contract = (new GetContractListQuery(context).Execute(
                new GetContractListParameterSet
                {
                    ContractId = parameters.ContractId
                })).FirstOrDefault();

            if (contract == null) return;

            var pSetList = new List<SetBalanceValueParameterSet>();

            for(var day = 1; day <= contract.ContractDate.DaysInMonth(); day++)
            {
                var dayContract = (new GetContractListQuery(context).Execute(
                    new GetContractListParameterSet
                    {
                        ContractDate = new DateTime(contract.ContractDate.Year, contract.ContractDate.Month, day),
                        SystemId = contract.SystemId,
                        TargetId = Target.Fact,
                        PeriodTypeId = PeriodType.Day,
                        IsFinal = false
                    })).FirstOrDefault();

                if (dayContract == null) continue;

                var dayValues = new GetBalanceValueListQuery(context).Execute(new GetBalanceValueListParameterSet { ContractId = dayContract.Id });

                foreach (var val in dayValues)
                {
                    var pSet =
                        pSetList.SingleOrDefault(
                            s =>
                                s.EntityId == val.EntityId && s.GasOwnerId == val.GasOwnerId &&
                                s.BalanceItem == val.BalanceItem);

                    if (pSet == null)
                    {
                        pSet = new SetBalanceValueParameterSet
                        {
                            ContractId = contract.Id,
                            EntityId = val.EntityId,
                            GasOwnerId = val.GasOwnerId,
                            BalanceItem = val.BalanceItem,
                            BaseValue = 0
                        };
                        pSetList.Add(pSet);
                    }

                    pSet.BaseValue += val.BaseValue;
                }
            }

            var saveCmd = new SetBalanceValueCommand(context);
            pSetList.ForEach(s => saveCmd.Execute(s));
        }
    }
}

