using GazRouter.DTO.Dictionaries.Integro;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.Service.Exchange.Lib.AsduEsg.ExchangeFormat
{
    public static class PeriodExportAsduHelper
    {
        public static DateTime GetPeriodStartDate(this PeriodType periodType, DateTime endDate)
        {
            if (periodType == PeriodType.Month)
                return endDate.AddMonths(-1);
            //начало и конец периода должны быть одинаковые
            return endDate;
        }

        public static string ScaleName(this PeriodType periodType)
        {
            string scaleName;
            string comment;
            periodType.Info(out scaleName, out comment);
            return scaleName;
        }

        public static string Comment(this PeriodType periodType)
        {
            string scaleName;
            string comment;
            periodType.Info(out scaleName, out comment);
            return comment;
        }

        public static string LogName(this PeriodType periodType)
        {
            string scaleName;
            string comment;
            periodType.Info(out scaleName, out comment);
            return $"{scaleName}_log";
        }

        public static string Template(this PeriodType periodType, string enterpriseCode, SessionDataType dtType)
        {
            string scaleName;
            string comment;
            string template;
            periodType.Info(dtType, out scaleName, out comment, out template);
            return template.Replace("{enterpriseCode}", enterpriseCode);
        }

        //public static DateTime CombinePeriod(this PeriodType periodType, DateTime date, PeriodTypeDetail detailPeriod)
        //{
        //    return periodType != PeriodType.Twohours ? date : date.AddHours((int)detailPeriod);
        //}

        //public static List<PeriodType> GetPeriods4SessionExchange()
        //{
        //    return new List<PeriodType>
        //    {
        //        PeriodType.Twohours,
        //        PeriodType.Day,
        //        PeriodType.Month,
        //    };
        //}


        /// <summary>
        /// Получаем информацию по указанному периоду
        /// </summary>
        /// <param name="periodType">тип периода обмена</param>
        /// <param name="scaleName">наименование шкалы для выгрузки</param>
        /// <param name="comment">коментарий</param>
        /// <param name="template">шаблон</param>
        private static void Info(this PeriodType periodType, out string scaleName, out string comment)
        {
            switch (periodType)
            {
                case PeriodType.Twohours:
                    scaleName = ExchangeTypeTemplates.Scale2HName;
                    comment = ExchangeTypeTemplates.Comment2H;
                    break;
                case PeriodType.Day:
                    scaleName = ExchangeTypeTemplates.Scale24HName;
                    comment = ExchangeTypeTemplates.Comment24H;
                    break;
                case PeriodType.Month:
                    scaleName = ExchangeTypeTemplates.Scale1MName;
                    comment = ExchangeTypeTemplates.Comment1M;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("periodType", "Не корректный PeriodType");
            }
        }

        private static void Info(this PeriodType periodType, SessionDataType dtType, out string scaleName, out string comment, out string template)
        {
            Info(periodType, out scaleName, out comment);
            switch (periodType)
            {
                case PeriodType.Twohours:
                    template = ExchangeTypeTemplates.Template2H.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo);
                    break;
                case PeriodType.Day:
                    {
                        switch (dtType)
                        {
                            case SessionDataType.UB:
                                template = ExchangeTypeTemplates.Template24H_UB.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo);
                                break;
                            case SessionDataType.PL:
                                template = ExchangeTypeTemplates.Template24H_PL.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo);
                                break;
                            case SessionDataType.RT:
                                template = ExchangeTypeTemplates.Template24H_RT.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo);
                                break;
                            default:
                                template = ExchangeTypeTemplates.Template24H_RT.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo);
                                break;
                        };
                    }
                    break;

                case PeriodType.Month:
                    {
                        switch (dtType)
                        {
                            case SessionDataType.PRO:
                                template = ExchangeTypeTemplates.TemplateP1M_PRO.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString());
                                break;
                            case SessionDataType.PL:
                                template = ExchangeTypeTemplates.TemplateP1M_PL.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString());
                                break;
                            case SessionDataType.PROD:
                                template = ExchangeTypeTemplates.TemplateP1M_PROD.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString());
                                break;
                            case SessionDataType.F1P:
                                template = ExchangeTypeTemplates.TemplateP1M_F1P.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString());
                                break;
                            default:
                                template = ExchangeTypeTemplates.TemplateP1M_PRO.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString());
                                break;
                        }
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException("periodType", "Не корректный PeriodType");
            }
        }

    }
}
