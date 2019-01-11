using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.DTO.Repairs.Agreed;

namespace GazRouter.Repair.Agreement.Converters
{
    public class RealAgreementPersonPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = String.Empty;
            AgreedRepairRecordDTO Data = value as AgreedRepairRecordDTO;
            if (Data != null && Data.AgreedResult.HasValue)
            {
                if (Data.RealAgreedUserId.HasValue)
                    result = Data.RealAgreedUserPosition;
                else
                    result = Data.AgreedUserPosition;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
