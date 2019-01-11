using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Balances.Routes
{
    [DataContract]
	public class RouteSectionDTO
    {
        /// <summary>
        /// Id секции маршрута
        /// </summary>
        [DataMember]
        public int RouteSectionId { get; set; }

        /// <summary>
        /// Id маршрута
        /// </summary>
        [DataMember]
        public int RouteId { get; set; }

        /// <summary>
        /// Id трубы
        /// </summary>
        [DataMember]
        public Guid PipelineId { get; set; }


        /// <summary>
        /// Имя трубы
        /// </summary>
        [DataMember]
        public string PipelineName { get; set; }


        /// <summary>
        /// Км конца секции
        /// </summary>
        [DataMember]
        public double? KilometerEnd { get; set; }

        /// <summary>
        /// Км начала секции
        /// </summary>
        [DataMember]
        public double? KilometerStart { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public int? SortOrder{ get; set; }

	}
}