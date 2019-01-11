﻿using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.StatesModel;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Controls.Converters
{
    public class ValveStateToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";

            return Convert((ValveState) value);

        }

        public string Convert(ValveState state)
        {
            return ClientCache.DictionaryRepository.ValveStates.Single(s => s.Id == (int)state).Name;
        }

        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
