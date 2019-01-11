using System;

namespace GazRouter.DTO.DataExchange.ExchangeTask
{
    public class RunProcParameterSet 
    {
        public int TaskId { get; set; }
        public DateTime TimeStamp { get; set; }
        public string ProcedureName { get; set; }
    }
}