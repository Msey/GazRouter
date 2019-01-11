using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.GasCompessibility;
using Utils.Calculations;
using Utils.Units;

namespace GazRouter.Modes.GasCosts.Dialogs.HydrateRemoveCosts
{
    public class HydrateRemoveCostsModel : Listing, ICostCalcModel
    {
        public HydrateRemoveCostsModel()
        {
            PurgeTime = 1;
            FillingCount = 1;
        }

        [Display(Name = "Давление газа в начале участка, кг/см²")]
        public Pressure PressureIn { get; set; }

        [Display(Name = "Давление газа в конце участка, кг/см²")]
        public Pressure PressureOut { get; set; }

        [Display(Name = "Температура газа в начале участка, Гр.С")]
        public Temperature TemperatureIn { get; set; } = Temperature.FromCelsius(0);

        [Display(Name = "Температура газа в конце участка, Гр.С")]
        public Temperature TemperatureOut { get; set; } = Temperature.FromCelsius(0);

        [Display(Name = "Плотность газа, кг/м³")]
        public Density Density { get; set; }

        [Display(Name = "Содержание азота в газе, мол.доля %")]
        public double NitrogenContent { get; set; }

        [Display(Name = "Содержание двуокиси углерода СО2 в газе, мол.доля %")]
        public double CarbonDioxideContent { get; set; }

        [Display(Name = "Диаметр свечного крана, м")]
        public Bleeder Bleeder { get; set; }

        [Display(Name = "Время продувки, с")]
        public int PurgeTime { get; set; }

        [Display(Name = "Длина дренажной линии, м")]
        public double Length { get; set; }

        [Display(Name = "Геометрический объем метанольного устройства, м³")]
        public double MethanolTankVolume { get; set; }

        [Display(Name = "Кол-во заправок метанольного устройства")]
        public int FillingCount { get; set; }

        [Display(Name = "Давление атмосферное мм рт.ст.")]
        public Pressure PressureAir { get; set; }

        public double Calculate()
        {
            var pAir = PressureAir;
            var p = (PressureIn + PressureOut)/2 + pAir;
            var tAvg = (TemperatureIn + TemperatureOut)/2;
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
            var z = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, p.Mpa, tAvg.Kelvins);

            //  Наименьший из диаметров поперечного сечения по всей длине дренажной линии
            //  (дроссельная шайба, свечной кран, участок трубы дренажной линии), м
            //var minDiameter = Math.Min(Bleeder.Diameter, Bleeder.Diameter); //TODO: здесь какая то ошибка

            // Расчет для критического истечения газа
            //var c = SupportCalculations.VelocityOfSound(p, tAvg, Density, NitrogenContent/100);
            //var q = Math.PI*Bleeder.Diameter*Bleeder.Diameter*c/4;
            //var m = SupportCalculations.DynamiсViscosity(p, tAvg, Density, NitrogenContent/100,
            //    CarbonDioxideContent/100);
            //var re = SupportCalculations.ReynoldsNumber(m, Density, Bleeder.Diameter, q);

            ////коэффициент расхода
            //var kq = re == 0 ? 0 : 0.587 + 5.5/Math.Sqrt(re) + 0.348/Math.Pow(re, 1.0/3.0) - 110.92/re;

            // показатель адиабаты (ГОСТ 30319.1)
            var listingK = "k";
            var k = SupportCalculations.AdiabaticIndex(p, tAvg, Density, NitrogenContent/100, ref listingK);

            var square = Math.PI * Math.Pow(Bleeder.Diameter, 2) / 4.0;
            var coefL = K.CalcK(p.Kgh, Bleeder.Diameter, Length);
            var Qp = coefL * Math.Pow(2.0 / (k + 1), 1.0 / (k - 1)) * square *
                        Math.Sqrt(2.0 * k / ((k + 1) * Density.KilogramsPerCubicMeter * tAvg.Kelvins * z) * StandardConditions.T.Kelvins / StandardConditions.P.Mpa) * p.Mpa * PurgeTime;

            //var Qp = 18.591*kq
            //         *Math.PI*Math.Pow(minDiameter, 2)/4
            //         *StandardConditions.T.Kelvins/StandardConditions.P.Mpa
            //         *Math.Pow(2/(k + 1), 1/(k - 1))
            //         *Math.Sqrt(k/Density.KilogramsPerCubicMeter/z/tAvg.Kelvins)*p.Mpa*PurgeTime/1000;

            var Qm = 0.001*StandardConditions.T.Kelvins/StandardConditions.P.Mpa*MethanolTankVolume*p.Mpa/tAvg.Kelvins/z*
                     FillingCount;

            #region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Qгп = {Qp} + {Qm} = {Qp + Qm} тыс.м³- объем газа, расходуемый при ликвидации гидратных пробок",
                "",
                $"Qпрод = {coefL} * СТЕПЕНЬ(2/({k}+1); 1/({k}-1)) * {square} * КОРЕНЬ(2*{k}/(({k}+1)*{Density.KilogramsPerCubicMeter}*{tAvg.Kelvins}*{z})*{StandardConditions.T.Kelvins}/{StandardConditions.P.Mpa}) * {p.Mpa} * {PurgeTime} = {Qp} тыс.м³ - объем газа, расходуемый при продувке участка газопровода через свечу",
                $"T = 0,5 * ({TemperatureIn.Kelvins} + {TemperatureOut.Kelvins}) = {tAvg.ToString(TemperatureUnit.Kelvin)} - температура газа в аппарате",
                $"P = 0,5 * ({PressureIn.Mpa} + {PressureOut.Mpa}) + {pAir.Mpa} = {p.ToString(PressureUnit.Mpa)} - абсолютное давление газа в аппарате",
                $"ρ = {Density.KilogramsPerCubicMeter} кг/м³ - плотность газа",
                $"z = {z} - коэффициент сжимаемости при P, T (по ГОСТ 30319.3)",
                $"τпр = {PurgeTime} c - время продувки",
                $"kL = {coefL}  коэффициент, учитывающий влияние длины дренажной линии на скорость продувки",
                $"S = {Math.PI} * ({Bleeder.Diameter}^2) / 4,0 = {square} м² - площадь поперечного сечения свечи",
                $"dсв = {Bleeder.Diameter} м - диаметр поперечного сечения свечи",
                $"PAir = {PressureAir.ToString(PressureUnit.Mpa)} - атмосферное давление",
                $"TSt = {StandardConditions.T.ToString(TemperatureUnit.Kelvin)} - температура газа при стандартных условиях",
                $"PSt = {StandardConditions.P.ToString(PressureUnit.Mpa)} - абсолютное давление газа при стандартных условиях",
                "",
                $"Qмет = 0,001 * {StandardConditions.T.Kelvins} / {StandardConditions.P.Mpa} * {MethanolTankVolume} * {p.Mpa} / {tAvg.Kelvins} / {z} * {FillingCount} = {Qm} тыс.м³ - объем газа, расходуемый при заправке метанольного устройства на участке МГ",
                $"Vм = {MethanolTankVolume} - геометрический объем метанольного устройства",
                "",
                $"{listingK}",
            });
            #endregion

            return Qp + Qm;
        }
    }
}