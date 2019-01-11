using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GazRouter.DTO.Balances.Values
{
    public class BalanceExchangeValueDTO : BalanceValueDTO
    {
        [DataMember]
        public string  EntityName { get; set; }

        [DataMember]
        public int PropertyTypeId { get; set; }

        [DataMember]
        public string PropertyUnitName { get; set; }

        [DataMember]
        public string PropertyDescription { get; set; }

        [DataMember]
        public string PropertyValueType { get; set; }

        [DataMember]
        public string ParameterGidString { get; set; }

        [DataMember]
        public DateTime ContractDate { get; set; }

        [DataMember]
        public string SqlQuery { get; set; }

    }
}
