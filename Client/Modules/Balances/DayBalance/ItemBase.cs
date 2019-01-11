using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Balances;
using GazRouter.DTO.Balances.SortOrder;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.OperConsumers;
using Utils.Extensions;
using SetBalSortOrderParameterSet = GazRouter.DTO.Balances.SortOrder.SetBalSortOrderParameterSet;


namespace GazRouter.Balances.DayBalance
{
    public class ItemBase : PropertyChangedBase
    {
        private string _alias;

        public ItemBase(CommonEntityDTO entity, DateTime day)
        {
            Entity = entity;
            _sortOrder = Entity?.SortOrder ?? 0;
            Day = day;
            Childs = new List<ItemBase>();
        }

        public ItemBase(string alias, DateTime day)
        {
            _alias = alias;
            Day = day;
            Childs = new List<ItemBase>();
        }

        
        public CommonEntityDTO Entity { get; set; }

        public bool NoEntity => Entity == null; 

        public DateTime Day { get; set; }
        
        public string Alias
        {
            get
            {
                if (!string.IsNullOrEmpty(_alias)) return _alias;

                return Entity != null
                    ? (!string.IsNullOrEmpty(Entity.BalanceName) ? Entity.BalanceName : Entity.Name)
                    : "";
            }
        }
        

        /// <summary>
        /// Расход газа, тыс.м3
        /// </summary>
        public virtual double? Current { get; set; }

        /// <summary>
        /// Расход за предыдущие сутки, тыс.м3
        /// </summary>
        public virtual double? Prev { get; set; }

        public double? Delta => Current - Prev;


        /// <summary>
        /// Накопительный с начала месяца (без учета текущих суток), тыс.м3
        /// </summary>
        public virtual double? MonthTotal { get; set; }


        /// <summary>
        /// Накопительный с начала месяца, тыс.м3
        /// </summary>
        public double? MonthTotalWithCurrent => MonthTotal + Current;

        /// <summary>
        /// План суточный
        /// </summary>
        public virtual double? DayPlan { get; set; }

        public double? DayPlanDelta => Current - DayPlan;


        /// <summary>
        /// План месяц
        /// </summary>
        public virtual double? MonthPlan { get; set; }

        /// <summary>
        /// Прогноз на конец месяца
        /// </summary>
        public double? Forecast => MonthTotalWithCurrent + (Day.DaysInMonth() - Day.Day)*Current;

        /// <summary>
        /// Отклонение прогнозного значения от плана
        /// </summary>
        public double? ForecastDelta => Forecast - MonthPlan;


        public void CopyValues(ItemBase item)
        {
            item.Current = Current;
            item.Prev = Prev;
            item.MonthTotal = MonthTotal;
            item.DayPlan = DayPlan;
            item.MonthPlan = MonthPlan;
        }



        public bool IsBold { get; set; }


        #region SORT ORDER

        public virtual BalanceItem BalItem { get; set; }
        private int _sortOrder;
        
        public int SortOrder
        {
            get { return _sortOrder; }
            set
            {
                if (Entity != null )
                {
                    if (SetProperty(ref _sortOrder, value))
                        UpdateSortOrder(value);
                }
            }
        }

        private async void UpdateSortOrder(int sortOrder)
        {
            await new BalancesServiceProxy().SetSortOrderAsync(
                new SetBalSortOrderParameterSet
                {
                    EntityId = Entity.Id,
                    BalItem = BalItem,
                    SortOrder = sortOrder
                });
        }
        
        #endregion


        public List<ItemBase> Childs { get; set; }

        public void AddChild(ItemBase item)
        {
            Childs.Add(item);
        }


        public bool IsExpanded { get; set; }
        

        public void Expand()
        {
            IsExpanded = true;
            OnPropertyChanged(() => IsExpanded);
            foreach (var child in Childs)
            {
                child.Expand();
            }
        }

        public void Collapse()
        {
            IsExpanded = false;
            OnPropertyChanged(() => IsExpanded);
            foreach (var child in Childs)
            {
                child.Collapse();
            }
        }
    }


    public class SummaryItem : ItemBase
    {
        public SummaryItem(CommonEntityDTO entity, DateTime day) : base(entity, day)
        {

        }

        public SummaryItem(string alias, DateTime day) : base(alias, day)
        {

        }


        public override double? Current => Childs.Sum(c => c.Current);

        public override double? Prev => Childs.Sum(c => c.Prev);

        public override double? MonthTotal => Childs.Sum(c => c.MonthTotal);

        public override double? DayPlan => Childs.Sum(c => c.DayPlan);

        public override double? MonthPlan => Childs.Sum(c => c.MonthPlan);
    }


    public class MeasStationItem : ItemBase
    {
        public MeasStationItem(MeasStationDTO dto, DateTime date) : base(dto, date)
        {

        }

        public new MeasStationDTO Entity => (MeasStationDTO)base.Entity;
    }


    public class DistrStationItem : ItemBase
    {
        public DistrStationItem(DistrStationDTO dto, DateTime date) : base(dto, date)
        {

        }

        public new DistrStationDTO Entity => (DistrStationDTO)base.Entity;

        public void Recalc(List<OperConsumerItem> consList)
        {
            if (consList.Count > 0)
            {
                Prev -= consList.Sum(c => c.Prev);
                Current -= consList.Sum(c => c.Current);
                MonthTotal -= consList.Sum(c => c.MonthTotal);
            }
        }

        public override BalanceItem BalItem => BalanceItem.Consumers;
    }


    public class OperConsumerItem : ItemBase
    {
        public OperConsumerItem(OperConsumerDTO dto, DateTime date) : base(dto, date)
        {

        }

        public new OperConsumerDTO Entity => (OperConsumerDTO)base.Entity;

        public override BalanceItem BalItem => BalanceItem.OperConsumers;
    }


    public class GasCostItem : ItemBase
    {
        public GasCostItem(CommonEntityDTO dto, DateTime date) : base(dto, date)
        {

        }

        public override BalanceItem BalItem => BalanceItem.OperConsumers;
    }
}