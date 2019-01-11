using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Repairs.Plan
{
    [DataContract]
    public class RepairPlanCompShopDTO : RepairPlanBaseDTO
    {
        /// <summary>
        /// Идентификатор МГ, к которому подключен цех
        /// </summary>
		[DataMember]
		public Guid PipelineId { get; set; }

        /// <summary>
        /// Наименование МГ, к которому подключен цех
        /// </summary>
        [DataMember]
        public string PipelineName { get; set; }

        /// <summary>
        /// Километр подключения цеха на МГ
        /// </summary>
        [DataMember]
        public double Kilometer { get; set; }
        
    }
}
