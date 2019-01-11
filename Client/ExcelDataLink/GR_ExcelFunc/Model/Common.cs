using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GR_ExcelFunc.Model
{
    public static class Common
    {
        public static object GetValue(BasePropertyValueDTO data)
        {
            var propertyValueDoubleDTO = data as PropertyValueDoubleDTO;
            if (propertyValueDoubleDTO != null)
                return propertyValueDoubleDTO.Value;
            var propertyValueDateDTO = data as PropertyValueDateDTO;
            if (propertyValueDateDTO != null)
            {
                return propertyValueDateDTO.Value;
            }
            var propertyValueStringDTO = data as PropertyValueStringDTO;
            return propertyValueStringDTO != null ? propertyValueStringDTO.Value : "-";
        }
        
    }
}
