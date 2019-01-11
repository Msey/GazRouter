using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.CompUnitRepairTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.Dictionaries.CompUnitStopTypes;

namespace GazRouter.DTO.ManualInput.CompUnitStates
{
    [DataContract]
    public class CompUnitStateDTO : BaseDto<int>
    {
        [DataMember]
        public Guid CompUnitId { get; set; }

		[DataMember]
        public DateTime StateChangeDate { get; set; }
        
        [DataMember]
        public TimeSpan InStateDuration { get; set; }
        
        [DataMember]
        public CompUnitState State { get; set; }
        
        [DataMember]
        public DateTime InputDate { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string UserSite { get; set; }

        [DataMember]
        public CompUnitStopType? StopType { get; set; }


        // Информация о резерве
        [DataMember]
        public bool IsRepairNext { get; set; }

        // Информация о ремонте
        [DataMember]
        public DateTime? CompletionDatePlan { get; set; }

        [DataMember]
        public CompUnitRepairType? RepairType { get; set; }

        [DataMember]
        public bool IsDelayed { get; set; }


        [DataMember]
        public CompUnitFailureDetailsDTO FailureDetails { get; set; }
    }
}
