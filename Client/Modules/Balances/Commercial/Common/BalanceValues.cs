using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceItems;

namespace GazRouter.Balances.Commercial.Common
{
    public class BalanceValues
    {
        private List<BalanceValueDTO> _valueList;
        public BalanceValues(List<BalanceValueDTO> valueList)
        {
            _valueList = valueList;
        }

        public BalanceValueDTO GetValue(Guid entityId, int ownerId, BalanceItem balItem, double coef = 1.0)
        {
            var val = _valueList.SingleOrDefault(v => v.EntityId == entityId && v.GasOwnerId == ownerId && v.BalanceItem == balItem);
            if (val != null)
            {
                val.BaseValue *= coef;
                val.Correction *= coef;
                val.IrregularityList?.ForEach(i => i.Value *= coef);
            }
            return val;
        }
    }
}
