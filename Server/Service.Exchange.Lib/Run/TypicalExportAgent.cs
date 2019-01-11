using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DAL.Core;
using GazRouter.DAL.Dictionaries.Enterprises;
using GazRouter.DAL.SysEvents;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.SysEvents;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib.Cryptography;
using GazRouter.Service.Exchange.Lib.Export;
using GazRouter.Service.Exchange.Lib.Import;

namespace GazRouter.Service.Exchange.Lib.Run
{
    public static class TypicalExportAgent
    {
        private static IEnumerable<EnterpriseDTO> GetNeighbours()
        {
            using (
                ExecutionContextReal context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, new MyLogger("exchangeLogger")))
            {
                return new GetEnterpriseExchangeNeighbourList(context).Execute(AppSettingsManager.CurrentEnterpriseId);
            }
        }


        private static void RunOnEvent(SysEventDTO @event, out string fullPath)
        {
            fullPath = null;
            IEnumerable<EnterpriseDTO> neighbours = GetNeighbours();
            foreach (EnterpriseDTO ent in neighbours)
            {
                ExchangeObject<TypicalExchangeData> eo;
                using (
                    var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, new MyLogger("exchangeLogger")))
                {
                    eo = new TypicalExchangeObjectExporter(context, ent).Build(@event.KeyDate);
                }

                string sourceCode = ExchangeHelper.CurrentEnterpriseDTO.Code;
                string timeStamp = eo.HeaderSection.TimeStamp.ToString("yyyy-mm-HH");
                var directory = ExchangeHelper.EnsureDirectoryExist(AppSettingsManager.ExchangeDirectory, "Export", "Typical");
                fullPath = Path.Combine(directory, $"{sourceCode}_{timeStamp}.{ent.Code}");

                var bytes = XmlHelper.GetBytes(eo);
                using (var fs = FileTools.OpenOrCreate(fullPath))
                {
                    var encrypt = Cryptoghraphy.Encrypt(bytes);
                    fs.Write(encrypt, 0, encrypt.Length);
                }
            }
        }

        public static void Run()
        {
            using (
                ExecutionContextReal context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                    new MyLogger("exchangeLogger")))
            {
                var events = new GetSysEventListQuery(context).Execute(new GetSysEventListParameters
                                                                                       {
                                                                                           EventTypeId = SysEventType.END_CALCULATION_AFTER_LOAD_DATA,
                                                                                           EventStatusId = SysEventStatus.Waiting
                                                                                       });
                if (!events.Any()) return;
                foreach (var @event in events)
                {
                    try
                    {
                        string fileName;
                        RunOnEvent(@event, out fileName);
                        new AddSysEventCommand(context).Execute(new AddSysEventParameters
                                                                {
                                                                    EventStatusId = SysEventStatus.Finished,
                                                                    EventStatusIdMii = SysEventStatus.Finished,
                                                                    EventTypeId = SysEventType.END_EXPORT_TASK,
                                                                    Description = $@"Файл: {fileName}",
                                                                    SeriesId = @event.SeriesId
                                                                });

                        new SetStatusSysEventCommand(context).Execute(new SetStatusSysEventParameters
                                                                      {
                                                                          EventId = @event.Id,
                                                                          EventStatusId = SysEventStatus.Finished,
                                                                          ResultId = SysEventResult.Success
                                                                      });
                    }
                    catch (Exception e)
                    {
                        new SetStatusSysEventCommand(context).Execute(new SetStatusSysEventParameters
                                                                      {
                                                                          EventId = @event.Id,
                                                                          EventStatusId = SysEventStatus.Finished,
                                                                          ResultId = SysEventResult.Error
                                                                      });
                        context.Logger.WriteException(e, string.Empty);
                    }
                }
            }
        }
    }
}