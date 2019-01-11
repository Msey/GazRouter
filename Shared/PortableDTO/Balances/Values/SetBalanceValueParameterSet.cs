using System;
using System.Collections.Generic;
using GazRouter.DTO.Balances.Corrections;
using GazRouter.DTO.Balances.Irregularity;
using GazRouter.DTO.Dictionaries.BalanceItems;

namespace GazRouter.DTO.Balances.Values
{
    public class SetBalanceValueParameterSet
    {
        public int ContractId { get; set; }
        public int GasOwnerId { get; set; }
        public Guid? EntityId { get; set; }
        public BalanceItem BalanceItem { get; set; }
        
        public double? BaseValue { get; set; }
        public double? Correction { get; set; }

        public List<SetIrregularityParameterSet> IrregularityList { get; set; }
        public List<SetCorrectionParameterSet> CorrectionList { get; set; }
    }
}