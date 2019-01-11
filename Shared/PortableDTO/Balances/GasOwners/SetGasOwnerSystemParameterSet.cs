namespace GazRouter.DTO.Balances.GasOwners
{
    public class SetGasOwnerSystemParameterSet
    {
        public int GasOwnerId { get; set; }

        public int SystemId { get; set; }

        public bool IsActive { get; set; }
    }
}