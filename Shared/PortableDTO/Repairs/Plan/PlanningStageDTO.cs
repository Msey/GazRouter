using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Repairs.Plan
{
    [DataContract]
    public class PlanningStageDTO : BaseDto<int>
    {
        public PlanningStageDTO()
        {
            Stage = PlanningStage.Filling;
        }

        [DataMember]
        public PlanningStage Stage { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public DateTime UpdateDate { get; set; }

        public override string ToString()
        {
            return $"{UserName}\r\n{UpdateDate:d}\r\n{UpdateDate:t}";
        }
    }
}