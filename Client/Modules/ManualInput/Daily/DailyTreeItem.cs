using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GazRouter.Application;
using GazRouter.Common.GoodStyles;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GazRouter.ManualInput.Daily
{

    public class ItemBase : PropertyChangedBase
    {
        /// <summary>
        /// Расход газа, тыс.м3
        /// </summary>
        public virtual double Current { get; set; }
        
        /// <summary>
        /// Расход за предыдущие сутки, тыс.м3
        /// </summary>
        public virtual double Prev { get; set; }

        public double Delta => Current - Prev;

        public virtual Brush WarnColor
            =>
                UserProfile.Current.UserSettings.DeltaThresholds.CheckDelta(PhysicalType.Volume, Current, Prev) ==
                ValueDeltaType.Warn
                    ? Brushes.SoftOrange
                    : Brushes.Transparent;


        /// <summary>
        /// Накопительный с начала месяца (без учета текущих суток), тыс.м3
        /// </summary>
        public virtual double MonthTotal { get; set; }


        /// <summary>
        /// Накопительный с начала месяца, тыс.м3
        /// </summary>
        public double MonthTotalWithCurrent => MonthTotal + Current; 

        

        public CommonEntityDTO Entity { get; set; }

        public virtual FontWeight FontWeight { get; set; } = FontWeights.Normal;

        public virtual FontStyle FontStyle { get; set; } = FontStyles.Normal;


    }



    public class GroupItem : ItemBase
    {
        public GroupItem()
        {
            Childs = new List<ItemBase>();
        }

        public List<ItemBase> Childs { get; set; }

        public override double Current { get { return Childs.Sum(l => l.Current); } }
        
        public override double Prev { get { return Childs.Sum(l => l.Prev); } } 
        
        public override double MonthTotal { get { return Childs.Sum(l => l.MonthTotal); } }

        public override Brush WarnColor => Brushes.Transparent;

        public void AddChild(ItemBase item)
        {
            item.PropertyChanged += OnChildChanged;
            Childs.Add(item);
        }

        private void OnChildChanged(object obj, PropertyChangedEventArgs args)
        {
            OnPropertyChanged(() => Current);
            //OnPropertyChanged(() => Prev);
            //OnPropertyChanged(() => MonthTotal);
            OnPropertyChanged(() => MonthTotalWithCurrent);
            OnPropertyChanged(() => Delta);
        }
    }



    public class InputItem : ItemBase
    {
        public InputItem(CommonEntityDTO entity, int serieId, int coef, double current, double prev, double monthTotal)
        {
            Entity = entity;
            _serieId = serieId;
            _coef = coef;
            _current = current;
            Prev = prev;
            MonthTotal = monthTotal;

        }
        private readonly int _coef;
        private readonly int _serieId;
        private double _current;
        
        public override double Current
        {
            get { return _current; }
            set
            {
                if (SetProperty(ref _current, value))
                {
                    OnPropertyChanged(() => MonthTotalWithCurrent);
                    OnPropertyChanged(() => Delta);
                    OnPropertyChanged(() => WarnColor);
                    SaveValue();
                }
            }
        }
        

        private void SaveValue()
        {
            new SeriesDataServiceProxy().SetPropertyValueAsync(
                new List<SetPropertyValueParameterSet>
                {
                    new SetPropertyValueParameterSet
                    {
                        EntityId = Entity.Id,
                        PropertyTypeId = PropertyType.Flow,
                        Value = _current/_coef,
                        SeriesId = _serieId
                    }
                });
        }

        public override FontStyle FontStyle => FontStyles.Italic;
    }
}