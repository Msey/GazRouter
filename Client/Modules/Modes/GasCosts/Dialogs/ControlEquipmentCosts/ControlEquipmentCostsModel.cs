using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.Modes.GasCosts.Dialogs.Model;
namespace GazRouter.Modes.GasCosts.Dialogs.ControlEquipmentCosts
{
    public class ControlEquipmentCostsModel : Listing, ICostCalcModel
    {
        public ControlEquipmentCostsModel()
        {
            Count = 1;
            Time = 1;
        }
        public double Calculate()
        {
            var result = 0.001 * Q / 1000 * 60 * Time * Count;
#region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Qкип = 0,001 * ({Q} / 1000) * (60 * {Time}) * {Count} = {result} тыс.м³",
                "",
                $"q = {Q} л/мин - расход газа прибором (сброс в атмосферу)",
                $"τ = {Time} ч - время работы прибора",
                $"k = {Count} - количество приборов",
            });
#endregion
            return result;
        }

        [Display(Name = "Тип(марка) прибора")]
        public string Type { get; set; }

        [Display(Name = "Время работы прибора, ч")]
        public int Time { get; set; }

        [Display(Name = "Количество приборов")]
        public int Count { get; set; }

        [Display(Name = "Расход газа прибором (сброс в атмосферу), м³/ч")]
        public double Q { get; set; }
    }
}