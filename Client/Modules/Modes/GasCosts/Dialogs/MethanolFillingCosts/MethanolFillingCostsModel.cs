using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.GasCompessibility;
using Utils.Calculations;
using Utils.Units;

namespace GazRouter.Modes.GasCosts.Dialogs.MethanolFillingCosts
{
    /// <summary>
    /// Расчет нормативного расхода газа при заправке метанольных и одоризационных установок
    /// </summary>
    public class MethanolFillingCostsModel : Listing, ICostCalcModel
    {
        public double Calculate()
        {
            var pressure = Pressure + PressureAir;
            var listingZ = "z";
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
            var z = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, pressure.Mpa, Temperature.Kelvins);
            var result = 0.001*StandardConditions.T.Kelvins/StandardConditions.P.Mpa*Volume*pressure.Mpa/
                         Temperature.Kelvins/z*Count;

            #region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Qмет = 0,001 * {StandardConditions.T.Kelvins} / {StandardConditions.P.Mpa} * {Volume} * {pressure.Mpa} / {Temperature.Kelvins} / {z} * {Count} = {result} тыс.м³",
                "",
                $"nмет = {Count} - количество метанольных устройств",
                $"Vм = {Volume} м³ - геометрический объем метанольного устройства",
                $"Pм = {Pressure.Mpa} + {PressureAir.Mpa} =  {pressure.ToString(PressureUnit.Mpa)} - абсолютное давление в метанольном устройстве",
                $"Tм = {Temperature.ToString(TemperatureUnit.Kelvin)} - температура газа в метанольном устройстве",
                $"PAir = {PressureAir.ToString(PressureUnit.Mpa)} - атмосферное давление",
                $"z = {z} - коэффициент сжимаемости при Pм, Tм (по ГОСТ 30319.3)",
                $"TSt = {StandardConditions.T.ToString(TemperatureUnit.Kelvin)} - температура газа при стандартных условиях",
                $"PSt = {StandardConditions.P.ToString(PressureUnit.Mpa)} - абсолютное давление газа при стандартных условиях",
            });
            #endregion

            return result;
        }

        [Display(Name = "Давление газа в устройстве, кг/см²")]
        public Pressure Pressure { get; set; }

        [Display(Name = "Температура газа в устройстве, Гр.С")]
        public Temperature Temperature  { get; set; } = Temperature.FromCelsius(0);

        //[Display(Name = "Плотность газа, кг/м³")]
        //public Density Density { get; set; }

        [Display(Name = "Давление атмосферное, мм рт.ст.")]
        public Pressure PressureAir { get; set; }

        [Display(Name = "Содержание азота в газе, мол.доля %")]
        public double NitrogenContent { get; set; }

        [Display(Name = "Содержание двуокиси углерода СО2 в газе, мол.доля %")]
        public double CarbonDioxideContent { get; set; }

        [Display(Name = "Геометрический объем устройства, м³")]
        public double Volume { get; set; }
        
        [Display(Name = "Кол-во заправок устройства")]
        public int Count { get; set; }
        
    
    }

}