using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Balances.GasOwners;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceItems;

namespace GazRouter.Balances.Commercial.SiteInput
{
    public class ItemBase : PropertyChangedBase
    {
        public ItemBase()
        {
            Childs = new List<ItemBase>();
        }

        public virtual string Name { get; set; }
        
        public virtual double? Current { get; set; }

        public virtual double? Measured { get; set; }

        public double? MeasuredDelta => Current - Measured;


        public virtual double? Plan { get; set; }

        public double? PlanDelta => Current - Plan;

        

        // Дочерние элементы

        #region CHILDS

        public List<ItemBase> Childs { get; set; }

        public void AddChild(ItemBase item)
        {
            Childs.Add(item);
            item.PropertyChanged += OnChildChanged;
        }

        public void AddChilds(IEnumerable<ItemBase> itemList)
        {
            foreach (var item in itemList)
            {
                Childs.Add(item);
                item.PropertyChanged += OnChildChanged;
            }
        }

        protected virtual void OnChildChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(() => Current);
            OnPropertyChanged(() => MeasuredDelta);
            OnPropertyChanged(() => PlanDelta);
        }

        #endregion


        public bool IsReadOnly => true;
        
        // Внешний вид
        #region APPEARANCE

        public virtual FontStyle FontStyle { get; set; } = FontStyles.Normal;

        public virtual FontWeight FontWeight { get; set; } = FontWeights.Normal;

        public ImageSource ImgSrc { get; set; }

        #endregion
    }


    public class OwnerItem : ItemBase
    {
        private double? _current;
        private Guid _entityId;
        private GasOwnerDTO _owner;
        private BalanceItem _balItem;
        public Action<Guid, int, BalanceItem, double?> SetValueAction { get; set; }


        public OwnerItem(Guid entityId, GasOwnerDTO owner, BalanceItem balItem, Action<Guid, int, BalanceItem, double?> setValueAction)
        {
            _entityId = entityId;
            _owner = owner;
            _balItem = balItem;
            SetValueAction = setValueAction;
        }

        public override string Name => _owner.Name;


        public override double? Current
        {
            get { return _current; }
            set
            {
                if (SetProperty(ref _current, value))
                {
                    OnPropertyChanged(() => MeasuredDelta);
                    OnPropertyChanged(() => PlanDelta);

                    SetValueAction?.Invoke(_entityId, _owner.Id, _balItem, value);
                }
            }
        }

        public bool GetVisibility(int systemId)
        {
            return !(_owner.DisableList.Any(d => d.EntityId == _entityId && d.BalanceItem == _balItem) || _owner.SystemList.All(s => s != systemId));
        }


        public override FontStyle FontStyle => FontStyles.Italic;
    }


    public class SummaryItem : ItemBase
    {
        public override double? Current
        {
            get { return Childs.Sum(c => c.Current); }
        }

        
        public override double? Plan
        {
            get { return Childs.Sum(c => c.Plan); }
        }

        private double? _measured;
        public override double? Measured
        {
            get { return _measured ?? Childs.Sum(c => c.Measured); }
            set { SetProperty(ref _measured, value); }
        }
    }
}