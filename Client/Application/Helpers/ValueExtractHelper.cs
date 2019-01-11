using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;


namespace GazRouter.Application.Helpers
{
    public static class ValueExtractHelper
    {
        public static T Extract<T>(
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> values,
            Guid entityId,
            PropertyType propType,
            DateTime date) where T : BasePropertyValueDTO
        {
            return values.GetOrDefault(entityId)?.GetOrDefault(propType)?.SingleOrDefault(v => v.Date == date) as T;
        }

        
        public static T Extract<T>(
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> values, 
            Guid entityId,
            PropertyType propType, 
            int seriesId) where T : BasePropertyValueDTO
        {
            return values.GetOrDefault(entityId)?.GetOrDefault(propType)?.SingleOrDefault(v => v.SeriesId == seriesId) as T;
        }
    }
}