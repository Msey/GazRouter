using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.Modes.GasCosts.GasCompessibility;
using Utils.Calculations;
using Utils.Units;
using Pressure = Utils.Units.Pressure;
namespace GazRouter.Modes.GasCosts.Dialogs.PopValveTuningCosts
{
    public class PopValveTuningCostsModel : Listing, ICostCalcModel
    {
        public PopValveTuningCostsModel()
        {
            CheckCount = 1;
            CheckTime = 1;
        }
        public double Calculate()
        {            
            var p = Pressure + PressureAir;
            var t = Temperature;
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
            var z = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, p.Mpa, t.Kelvins);
            var listingK = "k";
            // показатель адиабаты (ГОСТ 30319.1)
            var k = SupportCalculations.AdiabaticIndex(p, t, Density, NitrogenContent / 100, ref listingK);
            var square = ValveHeight < 0.25 * ValveDiameter ? 
                            2.22 * ValveDiameter * ValveHeight
                            : Math.PI * Math.Pow(ValveDiameter, 2) / 4.0;
            var result = CheckCount * Math.Pow(2.0 / (k + 1), 1.0 / (k - 1)) * square *
                         Math.Sqrt(2.0 * k * StandardConditions.T.Kelvins /
                                  ((k + 1) * Density.KilogramsPerCubicMeter * StandardConditions.P.Mpa * t.Kelvins * z)) * p.Mpa * CheckTime;
#region listing
            var sign = ValveHeight < 0.25 * ValveDiameter ? "<" : ">="; 
            var squareCondition = $"{ValveHeight} {sign} 0,25 * {ValveDiameter}";
            var squareListing = ValveHeight < 0.25 * ValveDiameter ? $"2,22 * {ValveDiameter} * {ValveHeight} = {square} (т.к. {squareCondition})" : $"{Math.PI} * {ValveDiameter}^2 / 4,0 = {square} (т.к. {squareCondition})";
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Q =  {CheckCount} * (2/({k} + 1))^(1/({k}-1)) * {square} *КОРЕНЬ(2*{k} * {StandardConditions.T.Kelvins} /(({k} + 1) * {Density.KilogramsPerCubicMeter} * {StandardConditions.P.Mpa} * {t.Kelvins} * {z})) * {p.Mpa} * {CheckTime} = {result} тыс.м³",
               $"",
                $"kкл = {CheckCount} - количество проверок предохранительного клапана",
                $"τср = {CheckTime} с - время срабатывания предохранительного клапана",
                $"S = {squareListing} м² - площадь сечения предохранительного клапана",
                $"h = {ValveHeight} м - высота подъема клапана",
                $"d = {ValveDiameter} м - внутренний диаметр предохранительного клапана",
                $"TSt = {StandardConditions.T.ToString(TemperatureUnit.Kelvin)} - температура газа при стандартных условиях",
                $"PSt = {StandardConditions.P.ToString(PressureUnit.Mpa)} - давление газа при стандартных условиях",
                $"Tкл = {t.ToString(TemperatureUnit.Kelvin)} - температура газа при проверке клапана",
                $"Pкл = {Pressure.Mpa} + {PressureAir.Mpa} = {p.ToString(PressureUnit.Mpa)} - абсолютное давление газа при проверке предохранительного клапана",
                $"PAir = {PressureAir.ToString(PressureUnit.Mpa)} - атмосферное давление",
                $"ρ = {Density.KilogramsPerCubicMeter} кг/м³ - плотность газа",
                $"z = {z} - коэффициент сжимаемости (по ГОСТ 30319.3)",
                $"",
                $"{listingK}",
            });
#endregion
            return result;
        }
        [Display(Name = "Давление газа при настройке клапана, кг/см²")]
        public Pressure Pressure { get; set; }
        [Display(Name = "Температура газа при настройке клапана, Гр.С")]
        public Temperature Temperature  { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Плотность газа, кг/м³")]
        public Density Density { get; set; }
        [Display(Name = "Содержание азота в газе (мол.доля), %")]
        public double NitrogenContent { get; set; }
        [Display(Name = "Содержание двуокиси углерода СО2 в газе, мол.доля %")]
        public double CarbonDioxideContent { get; set; }
        [Display(Name = "Давление атмосферное, кг/см²")]
        public Pressure PressureAir { get; set; }
        [Display(Name = "Выбранный тип клапана")]
        public PopValve SelectedPopValve { get; set; }
        [Display(Name = "Внутренний диаметр клапана, м")]
        public double ValveDiameter { get; set; }
        [Display(Name = "Количество проверок клапана")]
        public int CheckCount { get; set; }
        [Display(Name = "Интервал срабатывания клапана, с")]
        public int CheckTime { get; set; }
        [Display(Name = "Высота подъема клапана, м")]
        public double ValveHeight { get; set; }
    }
}