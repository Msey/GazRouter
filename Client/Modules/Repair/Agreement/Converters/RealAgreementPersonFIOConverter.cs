using System;
using System.Globalization;
using System.Windows.Data;
using GazRouter.DTO.Repairs.Agreed;

namespace GazRouter.Repair.Agreement.Converters
{
    public class RealAgreementPersonFIOConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = String.Empty;
            AgreedRepairRecordDTO Data = value as AgreedRepairRecordDTO;
            if (Data != null && Data.AgreedResult.HasValue)
            {
                if (Data.RealAgreedUserId.HasValue)
                    result = Data.RealAgreedUserName;
                else
                    result = Data.AgreedUserName;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
