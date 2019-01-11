using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.Controls;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.DTO.ManualInput.ValveSwitches;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.Series;
using Microsoft.Practices.Prism.Regions;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.ManualInput.Valves
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class ValveViewModel : LockableViewModel
    {
        private ValveSwitchDTO _selectedSwitch;
        private DateTime _selectedDate;

        public ValveViewModel()
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.Valve);

            RefreshCommand = new DelegateCommand(Refresh);
            AddCommand = new DelegateCommand(AddValveSwitch, () => editPermission && InputState == ManualInputState.Input);
            DeleteCommand = new DelegateCommand(DeleteValveSwitch,
                () => editPermission && InputState == ManualInputState.Input && SelectedSwitch != null);
            
            
            Init();
        }
        
        
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand RefreshCommand { get; set; }


        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if(SetProperty(ref _selectedDate, value))
                    Refresh();
            }
        }


        public List<SiteDTO> SiteList { get; set; }


        private SiteDTO _selectedSite;
        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                if (SetProperty(ref _selectedSite, value))
                {
                    IsolatedStorageManager.Set("LastSelectedSiteId", value.Id);
                    Refresh();
                }
            }
        }


        private async void Init()
        {
            _selectedDate = SeriesHelper.GetCurrentSession();
            OnPropertyChanged(() => SelectedDate);

            // получить список ЛПУ
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
            OnPropertyChanged(() => SiteList);

            _selectedSite = SiteList.First();
            IsolatedStorageManager.Set("LastSelectedSiteId", _selectedSite.Id);
            OnPropertyChanged(() => SelectedSite);

            Refresh();
        }


        public ManualInputState InputState { get; set; }

        // Информация о том, кто и когда установил текущий статус
        public string InputStateInfo { get; set; }


        /// <summary>
        /// Список переключений
        /// </summary>
        public List<ValveSwitchDTO> ValveSwitchList { get; set; }

        /// <summary>
        /// Выбранное переключение
        /// </summary>
        public ValveSwitchDTO SelectedSwitch
        {
            get { return _selectedSwitch; }
            set
            {
                if(SetProperty(ref _selectedSwitch, value))
                    DeleteCommand.RaiseCanExecuteChanged();
            }
        }


        public async void Refresh()
        {
            Lock();
            ValveSwitchList = await new ManualInputServiceProxy().GetValveSwitchListAsync(
                new GetValveSwitchListParameterSet
                {
                    BeginDate = SelectedDate.AddHours(-2).AddSeconds(1).ToLocal(),
                    EndDate = SelectedDate.ToLocal(),
                    SiteId = SelectedSite?.Id
                });

            OnPropertyChanged(() => ValveSwitchList);
            Unlock();

            GetInputState();
        }



        public async void DeleteValveSwitch()
        {
            Lock();

            await new ManualInputServiceProxy().DeleteValveSwitchAsync(
                new DeleteValveSwitchParameterSet
                {
                    ValveId = SelectedSwitch.Id,
                    ValveSwitchType = SelectedSwitch.SwitchType,
                    SwitchingDate = SelectedSwitch.SwitchingDate.ToLocal()
                });
                
            Unlock();

            Refresh();
        }

        public void AddValveSwitch()
        {
            var vm = new AddValveSwithViewModel(o => Refresh(), _selectedDate, _selectedSite.Id);
            var v = new AddValveSwitchView { DataContext = vm };
            v.ShowDialog();
        }


        private async void GetInputState()
        {
            Lock();

            var serie = new SeriesDataServiceProxy().AddSerieAsync(
                new AddSeriesParameterSet
                {
                    KeyDate = SelectedDate.ToLocal(),
                    PeriodTypeId = PeriodType.Twohours
                });

            
            var stateList = (await new ManualInputServiceProxy().GetInputStateListAsync(
                new GetManualInputStateListParameterSet
                {
                    SerieId = serie.Id,
                    SiteId = SelectedSite.Id
                }));


            var state = stateList.FirstOrDefault() ?? new ManualInputStateDTO();
            InputState = state.State;
            OnPropertyChanged(() => InputState);

            InputStateInfo = state.ChangeDate.HasValue
                ? $"{state.UserName} ({state.ChangeDate:dd.MM.yyyy HH:mm})"
                : string.Empty;
            OnPropertyChanged(() => InputStateInfo);

            AddCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();

            Unlock();

        }
    }
}