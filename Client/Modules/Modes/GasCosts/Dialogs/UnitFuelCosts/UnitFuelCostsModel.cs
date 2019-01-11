using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.GasCompessibility;
using Utils.Calculations;
using Utils.Units;
namespace GazRouter.Modes.GasCosts.Dialogs.UnitFuelCosts
{
    public class UnitFuelCostsModel : Listing, ICostCalcModel
    {
        public UnitFuelCostsModel()
        {
            Unit = new CompUnit();
        }
        public double Calculate()
        {
            var pIn  = PressureInlet + PressureAir;
            var pOut = PressureOutlet + PressureAir;
            var listingL = "Lп";
            var gasContent = new List<ComponentContent>
            {
                new ComponentContent
                {
                    Component = PropertyType.ContentMethane,
                    Concentration = 1 - (NitrogenContent/100.0 + CarbonDioxideContent/100.0)
                },
                new ComponentContent
                {
                    Component = PropertyType.ContentNitrogen,
                    Concentration = NitrogenContent/100.0
                },
                new ComponentContent
                {
                    Component = PropertyType.ContentCarbonDioxid,
                    Concentration = CarbonDioxideContent/100.0
                },
            };
            var z = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, pIn.Mpa, TemperatureInlet.Kelvins);
            var l = SupportCalculations.PolytropicWorkOfCompression(pIn, pOut, TemperatureInlet, Q, z, ref listingL);
            var kA = 1.02 + 0.0025 * (TemperatureAir.Celsius + 5);
            var kU = 1 + 0.025 * (Unit.HasRecoveryBoiler ? 1 : 0);
            var k  = kA * kU;
            var n  = GasConsumptionRate * k;
            var calc = 0.001 * n * l * 7000 / CombHeat.KCal;
#region listing
            var unitHasRecoveryBoiler = Unit.HasRecoveryBoiler ? 1 : 0;
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Qтг = 0,001 * {n} * {l} * 7000 / {CombHeat} = {calc} тыс.м³",
                $"",
                $"Qᵸp = {CombHeat} ккал/м³ - низшая теплота сгорания газа",
                $"Hтг = {GasConsumptionRate} * {k} = {n} кг у.т./(кВт*ч) - норматив расхода условного топлива на компримирование газа КЦ",
                $"H°тг = {GasConsumptionRate} кг у.т./(кВт*ч) - индивидуальный норматив расхода условного топлива",
                $"kk = {kA} * {kU} = {k} - коэффициент коррекции, учитывающий условия работы КЦ",
                $"kуто = 1 + 0,025 * {unitHasRecoveryBoiler} = {kU} - коэффициент, учитывающий влияние эксплуатации УТО",
                $"ka = 1,02 + 0,0025 * ({TemperatureAir.Celsius} + 5) = {kA} - коэффициент, учитывающий влияние атмосферных условий",
                $"TAir = {TemperatureAir.ToString(TemperatureUnit.Celsius)} - температура воздуха",
                $"",
                $"{listingL}",
                $"z = {z} - коэффициент сжимаемости (по ГОСТ 30319.3)",
                $"T1 = {TemperatureInlet.ToString(TemperatureUnit.Kelvin)} - температура газа на входе КЦ",
                $"Qкц = {Q} млн.м³ - объем газа, компримируемого КЦ",
                $"P1 = {PressureInlet.Mpa} + {PressureAir.Mpa} = {pIn.ToString(PressureUnit.Mpa)} - абсолютное давление газа на входе",
                $"P2 = {PressureOutlet.Mpa} + {PressureAir.Mpa} = {pOut.ToString(PressureUnit.Mpa)} - абсолютное давление газа на выходе",
                $"PAir = {PressureAir.ToString(PressureUnit.Mpa)} - атмосферное давление",
            });
#endregion 
            return calc;
        }
        [Display(Name = "Перекачка газа газа через ГПА, млн.м3")]
        public double Q { get; set; }
        //[Display(Name = "Плотность газа, кг/м3")]
        //public Density Density { get; set; }
        [Display(Name = "Низшая теплота сгорания газа, ккал/м3")]
        public CombustionHeat CombHeat { get; set; }
        [Display(Name = "Атмосферное давление, кг/см2")]
        public Pressure PressureAir { get; set; }
        [Display(Name = "Температура воздуха, Гр.С")]
        public Temperature TemperatureAir { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Индивидуальная норма расхода условного топлива ГПА, кг у.т./кВт*ч")]
        public double GasConsumptionRate { get; set; } 
        [Display(Name = "Давление газа на входе КЦ, кг/см2")]
        public Pressure PressureInlet { get; set; }
        [Display(Name = "Давление газа на выходе КЦ, кг/см2")]
        public Pressure PressureOutlet { get; set; }
        [Display(Name = "Температура газа на выходе КЦ, Гр.С")]
        public Temperature TemperatureInlet { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "ГПА")]
        public CompUnit Unit { get; set; }
        [Display(Name = "Содержание азота в газе, мол.доля %")]
        public double NitrogenContent { get; set; }
        [Display(Name = "Содержание двуокиси углерода СО2 в газе, мол.доля %")]
        public double CarbonDioxideContent { get; set; }
    }
}