using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Repairs.Complexes;
using GazRouter.DTO.Repairs.Plan;

namespace GazRouter.DTO.Repairs
{
    [DataContract]
    public class RepairPlanDataDTO
    {
        /// <summary>
        /// Список ремонтных работ
        /// </summary>
		[DataMember]
		public List<RepairPlanBaseDTO> RepairList { get; set; }

        /// <summary>
        /// Список комплексов
        /// </summary>
        [DataMember]
        public List<ComplexDTO> ComplexList { get; set; }

        /// <summary>
        /// Этап планирования
        /// </summary>
        [DataMember]
        public PlanningStageDTO PlanningStage { get; set; }
    }
}
