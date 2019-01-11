using System;
using GazRouter.DTO.Dictionaries.CompUnitFailureCauses;
using GazRouter.DTO.Dictionaries.CompUnitFailureFeatures;
using GazRouter.DTO.Dictionaries.CompUnitRepairTypes;
using GazRouter.DTO.Dictionaries.CompUnitStopTypes;
using GazRouter.DTO.Dictionaries.StatesModel;

namespace GazRouter.DTO.ManualInput.CompUnitStates
{
    public class AddCompUnitStateParameterSet
    {
        public Guid CompUnitId { get; set; }
        public DateTime StateChangeDate { get; set; }
        public CompUnitState State { get; set; }
        public CompUnitStopType? StopType { get; set; }

        public bool? IsRepairNext { get; set; }

        public CompUnitRepairType? RepairType { get; set; }
        public DateTime? RepairCompletionDate { get; set; }

        public bool IsCritical { get; set; }
        public CompUnitFailureCause? FailureCause { get; set; }
        public CompUnitFailureFeature? FailureFeature { get; set; }

        public string FailureExternalView { get; set; }
        public string FailureCauseDescription { get; set; }
        public string FailureWorkPerformed { get; set; }
    }
}