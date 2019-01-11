using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DAL.Calculations;
using GazRouter.DAL.Core;
using GazRouter.DAL.DataExchange.ExchangeEntity;
using GazRouter.DAL.SeriesData.GasInPipes;
using GazRouter.DAL.SysEvents;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.Transformation;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.SeriesData.GasInPipes;
using GazRouter.DTO.SysEvents;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib.Import.Astra;

namespace GazRouter.Service.Exchange.Lib.Import
{
    public class AstraExchangeImporter : SpecificExchangeImporter
    {
        private string gusSupplyTableName = "gas_supply";
        private const int AstraDataSourceId = 3;
        public override void Run(ExecutionContext context, ExchangeTaskDTO task, string fullPath)
        {
            ParsePipe(context, task, fullPath);
        }

        private void ParsePipe(ExecutionContext context, ExchangeTaskDTO task, string fullPath)
        {
            int serieId = -1;
            try
            {
                string input;
                var fileName = Path.GetFileName(fullPath);
                Exception error;
                var timeStamp = GetTimeStamp(task, fileName, out error);
                var logger = new MyLogger("exchangeLogger");
                if (error != null)
                {
                    logger.Info($"Неудачное извлечение метки времени из имени файла {fileName}. Пытаемся извлечь из данных файла");
                }
                input = FileTools.ReadFile(fullPath);

                var eo = Import<AstraPipeData>(new ImportParams
                {
                    Text = input,
                    Task = task,
                    TimeStamp = timeStamp,
                    FileName = fileName
                });

                if (eo?.Status == ExchangeStatus.NoData)
                {
                    throw new NotSupportedException("Импорт данных со статусом нет данных. Импорт не состоялся.");
                }

                eo?.Sync(context, task, out serieId);

                //добавление события завершения загрузки данных из файла
                new AddSysEventCommand(context).Execute(new AddSysEventParameters
                {
                    EventStatusId = SysEventStatus.Finished,
                    EventStatusIdMii = SysEventStatus.Finished,
                    EventTypeId = SysEventType.END_LOAD_ASTRA,
                    Description = $@"АСТРА.Запас. Имя файла: {fileName}",
                    SeriesId = serieId
                });

                ExchangeHelper.LogOk(task, serieId, null, null, "ok", context);

                logger.Info($"Импорт астры: запущен расчет по запасу газа по файлу: {fileName}.");
                new RunAstraCalcSqlCommand(context).Execute(new RunAstraCalcParameterSet
                {
                    SeriesId = serieId,
                    IsClearCalcValues = false,
                    IsExecTypedCalculation = true,
                    IsExecNonTypedCalculation = true,
                    IsExecAstraCalculation = true
                });
            }
            catch (Exception e)
            {
                e.Data.Add("SeriesId", serieId == -1 ? (int?)null : serieId);
                throw e;
            }

        }

        public override bool IsValid(ExchangeTaskDTO task, string fullPath)
        {
            return  task.DataSourceId == AstraDataSourceId && task.SqlProcedureName?.ToLower() == gusSupplyTableName && base.IsValid(task, fullPath) ;
        }
    }
}