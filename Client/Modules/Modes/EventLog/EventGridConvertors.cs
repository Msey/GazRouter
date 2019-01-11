using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;


namespace GazRouter.Modes.EventLog
{
    public class EventPrioritetImageConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageSource source = null;
            switch ((string)value)
            {
                case "На контроле":
                    source = (ImageSource)new ImageSourceConverter().ConvertFromString(@"/Common;component/Images/16x16/trace.png");
                    break;

                case "Корзина":
                    source = (ImageSource)new ImageSourceConverter().ConvertFromString(@"/Common;component/Images/16x16/trash.png");
                    break;
            }

            return source;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class CountVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
            {
                int count;
                if (int.TryParse(value.ToString(), out count))
                {
                    return count < 1
                               ? Visibility.Collapsed
                               : Visibility.Visible;
                }
            }
            else
            {
                switch ((string)value)
                {
                    case "Обычный":
                        return Visibility.Collapsed;

                    case "На контроле":
                        return Visibility.Visible;

                    case "Корзина":
                        return Visibility.Visible;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
