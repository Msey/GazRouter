using System.Collections.Generic;
using GazRouter.Balances.Commercial.Common;
using GazRouter.Balances.Commercial.Summary;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.Targets;

namespace GazRouter.Balances.Commercial.OwnersSummary
{
    public class OwnersSummaryViewModel : ViewModelBase
    {
        public OwnersSummaryViewModel()
        {
            Items = new List<OwnerSummary>();
        }
       
        public void UpdateSummary(IEnumerable<GasOwnerDTO> owners, BalanceSummary summary, Target target)
        {
            Items = new List<OwnerSummary>();

            foreach (var owner in owners)
            {
                Items.Add(new OwnerSummary
                {
                    OwnerName = owner.Name,
                    Values = new SummaryValues
                    {
                        Intake = summary.GetOwnerValue(BalanceItem.Intake, target, owner.Id) ?? 0,
                        PipeMinus = summary.GetOwnerValue(BalanceItem.PipeMinus, target, owner.Id) ?? 0,
                        Transit = summary.GetOwnerValue(BalanceItem.Transit, target, owner.Id) ?? 0,
                        Consumers = summary.GetOwnerValue(BalanceItem.Consumers, target, owner.Id) ?? 0,
                        AuxCosts = summary.GetOwnerValue(BalanceItem.AuxCosts, target, owner.Id) ?? 0,
                        BalanceLoss = summary.GetOwnerValue(BalanceItem.BalanceLoss, target, owner.Id) ?? 0,
                        OperConsumers = summary.GetOwnerValue(BalanceItem.OperConsumers, target, owner.Id) ?? 0,
                        PipePlus = summary.GetOwnerValue(BalanceItem.PipePlus, target, owner.Id) ?? 0
                    }
                });
            }

            OnPropertyChanged(() => Items);
        }

        public List<OwnerSummary> Items { get; set; }
        
    }



}
