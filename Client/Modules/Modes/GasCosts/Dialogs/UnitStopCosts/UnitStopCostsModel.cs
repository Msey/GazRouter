using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using System.Linq;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.GasCompessibility;
using Utils.Calculations;
using Utils.Units;
using Pressure = Utils.Units.Pressure;
namespace GazRouter.Modes.GasCosts.Dialogs.UnitStopCosts
{
    public class UnitStopCostsModel : Listing, ICostCalcModel
    {
#region Constructors
        public UnitStopCostsModel()
        {
            ValveShiftings = new List<ValveShifting>();
            Unit = new CompUnit();
        }
#endregion
        #region Public Properties
        [Display(Name = "ГПА")]
        public CompUnit Unit { get; set; }
        [Display(Name = "Плотность газа, кг/м³")]
        public Density Density { get; set; }
        [Display(Name = "Давление атмосферное, мм рт.ст.")]
        public Pressure PressureAir { get; set; }
        [Display(Name = "Давление газа на входе нагнетателя, кг/см²")]
        public Pressure PressureInlet { get; set; }
        [Display(Name = "Давление газа на выходе нагнетателя, кг/см²")]
        public Pressure PressureOutlet { get; set; }
        [Display(Name = "Количество остановов ГПА")]
        public int StopCount { get; set; }
        [Display(Name = "Температура газа на входе нагнетателя, °С")]
        public Temperature TemperatureInlet { get; set; }
        [Display(Name = "Температура газа на выходе нагнетателя, °С")]
        public Temperature TemperatureOutlet { get; set; }
        [Display(Name = "Список переключений кранов (по типам)")]
        public List<ValveShifting> ValveShiftings { get; set; }
        [Display(Name = "Переключения кранов выполняются в соотв. с алгоритмом")]
        public bool NormalShifting { get; set; }
        [Display(Name = "Содержание азота в газе, мол.доля %")]
        public double NitrogenContent { get; set; }
        [Display(Name = "Содержание двуокиси углерода СО2 в газе, мол.доля %")]
        public double CarbonDioxideContent { get; set; }
        #endregion
        #region Public Methods and Operators
        public double Calculate()
        {
            var pair = PressureAir;
            var pInAbs = PressureInlet + pair;
            var pOutAbs = PressureOutlet + pair;
            var avgT = (TemperatureInlet + TemperatureOutlet) / 2.0;
            var avgP = (pInAbs + pOutAbs) / 2.0;
            //var listingAvgz = "averageZ";
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
            var z = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, avgP.Mpa, avgT.Kelvins);
           // var avgZ = SupportCalculations.GasCompressibilityFactorApproximate(avgP, avgT, Density, ref listingAvgz);
            var evQ = Unit.InjectionProfileVolume * avgP.Mpa
                         * StandardConditions.T.Kelvins / avgT.Kelvins / z / StandardConditions.P.Mpa;
            var shifting = NormalShifting ? Unit.StopValveConsumption : ValveShiftings.Sum(vt => vt.Q);
            var result = 0.001 * (evQ + shifting) * StopCount;
#region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Qост = 0,001 * ({evQ} + {shifting}) * {StopCount} = {result} тыс.м³",
                "",
                $"Qоп = {Unit.InjectionProfileVolume} * {avgP.Mpa} / {avgT.Kelvins} / {z} * {StandardConditions.T.Kelvins} / {StandardConditions.P.Mpa} = {evQ} м³ - технологические потери газа при стравливании (опорожнении) контура ЦБК и технологических коммуникаций",
                $"Vцбк = {Unit.InjectionProfileVolume} м³ - геометрический объем контура нагнетателя",
                $"Pцбкср = ({PressureInlet.Mpa} + {PressureOutlet.Mpa}) / 2  = {avgP.ToString(PressureUnit.Mpa)} - среднее абсолютное давление в ЦБК до опорожнения",
                $"Pн = {PressureInlet.Mpa} + {PressureAir.Mpa} = {pInAbs.ToString(PressureUnit.Mpa)} - абсолютное давление газа на входе ЦБК",
                $"Pк = {PressureOutlet.Mpa} + {PressureAir.Mpa} = {pOutAbs.ToString(PressureUnit.Mpa)} - абсолютное давление газа на выходе ЦБК",
                $"Pair = {PressureAir.ToString(PressureUnit.Mpa)} - атмосферное давление",
                $"Tцбкср =  ({TemperatureInlet.ToString(TemperatureUnit.Kelvin)} + {TemperatureOutlet.ToString(TemperatureUnit.Kelvin)}) / 2 = {avgT.ToString(TemperatureUnit.Kelvin)} - средняя температура газа в ЦБК до опорожнения",
                $"tн = {TemperatureInlet.ToString(TemperatureUnit.Kelvin)} - температура газа на входе ЦБК",
                $"tк = {TemperatureOutlet.ToString(TemperatureUnit.Kelvin)} - температура газа на выходе ЦБК",
                $"PSt = {StandardConditions.P.ToString(PressureUnit.Mpa)} - абсолютное давление газа при стандартных условиях",
                $"TSt = {StandardConditions.T.ToString(TemperatureUnit.Kelvin)} - температура газа при стандартных условиях",
                $"z = {z} - коэффициент сжимаемости (по ГОСТ 30319.3)",
                NormalShifting ? $"Qзра = {Unit.StopValveConsumption} м³ - технологические потери газа на перестановку ЗРА" : $"{ValveShiftingsListing(ValveShiftings)}",
                $"kост = {StopCount} - количество остановок ГПА",
            });
#endregion
            return result;
        }
#endregion
    }
}