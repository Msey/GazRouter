using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GazRouter.Modes.GasCosts.Dialogs.Model;
namespace GazRouter.Modes.GasCosts.Dialogs.UnitStartCosts
{
    public class UnitStartCostsModel : Listing, ICostCalcModel
    {
        public UnitStartCostsModel()
        {
            ValveShiftings = new List<ValveShifting>();
            Unit = new CompUnit();
        }
        public double Calculate()
        {
            // Q газа на продувку, м³
            var qEv = ProfileIsNotEmpty ? 0 : Unit.InjectionProfileVolume * 3;
            // qEv = 3.0*12.5;
            //// Q газа на холодуную прокрутку, м³
            //// Для ГПА, у которых время холодной прокрутки алгоритмом пуска не оговаривается, 
            //// расход пускового газа для холодной прокрутки рассчитывают по формуле:
            //// Q газа пускового турбодетандера х время холодной прокрутки
            //var qDm = Unit.DryMotoringConsumption != 0
            //              ? Unit.DryMotoringConsumption
            //              : Unit.TurbineStarterConsumption * DryMotoringPeriod;

            // Q газа на холодную прокрутку, м³
            // В новом СТО нет оговорки для ГПА, у которых время холодной прокрутки алгоритмом пуска не оговаривается. 
            // расход пускового газа для холодной прокрутки берется строго в соответствие с технической документации ГПА
            var qDm = Unit.DryMotoringConsumption;
            //     qDm = 500.0;
            var shiftingCons = NormalShifting ? Unit.StartValveConsumption : ValveShiftings.Sum(vt => vt.Q);
            var qSt = Unit.TurbineStarterConsumption;
            //  qSt = 492.0;
            var calc = 0.001 * (Unit.TurbineStarterConsumption + qEv + qDm + shiftingCons) * StartCount;
#region listing
            ListingCalculation = string.Join("\n", new List<string>{
                    $"Qпуск = 0,001 * ({qSt} + {qEv} + {qDm} + {shiftingCons}) * {StartCount} = {calc} тыс.м³",
                    $"",
                    $"kпуск = {StartCount} - количество пусков ГПА",
                    $"Qтд = {qSt} м³ - технолог. потери газа при работе пускового турбодетандера",
                    ProfileIsNotEmpty ? $"Qпрод = {qEv} м³ - технолог. потери газа при продувке контура нагнетателя" : $"Qпрод = 3 * {Unit.InjectionProfileVolume} = {qEv} м³ - технолог. потери газа при продувке контура нагнетателя",
                    $"Vцбк = {Unit.InjectionProfileVolume} м³ - геометрический объем контура ЦБК и вспомогательных систем ГПА",
                    $"Qхп = {qDm} м³ - технолог. потери газа на холодную прокрутку",
                    NormalShifting ? $"Qзра = {shiftingCons} м³ - технолог. потери на перестановку ЗРА" : ValveShiftingsListing(ValveShiftings),
                });
#endregion listing
            return calc;
        }
        [Display(Name = "Количество пусков ГПА")]
        public int StartCount { get; set; }
        [Display(Name = "ГПА")]
        public CompUnit Unit { get; set; }
        //[Display(Name = "Время холодной прокрутки, с")]
        //public int DryMotoringPeriod { get; set; }
        [Display(Name = "Контур нагнетателя заполнен газом")]
        public bool ProfileIsNotEmpty { get; set; }
        [Display(Name = "Список переключений кранов (по типам)")]
        public List<ValveShifting> ValveShiftings { get; set; }
        [Display(Name = "Переключения кранов выполняются в соотв. с алгоритмом")]
        public bool NormalShifting { get; set; }
    }
}