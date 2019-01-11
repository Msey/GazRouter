using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Browser;
using GazRouter.Application;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataExchange.CustomSource.Dialogs;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.TransportTypes;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.Sites;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using UriBuilder = GazRouter.DataProviders.UriBuilder;

namespace DataExchange.Typical
{
    public class TypicalExchangeViewModel : LockableViewModel
    {
        public DelegateCommand RefreshCommand { get; private set; }
        public DelegateCommand RunCommand { get; set; }
        public DelegateCommand EditCommand{ get; set; }
        public DelegateCommand DownloadCommand { get; set; }
        private DateTime? _selectedTimeStamp;

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged(() => IsEnabled);
            }
        }

        public TypicalExchangeViewModel()
        {
            IsEnabled       = Authorization2.Inst.IsEditable(LinkType.TypicalExch);
            DownloadCommand = new DelegateCommand(DownloadFile, () => SelectedEnterprise != null && SelectedTimeStamp.HasValue);
            RunCommand      = new DelegateCommand(Run, () => SelectedTimeStamp.HasValue);
            RefreshCommand  = new DelegateCommand(Refresh);
            EditCommand     = new DelegateCommand(Edit, () => IsEnabled);
            Init();
        }

        private async void Edit()
        {

            var tasks = await new DataExchangeServiceProxy().GetExchangeTaskListAsync(
                new GetExchangeTaskListParameterSet
                {
                    ExchangeTypeId = ExchangeType.Export,
                });
            var task = tasks.FirstOrDefault(t => t.EnterpriseId.HasValue);
            var vm = new AddEditExchangeTaskViewModel(id => {}, task) {SecondTabVisible = false, FirstTabVisible = false};
            var v = new AddEditExchangeTaskView() { DataContext = vm };
            v.ShowDialog();
        }

        public TransportType? TransportTypeId
        {
            get { return _transportTypeId; }
            set { _transportTypeId = value; }
        }

        public string TransportAddress
        {
            get { return _transportAddress; }
            set { _transportAddress = value; }
        }


        public async void Init()
        {
            // получить список ЛПУ
            if (UserProfile.Current.Site.IsEnterprise)
            {
                SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                    new GetSiteListParameterSet
                    {
                        EnterpriseId = UserProfile.Current.Site.Id
                    });
            }
            OnPropertyChanged(() => SiteList);

            _selectedSite = SiteList.First();
            OnPropertyChanged(() => SelectedSite);

            _selectedType = EntityType.CompStation;
            OnPropertyChanged(() => SelectedType);

            ExchangeEnterprises = await new DataExchangeServiceProxy().GetTypicalExchangeEnterpriseListAsync();
            OnPropertyChanged(() => ExchangeEnterprises);

            _selectedEnterprise = ExchangeEnterprises.First();
            OnPropertyChanged(() => SelectedEnterprise);


            Refresh();
        }


        /// <summary>
        /// Список ЛПУ
        /// </summary>
        public List<SiteDTO> SiteList { get; set; }


        private SiteDTO _selectedSite;

        /// <summary>
        /// Выбранное ЛПУ
        /// </summary>
        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                if (SetProperty(ref _selectedSite, value))
                {
                    Refresh();
                }
            }
        }

        public List<NeighbourEnterpriseExchangeTask> ExchangeEnterprises { get; set; }


        private NeighbourEnterpriseExchangeTask _selectedEnterprise;

        public NeighbourEnterpriseExchangeTask SelectedEnterprise
        {
            get { return _selectedEnterprise; }
            set
            {
                if (SetProperty(ref _selectedEnterprise, value))
                {
                    Refresh();
                }
            }
        }

        public IEnumerable<EntityType> TypeList
        {
            get
            {
                yield return EntityType.CompStation;
                yield return EntityType.DistrStation;
                yield return EntityType.MeasStation;
                yield return EntityType.ReducingStation;
            }
        }

        private EntityType _selectedType;

        public EntityType SelectedType
        {
            get { return _selectedType; }
            set
            {
                if (SetProperty(ref _selectedType, value))
                    Refresh();
            }
        }

        public DateTime? SelectedTimeStamp
        {
            get { return _selectedTimeStamp; }
            set
            {
                _selectedTimeStamp = value;
                OnPropertyChanged(() => SelectedTimeStamp);
                DownloadCommand.RaiseCanExecuteChanged();
                RunCommand.RaiseCanExecuteChanged();
            }
        }

        private List<ExchangeEntityDTO> _bindings;

        private async void Refresh()
        {
            if (SelectedSite == null || SelectedEnterprise == null) return;

            try
            {
                Behavior.TryLock();

                Items = new List<BindableItem>();
                if (SelectedEnterprise.ExchangeTaskId != null)
                {
                    _bindings = await new DataExchangeServiceProxy().GetExchangeEntityListAsync(
                        new GetExchangeEntityListParameterSet
                        {
                            ExchangeTaskIdList = {SelectedEnterprise.ExchangeTaskId.Value}
                        });

                    switch (_selectedType)
                    {
                        case EntityType.CompStation:
                            var stationTree =
                                await new ObjectModelServiceProxy().GetCompStationTreeAsync(SelectedSite.Id);

                            foreach (var station in stationTree.CompStations)
                            {
                                var stationBinding = _bindings.SingleOrDefault(e => e.EntityId == station.Id) ??
                                                     new ExchangeEntityDTO
                                                     {
                                                         EntityId = station.Id,
                                                         EntityName = station.Name,
                                                         EntityTypeId = EntityType.CompStation
                                                     };
                                var stationItem = new BindableItem(stationBinding, UpdateEntityBinding) {IsEnabled = IsEnabled};
                                Items.Add(stationItem);
                            }
                            break;


                        case EntityType.DistrStation:
                            var distrTree = await new ObjectModelServiceProxy().GetDistrStationTreeAsync(
                                new GetDistrStationListParameterSet
                                {
                                    SiteId = SelectedSite.Id
                                });

                            Items = new List<BindableItem>();

                            foreach (var ds in distrTree.DistrStations)
                            {
                                var dsBinding = _bindings.SingleOrDefault(e => e.EntityId == ds.Id) ??
                                                new ExchangeEntityDTO
                                                {
                                                    EntityId = ds.Id,
                                                    EntityName = ds.Name,
                                                    EntityTypeId = EntityType.DistrStation
                                                };

                                var dsItem = new BindableItem(dsBinding, UpdateEntityBinding) { IsEnabled = IsEnabled };
                                Items.Add(dsItem);
                            }
                            break;


                        case EntityType.MeasStation:
                            var measTree = await new ObjectModelServiceProxy().GetMeasStationTreeAsync(
                                new GetMeasStationListParameterSet { SiteId = SelectedSite.Id });

                            Items = new List<BindableItem>();

                            foreach (var ms in measTree.MeasStations)
                            {
                                var dsBinding = _bindings.SingleOrDefault(e => e.EntityId == ms.Id) ??
                                                new ExchangeEntityDTO
                                                {
                                                    EntityId = ms.Id,
                                                    EntityName = ms.Name,
                                                    EntityTypeId = EntityType.MeasStation
                                                };

                                var msItem = new BindableItem(dsBinding, UpdateEntityBinding) { IsEnabled = IsEnabled };
                                Items.Add(msItem);
                            }
                            break;


                        case EntityType.ReducingStation:
                            var reduceStationList = await new ObjectModelServiceProxy().GetReducingStationListAsync(
                                new GazRouter.DTO.ObjectModel.ReducingStations.GetReducingStationListParameterSet
                                {
                                    SiteId = SelectedSite.Id
                                }
                                );

                            Items = new List<BindableItem>();

                            foreach (var rs in reduceStationList)
                            {
                                var dsBinding = _bindings.SingleOrDefault(e => e.EntityId == rs.Id) ??
                                                new ExchangeEntityDTO
                                                {
                                                    EntityId = rs.Id,
                                                    EntityName = rs.Name,
                                                    EntityTypeId = EntityType.ReducingStation
                                                };  

                                var rsItem = new BindableItem(dsBinding, UpdateEntityBinding) { IsEnabled = IsEnabled };
                                Items.Add(rsItem);
                            }
                            break;
                    }
                }
                else
                {
                    MessageBoxProvider.Alert(
                        $"Отсутствует запись в БД о настройках задания для обмена с {SelectedEnterprise.Name}. Обратитесь к службе сопровождения.",
                        "Задание обмена");
                }
                OnPropertyChanged(() => Items);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        public void DownloadFile()
        {
            var periodTypeId = IsTwoHoursSelected ? PeriodType.Twohours : PeriodType.Day;
            HtmlPage.Window.Navigate(UriBuilder.GetTypicalExchangeHandlerUri(SelectedEnterprise.Id,
                SelectedTimeStamp.Value, periodTypeId, IsCryptable));
        }


        private async void Run()
        {
            var periodTypeId = IsTwoHoursSelected ? PeriodType.Twohours : PeriodType.Day;
            await new DataExchangeServiceProxy().RunExchangeTaskAsync(new RunExchangeTaskParameterSet { TimeStamp = ((DateTime)SelectedTimeStamp).ToLocal(), PeriodTypeId = periodTypeId});

        }

        public async void UpdateEntityBinding(ExchangeEntityDTO dto)
        {
            await new DataExchangeServiceProxy().SetExchangeEntityAsync(
                new AddEditExchangeEntityParameterSet
                {
                    EntityId = dto.EntityId,
                    ExchangeTaskId = SelectedEnterprise.ExchangeTaskId.Value,
                    IsActive = dto.IsActive,
                    ExtId = "x"
                });
        }


        public List<BindableItem> Items { get; set; }

        private bool _isCryptable;
        private string _transportAddress;
        private TransportType? _transportTypeId;
        private bool _isTwoHoursSelected = true;

        public bool IsCryptable
        {
            get { return _isCryptable; }
            set
            {
                _isCryptable = value;
                OnPropertyChanged(() => IsCryptable);
            }
        }

        public bool IsTwoHoursSelected 
        {
            get { return _isTwoHoursSelected; }
            set
            {
                _isTwoHoursSelected = value;
                OnPropertyChanged(() => IsTwoHoursSelected);
            }
        }
    }
    
    public class BindableItem : PropertyChangedBase
    {
        private ExchangeEntityDTO _dto;
        private Action<ExchangeEntityDTO> _saveAction;

        public BindableItem(ExchangeEntityDTO dto, Action<ExchangeEntityDTO> saveAction)
        {
            _dto = dto;
            _saveAction = saveAction;
        }

        public string Name => _dto.EntityName; 
        
        public bool IsActive
        {
            get { return _dto.IsActive; }
            set
            {
                if (_dto.IsActive != value)
                {
                    _dto.IsActive = value;
                    OnPropertyChanged(() => IsActive);
                    _saveAction(_dto);
                }
            }
        }

        public EntityType EntityType => _dto.EntityTypeId;

        public bool IsEnabled { get; set; }
    }
}