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

    public class ExchangeTransportAgent
    {
        private static readonly MyLogger Logger = new MyLogger("exchangeLogger");

        private static readonly Lazy<ExchangeTransportAgent> Instance = new Lazy<ExchangeTransportAgent>(() => new ExchangeTransportAgent());

        private ExchangeTransportAgent()
        {
        }

        private void _Run()
        {
            try
            {
                List<ExchangeTaskDTO> exchangeTaskDtos;
                using (var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, new MyLogger("exchangeLogger")))
                {
                    exchangeTaskDtos = new GetExchangeTaskListQuery(context).Execute(new GetExchangeTaskListParameterSet());
                }

                exchangeTaskDtos.ForEach(RunTask);

            }
            catch (Exception e)
            {
                Logger.WriteException(e, e.Message);
            }
        }

        private void RunTask(ExchangeTaskDTO task)
        {
            try
            {
                BaseTransport.Create(task).Execute();
            }
            catch (Exception e)
            {
                Logger.WriteException(e, e.Message);
            }
        }

        public static void Run()
        {
            Instance.Value._Run();
        }

        public static void RunConfig(ExchangeTaskDTO cfg)
        {
            try
            {
                BaseTransport.Create(cfg).Execute();
            }
            catch (Exception e)
            {
                Logger.WriteException(e, e.Message);
            }
        }
    }
}