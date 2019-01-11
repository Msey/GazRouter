using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.GasCompessibility;
using Utils.Calculations;
using Utils.Units;
namespace GazRouter.Modes.GasCosts.Dialogs.PurgeCosts
{
    public class PurgeCostsModel : Listing, ICostCalcModel
    {
        public PurgeCostsModel()
        {
            PurgeCount = 1;
            PurgeTime = 1;
        }
        public double Calculate()
        {
            var pAir = PressureAir;
            var p = Pressure + pAir;
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
            
            ////  Наименьший из диаметров поперечного сечения по всей длине дренажной линии
            ////  (дроссельная шайба, свечной кран, участок трубы дренажной линии), м
            //var minDiameter = Min(PipeDiameter, Bleeder.Diameter);
            // показатель адиабаты (ГОСТ 30319.1)
            var listingK = "k";
            var k = SupportCalculations.AdiabaticIndex(p, t, Density, NitrogenContent / 100, ref listingK);
            //  Расход газа на одну продувку
            var square = Math.PI*Math.Pow(Bleeder.Diameter, 2)/4.0;
            var coefL = K.CalcK(p.Kgh, Bleeder.Diameter, PipeLength);
            var qPerPurge = coefL*Math.Pow(2.0/(k + 1), 1.0/(k - 1))*square*
                            Math.Sqrt(2*k/((k + 1)*Density.KilogramsPerCubicMeter*t.Kelvins*z)*
                                       StandardConditions.T.Kelvins/StandardConditions.P.Mpa)*p.Mpa*PurgeTime;
#region previous
            //if (PipeLength < Bleeder.CriticalLength)
            //{
            //    // Расчет для критического истечения газа
            //    var c = SupportCalculations.VelocityOfSound(p, t, Density, NitrogenContent / 100);
            //    var q = PI*PipeDiameter*PipeDiameter*c/4;
            //    var m = SupportCalculations.DynamiсViscosity(p, t, Density, NitrogenContent/100,
            //        CarbonDioxideContent/100);
            //    var re = SupportCalculations.ReynoldsNumber(m, Density, PipeDiameter, q);

            //    //коэффициент расхода
            //    var kq = re == 0 ? 0 : 0.587 + 5.5/Sqrt(re) + 0.348/Pow(re, 1.0/3.0) - 110.92/re;

            //    // показатель адиабаты (ГОСТ 30319.1)
            //    var k = SupportCalculations.AdiabaticIndex(p, t, Density, NitrogenContent / 100); 

            //    qPerPurge = 18.591*kq
            //        * PI * Pow(minDiameter, 2) / 4
            //        *StandardConditions.T.Kelvins/StandardConditions.P.Mpa
            //        *Pow(2/(k + 1), 1/(k - 1))
            //        *Sqrt(k/Density.KilogramsPerCubicMeter/z/t.Kelvins)*p.Mpa*PurgeTime / 1000;
            //}
            //else
            //{
            //    const double re = 5000.0;
            //    //  Эквивалентная шероховатость трубы дренажной линии, мм
            //    var ke = HasInnerCover ? 0.01 : 0.03;

            //    // коэффициент гидравлического сопротивления
            //    var lambda = PipeDiameter == 0? 0 : 0.067 * Pow(158.0/re + ke/1000/PipeDiameter, 0.2);

            //    qPerPurge = PI / 4 * Pow(minDiameter, 2)
            //        * Sqrt(
            //            StandardConditions.T.Kelvins * ( p.Mpa * p.Mpa - pAir.Mpa*pAir.Mpa) * minDiameter
            //            / (Density.KilogramsPerCubicMeter * StandardConditions.P.Mpa * t.Kelvins * z * (lambda * PipeLength + 2 * minDiameter * Log(p / pAir)))) * PurgeTime;

            //}
            #endregion
            var result = qPerPurge * PurgeCount;
#region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Qап = {qPerPurge} * {PurgeCount} = {result} тыс.м³",
                "",
                $"kап = {PurgeCount} - количество продувок аппарата",
                $"Qпр_ап = {coefL} * (2/({k}+1))^(1/({k}-1)) * {square} * КОРЕНЬ(2*{k} / (({k}+1) * {Density.KilogramsPerCubicMeter} * {t.Kelvins} * {z}) * {StandardConditions.T.Kelvins}/{StandardConditions.P.Mpa}) * {p.Mpa} * {PurgeTime} = {qPerPurge} тыс.м³ - объем газа, стравливаемого при продувке аппарата через свечу",
                $"kL = {coefL} - коэффициент, учитывающий влияние длины дренажной линии на скорость продувки",
                $"dсв = {Bleeder.Diameter} м - диаметр поперечного сечения свечи",
                $"S = {Math.PI} * ({Bleeder.Diameter}^2) / 4,0 = {square} м² - площаль сечения свечи",
                $"ρ = {Density.KilogramsPerCubicMeter} кг/м³ - плотность газа",
                $"Tап = {t.ToString(TemperatureUnit.Kelvin)} - абсолютная температура газа в аппарате",
                $"Pап = {Pressure.Mpa} + {pAir.Mpa} = {p.ToString(PressureUnit.Mpa)} - абсолютное давление газа в аппарате",
                $"PAir = {pAir.Mpa} - атмосферное давление",
                $"z = {z} - коэффициент сжимаемости (по ГОСТ 30319.3)",
                $"τпр = {PurgeTime} с - время продувки",
                $"TSt = {StandardConditions.T.ToString(TemperatureUnit.Kelvin)} - температура газа при стандартных условиях",
                $"PSt = {StandardConditions.P.ToString(PressureUnit.Mpa)} - давление газа при стандартных условиях",
                "",
                $"{listingK}",
            });
#endregion
            return result;
        }
        [Display(Name = "Давление газа в аппарате, кг/см²")]
        public Pressure Pressure { get; set; }
        [Display(Name = "Температура газа в аппарате, Гр.С")]
        public Temperature Temperature  { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Плотность газа, кг/м³")]
        public Density Density { get; set; }
        [Display(Name = "Содержание азота в газе, мол.доля %")]
        public double NitrogenContent { get; set; }
        [Display(Name = "Содержание двуокиси углерода СО2 в газе, мол.доля %")]
        public double CarbonDioxideContent { get; set; }
        [Display(Name = "Давление атмосферное, кг/см²")]
        public Pressure PressureAir { get; set; }
        [Display(Name = "Длина дренажной линии, м")]
        public double PipeLength { get; set; }
        [Display(Name = "Диаметр свечного крана, м")]
        public Bleeder Bleeder { get; set; }
        [Display(Name = "Количество продувок")]
        public int PurgeCount { get; set; }
        [Display(Name = "Время продувки, с")]
        public int PurgeTime { get; set; }
    }
}