using System;
using System.Globalization;
using System.Windows.Data;

namespace GazRouter.Repair.RepWorks.Dialogs
{
    public class AddEditRepairViewWindowWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int val = -1;
            if (value is int) val = (int)value;
            switch (val)
            {
                case 4: return 725.0;
                case 5: return 905.0;
                default: return 720.0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
