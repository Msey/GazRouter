
using System;

namespace GazRouter.DTO.DataExchange.ExchangeLog
{
    public class GetExchangeLogParameterSet
    {
        public GetExchangeLogParameterSet()
        {
            StartDate = DateTime.Now.AddDays(-1);
            EndDate = DateTime.Now;
            ExchangeTaskId = null;
        }

        public int? ExchangeTaskId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? SerieId { get; set; }
        /// <summary>
        /// Если признак установлен то получать содержимое контейнера
        /// Будет работать только если задан LogId (единственное значение)
        /// </summary>
        public bool GetDataContent { get; set; }
    }
}