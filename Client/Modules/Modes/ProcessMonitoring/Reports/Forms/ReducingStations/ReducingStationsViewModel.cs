using GazRouter.Application;
using GazRouter.Controls.Measurings;
using GazRouter.DataProviders.Balances;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Balances.Contracts;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.PropertyValues;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.Extensions;

namespace GazRouter.Modes.ProcessMonitoring.Reports.Forms.ReducingStations
{
    [RegionMemberLifetime(KeepAlive = false)]

    public class ReducingStationsViewModel : FormViewModelBase
    {
        public override ReportSettings GetReportSettings()
        {
            return new ReportSettings
            {
                SiteSelector = true,
                EmptySiteAllowed = true,
                SerieSelector = true,
                DetailView =false
            };
        }

        private ItemBase _selectedItem;

        public bool IsSelected => _selectedItem != null;

        public ItemBase SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    OnPropertyChanged(() => IsSelected);

                    RefreshDetails();

                }
            }
        }

        public List<ItemBase> Items { get; set; }

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
                        // Получить дерево УРЦ
                        var stations = await new ObjectModelServiceProxy().GetReducingStationListAsync(
                                    new GetReducingStationListParameterSet
                                    {
                                        SiteId = site.Id
                                    });
                        
                        var entityList = stations.Select(s => s.Id).ToList();


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

                        foreach (var station in stations)
                        {
                            var stationItem = new StationItem(station);
                            Items.Add(stationItem);

                            stationItem.PressureInlet.Extract(propValues, Timestamp);
                            stationItem.PressureOutlet.Extract(propValues, Timestamp);
                            stationItem.TemperatureInlet.Extract(propValues, Timestamp);
                            stationItem.TemperatureOutlet.Extract(propValues, Timestamp);
                            stationItem.OpeningPercentage.Extract(propValues, Timestamp);
                            stationItem.CompressionRatio.Extract(propValues, Timestamp);
                            stationItem.Flow.Extract(propValues, Timestamp);
                            stationItem.Plan = ExtractPlan(planValues, station.Id);

                        }
                    }

                }
                else
                {
                    // Получить дерево ГИС
                    var stations = await new ObjectModelServiceProxy().GetReducingStationListAsync(
                                    new GetReducingStationListParameterSet
                                    {
                                        SiteId = Site.Id
                                    });
                    
                    var entityList = stations.Select(s => s.Id).ToList();


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

                    foreach (var station in stations)
                    {
                        var stationItem = new StationItem(station);
                        Items.Add(stationItem);

                        stationItem.PressureInlet.Extract(propValues, Timestamp);
                        stationItem.PressureOutlet.Extract(propValues, Timestamp);
                        stationItem.TemperatureInlet.Extract(propValues, Timestamp);
                        stationItem.TemperatureOutlet.Extract(propValues, Timestamp);
                        stationItem.OpeningPercentage.Extract(propValues, Timestamp);
                        stationItem.CompressionRatio.Extract(propValues, Timestamp);
                        stationItem.Flow.Extract(propValues, Timestamp);
                        stationItem.Plan = ExtractPlan(planValues, station.Id);
                    }
                }
                OnPropertyChanged(() => Items);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        public List<PropertyType> PropTypeList { get; private set; }

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
            return  volDay / 24;
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

        public Guid Id { get; set; }

        public double? Plan { get; set; }


    }
    public class StationItem : ItemBase
    {
        public StationItem(ReducingStationDTO dto)
        {
            Dto = dto;
            Id = dto.Id;
            //Plan = dto.
            Flow = new DoubleMeasuring(Dto.Id, PropertyType.Flow, PeriodType.Twohours, true);
            PressureInlet = new DoubleMeasuring(Dto.Id, PropertyType.PressureInlet, PeriodType.Twohours, true);
            PressureOutlet = new DoubleMeasuring(Dto.Id, PropertyType.PressureOutlet, PeriodType.Twohours, true);
            TemperatureInlet = new DoubleMeasuring(Dto.Id, PropertyType.TemperatureInlet, PeriodType.Twohours, true);
            TemperatureOutlet = new DoubleMeasuring(Dto.Id, PropertyType.TemperatureOutlet, PeriodType.Twohours, true);
            CompressionRatio = new DoubleMeasuring(Dto.Id, PropertyType.CompressionRatio, PeriodType.Twohours, true);
            OpeningPercentage = new DoubleMeasuring(Dto.Id, PropertyType.OpeningPercentage, PeriodType.Twohours, true);
        }

        public ReducingStationDTO Dto { get; }


        public EntityType EntityType => Dto.EntityType;

        public DoubleMeasuring PressureInlet { get; set; }
        public DoubleMeasuring PressureOutlet { get; set; }
        public DoubleMeasuring TemperatureInlet { get; set; }
        public DoubleMeasuring TemperatureOutlet { get; set; }
        public DoubleMeasuring CompressionRatio { get; set; }
        public DoubleMeasuring OpeningPercentage { get; set; }
        public DoubleMeasuring Flow { get; set; }
        public double? Delta => Flow.Value.HasValue && Plan.HasValue ? Flow.Value - Plan : null;
    }

}

