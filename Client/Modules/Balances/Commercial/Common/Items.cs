using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Converters;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Balances.Commercial.Common
{
    public class ItemBase : PropertyChangedBase
    {
        protected ItemActions Actions;

        public ItemBase(ItemActions actions)
        {
            Actions = actions;
            Childs = new List<ItemBase>();
        }

        public virtual string Name { get; set; }
        

        #region VALUES

        public virtual double? PlanBase { get; set; }
        public virtual double? PlanCorrected { get; set; }
        public double? PlanCorrectedDelta => PlanCorrected - PlanBase;
        public double? PlanSummarized => PlanCorrected ?? PlanBase;
        

        public virtual double? FactBase { get; set; }
        public virtual double? FactCorrected { get; set; }
        public double? FactCorrectedDelta => FactCorrected - FactBase;
        public double? FactSummarized => FactCorrected ?? FactBase;
        
        public double? PlanFactDelta => FactSummarized - PlanSummarized;
        
        #endregion


        
        // Возвращает сумму для выбранного поставщика
        public virtual double? GetOwnerSum(int ownerId, BalanceItem balItem, Target target)
        {
            return null;
        }


        
        #region CHILDS

        public List<ItemBase> Childs { get; set; }

        public void AddChild(ItemBase item)
        {
            Childs.Add(item);
            item.ValueChanged = OnChildValueChanged;
        }

        public void AddChildList(IEnumerable<ItemBase> itemList)
        {
            foreach (var item in itemList)
            {
                Childs.Add(item);
                item.ValueChanged = OnChildValueChanged;
            }
        }

        protected virtual void OnChildValueChanged(BalValueType type)
        {
            NotifyValueChanged(type);
        }

        #endregion
        
        public Action<BalValueType> ValueChanged { get; set; }

        protected void NotifyValueChanged(BalValueType type)
        {
            ValueChanged?.Invoke(type);
        }


        public virtual void UpdateActions() {}


        public virtual bool IsReadOnly => true;

        public bool IsExpanded { get; set; }

        // Внешний вид
        #region APPEARANCE

        public virtual ImageSource ImgSrc { get; set; }

        public virtual FontStyle FontStyle { get; set; }

        public virtual FontWeight FontWeight { get; set; }

        public virtual bool IsContextMenuEnabled => false;
        
        #endregion
    }

    public enum BalValueType
    {
        PlanBase = 1,
        PlanCorrected = 2,
        FactBase = 3,
        FactCorrected = 4
    }

    


    /// Класс описывающий владельца газа - самый нижний элемент модели
    public class OwnerItem : ItemBase
    {
        public OwnerItem(GasOwnerDTO owner, EntityDTO entity, BalanceItem balItem, ItemActions actions)
            : base(actions)
        {
            Owner = owner;
            Entity = entity;
            BalItem = balItem;
        }

        public GasOwnerDTO Owner { get; }
        public BalanceItem BalItem { get; }
        public EntityDTO Entity { get; }

        public override string Name => Owner.Name;
        
        public override FontStyle FontStyle => FontStyles.Italic;

        public override bool IsReadOnly => false;


        #region VALUES

        public virtual Target Target { get; set; }

        public bool HasValues => Target == Target.Plan
           ? PlanSummarized.HasValue && !(PlanSummarized == 0 && PlanBase == 0)
           : FactSummarized.HasValue && !(FactSummarized == 0 && FactBase == 0);


        public virtual void InitValues(BalanceValues values, Target target, double coef)
        {
            var val = values.GetValue(Entity.Id, Owner.Id, BalItem, coef);
            if (val == null) return;
            switch (target)
            {
                case Target.Plan:
                    PlanBase = val.BaseValue;
                    PlanCorrected = val.Correction;

                    return;
                case Target.Fact:
                    FactBase = val.BaseValue;
                    FactCorrected = val.Correction;
                    return;
            }
        }

        public virtual SetBalanceValueParameterSet GetSetValueParamSet(int contractId, double coef)
        {
            var baseVal = Math.Round((Target == Target.Plan ? PlanBase : FactBase) / coef ?? 0, 3);
            var corVal = Math.Round((Target == Target.Plan ? PlanCorrected : FactCorrected) / coef ?? 0, 3);

            return baseVal > 0 || corVal > 0 ? 
                new SetBalanceValueParameterSet
                    {
                        ContractId = contractId,
                        EntityId = Entity.Id,
                        GasOwnerId = Owner.Id,
                        BalanceItem = BalItem,
                        BaseValue = (Target == Target.Plan ? PlanBase : FactBase)/coef ?? 0,
                        Correction = (Target == Target.Plan ? PlanCorrected : FactCorrected) / coef,
                    }
                    : null;
        }

        #endregion

        
        public override double? GetOwnerSum(int ownerId, BalanceItem balItem, Target target)
        {
            return Owner.Id == ownerId && BalItem == balItem ? 
                (target == Target.Plan ? PlanSummarized : FactSummarized) 
                : null;
        }


        public bool GetVibility(int systemId)
        {
            return
                !(!HasValues &&
                  (Owner.DisableList.Any(d => d.EntityId == Entity.Id && d.BalanceItem == BalItem) ||
                   Owner.SystemList.All(s => s != systemId)));
        }
    }
    

    public class SummaryItem : ItemBase
    {
        private readonly EntityDTO _entity;
        public SummaryItem(EntityDTO entity, BalanceItem balItem, bool isInOut, ItemActions actions)
            :base(actions)
        {
            _entity = entity;
            ShowHideOwnersCommand = new DelegateCommand(() => actions.ShowHideOwnerAction.Invoke(entity.Id, balItem),
                () => isInOut);
        }

        public SummaryItem(string alias, ItemActions actions)
            : base(actions)
        {
            Alias = alias;
        }
        
        public string Alias { get; set; }

        public override string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(Alias)) return Alias;
                return !string.IsNullOrEmpty(_entity.BalanceName) ? _entity.BalanceName : _entity.Name;
            }
        }

        public override ImageSource ImgSrc => EntityTypeToImageSourceConverter.Convert(_entity?.EntityType);

        public override bool IsContextMenuEnabled => _entity != null;

        public DelegateCommand ShowHideOwnersCommand { get; set; }



        #region VALUES

        public override double? PlanBase => Childs.Sum(v => v.PlanBase);

        public override double? PlanCorrected
        {
            get
            {
                var sum = Childs.Sum(v => v.PlanSummarized);
                return sum == PlanBase ? null : sum;
            }
        }

        public override double? FactBase => Childs.Sum(v => v.FactBase);

        public override double? FactCorrected
        {
            get
            {
                var sum = Childs.Sum(v => v.FactSummarized);
                return sum == FactBase ? null : sum;
            }
        }

        public override double? GetOwnerSum(int ownerId, BalanceItem balItem, Target target)
        {
            return Childs.Sum(c => c.GetOwnerSum(ownerId, balItem, target));
        }

        #endregion
    }
}