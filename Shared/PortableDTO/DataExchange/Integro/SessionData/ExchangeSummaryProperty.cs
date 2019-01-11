using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazRouter.DTO.DataExchange.Integro.SessionData
{
    public class ExchangeSummaryProperty
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UnitName { get; set; }
        public string Description { get; set; }
        public string ShortName { get; set; }
        public string SystemName { get; set; }
        public Guid? ParameterGid { get; set; }
    }
}
