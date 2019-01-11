using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.AlarmTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.Alarms
{
    [DataContract]
    public class AlarmDTO : BaseDto<int>
    {
        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public string EntityName { get; set; }

        [DataMember]
        public PropertyType PropertyTypeId { get; set; }

        [DataMember]
        public PeriodType PeriodTypeId { get; set; }

        [DataMember]
        public AlarmType AlarmTypeId { get; set; }

        [DataMember]
        public double Setting { get; set; }

        [DataMember]
        public DateTime ActivationDate { get; set; }

        [DataMember]
        public DateTime ExpirationDate { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public string UserLogin { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public Guid UserSiteId { get; set; }

        [DataMember]
        public string UserSiteName { get; set; }

        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public int Status { get; set; }


    }
}
