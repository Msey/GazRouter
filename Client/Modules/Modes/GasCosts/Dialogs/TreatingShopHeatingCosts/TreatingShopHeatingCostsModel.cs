using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.Modes.GasCosts.Dialogs.Model;
namespace GazRouter.Modes.GasCosts.Dialogs.TreatingShopHeatingCosts
{
    public class TreatingShopHeatingCostsModel : Listing, ICostCalcModel
    {
        public double Calculate()
        {            
            var result = ( HeaterConsumption/HeaterConsumptionFact*HeaterConsumptionAverage*Period ) / 1000;
#region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Qпг_цоог = ({HeaterConsumption} / {HeaterConsumptionFact} * {HeaterConsumptionAverage} * {Period}) / 1000 = {result} тыс.м³",
                "",
                $"Qᵖцоог = {HeaterConsumption} тыс.м³ - расход газа через ЦООГ в расчетном периоде",
                $"Qɸцоог = {HeaterConsumptionFact} тыс.м³ - фактический расход газа через ЦООГ",
                $"qɸцоог = {HeaterConsumptionAverage} м³/ч - фактическое среднее значение расхода газа в подогревателе газа ЦООГ за аналогичный период прошлого года",
                $"τпг = {Period} ч - время работы подогревателя в расчетном периоде",
            });
#endregion
            return result;
        }

        [Display(Name = "Номинальный расход на работу подогревателя газа в ЦООГ, тыс.м³")]
        public double HeaterConsumption { get; set; }

        [Display(Name = "Время работы подогревателя в расчетном периоде, ч")]
        public int Period { get; set; }

        [Display(Name = "Фактический расход газа через ЦООГ за аналогичный период прошлого года, тыс.м³")]
        public double HeaterConsumptionFact { get; set; }

        [Display(Name = "Фактический среднее значение расхода в подогревателе газа ЦООГ за аналогичный период прошлого года, м³/ч")]
        public double HeaterConsumptionAverage { get; set; }
    }
}