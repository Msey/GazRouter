using GazRouter.DTO.SysEvents;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using Utils.Extensions;

namespace GazRouter.DataServices.Infrastructure
{
    public static class AppSettingsManager
    {
        private static readonly Lazy<string> _connectionString =
            new Lazy<string>(() => ConfigurationManager.AppSettings["MainDb"]);

        public static string ConnectionString
        {
            get { return _connectionString.Value; }
        }

        public static int DispatherDayStartHour { get { return _dispatherDayStartHour.Value; } }


        public static int ServerTimeUtcOffset => (int)TimeZoneInfo.Local.BaseUtcOffset.TotalHours;


        private static readonly Lazy<int> _dispatherDayStartHour =
            new Lazy<int>(() =>
            {
                var dispatherDayStartHour = 10;

                var set = ConfigurationManager.AppSettings["dispatherDayStartHour"];
                if (!String.IsNullOrEmpty(set))
                    dispatherDayStartHour = Int32.Parse(set);
                return dispatherDayStartHour;
            });

        private static readonly Lazy<bool> _checkClientVersion =
            new Lazy<bool>(() =>
            {
                var checkClientVersion = true;

                var set = ConfigurationManager.AppSettings["checkClientVersion"];
                if (!String.IsNullOrEmpty(set))
                    checkClientVersion = Boolean.Parse(set);
                return checkClientVersion;
            });

        public static bool CheckClientVersion { get { return _checkClientVersion.Value; } }

        private static readonly Lazy<string> _asduExchangeDirectory =
            new Lazy<string>(() =>
                             {
                                 var dir = Path.Combine(AppSettingsManager.ExchangeDirectory, "Asdu");
                                 if (!Directory.Exists(dir))
                                 {
                                     Directory.CreateDirectory(dir);
                                 }
                                 return dir;
                             }
            );

        public static string AsduExchangeDirectory
        {
            get
            {
                return _asduExchangeDirectory.Value;
            }
        }

        public static string ExchangeDirectory { get { return _exchangeDirectory.Value; } }
        private static readonly Lazy<string> _exchangeDirectory = new Lazy<string>(() =>
        {
            var dir = Path.Combine(ConfigurationManager.AppSettings["rootDirectory"], "Exchange");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        });

        public static string AsduExchangeArchiveDirectory { get { return _asduExchangeArchiveDirectory.Value; } }
        private static readonly Lazy<string> _asduExchangeArchiveDirectory = new Lazy<string>(() =>
        {
            var dir = ConfigurationManager.AppSettings["asduExchangeArchiveDirectory"];
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        });

        public static string ExcelGeneratorTemplateDirectory { get { return _excelGeneratorTemplateDirectory.Value; } }
        private static readonly Lazy<string> _excelGeneratorTemplateDirectory = new Lazy<string>(() =>
        {
            var dir = Path.Combine((ConfigurationManager.GetSection("excelGeneratorService") as NameValueCollection)["TemplateDirectory"], "");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        });

        public static string ExcelGeneratorOutputDirectory { get { return _excelGeneratorOutputDirectory.Value; } }
        private static readonly Lazy<string> _excelGeneratorOutputDirectory = new Lazy<string>(() =>
        {
            var dir = Path.Combine((ConfigurationManager.GetSection("excelGeneratorService") as NameValueCollection)["OutputDirectory"], "");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        });


        private static readonly Lazy<List<string>> _excelGeneratorInputName = new Lazy<List<string>>(() => GetSettingsList("InputName"));

        public static List<string> ExcelGeneratorInputName { get { return _excelGeneratorInputName.Value; } }

        private static readonly Lazy<List<string>> _excelGeneratorOutputName = new Lazy<List<string>>(() => GetSettingsList("OutputName"));

        public static List<string> ExcelGeneratorOutputName { get { return _excelGeneratorOutputName.Value; } }

        private static readonly Lazy<List<string>> _excelGeneratorQueryString = new Lazy<List<string>>(() => GetSettingsList("QueryString"));

        public static List<string> ExcelGeneratorQueryString { get { return _excelGeneratorQueryString.Value; } }

        private static List<string> GetSettingsList(string prefix)
        {
            List<string> tempSettingsList = new List<string>();
            for (int postfixIndex = 1; (ConfigurationManager.GetSection("excelGeneratorService") as NameValueCollection)[prefix + postfixIndex] != null; postfixIndex++)
            {
                tempSettingsList.Add((ConfigurationManager.GetSection("excelGeneratorService") as NameValueCollection)[prefix + postfixIndex]);
            }
            return tempSettingsList;
        }

        public static string XslAstraDirectory { get { return _xslAstraDirectory.Value; } }
        private static readonly Lazy<string> _xslAstraDirectory = new Lazy<string>(() =>
        {
            var dir = Path.Combine(ExchangeDirectory, "XslAstra");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        });

        public static string AsduDirectory { get { return _asduDirectory.Value; } }
        private static readonly Lazy<string> _asduDirectory = new Lazy<string>(() =>
        {
            var dir = Path.Combine(ExchangeDirectory, "Asdu");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        });

        public static string ExchangeArchiveDirectory { get { return _archiveDirectory.Value; } }
        private static readonly Lazy<string> _archiveDirectory = new Lazy<string>(() =>
        {
            var dir = Path.Combine(ExchangeDirectory, "Archive");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        });
        public static string ExchangeImportDirectory { get { return _exchangeImportDirectory.Value; } }
        private static readonly Lazy<string> _exchangeImportDirectory = new Lazy<string>(() =>
        {
            var dir = Path.Combine(ExchangeDirectory, "Import");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        });


        public static Guid CurrentEnterpriseId { get { return _currentEnterpriseId.Value; } }
        private static readonly Lazy<Guid> _currentEnterpriseId = new Lazy<Guid>(() =>
        {
            var rawGuid = ConfigurationManager.AppSettings["currentEnterpriseId"];
            return Guid.Parse(rawGuid).Convert();
        });

        public static List<string> AdminLogins { get { return _adminLogins.Value; } }
        private static readonly Lazy<List<string>> _adminLogins = new Lazy<List<string>>(() =>
        {
            var strLogins = ConfigurationManager.AppSettings["adminLogins"];
            return String.IsNullOrEmpty(strLogins) ? new List<string>() : strLogins.Replace(" ", String.Empty).Split(';').ToList();
        });

        public static string ServerAssemblyVersion { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        public static DateTime ServerAssemblyDate 
        { 
            get
            {
                const string orgSourceValue = @"ClientBin/Client.xap";
                var xappath = HttpContext.Current.Server.MapPath("") + @"\" + orgSourceValue;
                return File.GetLastWriteTime(xappath);
            }
        }

        private static readonly Lazy<string> _activeDirectory = new Lazy<string>(() => ConfigurationManager.AppSettings["activeDirectory"]);
        public static string ActiveDirectory { get { return _activeDirectory.Value; } }


        private static readonly Lazy<string> _emailSmtpHost = new Lazy<string>(() => ConfigurationManager.AppSettings["emailSmtpHost"]);
        public static string EmailSmtpHost { get { return _emailSmtpHost.Value; } }

        private static readonly Lazy<string> _emailPop3Host = new Lazy<string>(() => ConfigurationManager.AppSettings["emailPop3Host"]);
        public static string EmailPop3Host { get { return _emailPop3Host.Value; } }

        private static readonly Lazy<string> _emailPassword = new Lazy<string>(() => ConfigurationManager.AppSettings["emailPassword"]);
        public static string EmailPassword { get { return _emailPassword.Value; } }

        private static readonly Lazy<string> _emailLogin = new Lazy<string>(() => ConfigurationManager.AppSettings["emailLogin"]);
        public static string EmailLogin { get { return _emailLogin.Value; } }

        private static readonly Lazy<string> _emailSystemAddress = new Lazy<string>(() => ConfigurationManager.AppSettings["emailSystemAddress"]);
        public static string EmailSystemAddress { get { return _emailSystemAddress.Value; } }

        private static readonly Lazy<string> _exchangeServerUrl = new Lazy<string>(() => ConfigurationManager.AppSettings["exchangeServerUrl"]);
        public static string ExchangeServerUrl { get{return _exchangeServerUrl.Value;}}

        private static readonly Lazy<string> _sapBoUrl = new Lazy<string>(() => ConfigurationManager.AppSettings["sapBoUrl"]);
        public static string SapBoUrl{ get { return _sapBoUrl.Value; } }

        private static readonly Lazy<string> _emailSubjectFlag = new Lazy<string>(() => ConfigurationManager.AppSettings["emailSubjectFlag"]);
        public static string EmailSubjectFlag { get { return _emailSubjectFlag.Value; } }

        private static readonly Lazy<TimeSpan> _asduTimerInterval = new Lazy<TimeSpan>(() => TimeSpan.FromMinutes(int.Parse(ConfigurationManager.AppSettings["asduTimerInterval"])));
        public static TimeSpan AsduTimerInterval { get { return _asduTimerInterval.Value; }  }

        private static readonly Lazy<TimeSpan> _transportTimerInterval = new Lazy<TimeSpan>(() => TimeSpan.FromMinutes(int.Parse(ConfigurationManager.AppSettings["transportTimerInterval"])));
        public static TimeSpan TransportTimerInterval { get { return _transportTimerInterval.Value; } }

        private static readonly Lazy<TimeSpan> _exportTimerInterval = new Lazy<TimeSpan>(() => TimeSpan.FromMinutes(int.Parse(ConfigurationManager.AppSettings["exportTimerInterval"])));
        public static TimeSpan ExportTimerInterval { get { return _exportTimerInterval.Value; } }

        //private static readonly Lazy<SysEventType?> _asduEsgExportEvent = new Lazy<SysEventType?>(() => (SysEventType?)int.Parse(ConfigurationManager.AppSettings["asduEsgExportEvent"]));
        //public static SysEventType? AsduEsgExportEvent { get { return _asduEsgExportEvent.Value; } }

        private static readonly Lazy<bool> _asduEsgExportRun = new Lazy<bool>(() => bool.Parse((ConfigurationManager.AppSettings["asduEsgExportRun"])));
        public static bool AsduEsgExportRun { get { return _asduEsgExportRun.Value; } }

        private static readonly Lazy<bool> _asspootiExportRun = new Lazy<bool>(() => bool.Parse((ConfigurationManager.AppSettings["asspootiExportRun"])));
        public static bool AsspootiExportRun { get { return _asspootiExportRun.Value; } }

        private static readonly Lazy<int> _asduEsg2HOffset = new Lazy<int>(() => int.Parse((ConfigurationManager.AppSettings["asduEsg2HOffset"])));
        public static int AsduEsg2HOffset { get { return _asduEsg2HOffset.Value; } }

        private static readonly Lazy<int> _asspooti2HOffset = new Lazy<int>(() => int.Parse((ConfigurationManager.AppSettings["asspooti2HOffset"])));
        public static int Asspooti2HOffset { get { return _asspooti2HOffset.Value; } }

        private static readonly Lazy<bool> _convertDateTime = new Lazy<bool>(() => bool.Parse((ConfigurationManager.AppSettings["convertDateTime"])));
        public static bool СonvertDateTime { get { return _convertDateTime.Value; } }

        private static readonly Lazy<int> _correctMoscowZone = new Lazy<int>(() => int.Parse((ConfigurationManager.AppSettings["correctMoscowZone"])));
        public static int CorrectMoscowZone { get { return _correctMoscowZone.Value; } }

        private static readonly Lazy<bool> _sessionExportDimension = new Lazy<bool>(() => bool.Parse((ConfigurationManager.AppSettings["sessionExportDimension"])));
        public static bool SessionExportDimension { get { return _sessionExportDimension.Value; } }

        private static readonly Lazy<bool> _parameterFullName = new Lazy<bool>(() => bool.Parse((ConfigurationManager.AppSettings["sessionExportParameterFullName"])));
        public static bool ParameterFullName { get { return _parameterFullName.Value; } }

        private static readonly Lazy<bool> _sessionValidateAfterExport = new Lazy<bool>(() => bool.Parse((ConfigurationManager.AppSettings["sessionValidateAfterExport"])));
        public static bool SessionValidateAfterExport { get { return _sessionValidateAfterExport.Value; } }

        #region Dictionaries Settings
        private static readonly Lazy<SettingDictionary> _unitDictionary = new Lazy<SettingDictionary>(() => new SettingDictionary("dictionaries/dimension")); 
        public static SettingDictionary UnitDictionary { get { return _unitDictionary.Value; } }

        private static readonly Lazy<SettingDictionary> _validationSchemeDictionary = new Lazy<SettingDictionary>(() => new SettingDictionary("dictionaries/ius-validation"));
        public static SettingDictionary ValidationSchemeDictionary { get { return _validationSchemeDictionary.Value; } }

        private static readonly Lazy<SettingDictionary> _externalSystemDir = new Lazy<SettingDictionary>(() => new SettingDictionary("dictionaries/externalsystem-dir"));
        public static SettingDictionary ExternalSystemDir { get { return _externalSystemDir.Value; } }
        #endregion

        private static readonly Lazy<string> _helpFileRelativeUri = new Lazy<string>(() => ConfigurationManager.AppSettings["helpFile"]);
        public static string HelpFileRelativeUri { get { return _helpFileRelativeUri.Value; } }

        private static readonly Lazy<string> _lastChangesFileRelativeUri = new Lazy<string>(() => ConfigurationManager.AppSettings["lastChangesFile"]);
        public static string LastChangesFileRelativeUri { get { return _lastChangesFileRelativeUri.Value; } }

        private static readonly Lazy<SettingDictionary> _asduSoapConfig = new Lazy<SettingDictionary>(() => new SettingDictionary("asduSoap"));
        public static SettingDictionary AsduSoapConfig { get { return _asduSoapConfig.Value; } }
        
        public static string EventExchangeAgentUrl { get { return _xmlGeneratorUrl.Value; } }
        private static readonly Lazy<string> _xmlGeneratorUrl = new Lazy<string>(() =>
        {
            try
            {
                return (ConfigurationManager.GetSection("eventExchangeAgent") as NameValueCollection)["url"];
            } 
            catch(Exception)
            {
                return null;
            }
        });

        public static string EventExchangeAgentUser { get { return _xmlGeneratorUser.Value; } }
        private static readonly Lazy<string> _xmlGeneratorUser = new Lazy<string>(() =>
        {
            try
            {
                return (ConfigurationManager.GetSection("eventExchangeAgent") as NameValueCollection)["user"];
            }
            catch (Exception)
            {
                return null;
            }

        });

        public static string EventExchangeAgentPassword { get { return _xmlGeneratorPassword.Value; } }
        private static readonly Lazy<string> _xmlGeneratorPassword = new Lazy<string>(() =>
        {
            try
            {
                return (ConfigurationManager.GetSection("eventExchangeAgent") as NameValueCollection)["password"];
            }
            catch (Exception)
            {
                return null;
            }
        });

        public static string EventExchangeAgentSoap { get { return _xmlGeneratorSoap.Value; } }
        private static readonly Lazy<string> _xmlGeneratorSoap = new Lazy<string>(() =>
        {
            try
            { 
            return (ConfigurationManager.GetSection("eventExchangeAgent") as NameValueCollection)["soap"];
            }
            catch (Exception)
            {
                return null;
            }
        });

        public static string EventExchangeAgentEnc { get { return _xmlGeneratorEnc.Value; } }
        private static readonly Lazy<string> _xmlGeneratorEnc = new Lazy<string>(() =>
        {
            try
            {
                return (ConfigurationManager.GetSection("eventExchangeAgent") as NameValueCollection)["enc"];
            }
            catch (Exception)
            {
                return null;
            }

        });

        public static string EventExchangeAgentAction { get { return _xmlGeneratorAction.Value; } }
        private static readonly Lazy<string> _xmlGeneratorAction = new Lazy<string>(() =>
        {
            try
            {
                return (ConfigurationManager.GetSection("eventExchangeAgent") as NameValueCollection)["action"];
            }
            catch (Exception)
            {
                return null;
            }
        });
    }
}