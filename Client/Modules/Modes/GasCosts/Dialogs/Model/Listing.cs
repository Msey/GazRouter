using System.Collections.Generic;
using System.Text;
using System.Linq;
using GazRouter.Modes.GasCosts.Dialogs.CompStationLoss;
namespace GazRouter.Modes.GasCosts.Dialogs.Model
{
    public abstract class Listing
    {
        protected string ListingCalculation = string.Empty;
        


        protected string ValveShiftingsListing(List<ValveShifting> valveShiftings)
        {
            var sb = new StringBuilder();
            sb.Append("Qзра =");
            var vars = new List<string>();
            valveShiftings.ForEach(item => sb.Append($" {item.Count} * {item.RatedConsumption} +"));
            sb.Remove(sb.Length - 1, 1);
            var valveShiftingsSum = valveShiftings.Sum(v => v.Q);
            if (valveShiftings.Count > 1)
            {
                sb.Append($"= ");
                valveShiftings.ForEach(item => sb.Append($" {item.Q} +"));
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append($"= {valveShiftingsSum} м³ - объем газа, стравливаемого при эксплуатации ЗРА");

            vars.Add(sb.ToString());
            return string.Join(" = ", vars);
        }
        protected string RegulatorRuntimesListing(List<RegulatorRuntime> regulatorRuntimes)
        {
            var sb = new StringBuilder();
            sb.Append("Qкр =");
            var vars = new List<string>();
            regulatorRuntimes.ForEach(item => sb.Append($" {item.Count} * {item.Runtime} * {item.RatedConsumption} +"));
            sb.Remove(sb.Length - 1, 1);
            var regulatorRuntimesSum = regulatorRuntimes.Sum(r => r.Q);
            if (regulatorRuntimes.Count > 1)
            {
                sb.Append($"= ");
                regulatorRuntimes.ForEach(item => sb.Append($" {item.Q} +"));
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append($"= {regulatorRuntimesSum} м³ - технологические потери газа при работе крана–регулятора");
            vars.Add(sb.ToString());
            return string.Join(" = ", vars);
        }

        public override string ToString()
        {
            return ListingCalculation;
        }
    }
}
