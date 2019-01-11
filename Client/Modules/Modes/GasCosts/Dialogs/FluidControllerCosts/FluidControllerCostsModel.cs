using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.Modes.GasCosts.Dialogs.Model;
namespace GazRouter.Modes.GasCosts.Dialogs.FluidControllerCosts
{
    public class FluidControllerCostsModel : Listing, ICostCalcModel
    {
        [Display(Name = "Объем газа, стравливаемого в атмосферу из пневморегуляторов, пневмоустройств, м³/ч")]
        public double Qpr { get; set; }


        [Display(Name = "Количество работающих пневморегуляторов, пневмоустройств данного типа")]
        public double N { get; set; }


        [Display(Name = "Время работы пневморегулятора, пневмоустройства, ч")]
        public double Time { get; set; }
        

        public double Calculate()
        {
            var calc = 0.001 * Qpr * Time * N;
#region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                 $"Qпр = 0,001 * {Qpr} * {Time} * {N}  = {calc} тыс.м³",
                 $"",
                 $"qпр  = {Qpr} м³/ч - объем газа, стравливаемого в атмосферу из пневморегуляторов, пневмоустройств",
                 $"τ = {Time} ч - время работы пневморегулятора, пневмоустройства",
                 $"n = {N} - количество работающих пневморегуляторов, пневмоустройств данного типа",
            });
#endregion
            return calc;
        }
    }
}
