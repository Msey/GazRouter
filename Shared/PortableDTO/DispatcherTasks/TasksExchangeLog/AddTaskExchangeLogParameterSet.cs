using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.DispatcherTasks.TasksExchangeLog
{
    public class AddTaskExchangeLogParameterSet
    {
        public AddTaskExchangeLogParameterSet()
        { }

        /// <summary>
        /// идентификатор для добавляемой строки отладка
        /// </summary>

        public Guid TaskExchangeID { get; set; }
        public Guid TaskId { get; set; }
        /// <summary>
        /// идентификатор ДЗ в М АСДУ
        /// </summary>
        public string GlobalTaskID { get; set; }
        /// <summary>
        /// дата и время информационного обмена
        /// </summary>
        public DateTime ExchangeDate { get; set; }
        /// <summary>
        /// статус сообщения в М АСДУ
        /// </summary>
        public string ExchangeStatus { get; set; }
        /// <summary>
        /// статус подтверждения о получении сообщения
        /// </summary>
        public bool? ExchangeConfirmed { get; set; }
    }

    public class EditTaskExchangeLogParameterSet : AddTaskExchangeLogParameterSet
    { }

    public class RemoveTaskExchangeLogParameterSet
    {
        /// <summary>
        /// уникальный идентификатор строки sys_guid
        /// </summary>
        public Guid TaskExchangeId { get; set; }
    }

    public class GetTaskExchangeLogParameterSet
    {

    }
}
