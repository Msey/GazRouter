using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using GazRouter.Application;
using GazRouter.Common.GoodStyles;
using GazRouter.Controls.Trends;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Controls.Measurings
{
    public class DoubleMeasuring : Measuring<double?>
    {
        private readonly bool _showEmpty;
        private readonly bool _showDelta;
        private double? _prevValue;

        
        /// Используется в том случае, если нужно отображать и вводить расход 
        /// в каких-то других единицах измерения отличных от тыс.м3
        protected int Coef;

        public DoubleMeasuring(Guid entityId, PropertyType propType, PeriodType periodType, bool showDelta = false,
            bool showEmpty = false, int coef = 1)
            : base(entityId, propType, periodType)
        {
            _showEmpty = showEmpty;
            _showDelta = showDelta;
            Coef = coef;

            TrendCommand = new DelegateCommand(() => TrendsHelper.ShowTrends(entityId, propType, periodType));
        }

        private double? Delta => Value - _prevValue;

        public double? PrevValue => _prevValue;


        public override string DisplayValue
        {
            get
            {
                if (!Value.HasValue)
                    return _showEmpty ? "X" : string.Empty;

                return $"{ConvertAndRound(Value):#,0.###}";
            }
        }

        /// <summary>
        ///     Отклонение от предыдущего значения
        /// </summary>
        public string DisplayDelta
        {
            get
            {
                if (!Delta.HasValue)
                    return string.Empty;
                
                return ConvertAndRound(Delta)?.ToString("+#,0.###;-#,0.###");
            }
        }

        public string UnitsName
            =>
                PropertyType.PropertyType == DTO.Dictionaries.PropertyTypes.PropertyType.Flow && Coef == 1000
                    ? "м3"
                    : UserProfile.UserUnitName(PropertyType.PropertyType);

        public ValueDeltaType DeltaType
            =>
                UserProfile.Current.UserSettings.DeltaThresholds.CheckDelta(PropertyType.PhysicalType.PhysicalType,
                    Value ?? 0, _prevValue ?? 0);

        /// <summary>
        ///     отображать отклонение от предыдущего значения или нет
        /// </summary>
        public bool ShowDelta => _showDelta && DeltaType != ValueDeltaType.None;
        
        public double? UserValue => ConvertAndRound(Value);

        public DelegateCommand TrendCommand { get; set; }

        public Brush DeltaColor => DeltaType == ValueDeltaType.Warn ? Brushes.Red : Brushes.Black;
        

        // Приводит к пользовательским единицам и округляет до точности по умолчанию
        protected double? ConvertAndRound(double? val)
        {
            return val.HasValue
                ? Math.Round(UserProfile.ToUserUnits(val.Value, PropertyType.PropertyType),
                    PropertyType.PhysicalType.DefaultPrecision) : val;
        }



        public void Extract(
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> valueDict,
            DateTime date)
        {
            // получение значения

            var dictionary = valueDict.GetValueOrNull(EntityId);
            Extract(dictionary, date);
        }

        public void Extract(Dictionary<PropertyType, List<BasePropertyValueDTO>> dictionary, DateTime date)
        {
            var prop = dictionary?.GetValueOrNull(PropertyType.PropertyType);
            Clear();
            var value = prop?.FirstOrDefault(v => v.Date == date);
            if (value != null)
            {
                var dblVal = value as PropertyValueDoubleDTO;
                if (dblVal != null)
                    Value = dblVal.Value * Coef;
                
                MessageList = value.MessageList;
                SourceType = value.SourceType;
            }

            if (_showDelta)
            {
                // Если нужна дельта, то пытаемся вытащить предыдущее значение
                var prevDate = PeriodType == PeriodType.Twohours ? date.AddHours(-2) : date.AddDays(-1);
                var prevValue = prop?.FirstOrDefault(v => v.Date == prevDate);
                if (prevValue != null)
                {
                    var dblVal = prevValue as PropertyValueDoubleDTO;
                    if (dblVal != null)
                        _prevValue = dblVal.Value * Coef;
                    
                    OnPropertyChanged(() => ShowDelta);
                    OnPropertyChanged(() => DisplayDelta);
                    OnPropertyChanged(() => DeltaColor);
                }
            }
        }

        private void Clear()
        {
            Value = null;
            MessageList = null;
        }
    }
}