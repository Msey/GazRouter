using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GazRouter.Controls.Measurings
{
    public class StringMeasuring : Measuring<string>
    {
        public StringMeasuring(Guid entityId, PropertyType propType, PeriodType periodType)
            : base(entityId, propType, periodType)
        {
        }

        public override string DisplayValue => Value;

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
                    var dblVal = value as PropertyValueStringDTO;
                    if (dblVal != null)
                    {
                        Value = dblVal.Value;
                    }

                    MessageList = value.MessageList;
                }
            }
        }
    }
}