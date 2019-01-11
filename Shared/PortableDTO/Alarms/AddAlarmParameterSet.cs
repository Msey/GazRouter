using System;
using GazRouter.DTO.Dictionaries.AlarmTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.Alarms
{
    public class AddAlarmParameterSet
    {
        public Guid EntityId { get; set; }
        public PropertyType PropertyTypeId { get; set; }
        public PeriodType PeriodTypeId { get; set; }
        public AlarmType AlarmTypeId { get; set; }
        public double Setting { get; set; }
        public DateTime ActivationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Description { get; set; }
    }
}
