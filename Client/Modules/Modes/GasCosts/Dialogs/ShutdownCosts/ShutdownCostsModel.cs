using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.Modes.GasCosts.GasCompessibility;
using System.Linq;
using GazRouter.Controls.Dialogs.PipingVolumeCalculator;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using Utils.Calculations;
using Utils.Units;
namespace GazRouter.Modes.GasCosts.Dialogs.ShutdownCosts
{
    public class ShutdownCostsModel : Listing, ICostCalcModel
    {
        public ShutdownCostsModel()
        {
            PressureFinalIn = Pressure.Zero;
            PressureFinalOut = Pressure.Zero;
            StopCount = 1;
            PipingIn = new List<PipeSection>();
            PipingOut = new List<PipeSection>();
        }
        public double Calculate()
        {
            var pAir           = PressureAir;
            var pAbsInitialIn  = PressureInitialIn + pAir;
            var pAbsInitialOut = PressureInitialOut + pAir;
            var pAbsFinalIn    = PressureFinalIn + pAir;
            var pAbsFinalOut   = PressureFinalOut + pAir;
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
            //
            var zInitialIn  = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, pAbsInitialIn.Mpa, TemperatureInitialIn.Kelvins);
            var zInitialOut = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, pAbsInitialOut.Mpa, TemperatureInitialOut.Kelvins);
            var zFinalIn    = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, pAbsFinalIn.Mpa, TemperatureFinalIn.Kelvins);
            var zFinalOut   = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, pAbsFinalOut.Mpa, TemperatureFinalOut.Kelvins);

            // Расчет расхода газа на удаление газовоздушной смеси
            double q;
            if ((PressureFinalIn == Pressure.Zero) && (PressureFinalOut == Pressure.Zero)) // полное стравливание
            {
                var qPr = 0.001 * 3 * (PipingVolumeIn + PipingVolumeOut);
                var qOp = 0.001 * (PipingVolumeIn * pAbsInitialIn.Mpa / TemperatureInitialIn.Kelvins / zInitialIn +
                           PipingVolumeOut * pAbsInitialOut.Mpa / TemperatureInitialOut.Kelvins / zInitialOut) *
                    StandardConditions.T.Kelvins / StandardConditions.P.Mpa;
                q = qPr + qOp;
                //
                ListingCalculation = string.Join("\n", new List<string>
                {
                    $"Qрр = { qOp } + { qPr } = { q } тыс.м³",
                    "",
                    $"Qоп = 0,001 * ({PipingVolumeIn} * {pAbsInitialIn.Mpa} / {TemperatureInitialIn.Kelvins} / {zInitialIn} + {PipingVolumeOut} * {pAbsInitialOut.Mpa} / {TemperatureInitialOut.Kelvins} / {zInitialOut})*{StandardConditions.T.Kelvins} / {StandardConditions.P.Mpa} = {qOp} тыс.м³ - объем газа при полном стравливании газа из коммуникаций КЦ",
                    $"Qпр = 0,001 * 3 * ({PipingVolumeIn} + {PipingVolumeOut}) = {qPr} тыс.м³ - расход газа при продувке коммуникаций КЦ",
                    $"Vвх  = {PipingVolumeIn} м³  - геометрический объем входных коммуникаций КЦ",
                    $"Vвых = {PipingVolumeOut} м³ - геометрический объем выходных коммуникаций КЦ",
                    $"P1вх  = {PressureInitialIn.Mpa} + {pAir.Mpa} = {pAbsInitialIn.ToString(PressureUnit.Mpa)} - абсолютное давление во входных коммуникациях цеха до стравливания",
                    $"P1вых = {PressureInitialOut.Mpa} + {pAir.Mpa} = {pAbsInitialOut.ToString(PressureUnit.Mpa)} - абсолютное давление в выходных коммуникациях цеха до стравливания",
                    $"PAir  = {PressureAir.ToString(PressureUnit.Mpa)} - атмосферное давление",
                    $"T1вх  = {TemperatureInitialIn.ToString(TemperatureUnit.Kelvin)}  - абсолютная температура газа во входных коммуникациях цеха до стравливания",
                    $"T1вых = {TemperatureInitialOut.ToString(TemperatureUnit.Kelvin)} - абсолютная температура газа в выходных коммуникациях цеха до стравливания",
                    $"TSt   = {StandardConditions.T.ToString(TemperatureUnit.Kelvin)} - абсолютная температура газа при стандартных условиях",
                    $"PSt   = {StandardConditions.P.ToString(PressureUnit.Mpa)} - абсолютное давление газа при стандартных условиях",
                    $"z1вх = {zInitialIn} - коэффициент сжимаемости газа во входных коммуникациях цеха до стравливания (по ГОСТ 30319.3)",
                    $"z1вых = {zInitialOut} - коэффициент сжимаемости газа в выходных коммуникациях цеха до стравливания (по ГОСТ 30319.3)",
                });
            }
            else // частичное стравливание
            {
                q = 0.001*
                    (PipingVolumeIn*
                     (pAbsInitialIn.Mpa/TemperatureInitialIn.Kelvins/zInitialIn -
                      pAbsFinalIn.Mpa/TemperatureFinalIn.Kelvins/zFinalIn) +
                     PipingVolumeOut*
                     (pAbsInitialOut.Mpa/TemperatureInitialOut.Kelvins/zInitialOut -
                      pAbsFinalOut.Mpa/TemperatureFinalOut.Kelvins/zFinalOut))*StandardConditions.T.Kelvins/
                    StandardConditions.P.Mpa;
                ListingCalculation = string.Join("\n", new List<string>
                {
                    $"Qрр = 0,001 * ({PipingVolumeIn} * ({pAbsInitialIn.Mpa} / {TemperatureInitialIn.Kelvins} / {zInitialIn} - {pAbsFinalIn.Mpa} / {TemperatureFinalIn.Kelvins} / {zFinalIn}) + {PipingVolumeOut} * ({pAbsInitialOut.Mpa} / {TemperatureInitialOut.Kelvins} / {zInitialOut} - {pAbsFinalOut.Mpa} / {TemperatureFinalOut.Kelvins} / {zFinalOut})) * {StandardConditions.T.Kelvins} / {StandardConditions.P.Mpa} = {q} тыс.м³",
                    "",
                    $"Vвх   = {PipingVolumeIn} м³  - геометрический объем входных коммуникаций КЦ",
                    $"Vвых  = {PipingVolumeOut} м³ - геометрический объем выходных коммуникаций КЦ",
                    $"P1вх  = {PressureInitialIn.Mpa} + {pAir.Mpa} = {pAbsInitialIn.ToString(PressureUnit.Mpa)}    - абсолютное давление во входных коммуникациях цеха до стравливания",
                    $"P1вых = {PressureInitialOut.Mpa} + {pAir.Mpa} = {pAbsInitialOut.ToString(PressureUnit.Mpa)}  - абсолютное давление в выходных коммуникациях цеха до стравливания",
                    $"P2вх  = {PressureFinalIn.Mpa} + {pAir.Mpa} = {pAbsFinalIn.ToString(PressureUnit.Mpa)}        - абсолютное давление во входных коммуникациях цеха после стравливания",
                    $"P2вых = {PressureFinalOut.Mpa} + {pAir.Mpa} = {pAbsFinalOut.ToString(PressureUnit.Mpa)}      - абсолютное давление в выходных коммуникациях цеха после стравливания",
                    $"PAir  = {PressureAir.ToString(PressureUnit.Mpa)} - атмосферное давление",
                    $"T1вх  = {TemperatureInitialIn.ToString(TemperatureUnit.Kelvin)}  - абсолютная температура газа во входных коммуникациях цеха до стравливания",
                    $"T1вых = {TemperatureInitialOut.ToString(TemperatureUnit.Kelvin)} - абсолютная температура газа в выходных коммуникациях цеха до стравливания",
                    $"T2вх  = {TemperatureFinalIn.ToString(TemperatureUnit.Kelvin)}    - абсолютная температура газа во входных коммуникациях цеха после стравливания",
                    $"T2вых = {TemperatureFinalOut.ToString(TemperatureUnit.Kelvin)}   - абсолютная температура газа в выходных коммуникациях цеха после стравливания",
                    $"TSt   = {StandardConditions.T.ToString(TemperatureUnit.Kelvin)} - абсолютная температура газа при стандартных условиях",
                    $"PSt   = {StandardConditions.P.ToString(PressureUnit.Mpa)}       - абсолютное давление газа при стандартных условиях",
                    $"z1вх  = {zInitialIn}  - коэффициент сжимаемости газа во входных коммуникациях цеха до стравливания (по ГОСТ 30319.3)",
                    $"z1вых = {zInitialOut} - коэффициент сжимаемости газа в выходных коммуникациях цеха до стравливания (по ГОСТ 30319.3)",
                    $"z2вх  = {zFinalIn}    - коэффициент сжимаемости газа во входных коммуникациях цеха после стравливания (по ГОСТ 30319.3)",
                    $"z2вых = {zFinalOut}   - коэффициент сжимаемости газа в выходных коммуникациях цеха после стравливания (по ГОСТ 30319.3)",
                });
            }

            return q; 
        }
        [Display(Name = "Давление атмосферное мм рт.ст.")]
        public Pressure PressureAir { get; set; }
        //[Display(Name = "Плотность газа, кг/м³")]
        //public Density Density { get; set; }
        [Display(Name = "Количество остановов КЦ")]
        public int StopCount { get; set; }
        [Display(Name = "Перечень газопроводов цеха")]
        public List<PipeSection> PipingIn { get; set; }
        public List<PipeSection> PipingOut { get; set; }
        [Display(Name = "Геометрический объем входных коммуникаций КЦ")]
        public double PipingVolumeIn { get; set; }
//        public double PipingVolumeIn
//        {
//            get { return PipingIn.Sum(p => p.Volume); }
//        }
        [Display(Name = "Геометрический объем входных коммуникаций КЦ")]
        public double PipingVolumeOut { get; set; }
        //{
        //    get { return PipingOut.Sum(p => p.Volume); }
        //}
        [Display(Name = "Давление газа в начале участка до отключения, кг/см²")]
        public Pressure PressureInitialIn { get; set; }
        [Display(Name = "Давление газа в конце участка до отключения, кг/см²")]
        public Pressure PressureInitialOut { get; set; }
        [Display(Name = "Температура газа в начале участка до отключения, Гр.С")]
        public Temperature TemperatureInitialIn { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Температура газа в конце участка до отключения, Гр.С")]
        public Temperature TemperatureInitialOut { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Давление газа в начале участка после стравливания, кг/см²")]
        public Pressure PressureFinalIn { get; set; }
        [Display(Name = "Давление газа в конце участка после стравливания, кг/см²")]
        public Pressure PressureFinalOut { get; set; }
        [Display(Name = "Температура газа в начале участка после стравливания, Гр.С")]
        public Temperature TemperatureFinalIn { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Температура газа в конце участка после стравливания, Гр.С")]
        public Temperature TemperatureFinalOut { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Содержание азота в газе, мол.доля %")]
        public double NitrogenContent { get; set; }
        [Display(Name = "Содержание двуокиси углерода СО2 в газе, мол.доля %")]
        public double CarbonDioxideContent { get; set; }
    }
}