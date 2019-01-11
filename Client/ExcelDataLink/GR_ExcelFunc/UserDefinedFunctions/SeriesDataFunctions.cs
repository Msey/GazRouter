using System;
using System.Globalization;
using System.Net.Http.Headers;
using DataProviders.SeriesData;
using ExcelDna.Integration;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using GR_ExcelFunc.Service_References.SeriesDataServ;
using Utils.Extensions;

namespace GR_ExcelFunc
{
    public class SeriesDataFunctions
    {
        static  string GetValue(BasePropertyValueDTO data)
        {
            var retData = "";
            var propertyValueDoubleDTO = data as PropertyValueDoubleDTO;
            if (propertyValueDoubleDTO != null)
                retData = propertyValueDoubleDTO.Value.ToString(CultureInfo.CurrentUICulture);
            else
            {
                var propertyValueDateDTO = data as PropertyValueDateDTO;
                if (propertyValueDateDTO != null)
                {
                    retData = propertyValueDateDTO.Value.ToString(CultureInfo.CurrentCulture);
                }
                else
                {
                    var propertyValueStringDTO = data as PropertyValueStringDTO;
                    if (propertyValueStringDTO != null)
                    {
                        retData = propertyValueStringDTO.Value;
                    }
                }
            }
            return retData;
        }

        [ExcelFunction(Description = "Значение параметра")]
        public static string GetValue(string entityId,string propertyTypeId, DateTime timestamp, string periodTypeId)
        {
            try
            {
                Guid id;
                if (Guid.TryParse(entityId, out id))
                {
                    var serv = new SeriesDataServiceClient();
                    PeriodType period;
                    if (!PeriodType.TryParse(periodTypeId, out period))
                        return "Неверный тип периода";
                    PropertyType propertyType;
                    if (!PropertyType.TryParse(propertyTypeId, out propertyType))
                        return "Неверный тип параметра";
                    var data  = FuncHelper.ExecuteSync(
                        new SeriesDataServiceProxy().GetPropertyValueAsync, new GetPropertyValueParameterSet
                    {
                        EntityId = id.Convert(),
                        PropertyTypeId = propertyType,
                        PeriodTypeId = period,
                        Timestamp = timestamp
                    });
                    return GetValue(data);
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
