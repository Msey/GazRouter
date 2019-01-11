using System;

namespace GazRouter.DTO.DataExchange.ExchangeLog
{
    public class AddEditExchangeLogParameterSet
    {
        public int ExchangeTaskId { get; set; }
        public int? SeriesId { get; set; }
        public bool IsOk { get; set; }
        public string Content { get; set; }
        public string Error { get; set; }
    }
}