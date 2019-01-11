using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Controls.Measurings;
using GazRouter.DataProviders.Balances;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.SeriesData.PropertyValues;
using Microsoft.Practices.Prism.Regions;
using Utils.Extensions;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.Application;

namespace GazRouter.Modes.ProcessMonitoring.Reports.Forms.DistrStations
{

    [RegionMemberLifetime(KeepAlive = false)]
    public class DistrStationsViewModel : FormViewModelBase
    {
        public override ReportSettings GetReportSettings()
        {
            return new ReportSettings
            {
                SiteSelector = true,
                EmptySiteAllowed = true,
                SerieSelector = true
            };
        }

        public List<SiteDTO> SiteList { get; set; }

        public override async void Refresh()
        {
            try
            {
                Behavior.TryLock();

                if (Site == null)
                {
                    if (UserProfile.Current.Site.IsEnterprise)
                    {
                        SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                        new GetSiteListParameterSet
                        {
                            EnterpriseId = UserProfile.Current.Site.Id
                        });
                    }
                    else
                    {
                        SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                        new GetSiteListParameterSet
                        {
                            SiteId = UserProfile.Current.Site.Id
                        });
                    }

                    Items = new List<ItemBase>();

                    foreach (var site in SiteList)
                    {

                        // Получить дерево ГРС
                        var stationTree = await new ObjectModelServiceProxy().GetDistrStationTreeAsync(
                            new GetDistrStationListParameterSet
                            {
                                SiteId = site.Id
                            });

                        var entityList = new List<Guid>();
                        entityList.AddRange(stationTree.DistrStations.Select(ds => ds.Id));
                        entityList.AddRange(stationTree.DistrStationOutlets.Select(dso => dso.Id));


                        // получаем значения измеренных параметров
                        var propValues = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                            new GetEntityPropertyValueListParameterSet
                            {
                                EntityIdList = entityList,
                                StartDate = Timestamp.AddHours(-2),
                                EndDate = Timestamp.ToLocal(),
                                PeriodType = PeriodType.Twohours,
                                LoadMessages = true
                            });

                        // получить плановые данные
                        var contract = await new BalancesServiceProxy().GetContractAsync(
                            new GetContractListParameterSet
                            {
                                ContractDate = Timestamp.Date.MonthStart(),
                                SystemId = site.SystemId,
                                PeriodTypeId = PeriodType.Month,
                                TargetId = Target.Plan,
                                IsFinal = true
                            });

                        var planValues = await new BalancesServiceProxy().GetBalanceValuesAsync(contract.Id);


                        // Сформировать дерево

                        foreach (var station in stationTree.DistrStations)
                        {
                            var stationItem = new StationItem(station);
                            Items.Add(stationItem);

                            stationItem.PressureInlet.Extract(propValues, Timestamp);
                            stationItem.TemperatureInlet.Extract(propValues, Timestamp);
                            stationItem.Flow.Extract(propValues, Timestamp);
                            stationItem.Plan = ExtractPlan(planValues, station.Id);

                            foreach (var output in stationTree.DistrStationOutlets.Where(o => o.ParentId == station.Id))
                            {
                                var outputItem = new OutputItem(output);
                                stationItem.Childs.Add(outputItem);

                                outputItem.PressureOutlet.Extract(propValues, Timestamp);
                                outputItem.TemperatureOutlet.Extract(propValues, Timestamp);
                                outputItem.Flow.Extract(propValues, Timestamp);
                            }
                        }

                    }

                }
                else
                {
                    // Получить дерево ГРС
                    var stationTree = await new ObjectModelServiceProxy().GetDistrStationTreeAsync(
                        new GetDistrStationListParameterSet
                        {
                            SiteId = Site.Id
                        });

                    var entityList = new List<Guid>();
                    entityList.AddRange(stationTree.DistrStations.Select(ds => ds.Id));
                    entityList.AddRange(stationTree.DistrStationOutlets.Select(dso => dso.Id));


                    // получаем значения измеренных параметров
                    var propValues = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                        new GetEntityPropertyValueListParameterSet
                        {
                            EntityIdList = entityList,
                            StartDate = Timestamp.ToLocal(),
                            EndDate = Timestamp.ToLocal(),
                            PeriodType = PeriodType.Twohours,
                            LoadMessages = true
                        });

                    // получить плановые данные
                    var contract = await new BalancesServiceProxy().GetContractAsync(
                        new GetContractListParameterSet
                        {
                            ContractDate = Timestamp.Date.MonthStart(),
                            SystemId = Site.SystemId,
                            PeriodTypeId = PeriodType.Month,
                            TargetId = Target.Plan,
                            IsFinal = true
                        });

                    var planValues = await new BalancesServiceProxy().GetBalanceValuesAsync(contract.Id);


                    // Сформировать дерево
                    Items = new List<ItemBase>();

                    foreach (var station in stationTree.DistrStations)
                    {
                        var stationItem = new StationItem(station);
                        Items.Add(stationItem);

                        stationItem.PressureInlet.Extract(propValues, Timestamp);
                        stationItem.TemperatureInlet.Extract(propValues, Timestamp);
                        stationItem.Flow.Extract(propValues, Timestamp);
                        stationItem.Plan = ExtractPlan(planValues, station.Id);

                        foreach (var output in stationTree.DistrStationOutlets.Where(o => o.ParentId == station.Id))
                        {
                            var outputItem = new OutputItem(output);
                            stationItem.Childs.Add(outputItem);

                            outputItem.PressureOutlet.Extract(propValues, Timestamp);
                            outputItem.TemperatureOutlet.Extract(propValues, Timestamp);
                            outputItem.Flow.Extract(propValues, Timestamp);
                        }
                    }
                }

                OnPropertyChanged(() => Items);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }


        public List<ItemBase> Items { get; set; }


        private double? ExtractPlan(List<BalanceValueDTO> values, Guid stationId)
        {
            var stationValues = values.Where(v => v.DistrStationId == stationId).ToList();
            var volDay = 0.0;
            foreach (var value in stationValues)
            {
                var irrVal =
                    value.IrregularityList.FirstOrDefault(v => v.StartDayNum <= Timestamp.Day && Timestamp.Day <= v.EndDayNum)?
                        .Value;
                if (irrVal.HasValue) volDay += irrVal.Value;
                else volDay += value.BaseValue / Timestamp.DaysInMonth();
            }
            return volDay / 24;
        }
    }


    public class ItemBase
    {
        public ItemBase()
        {
            Childs = new List<ItemBase>();
        }

        public List<ItemBase> Childs { get; set; }

        public string Name { get; set; }
    }


    public class StationItem : ItemBase
    {
        public StationItem(DistrStationDTO dto)
        {
            Dto = dto;

            PressureInlet = new DoubleMeasuring(Dto.Id, PropertyType.PressureInlet, PeriodType.Twohours, true);
            TemperatureInlet = new DoubleMeasuring(Dto.Id, PropertyType.TemperatureInlet, PeriodType.Twohours, true);
            Flow = new DoubleMeasuring(Dto.Id, PropertyType.Flow, PeriodType.Twohours, true);
        }

        public DistrStationDTO Dto { get; }

        public DoubleMeasuring PressureInlet { get; set; }
        public DoubleMeasuring TemperatureInlet { get; set; }
        public DoubleMeasuring Flow { get; set; }


        public double? Plan { get; set; }

        public double? PlanDelta => Plan.HasValue && Flow.Value.HasValue ? Flow.Value - Plan : null;


        /// <summary>
        /// Загрузка от проектной производительности ГРС, %
        /// </summary>
        public double UtilizationProject => Dto.CapacityRated != 0 ? Math.Round(Flow.Value / Dto.CapacityRated * 100 ?? 0) : 0;
        
        /// <summary>
        /// Технически возможная производительность ГРС, тыс.м3
        /// </summary>
        public double FlowPossible { get; set; }

        /// <summary>
        /// Загрузка от технически возможной величины, %
        /// </summary>
        public double UtilizationPossible => FlowPossible != 0 ? Math.Round(Flow.Value/FlowPossible*100 ?? 0) : 0;
    }


    public class OutputItem : ItemBase
    {
        public OutputItem(DistrStationOutletDTO dto)
        {
            Dto = dto;

            PressureOutlet = new DoubleMeasuring(Dto.Id, PropertyType.PressureOutlet, PeriodType.Twohours, true);
            TemperatureOutlet = new DoubleMeasuring(Dto.Id, PropertyType.TemperatureOutlet, PeriodType.Twohours, true);
            Flow = new DoubleMeasuring(Dto.Id, PropertyType.Flow, PeriodType.Twohours, true);
        }

        public DistrStationOutletDTO Dto { get; }

        public DoubleMeasuring PressureOutlet { get; set; }
        public DoubleMeasuring TemperatureOutlet { get; set; }
        public DoubleMeasuring Flow { get; set; }


        /// <summary>
        /// Загрузка от проектной производительности выхода, %
        /// </summary>
        public double UtilizationProject => Dto.CapacityRated != 0 ? Math.Round(Flow.Value / Dto.CapacityRated * 100 ?? 0) : 0;

        /// <summary>
        /// Технически возможная производительность выхода, тыс.м3
        /// </summary>
        public double FlowPossible { get; set; }

        /// <summary>
        /// Загрузка от технически возможной величины, %
        /// </summary>
        public double UtilizationPossible => FlowPossible != 0 ? Math.Round(Flow.Value / FlowPossible * 100 ?? 0) : 0;
    }
    
}