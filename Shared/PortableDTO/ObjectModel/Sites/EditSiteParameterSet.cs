namespace GazRouter.DTO.ObjectModel.Sites
{
	public class EditSiteParameterSet : EditEntityParameterSet
    {
		public int GasTransportSystemId { get; set; }

	    public int? BalanceGroupId { get; set; }
    }
}
