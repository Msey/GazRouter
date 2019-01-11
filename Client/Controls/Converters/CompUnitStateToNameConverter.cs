using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.StatesModel;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Controls.Converters
{
    public class CompUnitStateToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            return Convert((CompUnitState)value);
        }

        public string Convert(CompUnitState state)
        {
            return ClientCache.DictionaryRepository.CompUnitStates.Single(s => s.Id == (int) state).Name;
        }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}