using GazRouter.DTO.Dictionaries.Integro;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.DataExchange.Integro
{
    public class SessionDataParameterSet
    {
        public SessionDataParameterSet()
        {
            EntityIdList = new List<Guid>();
            PropertyList = new List<PropertyType>();

            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
            CreateEmpty = true;
            //WorkAnalyticTypeId = SettingsHelper.WorkAnalyticTypeId;
            //SystemId = SystemId.ASDU_ESG;
        }

        public string WorkAnalyticTypeId { get; set; }
        public List<Guid> EntityIdList { get; set; }

        public Guid? SummaryId { get; set; }

        public List<PropertyType> PropertyList { get; set; }

        public PeriodType PeriodType { get; set; }
        //public SystemId SystemId { get; set; }

        public int? SeriesId { get; set; }
        public List<int> ContractIds { get; set; }
        public SessionDataType SDType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }


        /// <summary>
        /// Нужно установить в truе, 
        /// если на клиенте необходимо отображать не только значение,
        /// но список ошибок или тревог по значению свойства
        /// Нужно всегда помнить, что подгрузка сообщений увеличивает размер
        /// передаваемых данных.
        /// </summary>
        //public bool LoadMessages { get; set; }


        public bool CreateEmpty { get; set; }

        public override string ToString()
        {
            return $"EntityIdList: {GetentityIdString()}, PeriodType: {PeriodType}, SeriesId: {SeriesId}, StartDate: {StartDate}, EndDate: {EndDate}, CreateEmpty: {CreateEmpty}";
        }

        private string GetentityIdString()
        {
            return EntityIdList.Count > 0 ? EntityIdList.Select(id => id.ToString()).Aggregate((a, b) => a + "," + b) : "Пусто";
        }
    }
}
