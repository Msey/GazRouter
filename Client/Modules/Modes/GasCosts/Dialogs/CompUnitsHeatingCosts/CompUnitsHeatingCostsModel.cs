using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.Modes.GasCosts.Dialogs.Model;
namespace GazRouter.Modes.GasCosts.Dialogs.CompUnitsHeatingCosts
{
    /// <summary> Расход газа на обогрев укрытий ГПА с использованием 
    /// воздухонагревательных агрегатов-утилизаторов Qкц/укр, тыс. м3 </summary>
    public class CompUnitsHeatingCostsModel : Listing, ICostCalcModel
    {
        /// <summary> Kt  – коэффициент регулирования тепловой мощности, учитывающий условия эксплуатации подогревателя газа (зависимость от температуры наружного воздуха), используют данные, приведенные в режимно-наладочной карте.
        /// </summary>
        [Display(Name = "Коэффициент регулирования тепловой мощности")]
        public double Kt { get; set; }       
        /// <summary> qba0 – номинальный расход газа на работу установки обогрева, м3/ч, используют паспортные данные;
        /// </summary>
        [Display(Name = "Номинальный расход газа на работу установки обогрева")]
        public double Qba { get; set; }
        /// <summary> tau - время работы установки обогрева в расчетный период, ч; </summary>
        [Display(Name = "Время работы установки обогрева в расчетный период")]
        public double Time { get; set; }
        public double Calculate()
        {
            var calc = 0.001 * Kt * Qba * Time;
#region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                 $"Qукр = {0.001} * {Qba} * {Kt} * {Time} = {calc} тыс.м³",
                 $"",
                 $"qBAₒ = {Qba} м³/ч - номинальный расход газа на работу установки обогрева", 
                 $"Kt = {Kt} - коэффициент регулирования тепловой мощности", 
                 $"τ = {Time} ч - время работы установки обогрева",
            });
#endregion
            return calc;
        }
    }
}

