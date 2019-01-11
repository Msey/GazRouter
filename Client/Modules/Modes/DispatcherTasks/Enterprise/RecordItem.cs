using System;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.Modes.DispatcherTasks.Common;
using GazRouter.Modes.DispatcherTasks.Common.RecordUrgency;

namespace GazRouter.Modes.DispatcherTasks.Enterprise
{
    public class RecordItem
    {
        private readonly bool _checkUrgency;
        private const int AlarmPeriod = 30 * 60; // Seconds

        public TaskRecordDTO Dto { get; set; }
        public RecordItem(TaskRecordDTO dto, bool checkUrgency)
        {
            Dto = dto;
            _checkUrgency = checkUrgency && Dto.CompletionDate.HasValue;
        }

        public RecordUrgency Urgency
        {
            get
            {
                if (!_checkUrgency || IsCompleted)
                    return RecordUrgency.Default;

                if (Dto.CompletionDate.Value <= DateTime.Now)
                    return RecordUrgency.Alarm;

                if ((Dto.CompletionDate.Value - DateTime.Now).TotalSeconds <= AlarmPeriod)
                    return RecordUrgency.Urgent;

                return RecordUrgency.Default;
            }
        }
       

        public bool IsCompleted => Dto.ExecutedDate.HasValue;
    }
}