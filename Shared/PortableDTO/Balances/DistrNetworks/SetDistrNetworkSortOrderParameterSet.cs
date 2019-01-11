namespace GazRouter.DTO.Balances.DistrNetworks
{
    public class SetDistrNetworkSortOrderParameterSet
    {
        public int DistrNetworkId { get; set; }
        public UpOrDownSortOrder UpDown { get; set; }
    }

    public enum UpOrDownSortOrder
    {
        Up = 1,
        Down = 2
    }

}