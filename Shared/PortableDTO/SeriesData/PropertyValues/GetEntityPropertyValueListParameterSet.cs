using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.DTO.SeriesData.PropertyValues
{
    public class GetEntityPropertyValueListParameterSet
    {
        public GetEntityPropertyValueListParameterSet()
        {
            EntityIdList = new List<Guid>();
            PropertyList = new List<PropertyType>();

            CreateEmpty = true;
        }

        public List<Guid> EntityIdList { get; set; }

        public List<PropertyType> PropertyList { get; set; }

        public PeriodType PeriodType { get; set; }


        public int? SeriesId { get; set; }


        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }


        // Год, месяц, день используется для получения суточных данных,
        // т.к. при передачи свойства типа дата происходит автоматическое преобразование
        // с приведением часовых поясов
        public int? Year { get; set; }

        public int? Month { get; set; }

        public int? Day { get; set; }



        /// <summary>
        /// Нужно установить в truе, 
        /// если на клиенте необходимо отображать не только значение,
        /// но список ошибок или тревог по значению свойства
        /// Нужно всегда помнить, что подгрузка сообщений увеличивает размер
        /// передаваемых данных.
        /// </summary>
        public bool LoadMessages { get; set; }


        /// <summary>
        /// Если значения 
        /// </summary>
        public bool CreateEmpty { get; set; }



        public override string ToString()
        {
            return $"EntityIdList: {GetEntityIdString()}, PeriodType: {PeriodType}, SeriesId: {SeriesId}, StartDate: {StartDate}, EndDate: {EndDate}, LoadMessages: {LoadMessages}, CreateEmpty: {CreateEmpty}";
        }

        private string GetEntityIdString()
        {
          return  EntityIdList?.Count > 0 ? EntityIdList.Select(id => id.ToString()).Aggregate((a, b) => a + "," + b) : "Пусто";
        }
    }
}
