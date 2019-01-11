using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GazRouter.DTO.Dictionaries.CompUnitSealingTypes;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using Utils.Units;
using Pressure = Utils.Units.Pressure;
namespace GazRouter.Modes.GasCosts.Dialogs.UnitBleedingCosts
{
    public class UnitBleedingCostsModel : Listing, ICostCalcModel
    {
        public UnitBleedingCostsModel()
        {
            Unit = new CompUnit();
        }
        public double Calculate()
        {
            if (Unit.BleedingRate > 0)
            {
                var result = 0.001*Unit.BleedingRate*Runtime*Unit.SealingCount;
                ListingCalculation = string.Join("\n", new List<string>
                {
                    $"Q = 0,001*{Unit.BleedingRate}*{Runtime}*{Unit.SealingCount} = {result} тыс.м³",
                    "",
                    $"qупл = {Unit.BleedingRate} м³/ч - объемный расход выбросов газа из-за негерметичности уплотнения",
                    $"P = {Pressure.ToString(PressureUnit.Mpa)} - давление уплотняемого газа",
                    $"τ = {Runtime} ч - время работы ГПА",
                    $"m = {Unit.SealingCount} - количество уплотнений",
                });
                return result;
            }
            if (IsEmissionPowerKnown) return EmissionPowerKnownCalc();
            if (!Unit.SealingType.HasValue) return 0;
            switch (Unit.SealingType)
            {
                case CompUnitSealingType.BabbitSlit:  return BabitSlitCalc();// Бабитовые, щелевые
                case CompUnitSealingType.CeramicSlit: return CeramicSlit();  // Керамические, щелевые
                case CompUnitSealingType.CeramicButt: return CeramicButt();  // Керамические, торцевые
                case CompUnitSealingType.Dry:         return Dry();          // Сухие
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
#region calcs
        private double EmissionPowerKnownCalc()
        {
            var result = 0.001*(3.6*EmissionPower/Density.KilogramsPerCubicMeter)*Runtime*Unit.SealingCount;
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Q = 0,001 * (3,6 * {EmissionPower} / {Density.KilogramsPerCubicMeter}) * {Runtime} * {Unit.SealingCount} = {result} тыс.м³",
                "",
                $"M = {EmissionPower} г/с - мощность выброса газа",
                $"ρ = {Density.KilogramsPerCubicMeter} кг/м³ - плотность газа",
                $"τ = {Runtime} ч - время работы ГПА",
                $"m = {Unit.SealingCount} - количество уплотнений",
            });
            return result;
        }
        /// <summary> Бабитовые, щелевые </summary>
        /// <returns></returns>
        private double BabitSlitCalc()
        {
            //  var result = 0.001*0.0969*Math.Pow(Pressure.Mpa, 2.299)*Runtime*Unit.SealingCount;
            var a = 0.284*Math.Pow(Pressure.Mpa, 2) - 1.0936*Pressure.Mpa + 2.2266;
            var qM = IsEmissionsFactKnown ? EmissionsFact : ((new List<double> { 7.5, 5.5, 4, 3 }).Any(l => l == Pressure.Mpa) ? Math.Round(a, 1) : a);
            var result = 0.001*qM*Runtime*Unit.SealingCount;

            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Q = 0,001*{qM}*{Runtime}*{Unit.SealingCount} = {result} тыс.м³",
                "",
                $"qупл = {qM} м³/ч - объемный расход выбросов газа из-за негерметичности уплотнения",
                $"P = {Pressure.ToString(PressureUnit.Mpa)} - давление уплотняемого газа",
                $"τ = {Runtime} ч - время работы ГПА",
                $"m = {Unit.SealingCount} - количество уплотнений",
            });
            return result;
        }
        /// <summary> Керамические, щелевые </summary>
        /// <returns></returns>
        private double CeramicSlit()
        {
            var y1 = -0.0086 * Pressure.Mpa * Pressure.Mpa + 0.1214 * Pressure.Mpa - 0.3286;
            const double y2 = 0.02;
            var qM = IsEmissionsFactKnown ? EmissionsFact : (Pressure.Mpa > 4 ? ((new List<double> { 7.5, 5.5, 4, 3 }).Any(l => l == Pressure.Mpa) ? Math.Round(y1, 2) : y1) : y2);
            var result = 0.001 * qM * Runtime * Unit.SealingCount;
            
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Q = 0,001*{qM}*{Runtime}*{Unit.SealingCount} = {result} тыс.м³",
                "",
                $"qупл = {qM} м³/ч - объемный расход выбросов газа из-за негерметичности уплотнения",
                $"P = {Pressure.ToString(PressureUnit.Mpa)} - давление уплотняемого газа",
                $"τ = {Runtime} ч - время работы ГПА",
                $"m = {Unit.SealingCount} - количество уплотнений",
            });
            
            return result;
        }
        /// <summary> Керамические, торцевые </summary>
        /// <returns></returns>
        private double CeramicButt()
        {
            var a = -0.001*Pressure.Mpa*Pressure.Mpa + 0.0224*Pressure.Mpa - 0.0643;
            var qM = IsEmissionsFactKnown ? EmissionsFact : (Pressure.Mpa > 4 ? ((new List<double> { 7.5, 5.5, 4, 3 }).Any(l => l == Pressure.Mpa) ? Math.Round(a, 2) : a) : 0.01);
            var result = 0.001 * (qM) * Runtime * Unit.SealingCount;

            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Q = 0,001*{qM}*{Runtime}*{Unit.SealingCount} = {result} тыс.м³",
                "",
                $"qупл = {qM} м³/ч - объемный расход выбросов газа из-за негерметичности уплотнения",
                $"P = {Pressure.ToString(PressureUnit.Mpa)} - давление уплотняемого газа",
                $"τ = {Runtime} ч - время работы ГПА",
                $"m = {Unit.SealingCount} - количество уплотнений",
            });
            return result;
        }
        /// <summary> Сухие </summary>
        /// <returns></returns>
        private double Dry()
        {
            var a = -0.0247*Math.Pow(Pressure.Mpa, 3) + 0.5536*Math.Pow(Pressure.Mpa, 2) - 2.1103*Pressure.Mpa + 7.0574;
            //   var result = 0.001*2.764*Math.Exp(0.1949*Pressure.Mpa)*Runtime*Unit.SealingCount; 
            var qCY = IsEmissionsFactKnown ? EmissionsFact : ((new List<double> {11.0, 7.5, 5.5, 4, 3}).Any(l => l == Pressure.Mpa) ? Math.Round(a) : a);
            var result = 0.001 * qCY * Runtime * Unit.SealingCount;
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Q = 0,001*{qCY}*{Runtime}*{Unit.SealingCount} = {result} тыс.м³",
                "",
                $"qупл = {qCY} м³/ч - объемный расход выбросов газа из-за негерметичности уплотнения",
                $"P = {Pressure.ToString(PressureUnit.Mpa)} - давление уплотняемого газа",
                $"τ = {Runtime} ч - время работы ГПА",
                $"m = {Unit.SealingCount} - количество уплотнений",
            });            
            return result;
        }
#endregion
#region property
        [Display(Name = "Время работы ГПА, ч")]
        public int Runtime { get; set; }
        [Display(Name = "Давление уплотняемого газа, кг/см²")]
        public Pressure Pressure { get; set; }
        [Display(Name = "ГПА")]
        public CompUnit Unit { get; set; }
        [Display(Name = "Плотность газа при стандартных условиях, кг/м³")]
        public Density Density { get; set; }
        [Display(Name = "Мощность выброса, г/с")]
        public double EmissionPower { get; set; }
        [Display(Name = "Мощность выброса известна")]
        public bool IsEmissionPowerKnown { get; set; }
        [Display(Name = "Объемный расход выбросов, м³/ч")]
        public double EmissionsFact { get; set; }
        [Display(Name = "Объемный расход выбросов известен")]
        public bool IsEmissionsFactKnown { get; set; }
        #endregion
    }
}
