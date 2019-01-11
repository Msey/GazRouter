using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.Controls.Converters
{
    public class EntityTypeToImageSourceConverter: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            
            var type = value as EntityType?;
            return Convert(type);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public static BitmapImage Convert(EntityType? type)
        {
            const string folder = "/Common;component/Images/16x16/EntityTypes/";

            return
                new BitmapImage(
                    new Uri(
                        type.HasValue && _fileDict.ContainsKey(type.Value)
                            ? folder + _fileDict[type.Value]
                            : "/Common;component/Images/16x16/object2.png", UriKind.Relative));
        }

        private static readonly Dictionary<EntityType, string> _fileDict = new Dictionary<EntityType, string>
        {
            { EntityType.CoolingStation, "cooling_station.png" },
            { EntityType.DistrStation, "distr_station.png" },
            { EntityType.ReducingStation, "reducing_station.png" },
            { EntityType.MeasStation, "meas_line.png" },
            { EntityType.MeasLine, "meas_line.png" },
            { EntityType.MeasPoint, "meas_point.png" },
            { EntityType.PowerUnit, "power_unit3.png" },
            { EntityType.Valve, "valve.png" },
            { EntityType.Pipeline, "pipeline.png" },

        };
    }
}