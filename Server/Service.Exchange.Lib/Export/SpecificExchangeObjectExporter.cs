using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DAL.Core;
using GazRouter.DAL.DataExchange.ExchangeEntity;
using GazRouter.DAL.DataExchange.ExchangeProperty;
using GazRouter.DAL.DataExchange.ExchangeTask;
using GazRouter.DAL.ObjectModel.Aggregators;
using GazRouter.DAL.ObjectModel.BoilerPlants;
using GazRouter.DAL.ObjectModel.Boilers;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.CompUnits;
using GazRouter.DAL.ObjectModel.Consumers;
using GazRouter.DAL.ObjectModel.CoolingStations;
using GazRouter.DAL.ObjectModel.CoolingUnit;
using GazRouter.DAL.ObjectModel.DistrStationOutlets;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DAL.ObjectModel.MeasLine;
using GazRouter.DAL.ObjectModel.MeasPoint;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DAL.ObjectModel.OperConsumers;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DAL.ObjectModel.PowerPlants;
using GazRouter.DAL.ObjectModel.PowerUnits;
using GazRouter.DAL.ObjectModel.ReducingStations;
using GazRouter.DAL.ObjectModel.Sites;
using GazRouter.DAL.ObjectModel.Valves;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeProperty;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;

namespace GazRouter.Service.Exchange.Lib.Export
{
    public class SpecificExchangeObjectExporter : ExchangeObjectExporterBase
    {
        protected List<ExchangeEntityDTO> _extEntities;
        protected List<ExchangePropertyDTO> _extProperties;

        public SpecificExchangeObjectExporter(ExecutionContext context, ExchangeTaskDTO task) : base(context)
        {
            _task = task;
            _periodTypeId = task.PeriodTypeId;
            _extEntities =
                new GetExchangeEntityListQuery(_context).Execute(new GetExchangeEntityListParameterSet
                                                                 {
                                                                     ExchangeTaskIdList = new List<int>{_task.Id},
                                                                     IsActive = true
                                                                 });

            _extProperties =
                new GetExchangePropertyListQuery(_context).Execute(new GetExchangeEntityListParameterSet
                                                                   {
                                                                       ExchangeTaskIdList = new List<int> {_task.Id}, 
                                                                       IsActive = true
                                                                   });

        }

        public SpecificExchangeObjectExporter(ExecutionContext context, int cfgId)
            : this(context, new GetExchangeTaskListQuery(context).Execute(new GetExchangeTaskListParameterSet {Id = cfgId}).FirstOrDefault())
        {
        }


        protected override ExchangeItem<TDto> GetExchangeItem<TDto>(TDto dto) 
        {
            return new ExchangeItem<TDto>
                   {
                       Dto = dto,
                       ExtKey = _extEntities.Where(b => b.EntityId == dto.Id).Select(b => b.ExtId).SingleOrDefault(),
                       Properties = GetProperties(dto.Id)
                   };
        }

        protected override List<ExchangeProperty> GetProperties(Guid entityId)
        {
            IEnumerable<ExchangeProperty> notExtProperties = new List<ExchangeProperty>();
            var extProperties = 
                _extProperties
                .Where(ep => ep.EntityId == entityId)
                .Where(ep => ep.ExchangeTaskId == _task.Id)
                .Select(GetExchangeProperties);
            if (_data.ContainsKey(entityId))
            {
                notExtProperties = _data[entityId]
                    .Where(pair => extProperties.All(ep => ep.PropertyType != pair.Key))
                    .Select(pair => GetExchangeProperties(entityId, pair));
            }
            return extProperties.Union(notExtProperties).ToList();
        }

        private ExchangeProperty GetExchangeProperties(Guid entityId, KeyValuePair<PropertyType, List<BasePropertyValueDTO>> pair)
        {
            var propertyTypeDTO = _propertyTypes.Single(et => et.PropertyType == pair.Key);
            dynamic value = null;
            if (_data.ContainsKey(entityId) && _data[entityId].ContainsKey(propertyTypeDTO.PropertyType))
            {
                var val = _data[entityId][propertyTypeDTO.PropertyType].FirstOrDefault();
                value = ExchangeHelper.GetValue(val);
            }
            return GetExchangeProperty(entityId, value, propertyTypeDTO, null);
        }
        private ExchangeProperty GetExchangeProperties(ExchangePropertyDTO dto)
        {
            var entityId = dto.EntityId;
            var extId = dto.ExtId;
            var propertyTypeDTO = _propertyTypes.Single(et => et.PropertyType == dto.PropertyTypeId);
            dynamic value = null;
            if (_data.ContainsKey(entityId) && _data[entityId].ContainsKey(propertyTypeDTO.PropertyType))
            {
                var val = _data[entityId][propertyTypeDTO.PropertyType].FirstOrDefault();
                value = ExchangeHelper.GetValue(val);
            }
            return GetExchangeProperty(entityId, value, propertyTypeDTO, extId);
        }

        protected override List<ExchangeItem<TDto>> GetExchangeItems<TDto>(QueryReader<Guid, TDto> query, EntityType currentEntityType)
        {
            var result = Enumerable.Union(
                            _extEntities.Select(ee => new {ee.EntityId, ee.EntityTypeId}), 
                            _extProperties.Select(ep => new {ep.EntityId, ep.EntityTypeId}))
                            .Distinct()
                            .Where(e => e.EntityTypeId == currentEntityType)
                            .Select(e =>
                                    {
                                        var dto = query.Execute(e.EntityId);
                                        return GetExchangeItem(dto);
                                    }).ToList();
            return result;
        }


        //protected ExchangeProperty GetExchangeProperty(Guid entityId, dynamic value, PropertyTypeDTO propertyTypeDTO)
        //{
        //    var extKey = _extProperties
        //        .Where(p => p.EntityId == entityId)
        //        .Where(p => p.ExchangeTaskId == _task.Id)
        //        .Where(p => p.PropertyTypeId == propertyTypeDTO.PropertyType)
        //        .Select(p => p.ExtId)
        //        .FirstOrDefault();
        //    if (!string.IsNullOrEmpty(extKey) || !Object.ReferenceEquals(null, value))
        //    {
        //        return new ExchangeProperty
        //        {
        //            SysName = propertyTypeDTO.SysName,
        //            Value = value,
        //            PropertyType = propertyTypeDTO.PropertyType,
        //            ExtKey = extKey
        //        };
                
        //    }
        //    return null;
        //}

        public ExchangeObject<SpecificExchangeData> Export(DateTime dt)
        {
            string dateFormat;
            //dt = ExchangeHelper.AdjustTimeStamp(_task.FileNameMask, dt, out dateFormat);
            var serie = ExchangeHelper.GetSerie(_context, dt: dt, periodTypeId : _periodTypeId);
            this._startDate = dt;
            Debug.Assert(_periodTypeId != null, "_periodTypeId != null");
            _endDate = _startDate.Add(((PeriodType)_periodTypeId).ToTimeSpan());
            if (serie == null)
            {
                return GetErrorData();
            }
            return Export(serie);
        }

        private ExchangeObject<SpecificExchangeData> GetErrorData()
        {
            return new ExchangeObject<SpecificExchangeData>
            {
                HeaderSection = GetExchangeHeader(),
                Status = ExchangeStatus.NoData
            };
        }


        public ExchangeObject<SpecificExchangeData> Export(SeriesDTO serie)
        {
            if (serie == null)
            {
                return GetErrorData();
            }

            var result = base.Build<SpecificExchangeData>(serie);
            if (_task.IsSql)
            {
                var raw = new RunProcCommand(_context).Execute(new RunProcParameterSet
                {
                    TimeStamp = serie.KeyDate,
                    TaskId = _task.Id,
                    ProcedureName = _task.SqlProcedureName
                });
                var root = XDocument.Parse($@"<root>{raw}</root>").Root;
                var descendantNodes = root.DescendantNodes();
                var elements = descendantNodes.OfType<XElement>().ToList();

                result.DataSection.RawData.Nodes = elements.Any()
                    ? elements
                    : descendantNodes.OfType<XText>().Select(xt => new XElement("text", xt.Value)).ToList();

            }
            result.DataSection.CompStationDTOs = GetExchangeItems(new GetCompStationByIdQuery(_context), EntityType.CompStation);
            result.DataSection.CompShopDtos = GetExchangeItems(new GetCompShopByIdQuery(_context), EntityType.CompShop);
            result.DataSection.DistrStationDtos = GetExchangeItems(new GetDistrStationByIdQuery(_context), EntityType.DistrStation);
            result.DataSection.MeasPointDtos = GetExchangeItems(new GetMeasPointByIdQuery(_context), EntityType.MeasPoint);
            result.DataSection.MeasLineDtos = GetExchangeItems(new GetMeasLineByIdQuery(_context), EntityType.MeasLine);
            result.DataSection.MeasStationDtos = GetExchangeItems(new GetMeasStationByIdQuery(_context), EntityType.MeasStation);
            result.DataSection.CompUnitDtos = GetExchangeItems(new GetCompUnitByIdQuery(_context), EntityType.CompUnit);
            result.DataSection.ReducingStationDtos = GetExchangeItems(new GetReducingStationByIdQuery(_context), EntityType.ReducingStation);
            result.DataSection.ValveDtos = GetExchangeItems(new GetValveByIdQuery(_context), EntityType.Valve);
            result.DataSection.SiteDtos = GetExchangeItems(new GetSiteByIdQuery(_context), EntityType.Site);
            result.DataSection.DistrStationOutletDtos = GetExchangeItems(new GetDistrStationOutletByIdQuery(_context), EntityType.DistrStationOutlet);
            result.DataSection.CoolingStations = GetExchangeItems(new GetCoolingStationByIdQuery(_context), EntityType.CoolingStation);
            result.DataSection.CoolingUnits = GetExchangeItems(new GetCoolingUnitByIdQuery(_context), EntityType.CoolingUnit);
            result.DataSection.OperConsumers = GetExchangeItems(new GetOperConsumerByIdQuery(_context), EntityType.OperConsumer);
            result.DataSection.Aggregators = GetExchangeItems(new GetAggregatorByIdQuery(_context), EntityType.Aggregator);
            result.DataSection.Boilers = GetExchangeItems(new GetBoilerByIdQuery(_context), EntityType.Boiler);
            result.DataSection.BoilerPlants = GetExchangeItems(new GetBoilerPlantByIdQuery(_context), EntityType.BoilerPlant);
            result.DataSection.Consumers = GetExchangeItems(new GetConsumerByIdQuery(_context), EntityType.Consumer);
            result.DataSection.PowerPlants = GetExchangeItems(new GetPowerPlantByIdQuery(_context), EntityType.PowerPlant);
            result.DataSection.PowerUnits = GetExchangeItems(new GetPowerUnitByIdQuery(_context), EntityType.PowerUnit);
            //result.DataSection.Pipelines = GetExchangeItems(new GetPipelineByIdQuery(_context), EntityType.Pipeline);

            return result;
        }
    }
}