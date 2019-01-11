using System;
using System.Globalization;
using System.Windows.Data;

namespace GazRouter.Modes.DispatcherTasks.Common.TaskListType
{
    public class TaskListTypeToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var type = value as TaskListType?;
            
            switch (type)
            {
                case TaskListType.Current:
                    return "Текущие";

                case TaskListType.Archive:
                    return "Архив";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}