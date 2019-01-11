using GazRouter.DTO.Dictionaries.Integro;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.Service.Exchange.Lib.AsduEsg
{

    public static class ExchangeTypeTemplates
    {
        public const string Comment1M = "Файл со значениями технологических параметров.Месяц.";
        public const string Comment24H = "Файл со значениями технологических параметров.Сутки.";
        public const string Comment2H = "Файл со значениями технологических параметров.Часовой.";
        public const string CommentP1M_PL = "Месячный. Плановые значения.";
        public const string CommentP1M_PRO = "Месячный. Производственно-отчтная информация. Отчет о работе предприятия.";
        public const string CommentP1M_PROD = "Месячный. Производственно-отчтная информация. Отчет о добычи.";
        public const string CommentP1M_F1P = "Месячный. Производственно-отчтная информация. Отчет 1П.";
        public const string Scale1MName = "P1M";
        public const string Scale24HName = "PT24H";
        public const string Scale2HName = "PT2H";
        public const string TemplateP1M_PL = "{enterpriseCode}.P1M.PL.V{VersionNo}"; //
        public const string TemplateP1M_PRO = "{enterpriseCode}.P1M.PRO.V{VersionNo}";  //
        public const string TemplateP1M_PROD = "{enterpriseCode}.P1M.PROD.V{VersionNo}"; //
        public const string TemplateP1M_F1P = "{enterpriseCode}.P1M.F1P.V{VersionNo}"; // Отчет 1П
        public const string Template24H_PL = "{enterpriseCode}.PT24H.PL.V{VersionNo}";
        public const string Template24H_RT = "{enterpriseCode}.PT24H.RT.V{VersionNo}";
        public const string Template24H_UB = "{enterpriseCode}.PT24H.UB.V{VersionNo}";
        public const string Template2H = "{enterpriseCode}.PT2H.RT.V{VersionNo}";
        public const string VersionNo = "1";
    }

    public class PeriodTypeManager
    {

        public static PeriodTypeFullInfo GetFullInfo(PeriodType pt, SessionDataType dtType)
        {
            PeriodTypeFullInfo result;
            switch (pt)
            {
                case PeriodType.Twohours:
                    result = new PeriodTypeFullInfo
                    {
                        SessionExportTimeSpan = TimeSpan.FromHours(0),

                        Scale = ExchangeTypeTemplates.Scale2HName,
                        Template = ExchangeTypeTemplates.Template2H.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString()),
                        Comment = ExchangeTypeTemplates.Comment2H,
                    };
                    break;
                case PeriodType.Day:
                    {
                        result = new PeriodTypeFullInfo
                        {
                            SessionExportTimeSpan = TimeSpan.FromDays(0),
                            Scale = ExchangeTypeTemplates.Scale24HName,
                            Comment = ExchangeTypeTemplates.Comment24H,
                        };
                        switch (dtType)
                        {
                            case SessionDataType.UB:
                                result.Template = ExchangeTypeTemplates.Template24H_UB.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString());
                                break;
                            case SessionDataType.PL:
                                result.Template = ExchangeTypeTemplates.Template24H_PL.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString());
                                break;
                            case SessionDataType.RT:
                                result.Template = ExchangeTypeTemplates.Template24H_RT.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString());
                                break;
                            default:
                                result.Template = ExchangeTypeTemplates.Template24H_RT.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString());
                                break;
                        }
                    }
                    break;
                case PeriodType.Month:
                    result = new PeriodTypeFullInfo
                    {
                        SessionExportTimeSpan = TimeSpan.FromDays(30),
                        Scale = ExchangeTypeTemplates.Scale1MName,
                        //Template = ExchangeTypeTemplates.Template1M.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString()),
                        Comment = ExchangeTypeTemplates.Comment1M,
                    };
                    switch (dtType)
                    {
                        case SessionDataType.PRO:
                            result.Template = ExchangeTypeTemplates.TemplateP1M_PRO.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString());
                            break;
                        case SessionDataType.PL:
                            result.Template = ExchangeTypeTemplates.TemplateP1M_PL.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString());
                            break;
                        case SessionDataType.PROD:
                            result.Template = ExchangeTypeTemplates.TemplateP1M_PROD.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString());
                            break;
                        case SessionDataType.F1P:
                            result.Template = ExchangeTypeTemplates.TemplateP1M_F1P.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString());
                            break;
                        default:
                            result.Template = ExchangeTypeTemplates.TemplateP1M_PRO.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString());
                            break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("pt", "Не корректный PeriodType");
            }
            result.DataType = dtType.ToString();
            result.Version = "1";
            return result;
        }

        public static string GetStringScale(PeriodType pt)
        {
            switch (pt)
            {
                case PeriodType.Twohours:
                    return ExchangeTypeTemplates.Scale2HName;

                case PeriodType.Day:
                    return ExchangeTypeTemplates.Scale24HName;
                case PeriodType.Month:
                    return ExchangeTypeTemplates.Scale1MName;
                default:
                    throw new ArgumentOutOfRangeException("pt", "Не корректный PeriodType");
            }
        }

        public static string GetStringComment(PeriodType pt)
        {
            switch (pt)
            {
                case PeriodType.Twohours:
                    return ExchangeTypeTemplates.Comment2H;

                case PeriodType.Day:
                    return ExchangeTypeTemplates.Comment24H;
                case PeriodType.Month:
                    return ExchangeTypeTemplates.Comment1M;
                default:
                    throw new ArgumentOutOfRangeException("pt", "Не корректный PeriodType");
            }
        }

        public static string GetTemplate(PeriodType pt, string enterpriseCode, SessionDataType dtType)
        {
            switch (pt)
            {
                case PeriodType.Twohours:
                    return ExchangeTypeTemplates.Template2H.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo).Replace("{enterpriseCode}", enterpriseCode);

                case PeriodType.Day:
                    {
                        string result = string.Empty;
                        switch (dtType)
                        {
                            case SessionDataType.UB:
                                result = ExchangeTypeTemplates.Template24H_UB.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo).Replace("{enterpriseCode}", enterpriseCode);
                                break;
                            case SessionDataType.PL:
                                result = ExchangeTypeTemplates.Template24H_PL.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo).Replace("{enterpriseCode}", enterpriseCode);
                                break;
                            case SessionDataType.RT:
                                result = ExchangeTypeTemplates.Template24H_RT.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo).Replace("{enterpriseCode}", enterpriseCode);
                                break;
                            default:
                                result = ExchangeTypeTemplates.Template24H_RT.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo).Replace("{enterpriseCode}", enterpriseCode);
                                break;
                        }
                        return result;
                        //return ExchangeTypeTemplates.Template24H_RT.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo).Replace("{enterpriseCode}", enterpriseCode);
                    }
                case PeriodType.Month:
                    {
                        string result = string.Empty;
                        switch (dtType)
                        {
                            case SessionDataType.PRO:
                                result = ExchangeTypeTemplates.TemplateP1M_PRO.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString()).Replace("{enterpriseCode}", enterpriseCode);
                                break;
                            case SessionDataType.PL:
                                result = ExchangeTypeTemplates.TemplateP1M_PL.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString()).Replace("{enterpriseCode}", enterpriseCode);
                                break;
                            case SessionDataType.PROD:
                                result = ExchangeTypeTemplates.TemplateP1M_PROD.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString()).Replace("{enterpriseCode}", enterpriseCode);
                                break;
                            case SessionDataType.F1P:
                                result = ExchangeTypeTemplates.TemplateP1M_F1P.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString()).Replace("{enterpriseCode}", enterpriseCode);
                                break;
                            default:
                                result = ExchangeTypeTemplates.TemplateP1M_PRO.Replace("{VersionNo}", ExchangeTypeTemplates.VersionNo.ToString());
                                break;
                        }
                        return result;
                    }                  
                    
                default:
                    throw new ArgumentOutOfRangeException("pt", "Не корректный PeriodType");
            }
        }


        public static string GetLogName(PeriodType pt)
        {
            return null;
        }

        public static SessionDataType GetDataTypeByString(string dtTypeStr)
        {
            if (string.IsNullOrEmpty(dtTypeStr))
                return SessionDataType.RT;
            switch (dtTypeStr)
            {
                case "RT":
                    return SessionDataType.RT;
                case "UB":
                    return SessionDataType.UB;
                case "PL":
                    return SessionDataType.PL;
                case "NSI":
                    return SessionDataType.NSI;
            }
            return SessionDataType.RT;
        }

        public static TimeSpan GetTimeSpan(PeriodType pt)
        {
            switch (pt)
            {
                case PeriodType.Twohours:
                case PeriodType.Day:
                    return TimeSpan.FromDays(0);
                case PeriodType.Month:
                    return TimeSpan.FromDays(30);
                default:
                    throw new ArgumentOutOfRangeException("pt", "Не корректный PeriodType");
            }
        }
    }

    public class PeriodTypeFullInfo
    {
        public TimeSpan SessionExportTimeSpan { get; set; }
        public string Scale { get; set; }
        public string Template { get; set; }
        public string Comment { get; set; }
        public string LogName { get; set; }
        public string Code { get; set; }
        public string DataType { get; set; }
        public string Version { get; set; }
    }
}
