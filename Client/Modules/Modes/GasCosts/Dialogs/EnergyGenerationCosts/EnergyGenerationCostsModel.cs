using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using Utils.Units;
namespace GazRouter.Modes.GasCosts.Dialogs.EnergyGenerationCosts
{
    public class EnergyGenerationCostsModel : Listing, ICostCalcModel
    {
        public double Calculate()
        {
            // Коэф. загрузки энергоблока
            var kz = Power / PowerUnitType.RatedPower;
            var kn = PowerUnitType.EngineGroup == 0
                ? 0.75 + 0.25 / kz * Math.Sqrt((TemperatureAir.Kelvins + 5) / 288)
                : 0.89 + 0.1 / kz + 0.01 / (kz * kz);
            var calc = 0.001 * (Power * PowerUnitType.FuelConsumptionRate * kn * RunningTimeCoefficient * Period * 7000 / CombHeat.KCal +
                   TurbineConsumption * TurbineRuntime * TurbineStartCount);
            #region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Qэсн = 0,001 * ({Power} * {PowerUnitType.FuelConsumptionRate} * {kn} * {RunningTimeCoefficient} * {Period} * 7000 / {CombHeat} + {TurbineConsumption} * {TurbineRuntime} * {TurbineStartCount})= {calc} тыс.м³",
                "",
                $"Nэб = {Power} кВт - электрическая мощность электроблока",
                $"Hэб = {PowerUnitType.FuelConsumptionRate} кг у.т./(кВт*ч) - норма расхода условного топлива на выработку электроэнергии энергоблоком в зависимости от нагрузки",
                PowerUnitType.EngineGroup == 0 ? $"kn = 0,75 + 0,25 / {kz} * (({TemperatureAir.Kelvins} + 5) / 288)^0,5 = {kn} - коэффициент, учитывающий влияние снижения мощности электроагрегата": $"kn = 0,89 + 0,1 / {kz} + 0,01 / ({kz}*{kz}) = {kn} - коэффициент, учитывающий влияние снижения мощности электроагрегата",
                $"kz = {Power} / {PowerUnitType.RatedPower} = {kz} - коэффициент загрузки энергоблока",
                $"Nэбₒ = {PowerUnitType.RatedPower} кВт - номинальная электрическая мощность электроагрегата",
                $"TAir = {TemperatureAir.Kelvins} - температура воздуха",
                $"kнар = {RunningTimeCoefficient} - коэффициент, учитывающий наработку агрегата (ГТУ или газопоршневого агрегата) с начала эксплуатации",
                $"τэб = {Period} ч - время работы энергоблока",
                $"Qᵸp = {CombHeat} ккал/м³ - низшая теплота сгорания газа",
                $"qтд = {TurbineConsumption} м³/c - расход газа на работу турбодетандера ГТУ",
                $"τтд = {TurbineRuntime} с - время работы турбодетандера",
                $"kзап = {TurbineStartCount} - количество запусков ГТУ энергоблока",
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
        [Display(Name = "Электрическая мощность энергоблока, кВт")]
        public int Power { get; set; }
        [Display(Name = "Температура воздуха, Гр.С")]
        public Temperature TemperatureAir { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Расход газа на работу турбодетандера ГТУ, м³/с")]
        public double TurbineConsumption { get; set; }
        [Display(Name = "Время работы турбодетандера, с")]
        public int TurbineRuntime { get; set; }
        [Display(Name = "Количество запусков ГТУ энергоблока")]
        public int TurbineStartCount { get; set; }
        [Display(Name = "Низшая теплота сгорания газа, ккал/м³")]
        public CombustionHeat CombHeat { get; set; }
    }
}