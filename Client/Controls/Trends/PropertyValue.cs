using System;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GazRouter.Controls.Trends
{
    public class PropertyValue : PropertyChangedBase
    {
        private readonly PropertyValueDoubleDTO _value;

        public PropertyValue(PropertyValueDoubleDTO value, Trend trend)
		{
            _value = value;
            Trend = trend;
		}
        
        public double Value => _value.Value;

        public DateTime Date => _value.Date;

        public Trend Trend { get; }
    }
}