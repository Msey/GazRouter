using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model
{
    public class DashboardData
    {
        public DashboardData()
        {
            Measurings = new Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>>();
        }
        
        /// <summary>
        /// Выбранная дата
        /// </summary>
        public DateTime KeyDate { get; set; }


        /// <summary>
        /// Список значений измерений
        /// </summary>
        public Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> Measurings { get; set; }
        
    }
    
}