using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.SeriesData.ValueMessages
{
    [DataContract]
    public class PropertyValueMessageDTO : BaseDto<Guid>
    {
		[DataMember]
        public DateTime Timestamp { get; set; }

        [DataMember]
        public Guid EntityId { get; set; }


        [DataMember]
        public EntityType EntityType { get; set; }

        [DataMember]
        public string EntityName { get; set; }

        [DataMember]
        public string EntityPath { get; set; }

        [DataMember]
        public Guid SiteId { get; set; }
    

        [DataMember]
        public PropertyType PropertyType { get; set; }
		
        [DataMember]
        public string MessageText { get; set; }
        
        [DataMember]
        public PropertyValueMessageType MessageType { get; set; }
		
        
        [DataMember]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Имя пользователя инициировавшего проверку, сформировавшую это сообщение
        /// </summary>
        [DataMember]
        public string CreateUserName { get; set; }

        [DataMember]
        public string CreateUserSite { get; set; }


        [DataMember]
        public DateTime AckDate { get; set; }

        /// <summary>
        /// Имя пользователя, квитировавшего сообщение (только для сообщений типа тревога)
        /// </summary>
        [DataMember]
        public string AckUserName { get; set; }

        [DataMember]
        public string AckUserSite { get; set; }


        public bool IsAcked 
        {
            get { return !string.IsNullOrEmpty(AckUserName); }
        }

        public bool IsError
        {
            get { return MessageType == PropertyValueMessageType.Error; }
        }
    }
}
