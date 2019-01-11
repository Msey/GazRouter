using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Utilites
{
    public class ItemVisibilityToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ItemVisibility)
            {
                var itemVisibility = (ItemVisibility) value;
                switch (itemVisibility)
                {
                    case ItemVisibility.Visible:
                        return Visibility.Visible;
                    case ItemVisibility.Collapsed:
                    case ItemVisibility.Virtualized:
                        return Visibility.Collapsed;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility)) return value;

            var itemVisibiliy = ((Visibility) value);
            switch (itemVisibiliy)
            {
                case Visibility.Visible:
                    return ItemVisibility.Visible;
                case Visibility.Collapsed:
                    return ItemVisibility.Collapsed;
            }
            return value;
        }

        public static IValueConverter Instance { get; } = new ItemVisibilityToVisibilityConverter();
    }
}