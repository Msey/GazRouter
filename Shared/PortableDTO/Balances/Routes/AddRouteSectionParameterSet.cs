using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Balances.Routes
{
    [DataContract]
	public class AddRouteSectionParameterSet
    {
        /// <summary>
        /// Id маршрута
        /// </summary>
        public int RouteId { get; set; }

        /// <summary>
        /// Id трубы
        /// </summary>
        public Guid PipelineId { get; set; }

        /// <summary>
        /// Км конца секции
        /// </summary>
        public double? KilometerEnd { get; set; }

        /// <summary>
        /// Км начала секции
        /// </summary>
        public double? KilometerStart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? SortOrder{ get; set; }

	}
}