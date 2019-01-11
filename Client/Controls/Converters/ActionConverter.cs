using System;
using System.Globalization;
using System.Windows.Data;

namespace GazRouter.Controls.Converters
{
    public class ActionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var a = (int)value;
            switch (a)
            {
                case 1: return "новая запись";
                case 2: return "изменение";
                case 3: return "удаление";
                case 4: return "выполнение";
                default: return "неизвестно";
            }            
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
