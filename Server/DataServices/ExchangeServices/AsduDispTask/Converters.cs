using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GazRouter.DataServices.ExchangeServices.AsduDispTask
{
    public class Converters
    {
        public static DTO.Dictionaries.PeriodTypes.PeriodType PeriodTypes(scaleType scale)
        {
            switch (scale)
            {
                case scaleType.PT2H:  return DTO.Dictionaries.PeriodTypes.PeriodType.Twohours;
                case scaleType.PT24H: return DTO.Dictionaries.PeriodTypes.PeriodType.Day;
                case scaleType.P1M:   return DTO.Dictionaries.PeriodTypes.PeriodType.Month;
                case scaleType.P1Q:   return DTO.Dictionaries.PeriodTypes.PeriodType.Quarter;
                case scaleType.P1Y:   return DTO.Dictionaries.PeriodTypes.PeriodType.Year;
                //case scaleType.P1M: return DTO.Dictionaries.PeriodTypes.PeriodType.Month; 
                //case scaleType.P1Q: return DTO.Dictionaries.PeriodTypes.PeriodType.Quarter; 
                //case scaleType.P1Y: return DTO.Dictionaries.PeriodTypes.PeriodType.Year; 
                //case scaleType.PACT: return DTO.Dictionaries.PeriodTypes.PeriodType.None; 
                //case scaleType.PT24H: return DTO.Dictionaries.PeriodTypes.PeriodType.Day; 
                //case scaleType.PT2H: return DTO.Dictionaries.PeriodTypes.PeriodType.Twohours;
                ////case scaleType.PT5M: return DTO.Dictionaries.PeriodTypes.PeriodType.FiveDays;
                default: return DTO.Dictionaries.PeriodTypes.PeriodType.None;
            }
        }

        public static scaleType PeriodTypes(DTO.Dictionaries.PeriodTypes.PeriodType scale)
        {
            switch (scale)
            {
                case DTO.Dictionaries.PeriodTypes.PeriodType.Twohours: return scaleType.PT2H;
                case DTO.Dictionaries.PeriodTypes.PeriodType.Day:      return scaleType.PT24H;
                case DTO.Dictionaries.PeriodTypes.PeriodType.Month:    return scaleType.P1M;
                case DTO.Dictionaries.PeriodTypes.PeriodType.Quarter:  return scaleType.P1Q;
                case DTO.Dictionaries.PeriodTypes.PeriodType.Year:     return scaleType.P1Y;
                default: return scaleType.PACT;

                //case DTO.Dictionaries.PeriodTypes.PeriodType.Month: return scaleType.P1M;
                //case DTO.Dictionaries.PeriodTypes.PeriodType.Quarter: return scaleType.P1Q;
                //case DTO.Dictionaries.PeriodTypes.PeriodType.Year: return scaleType.P1Y;
                //case DTO.Dictionaries.PeriodTypes.PeriodType.None: return scaleType.PACT;
                //case DTO.Dictionaries.PeriodTypes.PeriodType.Day: return scaleType.PT24H;
                //case DTO.Dictionaries.PeriodTypes.PeriodType.Twohours: return scaleType.PT2H;
                ////case DTO.Dictionaries.PeriodTypes.PeriodType.FiveDays: return scaleType.PT5M;
                //default: return scaleType.PACT;
            }
        }
    }
}
