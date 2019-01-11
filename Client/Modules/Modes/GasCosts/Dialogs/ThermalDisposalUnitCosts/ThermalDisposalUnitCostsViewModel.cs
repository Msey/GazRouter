using System;
using System.Collections.Generic;
using GazRouter.Common.Ui.Templates;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using JetBrains.Annotations;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
namespace GazRouter.Modes.GasCosts.Dialogs.ThermalDisposalUnitCosts
{
    public class ThermalDisposalUnitCostsViewModel : CalcViewModelBase<ThermalDisposalUnitCostsModel>
    {        
#region constructor
        public ThermalDisposalUnitCostsViewModel([NotNull] GasCostDTO gasCost, 
            Action<GasCostDTO> closeCallback,
            List<DefaultParamValues> defaultParamValues, bool ShowDayly,
            bool useMeasuringLoader = false) 
            : base(gasCost, closeCallback, defaultParamValues, useMeasuringLoader)
        {
            this.ShowDayly = ShowDayly;
            PerformCalculate();
            FormulaFormat1 = new FormulaFormatDescription("Q", "КПТГ", "КТО");
            FormulaFormat2 = new FormulaFormatDescription("Н", "mo");
            FormulaFormat3 = new FormulaFormatDescription("m", "оmх");
        }
#endregion
#region property
        public FormulaFormatDescription FormulaFormat1 { get; set; }
        public FormulaFormatDescription FormulaFormat2 { get; set; }
        public FormulaFormatDescription FormulaFormat3 { get; set; }

        public double Hm0
        {
            get { return Model.Hm0; }
            set
            {
                Model.Hm0 = value;
                OnPropertyChanged(() => Hm0);
                PerformCalculate();
            }
        }
        public double Motx
        {
            get { return Model.Motx; }
            set
            {
                Model.Motx = value;
                OnPropertyChanged(() => Motx);
                PerformCalculate();
            }
        }
#endregion
#region methods
        protected override void SetValidationRules()
        {
            AddValidationFor(() => Hm0)
                .When(() => Hm0 <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");
            // 
            AddValidationFor(() => Motx)
                .When(() => Motx <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");
            
        }
#endregion
    }
}
