using System.Runtime.Serialization;

namespace GazRouter.DTO.Balances.Routes.Exceptions
{
    [DataContract]
	public class RouteExceptionDTO
    {
        [DataMember]
        public int RouteExceptionId { get; set; }

        [DataMember]
        public int RouteId { get; set; }


        [DataMember]
        public int OwnerId { get; set; }

        [DataMember]
        public string OwnerName { get; set; }


        [DataMember]
        public double Length { get; set; }
	}
}