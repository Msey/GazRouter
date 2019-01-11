using System;

namespace GazRouter.DTO.Infrastructure.Faults
{
    public class FaultDetail
    {
        public Guid LogRecordId { get; set; }

        public FaultType FaultType { get; set; }

        public string Message { get; set; }
    
    }
}