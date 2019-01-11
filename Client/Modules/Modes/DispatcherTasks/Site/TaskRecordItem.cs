using System;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using GazRouter.Modes.DispatcherTasks.Common;
using GazRouter.Modes.DispatcherTasks.Common.RecordUrgency;


namespace GazRouter.Modes.DispatcherTasks.Site
{
    public class TaskRecordItem : PropertyChangedBase
    {
        private const int AlarmPeriod = 30 * 60; // Seconds

        public TaskRecordItem(TaskRecordPdsDTO dto)
        {
            Dto = dto;
        }


        public TaskRecordPdsDTO Dto { get; }

        public bool IsAck => Dto.AckDate.HasValue;
        public bool IsCompleted => Dto.ExecutedDate.HasValue;

        public bool IsOverdue => !IsCompleted && Dto.CompletionDate < DateTime.Now;

        public RecordUrgency Urgency
        {
            get
            {
                if (!Dto.CompletionDate.HasValue || IsCompleted)
                    return RecordUrgency.Default;

                if (Dto.CompletionDate.Value <= DateTime.Now)
                    return RecordUrgency.Alarm;

                if ((Dto.CompletionDate.Value - DateTime.Now).TotalSeconds <= AlarmPeriod)
                    return RecordUrgency.Urgent;

                return RecordUrgency.Default;
            }
        }
    }
}