using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DAL.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.TransportTypes;
using GazRouter.Log;

namespace GazRouter.Service.Exchange.Lib.Transport
{
    public class TransportAgent
    {
        private static readonly Lazy<TransportAgent> Instance =
            new Lazy<TransportAgent>(() => new TransportAgent());

        protected readonly MyLogger Logger = new MyLogger("exchangeLogger");

        private TransportAgent()
        {
        }

        public IEnumerable<ExchangeTaskDTO> GetImportTasks()
        {
            // фейковые таски, смысл которых в том, чтобы забрать все файлы с обменных серверов и положить в папку импорта
            yield return
                new ExchangeTaskDTO
                {
                    Id = -1,
                    ExchangeTypeId = ExchangeType.Import,
                    TransportAddress = AppSettingsManager.ExchangeServerUrl,
                    TransportTypeId = TransportType.Ftp
                };
            yield return
                new ExchangeTaskDTO
                {
                    Id = -2,
                    ExchangeTypeId = ExchangeType.Import,
                    TransportTypeId = TransportType.Email
                };
        }

        private void _Run()
        {
            using (
                var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
                    new MyLogger("exchangeLogger")))
            {
                var exportedTasks =
                    new GetExchangeTaskListQuery(context).Execute(new GetExchangeTaskListParameterSet
                    {
                        ExchangeTypeId = ExchangeType.Export
                    }).Where(t => t.TransportTypeId.HasValue && !string.IsNullOrEmpty(t.TransportAddress));
                var importedTasks = GetImportTasks();
                var unionTasks = exportedTasks.Union(importedTasks).ToList();

                unionTasks.ForEach(t =>
                {
                    try
                    {
                        BaseTransport.Create(t).Execute();
                    }
                    catch (Exception e)
                    {
                        Logger.WriteFullException(e, e.Message);
                    }
                });
            }
        }

        public static void RunTask(ExchangeTaskDTO task)
        {
            try
            {
                BaseTransport.Create(task).Execute();
            }
            catch (Exception e)
            {
                throw new Exception(
                    $"Обработка и транспортировка таска: Id {task.Id}, TransportAddress {task.TransportAddress}, Login {task.TransportLogin}, Pwd {task.TransportPassword} ",
                    e);
            }
        }


        public static void Run()
        {
            Instance.Value._Run();
        }
    }
}