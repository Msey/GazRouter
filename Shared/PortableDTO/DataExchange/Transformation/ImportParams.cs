using System;
using GazRouter.DTO.DataExchange.ExchangeTask;

namespace GazRouter.DTO.DataExchange.Transformation
{
    public class ImportParams
    {
        public string Text { get; set; }
        public ExchangeTaskDTO Task { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string FileName { get; set; }
        public string Transformation { get; set; }
    }
}