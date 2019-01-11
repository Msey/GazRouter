using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GazRouter.Application.Helpers;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Appearance;
using GazRouter.DTO.Appearance.Positions;
using GazRouter.DTO.Appearance.Versions;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.Flobus.Model;
using GazRouter.Flobus.Support;
using GazRouter.Flobus.Utilites;
using GazRouter.Flobus.VM.Model;
using GazRouter.Flobus.VM.Serialization;
using JetBrains.Annotations;
using Utils.Extensions;
using GazRouter.Flobus.UiEntities.FloModel;
using GazRouter.DTO.ObjectModel.Segment;

namespace GazRouter.Flobus.VM.FloModel
{
    public class FloModelHelper
    {
        private static readonly Dictionary<Guid, ValveJson> ValveJsonDict = new Dictionary<Guid, ValveJson>();

        private static readonly Dictionary<Guid, DistributingStationJson> DistributingStationJsonsDict =
            new Dictionary<Guid, DistributingStationJson>();

        private static readonly Dictionary<Guid, ReducingStationJson> ReducingStationJsonsDict =
            new Dictionary<Guid, ReducingStationJson>();

        private static readonly Dictionary<Guid, MeasuringLineJson> MeasuringLineJsonsDict =
            new Dictionary<Guid, MeasuringLineJson>();

        public static async Task LoadMeasuringsAsync(DateTime? date, SchemeViewModel model, PeriodType pType = PeriodType.Twohours )
        {
            if (model == null)
            {
                return;
            }

            var timestamp = date ?? SeriesHelper.GetLastCompletedSession();
            if (pType == PeriodType.Day)
                timestamp = new DateTime(timestamp.Year, timestamp.Month, timestamp.Day).ToLocal();

            if (pType == PeriodType.Twohours)
            {
                var data = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(

                    new GetEntityPropertyValueListParameterSet
                    {
                        PeriodType = pType,
                        StartDate = timestamp.ToLocal(),
                        EndDate = timestamp.ToLocal(),
                        CreateEmpty = true,
                        LoadMessages = true
                    }
                ).ConfigureAwait(true);

                ParseMeasurings(model, data, timestamp);
            }
            else
            {
                var data = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                new GetEntityPropertyValueListParameterSet
                {
                    PeriodType = pType,
                    Year = timestamp.Year,
                    Month = timestamp.Month,
                    Day = timestamp.Day,
                    CreateEmpty = true,
                    LoadMessages = true
                }
                ).ConfigureAwait(true);

                ParseMeasurings(model, data, timestamp);

                
                DateTime date10 = new DateTime(timestamp.Year, timestamp.Month, timestamp.Day, 10, 0, 0).ToLocal();

                var data10 = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(

                    new GetEntityPropertyValueListParameterSet
                    {
                        PeriodType = PeriodType.Twohours,
                        StartDate = date10,
                        EndDate = date10,
                        CreateEmpty = false,
                        LoadMessages = true,
                        PropertyList = new List<PropertyType>() {
                            PropertyType.CompressorUnitsInReserve,
                            PropertyType.CompressorUnitsInUse,
                            PropertyType.CompressorUnitsUnderRepair,
                            PropertyType.CoolingUnitsInReserve,
                            PropertyType.CoolingUnitsInUse,
                            PropertyType.CoolingUnitsInUseAddOn,
                            PropertyType.CoolingUnitsUnderRepair,
                            PropertyType.DustCatchersInReserve,
                            PropertyType.DustCatchersInUse,
                            PropertyType.DustCatchersUnderRepair,
                            PropertyType.StateBypass1,
                            PropertyType.StateBypass2,
                            PropertyType.StateBypass3,
                            PropertyType.StateCompressorShop,
                            PropertyType.StateValve,
                            PropertyType.TurborefrigeratingUnitsInReserve,
                            PropertyType.TurborefrigeratingUnitsInUse,
                            PropertyType.TurborefrigeratingUnitsUnderRepair
                        }
                    } 
                ).ConfigureAwait(true);
                

                ParseMeasurings(model, data10, date10, false);
                
            }
        }

        /// <summary>
        ///     Сохранение модели (сохраняется не все, только расположение объектов на схеме и параметры отображения)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static async Task<int> SaveModelAsync(SchemeViewModel model)
        {
            var svps = new SchemeVersionParameterSet
            {
                SchemeId = model.SchemeInfo.SchemeId,
                Content = model.Save()
            };

            model.IsChanged = false;
            var versionId = await new SchemeServiceProxy().AddSchemeVersionAsync(svps);
            model.SchemeInfo.VersionId = versionId;
            return versionId;
        }

        internal SchemeViewModel ParseModel(SchemeModelDTO schemeModelDto)
        {
            ValveJsonDict.Clear();
            DistributingStationJsonsDict.Clear();
            MeasuringLineJsonsDict.Clear();
            ReducingStationJsonsDict.Clear();

            Debug.WriteLine("Начинаем парсить модель");
            var model = new SchemeViewModel(schemeModelDto)
            {
                SchemeInfo = new SchemeVersion
                {
                    SchemeId = schemeModelDto.SchemeVersion.SchemeId,
                    SchemeName = schemeModelDto.SchemeVersion.SchemeName,
                    Creator = schemeModelDto.SchemeVersion.CreatorName,
                    CreationDate = schemeModelDto.SchemeVersion.CreateDate,
                    IsPublished = schemeModelDto.SchemeVersion.IsPublished,
                    VersionId = schemeModelDto.SchemeVersion.Id
                }
            };

            var pipelineIndexes = new Dictionary<Guid, Pipeline>();

            Debug.WriteLine("Грузим json");           
            var json = schemeModelDto.SchemeVersion.Content != null ? 
                model.Load(schemeModelDto.SchemeVersion.Content) : null;
            var positionIndex = new Dictionary<Guid, PositionDTO>();
            ParsePipelines(model, schemeModelDto, pipelineIndexes, json, positionIndex);
            ParseCompShops(model, schemeModelDto, json, positionIndex);
            if (json != null)
            {
                ParseTextBlocks(model, json);
                ParsePolyLine(model, json);
                ParseCheckValve(model, json);
            }
            model.IsChanged = false;
            Debug.WriteLine("возвращаем модель");
            return model;
        }

        private static void ParsePipelineConnections(SchemeViewModel model, SchemeModelDTO schemeModelDto,
            Dictionary<Guid, Pipeline> pipelineIndexes)
        {
            foreach (var pipelineConnDTO in schemeModelDto.PipelineConnList)
            {
                if (!pipelineConnDTO.DestPipelineId.HasValue)
                {
                    continue;
                }
                var pipeline = pipelineIndexes.GetOrDefault(pipelineConnDTO.PipelineId);
                var destPipe = pipelineIndexes.GetOrDefault(pipelineConnDTO.DestPipelineId.Value);
                if (destPipe != null && pipeline != null && pipelineConnDTO.Kilometr >= destPipe.KmBegining &&
                    pipelineConnDTO.Kilometr <= destPipe.KmEnd)
                {
                    model.PipelineConnections.Add(destPipe.AddPipelineConnection(pipelineConnDTO, pipeline));
                }
            }
        }

        private static void ParseCompShops(SchemeViewModel model, SchemeModelDTO schemeModelDto,
            [CanBeNull] SchemeJson json, Dictionary<Guid, PositionDTO> positionIndex)
        {
            var compShopsDic = schemeModelDto.CompShopList.ToDictionary(k => k.Id, k => k);
            if (json != null)
            {
                foreach (var csJson in json.CompressorShops)
                {
                    var compShop = compShopsDic.GetOrDefault(csJson.Id);
                    if (compShop != null)
                    {
                        var cs = new CompressorShop(compShop, csJson.Position);
                        model.CompressorShops.Add(cs);
                        var num = 1;
                        foreach (
                            var compUnit in
                                schemeModelDto.CompUnitList.Where(c => c.ParentId == compShop.Id)
                                    .OrderBy(u => u.SortOrder))
                        {
                            cs.CompressorUnits.Add(new CompressorUnit(compUnit, num));
                            num++;
                        }
                    }
                }
            }
            else
            {
                foreach (var compShop in schemeModelDto.CompShopList)
                {
                    var shop = compShop;
                    var positionDTO = positionIndex.GetOrDefault(shop.Id);
                    if (positionDTO == null)
                    {
                        continue;
                    }

                    var point = new Point(positionDTO.X, positionDTO.Y);
                    var cs = new CompressorShop(compShop, point);
                    model.CompressorShops.Add(cs);
                }
            }
        }

        private static int counter = 1;
        private static Guid getGuid
        {
            get
            {
                string gi = "FFFF" + counter.ToString();
                counter++;
                while (gi.Length < 32)
                    gi = "0" + gi;
                return new Guid(gi);
            }
        }

        private static Pipeline SegmentsOverlay(double start, double end)
        {
            Pipeline pb = new Pipeline(new PipelineDTO()
            {
                EntityType = DTO.Dictionaries.EntityTypes.EntityType.PipelineGroup,
                KilometerOfStartPoint = start > end ? end  : start,
                KilometerOfEndPoint = start > end ? start : end,
                Type = DTO.Dictionaries.PipelineTypes.PipelineType.Main,
                Id = getGuid
            });
            pb.Color = pb.BreakColor;
            pb.Thickness = 5;
            //pb.ZIndex

            return pb;
        }


        private static Point GetPointCoordinate(double bKm, PositionDTO point1, PositionDTO point2)
        {
            if (point1.Km == point2.Km)
                return new Point(point1.X, point1.Y);
            else if (bKm == point1.Km)
                return new Point(point1.X, point1.Y);
            else if (bKm == point2.Km)
                return new Point(point2.X, point2.Y); 
            else
            {
                if (point1.X == point2.X)
                    return new Point(point1.X, point1.Y + (((point1.Y - point2.Y) / (point2.Km - point1.Km)) * (point1.Km - bKm)));
                else
                    return new Point(point1.X + (((point1.X - point2.X) / (point2.Km - point1.Km)) * (point1.Km - bKm)), point1.Y);
            }
        }


        private static PointJson GetPointCoordinate(double bKm, PointJson point1, PointJson point2)
        {
            if (point1.Km == point2.Km)
                return new PointJson(new Point(point1.Point.X, point1.Point.Y), bKm);
            else if (bKm == point1.Km)
                return new PointJson(new Point(point1.Point.X, point1.Point.Y), bKm);
            else if (bKm == point2.Km)
                return new PointJson(new Point(point2.Point.X, point2.Point.Y), bKm);
            else
            {
                if (point1.Point.X == point2.Point.X)
                    return new PointJson(new Point(point1.Point.X, point1.Point.Y + (((point1.Point.Y - point2.Point.Y) / (point2.Km - point1.Km)) * (point1.Km - bKm))), bKm);
                else
                    return new PointJson(new Point(point1.Point.X + (((point1.Point.X - point2.Point.X) / (point2.Km - point1.Km)) * (point1.Km - bKm)), point1.Point.Y), bKm);
            }
        }

        /// <summary>
        ///     Газопроводы
        /// </summary>
        private static void ParsePipelines(SchemeViewModel model, SchemeModelDTO schemeModelDto,
            Dictionary<Guid, Pipeline> pipelineIndexes, [CanBeNull] SchemeJson json,
            Dictionary<Guid, PositionDTO> positionIndex)
        {

            var pipelineDictionary = schemeModelDto.PipelineList.ToDictionary(k => k.Id, v => v);
            if (json != null)
            {
                ParsePipelineJson(model, pipelineIndexes, json, pipelineDictionary, schemeModelDto);
            }
            else
            {
                foreach (var group in schemeModelDto.SchemeVersion.Positions.GroupBy(p => p.EntityId))
                {
                    var positions = group.OrderBy(p => p.Km).ToList();
                    var dto = pipelineDictionary.GetOrDefault(group.Key);
                    if (dto == null)
                    {
                        foreach (var positionDTO in positions)
                        {
                            positionIndex.Add(positionDTO.EntityId, positionDTO);
                        }

                        continue;
                    }
                    if (positions.Count < 2)
                    {
                        continue;
                    }
                    var pipeline = pipelineIndexes.GetOrDefault(group.Key);
                    if (pipeline == null)
                    {
                        pipeline = new Pipeline(dto);
                        pipelineIndexes.Add(dto.Id, pipeline);
                    }

                    pipeline.StartPoint = new Point(positions[0].X, positions[0].Y);
                    pipeline.EndPoint = new Point(positions[positions.Count - 1].X, positions[positions.Count - 1].Y);
                    for (var index = 1; index < positions.Count - 1; index++)
                    {
                        var positionDTO = positions[index];

                        if (positionDTO.Km > pipeline.KmBegining && positionDTO.Km < pipeline.KmEnd)
                        {
                            pipeline.AddPoint(positionDTO.Km, new Point(positionDTO.X, positionDTO.Y));
                        }
                    }

                    List<Pipeline> breaks = new List<Pipeline>();

                    //Pipeline pp = null;
                    var segments = schemeModelDto.DiameterSegments.Where(o => o.PipelineId == pipeline.Id).OrderBy(o => o.KilometerOfStartPoint).ToList();
                    if (segments != null && segments.Count > 0)
                    {
                        pipeline.DiameterSegments = new List<UiEntities.FloModel.PipelineDiameterSegment>();

                        foreach (var sgmnt in segments)
                        {
                            pipeline.DiameterSegments.Add(new UiEntities.FloModel.PipelineDiameterSegment()
                            {
                                DiameterConv = sgmnt.DiameterConv,
                                DiameterId = sgmnt.DiameterId,
                                ExternalDiameterId = sgmnt.ExternalDiameterId,
                                DiameterName = sgmnt.DiameterName,
                                DiameterReal = sgmnt.DiameterReal,
                                KmBegining = sgmnt.KilometerOfStartPoint,
                                KmEnd = sgmnt.KilometerOfEndPoint
                            });
                        }
                    }

                    model.Pipelines.Add(pipeline);
                }

                foreach (var style in schemeModelDto.SchemeVersion.Styles)
                {
                    if (style != null)
                    {
                        var pipe = pipelineIndexes[style.EntityId];
                        pipe.Color = ColorConverter.FromHex((uint) style.Color);
                        pipe.Thickness = style.Size;
                    }
                }
            }

            // Создать список кранов для газопровода
            foreach (var valveDto in schemeModelDto.ValveList)
            {
                if (valveDto.ParentId == null)
                {
                    continue;
                }

                var pipeline = pipelineIndexes.GetOrDefault(valveDto.ParentId.Value);

                if (pipeline == null)
                {
                    continue;
                }
                var valve = pipeline.AddValve(valveDto);
                if (ValveJsonDict.ContainsKey(valve.Id))
                {
                    valve.TextAngle = ValveJsonDict[valve.Id].TextAngle;
                }
                model.Valves.Add(valve);
            }

            foreach (var reducingStationDTO in schemeModelDto.ReducingStationList)
            {
                var pipeline = pipelineIndexes.GetOrDefault(reducingStationDTO.PipelineId);

                if (pipeline != null)
                {
                    var station = pipeline.AddReducingStation(reducingStationDTO);
                    if (ReducingStationJsonsDict.ContainsKey(station.Id))
                    {
                        station.TextAngle = ReducingStationJsonsDict[station.Id].TextAngle;
                        station.Hidden = ReducingStationJsonsDict[station.Id].Hidden;
                        station.ContainerPosition = ReducingStationJsonsDict[station.Id].ContainerPosition;
                    }
                    model.ReducingStations.Add(station);
                }
            }

            foreach (var measLine in schemeModelDto.MeasLineList)
            {
                var pipeline = pipelineIndexes.GetOrDefault(measLine.PipelineId);

                if (pipeline != null)
                {
                    var measuringLine = pipeline.AddMeasuringLine(measLine);
                    if (MeasuringLineJsonsDict.ContainsKey(measuringLine.Id))
                    {
                        measuringLine.TextAngle = MeasuringLineJsonsDict[measuringLine.Id].TextAngle;
                        measuringLine.Hidden = MeasuringLineJsonsDict[measuringLine.Id].Hidden;
                        measuringLine.ContainerPosition = MeasuringLineJsonsDict[measuringLine.Id].ContainerPosition;
                    }
                    model.MeasuringLines.Add(measuringLine);
                }
            }

            foreach (var distrStation in schemeModelDto.DistrStationList)
            {
                var pl = schemeModelDto.PipelineConnList.FirstOrDefault(pc => pc.DistrStationId == distrStation.Id);

                if (pl != null)
                {
                    var pipeline = pipelineIndexes.GetOrDefault(pl.PipelineId);
                    if (pipeline != null)
                    {
                        var ds = pipeline.AddDistributingStation(distrStation);
                        if (DistributingStationJsonsDict.ContainsKey(ds.Id))
                        {
                            ds.TextAngle = DistributingStationJsonsDict[ds.Id].TextAngle;
                            ds.Hidden = DistributingStationJsonsDict[ds.Id].Hidden;
                            ds.ContainerPosition = DistributingStationJsonsDict[ds.Id].ContainerPosition;
                        }
                        model.DistributingStations.Add(ds);
                    }
                }
            }

            ParsePipelineConnections(model, schemeModelDto, pipelineIndexes);
        }

        private static void ParsePipelineJson(SchemeViewModel model, Dictionary<Guid, Pipeline> pipelineIndexes,
            SchemeJson json,
            Dictionary<Guid, PipelineDTO> pipelineDictionary, SchemeModelDTO schemeModelDto)
        {
            foreach (var pipelineJson in json.Pipelines)
            {
                var pipelineDto = pipelineDictionary.GetOrDefault(pipelineJson.Id);
                if (pipelineDto != null)
                {
                    var pipeline = new Pipeline(pipelineDto)
                    {
                        Color = pipelineJson.Color,
                        Thickness = pipelineJson.Thickness,
                        StartPoint = pipelineJson.StartPoint.Round()
                    };

                    foreach (var pointJson in pipelineJson.IntermediatePoints)
                    {
                        if (pointJson.Km > pipeline.KmBegining && pointJson.Km < pipeline.KmEnd)
                        {
                            pipeline.AddPoint(pointJson.Km, pointJson.Point.Round());
                        }
                    }

                    foreach (var valveJson in pipelineJson.Valves)
                    {
                        ValveJsonDict.Add(valveJson.Id, valveJson);
                    }

                    foreach (var distributingStationJson in pipelineJson.DistributingStations)
                    {
                        DistributingStationJsonsDict.Add(distributingStationJson.Id, distributingStationJson);
                    }

                    foreach (var distributingStationJson in pipelineJson.ReducingStations)
                    {
                        ReducingStationJsonsDict.Add(distributingStationJson.Id, distributingStationJson);
                    }

                    foreach (var measuringLinesJson in pipelineJson.MeasuringLines)
                    {
                        if (!MeasuringLineJsonsDict.ContainsKey(measuringLinesJson.Id))
                        MeasuringLineJsonsDict.Add(measuringLinesJson.Id, measuringLinesJson);
                    }
                    pipeline.EndPoint = pipelineJson.EndPoint.Round();
                    model.Pipelines.Add(pipeline);
                    pipelineIndexes[pipelineJson.Id] = pipeline;


                    List<Pipeline> breaks = new List<Pipeline>();

                    var segments = schemeModelDto.DiameterSegments.Where(o => o.PipelineId == pipeline.Id).OrderBy(o => o.KilometerOfStartPoint).ToList();
                    if (segments != null && segments.Count > 0)
                    {
                        pipeline.DiameterSegments = new List<UiEntities.FloModel.PipelineDiameterSegment>();

                        foreach (var sgmnt in segments)
                        {
                            pipeline.DiameterSegments.Add(new UiEntities.FloModel.PipelineDiameterSegment()
                            {
                                DiameterConv = sgmnt.DiameterConv,
                                ExternalDiameter = sgmnt.ExternalDiameter,
                                DiameterId = sgmnt.DiameterId,
                                ExternalDiameterId = sgmnt.ExternalDiameterId,
                                DiameterName = sgmnt.DiameterName,
                                DiameterReal = sgmnt.DiameterReal,
                                KmBegining = sgmnt.KilometerOfStartPoint,
                                KmEnd = sgmnt.KilometerOfEndPoint
                            });
                        }                        
                    }
                }
            }
        }

        private static void ParseMeasurings(
            SchemeViewModel model,
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> measurings,
            DateTime timestamp, bool showAll = true)
        {
            try
            {
                if (showAll)
                // Параметры по ГРС
                foreach (var distrStation in model.DistributingStations)
                {
                    var prop = measurings.GetOrDefault(distrStation.Id);
                    distrStation.DistributingStationMeasuring.PressureInlet.Extract(prop, timestamp);
                    distrStation.DistributingStationMeasuring.TemperatureInlet.Extract(prop, timestamp);
                    distrStation.DistributingStationMeasuring.Flow.Extract(prop, timestamp);
                    distrStation.DistributingStationMeasuring.PressureOutlets.ForEach(o => o.PressureOutlet.Extract(measurings.GetOrDefault(o.Dto.Id), timestamp));
                }
                // Параметры по КЦ
                foreach (var compShop in model.CompressorShops)
                {
                    if (showAll)
                    {
                        var prop = measurings.GetOrDefault(compShop.Id);
                        compShop.CompressorShopMeasuring.PressureInlet.Extract(prop, timestamp);
                        compShop.CompressorShopMeasuring.PressureOutlet.Extract(prop, timestamp);
                        compShop.CompressorShopMeasuring.CompressionRatio.Extract(prop, timestamp);

                        compShop.CompressorShopMeasuring.TemperatureInlet.Extract(prop, timestamp);
                        compShop.CompressorShopMeasuring.TemperatureOutlet.Extract(prop, timestamp);
                        compShop.CompressorShopMeasuring.TemperatureCooling.Extract(prop, timestamp);

                        compShop.CompressorShopMeasuring.FuelGasConsumption.Extract(prop, timestamp);
                        compShop.CompressorShopMeasuring.Pumping.Extract(prop, timestamp);
                        compShop.CompressorShopMeasuring.Pattern.Extract(measurings, timestamp);
                    }
                    // По ГПА
                    foreach (var compUnit in compShop.CompressorUnits)
                    {
                        compUnit.CompressorUnitMeasuring.CompressorUnitState.Extract(measurings, timestamp);
                        compUnit.CompressorUnitMeasuring.StateChangingTimestamp.Extract(measurings, timestamp);
                    }
                }

                // Параметры по кранам
                foreach (var valve in  model.Valves)
                {
                    valve.ValveMeasuring.StateBypass1.Extract(measurings, timestamp);
                    valve.ValveMeasuring.StateBypass2.Extract(measurings, timestamp);
                    valve.ValveMeasuring.StateBypass3.Extract(measurings, timestamp);
                    valve.ValveMeasuring.State.Extract(measurings, timestamp);
                    valve.ValveMeasuring.StateChangingTimestamp.Extract(measurings, timestamp);
                }

                if (showAll)
                {
                    // Параметры по линиям замера газа
                    foreach (var measLine in model.MeasuringLines)
                    {
                        measLine.MeasuringLineMeasuring.Pressure.Extract(measurings, timestamp);
                        measLine.MeasuringLineMeasuring.Temperature.Extract(measurings, timestamp);
                        measLine.MeasuringLineMeasuring.Flow.Extract(measurings, timestamp);
                    }

                    // Параметры по узлу редуцирования газа

                    foreach (var reducingStation in model.ReducingStations)
                    {
                        reducingStation.ReducingStationMeasuring.PressureInlet.Extract(measurings, timestamp);
                        reducingStation.ReducingStationMeasuring.PressureOutlet.Extract(measurings, timestamp);
                        reducingStation.ReducingStationMeasuring.TemperatureInlet.Extract(measurings, timestamp);
                        reducingStation.ReducingStationMeasuring.TemperatureOutlet.Extract(measurings, timestamp);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ParseTextBlocks(SchemeViewModel model, [NotNull] SchemeJson json)
        {
            foreach (var tb in json.TextBlocks)
            {
                var textBlock = new TextBlock(tb);
                model.TextBlocks.Add(textBlock);
            }
        }

        private void ParsePolyLine(SchemeViewModel model, [NotNull] SchemeJson json)
        {
            foreach (var pl in json.PolyLines)
            {
                var polyline = new PolyLine(pl);
                model.PolyLines.Add(polyline);
            }
        }
        private void ParseCheckValve(SchemeViewModel model, [NotNull] SchemeJson json)
        {
            foreach (var cv in json.CheckValves)
            {
                var check_valve = new CheckValve(cv);
                model.CheckValves.Add(check_valve);
            }
        }
    }
}