using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using Utils.Units;
namespace GazRouter.Modes.GasCosts.Dialogs.CoolingCosts
{
    public class CoolingCostsModel : Listing, ICostCalcModel
    {
        public double Calculate()
        {
            if (UnitType == null)
            {
                return 0.0;
            }
            var k = 1.02 + 0.0025 * (TemperatureAir.Celsius + 5);
            var calc = 0.001 * UnitType.RatedPower * UnitType.FuelConsumptionRate * k * Period * 7000 / CombHeat.KCal;
#region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Q = 0,001 * {UnitType.RatedPower} * {UnitType.FuelConsumptionRate} * {k} * {Period} * 7000 / {CombHeat} = {calc} тыс. м³",
                $"",
                $"N°сог = {UnitType.RatedPower} кВт - норматив располагаемой мощности ГТУ СОГ",
                $"H°сог = {UnitType.FuelConsumptionRate} кг у.т./(кВт*ч) - индивидуальный норматив расхода условного топлива ГТУ СОГ",
                $"ka = 1,02 + 0,0025 * ({TemperatureAir.Celsius} + 5) = {k} - коэффициент, учитывающий влияния атмосферных условий",
                $"TAir = {TemperatureAir.ToString(TemperatureUnit.Celsius)} - температура воздуха",
                $"τсог = {Period} ч - время работы СОГ",
                $"Qᵸp = {CombHeat} ккал/м³ - низшая теплота сгорания газа",                
            });
#endregion
            return calc;
        }
        [Display(Name = "Тип газотурбинного привода")]
        public CoolingUnitType UnitType { get; set; }
        [Display(Name = "Время работы установки СОГ, ч")]
        public int Period { get; set; }
        [Display(Name = "Средняя температура воздуха за период работы СОГ, Гр.С")]
        public Temperature TemperatureAir { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Низшая теплота сгорания газа, ккал/м³")]
        public CombustionHeat CombHeat { get; set; }
    }
}