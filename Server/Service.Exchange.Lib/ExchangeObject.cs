using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using GazRouter.DAL.Core;
using GazRouter.DAL.DataExchange.ExchangeEntity;
using GazRouter.DAL.DataExchange.ExchangeLog;
using GazRouter.DAL.DataExchange.ExchangeProperty;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.CompUnits;
using GazRouter.DAL.ObjectModel.CoolingStations;
using GazRouter.DAL.ObjectModel.DistrStationOutlets;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DAL.ObjectModel.Entities;
using GazRouter.DAL.ObjectModel.MeasLine;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DAL.ObjectModel.ReducingStations;
using GazRouter.DAL.ObjectModel.Sites;
using GazRouter.DAL.SeriesData.GasInPipes;
using GazRouter.DAL.SeriesData.PropertyValues;
using GazRouter.DAL.SeriesData.Series;
using GazRouter.DAL.SysEvents;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.DataExchange.ExchangeProperty;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.CompUnitSealingTypes;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Infrastructure;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Aggregators;
using GazRouter.DTO.ObjectModel.BoilerPlants;
using GazRouter.DTO.ObjectModel.Boilers;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.CoolingStations;
using GazRouter.DTO.ObjectModel.CoolingUnit;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.OperConsumers;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.PowerPlants;
using GazRouter.DTO.ObjectModel.PowerUnits;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.DTO.SeriesData.GasInPipes;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.DTO.SysEvents;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib.Import.Astra;
using Utils.Extensions;

namespace GazRouter.Service.Exchange.Lib
{
    public enum ExchangeStatus
    {
        Ok,
        NoData
    }

    [XmlRoot("ExchangeMessage")]
    [Serializable]
    public class ExchangeObject<T> where T : new()
    {
        public ExchangeObject()
        {
            HeaderSection = new ExchangeHeader();
            DataSection = new T();
        }

        [XmlElement("HeaderSection")]
        public ExchangeHeader HeaderSection { get; set; }

        [XmlElement("DataSection")]
        public T DataSection { get; set; }

        [XmlElement("Status")]
        public ExchangeStatus Status { get; set; }


    }



    public class ExchangeHeader
    {
        public DateTime TimeStamp { get; set; }
        public PeriodType PeriodType { get; set; }
        public string Sender { get; set; }
        public string Receiver { get; set; }
        public DateTime GeneratedTime { get; set; }
        public string Comment { get; set; }
    }

    #region SpecificExchangeData


    public class ExtData
    {
        public ExtData()
        {
            Items = new List<ExtItem>();
        }

        [XmlArray("ExtItems")]
        [XmlArrayItem("ExtItem")]
        public List<ExtItem> Items { get; set; }
    }

    [XmlInclude(typeof(PropertyValueDoubleDTO))]
    [XmlInclude(typeof(PropertyValueDateDTO))]
    [XmlInclude(typeof(PropertyValueStringDTO))]
    [XmlInclude(typeof(PropertyValueEmptyDTO))]
    public class ExtItem
    {
        public string ExtKey { get; set; }

        public string Value { get; set; }
    }


    public class SpecificExchangeData
    {
        public SpecificExchangeData()
        {
            SiteDtos = new List<ExchangeItem<SiteDTO>>();
            CompStationDTOs = new List<ExchangeItem<CompStationDTO>>();
            CompShopDtos = new List<ExchangeItem<CompShopDTO>>();
            DistrStationDtos = new List<ExchangeItem<DistrStationDTO>>();
            DistrStationOutletDtos = new List<ExchangeItem<DistrStationOutletDTO>>();
            MeasPointDtos = new List<ExchangeItem<MeasPointDTO>>();
            MeasLineDtos = new List<ExchangeItem<MeasLineDTO>>();
            CompUnitDtos = new List<ExchangeItem<CompUnitDTO>>();
            ReducingStationDtos = new List<ExchangeItem<ReducingStationDTO>>();
            ValveDtos = new List<ExchangeItem<ValveDTO>>();
            CoolingStations = new List<ExchangeItem<CoolingStationDTO>>();
            CoolingUnits = new List<ExchangeItem<CoolingUnitDTO>>();
            OperConsumers = new List<ExchangeItem<OperConsumerDTO>>();
            Aggregators = new List<ExchangeItem<AggregatorDTO>>();
            BoilerPlants = new List<ExchangeItem<BoilerPlantDTO>>();
            Boilers = new List<ExchangeItem<BoilerDTO>>();
            Consumers = new List<ExchangeItem<ConsumerDTO>>();
            //Pipelines = new List<ExchangeItem<PipelineDTO>>();
            PowerPlants = new List<ExchangeItem<PowerPlantDTO>>();
            PowerUnits = new List<ExchangeItem<PowerUnitDTO>>();
            RawData = new XmlRawData();
        }
        public XmlRawData RawData { get; set; }

        [XmlArray("Sites")]
        [XmlArrayItem("Site")]
        public List<ExchangeItem<SiteDTO>> SiteDtos { get; set; }

        [XmlArray("CompressorStations")]
        [XmlArrayItem("CompressorStation")]
        public List<ExchangeItem<CompStationDTO>> CompStationDTOs { get; set; }

        [XmlArray("CompressorShops")]
        [XmlArrayItem("CompressorShop")]
        public List<ExchangeItem<CompShopDTO>> CompShopDtos { get; set; }

        [XmlArray("MeasuringStations")]
        [XmlArrayItem("MeasuringStation")]
        public List<ExchangeItem<MeasStationDTO>> MeasStationDtos { get; set; }

        [XmlArray("DistributingStations")]
        [XmlArrayItem("DistributingStation")]
        public List<ExchangeItem<DistrStationDTO>> DistrStationDtos { get; set; }

        [XmlArray("Outputs")]
        [XmlArrayItem("Output")]
        public List<ExchangeItem<DistrStationOutletDTO>> DistrStationOutletDtos { get; set; }

        [XmlArray("MeasurePoints")]
        [XmlArrayItem("MeasurePoint")]
        public List<ExchangeItem<MeasPointDTO>> MeasPointDtos { get; set; }

        [XmlArray("MeasureLines")]
        [XmlArrayItem("MeasureLine")]
        public List<ExchangeItem<MeasLineDTO>> MeasLineDtos { get; set; }

        [XmlArray("CompUnits")]
        [XmlArrayItem("CompUnit")]
        public List<ExchangeItem<CompUnitDTO>> CompUnitDtos { get; set; }

        [XmlArray("ReducingStations")]
        [XmlArrayItem("ReducingStation")]
        public List<ExchangeItem<ReducingStationDTO>> ReducingStationDtos { get; set; }

        [XmlArray("Valves")]
        [XmlArrayItem("Valve")]
        public List<ExchangeItem<ValveDTO>> ValveDtos { get; set; }

        [XmlArray("CoolingStations")]
        [XmlArrayItem("CoolingStation")]
        public List<ExchangeItem<CoolingStationDTO>> CoolingStations { get; set; }

        [XmlArray("CoolingUnits")]
        [XmlArrayItem("CoolingUnit")]
        public List<ExchangeItem<CoolingUnitDTO>> CoolingUnits { get; set; }

        [XmlArray("OperConsumers")]
        [XmlArrayItem("OperConsumer")]
        public List<ExchangeItem<OperConsumerDTO>> OperConsumers { get; set; }


        [XmlArray("Aggregators")]
        [XmlArrayItem("Aggregator")]
        public List<ExchangeItem<AggregatorDTO>> Aggregators { get; set; }


        [XmlArray("Boilers")]
        [XmlArrayItem("Boiler")]
        public List<ExchangeItem<BoilerDTO>> Boilers { get; set; }


        [XmlArray("BoilerPlants")]
        [XmlArrayItem("BoilerPlant")]
        public List<ExchangeItem<BoilerPlantDTO>> BoilerPlants { get; set; }

        [XmlArray("Consumers")]
        [XmlArrayItem("Consumer")]
        public List<ExchangeItem<ConsumerDTO>> Consumers { get; set; }


        [XmlArray("PowerPlants")]
        [XmlArrayItem("PowerPlant")]
        public List<ExchangeItem<PowerPlantDTO>> PowerPlants { get; set; }


        [XmlArray("PowerUnits")]
        [XmlArrayItem("PowerUnit")]
        public List<ExchangeItem<PowerUnitDTO>> PowerUnits { get; set; }


        //[XmlArray("Pipelines")]
        //[XmlArrayItem("Pipeline")]
        //public List<ExchangeItem<PipelineDTO>> Pipelines { get; set; }
    }

    public class XmlRawData
    {
        public XmlRawData()
        {
            Nodes = new List<XElement>();
        }

        [XmlAnyElement]
        public List<XElement> Nodes { get; set; }
    }

    #endregion

#region TypicalExchangeData

public class TypicalExchangeData
    {
        public TypicalExchangeData()
        {
            SiteDtos = new List<ExchangeSite>();
        }

        [XmlArray("Sites")]
        [XmlArrayItem("Site")]
        public List<ExchangeSite> SiteDtos { get; set; }
    }

    public class ExchangeSite : ExchangeItem<SiteDTO>
    {
        public ExchangeSite()
        {
            MeasStationDtos = new List<ExchangeMeasStation>();
            DistrStationDtos = new List<ExchangeDistrStation>();
            ReducingStationDtos = new List<ExchangeItem<ReducingStationDTO>>();
            CompStationDTos= new List<ExchangeCompStation>();
            OperConsumers = new List<ExchangeItem<OperConsumerDTO>>();
            CoolingUnits = new List<ExchangeItem<CoolingUnitDTO>>();
        }

        public ExchangeSite(ExchangeItem<SiteDTO> site) : base(site)
        {
        }

        [XmlArray("MeasuringStations")]
        [XmlArrayItem("MeasuringStation")]
        public List<ExchangeMeasStation> MeasStationDtos { get; set; }

        [XmlArray("DistributingStations")]
        [XmlArrayItem("DistributingStation")]
        public List<ExchangeDistrStation> DistrStationDtos { get; set; }

        [XmlArray("ReducingStations")]
        [XmlArrayItem("ReducingStation")]
        public List<ExchangeItem<ReducingStationDTO>> ReducingStationDtos { get; set; }

        [XmlArray("CompressorStations")]
        [XmlArrayItem("CompressorStation")]
        public List<ExchangeCompStation> CompStationDTos { get; set; }


        [XmlArray("CoolingUnits")]
        [XmlArrayItem("CoolingUnit")]
        public List<ExchangeItem<CoolingUnitDTO>> CoolingUnits { get; set; }

        [XmlArray("OperConsumers")]
        [XmlArrayItem("OperConsumer")]
        public List<ExchangeItem<OperConsumerDTO>> OperConsumers { get; set; }
    }

    public class ExchangeMeasStation : ExchangeItem<MeasStationDTO>
    {
        public ExchangeMeasStation(ExchangeItem<MeasStationDTO> item) : base(item)
        {
        }

        public ExchangeMeasStation()
        {
             MeasLines = new List<ExchangeMeasLine>();
        }

        [XmlArray("MeasureLines")]
        [XmlArrayItem("MeasureLine")]
        public List<ExchangeMeasLine> MeasLines { get; set; }
    }

    public class ExchangeMeasLine : ExchangeItem<MeasLineDTO>
    {
        public ExchangeMeasLine(ExchangeItem<MeasLineDTO> item) : base(item)
        {
        }

        public ExchangeMeasLine()
        {
            MeasPoints = new List<ExchangeItem<MeasPointDTO>>();
        }

        [XmlArray("MeasurePoints")]
        [XmlArrayItem("MeasurePoint")]
        public List<ExchangeItem<MeasPointDTO>> MeasPoints { get; set; }
    }

    public class ExchangeDistrStation : ExchangeItem<DistrStationDTO>
    {
        public ExchangeDistrStation(ExchangeItem<DistrStationDTO> item) : base(item)
        {
        }

        public ExchangeDistrStation()
        {
            DistrStationOutlets = new List<ExchangeItem<DistrStationOutletDTO>>();
        }

        //[XmlArray("Consumers")]
        //[XmlArrayItem("Consumer")]
        //public List<ExchangeItem<ConsumerDTO>> Consumers { get; set; }

        [XmlArray("DistrStaionOutlets")]
        [XmlArrayItem("DistrStaionOutlet")]
        public List<ExchangeItem<DistrStationOutletDTO>> DistrStationOutlets { get; set; }
    }

    public class ExchangeCompStation : ExchangeItem<CompStationDTO>
    {
        public ExchangeCompStation(ExchangeItem<CompStationDTO> item) : base(item)
        {
        }

        public ExchangeCompStation()
        {
            CompShops = new List<ExchangeCompShop>();
        }

        [XmlArray("CompShops")]
        [XmlArrayItem("CompShop")]
        public List<ExchangeCompShop> CompShops { get; set; }
    }

    public class ExchangeCompShop : ExchangeItem<CompShopDTO>
    {
        public ExchangeCompShop(ExchangeItem<CompShopDTO> item) : base(item)
        {
        }

        public ExchangeCompShop()
        {
            CompUnits = new List<ExchangeItem<CompUnitDTO>>();
        }

        [XmlArray("CompUnits")]
        [XmlArrayItem("CompUnit")]
        public List<ExchangeItem<CompUnitDTO>> CompUnits { get; set; }
    }

    #endregion

    [XmlInclude(typeof (PropertyValueDoubleDTO))]
    [XmlInclude(typeof (PropertyValueDateDTO))]
    [XmlInclude(typeof (PropertyValueStringDTO))]
    [XmlInclude(typeof (PropertyValueEmptyDTO))]
    public class ExchangeItem<T> where T : CommonEntityDTO, new()
    {
        public ExchangeItem(ExchangeItem<T> item)
        {
            Dto = item.Dto;
            ExtKey = item.ExtKey;
            Properties = item.Properties;
        }

        public ExchangeItem()
        {
            Properties = new List<ExchangeProperty>();
        }

        [DefaultValue(typeof(string), "")]
        public string ExtKey { get; set; }

        [XmlElement("Item")]
        public T Dto { get; set; }

        [XmlArray("Properties")]
        [XmlArrayItem("Property")]
        public List<ExchangeProperty> Properties { get; set; }

        [XmlElement("Formatted_ID")]
        public string FormattedId
        {
            get { return Dto?.Id.Convert().ToString("N").ToUpper(); }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                if (Dto == null)
                {
                    Dto = new T();
                }
                var bytes = ExchangeHelper.ParseHex(value);
                Dto.Id = new Guid(bytes);
            }
        }
    }

    public class ExchangeProperty
    {
        [DefaultValue(typeof(string), "")]
        public string ExtKey { get; set; }
        public string SysName { get; set; }
        public dynamic Value { get; set; }
        public PropertyType PropertyType { get; set; }

        [XmlElement("PropertyTypeSysName")]
        public string PropertyTypeSysName
        {
            get { return PropertyType.ToString(); }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    PropertyType = (PropertyType) Enum.Parse(typeof (PropertyType), value, true);
                }
            }
        }
    }

    public class ExchangeProperty2<TPropertyDto>
    {
        public TPropertyDto Dto { get; set; }
        public dynamic Value { get; set; }
    }


    public static class ExchangeObjectHelper
    {

        public static string ToXml<T>(this ExchangeObject<T> eo) where T : new()
        {
            using (var sw = new StringWriter())
            {
                var xml = XmlSerializer.FromTypes(new[] {typeof (ExchangeObject<T>)})[0];
                xml.Serialize(sw, eo);
                return sw.ToString();
            }
        }
        //public static string ToXml<T>(this ExchangeObject<SpecificExchangeData> eo)
        //{
        //    var serializer = new XmlSerializer(eo.GetType());
        //    var settings = new XmlWriterSettings() { };

        //    string result;
        //    using (var ms = new MemoryStream())
        //    using (StreamReader sr = new StreamReader(ms))
        //    using (var writer = XmlWriter.Create(ms, settings))
        //    {
        //        serializer.Serialize(writer, eo, null);
        //        ms.Position = 0;
        //        result = sr.ReadToEnd();
        //    }

        //    result = result.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&apos;", "'");
        //    return result;
        //}

        public static byte[] ToArray<T>(this ExchangeObject<T> eo) where T : new()
        {
            var xml = XmlSerializer.FromTypes(new[] {typeof (ExchangeObject<T>)})[0];
            var serializer = new XmlSerializer(eo.GetType());
            var settings = new XmlWriterSettings() {};

            using (var ms = new MemoryStream())
            using (var writer = XmlWriter.Create(ms, settings))
            {
                serializer.Serialize(writer, eo, null);
                return ms.ToArray();
            }
        }

/*
        public static void Sync(this ExchangeObject<ImportSpecificExchangeData1> obj, ExecutionContext context)
        {
            var logger = new MyLogger("exchangeLogger");
            var periodType = obj.HeaderSection.PeriodType;
            var timeStamp = obj.HeaderSection.TimeStamp;
            var seriesId = GetSeriesId(timeStamp, periodType, context);
            var result = GetSyncCommands(obj, seriesId);
            var eventId = new Guid();

            eventId = new AddSysEventCommand(context).Execute(new AddSysEventParameters
            {
                EventStatusId = SysEventStatus.Finished,
                EventStatusIdMii = SysEventStatus.Finished,
                EventTypeId = SysEventType.END_LOAD_NEIGHBOR,
                Description = string.Format(@"Нетиповой обмен: {0}", obj.HeaderSection.Comment),
                SeriesId = seriesId
            });

            foreach (var action in result)
            {
                try
                {
                    action(context);
                }
                catch (Exception e)
                {
                    logger.WriteException(e, e.Message);
                }
            }

            new SetStatusSysEventCommand(context).Execute(new SetStatusSysEventParameters
            {
                EventId = eventId,
                EventStatusId = SysEventStatus.Finished,
                ResultId = SysEventResult.Success
            });
        }
*/
        public static void Sync(this ExchangeObject<AstraPipeData> obj, ExecutionContext context, ExchangeTaskDTO task,
            out int seriesId)
        {
            var logger = new MyLogger("exchangeLogger");
            var periodType = task.PeriodTypeId;
            var timeStamp = obj.HeaderSection.TimeStamp as DateTime?;
            var serie = ExchangeHelper.GetOrCreateSerie(context, null, timeStamp, periodType);
            seriesId = serie.Id;

            //выборка привязок для газопроводов
            var piplineBindings =
                new GetExchangeEntityListQuery(context).Execute(
                    new GetExchangeEntityListParameterSet
                    {
                        ExchangeTaskIdList = new List<int>() {task.Id},
                    }).Where(bindingDTO => bindingDTO.EntityTypeId == EntityType.Pipeline).ToList();

            var exchTable = obj.DataSection.Items.Join(piplineBindings,
                o => o.ExtKey,
                i => i.ExtId,
                (o, i) => new
                {
                    entity_id = i.EntityId,
                    description = o.Description,
                    value = o.Value,
                    startKm = o.KmStart,
                    endKm = o.KmEnd,
                    dt = timeStamp
                }
            ).ToList();


            if (exchTable.Any())
            {
                seriesId = ExchangeHelper.GetOrCreateSerie(context, dt: timeStamp).Id;
                //очистка значений запаса за эту серию
                new DeleteGasInPipeCommand(context).Execute((int) seriesId);

                //группировка сегментов по МГ , начальному, конечному километру
                var segmentGroupes = (exchTable.GroupBy(segm => new {segm.entity_id, segm.startKm, segm.endKm})
                    .Select(gr => new
                    {
                        gr.Key.entity_id,
                        gr.Key.startKm,
                        gr.Key.endKm,
                        Value = gr.Sum(v => v.value),
                        Description = string.Join(" | ", gr.Select(v => v.description))
                    })
                ).ToList();
                //запись значения запаса в БД
                int id = seriesId;
                IEnumerable<Action<ExecutionContext>> syncCommands = segmentGroupes.Select(s => new Action<ExecutionContext>(ctx => new AddGasInPipeCommand(ctx).Execute(new AddGasInPipeParameterSet
                    {
                        SeriesId = id,
                        PipelineId = s.entity_id,
                        StartKm = s.startKm,
                        EndKm = s.endKm,
                        Value = s.Value,
                        Description = s.Description
                    }
                ))).ToList();

                var exs = new List<Exception>();
                try
                {
                    Action<Exception> onMultipleCommandException = e => exs.Add(e);
                    new MultipleCommand(context, syncCommands) { OnException = onMultipleCommandException }.Execute();
                }
                finally
                {
                    if (exs.Any())
                    {
                        ExchangeHelper.LogError(task, seriesId, null, null, "error", $@"Возникло {exs.Count} ошибок. См. файл лога", context);
                        foreach (var e in exs)
                        {
                            logger.WriteFullException(e, $@"Astra: Произошла ошибка за метку {seriesId}");
                        }
                    }
                    else
                    {
                        ExchangeHelper.LogOk(task, seriesId, null, null, "Ок", context);
                        logger.Info($@"Astra: Импорт файла за метку {seriesId} удачно завершен");
                    }
                }
            }
        }

        public static void Sync(this ExchangeObject<ExtData> obj, ExecutionContext context, ExchangeTaskDTO task, out int seriesId)
        {
            var logger = new MyLogger("exchangeLogger");
            var periodType = task.PeriodTypeId;
            var timeStamp = obj.HeaderSection.TimeStamp as DateTime?;
            var serie = ExchangeHelper.GetOrCreateSerie(context, null, timeStamp, periodType);
            seriesId = serie.Id;

            var properties = GetExchangeProperties(task, obj, context);


            var eventId = new AddSysEventCommand(context).Execute(new AddSysEventParameters
            {
                EventStatusId = SysEventStatus.Finished,
                EventStatusIdMii = SysEventStatus.Finished,
                EventTypeId = SysEventType.END_LOAD_NEIGHBOR,
                Description = $@"Нетиповой обмен: {obj.HeaderSection.Comment}",
                SeriesId = seriesId
            });

            int id = seriesId;
            IEnumerable<Action<ExecutionContext>> syncCommands = properties.Select(prop => new Action<ExecutionContext>(
                ctx => {
                        new SetExchangePropertyValueCommand(context).Execute(new SetPropertyValueParameterSet
                        {
                            SeriesId = id,
                            EntityId = prop.Dto.EntityId,
                            PropertyTypeId = prop.Dto.PropertyTypeId,
                            Value = prop.Value,
                            Annotation = $@"Файл:{task?.Id}", 
                        });
                }
            )).ToList();

            var exs = new List<Exception>();
            try
            {
                Action<Exception> onMultipleCommandException = e => exs.Add(e);
                new MultipleCommand(context, syncCommands) { OnException = onMultipleCommandException }.Execute();
            }
            finally
            {
                if (exs.Any())
                {
                    ExchangeHelper.LogError(task, seriesId, null, null, "error", $@"Возникло {exs.Count} ошибок. См. файл лога", context);
                    foreach (var e in exs)
                    {
                        logger.WriteFullException(e, $@"Нетиповой импорт: Произошла ошибка в файле (маска {task?.FileNameMask}) за метку {seriesId}");
                    }
                    logger.Info($@"Нетиповой импорт: Импорт файла (маска {task?.FileNameMask}) за метку {seriesId} завершен c {exs.Count} ошибками");
                }
                else
                {
                    ExchangeHelper.LogOk(task, seriesId, null, null, "Ок", context);
                    logger.Info($@"Нетиповой импорт: Импорт файла (маска {task?.FileNameMask}) за метку {seriesId} удачно завершен");
                }
            }
            new SetStatusSysEventCommand(context).Execute(new SetStatusSysEventParameters
            {
                EventId = eventId,
                EventStatusId = SysEventStatus.Finished,
                ResultId = SysEventResult.Success
            });
        }


        private static IEnumerable<ExchangeProperty2<ExchangePropertyDTO>> GetExchangeProperties(ExchangeTaskDTO task, ExchangeObject<ExtData> extData, ExecutionContext context)
        {
            var exchangePropertyDtos = new GetExchangePropertyListQuery(context).Execute(new GetExchangeEntityListParameterSet
                                                                                        {
                                                                                            IsActive = true,
                                                                                            ExchangeTaskIdList = new List<int> { task.Id },
                                                                                        });
            foreach (var prop in extData.DataSection.Items)
            {
                var dto = exchangePropertyDtos.SingleOrDefault(ep => ep.ExtId == prop.ExtKey);

                if (dto == null) continue;
                double doubleValue;
                if ((double.TryParse(prop.Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out doubleValue)))
                {
                    yield return new ExchangeProperty2<ExchangePropertyDTO>
                                 {
                                     Dto = dto,
                                     Value = doubleValue
                                 };
                }
                else
                {
                    yield return new ExchangeProperty2<ExchangePropertyDTO>
                    {
                        Dto = dto,
                        Value = prop.Value
                    };
                }

            }
        }


        public static void Sync(this ExchangeObject<TypicalExchangeData> obj, ExecutionContext context, ExchangeTaskDTO task)
        {
            if (obj.Status == ExchangeStatus.NoData) return;
            var periodType = obj.HeaderSection.PeriodType;
            var timeStamp = obj.HeaderSection.TimeStamp;
            var serie = ExchangeHelper.GetOrCreateSerie(context, null, timeStamp, periodType);
            var seriesId = serie.Id;
            var result = GetSyncCommands(obj, seriesId, task);
            var exs = new List<Exception>();
            Action<Exception> onMultipleCommandException = e => exs.Add(e) ;
            try
            {
                new MultipleCommand(context, result) {OnException = onMultipleCommandException}.Execute();
                //foreach (var action in result) action(context);
            }
            finally
            {
                var logger = new MyLogger("exchangeLogger");
                if (exs.Any())
                {
                    ExchangeHelper.LogError(task, seriesId, null, null, "error", $@"Возникло {exs.Count} ошибок. См. файл лога", context);
                    foreach (var e in exs)
                    {
                        logger.WriteFullException(e, $@"Типовой импорт: Произошла ошибка за метку {seriesId}");
                    }
                }
                else
                {
                    ExchangeHelper.LogOk(task, seriesId, null, null, "Ок", context);
                    logger.Info($@"Типовой импорт: Импорт файла за метку {seriesId} удачно завершен");
                }
            }
        }

        public static IEnumerable<Action<ExecutionContext>> GetSyncCommands(ExchangeObject<TypicalExchangeData> doc, int seriesId, ExchangeTaskDTO task)
        {
            #region commands

            Func<SiteDTO, Action<ExecutionContext>> addSiteCommand = dto => ctx => new AddSiteCommand(ctx).Execute(
                new AddSiteParameterSet
                {
                    Name = dto.Name,
                    ParentId = dto.ParentId,
                    SortOrder = dto.SortOrder,
                    Id = dto.Id,
                    GasTransportSystemId = dto.SystemId
                });
            Func<SiteDTO, Action<ExecutionContext>> editSiteCommand = dto => ctx => new EditSiteCommand(ctx).Execute(
                new EditSiteParameterSet
                {
                    Name = dto.Name,
                    ParentId = dto.ParentId,
                    SortOrder = dto.SortOrder,
                    Id = dto.Id,
                    GasTransportSystemId = dto.SystemId
                });

            Func<CompStationDTO, Action<ExecutionContext>> addCstCommand =
                dto => ctx => new AddCompStationCommand(ctx).Execute(
                    new AddCompStationParameterSet
                    {
                        Name = dto.Name,
                        ParentId = dto.ParentId,
                        RegionId = dto.RegionId,
                        SortOrder = dto.SortOrder,
                        Id = dto.Id
                    });
            Func<CompStationDTO, Action<ExecutionContext>> editCstCommand =
                dto => ctx => new EditCompStationCommand(ctx).Execute(
                    new EditCompStationParameterSet
                    {
                        Name = dto.Name,
                        ParentId = dto.ParentId,
                        RegionId = dto.RegionId,
                        SortOrder = dto.SortOrder,
                        Id = dto.Id,
                    });

            Func<CompShopDTO, Action<ExecutionContext>> addCshCommand =
                dto => ctx => new AddCompShopCommand(ctx).Execute(
                    new AddCompShopParameterSet
                    {
                        Name = dto.Name,
                        ParentId = dto.ParentId,
                        SortOrder = dto.SortOrder,
                        KmOfConn = dto.KmOfConn ?? 0,
                        EngineClassId = dto.EngineClass,
                        PipelineId = dto.PipelineId,
                        PipingVolume = dto.PipingVolume ?? 0,
                        Id = dto.Id,
                    });
            Func<CompShopDTO, Action<ExecutionContext>> editCshCommand =
                dto => ctx => new EditCompShopCommand(ctx).Execute(
                    new EditCompShopParameterSet
                    {
                        Name = dto.Name,
                        ParentId = dto.ParentId,
                        SortOrder = dto.SortOrder,
                        KmOfConn = dto.KmOfConn ?? 0,
                        EngineClassId = dto.EngineClass,
                        PipelineId = dto.PipelineId,
                        PipingVolume = dto.PipingVolume ?? 0,
                        Id = dto.Id
                    });

            Func<CompUnitDTO, Action<ExecutionContext>> addCuCommand =
                dto => ctx => new AddCompUnitCommand(ctx).Execute(
                    new AddCompUnitParameterSet
                    {
                        Name = dto.Name,
                        ParentId = dto.ParentId,
                        SortOrder = dto.SortOrder,
                        CompUnitTypeId = dto.CompUnitTypeId,
                        DryMotoringConsumption = dto.DryMotoringConsumption,
                        HasRecoveryBoiler = dto.HasRecoveryBoiler,
                        InjectionProfileVolume = dto.InjectionProfileVolume,
                        SuperchargerTypeId = dto.SuperchargerTypeId,
                        TurbineStarterConsumption = dto.TurbineStarterConsumption,
                        IsVirtual = dto.IsVirtual,
                        Id = dto.Id,
                        BleedingRate = dto.BleedingRate,
                        InjectionProfilePiping = dto.InjectionProfilePiping,
                        SealingCount = dto.SealingCount,
                        SealingType = dto.SealingType ?? CompUnitSealingType.Dry,
                        StartValveConsumption = dto.StartValveConsumption,
                        StopValveConsumption = dto.StopValveConsumption,
                        ValveConsumptionDetails = dto.ValveConsumptionDetails
                    });
            Func<CompUnitDTO, Action<ExecutionContext>> editCuCommand =
                dto => ctx => new EditCompUnitCommand(ctx).Execute(
                    new EditCompUnitParameterSet
                    {
                        Name = dto.Name,
                        ParentId = dto.ParentId,
                        SortOrder = dto.SortOrder,
                        CompUnitTypeId = dto.CompUnitTypeId,
                        DryMotoringConsumption = dto.DryMotoringConsumption,
                        HasRecoveryBoiler = dto.HasRecoveryBoiler,
                        InjectionProfileVolume = dto.InjectionProfileVolume,
                        SuperchargerTypeId = dto.SuperchargerTypeId,
                        TurbineStarterConsumption = dto.TurbineStarterConsumption,
                        IsVirtual = dto.IsVirtual,
                        Id = dto.Id, 
                        BleedingRate = dto.BleedingRate,
                        InjectionProfilePiping = dto.InjectionProfilePiping,
                        SealingCount = dto.SealingCount,
                        SealingType = dto.SealingType ?? CompUnitSealingType.Dry, 
                        StartValveConsumption = dto.StartValveConsumption,
                        StopValveConsumption = dto.StopValveConsumption,
                        ValveConsumptionDetails = dto.ValveConsumptionDetails
                    });

            Func<DistrStationDTO, Action<ExecutionContext>> addDsCommand =
                dto => ctx => new AddDistrStationCommand(ctx).Execute(
                    new AddDistrStationParameterSet
                    {
                        Name = dto.Name,
                        ParentId = dto.ParentId,
                        RegionId = dto.RegionId,
                        CapacityRated = dto.CapacityRated,
                        SortOrder = dto.SortOrder,
                        IsVirtual = dto.IsVirtual,
                        PressureRated = dto.PressureRated,
                        Id = dto.Id
                    });

            Func<DistrStationDTO, Action<ExecutionContext>> editDsCommand =
                dto => ctx => new EditDistrStationCommand(ctx).Execute(
                    new EditDistrStationParameterSet
                    {
                        Name = dto.Name,
                        ParentId = dto.ParentId,
                        RegionId = dto.RegionId,
                        CapacityRated = dto.CapacityRated,
                        SortOrder = dto.SortOrder,
                        IsVirtual = dto.IsVirtual,
                        PressureRated = dto.PressureRated,
                        Id = dto.Id
                    });

            Func<DistrStationOutletDTO, Action<ExecutionContext>> addDsoCommand =
                dto => ctx => new AddDistrStationOutletCommand(ctx).Execute(
                    new AddDistrStationOutletParameterSet
                    {
                        Name = dto.Name,
                        ParentId = dto.ParentId,
                        SortOrder = dto.SortOrder,
                        IsVirtual = dto.IsVirtual,
                        CapacityRated = dto.CapacityRated,
                        PressureRated = dto.PressureRated,
                        Id = dto.Id
                    });
            Func<DistrStationOutletDTO, Action<ExecutionContext>> editDsoCommand =
                dto => ctx => new EditDistrStationOutletCommand(ctx).Execute(
                    new EditDistrStationOutletParameterSet
                    {
                        Name = dto.Name,
                        ParentId = dto.ParentId,
                        SortOrder = dto.SortOrder,
                        IsVirtual = dto.IsVirtual,
                        CapacityRated = dto.CapacityRated,
                        PressureRated = dto.PressureRated,
                        Id = dto.Id
                    });
            Func<MeasStationDTO, Action<ExecutionContext>> addMsCommand =
                dto => ctx => new AddMeasStationCommand(ctx).Execute(
                    new AddMeasStationParameterSet
                    {
                        Name = dto.Name,
                        ParentId = dto.ParentId,
                        SortOrder = dto.SortOrder,
                        IsVirtual = dto.IsVirtual,
                        BalanceSignId = dto.BalanceSignId,
                        Id = dto.Id
                    });
            Func<MeasStationDTO, Action<ExecutionContext>> editMsCommand =
                dto => ctx => new EditMeasStationCommand(ctx).Execute(
                    new EditMeasStationParameterSet
                    {
                        Name = dto.Name,
                        ParentId = dto.ParentId,
                        SortOrder = dto.SortOrder,
                        IsVirtual = dto.IsVirtual,
                        BalanceSignId = dto.BalanceSignId,
                        Id = dto.Id
                        //Status = dto.Status == EntityStatus.Deleted
                    });

            Func<MeasLineDTO, Action<ExecutionContext>> addMlCommand =
                dto => ctx => new AddMeasLineCommand(ctx).Execute(
                    new AddMeasLineParameterSet
                    {
                        Name = dto.Name,
                        ParentId = dto.ParentId,
                        SortOrder = dto.SortOrder,
                        IsVirtual = dto.IsVirtual,
                        KmOfConn = dto.KmOfConn,
                        PipelineId = dto.PipelineId,
                        Id = dto.Id
                    });
            Func<MeasLineDTO, Action<ExecutionContext>> editMlCommand =
                dto => ctx => new EditMeasLineCommand(ctx).Execute(
                    new EditMeasLineParameterSet
                    {
                        Name = dto.Name,
                        ParentId = dto.ParentId,
                        SortOrder = dto.SortOrder,
                        IsVirtual = dto.IsVirtual,
                        KmOfConn = dto.KmOfConn,
                        PipelineId = dto.PipelineId,
                        Id = dto.Id,
                        Status = dto.Status == EntityStatus.Deleted
                    });

            Func<ReducingStationDTO, Action<ExecutionContext>> addRsCommand =
                dto => ctx => new AddReducingStationCommand(ctx).Execute(
                    new AddReducingStationParameterSet
                    {
                        Name = dto.Name,
                        SortOrder = dto.SortOrder,
                        IsVirtual = dto.IsVirtual,
                        MainPipelineId = dto.PipelineId,
                        SiteId = dto.SiteId,
                        Kilometer = dto.Kilometer,
                        Id = dto.Id
                    });

            Func<ReducingStationDTO, Action<ExecutionContext>> editRsCommand =
                dto => ctx => new EditReducingStationCommand(ctx).Execute(
                    new EditReducingStationParameterSet
                    {
                        Name = dto.Name,
                        SortOrder = dto.SortOrder,
                        IsVirtual = dto.IsVirtual,
                        MainPipelineId = dto.PipelineId,
                        SiteId = dto.SiteId,
                        Kilometer = dto.Kilometer,
                        Id = dto.Id,
                        Status = dto.Status == EntityStatus.Deleted
                    });


            #endregion

            var exchangeSites = doc.DataSection.SiteDtos;
            foreach (var esite in exchangeSites)
            {
                var esite1 = esite;
                yield return CommandAction(esite1, addSiteCommand, editSiteCommand, seriesId, task);
                foreach (var ecst in esite.CompStationDTos)
                {
                    var ecst1 = ecst;
                    yield return CommandAction(ecst1, addCstCommand, editCstCommand, seriesId, task);

                    foreach (var ecsh in ecst.CompShops)
                    {
                        var ecsh1 = ecsh;
                        yield return CommandAction(ecsh1, addCshCommand, editCshCommand, seriesId, task);

                        foreach (var ecu in ecsh.CompUnits)
                        {
                            var ecu1 = ecu;
                            yield return CommandAction(ecu1, addCuCommand, editCuCommand, seriesId, task);
                        }
                    }
                }
                foreach (var eds in esite.DistrStationDtos)
                {
                    var eds1 = eds;
                    yield return CommandAction(eds1, addDsCommand, editDsCommand, seriesId, task);
                    //foreach (var ecm in eds.Consumers)
                    //{
                    //    var ecm1 = ecm;
                    //    yield return CommandAction(ecm1, addCnsCommand, editCnsCommand, seriesId);
                    //}
                    foreach (var edso in eds.DistrStationOutlets)
                    {
                        var edso1 = edso;
                        yield return CommandAction(edso1, addDsoCommand, editDsoCommand, seriesId, task);
                    }
                }
                foreach (var ems in esite.MeasStationDtos)
                {
                    var ems1 = ems;
                    yield return CommandAction(ems1, addMsCommand, editMsCommand, seriesId, task);

                    foreach (var eml in ems.MeasLines)
                    {
                        var eml1 = eml;
                        yield return CommandAction(eml1, addMlCommand, editMlCommand, seriesId, task);
                    }
                }
                foreach (var rs in esite.ReducingStationDtos)
                {
                    var rs1 = rs;
                    yield return CommandAction(rs1, addRsCommand, editRsCommand, seriesId, task);
                }
            }
        }

        private static Action<ExecutionContext> CommandAction<TDto>(ExchangeItem<TDto> edto, Func<TDto, Action<ExecutionContext>> addCommand, Func<TDto, Action<ExecutionContext>> editCommand, int seriesId, ExchangeTaskDTO task) where TDto : CommonEntityDTO, new()
        {
            return ctx =>
            {
                if (ctx.DoesEntityExist(edto)) editCommand(edto.Dto)(ctx);
                else addCommand(edto.Dto)(ctx);

                edto.Properties.ForEach(
                    prop =>
                        new SetExchangePropertyValueCommand(ctx).Execute(new SetPropertyValueParameterSet
                        {
                            SeriesId = seriesId,
                            EntityId = edto.Dto.Id,
                            PropertyTypeId = prop.PropertyType,
                            Value = prop.Value,
                            Annotation = $@"Файл:{task?.Id}"
                        }));
            };
        }




        private static bool DoesEntityExist<TDto>(this ExecutionContext context, ExchangeItem<TDto> edto)
            where TDto : CommonEntityDTO, new()
        {
            var dto = edto.Dto;
            EntityStatus? status = (dto as dynamic).Status;
            return status == EntityStatus.Deleted || new CheckEntityQuery(context).Execute(dto.Id);
        }
    }
}