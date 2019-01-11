using System.Runtime.Serialization;

// Типы ремонтных работ 
namespace GazRouter.DTO.Repairs.RepairWorks
{
    [DataContract]
    public class RepairWorkDTO : BaseDto<int>
    {
		[DataMember]
		public int WorkTypeId { get; set; }

		[DataMember]
		public string WorkTypeName { get; set; }

		[DataMember]
		public int RepairId { get; set; }

		[DataMember]
    	public double? KilometerStart { get; set; }

		[DataMember]
		public double? KilometerEnd { get; set; }

		[DataMember]
        public bool IsUseWork { get; set; }
    }
}
