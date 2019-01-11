namespace GazRouter.DTO.Balances.GasOwners
{
    public class SetGasOwnerSortOrderParameterSet
    {
        public int GasOwnerId { get; set; }
        public UpOrDownSortOrder UpDown { get; set; }
    }

    public enum UpOrDownSortOrder
    {
        Up = 1,
        Down = 2
    }

}