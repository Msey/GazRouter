using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib.Cryptography;
using GazRouter.Service.Exchange.Lib.Transport;

namespace GazRouter.Service.Exchange.Lib.Run
{
    public class ImportAgent
    {
        private static readonly MyLogger Logger = new MyLogger("exchangeLogger");

        private static readonly Lazy<ImportAgent> Instance = new Lazy<ImportAgent>(() => new ImportAgent());


        private ImportAgent()
        {
        }

        public static void RunSpecific()
        {
            Instance.Value._RunSpecific();
        }


        public static void RunTypical()
        {
            Instance.Value._RunTypical();
        }




        private void _RunSpecific()
        {
            try
            {
                var folder = ExchangeHelper.EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "Import", "Specific");
                var messages = EmailTransport.Get(m => m.Subject.Contains(ExchangeHelper.DefaultEmailSubject));
                messages
                    .Select(m => new { Name = Regex.Match(m.Subject, "#(.*)#").Groups[1].Value, m.Body }).ToList()
                    .ForEach(m =>
                    {
                        var fullName = Path.Combine(folder, m.Name);
                        File.WriteAllText(fullName, m.Body);
                    });
            }
            catch (Exception e)
            {
                Logger.WriteException(e, e.Message);
            }

        }

        private void _RunTypical()
        {
            try
            {
                string importFolder = ExchangeHelper.EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "Import", "Typical");
                var fileInfos = new DirectoryInfo(importFolder).EnumerateFiles("*", SearchOption.AllDirectories).ToList();
                if (!fileInfos.Any()) return;
                using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, new MyLogger("exchangeLogger")))
                {

                    fileInfos.ForEach(fi =>
                                      {
                                          XmlSerializer xml = XmlSerializer.FromTypes(new[] { typeof(ExchangeObject<TypicalExchangeData>) })[0];
                                          using (var fs = new FileStream(fi.FullName, FileMode.OpenOrCreate))
                                          {
                                              byte[] buffer = new byte[fs.Length];
                                              fs.Read(buffer, 0, (int) fs.Length);
                                              var decrypt = Cryptoghraphy.Decrypt(buffer);
                                              var eo = XmlHelper.Get<ExchangeObject<TypicalExchangeData>>(decrypt);
                                              eo?.Sync(context);
                                              fi.Delete();
                                          }
                                      });
                }
            }
            catch (Exception e)
            {
                Logger.WriteException(e, e.Message);
            }
        }

    }
}