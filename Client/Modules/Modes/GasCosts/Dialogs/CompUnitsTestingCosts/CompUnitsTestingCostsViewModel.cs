using System;
using System.Collections.Generic;
using GazRouter.Application.Helpers;
using GazRouter.Common.Ui.Templates;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using JetBrains.Annotations;
using Utils.Units;
using System.Linq;
using GazRouter.DTO.ObjectModel.CompUnits;

namespace GazRouter.Modes.GasCosts.Dialogs.CompUnitsTestingCosts
{
    public class CompUnitsTestingCostsViewModel : CalcViewModelBase<CompUnitsTestingCostsModel>
    {
        public CompUnitsTestingCostsViewModel([NotNull] GasCostDTO gasCost, Action<GasCostDTO> closeCallback, 
               List<DefaultParamValues> defaultParamValues, bool ShowDayly, bool useMeasuringLoader = false)
            : base(gasCost, closeCallback, defaultParamValues, useMeasuringLoader)
        {
            this.ShowDayly = ShowDayly;
            Qtg0Format = new FormulaFormatDescription("q", "тг0");
            NeFormat = new FormulaFormatDescription("N", "e");
            Ne0Format = new FormulaFormatDescription("N", "e0");
            TaFormat = new FormulaFormatDescription("T", "a");            
            KtgFormat = new FormulaFormatDescription("K", "тг");
            PaFormat = new FormulaFormatDescription("P", "a");
            CombHeatFormat = new FormulaFormatDescription("Q", "н", "р");            
            TpcFormat = new FormulaFormatDescription("τ", "рс");
            ResultFormat = new FormulaFormatDescription("Q", "");
            if (!IsEdit)
            {
                if (TargetId == Target.Fact && IsCurrentMonth)
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);

                var defaultValues = DefaultParamValues.Single(d => d.Target == TargetId);
                PressureAir = defaultValues.PressureAir;
                CombHeat = CombustionHeat.FromKCal(defaultValues.CombHeat);

            }
            if (gasCost.Entity.EntityType == DTO.Dictionaries.EntityTypes.EntityType.CompUnit)
            {
                var CompUnit = gasCost.Entity as CompUnitDTO;
                var tmp = ClientCache.DictionaryRepository.CompUnitTypes.FirstOrDefault(t => t.Id == CompUnit.CompUnitTypeId);
                if (Ne0 == 0) Ne0 = tmp.RatedPower;
                if (Ktg == 0) Ktg = (double)tmp.KTechStateFuel;
            }

            PerformCalculate();
        }
#region formula_descriptions
        public FormulaFormatDescription Qtg0Format { get; set; }
        public FormulaFormatDescription NeFormat { get; set; }
        public FormulaFormatDescription Ne0Format { get; set; }
        public FormulaFormatDescription TaFormat { get; set; }
        public FormulaFormatDescription KtgFormat { get; set; }
        public FormulaFormatDescription PaFormat { get; set; }
        public FormulaFormatDescription CombHeatFormat { get; set; }
        public FormulaFormatDescription TpcFormat { get; set; }
        public FormulaFormatDescription ResultFormat { get; set; }
        #endregion
#region property
        public double Qtg0
        {
            get { return Model.Qtg0; }
            set
            {                
                Model.Qtg0 = value;
                OnPropertyChanged(() => Qtg0);
                PerformCalculate();
            }
        }
        public double Ne
        {
            get { return Model.Ne; }
            set
            {
                Model.Ne = value;
                OnPropertyChanged(() => Ne);
                PerformCalculate();
            }
        }
        public double Ne0
        {
            get { return Model.Ne0; }
            set
            {
                Model.Ne0 = value;
                OnPropertyChanged(() => Ne0);
                PerformCalculate();
            }
        }
        public Temperature Ta
        {
            get { return Model.Ta; }
            set
            {
                Model.Ta = value;
                OnPropertyChanged(() => Ta);
                PerformCalculate();
            }
        }
        public double Ktg
        {
            get { return Model.Ktg; }
            set
            {
                Model.Ktg = value;
                OnPropertyChanged(() => Ktg);
                PerformCalculate();
            }
        }
        public double PressureAir
        {
            get { return Model.Pa.MmHg; }
            set
            {
                Model.Pa = Pressure.FromMmHg(value);
                OnPropertyChanged(() => PressureAir);
                PerformCalculate();
            }
        }
        public CombustionHeat CombHeat
        {
            get { return Model.Qnp; }
            set
            {
                Model.Qnp = value;
                OnPropertyChanged(() => CombHeat);
                PerformCalculate();
            }
        }
        public double Tpc
        {
            get { return Model.Tpc; }
            set
            {
                Model.Tpc = value;
                OnPropertyChanged(() => Tpc);
                PerformCalculate();
            }
        }
#endregion
        protected override void SetValidationRules()
        {
            AddValidationFor(() => Qtg0)
                .When(() => Qtg0 <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => Ne)
                .When(() => Ne <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => Ne0)
                .When(() => Ne0 <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => Ktg)
                .When(() => Ktg <= 0 || Ktg > 2)
                .Show("Недопустимое значение (интервал допустимых значений от 0 до 2).");
            
            AddValidationFor(() => Ta)
                .When(() => ValueRangeHelper.TemperatureRange.IsOutsideRange(Ta))
                .Show(ValueRangeHelper.TemperatureRange.ViolationMessage);

            AddValidationFor(() => PressureAir)
                .When(() => ValueRangeHelper.PressureAirRange.IsOutsideRange(PressureAir))
                .Show(ValueRangeHelper.PressureAirRange.ViolationMessage);

            AddValidationFor(() => CombHeat)
                .When(() => ValueRangeHelper.CombHeatRange.IsOutsideRange(CombHeat))
                .Show(ValueRangeHelper.CombHeatRange.ViolationMessage);
            
            AddValidationFor(() => Tpc).When(() => Tpc <= 0 || Tpc > RangeInHours)
                           .Show($"Недопустимое значение (интервал допустимых значений 1 - {RangeInHours})");
        }
    }
}
