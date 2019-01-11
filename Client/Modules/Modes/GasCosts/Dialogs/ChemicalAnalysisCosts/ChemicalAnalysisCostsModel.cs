using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Automation;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using Utils.Calculations;
using Utils.Units;

namespace GazRouter.Modes.GasCosts.Dialogs.ChemicalAnalysisCosts
{
    public class ChemicalAnalysisCostsModel : Listing, ICostCalcModel
    {
        public double Calculate()
        {
            double qSampling;
            var molMass = SupportCalculations.MolarMass(NitrogenContent / 100.0, CarbonDioxideContent / 100.0);
            var a = 19783.0*Math.Pow(molMass, -0.545);
            var pAbs = P + PressureAir;
            var result = 0.001*(Q/1000)*MeasCount*Time;
            if (Mode == 1)
            {
                var V1 = 10.2 * a * Square * pAbs.Mpa * (PurgeTime / Math.Sqrt(TemperatureAir.Kelvins));
                var V2 = 2894.0 * Volume * pAbs.Mpa / T.Kelvins;
                qSampling = 0.001 * 1.3 * (V1 + V2) * TestCount;
                result += qSampling;

                ListingCalculation = string.Join("\n", new List<string>
                {
                    $"Result = 0,001 * {Q/1000} * {MeasCount} * {Time} + {qSampling} = {result} тыс.м³",
                    "",
                    $"qхр = {Q} л/мин / 1000 = {Q/1000} м³/мин - расход анализируемого газа в хроматографе",
                    $"nхр = {MeasCount} - количество измерений",
                    $"τхр = {Time} мин - время проведения анализа",
                    $"",
                    $"Qпр = 0,001 * 1,3 * ({V1} + {V2}) * {TestCount} = {qSampling} тыс.м³ - расход газа при взятии проб (в соответствии с СТО Газпром 11-2005)",
                    $"V1 = 10,2 * {a} * {Square} * {pAbs.Mpa} * ({PurgeTime} / КОРЕНЬ({TemperatureAir.Kelvins})) = {V1} м³ - объем продукта, расходуемого на продувку анализной линии и пробоотборника перед отбором пробы",
                    $"V2 = 2894*{Volume}*{pAbs.Mpa}/{T.Kelvins} = {V2} м³ - объем одной отбираемой пробы",
                    $"n = {TestCount} - количество анализов данного вида",
                    $"A = {a} - коэффициент, зависящий от молекулярной массы газа",
                    $"M = {molMass} г/моль - молекулярная масса газа",
                    $"F = {Square} м² - площадь продувочного сечения вентиля",
                    $"P = {P.Mpa} + {PressureAir.Mpa} = {pAbs.ToString(PressureUnit.Mpa)} - абсолютное давление газа",
                    $"T = {T.ToString(TemperatureUnit.Kelvin)} - температура газа",
                    $"τ = {PurgeTime} с - продолжительность продувки",
                    $"Vгеом_пб = {Volume} м³ - геометрический объем пробоотборника",
                    $"PAir = {PressureAir.ToString(PressureUnit.Mpa)} - атмосферное давление",
                    $"TAir = {TemperatureAir.ToString(TemperatureUnit.Kelvin)} - температура воздуха",
                });
            }
            if (Mode == 2)
            {
                qSampling = 0.001*(36710*a*Square*pAbs.Mpa*(PurgeTime/Math.Sqrt(T.Kelvins)) + Qdev*PurgeTime);
                result += qSampling;

                ListingCalculation = string.Join("\n", new List<string>
                {
                    $"Result = 0,001 * {Q/1000} * {MeasCount} * {Time} + {qSampling} = {result} тыс.м³",
                    "",
                    $"qхр = {Q} л/мин / 1000 = {Q/1000} м³/мин - расход анализируемого газа в хроматографе",
                    $"nхр = {MeasCount} - количество измерений",
                    $"τхр = {Time} мин - время проведения анализа",
                    $"",
                    $"Qпр = 0,001 * (36710*{a}*{Square}*{pAbs.Mpa}*({PurgeTime}/КОРЕНЬ({T.Kelvins})) + {Qdev}*{PurgeTime}) = {qSampling} тыс.м³ - расход газа при взятии проб (в соответствии с СТО Газпром 11-2005)",
                    $"A = {a} - коэффициент, зависящий от молекулярной массы газа",
                    $"M = {molMass} г/моль - молекулярная масса газа",
                    $"F = {Square} м² - площадь проходного сечения вентиля арматуры на линии сброса газа в атмосферу",
                    $"P = {P.Mpa} + {PressureAir.Mpa} = {pAbs.ToString(PressureUnit.Mpa)} - абсолютное давление газа перед арматурой на линии сброса в атмосферу",
                    $"T = {T.ToString(TemperatureUnit.Kelvin)} - температура газа перед арматурой на линии сброса в атмосферу",
                    $"τ = {PurgeTime} ч - продолжительность расчетного периода",
                    $"Q = {Qdev} м³/ч - расход газа на прибор",
                    $"PAir = {PressureAir.ToString(PressureUnit.Mpa)} - атмосферное давление",
                });
            }
            //   return 0.001 * Q * MeasCount * Time / 1000;
            return result;
        }
        /// <summary>
        /// Коэффициент, зависящий от молекулярной массы газа
        /// </summary>
        /// <param name="molarMass">Молярная масса газовой смеси, кг/кмоль</param>
        /// <returns>A</returns>
        private double CoeffA(double molarMass)
        {
            return 19783*Math.Pow(molarMass, -0.545);
        }
        
        [Display(Name = "Расход анализируемого газа в хроматографе, л/мин, в соответствии с паспортными данными")]
        public double Q { get; set; }

        [Display(Name = "Время проведения анализа, мин")]
        public double Time { get; set; }

        [Display(Name = "Количество измерений")]
        public int MeasCount { get; set; }
        
        [Display(Name = "Способ отбора проб (1 - Проведение разовых анализов, 2 - Работа приборов на потоке)")]
        public int Mode { get; set; }

        [Display(Name = "Площадь продувочного сечения вентиля, м²")]
        public double Square { get; set; }

        [Display(Name = "Степень открытия вентиля ВИ-160")]
        public double OpeningDegree { get; set; }

        [Display(Name = "Продолжительность продувки, с")]
        public double  PurgeTime { get; set; }

        [Display(Name = "Температура воздуха")]
        public Temperature TemperatureAir { get; set; } = Temperature.FromCelsius(0);

        [Display(Name = "Атмосферное давление")]
        public Pressure PressureAir { get; set; }

        [Display(Name = "Давление газа")]
        public Pressure P { get; set; }

        [Display(Name = "Температура газа")]
        public Temperature T { get; set; } = Temperature.FromCelsius(0);

        [Display(Name = "Геометрический объем пробоотборника, м³")]
        public double Volume { get; set; }

        [Display(Name = "Количество анализов")]
        public int TestCount { get; set; }

        [Display(Name = "Содержание азота в газе, мол.доля %")]
        public double NitrogenContent { get; set; }

        [Display(Name = "Содержание двуокиси углерода СО2 в газе, мол.доля %")]
        public double CarbonDioxideContent { get; set; }

        [Display(Name = "Расход газа на прибор, м³/ч")]
        public double Qdev { get; set; }

        [Display(Name = "Выбран способ отбора пробы 1?")]
        public bool IsModeOne { get; set; }
    }
}