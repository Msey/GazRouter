using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using System.Linq;
using GazRouter.Modes.GasCosts.GasCompessibility;
using GazRouter.Controls.Dialogs.PipingVolumeCalculator;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using Utils.Calculations;
using Utils.Units;
namespace GazRouter.Modes.GasCosts.Dialogs.DiaphragmReplacementCosts
{
    public class DiaphragmReplacementCostsModel : Listing, ICostCalcModel
    {
        public DiaphragmReplacementCostsModel()
        {
            Piping = new List<PipeSection>();
        }
        public double Calculate()
        {
            var pAir = PressureAir;          
            var pIn = PressureIn + pAir;
            var pOut = PressureOut + pAir;
            var pAverage = Pressure.FromMpa(2.0/3.0*(pIn.Mpa + pOut.Mpa*pOut.Mpa/(pIn.Mpa + pOut.Mpa)));
            var tAverage = (TemperatureIn+TemperatureOut) / 2.0;
          //  var listZ = "z";
          //  var z = SupportCalculations.GasCompressibilityFactorApproximate(pAverage, tAverage, Density, ref listZ);
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
            var z = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, pAverage.Mpa, tAverage.Kelvins);
            var result = 0.001 * (3 * PipingVolume + PipingVolume * pAverage.Mpa / tAverage.Kelvins / z * StandardConditions.T.Kelvins / StandardConditions.P.Mpa);
#region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Qси = 0,001 * (3 * {PipingVolume} + {PipingVolume} * {pAverage.Mpa} / {tAverage.Kelvins} / {z} * {StandardConditions.T.Kelvins} / {StandardConditions.P.Mpa}) = {result} тыс.м³",
                "",
                $"Vуч = {PipingVolume} м³ - геометрический объем участка трубопровода",
                $"Pучср = 2/3*({pIn.Mpa} + {pOut.Mpa}*{pOut.Mpa}/({pIn.Mpa} + {pOut.Mpa})) = {pAverage.ToString(PressureUnit.Mpa)} - среднее абсолютное давление газа на участке трубопровода до опорожнения",
                $"Pн = {PressureIn.Mpa} + {pAir.Mpa} = {pIn.ToString(PressureUnit.Mpa)} - абсолютное давление газа в начале участка трубопровода до опорожнения",
                $"Pк = {PressureOut.Mpa} + {pAir.Mpa} = {pOut.ToString(PressureUnit.Mpa)} - абсолютное давление газа в конце участка трубопровода до опорожнения",
                $"Pair = {PressureAir.ToString(PressureUnit.Mpa)} - атмосферное давление",
                $"Tучср = ({TemperatureIn.Kelvins} + {TemperatureOut.Kelvins}) / 2 = {tAverage.ToString(TemperatureUnit.Kelvin)} - средняя температура газа на участке трубопровода до опорожнения",
                $"tн = {TemperatureIn.ToString(TemperatureUnit.Kelvin)} - температура газа в начале участка трубопровода до опорожнения",
                $"tк = {TemperatureOut.ToString(TemperatureUnit.Kelvin)} - температура газа в конце участка трубопровода до опорожнения",
                $"z = {z} - коэффициент сжимаемости (по ГОСТ 30319.3)",
                $"TSt = {StandardConditions.T.Kelvins} - температура газа при стандартных условиях",
                $"PSt = {StandardConditions.P.Mpa} - абсолютное давление газа при стандартных условиях",
            });
#endregion
            return result;
        }
        [Display(Name = "Давление газа в начале участка трубопровода до опорожнения, кг/см²")]
        public Pressure PressureIn { get; set; }
        [Display(Name = "Давление газа в конце участка трубопровода до опорожнения, кг/см²")]
        public Pressure PressureOut { get; set; }
        [Display(Name = "Температура газа в начале участка трубопровода до опорожнения, Гр.С")]
        public Temperature TemperatureIn { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Температура газа в конце участка трубопровода до опорожнения, Гр.С")]
        public Temperature TemperatureOut { get; set; } = Temperature.FromCelsius(0);
        //[Display(Name = "Плотность газа, кг/м³")]
        //public Density Density { get; set; }
        [Display(Name = "Давление атмосферное, мм рт.ст.")]
        public Pressure PressureAir { get; set; }
        [Display(Name = "Содержание азота в газе, мол.доля %")]
        public double NitrogenContent { get; set; }
        [Display(Name = "Содержание двуокиси углерода СО2 в газе, мол.доля %")]
        public double CarbonDioxideContent { get; set; }
        public double PipingVolume
        {
            get { return Piping.Sum(p => p.Volume); }
        }
        [Display(Name = "Перечень газопроводов")]
        public List<PipeSection> Piping { get; set; }
    }
}