using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using GazRouter.Controls.Converters;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GazRouter.Controls.Measurings
{
    public class StateMeasuring : Measuring<double?>
    {
        public StateMeasuring(Guid entityId, PropertyType propType, PeriodType periodType)
            : base(entityId, propType, periodType)
        {
        }

        public override string DisplayValue
        {
            get
            {
                if (!Value.HasValue)
                {
                    return string.Empty;
                }
                switch (PropertyType.PropertyType)
                {
                    case DTO.Dictionaries.PropertyTypes.PropertyType.CompressorUnitState:
                        return new CompUnitStateToNameConverter().Convert((CompUnitState) Value);

                    case DTO.Dictionaries.PropertyTypes.PropertyType.StateValve:
                        return new ValveStateToNameConverter().Convert((ValveState) Value);

                    default:
                        return string.Empty;
                }
            }
        }

        public Brush StateColor
        {
            get
            {
                if (!Value.HasValue)
                {
                    return new SolidColorBrush(Colors.Transparent);
                }
                switch (PropertyType.PropertyType)
                {
                    case DTO.Dictionaries.PropertyTypes.PropertyType.CompressorUnitState:
                        return new CompUnitStateToColorConverter().Convert((CompUnitState) Value);

                    case DTO.Dictionaries.PropertyTypes.PropertyType.StateValve:
                        return new ValveStateToColorConverter().Convert((ValveState) Value);

                    default:
                        return new SolidColorBrush(Colors.Transparent);
                }
            }
        }

        public void Extract(
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> valueDict,
            DateTime date)
        {
            // получение значения
            if (valueDict.ContainsKey(EntityId) && valueDict[EntityId].ContainsKey(PropertyType.PropertyType))
            {
                var value = valueDict[EntityId][PropertyType.PropertyType].SingleOrDefault(v => v.Date == date);
                if (value != null)
                {
                    var dblVal = value as PropertyValueDoubleDTO;
                    if (dblVal != null)
                    {
                        Value = dblVal.Value;
                    }

                    MessageList = value.MessageList;

                    OnPropertyChanged(() => StateColor);
                }
            }
        }
    }
}