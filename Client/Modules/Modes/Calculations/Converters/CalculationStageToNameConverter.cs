using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using GazRouter.DTO.Calculations.Calculation;


namespace GazRouter.Modes.Calculations.Converters
{
    public class CalculationStageToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typeId = value as CalculationStage?;
            if (typeId.HasValue)
                return StageDict[typeId.Value];

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static readonly Dictionary<CalculationStage, string> StageDict = new Dictionary<CalculationStage, string>
        {
            { CalculationStage.BeforeStandard, "До типовых расчетов"},
            { CalculationStage.AfterStandard, "После типовых расчетов"},
            { CalculationStage.AfterAggregators, "После расчета агрегаторов"}
        };

    }
}