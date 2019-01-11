using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.SeriesData.ValueMessages;

namespace GazRouter.DTO.SeriesData.DayBalanceValue
{
    [DataContract]
    public abstract class DayBalanceValueDTO
    {
        [DataMember]
        public Guid EntityId { get; set; }


        [DataMember]
        public double Current { get; set; }

        [DataMember]
        public double Prev { get; set; }

        [DataMember]
        public double MonthTotal { get; set; }

        [DataMember]
        public int DayPlan { get; set; }

        [DataMember]
        public int MonthPlan { get; set; }
        
    }

    
}