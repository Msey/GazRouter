using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using Telerik.Windows.Controls.Map;
using Utils.Units;
namespace GazRouter.Modes.GasCosts.Dialogs.KptgOwnNeedsCosts
{
    public class KptgOwnNeedsCostsModel : Listing, ICostCalcModel
    {
        public double Calculate()
        {
            var h_kptg = 2.05;
            var qkptg_pp   = 0.001 * h_kptg*Qkptg;
            var qkptg_kto  = 0.001 * Hto*Motx;
            var qkptg_tp   = 0.001 * (Kp*Qreg*Treg + Qdoj*Tdoj + Qpod*Tpod);
            var qkptg_kond = 1000 * 24.04*Gkond*Cg / Mg;
            var result = qkptg_pp + qkptg_kto + qkptg_tp + qkptg_kond;
#region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                 $"Qкптг = {qkptg_pp} + {qkptg_kto} + {qkptg_tp} + {qkptg_kond} = {result} тыс.м³",
                 "",
                 $"Qкптг_pp = 0,001 * {h_kptg} * {Qkptg} = {qkptg_pp} тыс.м³ - расход газа на эксплуатацию КПТГ",
                 $"H_кптг = {h_kptg} м³/тыс.м³ - норматив удельного расхода газа на эксплуатацию КПТГ",
                 $"Qкптг = {Qkptg} тыс.м³ - объем газа, проходящий через КПТГ за расчетный период",
                 $"Qкптг_кто = 0,001 * {Hto} * {Motx} = {qkptg_kto} тыс.м³ - расход газа для работы комплекса термического обезвреживания",
                 $"Hтₒ = {Hto} м³/кг - норматив расхода топлива на сжигание 1 кг отходов",
                 $"mотх = {Motx} кг - масса жидких и твёрдых отходов, подвергшихся утилизации",
                 $"Qкптг_тг = 0,001 * {Kp} * ({Qreg} * {Treg}) + {Qdoj}*{Tdoj} + {Qpod}*{Tpod} = {qkptg_tp} тыс.м³ - расход газа для печей регенерации, дожига и подогрева жидкого теплоносителя",
                 $"kp = {Kp} - количество работающих печей регенерации",
                 $"qрег = {Qreg} м³/ч - расход газа для работы печей регенерации",
                 $"τрег = {Treg} ч - расчетное время работы печей регенерации",
                 $"qдож = {Qdoj} м³/ч - расход газа для работы печей дожига",
                 $"τдож = {Tdoj} ч - расчетное время работы печей дожига",
                 $"qпод = {Qpod} м³/ч - расход газа для работы печей подогрева жидкого теплоносителя",
                 $"τпод = {Tpod} ч - расчетное время работы печей подогрева жидкого теплоносителя",
                 $"Qкптг_конд = 1000 * 24,04 * {Gkond} * {Cg} / {Mg} = {qkptg_kond} тыс.м³ - расход газа, перешедшего в конденсат при подготовке газа к транспорту на КПТГ в соответствии с технологической схемой",
                 $"Gконд = {Gkond} т - количество газового конденсата, получаемого в расчетном периоде",
                 $"Cг = {Cg} масс. доля - концентрация компонентов «сухого» газа в товарном конденсате газовом",
                 $"Mг = {Mg} кг/кмоль - средняя молярная масса компонентов газа, содержащихся в газовом конденсате",
            });
#endregion
            return result;
        }
        [Display(Name = "Норматив удельного расхода газа на эксплуатацию КПТГ, тыс.м³")]
        public double Qkptg { get; set; }

        [Display(Name = "Норматив расхода топлива на сжигание 1 кг отходов, м³/кг")]
        public double Hto { get; set; }
        
        [Display(Name = "Масса жидких и твёрдых отходов, подвергшихся утилизации, м³/кг")]
        public double Motx { get; set; }

        [Display(Name = "Количество работающих печей регенерации")]
        public double Kp { get; set; }

        [Display(Name = "Расход газа для работы печей регенерации, м³/ч")]
        public double Qreg { get; set; }

        [Display(Name = "Расчетное время работы печей регенерации, ч")]
        public double Treg { get; set; }

        [Display(Name = "Расход газа для работы печей дожига, м³/ч")]
        public double Qdoj { get; set; }

        [Display(Name = "Расчетное время работы печей дожига , ч")]
        public double Tdoj { get; set; }

        [Display(Name = "Расход газа для работы печей подогрева жидкого теплоносителя, м³/ч")]
        public double Qpod { get; set; }

        [Display(Name = "Расчетное время работы печей подогрева жидкого теплоносителя, ч")]
        public double Tpod { get; set; }

        [Display(Name = "Количество газового конденсата, получаемого в расчетном периоде, т")]
        public double Gkond { get; set; }

        [Display(Name = "Концентрация компонентов «сухого» газа в товарном конденсате, масс. доля")]
        public double Cg { get; set; }

        [Display(Name = "Средняя молярная масса компонентов газа, содержащихся в газовом конденсате, кг/кмоль")]
        public double Mg { get; set; }
    }
}