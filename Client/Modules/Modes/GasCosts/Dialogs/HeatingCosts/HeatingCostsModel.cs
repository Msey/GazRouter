using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using Utils.Units;

namespace GazRouter.Modes.GasCosts.Dialogs.HeatingCosts
{
    public class HeatingCostsModel : Listing, ICostCalcModel
    {
        public HeatingCostsModel()
        {
            BoilerType = new BoilerType();
        }
        public double Calculate()
        {
            double result;
            if (BoilerType.IsSmall) { 
                // Расчет для котлов малой мощности
                result = 0.001 * 142.9 / BoilerType.EfficiencyRated * BoilerType.HeatProductivityRated * Period * 7000 / CombHeat.KCal;
#region listing
                ListingCalculation = string.Join("\n", new List<string>
                {
                    $"Qкот = 0,001 * 142,9 / {BoilerType.EfficiencyRated} * {BoilerType.HeatProductivityRated} * {Period} * 7000 / {CombHeat} = {result} тыс.м³",
                    "",
                    $"ƞkₒ = {BoilerType.EfficiencyRated} - номинальный КПД котла",
                    $"gkₒ = {BoilerType.HeatProductivityRated} Гкал/ч - номинальная теплопроизводительность котла малой мощности",
                    $"τk = {Period} ч - планируемая продолжительность работы котла",
                    $"Qᵸp = {CombHeat} ккал/м³ - низшая теплота сгорания газа",
                });
#endregion
                return result;
            }
            // Нормативный расход отпущенной тепловой энергии, Гкал
            var qkot = HeatSupplySystemLoad * Period;
            // норма расхода условного топлива котельной, кг у.т./Гкал
            var hkot = 142.857 / BoilerType.EfficiencyRated / (1 - HeatLossFactor);
            var b = 0.0;
            if (BoilerType.HeatingArea <= 50)
                b = ShutdownPeriod > 48 ? 300 : 4.1468*ShutdownPeriod + 0.6417;
            if (BoilerType.HeatingArea > 50 && BoilerType.HeatingArea <= 100)
                b = ShutdownPeriod > 48 ? 600 : 8.3294*ShutdownPeriod + 0.1283;
            if (BoilerType.HeatingArea > 100 && BoilerType.HeatingArea <= 200)
                b = ShutdownPeriod > 48 ? 1200 : 16.659*ShutdownPeriod + 0.2567;
            if (BoilerType.HeatingArea > 200 && BoilerType.HeatingArea <= 300)
                b = ShutdownPeriod > 48 ? 1800 : 24.967*ShutdownPeriod + 0.7701;
            if (BoilerType.HeatingArea > 300 && BoilerType.HeatingArea <= 400)
                b = ShutdownPeriod > 48 ? 2400 : 33.317*ShutdownPeriod + 0.5134;
            if (BoilerType.HeatingArea > 400 && BoilerType.HeatingArea <= 500)
                b = ShutdownPeriod > 48 ? 3000 : 41.647*ShutdownPeriod + 0.6417;
            result = 0.001 *(hkot*qkot + b*LightingCount)*7000/CombHeat.KCal;
#region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Qкот = 0,001 * ({hkot} * {qkot} + {b} * {LightingCount}) * 7000 / {CombHeat} = {result} тыс.м³",
                "",
                $"Hкот = {142.857 / BoilerType.EfficiencyRated} / (1 - {HeatLossFactor}) = {hkot} кг у.т./Гкал - норма расхода условного топлива котельной",
                $"Hₒ =  142,857 / {BoilerType.EfficiencyRated} = {142.857 / BoilerType.EfficiencyRated} кг у.т./Гкал - индивидуальный норматив расхода условного топлива на выработку единицы тепла котлом",
                $"ƞkₒ = {BoilerType.EfficiencyRated} - номинальный КПД котла",
                $"dс.н. = {HeatLossFactor} - коэффициент внутрикотельных потерь",
                $"Gтэ = {HeatSupplySystemLoad} * {Period} = {qkot} Гкал - количество тепловой энергии, вырабатываемой за расчетный период",
                $"gₒ = {HeatSupplySystemLoad} Гкал/ч - присоединенная нагрузка системы теплоснабжения",
                $"τk = {Period} ч - планируемая продолжительность работы котла",
                $"bраст = {b} кг у.т. - удельный расход условного топлива на одну растопку котла",
                $"kраст = {LightingCount} - количество растопок за расчетный период",
                $"Qᵸp = {CombHeat} ккал/м³ - низшая теплота сгорания газа",
            });
#endregion
            return result;
        }
        [Display(Name = "Тип котлоагрегата")]
        public BoilerType BoilerType { get; set; }

        [Display(Name = "Коэф. внутрикотельных потерь")]
        public double HeatLossFactor { get; set; }

        [Display(Name = "Присоединенная нагрузка системы теплоснабжения, Гкал/ч")]
        public double HeatSupplySystemLoad { get; set; }

        [Display(Name = "Время работы котлоагрегата, ч")]
        public int Period { get; set; }
        
        [Display(Name = "Кол-во растопок")]
        public int LightingCount { get; set; }
        
        [Display(Name = "Длительность остановки между пусками котла, ч")]
        public int ShutdownPeriod { get; set; }

        [Display(Name = "Низшая теплота сгорания газа, ккал/м³")]
        public CombustionHeat CombHeat { get; set; }
    }
}