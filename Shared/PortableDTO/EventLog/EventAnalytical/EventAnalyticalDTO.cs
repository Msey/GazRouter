using System.Runtime.Serialization;

namespace GazRouter.DTO.EventLog.EventAnalytical
{
	[DataContract]
    public class EventAnalyticalDTO
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
        public int Max { get; set; }

		[DataMember]
        public int Avg { get; set; }

		[DataMember]
        public int Total { get; set; }


	}
}
