using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Attachments;
using GazRouter.DTO.Dictionaries.CompUnitFailureCauses;
using GazRouter.DTO.Dictionaries.CompUnitFailureFeatures;

namespace GazRouter.DTO.ManualInput.CompUnitStates
{
    [DataContract]
    public class CompUnitFailureDetailsDTO
    {
        [DataMember]
        public int FailureId { get; set; }

        [DataMember]
        public bool IsCritical { get; set; }

        [DataMember]
        public CompUnitFailureCause FailureCause { get; set; }

        [DataMember]
        public CompUnitFailureFeature FailureFeature { get; set; }

        [DataMember]
        public string FailureExternalView { get; set; }

        [DataMember]
        public string FailureCauseDescription { get; set; }

        [DataMember]
        public string FailureWorkPerformed { get; set; }

        [DataMember]
        public List<FailureRelatedUnitStartDTO> UnitStartList { get; set; }

        [DataMember]
        public List<AttachmentDTO<int, int>> AttachmentList { get; set; }

        /// <summary>
        /// Дата перевода агрегата в резерв после востановительного ремонта
        /// </summary>
        [DataMember]
        public DateTime? ToReserveDate { get; set; }

        /// <summary>
        /// Дата перевода пуска агрегата после востановительного ремонта
        /// </summary>
        [DataMember]
        public DateTime? ToWorkDate { get; set; }

    }
}
