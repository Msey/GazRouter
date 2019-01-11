
namespace GazRouter.DTO.Balances.Irregularity
{
    public class SetIrregularityParameterSet
    {
        public int BalanceValueId { get; set; }
        public int StartDayNum { get; set; }
        public int EndDayNum { get; set; }
        public double Value { get; set; }
    }

}