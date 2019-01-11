using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using Utils.Calculations;
using Utils.Units;
namespace GazRouter.Modes.GasCosts.Dialogs.HeaterWorkCosts
{
    public class HeaterWorkCostsModel : Listing, ICostCalcModel
    {
        public double Calculate()
        {
            // Расчет по СТО Газпром 3.3-2-024-2011 (не действующий сейчас)
            //var c = SupportCalculations.HeatCapacity2(
            //    (PIn + PressureAir).Mpa,
            //    TIn.Kelvins);
            //var ret = Density.KilogramsPerCubicMeter*Q/1000*c*(TOut.Kelvins - TIn.Kelvins)*Period/HeaterEfficiency/
            //          CombustionHeat;
            var listing = "Cp";
            ///////////////////////////////////////////////////
            // Расчет по новому СТО Газпром 3-Х-Х-2016
            var pInAbs = PIn + PressureAir;
            var c = SupportCalculations.HeatCapacity3(pInAbs.Mpa,
                                                       TIn.Kelvins,
                                                       NitrogenContent / 100.0,
                                                       CarbonDioxideContent / 100.0,
                                                       Density, ref listing);

            var cHeat = CombustionHeat.KCal * 4.184; // перевод в кДж/м3 из ккал/м3
            if (TOutIsKnown)
            {
                var ret = Density.KilogramsPerCubicMeter * Q * c * (TOut.Kelvins - TIn.Kelvins) / (HeaterEfficiency * cHeat);
                // 
                ListingCalculation = string.Join("\n", new List<string>
                    {
                        $"Qпг = {Density.KilogramsPerCubicMeter}*{Q}*{c}*({TOut.Kelvins} - {TIn.Kelvins})/({HeaterEfficiency}*{cHeat}) = {ret} тыс.м³",
                        $"",
                        $"ρ = {Density.KilogramsPerCubicMeter} кг/м³ - плотность газа",
                        $"Qгрс = {Q} тыс.м³ - объем газа, проходящий через ГРС потребителям за расчетный период",
                        $"T1 = {TIn.ToString(TemperatureUnit.Kelvin)} - температура газа на входе ГРС",
                        $"Tпг = {TOut.ToString(TemperatureUnit.Kelvin)} - температура газа на выходе из подогревателя газа",
                        $"ƞпгₒ = {HeaterEfficiency} - номинальный КПД подогревателя газа",
                        $"Qⁿp = {CombustionHeat} ккал/м³ * 4,184  = {cHeat} кДж/м³ - низшая теплота сгорания газа",
                        $"",
                        $"{listing}",
                    });

                return ret;
            }

            var listingCoef = "Di";
            var pOutAbs = POut + PressureAir;
            var di = SupportCalculations.JouleThomsonCoefficient(PIn.Mpa, TIn.Kelvins, NitrogenContent,
                CarbonDioxideContent, Density, ref listingCoef);
            var result = Density.KilogramsPerCubicMeter * Q * c * (TOut.Kelvins - TIn.Kelvins + di*(pInAbs.Mpa- pOutAbs.Mpa)) / (HeaterEfficiency * cHeat);
            ListingCalculation = string.Join("\n", new List<string>
                    {
                        $"Qпг = {Density.KilogramsPerCubicMeter}*{Q}*{c}*({TOut.Kelvins} - {TIn.Kelvins} + {di} * ({pInAbs.Mpa} - {pOutAbs.Mpa}))/({HeaterEfficiency}*{cHeat}) = {result} тыс.м³",
                        $"",
                        $"ρ = {Density.KilogramsPerCubicMeter} кг/м³ - плотность газа",
                        $"Qгрс = {Q} тыс.м³ - объем газа, проходящий через ГРС потребителям за расчетный период",
                        $"T1 = {TIn.ToString(TemperatureUnit.Kelvin)} - температура газа на входе ГРС",
                        $"T2 = {TOut.ToString(TemperatureUnit.Kelvin)} - температура газа на выходе ГРС после регуляторов давления",
                        $"P1 = {PIn.Mpa} + {PressureAir.Mpa} = {pInAbs.ToString(PressureUnit.Mpa)} - абсолютное давление газа на входе ГРС",
                        $"P2 = {POut.Mpa} + {PressureAir.Mpa} = {pOutAbs.ToString(PressureUnit.Mpa)} - абсолютное давление газа на выходе ГРС после регулятора давления",
                        $"PAir = {PressureAir.ToString(PressureUnit.Mpa)} - атмосферное давление",
                        $"ƞпгₒ = {HeaterEfficiency} - номинальный КПД подогревателя газа",
                        $"Qⁿp = {CombustionHeat} ккал/м³ * 4,184  = {cHeat} кДж/м³ - низшая теплота сгорания газа",
                        $"",
                        $"{listing}",
                        $"",
                        $"{listingCoef}",
                    });

            return result;
        }

        [Display(Name = "Температура газа на выходе из подогревателя известна")]
        public bool TOutIsKnown { get; set; }
        [Display(Name = "Тип подогревателя газа")]
        public HeaterType HeaterType { get; set; }
        [Display(Name = "Номинальный КПД")]
        public double HeaterEfficiency { get; set; }
        [Display(Name = "Плотность газа, кг/м³")]
        public Density Density { get; set; }
        [Display(Name = "Низшая теплота сгорания газа, ккал/м³")]
        public CombustionHeat CombustionHeat { get; set; }
        [Display(Name = "Объем газа, проходящий через ГРС потребителям, тыс. м³")]
        public double Q { get; set; }
        [Display(Name = "Давление газа на входе в ГРС")]
        public Pressure PIn { get; set; }
        [Display(Name = "Давление газа на выходе ГРС после регулятора давления")]
        public Pressure POut { get; set; }
        [Display(Name = "Температура газа на входе в ГРС, °С")]
        public Temperature TIn { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Температура газа на выходе из ПГ, °С")]
        public Temperature TOut { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Время работы ПТПГ, ч")]
        
        public int Period { get; set; }
        [Display(Name = "Давление атмосферное, мм рт.ст.")]
        public Pressure PressureAir { get; set; }
        [Display(Name = "Содержание азота в газе, мол.доля %")]
        public double NitrogenContent { get; set; }
        [Display(Name = "Содержание двуокиси углерода СО2 в газе, мол.доля %")]
        public double CarbonDioxideContent { get; set; }
    }
}