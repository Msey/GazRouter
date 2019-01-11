using System.Windows;
using Telerik.Windows.Controls;
using GazRouter.DTO.DispatcherTasks.TaskRecords;
using System;
using System.Collections.Generic;

namespace GazRouter.Modes.DispatcherTasks.PDS
{
    public class TaskRecordPDSStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var obj = (TaskRecordDTO)item;

            Style style;
            if (StyleDict.TryGetValue(TaskRowFormatter.GetUrgency(obj), out style))
                return style;

            return base.SelectStyle(item, container);
        }

        private Dictionary<TaskUrgency, Style> StyleDict => new Dictionary<TaskUrgency, Style>() {
            { TaskUrgency.Alarm, AlarmStyle },
            { TaskUrgency.Urgent, UrgentStyle } };

        public Style UrgentStyle { get; set; }

        public Style AlarmStyle { get; set; }
    }
}
