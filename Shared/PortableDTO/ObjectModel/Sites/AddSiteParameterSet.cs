using System;

namespace GazRouter.DTO.ObjectModel.Sites
{
	public class AddSiteParameterSet : AddEntityParameterSet
    {
		public int GasTransportSystemId { get; set; }
	    public Guid? Id { get; set; }

	    public int? BalanceGroupId { get; set; }
    }
}
