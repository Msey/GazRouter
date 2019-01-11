using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using Utils.Units;
namespace GazRouter.Modes.GasCosts.Dialogs.CompUnitsTestingCosts
{
    public class CompUnitsTestingCostsModel : Listing, ICostCalcModel
    {
        public double Calculate()
        {
            var result = 0.001* Qtg0 * (0.75* Ne/Ne0 + 0.25*Math.Sqrt(Ta.Kelvins / 288) * Pa.Mpa/0.1013)*7000 / Qnp.KCal*Ktg*Tpc;
#region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                 $"Qop = 0,001 * {Qtg0} * (0,75 * {Ne}/{Ne0} + 0,25*({Ta.Kelvins}/288)^0,5 * {Pa.Mpa}/0,1013)*7000 / {Qnp}*{Ktg}*{Tpc} = {result} тыс.м³",
                 "",
                 $"qтгₒ = {Qtg0} тыс. м³/ч - номинальный расход топливного газа ГПА",
                 $"Ne   = {Ne} кВт - мощность ГПА на режиме опробования на работоспособность",
                 $"Neₒ  = {Ne0} кВт - номинальная мощность ГПА",
                 $"Ta   = {Ta.Kelvins} К - температура воздуха на входе в ГТУ",
                 $"Taₒ  = {288} К - номинальная температура воздуха на входе в ГТУ",
                 $"Pa   = {Pa.Mpa} МПа - атмосферное давление воздуха",
                 $"Qᵸут = {7000} ккал/кг у.т. - низшая теплота сгорания условного топлива",
                 $"Qᵸp  = {Qnp} ккал/м³ - низшая теплота сгорания газа",
                 $"Kтг  = {Ktg} - коэффициент техсостояния ГТУ по топливу",
                 $"τраб  = {Tpc} ч - время работы агрегата на режиме опробывания на работоспособность",
            });
#endregion
            return result;
        }

        [Display(Name = "Номинальный расход топливного газа ГПА, тыс. м³/ч")]
        public double Qtg0 { get; set; }

        [Display(Name = "Мощность ГПА на режиме опробывания на работоспособность, кВт")]
        public double Ne { get; set; }

        [Display(Name = "Номинальная мощность ГПА, кВт")]
        public double Ne0 { get; set; }

        [Display(Name = "Температура воздуха на входе в ГТУ, °C")]
        public Temperature Ta { get; set; } = Temperature.FromCelsius(0);

        [Display(Name = "Коэффициент техсостояния ГТУ по топливу")]
        public double Ktg { get; set; }

        [Display(Name = "Атмосферное давление воздуха, мм рт.ст.")]
        public Pressure Pa { get; set; }

        [Display(Name = "Низшая теплота сгорания газа, ккал/м³")]
        public CombustionHeat Qnp { get; set; }

        [Display(Name = "Время работы агрегата на режиме опробывания на работоспособность, ч")]
        public double Tpc { get; set; }
    }
}
