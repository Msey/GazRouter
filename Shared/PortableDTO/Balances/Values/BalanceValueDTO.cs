using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GazRouter.DTO.Balances.Corrections;
using GazRouter.DTO.Balances.Irregularity;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DTO.Balances.Values
{
    [DataContract]
    public class BalanceValueDTO : BaseDto<int>
    {
        public BalanceValueDTO()
        {
            IrregularityList = new List<IrregularityDTO>();
        }

        [DataMember]
        public int ContractId { get; set; }

        [DataMember]
        public int GasOwnerId { get; set; }



        [DataMember]
        public Guid EntityId { get; set; }

        [DataMember]
        public EntityType? EntityType { get; set; }


        [DataMember]
        public Guid? DistrStationId { get; set; }


        [DataMember]
        public double BaseValue { get; set; }
        
        [DataMember]
        public double? Correction { get; set; }


        [DataMember]
        public BalanceItem BalanceItem { get; set; }


        [DataMember]
        public List<IrregularityDTO> IrregularityList { get; set; }


        [DataMember]
        public List<CorrectionDTO> CorrectionList { get; set; }
    }
}