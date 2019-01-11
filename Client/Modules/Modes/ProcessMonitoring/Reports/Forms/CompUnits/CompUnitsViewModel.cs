using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.Controls.Measurings;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.CompUnitStopTypes;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.ManualInput.CompUnitStates;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.SeriesData.PropertyValues;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using Utils.Extensions;
using GazRouter.DTO.ObjectModel.Sites;

namespace GazRouter.Modes.ProcessMonitoring.Reports.Forms.CompUnits
{

    [RegionMemberLifetime(KeepAlive = false)]
    public class CompUnitsViewModel : FormViewModelBase
    {
        public override bool HasUnitCondition { get; } = true;
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

                    Items = new List<GridItem>();

                    foreach (var site in SiteList)
                    {
                        // Получить список ГПА
                        var stationTree = await new ObjectModelServiceProxy().GetCompStationTreeAsync(site.Id);

                        // Получить текущие состояния ГПА
                        var states = await new ManualInputServiceProxy().GetCompUnitStateListAsync(
                            new GetCompUnitStateListParameterSet
                            {
                                SiteId = site.Id,
                                Timestamp = Timestamp.ToLocal()
                            });


                        // получаем значения измеренных параметров
                        var propValues = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                            new GetEntityPropertyValueListParameterSet
                            {
                                EntityIdList = stationTree.CompUnits.Select(u => u.Id).ToList(),
                                StartDate = Timestamp.ToLocal(),
                                EndDate = Timestamp.ToLocal(),
                                PeriodType = PeriodType.Twohours,
                                LoadMessages = true
                            });


                        // Сформировать дерево

                        foreach (var station in stationTree.CompStations)
                        {
                            var stationItem = new GridItem(station);
                            Items.Add(stationItem);

                            foreach (var shop in stationTree.CompShops.Where(cs => cs.ParentId == station.Id))
                            {
                                var shopItem = new GridItem(shop);
                                stationItem.Children.Add(shopItem);

                                foreach (var unit in stationTree.CompUnits.Where(u => u.ParentId == shop.Id))
                                {
                                    var unitItem = new GridItem(unit, states.SingleOrDefault(s => s.CompUnitId == unit.Id), Timestamp);
                                    if (SelectedState != null) { if (unitItem.State == SelectedState.State) shopItem.Children.Add(unitItem); }
                                    else shopItem.Children.Add(unitItem);

                                    if (unitItem.IsInUse)
                                    {
                                        unitItem.PressureInlet.Extract(propValues, Timestamp);
                                        unitItem.PressureOutlet.Extract(propValues, Timestamp);
                                        unitItem.TemperatureInlet.Extract(propValues, Timestamp);
                                        unitItem.TemperatureOutlet.Extract(propValues, Timestamp);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {

                    // Получить список ГПА
                    var stationTree = await new ObjectModelServiceProxy().GetCompStationTreeAsync(Site.Id);

                    // Получить текущие состояния ГПА
                    var states = await new ManualInputServiceProxy().GetCompUnitStateListAsync(
                        new GetCompUnitStateListParameterSet
                        {
                            SiteId = Site.Id,
                            Timestamp = Timestamp.ToLocal()
                        });


                    // получаем значения измеренных параметров
                    var propValues = await new SeriesDataServiceProxy().GetEntityPropertyValueListAsync(
                        new GetEntityPropertyValueListParameterSet
                        {
                            EntityIdList = stationTree.CompUnits.Select(u => u.Id).ToList(),
                            StartDate = Timestamp.ToLocal(),
                            EndDate = Timestamp.ToLocal(),
                            PeriodType = PeriodType.Twohours,
                            LoadMessages = true
                        });


                    // Сформировать дерево
                    Items = new List<GridItem>();

                    foreach (var station in stationTree.CompStations)
                    {
                        var stationItem = new GridItem(station);
                        Items.Add(stationItem);

                        foreach (var shop in stationTree.CompShops.Where(cs => cs.ParentId == station.Id))
                        {
                            var shopItem = new GridItem(shop);
                            stationItem.Children.Add(shopItem);

                            foreach (var unit in stationTree.CompUnits.Where(u => u.ParentId == shop.Id))
                            {
                                var unitItem = new GridItem(unit, states.SingleOrDefault(s => s.CompUnitId == unit.Id), Timestamp);
                                if (SelectedState != null) { if (unitItem.State == SelectedState.State) shopItem.Children.Add(unitItem); }
                                else shopItem.Children.Add(unitItem);

                                if (unitItem.IsInUse)
                                {
                                    unitItem.PressureInlet.Extract(propValues, Timestamp);
                                    unitItem.PressureOutlet.Extract(propValues, Timestamp);
                                    unitItem.TemperatureInlet.Extract(propValues, Timestamp);
                                    unitItem.TemperatureOutlet.Extract(propValues, Timestamp);
                                }

                            }
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
        

        public List<GridItem> Items { get; set; }
        
    }

    public class GridItem
    {
        private readonly CommonEntityDTO _entityDto;
        private readonly CompUnitStateDTO _stateDto;
        private readonly DateTime? _period;


        public GridItem(CommonEntityDTO entityDto, CompUnitStateDTO stateDto = null, DateTime? period = null)
        {
            _entityDto = entityDto;
            _stateDto = stateDto;
            _period = period;
            

            Children = new List<GridItem>();

            PressureInlet = new DoubleMeasuring(_entityDto.Id, PropertyType.PressureSuperchargerInlet, PeriodType.Twohours, true);
            PressureOutlet = new DoubleMeasuring(_entityDto.Id, PropertyType.PressureSuperchargerOutlet, PeriodType.Twohours, true);
            TemperatureInlet = new DoubleMeasuring(_entityDto.Id, PropertyType.TemperatureSuperchargerInlet, PeriodType.Twohours, true);
            TemperatureOutlet = new DoubleMeasuring(_entityDto.Id, PropertyType.TemperatureSuperchargerOutlet, PeriodType.Twohours, true);
        }

        public DateTime? Period
        {
            get { return _period; }
        }

        public CommonEntityDTO EntityDto
        {
            get { return _entityDto; }
        }

        public CompUnitStateDTO StateDto
        {
            get { return _stateDto; }
        }

        public Guid Id
        {
            get { return _entityDto.Id; }
        }

        public string Name
        {
            get { return _entityDto.Name; }
        }
        
        [Display(AutoGenerateField = false)]
        public List<GridItem> Children { get; set; }

        public CompUnitState? State
        {
            get { return _stateDto?.State; }
        }

        public DateTime? StateChangeDate
        {
            get { return _stateDto != null ? (DateTime?) _stateDto.StateChangeDate : null; }
        }


        public TimeSpan? InStateDuration
        {
            get { return _stateDto != null ? (TimeSpan?)_stateDto.InStateDuration : null; }
        }


        public bool IsInUse
        {
            get { return _stateDto != null && _stateDto.State == CompUnitState.Work; }
        }

        public bool IsFailure
        {
            get { return _stateDto != null && _stateDto.StopType.HasValue && _stateDto.StopType.Value != CompUnitStopType.Planned; }
        }

        public bool IsRepair
        {
            get { return _stateDto != null && _stateDto.State == CompUnitState.Repair; }
        }

        public bool IsReserve
        {
            get { return _stateDto != null && _stateDto.State == CompUnitState.Reserve; }
        }

        /// <summary>
        /// Тип нагнетателя
        /// </summary>
        public string SuperchargerTypeName
        {
            get
            {
                if (_entityDto.EntityType != EntityType.CompUnit) return "";
                var unit = (CompUnitDTO) _entityDto;
                return
                    ClientCache.DictionaryRepository.SuperchargerTypes.Single(st => st.Id == unit.SuperchargerTypeId)
                        .Name;
            }
        }

        /// <summary>
        /// Тип ГПА
        /// </summary>
        public string CompUnitTypeName
        {
            get
            {
                if (_entityDto.EntityType != EntityType.CompUnit) return "";
                var unit = (CompUnitDTO)_entityDto;
                return ClientCache.DictionaryRepository.CompUnitTypes.Single(t => t.Id == unit.CompUnitTypeId).Name;
            }
        }

        public string CompUnitTypeStyle
        {
            get
            {
                return (_entityDto.EntityType == EntityType.CompUnit) ? "Normal" : "Bold";
            }
        }

        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();
        // Измеренные значения
        public DoubleMeasuring PressureInlet { get; set; }
        public DoubleMeasuring PressureOutlet { get; set; }
        public DoubleMeasuring TemperatureInlet { get; set; }
        public DoubleMeasuring TemperatureOutlet { get; set; }
        
    }
}