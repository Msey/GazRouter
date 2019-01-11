using System;
using System.Globalization;
using System.Windows.Data;

namespace GazRouter.Flobus.Utilites
{
    public class GeometryConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
             var geometryString = value as string;

             if (geometryString != null)
             {
                 return GeometryParser.GetGeometry(geometryString);
             }
            /* var geometry = value as Geometry;

             if (geometry != null)
             {
                 if (geometry is PathGeometry)
                 {
                     return GeometryParser.GetGeometry(geometry.ToString());
                 }
             }*/
            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}