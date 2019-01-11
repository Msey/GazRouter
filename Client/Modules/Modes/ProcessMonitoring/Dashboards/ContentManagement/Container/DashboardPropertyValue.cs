using System;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container
{
    
    public class DashboardPropertyValue : BasePropertyValueDTO, IComparable
    {
        public new int CompareTo(object obj)
        {
            if (obj == null) return -1;

            var v = (DashboardPropertyValue)obj;
            if (v.Date > Date) return -1;
            
            return 1;

        }
        
    }
    
}