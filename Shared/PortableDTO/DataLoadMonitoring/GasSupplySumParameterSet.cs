using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.DataLoadMonitoring
{
    [DataContract]
    public class GasSupplySumParameterSet
    {
        //время
        [DataMember]
        public DateTime KeyDate { get; set; }
        //количество дней , за которое выбирать значение
        [DataMember]
        public int CountDays { get; set; }
        //тип периода
        [DataMember]
        public PeriodType PeriodType { get; set; }
        //ид газопровода
        [DataMember]
        public Guid? PipelineId { get; set; }

        [DataMember]
        public double? KmBegin { get; set; }
        [DataMember]
        public double? KmEnd { get; set; }
    }
}
