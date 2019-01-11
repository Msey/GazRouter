using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GazRouter.Common.GoodStyles;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.ValueMessages;

namespace GazRouter.Controls.Measurings
{
    public abstract class MeasuringBase : ViewModelBase
    {
        private readonly PropertyType _propType;
        private List<PropertyValueMessageDTO> _messageList;

        protected MeasuringBase(Guid entityId, PropertyType propType, PeriodType periodType)
        {
            EntityId = entityId;
            _propType = propType;
            PeriodType = periodType;
        }

        public Guid EntityId { get; }

        public PropertyTypeDTO PropertyType
        {
            get { return ClientCache.DictionaryRepository.PropertyTypes.Single(pt => pt.PropertyType == _propType); }
        }

        public PeriodType PeriodType { get; }

        public SourceType? SourceType { get; set; }

        

        /// <summary>
        ///     Список сообщений
        /// </summary>
        public List<PropertyValueMessageDTO> MessageList
        {
            get { return _messageList; }
            set
            {
                if (SetProperty(ref _messageList, value))
                {
                    OnPropertyChanged(() => WarningColor);
                    OnPropertyChanged(() => WarningForeground);
                }
            }
        }

        public Brush WarningColor => SwitchOnType(
            normal: () => Brushes.Transparent,
            err: () => Brushes.Red,
            warn: () => Brushes.Orange);

        public Brush WarningForeground => SwitchOnType(
            normal: () => Brushes.Black,
            err: () => Brushes.White,
            warn: () => Brushes.Black);

        private T SwitchOnType<T>(Func<T> normal, Func<T> err, Func<T> warn)
        {
            if (MessageList == null || MessageList.Count == 0)
                return normal();
            else if (MessageList.Any(m => m.IsError))
                return err();
            else
                return warn();
        }

        /// <summary>
        ///     Создает измерение нужного типа
        /// </summary>
        public static MeasuringBase Create(Guid entityId, PropertyType propType, PeriodType periodType,
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> valueDict, DateTime date)
        {
            var propTypeDto = ClientCache.DictionaryRepository.PropertyTypes.Single(pt => pt.PropertyType == propType);

            switch (propTypeDto.PhysicalType.PhysicalType)
            {
                case PhysicalType.StateSet:
                    var stateMeas = new StateMeasuring(entityId, propType, periodType);
                    stateMeas.Extract(valueDict, date);
                    return stateMeas;

                case PhysicalType.Text:
                    var strMeas = new StringMeasuring(entityId, propType, periodType);
                    strMeas.Extract(valueDict, date);
                    return strMeas;

                case PhysicalType.Timestamp:
                    var dateMeas = new DateMeasuring(entityId, propType, periodType);
                    dateMeas.Extract(valueDict, date);
                    return dateMeas;

                default:
                    var dblMeas = new DoubleMeasuring(entityId, propType, periodType);
                    dblMeas.Extract(valueDict, date);
                    return dblMeas;
            }
        }
    }
}