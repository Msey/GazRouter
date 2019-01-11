using System.Collections.Generic;
using GazRouter.Modes.GasCosts.Dialogs.Model;
namespace GazRouter.Modes.GasCosts.Dialogs.ThermalDisposalUnitCosts
{
    /// <summary> Q кптг/кто расход газа для работы комплекса термического 
    /// обезвреживания, тыс. м3, рассчитывают по формуле </summary>
    public class ThermalDisposalUnitCostsModel : Listing, ICostCalcModel
    {
        public double Hm0 { get; set; }
        public double Motx { get; set; }
        public double Calculate()
        {
            var calc = 0.001 * Hm0 * Motx;
#region listing
            ListingCalculation = string.Join("\n", new List<string>
            {
                $"Qуто = 0,001 * {Hm0} * {Motx} = {calc} тыс. м³", 
                $"", 
                $"Hтₒ = {Hm0} м³/кг - норматив расхода топлива на сжигание 1 кг отходов",
                $"mотх = {Motx} кг - масса жидких и твёрдых отходов, подвергшихся утилизации",
            });
#endregion
            return calc;
        }
    }
}
#region trash

        //        private double _hm0;
        /// <summary> норматив расхода топлива на сжигание 1 кг отходов, м3/кг, в соответствии с паспортными данными; </summary>
        //        [Display(Name = "Норматив расхода топлива на сжигание 1 кг отходов")]
        //        public double Hm0 {
        //            get
        //            {
        //                return _hm0;
        //            }
        //            set
        //            {
        //                _hm0 = value;
        //            }
        //        }
        /// <summary> масса жидких и твёрдых отходов, подвергшихся утилизации, кг; </summary>
        //        [Display(Name = "Масса жидких и твёрдых отходов, подвергшихся утилизации,")]
        //        public double Motx { get; set; }
        /// <summary> Q кптг/кто - расход газа для работы комплекса термического обезвреживания, тыс. м3, рассчитывают по формуле (5.41) </summary>
        /// <returns></returns>
        
//            return 0.001 * _hm0 * Motx;
#endregion