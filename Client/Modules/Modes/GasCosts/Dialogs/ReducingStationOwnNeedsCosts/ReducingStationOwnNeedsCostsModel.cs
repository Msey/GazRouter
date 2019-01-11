using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.Dictionaries.HeaterTypes;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.Dialogs.RepairCosts;
namespace GazRouter.Modes.GasCosts.Dialogs.ReducingStationOwnNeedsCosts
{
    public class ReducingStationOwnNeedsCostsModel : RepairCostsModel, ICostCalcModel
    {
        double ICostCalcModel.Calculate()
        {
            if (CalcType)
            {
                var result1 = base.Calculate();
                return result1;
            }
            var result2 = 0.001 * Qtg0 * Time;
#region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"QПГ_арп = 0,001 * {Qtg0} * {Time} = {result2} тыс.м³ - расход газа на работу подогревателя газа для подогрева редуцируемого газа перед регулятором давления",
                "",
                $"qпгₒ = {Qtg0} м³/ч - номинальный расход газа в подогревателе газа",
                $"τпг = {Time} ч - время работы подогревателя газа",
            });
#endregion
            return result2;
        }
        public bool CalcType { get; set; }
        [Display(Name = "номинальный расход газа в подогревателе газа, м³/ч")]
        public double Qtg0 { get; set; }
        [Display(Name = "время работы подогревателя газа, ч")]
        public int Time { get; set; }
        [Display(Name = "выбранный тип подогревателя")]
        public HeaterTypeDTO SelectedHeaterType { get; set; }

    }
}
