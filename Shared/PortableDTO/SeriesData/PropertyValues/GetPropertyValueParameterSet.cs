using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.SeriesData.PropertyValues
{
    public class GetPropertyValueParameterSet
    {
        public Guid EntityId { get; set; }
        public PropertyType PropertyTypeId { get; set; }
        public DateTime? Timestamp { get; set; }
        public PeriodType PeriodTypeId { get; set; }

        public int? SeriesId { get; set; }


        /// <summary>
        /// Нужно установить в truе, 
        /// если на клиенте необходимо отображать не только значение,
        /// но список ошибок или тревог по значению свойства
        /// Нужно всегда помнить, что подгрузка сообщений увеличивает размер
        /// передаваемых данных.
        /// </summary>
        public bool LoadMessages { get; set; }
	}
}
