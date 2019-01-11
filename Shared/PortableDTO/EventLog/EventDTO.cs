using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EventPriorities;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.DTO.EventLog
{
    [DataContract]
    public class EventDTO : BaseDto<int>
    {
		[DataMember]
        public int TypeId { get; set; }
        
        public string TypeName { get; set; }

        [DataMember]
        public DateTime? EventDate { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int CreateUserId { get; set; }

        [DataMember]
        public string CreateUserName { get; set; }

        [DataMember]
        public string CheckUser { get; set; }

        [DataMember]
        public DateTime? CreateDate { get; set; }
        
        [DataMember]
        public Guid UserEntityId { get; set; }

        [DataMember]
        public string UserEntityName { get; set; }

        [DataMember]
        public string SerialNumber { get; set; }

        [DataMember]
        public int CommentsCount { get; set; }

        [DataMember]
        public int AttachmentsCount { get; set; }

        [DataMember]
        public EventPriority Priority { get; set; }

        [DataMember]
        public double? Kilometer { get; set; }
        
        public string PriorityName 
        {
            get
            {
                string priorityName = null;
                switch (Priority)
                {
                    case EventPriority.Normal:
                        priorityName = "Обычный";
                        break;

                    case EventPriority.Control:
                        priorityName = "На контроле";
                        break;

                    case EventPriority.Trash:
                        priorityName = "Корзина";
                        break;
                }
                return priorityName;
            }
        }

		[DataMember]
		public bool IsQuote { get; set; }
        
        [DataMember]
        public string SiteName { get; set; }

        [DataMember]
        public bool IsEmergency { get; set; }


        public string EmergencyDescription => IsEmergency ? "Нештатная ситуация" : "Обычное";
        

        [DataMember]
        public CommonEntityDTO Entity { get; set; }
    }
}
