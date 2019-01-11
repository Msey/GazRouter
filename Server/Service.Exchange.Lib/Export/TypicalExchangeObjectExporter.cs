using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DAL.Core;
using GazRouter.DAL.DataExchange.ExchangeEntity;
using GazRouter.DAL.DataExchange.ExchangeTask;
using GazRouter.DAL.Dictionaries.Enterprises;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.CompUnits;
using GazRouter.DAL.ObjectModel.DistrStationOutlets;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DAL.ObjectModel.MeasLine;
using GazRouter.DAL.ObjectModel.MeasPoint;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DAL.ObjectModel.ReducingStations;
using GazRouter.DAL.ObjectModel.Sites;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.Series;

namespace GazRouter.Service.Exchange.Lib.Export
{
    public class TypicalExchangeObjectExporter : ExchangeObjectExporterBase
    {
        public EnterpriseDTO Enterprise { get; }
        private readonly List<ExchangeEntityDTO> _exchangeEntities;

        public TypicalExchangeObjectExporter(ExecutionContext context, EnterpriseDTO enterprise) : base(context)
        {
            Enterprise = enterprise;
            _exchangeEntities = GetExchageEntities();
        }

        public TypicalExchangeObjectExporter(ExchangeTaskDTO task, ExecutionContext context, Guid entId)
            : this(context, new GetEnterpriseQuery(context).Execute(entId)) 
        {
            _task = task;
        }

        private List<ExchangeEntityDTO> GetExchageEntities()
        {
            var targetTasks = new GetExchangeTaskListQuery(_context).Execute(new GetExchangeTaskListParameterSet
            {
                ExchangeTypeId = ExchangeType.Export,
                EnterpriseId = Enterprise.Id
            });
            return new GetExchangeEntityListQuery(_context).Execute(new GetExchangeEntityListParameterSet
                                                                    {
                                                                        ExchangeTaskIdList = targetTasks.Select(t => t.Id).ToList(),
                                                                        IsActive = true
                                                                    }).ToList();
        }

        protected override List<ExchangeItem<TDto>> GetExchangeItems<TDto>(QueryReader<Guid, TDto> query, EntityType currentEntityType)
        {
            return _exchangeEntities.Where(e => e.EntityTypeId == currentEntityType).Select(e =>
                                                                                          {
                                                                                              var dto = query.Execute(e.EntityId);
                                                                                              return GetExchangeItem(dto);
                                                                                          }).ToList();
        }

        public ExchangeObject<TypicalExchangeData> Build(SeriesDTO serie)
        {
            if (serie == null)
            {
                return GetErrorData();
            }
            var result = Build<TypicalExchangeData>(serie);
            result.DataSection.SiteDtos = GetTree();
            return result;
        }

        private ExchangeObject<TypicalExchangeData> GetErrorData()
        {
            return new ExchangeObject<TypicalExchangeData>
            {
                HeaderSection = GetExchangeHeader(),
                Status = ExchangeStatus.NoData
            };
        }

        public ExchangeObject<TypicalExchangeData> Build(DateTime dt)
        {
            _startDate = dt;
            _endDate = _startDate.Add((_periodTypeId ?? PeriodType.Twohours).ToTimeSpan());
            var serie = ExchangeHelper.GetSerie(_context, dt: dt, periodTypeId: _periodTypeId);
            return Build(serie);
        }


        protected List<ExchangeSite> GetTree()
        {
            var sites = new GetSiteListQuery(_context).Execute(new GetSiteListParameterSet())
                .Where(s => GetExchangeSiteIds().Contains(s.Id))
                .Select<SiteDTO, ExchangeItem<SiteDTO>>(GetExchangeItem)
                .ToList();

            var _compStations = GetExchangeItems(new GetCompStationByIdQuery(_context), EntityType.CompStation);
            var _distrStations = GetExchangeItems(new GetDistrStationByIdQuery(_context), EntityType.DistrStation);
            var _measStations = GetExchangeItems(new GetMeasStationByIdQuery(_context), EntityType.MeasStation);
            var _reducingStations = GetExchangeItems(new GetReducingStationByIdQuery(_context), EntityType.ReducingStation);


            var result =
                from site in sites
                let siteId = site.Dto.Id
                let compStations = from css in _compStations.Where(cs => cs.Dto.ParentId == siteId)
                    group css by css into gcs
                    let compShops =
                        from csh in
                            new GetCompShopListQuery(_context).Execute(new GetCompShopListParameterSet
                            {
                                StationIdList = new List<Guid> { gcs.Key.Dto.Id }
                            })
                        let compUnits =
                            new GetCompUnitListQuery(_context).Execute(new GetCompUnitListParameterSet {ShopId = csh.Id})
                                .Select<CompUnitDTO, ExchangeItem<CompUnitDTO>>(GetExchangeItem)
                                .ToList()
                        select new ExchangeCompShop(GetExchangeItem(csh)) {CompUnits = compUnits}
                    select new ExchangeCompStation(gcs.First())
                    {
                        CompShops = compShops.ToList()
                    }
                let distrStations =
                    from dss in _distrStations.Where(dss => dss.Dto.ParentId == siteId)
                    group dss by dss
                    into gds
                    let distrStationId = gds.Key.Dto.Id
                    //let consumers =
                    //    new GetConsumerListQuery(_context).Execute(new GetConsumerListParameterSet
                    //    {
                    //        DistrStationId = distrStationId
                    //    }).Select<ConsumerDTO, ExchangeItem<ConsumerDTO>>(GetExchangeItem)
                    let outlets =
                        new GetDistrStationOutletListQuery(_context).Execute(new GetDistrStationOutletListParameterSet
                        {
                            DistrStationId = distrStationId
                        }).Select<DistrStationOutletDTO, ExchangeItem<DistrStationOutletDTO>>(GetExchangeItem)
                    select new ExchangeDistrStation(gds.Key)
                    {
                        //Consumers = consumers.ToList(),
                        DistrStationOutlets = outlets.ToList()
                    }
                let measStations =
                    from mss in _measStations.Where(mss => mss.Dto.ParentId == siteId)
                    group mss by mss
                    into gms
                    let measLines =
                        from mls in
                            new GetMeasLineListQuery(_context).Execute(new GetMeasLineListParameterSet
                            {
                                MeasStationId = gms.Key.Dto.Id
                            })
                        group mls by mls
                        into gml
                        let measPoints =
                            new GetMeasPointListQuery(_context).Execute(new GetMeasPointListParameterSet
                            {
                                ParentId = gml.Key.Id
                            }).Select<MeasPointDTO, ExchangeItem<MeasPointDTO>>(GetExchangeItem)
                        select new ExchangeMeasLine(GetExchangeItem(gml.Key))
                        {
                            MeasPoints = measPoints.ToList()
                        }
                    select new ExchangeMeasStation(gms.Key)
                    {
                        MeasLines = measLines.ToList()
                    }
                select new ExchangeSite(site)
                {
                    CompStationDTos = compStations.ToList(),
                    DistrStationDtos = distrStations.ToList(),
                    MeasStationDtos = measStations.ToList(),
                    ReducingStationDtos = _reducingStations.ToList()
                };

            return result.Where(s => s.CompStationDTos.Any() || s.DistrStationDtos.Any() || s.MeasStationDtos.Any() || s.ReducingStationDtos.Any()).ToList();
        }

        protected IEnumerable<Guid> GetExchangeSiteIds()
        {
            return new GetSiteListQuery(_context)
                .Execute(new GetSiteListParameterSet { EnterpriseId = AppSettingsManager.CurrentEnterpriseId, })
                .Select(s => s.Id).Distinct().ToList();
        }

        protected List<T> GetExchangeResultList<T, TDto>(QueryReader<Guid, TDto> query, EntityType entityType, Func<TDto, T> func)
        {
            return _exchangeEntities.Where(e => e.EntityTypeId == entityType).Select(e =>
                                                                                   {
                                                                                       var dto = query.Execute(e.EntityId);
                                                                                       return func(dto);
                                                                                   }).ToList();
        }
    }
}