
namespace GazRouter.DTO.Repairs.RepairWorks
{

	public class RepairWorkParameterSet
    {
		public int WorkTypeId { get; set; }
		public int RepairId { get; set; }
		public double? KilometerStart { get; set; }
		public double? KilometerEnd { get; set; }
    }
}