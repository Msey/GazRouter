using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.Balances.Commercial.Common;
using GazRouter.Common.Cache;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.Targets;
using Microsoft.Practices.ServiceLocation;
using Utils.Extensions;

namespace GazRouter.Balances.Commercial.Summary
{
    public class BalanceSummary
    {
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();
        private readonly List<GasOwnerDTO> _owners;
        private Dictionary<BalanceItem, SummaryItemBase> _items;
        public BalanceSummary(IEnumerable<GasOwnerDTO> owners)
        {
            _owners = owners?.ToList();
            _items = new Dictionary<BalanceItem, SummaryItemBase>();
        }
        
        public GroupSummaryItem Resources { get; set; }

        public GroupSummaryItem Distribution { get; set; }

        public GroupSummaryItem GasSupply { get; set; }


        public double? GetValue(BalanceItem balItem, Target target)
        {
            return target == Target.Plan
                ? _items?.GetOrDefault(balItem)?.PlanValue
                : _items?.GetOrDefault(balItem)?.FactValue;
        }

        public double? GetOwnerValue(BalanceItem balItem, Target target, int ownerId)
        {
            var ownerItem =
                _items?.GetOrDefault(balItem)?
                    .Childs.OfType<OwnerSummaryItem>()
                    .SingleOrDefault(o => o.OwnerId == ownerId);
            return target == Target.Plan ? ownerItem?.PlanValue : ownerItem?.FactValue;
        }

        public double BalanceDelta(Target target)
        {
            return
                _items.Sum(
                    item =>
                        (target == Target.Plan ? item.Value.PlanValue : item.Value.FactValue)*
                        ClientCache.DictionaryRepository.BalanceItems.Single(i => i.BalanceItem == item.Key).SignCoef) ?? 0;
        }


        public void CreateSummary(List<OwnerItem> ownerItems)
        {
            _items.Clear();
            foreach (var balItem in Enum.GetValues(typeof(BalanceItem)).OfType<BalanceItem>())
            {
                var item = CreateItem(balItem, ownerItems);
                _items.Add(balItem, item);
            }
            UpdateRootItems();
        }

        public void UpdateItem(BalanceItem balItem, List<OwnerItem> ownerItems)
        {
            _items[balItem] = CreateItem(balItem, ownerItems);
            if (balItem == BalanceItem.GasSupply)
            {
                _items[BalanceItem.PipeMinus] = CreateItem(BalanceItem.PipeMinus, ownerItems);
                _items[BalanceItem.PipePlus] = CreateItem(BalanceItem.PipePlus, ownerItems);
            }
            UpdateRootItems();
        }

        private GroupSummaryItem CreateItem(BalanceItem balItem, List<OwnerItem> ownerItems)
        {
            var item = new GroupSummaryItem {Name = BalanceItemToNameConverter.GetItemName(balItem)};

            switch (balItem)
            {
                case BalanceItem.PipePlus:
                {
                    var items = ownerItems.Where(i => i.BalItem == BalanceItem.GasSupply).ToList();
                    foreach (var owner in _owners)
                    {
                        var sum =
                            items.Where(i => i.Owner.Id == owner.Id)
                                .Sum(i => i.FactCorrected - (i.FactBase ?? 0) > 0 ? i.FactCorrected - (i.FactBase ?? 0) : 0);
                        var oItem = new OwnerSummaryItem
                        {
                            Name = owner.Name,
                            OwnerId = owner.Id,
                            FactValue = sum,
                            CalcDelta = false
                        };
                        item.Childs.Add(oItem);
                    }
                    item.CalcDelta = false;
                    break;
                }

                case BalanceItem.PipeMinus:
                {
                    var items = ownerItems.Where(i => i.BalItem == BalanceItem.GasSupply).ToList();
                    foreach (var owner in _owners)
                    {
                        var sum =
                            items.Where(i => i.Owner.Id == owner.Id)
                                .Sum(i => (i.FactBase ?? 0) - i.FactCorrected > 0 ? (i.FactBase ?? 0) - i.FactCorrected : 0);
                        var oItem = new OwnerSummaryItem
                        {
                            Name = owner.Name,
                            OwnerId = owner.Id,
                            FactValue = sum,
                            CalcDelta = false
                        };
                        item.Childs.Add(oItem);
                    }
                    item.CalcDelta = false;
                    break;
                }

                default:
                {
                    item.Childs.AddRange(
                        _owners.Select(
                            o =>
                                new OwnerSummaryItem
                                {
                                    Name = o.Name,
                                    OwnerId = o.Id,
                                    FactValue = ownerItems.Sum(i => i.GetOwnerSum(o.Id, balItem, Target.Fact)),
                                    PlanValue = ownerItems.Sum(i => i.GetOwnerSum(o.Id, balItem, Target.Plan)),
                                    CalcDelta = balItem != BalanceItem.GasSupply
                                }).Where(i => i.HasValues));
                    break;
                }
            }
            
            return item;
        }

        private void UpdateRootItems()
        {
            Resources = new GroupSummaryItem { Name = "РЕСУРСЫ"};
            Distribution = new GroupSummaryItem { Name = "РАСПРЕДЕЛЕНИЕ"};
            GasSupply = new GroupSummaryItem { Name = "ЗАПАС ГАЗА", CalcDelta = false};
            foreach (var item in _items)
            {
                var sign = ClientCache.DictionaryRepository.BalanceItems.Single(i => i.BalanceItem == item.Key).BalanceSign;
                if (sign == Sign.In) Resources.Childs.Add(item.Value);
                if (sign == Sign.Out) Distribution.Childs.Add(item.Value);
                if (item.Key == BalanceItem.GasSupply) GasSupply.Childs.AddRange(item.Value.Childs);
            }
        }


        public IEnumerable<SummaryItemBase> Tree
        {
            get
            {
                yield return Resources;
                yield return Distribution;
                yield return GasSupply;
            }
        } 
    }




    public class SummaryItemBase
    {
        public SummaryItemBase()
        {
            Childs = new List<SummaryItemBase>();
            CalcDelta = true;
        }
        
        public virtual string Name { get; set; }

        public virtual double? FactValue { get; set; }

        public virtual double? PlanValue { get; set; }


        public bool HasValues => FactValue.HasValue || PlanValue.HasValue;


        public bool CalcDelta { get; set; }

        public double? Delta => CalcDelta ? FactValue - PlanValue : null;

        public virtual FontStyle FontStyle => FontStyles.Normal;


        public List<SummaryItemBase> Childs { get; set; }

        public bool IsExpanded { get; set; }
    }

    public class OwnerSummaryItem : SummaryItemBase
    {
        public int OwnerId { get; set; }

        public override FontStyle FontStyle => FontStyles.Italic;
    }

    public class GroupSummaryItem : SummaryItemBase
    {
        public override double? FactValue => Childs.Sum(c => c.FactValue);
        public override double? PlanValue => Childs.Sum(c => c.PlanValue);
    }
}
