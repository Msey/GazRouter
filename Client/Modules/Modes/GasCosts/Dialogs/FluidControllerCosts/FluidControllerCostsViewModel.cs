using System;
using System.Collections.Generic;
using GazRouter.Common.Ui.Templates;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using JetBrains.Annotations;
namespace GazRouter.Modes.GasCosts.Dialogs.FluidControllerCosts
{
    public class FluidControllerCostsViewModel : CalcViewModelBase<FluidControllerCostsModel>
    {
#region constructor
        public FluidControllerCostsViewModel([NotNull] GasCostDTO gasCost,
                                Action<GasCostDTO> closeCallback,
                                List<DefaultParamValues> defaultParamValues, bool ShowDayly,
                                bool useMeasuringLoader = false) 
            : base(gasCost, closeCallback, defaultParamValues, useMeasuringLoader)
        {

            this.ShowDayly = ShowDayly;
            SetValidationRules();
            PerformCalculate();
            FormulaFormat1 = new FormulaFormatDescription("q", "пр");
            ResultFormat = new FormulaFormatDescription("Q", "грс", "ПР");          
            if(!IsEdit) { Qpr = 0.6;}
        }
#endregion

#region formula_descriptions
        public FormulaFormatDescription FormulaFormat1 { get; set; }
        public FormulaFormatDescription ResultFormat { get; set; }
#endregion

#region property
        public double Qpr
        {
            get { return Model.Qpr; }
            set
            {
                Model.Qpr = value;
                OnPropertyChanged(() => Qpr);
                PerformCalculate();
            }
        }
        public double N
        {
            get { return Model.N; }
            set
            {
                Model.N = value;
                OnPropertyChanged(() => N);
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
            AddValidationFor(() => Qpr)
                        .When(() => Qpr <= 0)
                        .Show("Недопустимое значение. Должно быть больше 0.");
            AddValidationFor(() => N)
                        .When(() => N <= 0)
                        .Show("Недопустимое значение. Должно быть больше 0.");
            AddValidationFor(() => Time)
                        .When(() => Time <= 0 || Time > RangeInHours)
                        .Show($"Недопустимое значение (интервал допустимых значений 1 - {RangeInHours})");
            ShowListingCommand?.RaiseCanExecuteChanged();
        }
    }
}
