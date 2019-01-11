using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using System.Linq;
using GazRouter.Controls.Dialogs.PipingVolumeCalculator;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.GasCompessibility;
using Utils.Calculations;
using Utils.Units;
using Pressure = Utils.Units.Pressure;

namespace GazRouter.Modes.GasCosts.Dialogs.CleaningCosts
{
    /// <summary>
    /// Расчет расхода газа при очистке внутренней полости участков МГ очистными устройствами
    /// </summary>
    public class CleaningCostsModel : Listing, ICostCalcModel
    {
        public CleaningCostsModel()
        {
            PurgeTime = 1;
            Count = 1;
            
            StartPiping = new List<PipeSection>();
            EndPiping = new List<PipeSection>();
        }

        public double Calculate()
        {
            var pAir = PressureAir;
            var pS = StartChamberPressure + pAir;
            var tS = StartChamberTemperature;
     //       var listingZS = "zS";
       //     var zS = SupportCalculations.GasCompressibilityFactorApproximate(pS, tS, Density, ref listingZS);
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
            var zS = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, pS.Mpa, tS.Kelvins);

            var pE = EndChamberPressure + pAir;
            var zE = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, pE.Mpa, EndChamberTemperature.Kelvins);

            // Расчет расхода газа через продувочную свечу
            //var c = SupportCalculations.VelocityOfSound(pE, EndChamberTemperature, Density, NitrogenContent / 100);
            //var q = Math.PI * Bleeder.Diameter * Bleeder.Diameter * c / 4;
            //var m = SupportCalculations.DynamiсViscosity(pE, EndChamberTemperature, Density, NitrogenContent / 100,
            //    CarbonDioxideContent / 100);
            // var reynoldsNumber = SupportCalculations.ReynoldsNumber(m, Density, Bleeder.Diameter, q);

            //коэффициент расхода
            //var kq = reynoldsNumber == 0 ? 0 : 0.587 + 5.5 / Math.Sqrt(reynoldsNumber) + 0.348 / Math.Pow(reynoldsNumber, 1.0 / 3.0) - 110.92 / reynoldsNumber;

            // показатель адиабаты (ГОСТ 30319.1)
            var listingK = "k";
            var k = SupportCalculations.AdiabaticIndex(pE, EndChamberTemperature, Density, NitrogenContent/100,
                ref listingK);
            
            var qS = 0.0; var qE = 0.0;
            var listingqSt = new List<string>();
            var listingqEnd = new List<string>();
            if (StartChamberVolume > 0 || StartPipingVolume > 0)
                // Объем при запуске очистного устройства
            {
                qS =
                    0.001 * (
                        (StartChamberVolume + StartPipingVolume) * pS.Mpa / tS.Kelvins / zS * StandardConditions.T.Kelvins /
                        StandardConditions.P.Mpa
                        + 3 * (StartChamberVolume + StartPipingVolume));
                var qOP = 0.001*(StartChamberVolume + StartPipingVolume)*pS.Mpa/tS.Kelvins/zS*StandardConditions.T.Kelvins/StandardConditions.P.Mpa;
                var qVV = 0.001*3*(StartChamberVolume + StartPipingVolume);
                qS = qOP + qVV;

                // var q
                listingqSt = new List<string>
                {
                    $"Qзап = {qOP} + {qVV}  = {qS} тыс.м³ - объем газа, расходуемого при запуске очистного устройства",
                    $"Qоп_кз = 0,001*({StartChamberVolume}+{StartPipingVolume})*{pS.Mpa}/{tS.Kelvins}/{zS}*{StandardConditions.T.Kelvins}/{StandardConditions.P.Mpa} = {qOP} тыс.м³ - объем газа, расходуемый на опорожнение КЗ очистного устройства и участка МГ перед КЗ очистного устройства",
                    $"Pкз = {StartChamberPressure.Mpa} + {pAir.Mpa} = {pS.ToString(PressureUnit.Mpa)} - абсолютное давление газа в КЗ до опорожнения",
                    $"Tкз = {tS.ToString(TemperatureUnit.Kelvin)} - температура газа в камере запуска до опорожнения",
                    $"Vкз    = {StartChamberVolume} м³ - геометрический объем КЗ очистного устройства",
                    $"Vкз_уч = {StartPipingVolume} м³ - геометрический объем участка МГ перед КЗ очистного устройства",
                    $"zкз = {zS} - коэффициент сжимаемости при Pкз, Tкз (по ГОСТ 30319.3)",
                    $"Qвв_кз = 0,001*3*({StartChamberVolume}+{StartPipingVolume}) = {qVV} тыс.м³ - объем газа, расходуемый на вытеснение воздуха из камеры запуска очистного устройства и участка МГ от КЗ до секущего крана",
                };
            }

            if (EndChamberVolume > 0 || EndPipingVolume > 0)
            {
                var square = Math.PI * Math.Pow(Bleeder.Diameter, 2) / 4.0;
                var coefL = K.CalcK(pE.Kgh, Bleeder.Diameter, Length);
                var qP = coefL * Math.Pow(2.0 / (k + 1), 1.0 / (k - 1)) * square *
                         Math.Sqrt(2 * k / ((k + 1) * Density.KilogramsPerCubicMeter * EndChamberTemperature.Kelvins * zE) *
                                   StandardConditions.T.Kelvins / StandardConditions.P.Mpa) * pE.Mpa * PurgeTime;
                var qvv = 0.001*3*(EndChamberVolume + EndPipingVolume);
                var qopkp = 0.001*(EndChamberVolume + EndPipingVolume)*pE.Mpa/EndChamberTemperature.Kelvins/zE*
                            StandardConditions.T.Kelvins/StandardConditions.P.Mpa;
                var qopksb = 0.001 * CondensateTankVolume *pE.Mpa/EndChamberTemperature.Kelvins/zE*StandardConditions.T.Kelvins/
                             StandardConditions.P.Mpa;
                // Объем при приеме очистного устройства
                 qE = 0.001*(
                    3*(EndChamberVolume + EndPipingVolume)
                    +
                    (EndChamberVolume + EndPipingVolume)*pE.Mpa/EndChamberTemperature.Kelvins/zE*
                    StandardConditions.T.Kelvins/StandardConditions.P.Mpa
                    +
                    CondensateTankVolume*pE.Mpa/EndChamberTemperature.Kelvins/zE*StandardConditions.T.Kelvins/
                    StandardConditions.P.Mpa)
                     + qP;
               
                listingqEnd = new List<string>
                {
                    $"Qпр = {qvv} + {qopkp} + {qopksb} + {qP} = {qE} тыс.м³ - расход газа при приеме очистного устройства",
                    $"Qвв_кп  = 0,001*3*({EndChamberVolume} + {EndPipingVolume}) = {qvv} тыс.м³ - объем газа, необходимый для вытеснение воздуха из КП очистного устройства и участка МГ перед камерой приема очистного устройства",
                    $"Vкп    = {EndChamberVolume} м³ - геометрический объем КП очистного устройства",
                    $"Vкп_уч = {EndPipingVolume} м³ - геометрический объем участка МГ перед КП очистного устройства",
                    $"Qоп_кп  = 0,001*({EndChamberVolume}+{EndPipingVolume})*{pE.Mpa}/{EndChamberTemperature.Kelvins}/{zE}*{StandardConditions.T.Kelvins}/{StandardConditions.P.Mpa} = {qopkp} тыс.м³ - объем газа, необходимый для опорожнения КП очистного устройства и участка МГ перед КП очистного устройства",
                    $"Pкп = {EndChamberPressure.Mpa} + {pAir.Mpa} = {pE.ToString(PressureUnit.Mpa)} - среднее абсолютное давление на участке перед КП до опорожнения",
                    $"Tкп = {EndChamberTemperature.ToString(TemperatureUnit.Kelvin)} - средняя температура газа на участке перед КП до опорожнения",
                    $"zкп = {zE} - коэффициент сжимаемости газа при Pкп, Tкп (по ГОСТ 30319.3)",
                    $"Qоп_ксб = 0,001*{CondensateTankVolume}*{pE.Mpa}/{EndChamberTemperature.Kelvins}/{zE}*{StandardConditions.T.Kelvins}/{StandardConditions.P.Mpa} = {qopksb} тыс.м³ - объем газа, необходимый для опорожнения конденсатосборника для сброса шлама",
                    $"Vксб = {CondensateTankVolume} м³ - геометрический объем конденсатосборника",
                    $"Qпр_кп = {coefL} * СТЕПЕНЬ(2,0/({k}+1); 1/({k}-1)) * {square} * КОРЕНЬ(2*{k}/(({k}+1)*{Density.KilogramsPerCubicMeter}*{EndChamberTemperature.Kelvins}*{zE})*{StandardConditions.T.Kelvins}/{StandardConditions.P.Mpa}) * {pE.Mpa} * {PurgeTime} = {qP} тыс.м³ - объем газа, необходимый для продувки КП очистного устройства",
                    $"kL = {coefL} - коэффициент, учитывающий влияние длины дренажной линии на скорость продувки",
                    $"S = {Math.PI} * ({Bleeder.Diameter}^2) / 4,0 = {square} м² - площадь поперечного сечения свечи",
                    $"dсв = {Bleeder.Diameter} м - диаметр поперечного сечения свечи",
                    $"",
                    $"{listingK}",
                };
            }
            var result = (qS + qE)*Count;

            #region listing

            ListingCalculation = string.Join("\n",
                string.Join("\n", $"Qоч_лч = ({qS} + {qE}) * {Count} = {result} тыс.м³",
                    $"kоч = {Count} - количество пропусков (циклов) очистного устройства"),
                    $"pAir = {pAir.Mpa} МПа - атмосферное давление",
                    $"TSt = {StandardConditions.T.ToString(TemperatureUnit.Kelvin)} - температура газа при стандартных условиях",
                    $"PSt = {StandardConditions.P.ToString(PressureUnit.Mpa)} - абсолютное давление газа при стандартных условиях \n", 
                $"Объем газа, расходуемый при запуске очистного устройства",
                qS > 0.0 ? string.Join("\n", listingqSt) : string.Join("\n", new List<string> {$"Qзап = {qS}"}),
                $"\nРасход газа при приеме очистного устройства",
                qE > 0.0 ? string.Join("\n", listingqEnd) : string.Join("\n", new List<string> {$"Qпр = {qE}"}));
            //ListingCalculation = qS > 0.0 ? string.Join("\n", listingqSt) : string.Join("\n", new List<string> { $"qS = {qS}" });
            //ListingCalculation = qE > 0.0 ? string.Join("\n", listingqEnd) : string.Join("\n", new List<string> { $"qE = {qE}" });
            #endregion

            return result;
   
        }

        [Display(Name = "Кол-во пропусков(циклов) очистного устройства")]
        public int Count { get; set; }


        [Display(Name = "Геометрический объем камеры запуска очистного устройства (КЗОУ), м³")]
        public double StartChamberVolume { get; set; }

        [Display(Name = "Геометрический объем участка МГ после КЗОУ, м3")]
        public double StartPipingVolume 
        {
            get { return StartPiping.Sum(p => p.Volume); }
        }


        [Display(Name = "Перечень газопроводов после КЗОУ")]
        public List<PipeSection> StartPiping { get; set; }


        [Display(Name = "Геометрический объем камеры приема очистного устройства (КПОУ), м³")]
        public double EndChamberVolume { get; set; }

        [Display(Name = "Геометрический объем участка МГ перед КПОУ, м3")]
        public double EndPipingVolume
        {
            get { return EndPiping.Sum(p => p.Volume); }
        }

        [Display(Name = "Перечень газопроводов перед КПОУ")]
        public List<PipeSection> EndPiping { get; set; }

        

        [Display(Name = "Давление газа в КЗОУ до опорожнения, кг/см²")]
        public Pressure StartChamberPressure { get; set; }

        [Display(Name = "Температура газа в КЗОУ до опорожнения, Гр.С")]
        public Temperature StartChamberTemperature { get; set; } = Temperature.FromCelsius(0);
        
        [Display(Name = "Давление газа в КПОУ до опорожнения, кг/см²")]
        public Pressure EndChamberPressure { get; set; } 

        [Display(Name = "Температура газа в КПОУ до опорожнения, Гр.С")]
        public Temperature EndChamberTemperature { get; set; } = Temperature.FromCelsius(0);


        [Display(Name = "Геометрический объем конденсатосборника, м³")]
        public double CondensateTankVolume { get; set; }


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

        [Display(Name = "Давление атмосферное, мм рт.ст.")]
        public Pressure PressureAir { get; set; }
        
        [Display(Name = "Длина дренажной линии, м")]
        public double Length { get; set; }
    
    }

}