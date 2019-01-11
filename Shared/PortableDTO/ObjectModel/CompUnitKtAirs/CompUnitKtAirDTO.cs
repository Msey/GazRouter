using System.Runtime.Serialization;

namespace GazRouter.DTO.ObjectModel.CompUnitKtAirs
{
    public class CompUnitKtAirDTO
	{
        /// <summary>
        /// Минимальная температура интервала, соответствующего данному KtValue
        /// </summary>
        [DataMember]
        public double? TMin { get; set; }

        /// <summary>
        /// Максимальная температура интервала, соответствующего данному KtValue
        /// </summary>
        [DataMember]
        public double? TMax { get; set; }

        /// <summary>
        /// Коэффициент, учитывающий влияние температуры воздуха
        /// </summary>
        [DataMember]
        public double? KtValue { get; set; }
	}
}