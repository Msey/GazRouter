using GazRouter.DTO.SeriesData.PropertyValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.DataExchange.Integro.SessionData
{
    [DataContract]
    public class IntegroExchangePropertyDto
    {
        [DataMember]
        public int? SeriesId { get; set; }

        [DataMember]
        public int? ContractId { get; set; }

        [DataMember]
        public Guid EntityId { get; set; }
        /// <summary>
        /// Для NSI
        /// </summary>
        [DataMember]
        public Guid? AnalyticGid { get; set; }

        [DataMember]
        public string EntityName { get; set; }

        [DataMember]
        public ExchangeSummaryProperty PropertyType { get; set; }

        [DataMember]
        public BasePropertyValueDTO PropertyValue { get; set; }

        [DataMember]
        public string ParameterGidString { get; set; }
    }
}
