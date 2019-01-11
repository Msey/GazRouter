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
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.SeriesData.PropertyValues;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;
using Utils.Extensions;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.Application;

namespace GazRouter.Modes.ProcessMonitoring.Reports.Forms.MeasStations
{

    [RegionMemberLifetime(KeepAlive = false)]
    public class MeasStationsViewModel : FormViewModelBase
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
                        // Получить дерево ГИС
                        var stationTree = await new ObjectModelServiceProxy().GetMeasStationTreeAsync(
                            new GetMeasStationListParameterSet { SiteId = site.Id });

                        var entityList = new List<Guid>();
                        entityList.AddRange(stationTree.MeasStations.Select(ds => ds.Id));
                        entityList.AddRange(stationTree.MeasLines.Select(ds => ds.Id));


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

                        foreach (var station in stationTree.MeasStations)
                        {
                            var stationItem = new StationItem(station);
                            Items.Add(stationItem);
                            stationItem.Flow.Extract(propValues, Timestamp);
                            stationItem.Plan = ExtractPlan(planValues, station.Id);

                            foreach (var line in stationTree.MeasLines.Where(l => l.ParentId == station.Id))
                            {
                                var lineItem = new LineItem(line);
                                stationItem.Childs.Add(lineItem);

                                lineItem.Pressure.Extract(propValues, Timestamp);
                                lineItem.Temperature.Extract(propValues, Timestamp);
                                lineItem.Flow.Extract(propValues, Timestamp);
                            }
                        }
                    }

                }
                else
                {
                    // Получить дерево ГИС
                    var stationTree = await new ObjectModelServiceProxy().GetMeasStationTreeAsync(
                        new GetMeasStationListParameterSet { SiteId = Site.Id });

                    var entityList = new List<Guid>();
                    entityList.AddRange(stationTree.MeasStations.Select(ds => ds.Id));
                    entityList.AddRange(stationTree.MeasLines.Select(ds => ds.Id));


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

                    foreach (var station in stationTree.MeasStations)
                    {
                        var stationItem = new StationItem(station);
                        Items.Add(stationItem);
                        stationItem.Flow.Extract(propValues, Timestamp);
                        stationItem.Plan = ExtractPlan(planValues, station.Id);

                        foreach (var line in stationTree.MeasLines.Where(l => l.ParentId == station.Id))
                        {
                            var lineItem = new LineItem(line);
                            stationItem.Childs.Add(lineItem);

                            lineItem.Pressure.Extract(propValues, Timestamp);
                            lineItem.Temperature.Extract(propValues, Timestamp);
                            lineItem.Flow.Extract(propValues, Timestamp);
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
            var stationValues = values.Where(v => v.EntityId == stationId).ToList();
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
        public StationItem(MeasStationDTO dto)
        {
            Dto = dto;
            Flow = new DoubleMeasuring(Dto.Id, PropertyType.Flow, PeriodType.Twohours, true);
        }

        public MeasStationDTO Dto { get; }
        
        public DoubleMeasuring Flow { get; set; }

        public double? Plan { get; set; }

        public double? Delta => Flow.Value.HasValue && Plan.HasValue ? Flow.Value - Plan : null;
    }




    public class LineItem : ItemBase
    {
        public MeasLineDTO Dto { get; }

        public LineItem(MeasLineDTO dto)
        {
            Dto = dto;

            Pressure = new DoubleMeasuring(Dto.Id, PropertyType.PressureInlet, PeriodType.Twohours, true);
            Temperature = new DoubleMeasuring(Dto.Id, PropertyType.TemperatureInlet, PeriodType.Twohours, true);
            Flow = new DoubleMeasuring(Dto.Id, PropertyType.Flow, PeriodType.Twohours, true);
        }

        
        public DoubleMeasuring Pressure { get; set; }
        public DoubleMeasuring Temperature { get; set; }
        public DoubleMeasuring Flow { get; set; }
        
    }
    
}