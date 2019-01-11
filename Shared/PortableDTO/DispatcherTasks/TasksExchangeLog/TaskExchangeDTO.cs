using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.DispatcherTasks.TasksExchangeLog
{
    [DataContract]
    public class TaskExchangeDTO : BaseDto<Guid>
    {
        public TaskExchangeDTO()
        { }

        [DataMember]
        /// <summary>
        /// ID задания
        /// </summary>
        public Guid TaskId { get; set; }
        [DataMember]
        /// <summary>
        /// идентификатор ДЗ в М АСДУ
        /// </summary>
        public string GlobalTaskID { get; set; }
        [DataMember]
        /// <summary>
        /// дата и время информационного обмена
        /// </summary>
        public DateTime ExchangeDate { get; set; }
        [DataMember]
        /// <summary>
        /// статус сообщения в М АСДУ
        /// </summary>
        public string ExchangeStatus { get; set; }
        [DataMember]
        /// <summary>
        /// статус подтверждения о получении сообщения
        /// </summary>
        public bool? ExchangeConfirmed { get; set; }
    }
}
