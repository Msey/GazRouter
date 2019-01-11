using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using GazRouter.Common.GoodStyles;
using GazRouter.DTO.ASDU;
using Colors = GazRouter.Common.GoodStyles.Colors;

namespace DataExchange.ASDU
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Boolean)
            {
                var val = (Boolean) value;
                if (parameter != null && parameter.ToString() == "reverse")
                {
                    val = !val;
                }
                return val ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
            {
                if (parameter != null && parameter.ToString() == "reverse")
                {
                    return (Visibility)value != Visibility.Visible;
                }
                return (Visibility) value == Visibility.Visible;
            }
            return true;
        }
    }


    public class ErrorCountToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int)
            {
                return (int)value > 0 ? new SolidColorBrush(Colors.Red) : null;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MetadataNodeTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MetadataTreeNodeType)
            {
                switch ((MetadataTreeNodeType)value)
                {
                    case MetadataTreeNodeType.IusObj:
                    case MetadataTreeNodeType.AsduObj:
                        return @"/DataExchange;component/ASDU/img/entity_26px.png";
                    case MetadataTreeNodeType.IusAttr:
                    case MetadataTreeNodeType.AsduAttr:
                        return @"/DataExchange;component/ASDU/img/attribute_26px.png";
                    case MetadataTreeNodeType.IusAttrLink:
                    case MetadataTreeNodeType.AsduAttrLink:
                        return @"/DataExchange;component/ASDU/img/attr_link_26px.png";
                    case MetadataTreeNodeType.IusParam:
                    case MetadataTreeNodeType.AsduParam:
                        return @"/DataExchange;component/ASDU/img/param_26px.png";
                    default:
                        return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DataNodeTypeToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DataTreeNodeType)
            {
                switch ((DataTreeNodeType)value)
                {
                    case DataTreeNodeType.IusObj:
                    case DataTreeNodeType.AsduObj:
                        return @"/DataExchange;component/ASDU/img/entity_26px.png";
                    case DataTreeNodeType.IusAttr:
                    case DataTreeNodeType.AsduAttr:
                        return @"/DataExchange;component/ASDU/img/attribute_26px.png";
                    case DataTreeNodeType.IusAttrLink:
                    case DataTreeNodeType.AsduAttrLink:
                        return @"/DataExchange;component/ASDU/img/attr_link_26px.png";
                    case DataTreeNodeType.IusParam:
                    case DataTreeNodeType.AsduParam:
                        return @"/DataExchange;component/ASDU/img/param_26px.png";
                    default:
                        return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class LinkedToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value?.ToString()))
            {
                return @"/DataExchange;component/ASDU/img/link_26px.png";
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DataEqualityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DataValueEquality)
            {
                var val = value as DataValueEquality?;
                return val.Value == DataValueEquality.Equal ? Brushes.NiceGreen : Brushes.Orange;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
}