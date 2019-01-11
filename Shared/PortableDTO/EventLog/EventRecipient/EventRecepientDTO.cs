using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.EventLog.EventRecipient
{
        [DataContract]
    public class EventRecepientDTO : BaseDto<Guid>
        {
			[DataMember]
			public int EventId { get; set; }
			[DataMember]
			public DateTime AssignementDate { get; set; }
			[DataMember]
			public int PriorityId { get; set; }
			[DataMember]
			public Guid SiteId { get; set; }
			[DataMember]
			public DateTime? AckDate { get; set; }
            [DataMember]
            public string Recepient { get; set; }
            [DataMember]
            public string AckRecepient { get; set; }
            [DataMember]
            public string EntityName { get; set; }

            public int? AckDelay
            {
                get
                {
                    if (!AckDate.HasValue)
                        return null;
                    return (int?)Math.Floor((AckDate.Value - AssignementDate).TotalMinutes);
                }
            }

            /// <summary>
            /// Событие квитированно
            /// </summary>
            public bool IsAckOverdue
            {
                get { return !AckDate.HasValue && (DateTime.Now - AssignementDate).TotalMinutes > 10; }
            }
    }
}