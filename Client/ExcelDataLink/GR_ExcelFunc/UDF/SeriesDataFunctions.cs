using System;
using DataProviders;
using DataProviders.SeriesData;
using ExcelDna.Integration;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using GR_ExcelFunc.Model;
using Utils.Extensions;

namespace GR_ExcelFunc.UDF
{
    public class SeriesDataSync : BaseSync
    {
        

        [ExcelFunction(Description = "Значение параметра")]
        public static object GetValue(string entityId,string propertyTypeId, DateTime timestamp, string periodTypeId)
        {
            try
            {
                Guid id;
                if (Guid.TryParse(entityId, out id))
                {
                    PeriodType period;
                    if (!PeriodType.TryParse(periodTypeId, out period))
                        return "Неверный тип периода";
                    PropertyType propertyType;
                    if (!PropertyType.TryParse(propertyTypeId, out propertyType))
                        return "Неверный тип параметра";

                    var data = ExecuteSync(new SeriesDataServiceProxy().GetPropertyValueAsync, new GetPropertyValueParameterSet
                    {
                        EntityId = id.Convert(),
                        PropertyTypeId = propertyType,
                        PeriodTypeId = period,
                        Timestamp = timestamp
                    });

                    
                    return Common.GetValue(data);
                }
                return "Неверный идентификатор объекта";
            }
            catch (Exception err)
            {
                return err.ToString();
            }
        }
    }
}
