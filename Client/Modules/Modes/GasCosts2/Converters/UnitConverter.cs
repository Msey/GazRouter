using System;
using System.Globalization;
using System.Windows.Data;
namespace GazRouter.Modes.GasCosts2.Converters
{
    public class UnitConverter : IValueConverter
    {
        public UnitConverter()
        {
            _units = 1000;
        }

        private int _units;

        public object Convert(object value, Type targetType, 
                              object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            return ((double)value * _units).ToString("#,0.000", culture);
        }
        /// <summary>
        /// 
        /// // var provider = CultureInfo.CurrentCulture; // CreateSpecificCulture("en-US")
        /// 
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, 
                                  object parameter, CultureInfo culture)
        {
            double number;
            var isParsed = double.TryParse(value.ToString(), NumberStyles.Float, culture, out number);
            var result = number / _units;
            return result;
        }
        public void SetUnits(int i)
        {
            _units = i;
        }
    }
}
#region trash
// todo:
//            var v = value.ToString();
//            return double.Parse(v, NumberStyles.AllowTrailingSign | 
//                                   NumberStyles.AllowParentheses  | 
//                                   NumberStyles.AllowThousands    | 
//                                   NumberStyles.Float);
// StringFormat='#,0.000'
#endregion


