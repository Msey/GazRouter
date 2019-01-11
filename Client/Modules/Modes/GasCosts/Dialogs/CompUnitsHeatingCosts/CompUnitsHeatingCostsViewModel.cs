using System;
using System.Collections.Generic;
using GazRouter.Common.Ui.Templates;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using JetBrains.Annotations;
namespace GazRouter.Modes.GasCosts.Dialogs.CompUnitsHeatingCosts
{
    public class CompUnitsHeatingCostsViewModel : CalcViewModelBase<CompUnitsHeatingCostsModel>
    {
#region constructor
        public CompUnitsHeatingCostsViewModel([NotNull] GasCostDTO gasCost, 
                                Action<GasCostDTO> closeCallback, 
                                List<DefaultParamValues> defaultParamValues,
                                bool ShowDayly,
                                bool useMeasuringLoader = false) 
            : base(gasCost, closeCallback, defaultParamValues, useMeasuringLoader)
        {
            this.ShowDayly = ShowDayly;
            SetValidationRules();
            PerformCalculate();

            FormulaFormat1 = new FormulaFormatDescription("Q", "кц", "укр");
            FormulaFormat2 = new FormulaFormatDescription("q", "ba");
            FormulaFormat3 = new FormulaFormatDescription("K", "t");           
        }
#endregion

#region formula_descriptions
        public FormulaFormatDescription FormulaFormat1 { get; set; }
        public FormulaFormatDescription FormulaFormat2 { get; set; }
        public FormulaFormatDescription FormulaFormat3 { get; set; }
#endregion

#region property
        public double Qba
        {
            get { return Model.Qba; }
            set
            {
                Model.Qba = value;
                OnPropertyChanged(() => Qba);
                PerformCalculate();
            }
        }
        public double Kt
        {
            get { return Model.Kt; }
            set
            {
                Model.Kt = value;
                OnPropertyChanged(() => Kt);
                PerformCalculate();
            }
        }
        public double Time
        {
            get { return Model.Time; }
            set
            {
                Model.Time = value;
                OnPropertyChanged(() => Time);
                PerformCalculate();
            }
        }
        #endregion
        protected override void SetValidationRules()
        {
                AddValidationFor(() => Qba)
                            .When(() => Qba <= 0)
                            .Show("Недопустимое значение. Должно быть больше 0.");
                AddValidationFor(() => Kt)
                            .When(() => Kt <= 0)
                            .Show("Недопустимое значение. Должно быть больше 0.");
                AddValidationFor(() => Time)
                            .When(() => Time <= 0 || Time > RangeInHours)
                            .Show($"Недопустимое значение (интервал допустимых значений 1 - {RangeInHours})");
            ShowListingCommand?.RaiseCanExecuteChanged();
        }
    }
}
