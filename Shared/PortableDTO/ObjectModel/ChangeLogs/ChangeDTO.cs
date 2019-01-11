using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.EventPriorities;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.DTO.ObjectModel.ChangeLogs
{
    [DataContract]
    public class ChangeDTO : BaseDto<int>
    {
        [DataMember]
        public int LogId { get; set; }
        [DataMember]
        public DateTime? ActionDate { get; set; }

        [DataMember]
        public string Login { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Site { get; set; }

        [DataMember]
        public int? Action { get; set; }

        [DataMember]
        public string ActionName { get; set; }

        [DataMember]
        public string TableName { get; set; }
    }
}
