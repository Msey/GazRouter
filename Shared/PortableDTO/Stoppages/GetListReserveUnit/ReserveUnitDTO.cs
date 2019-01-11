using System;
using System.Runtime.Serialization;

namespace GazRouter.DTO.Stoppages.GetListReserveUnit
{
    [DataContract]
    public class ReserveUnitDTO
    {
        [DataMember]
        public int StoppageId { get; set; }

        [DataMember]
        public Guid UnitId { get; set; }

        [DataMember]
        public string UnitName { get; set; }
    }
}
