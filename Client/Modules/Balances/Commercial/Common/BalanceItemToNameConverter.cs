using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.BalanceItems;
using Microsoft.Practices.ServiceLocation;


namespace GazRouter.Balances.Commercial.Common
{
    public class BalanceItemToNameConverter : IValueConverter
    {
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as BalanceItem?;
            return val.HasValue ? GetItemName(val.Value) : "";
        }


        public static string GetItemName(BalanceItem balItem)
        {
            return ClientCache.DictionaryRepository.BalanceItems.SingleOrDefault(i => i.BalanceItem == balItem)?.Name ?? "";
        }
        
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}