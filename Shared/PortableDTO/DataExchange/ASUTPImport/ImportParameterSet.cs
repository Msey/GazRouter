using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.DTO.DataExchange.ASUTPImport
{
    public class ASUTPImportParameterSet
    {
        public DateTime Timestamp { get; set; }

        public PeriodType PeriodType { get; set; }
    }
}