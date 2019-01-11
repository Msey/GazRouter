using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.ValueMessages;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
namespace GazRouter.ManualInput.Daily
{
    public class DailyViewModel : LockableViewModel
    {
        private readonly bool _writePermision;

        public DailyViewModel()
        {
            _writePermision = Authorization2.Inst.IsEditable(LinkType.Daily);

            _selectedUnitType = IsolatedStorageManager.Get<int?>("VolumeInputUnits") ?? 1;

            Tabs = new List<TabBaseViewModel>
            {
                new MeasStationsViewModel(Sign.In) { Header = "Поступление" },
                new MeasStationsViewModel(Sign.Out) { Header = "Транзит" },
                new DistrStationsViewModel { Header = "ГРС" },
                new OperConsumersViewModel { Header = "ПЭН" },
                new ReducingStationViewModel { Header = "УРГ" },
                new ConsumersViewModel { Header = "Подключения ГРС" }
            };

            RefreshCommand = new DelegateCommand(LoadData);

            _selectedDate = SeriesHelper.GetPastDispDay();

            LoadSiteList();
        }


        public bool IsReadOnly => !_writePermision || InputState != ManualInputState.Input;


        public List<TabBaseViewModel> Tabs { get; set; }


        public DelegateCommand RefreshCommand { get; set; }
        
        
        public List<SiteDTO> SiteList { get; set; }



        private SiteDTO _selectedSite;
        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                if (SetProperty(ref _selectedSite, value))
                    LoadData();
            }
        }



        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if(SetProperty(ref _selectedDate, value.ToLocal()))
                    LoadData();
            }
        }

        public DateTime MaxDate { get; set; }

        
        /// <summary>
        /// Загрузка списка ЛПУ
        /// </summary>
        private async void LoadSiteList()
        {
            SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                new GetSiteListParameterSet
                {
                    EnterpriseId = UserProfile.Current.Site.IsEnterprise ? UserProfile.Current.Site.Id : (Guid?) null
                });

            if (!UserProfile.Current.Site.IsEnterprise)
            {
                var site = SiteList.Single(s => s.Id == UserProfile.Current.Site.Id);
                SiteList = SiteList.Where(s => s.Id == site.Id || site.DependantSiteIdList.Contains(s.Id)).ToList();
            }
            
            OnPropertyChanged(() => SiteList);
            if (SiteList.Count > 0) SelectedSite = SiteList.First();
        }


        private DailyData _data;

        private async void LoadData()
        {
            MaxDate = SeriesHelper.GetPastDispDay();
            OnPropertyChanged(() => MaxDate);
            
            if (SelectedSite != null)
            {
                Lock();

                _data = await DailyData.Load(_selectedDate, SelectedSite);

                _inputState = _data.InputState.State;
                OnPropertyChanged(() => InputState);

                InputStateInfo = _data.InputState.ChangeDate.HasValue
                    ? $"{_data.InputState.UserName} ({_data.InputState.ChangeDate:dd.MM.yyyy HH:mm})"
                    : string.Empty;
                OnPropertyChanged(() => InputStateInfo);

                OnPropertyChanged(() => IsReadOnly);
                OnPropertyChanged(() => IsInputStateChangeAllowed);
                    

                Tabs.ForEach(t => t.IsReadOnly = IsReadOnly);
                Tabs.ForEach(t => t.Refresh(_data, Coef));

                Unlock();
            }
        }


        private async void CheckData()
        {
            if (_data == null) return;

            var checkList = Tabs.SelectMany(t => t.CheckEntityList).Select(id => new PerformCheckingParameterSet
            {
                EntityId = id,
                SerieId = _data.Serie.Id
            });
            await new SeriesDataServiceProxy().PerformCheckingAsync(checkList.ToList());
        }



        #region INPUT STATE

        /// <summary>
        /// Список возможных статусов (ввод, подтверждено)
        /// </summary>
        public IEnumerable<ManualInputState> InputStateList
        {
            get
            {
                yield return ManualInputState.Input;
                yield return ManualInputState.Approved;
            }
        }

        private ManualInputState _inputState;

        /// <summary>
        /// Текущей статус ввода (ввод, подтверждено)
        /// </summary>
        public ManualInputState InputState
        {
            get { return _inputState; }
            set
            {
                if (SetProperty(ref _inputState, value))
                {
                    SetInputState(value);
                }
            }
        }

        // Информация о том, кто и когда установил текущий статус
        public string InputStateInfo { get; set; }


        /// <summary>
        /// Разрешена ли смена текущего статуса ввода
        /// </summary>
        public bool IsInputStateChangeAllowed
        {
            get
            {
                switch (InputState)
                {
                    case ManualInputState.Input:
                        // Изменить статус можно только если нет ошибок данных по объектам
                        return _writePermision;

                    case ManualInputState.Approved:
                        // Сбросить статус "Подтверждено" может только пользователь ПДС
                        return UserProfile.Current.Site.IsEnterprise && _writePermision;

                    default:
                        return false;
                }
            }
        }

        #endregion
        


        private async void SetInputState(ManualInputState targetState)
        {
            if (SelectedSite == null || _data == null) return;
            await new ManualInputServiceProxy().SetInputStateAsync(
                new SetManualInputStateParameterSet
                {
                    SerieId = _data.Serie.Id,
                    SiteId = SelectedSite.Id,
                    State = targetState
                });

            if (targetState == ManualInputState.Approved)
                CheckData();
            
            LoadData();
        }



        private int _selectedUnitType;
        // Тип единиц измерения для ввода расхода газа
        public int SelectedUnitType
        {
            get { return _selectedUnitType; }
            set
            {
                if (SetProperty(ref _selectedUnitType, value))
                {
                    IsolatedStorageManager.Set("VolumeInputUnits", value);
                    LoadData();
                }
            }
        }

        private int Coef => _selectedUnitType == 0 ? 1 : 1000;

    }

    public class SourceBase
    {
        public virtual string Name { get; set; }
    }

    public class SiteSource : SourceBase
    {
        public SiteSource(SiteDTO dto)
        {
            Dto = dto;
        }

        public SiteDTO Dto { get; set; }

        public override string Name => Dto.Name;
    }


    public class NeighbourSource : SourceBase
    {
        public NeighbourSource(ExchangeTaskDTO dto)
        {
            Dto = dto;
        }

        public ExchangeTaskDTO Dto { get; set; }

        public override string Name => Dto.Name;
    }



}