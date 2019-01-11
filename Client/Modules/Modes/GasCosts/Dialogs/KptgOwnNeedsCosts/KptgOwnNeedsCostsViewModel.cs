using System;
using System.Collections.Generic;
using GazRouter.Common.Ui.Templates;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.EnergyGenerationCosts;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
using JetBrains.Annotations;
namespace GazRouter.Modes.GasCosts.Dialogs.KptgOwnNeedsCosts
{
    public class KptgOwnNeedsCostsViewModel : CalcViewModelBase<KptgOwnNeedsCostsModel>
    {
        public KptgOwnNeedsCostsViewModel([NotNull] GasCostDTO gasCost, Action<GasCostDTO> closeCallback, List<DefaultParamValues> defaultParamValues, bool ShowDayly,bool useMeasuringLoader = false) : base(gasCost, closeCallback, defaultParamValues, useMeasuringLoader)
        {

            this.ShowDayly = ShowDayly;
            if (!IsEdit)
            {

            }
            SetValidationRules();
            PerformCalculate();

            QkptgDescr = new FormulaFormatDescription("Q", "КПТГ");
            HtoDescr = new FormulaFormatDescription("Н", "m0");
            MotxDescr = new FormulaFormatDescription("m", "отх");
            KpDescr = new FormulaFormatDescription("k", "р");
            QregDescr = new FormulaFormatDescription("q", "рег");
            TregDescr = new FormulaFormatDescription("τ", "рег");
            QdojDescr = new FormulaFormatDescription("q", "дож");
            TdojDescr = new FormulaFormatDescription("τ", "дож");
            QpodDescr = new FormulaFormatDescription("q", "под");
            TpodDescr = new FormulaFormatDescription("τ", "под");
            GkondDescr = new FormulaFormatDescription("G", "конд");
            CgDescr = new FormulaFormatDescription("C", "г");
            MgDescr = new FormulaFormatDescription("M", "г");

        }

#region description
        public FormulaFormatDescription QkptgDescr { get; set; }
        public FormulaFormatDescription HtoDescr { get; set; }
        public FormulaFormatDescription MotxDescr { get; set; }
        public FormulaFormatDescription KpDescr { get; set; }
        public FormulaFormatDescription QregDescr { get; set; }
        public FormulaFormatDescription TregDescr { get; set; }
        public FormulaFormatDescription QdojDescr { get; set; }
        public FormulaFormatDescription TdojDescr { get; set; }
        public FormulaFormatDescription QpodDescr { get; set; }
        public FormulaFormatDescription TpodDescr { get; set; }
        public FormulaFormatDescription GkondDescr { get; set; }
        public FormulaFormatDescription CgDescr { get; set; }
        public FormulaFormatDescription MgDescr { get; set; }
#endregion

        public double Qkptg
        {
            get { return Model.Qkptg; }
            set
            {
                Model.Qkptg = value;
                OnPropertyChanged(() => Qkptg);
                PerformCalculate();
            }
        }
        public double Hto
        {
            get { return Model.Hto; }
            set
            {
                Model.Hto = value;
                OnPropertyChanged(() => Hto);
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
        public double Kp
        {
            get { return Model.Kp; }
            set
            {
                Model.Kp = value;
                OnPropertyChanged(() => Kp);
                PerformCalculate();
            }
        }
        public double Qreg
        {
            get { return Model.Qreg; }
            set
            {
                Model.Qreg = value;
                OnPropertyChanged(() => Qreg);
                PerformCalculate();
            }
        }
        public double Treg
        {
            get { return Model.Treg; }
            set
            {
                Model.Treg = value;
                OnPropertyChanged(() => Treg);
                PerformCalculate();
            }
        }
        public double Qdoj
        {
            get { return Model.Qdoj; }
            set
            {
                Model.Qdoj = value;
                OnPropertyChanged(() => Qdoj);
                PerformCalculate();
            }
        }
        public double Tdoj
        {
            get { return Model.Tdoj; }
            set
            {
                Model.Tdoj = value;
                OnPropertyChanged(() => Tdoj);
                PerformCalculate();
            }
        }
        public double Qpod
        {
            get { return Model.Qpod; }
            set
            {
                Model.Qpod = value;
                OnPropertyChanged(() => Qpod);
                PerformCalculate();
            }
        }
        public double Tpod
        {
            get { return Model.Tpod; }
            set
            {
                Model.Tpod = value;
                OnPropertyChanged(() => Tpod);
                PerformCalculate();
            }
        }
        public double Gkond
        {
            get { return Model.Gkond; }
            set
            {
                Model.Gkond = value;
                OnPropertyChanged(() => Gkond);
                PerformCalculate();
            }
        }
        public double Cg
        {
            get { return Model.Cg; }
            set
            {
                Model.Cg = value;
                OnPropertyChanged(() => Cg);
                PerformCalculate();
            }
        }
        public double Mg
        {
            get { return Model.Mg; }
            set
            {
                Model.Mg = value;
                OnPropertyChanged(() => Mg);
                PerformCalculate();
            }
        }

        protected override void SetValidationRules()
        {
            AddValidationFor(() => Qkptg).When(() => Qkptg <= 0).Show("Недопустимое значение. Должно быть больше 0.");
            AddValidationFor(() => Hto).When(() => Hto <= 0).Show("Недопустимое значение. Должно быть больше 0.");
            AddValidationFor(() => Motx).When(() => Motx <= 0).Show("Недопустимое значение. Должно быть больше 0.");
            AddValidationFor(() => Kp).When(() => Kp <= 0).Show("Недопустимое значение. Должно быть больше 0.");
            AddValidationFor(() => Qreg).When(() => Qreg <= 0).Show("Недопустимое значение. Должно быть больше 0.");
            AddValidationFor(() => Treg).When(() => Treg <= 0).Show("Недопустимое значение. Должно быть больше 0.");
            AddValidationFor(() => Qdoj).When(() => Qdoj <= 0).Show("Недопустимое значение. Должно быть больше 0.");
            AddValidationFor(() => Tdoj).When(() => Tdoj <= 0).Show("Недопустимое значение. Должно быть больше 0.");
            AddValidationFor(() => Qpod).When(() => Qpod <= 0).Show("Недопустимое значение. Должно быть больше 0.");
            AddValidationFor(() => Tpod).When(() => Tpod <= 0).Show("Недопустимое значение. Должно быть больше 0.");
            AddValidationFor(() => Gkond).When(() => Gkond <= 0).Show("Недопустимое значение. Должно быть больше 0.");
            AddValidationFor(() => Cg).When(() => Cg <= 0).Show("Недопустимое значение. Должно быть больше 0.");
            AddValidationFor(() => Mg).When(() => Mg <= 0).Show("Недопустимое значение. Должно быть больше 0.");
        }
    }
}
