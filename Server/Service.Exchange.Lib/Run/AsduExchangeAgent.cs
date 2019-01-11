using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DAL.DataExchange.ExchangeTask;
using GazRouter.DAL.SysEvents;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.TransportTypes;
using GazRouter.DTO.SysEvents;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib.Asdu;
using GazRouter.Service.Exchange.Lib.Cryptography;
using GazRouter.Service.Exchange.Lib.Export;
using GazRouter.Service.Exchange.Lib.Import;
using GazRouter.Service.Exchange.Lib.Transport;

namespace GazRouter.Service.Exchange.Lib.Run
{
    public static class AsduExchangeAgent
    {
        public static void Run(DateTime keyDate, PeriodType periodType, out string fullPath)
        {
            ExchangeObject<SpecificExchangeData> eo;
            using (
                var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                    new MyLogger("exchangeLogger")))
            {
                var exporter = new AsduExchangeObjectExporter(context, keyDate, periodType);
                var asduExchangeObject = exporter.Build();
                fullPath = Path.Combine(AppSettingsManager.AsduDirectory, $"{keyDate.ToString("yyyy.MM.dd_HH")}.asdu");
                XmlHelper.Save(asduExchangeObject, fullPath);
            }
        }


        private static bool TryReadFile(string fileName, out string result, out Exception error)
        {
            try
            {
                error = null;
                var fullPath = Path.Combine(AppSettingsManager.XslAstraDirectory, fileName);
                result = FileTools.ReadFile(fullPath);
                return true;
            }
            catch (Exception e)
            {
                result =  string.Empty;
                error = e;
                return false;
            }
        }
    }
}