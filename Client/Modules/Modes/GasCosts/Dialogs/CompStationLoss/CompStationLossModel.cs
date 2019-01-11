using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.Modes.GasCosts.Dialogs.Model;
namespace GazRouter.Modes.GasCosts.Dialogs.CompStationLoss
{
    public class CompStationLossModel : Listing, ICostCalcModel
    {
        [Display(Name = "Количество ЗРА/свечей с утечками, -")]
        public double LeakageCount { get; set; }
        
        [Display(Name = "Время существования утечки через ЗРА/свечи, ч")]
        public double LeaksDuration { get; set; }
        
        [Display(Name = "Технологические потери на КС от ЗРА/свечей (в положении свечных кранов \"закрыто\")")]
        public double Loss { get; set; }
        
        [Display(Name = "Количество ЗРА/свечей всего, -")]
        public double TotalCount { get; set; }

        public bool IsItValve { get; set; }

        public bool IsItCandle { get; set; }

        public double GasConsumption { get; set; }

        public string EntityName { get; set; }
        
        public double Calculate()
        {
            // Эксплуатационные утечки
            //var valveLeaksSpeed = 0.003; // значение по справочнику, м³/мин
            //var candleLeaksSpeed = 0.06;   // значение по справочнику, м³/мин
            var leaksSpeed = GasConsumption; // IsItValve ? valveLeaksSpeed : candleLeaksSpeed;
            var operationalLeaks = 0.06*LeakageCount*(leaksSpeed)*LeaksDuration;

            // Технологические потери
            var technologicalLoss = Loss* TotalCount;

            var calc = operationalLeaks + technologicalLoss;

#region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"QУТ_{EntityName} = {operationalLeaks} + {technologicalLoss} = {calc} тыс.м³",
                "",
                $"Qэу_{EntityName} = 0,06 * {LeakageCount} * {leaksSpeed} * {LeaksDuration} = {operationalLeaks} тыс.м³ - эксплуатационные утечки газа",
                $"k = {LeakageCount} - количество ЗРА/свечей, где обнаружены утечки",
                $"qут= {leaksSpeed} м³/мин- объемные расходы утечек газа через ЗРА/устья свечей (в положении свечных кранов «закрыто»)",
                $"τж = {LeaksDuration} ч - время существования утечки",
                $"Qут_{EntityName} = {Loss} * {TotalCount} = {technologicalLoss} тыс.м³ - технологические потери газа на {EntityName}, обусловленные утечками газа",
                $"Qут = {Loss} тыс.м³ - технологические потери газа на {EntityName}, обусловленные утечками от ЗРА/свечей (в положении свечных кранов «закрыто»),",
                $"n = {TotalCount} - количество ЗРА/свечей на {EntityName}",
            });
#endregion
            return calc;
        }
    }
}
