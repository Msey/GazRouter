using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DAL.Calculations;
using GazRouter.DAL.Core;
using GazRouter.DAL.DataExchange.ExchangeEntity;
using GazRouter.DAL.SeriesData.GasInPipes;
using GazRouter.DAL.SeriesData.PropertyValues;
using GazRouter.DAL.SeriesData.Series;
using GazRouter.DAL.SysEvents;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.GasInPipes;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.DTO.SysEvents;
using GazRouter.Log;

namespace GazRouter.Service.Exchange.Lib.Import.Astra
{
    /// <summary>
    ///     класс предназначен для
    ///     1) загрузки файлов АСТРЫ в формате csv
    ///     2) разборки файла и записи значений в БД ИУС ПТП
    ///     3) переноса файла в папку архива
    ///     Настройки путей к папкам выбираются из таблицы sys_params
    ///     Настройки источника данных для астры выбираются из таблицы sources
    /// </summary>
    public class AstraParsing
    {
        private const string CurrentCulture = "en-us";
            //текущая культура, требуется установка для корректно пребразования типов из файла Астры

        private const char ValueSeparator = ','; // разделитель значений в файле астры , по которомы парасятся значения
        private const string AstraSystemName = "astra"; // системное имя источник данных АСТРА в таблице  Sources
        private const int AstraImportTaskId = 13; // ид задания импорта для АСТРЫ

        public const string PipeSupplyFileNamePreffix = "pipe_";
            //преффикс выходного файла АСТРЫ со значениями запаса газа по участкам

        public const string CShopeFileNamePreffix = "cshop_";
            //преффикс выходного файла АСТРЫ со значениями запаса газа по участкамcshop_

        private const string pipeBindingPreffix = "pipe_";
            // преффикс для кодов МГ в БД ИУС ПТП, введен так как код объекта АСТРЫ не уникален

        private readonly CultureInfo _culture;
        // readonly Source _astraSource;
        protected readonly ExecutionContext Context;
        private SeriesQueue seriesQueue;

        public AstraParsing(ExecutionContext context)
        {
            _culture = new CultureInfo(CurrentCulture);
            Context = context;
        }

        /// <summary>
        ///     разборка файла астры с со значениями запаса газа по участкам
        /// </summary>
        private void AstraParsePipeSectionList(string astraFilePath, IEnumerable<ExchangeEntityDTO> bindingList)
        {
            var astraRows = FileTools.ReadFileCyrillicEncoding(astraFilePath);
            if (astraRows.Count > 0)
            {
                var pipeSectionList =
                    astraRows.Select(astraRow => new AstraPipeSection(astraRow, _culture, ValueSeparator)).ToList();
                var exchTable = pipeSectionList.Distinct().Join(bindingList,
                    o => string.Concat(pipeBindingPreffix, o.GasMainAstraCode.ToString()),
                    i => i.ExtId,
                    (o, i) => new
                    {
                        entity_id = i.EntityId,
                        // name = i.Name,
                        astraName = o.Description,
                        astraCode = o.PipeSectionAstraCode,
                        //ext_key = i.ExtEntityId,
                        value = o.GasVolume,
                        startKm = o.KilometerStart,
                        endKm = o.KilometerEnd,
                        dt = o.Timestamp
                    }
                    ).ToList();

                if (exchTable.Any())
                {
                    var dt = exchTable.Select(var1 => var1.dt).First();
                    var serieId = GetSerie(dt);
                    //очистка значений запаса за эту серию
                    new DeleteGasInPipeCommand(Context).Execute(serieId);

                    //группировка сегментов по МГ , начальному, конечному километру
                    var segmentGroupes = (exchTable.GroupBy(segm => new {segm.entity_id, segm.startKm, segm.endKm})
                        .Select(gr => new
                        {
                            gr.Key.entity_id,
                            gr.Key.startKm,
                            gr.Key.endKm,
                            Value = gr.Sum(v => v.value),
                            Description = string.Concat(
                                string.Join(";", gr.Select(d => d.astraCode)),
                                "|",
                                string.Join(";", gr.Select(d => d.astraName)))
                        })
                        ).ToList();
                    //запись значения запаса в БД

                    foreach (var segment in segmentGroupes)
                    {
                        new AddGasInPipeCommand(Context).Execute(new AddGasInPipeParameterSet
                        {
                            SeriesId = serieId,
                            PipelineId = segment.entity_id,
                            StartKm = segment.startKm,
                            EndKm = segment.endKm,
                            Value = segment.Value,
                            Description = segment.Description
                        }
                            );
                    }
                    //добавление события завершения загрузки данных из файла
                    new AddSysEventCommand(Context).Execute(new AddSysEventParameters
                    {
                        EventStatusId = SysEventStatus.Finished,
                        EventStatusIdMii = SysEventStatus.Finished,
                        EventTypeId = SysEventType.END_LOAD_ASTRA,
                        Description = string.Format(@"АСТРА.Запас. Имя файла: {0}", astraFilePath),
                        SeriesId = serieId
                    });

                    seriesQueue.AddSerie(serieId);
                }
            }
        }

        /// <summary>
        ///     парсинг файла астры со значениями перекачки газа по цеху
        /// </summary>
        private void AstraParseCshopList(string astraFilePath, IEnumerable<ExchangeEntityDTO> bindingList)
        {
            var astraRows = FileTools.ReadFileCyrillicEncoding(astraFilePath);
            if (astraRows.Count > 0)
            {
                var pipeCsList =
                    astraRows.Select(astraRow => new AstraCompressorShop(astraRow, _culture, ValueSeparator)).ToList();

                var exchTable = pipeCsList.Join(bindingList,
                    shop => shop.AsduCode,
                    binding => binding.ExtId,
                    (shop, binding) => new
                    {
                        entity_id = binding.EntityId,
                        value = shop.Pumping,
                        dt = shop.Timestamp
                    }
                    ).ToList();

                if (exchTable.Any())
                {
                    var dt = exchTable.Select(var1 => var1.dt).First();
                    var serieId = GetSerie(dt);

                    foreach (var pumpValue in exchTable)
                    {
                        //запись значения в базу 
                        new SetPropertyValueCommand(Context).Execute(new SetPropertyValueParameterSet
                        {
                            SeriesId = serieId,
                            EntityId = pumpValue.entity_id,
                            PropertyTypeId = PropertyType.Pumping,
                            Value = pumpValue.value
                        });
                    }

                    //добавление события завершения загрузки данных из файла
                    new AddSysEventCommand(Context).Execute(new AddSysEventParameters
                    {
                        EventStatusId = SysEventStatus.Finished,
                        EventStatusIdMii = SysEventStatus.Finished,
                        EventTypeId = SysEventType.END_LOAD_ASTRA,
                        Description = string.Format(@"АСТРА.Перекачка газа. Имя файла: {0}", astraFilePath),
                        SeriesId = serieId
                    });


                    seriesQueue.AddSerie(serieId);
                    //запуск расчета астры по перекачке газа

                    //new RunAstraCalcSqlCommand(Context).Execute(new RunAstraCalcParameterSet
                    //{
                    //    SeriesId = serieId,
                    //    IsClearCalcValues = false,
                    //    IsExecTypedCalculation = true,
                    //    IsExecNonTypedCalculation = true,
                    //    IsExecAstraCalculation = true
                    //});
                }
            }
        }

        /// <summary>
        ///     процедура запускает сканирование преднастроенной папки астры и парсинга файлов
        /// </summary>
        //public void Parse()
        //{
        //    try
        //    {
        //        var fileList = GetAstraFileList();
        //        if (!fileList.Any()) return;
        //        var bindingList = GetEntityBinding().ToList();
        //        //выборка привязок для газопроводов
        //        var piplineBindings =
        //            bindingList.Where(bindingDTO => bindingDTO.EntityTypeId == EntityType.Pipeline).ToList();
        //        //выборка привязок для компрессорных цехов
        //        var cShopBindings =
        //            bindingList.Where(bindingDTO => bindingDTO.EntityTypeId == EntityType.CompShop).ToList();

        //        seriesQueue = new SeriesQueue();
        //        //парсинг файлов
        //        foreach (var astraFileName in fileList)
        //        {
        //            var fi = new FileInfo(astraFileName);

        //            //файл астра с содержимым запаса газа по участкам
        //            if (fi.Name.Contains(PipeSupplyFileNamePreffix))
        //            {
        //                AstraParsePipeSectionList(astraFileName, piplineBindings);
        //            } // файлы астра с содержимым перекачки газа по цехам
        //            else if (fi.Name.Contains(CShopeFileNamePreffix))
        //            {
        //                AstraParseCshopList(astraFileName, cShopBindings);
        //            }
        //        }

        //        //запуск расчетов по сериям
        //        if (seriesQueue.SeriesList.Any())
        //        {
        //            foreach (var serieId in seriesQueue.SeriesList)
        //            {
        //                new RunAstraCalcSqlCommand(Context).Execute(new RunAstraCalcParameterSet
        //                {
        //                    SeriesId = serieId,
        //                    IsClearCalcValues = false,
        //                    IsExecTypedCalculation = true,
        //                    IsExecNonTypedCalculation = true,
        //                    IsExecAstraCalculation = true
        //                });
        //            }
        //        }


        //        ////перенос файлов в архив
        //        //#region перенос файлов в архив -- на время отладки закомментировано

        //        //var archFolder = AppSettingsManager.ExchangeArchiveDirectory;

        //        //FileTools.TransferFilesToArchiveDirectory(fileList, Path.Combine(archFolder, AstraSystemName));
        //        //#endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        Context.Logger.WriteException(ex, "Ошибка при парсинге");
        //    }
        //}

        /// <summary>
        ///     процедура запускает сканирование преднастроенной папки астры и парсинга файлов
        /// </summary>
        public void ParseCSshop(string astraFileName)
        {
            var bindingList = GetEntityBinding().ToList();
            //выборка привязок для газопроводов
            var cShopBindings =
                bindingList.Where(bindingDTO => bindingDTO.EntityTypeId == EntityType.CompShop).ToList();

            seriesQueue = new SeriesQueue();
            var fi = new FileInfo(astraFileName);

            //файл астра с содержимым запаса газа по участкам
            if (fi.Name.Contains(CShopeFileNamePreffix))
            {
                AstraParseCshopList(astraFileName, cShopBindings);
            }
            //запуск расчетов по сериям
            if (seriesQueue.SeriesList.Any())
            {
                foreach (var serieId in seriesQueue.SeriesList)
                {
                    new RunAstraCalcSqlCommand(Context).Execute(new RunAstraCalcParameterSet
                    {
                        SeriesId = serieId,
                        IsClearCalcValues = false,
                        IsExecTypedCalculation = true,
                        IsExecNonTypedCalculation = true,
                        IsExecAstraCalculation = true
                    });
                }
            }


            ////перенос файлов в архив
            //#region перенос файлов в архив -- на время отладки закомментировано

            //var archFolder = AppSettingsManager.ExchangeArchiveDirectory;

            //FileTools.TransferFileToArchiveDirectory(astraFileName, Path.Combine(archFolder, AstraSystemName));
        }

        public void ParsePipe(string astraFileName)
        {
            var bindingList = GetEntityBinding().ToList();
            //выборка привязок для газопроводов
            var piplineBindings =
                bindingList.Where(bindingDTO => bindingDTO.EntityTypeId == EntityType.Pipeline).ToList();

            seriesQueue = new SeriesQueue();
            var fi = new FileInfo(astraFileName);

            //файл астра с содержимым запаса газа по участкам
            if (fi.Name.Contains(PipeSupplyFileNamePreffix))
            {
                AstraParsePipeSectionList(astraFileName, piplineBindings);
            } // файлы астра с содержимым перекачки газа по цехам
              //запуск расчетов по сериям
            if (seriesQueue.SeriesList.Any())
            {
                foreach (var serieId in seriesQueue.SeriesList)
                {
                    new RunAstraCalcSqlCommand(Context).Execute(new RunAstraCalcParameterSet
                    {
                        SeriesId = serieId,
                        IsClearCalcValues = false,
                        IsExecTypedCalculation = true,
                        IsExecNonTypedCalculation = true,
                        IsExecAstraCalculation = true
                    });
                }
            }


            ////перенос файлов в архив
            //#region перенос файлов в архив -- на время отладки закомментировано

            //var archFolder = AppSettingsManager.ExchangeArchiveDirectory;

            //FileTools.TransferFileToArchiveDirectory(astraFileName, Path.Combine(archFolder, AstraSystemName));
            
        }

        /// <summary>
        ///     процедура запускает сканирование преднастроенной папки астры и парсинга файлов
        /// </summary>
        private IEnumerable<ExchangeEntityDTO> GetEntityBinding()
        {
            var taskIdList = new List<int> {AstraImportTaskId};
            var entBindings = new GetExchangeEntityListQuery(Context).Execute(
                new GetExchangeEntityListParameterSet
                {
                    ExchangeTaskIdList = taskIdList
                });
            return entBindings;
        }

        /// <summary>
        ///     функция возвращает файлы в папке астры( настраивается в системных переменных sys_param
        /// </summary>
        /// <returns></returns>
        //public List<string> GetAstraFileList()
        //{
        //    var astraFileList = new List<string>();
        //    var folder = Path.Combine(AppSettingsManager.ExchangeImportDirectory, "Astra");
        //    try
        //    {
        //        astraFileList = Directory.GetFiles(folder, "*.csv").ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        Context.Logger.WriteException(ex, "Ошибка доступа к папке");
        //    }
        //    return astraFileList;
        //}

        /// <summary>
        ///     возвращает id серии данных по метке времени
        /// </summary>
        private int GetSerie(DateTime dt)
        {
            int serieId;
            //получение имеющейся серии
            var availSeries = new GetSeriesQuery(Context).Execute(new GetSeriesParameterSet
            {
                PeriodType = PeriodType.Twohours,
                TimeStamp = dt
            });
            if (availSeries == null)
            {
                //создание новой серии
                serieId = new AddSeriesCommand(Context).Execute(new AddSeriesParameterSet
                {
                    PeriodTypeId = PeriodType.Twohours,
                    KeyDate = dt,
                    Description = string.Empty
                });
            }
            else
            {
                serieId = availSeries.Id;
            }

            return serieId;
        }

        //public static void Run()
        //{
        //    using (
        //        var context = DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin,
        //            new MyLogger("exchangeLogger")))
        //    {
        //        new AstraParsing(context).Parse();
        //    }
        //}
    }


    public class SeriesQueue
    {
        public SeriesQueue()
        {
            SeriesList = new List<int>();
        }

        public List<int> SeriesList { get; }

        public void AddSerie(int serie)
        {
            if (!SeriesList.Contains(serie))
                SeriesList.Add(serie);
        }
    }
}