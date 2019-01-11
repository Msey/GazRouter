
using System.Runtime.Serialization;
using GazRouter.DTO.Dictionaries.Enterprises;

namespace GazRouter.DTO.DataExchange.ExchangeTask
{
    [DataContract]
    public class NeighbourEnterpriseExchangeTask : EnterpriseDTO
    {
        [DataMember]
        public int? ExchangeTaskId { get; set; }
    }
}


