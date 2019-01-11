
using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;

namespace Utils.ValueExtrators
{
    
    public class EntityPropertyValueExtractor
    {
        private Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> _values;
        public EntityPropertyValueExtractor(Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> values)
        {
            _values = values;
        }

         
        public double? GetSum(Guid entityId, PropertyType propType)
        {
            return
                _values.GetOrDefault(entityId)?
                    .GetOrDefault(propType)
                    .OfType<PropertyValueDoubleDTO>()
                    .Sum(v => v.Value);
        }

        public double? GetDayValue(Guid entityId, PropertyType propType, DateTime day)
        {
            var val =
                _values.GetOrDefault(entityId)?
                    .GetOrDefault(propType)?
                    .FirstOrDefault(p => p.Day == day.Day && p.Month == day.Month) as PropertyValueDoubleDTO;
            return val?.Value;
        }

        public double? GetMonthSum(Guid entityId, PropertyType propType, DateTime day)
        {
            return 
                _values.GetOrDefault(entityId)?
                    .GetOrDefault(propType)?
                    .Where(p => p.Day <= day.Day && p.Month == day.Month)
                    .OfType<PropertyValueDoubleDTO>()
                    .Sum(p => p.Value);
        }
    }
}