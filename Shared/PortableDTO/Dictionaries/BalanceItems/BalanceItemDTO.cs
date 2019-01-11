using System;
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.BalanceSigns;

namespace GazRouter.DTO.Dictionaries.BalanceItems
{
	[DataContract]
	public class BalanceItemDTO : BaseDictionaryDTO
	{
        public BalanceItem BalanceItem => (BalanceItem)Id;

        [DataMember]
        public Sign BalanceSign { get; set; }

	    public int SignCoef
	    {
	        get
	        {
                switch (BalanceSign)
                {
                    case Sign.In:
                        return 1;
                    case Sign.Out:
                        return -1;
                    default:
                        return 0;
                }
	            
	        }
	    }
	}
}