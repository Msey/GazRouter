using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using Utils.Units;

namespace GazRouter.Modes.GasCosts.Dialogs.ReservePowerStationMaintenanceCosts
{
    public class ReservePowerStationMaintenanceCostsModel : Listing, ICostCalcModel
    {
        public double Calculate()
        {
            var calc = 0.001 * (PowerUnitType.RatedPower * PowerUnitType.FuelConsumptionRate * TurbineLoadCount * RunningTimeCoefficient * Period * 7000 / CombHeat.KCal +
                   TurbineConsumption * TurbineRuntime * TurbineColdCount + Qpr);
            #region listing
            ListingCalculation = string.Join("\n", new List<string>
                        {
                            $"Qри = 0,001 * ({PowerUnitType.RatedPower} * {PowerUnitType.FuelConsumptionRate} * {TurbineLoadCount} * {RunningTimeCoefficient} * {Period} * 7000 / {CombHeat} + {TurbineConsumption} * {TurbineRuntime} * {TurbineColdCount} + {Qpr}) = {calc} тыс.м³",
                            "",
                            $"Nэбₒ = {PowerUnitType.RatedPower} кВт - номинальная электрическая мощность электроагрегата",
                            $"Hэбₒ = {PowerUnitType.FuelConsumptionRate} кг у.т./(кВт*ч) - индивидуальный норматив расхода условного топлива на выработку электроэнергии энергоблоком",
                            $"kнар = {RunningTimeCoefficient} - коэффициент, учитывающий наработку агрегата (ГТУ или газопоршневого агрегата) с начала эксплуатации",
                            $"τэб = {Period} ч - время работы резервного энергоблока под нагрузкой",
                            $"Qᵸp = {CombHeat} ккал/м³ - низшая теплота сгорания газа",
                            $"qтд = {TurbineConsumption} м³/c - расход газа на работу турбодетандера ГТУ",
                            $"τтд = {TurbineRuntime} с - время работы турбодетандера",
                            $"kнагр = {TurbineLoadCount} - количество пусков резервного энергоблока для работы под нагрузкой",
                            $"kхп = {TurbineColdCount} - количество пусков резервного энергоблока для работы при холодной прокрутке",
                            $"Qпр = {Qpr} м³ - расход газа на прокрутку вала ГТУ энергоблока",
                        });
            #endregion
            return calc;
        }
        
        [Display(Name = "Тип энергоблока")]
        public PowerUnitType PowerUnitType { get; set; }
        [Display(Name = "Коэффициент наработки агрегата")]
        public double RunningTimeCoefficient { get; set; }
        [Display(Name = "Время работы энергоблока, ч")]
        public int Period { get; set; }
        [Display(Name = "Расход газа на работу турбодетандера ГТУ, м³/с")]
        public double TurbineConsumption { get; set; }
        [Display(Name = "Время работы турбодетандера, с")]
        public int TurbineRuntime { get; set; }
        [Display(Name = "Низшая теплота сгорания газа, ккал/м³")]
        public CombustionHeat CombHeat { get; set; }
        [Display(Name = "Расход газа на прокрутку вала ГТУ энергоблока, м³")]
        public double Qpr { get; set; }
        [Display(Name = "Количество пусков энергоблока для работы под нагрузкой")]
        public double TurbineLoadCount { get; set; }
        [Display(Name = "Количество пусков энергоблока для работы при холодной прокрутке")]
        public double TurbineColdCount { get; set; }
    }
}
