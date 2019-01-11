using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using Utils.Calculations;
using Utils.Units;
namespace GazRouter.Modes.GasCosts.Dialogs.FuelGasHeatingCosts
{
    public class FuelGasHeatingCostModel : Listing, ICostCalcModel
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
            var c = SupportCalculations.HeatCapacity3((PIn + PressureAir).Mpa, 
                                                       TIn.Kelvins, 
                                                       NitrogenContent/100.0, 
                                                       CarbonDioxideContent/100.0, 
                                                       Density, ref listing);
            var cHeat = CombustionHeat.KCal*4.184; // перевод в кДж/м3 из ккал/м3
            var ret = Density.KilogramsPerCubicMeter*Q*c*(TOut.Kelvins - TIn.Kelvins)/(HeaterEfficiency*cHeat);
            // 
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Qптпг = {Density.KilogramsPerCubicMeter}*{Q}*{c}*({TOut.Kelvins} - {TIn.Kelvins})/({HeaterEfficiency}*{cHeat}) = {ret} тыс.м³",
                $"",
                $"ρc = {Density.KilogramsPerCubicMeter} кг/м³ - плотность газа",
                $"Qтг = {Q} тыс.м³ - объем подогреваемого топливного газа КЦ",
                $"T1птпг = {TIn.ToString(TemperatureUnit.Kelvin)} - температура газа на входе в ПТПГ",
                $"T2птпг = {TOut.ToString(TemperatureUnit.Kelvin)} - температура газа на выходе из ПТПГ",
                $"ηпгₒ = {HeaterEfficiency} - номинальный КПД подогревателя газа",
                $"Qᵸp = {CombustionHeat} ккал/м³ * 4,184  = {cHeat} кДж/м³ - низшая теплота сгорания газа",
                $"PAir = {PressureAir.ToString(PressureUnit.Mpa)} - атмосферное давление",
                $"",
                $"{listing}",
            });
            //
            return ret;
        }
        [Display(Name = "Тип подогревателя газа")]
        public HeaterType HeaterType { get; set; }
        [Display(Name = "Номинальный КПД")]
        public double HeaterEfficiency { get; set; }
        [Display(Name = "Плотность газа, кг/м³")]
        public Density Density { get; set; }
        [Display(Name = "Низшая теплота сгорания газа, ккал/м³")]
        public CombustionHeat CombustionHeat { get; set; }
        //[Display(Name = "Расход подогреваемого топливного газа, м³/ч")]
        //public double Q { get; set; }
        [Display(Name = "Объем подогреваемого топливного газа, тыс. м³")]
        public double Q { get; set; }
        [Display(Name = "Давление газа на входе в ПТПГ")]
        public Pressure PIn { get; set; }
        [Display(Name = "Температура газа на входе в ПТПГ, °С")]
        public Temperature TIn { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Норматив температуры газа на выходе из ПТПГ, °С")]
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