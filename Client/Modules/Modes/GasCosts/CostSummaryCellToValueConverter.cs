using System;
using System.Globalization;
using System.Windows.Data;
using JetBrains.Annotations;

namespace GazRouter.Modes.GasCosts
{
    internal class CostSummaryCellToValueConverter : IValueConverter
    {
        private readonly GasCostsMainViewModel _gasCostsMainViewModel;

        public CostSummaryCellToValueConverter([NotNull] GasCostsMainViewModel gasCostsMainViewModel)
        {
            if (gasCostsMainViewModel == null) throw new ArgumentNullException("gasCostsMainViewModel");
            _gasCostsMainViewModel = gasCostsMainViewModel;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var cell = value as ICostSummarySell;
            if (cell == null)
                return null;

            if (!cell.MayContainValue)
                return null;
            return cell[_gasCostsMainViewModel.SelectedTarget.Target];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}