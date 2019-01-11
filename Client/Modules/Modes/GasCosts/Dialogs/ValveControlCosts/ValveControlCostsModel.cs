using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GazRouter.Modes.GasCosts.Dialogs.Model;
namespace GazRouter.Modes.GasCosts.Dialogs.ValveControlCosts
{
    public class ValveControlCostsModel : Listing, ICostCalcModel
    {
        public ValveControlCostsModel()
        {
            ValveShiftings = new List<ValveShifting>();
            RegulatorRuntimes = new List<RegulatorRuntime>();
        }
        public double Calculate()
        {
            var valveShiftingsSum = ValveShiftings.Sum(v => v.Q);
            var regulatorRuntimesSum = RegulatorRuntimes.Sum(r => r.Q);
            var calc = 0.001 * (valveShiftingsSum + regulatorRuntimesSum);
#region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Q = 0,001 * ({valveShiftingsSum} + {regulatorRuntimesSum}) = {calc} тыс. м³",
                "",
                ValveShiftingsListing(ValveShiftings),
                RegulatorRuntimesListing(RegulatorRuntimes),
            });
#endregion 
            return calc;
        }
        [Display(Name = "Список переключений кранов (по типам)")]
        public List<ValveShifting> ValveShiftings { get; private set; }
        [Display(Name = "Список работающих регуляторов (по типам)")]
        public List<RegulatorRuntime> RegulatorRuntimes { get; private set; }
    }
}
#region trash


//            ValveShiftings.ForEach(item => vars.Add($"{item.Name}= {item.Count} * {item.RatedConsumption} = {item.Q}"));
//            sb1.Append("ValveShiftings =");
//            ValveShiftings.ForEach(item => sb1.Append($" {item.Q} +"));
//            sb1.Remove(sb1.Length - 1, 1).Append($"= {valveShiftingsSum}");
#endregion