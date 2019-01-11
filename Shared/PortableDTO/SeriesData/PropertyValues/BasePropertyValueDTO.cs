using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.SeriesData.ValueMessages;

namespace GazRouter.DTO.SeriesData.PropertyValues
{
    [DataContract]
    public abstract class BasePropertyValueDTO : IComparable
    {
        protected BasePropertyValueDTO()
        {
            MessageList = new List<PropertyValueMessageDTO>();

            QualityCode = QualityCode.Good;
            QualityDesсription = "";
        }
        [DataMember]
        public int SeriesId { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public int Year { get; set; }

        [DataMember]
        public int Month { get; set; }

        [DataMember]
        public int Day { get; set; }


        [DataMember]
        public PeriodType PeriodTypeId { get; set; }


        [DataMember]
        public List<PropertyValueMessageDTO> MessageList { get; set; }


        [DataMember]
        public SourceType? SourceType { get; set; }
        

        [DataMember]
        public QualityCode QualityCode { get; set; }
        
        
        [DataMember]
        public string QualityDesсription { get; set; }
        

        public int CompareTo(object obj)
        {
            if (obj == null) return -1;

            var v = obj as BasePropertyValueDTO;
            if (v.Date > Date) return -1;

            return 1;

        }
    }

    public enum QualityCode
    {
        Good = 1,
        Bad = 2,
        NoValue = 3
    }

    public enum SourceType
    {
        StandardCalculation = 1,
        CustomCalculation   = 2,
        OtherCalculation    = 3,
        ManualInput         = 5,
        CustomExchange      = 6,
    }
}