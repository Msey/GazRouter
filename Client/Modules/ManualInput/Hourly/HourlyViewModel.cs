using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.Controls;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.EntityValidationStatus;
using GazRouter.DTO.SeriesData.ValueMessages;
using GazRouter.ManualInput.Hourly.QuickForms;
using GazRouter.ManualInput.Hourly.QuickForms.CompShops;
using GazRouter.ManualInput.Hourly.QuickForms.CompStations;
using GazRouter.ManualInput.Hourly.QuickForms.CompUnits;
using GazRouter.ManualInput.Hourly.QuickForms.DistrStationOutlets;
using GazRouter.ManualInput.Hourly.QuickForms.DistrStations;
using GazRouter.ManualInput.Hourly.QuickForms.MeasLines;
using GazRouter.ManualInput.Hourly.QuickForms.MeasPoints;
using GazRouter.ManualInput.Hourly.QuickForms.ReducingStations;
using GazRouter.ManualInput.Hourly.QuickForms.Valves;
using Microsoft.Practices.Prism.Commands;
using Utils.Extensions;


namespace GazRouter.ManualInput.Hourly
{
    public class HourlyViewModel : LockableViewModel
    {
        private readonly bool _editPermission;

        public HourlyViewModel()
        {
            _editPermission = Authorization2.Inst.IsEditable(LinkType.Hourly);

            _selectedUnitType = IsolatedStorageManager.Get<int?>("VolumeInputUnits") ?? 1;
            
            RefreshCommand = new DelegateCommand(RefreshAll);
            RunCheckingsCommand = new DelegateCommand(RunCheckings, () => IsInputAllowed);
            CopyAllCommand = new DelegateCommand(CopyAll, () => IsInputAllowed);
            CopyCurrentTabCommand = new DelegateCommand(CopyCurrentTab, () => IsInputAllowed);

            Init();
        }


        private DataRefreshWatcher _refreshWatcher = new DataRefreshWatcher();
        private async void Init()
        {
            OnPropertyChanged(() => InputStateList);

            _selectedDate = DateTimePickerTwoHours.NowDateTime; //todo: заменить это на вызов новой функции
            OnPropertyChanged(() => SelectedDate);

            // получить список ЛПУ
            var siteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                new GetSiteListParameterSet
                {
                    EnterpriseId = UserProfile.Current.Site.IsEnterprise ? UserProfile.Current.Site.Id : (Guid?)null
                });
            if (UserProfile.Current.Site.IsEnterprise)
                SiteList = siteList;
            else
            {
                var site = siteList.Single(s => s.Id == UserProfile.Current.Site.Id);
                SiteList = siteList.Where(s => s.Id == site.Id || site.DependantSiteIdList.Contains(s.Id)).ToList();
            }
            OnPropertyChanged(() => SiteList);

            _selectedSite = SiteList.First();
            OnPropertyChanged(() => SelectedSite);

            RefreshAll();
            _refreshWatcher.TimeToRefresh += QuickRefresh;
            _refreshWatcher.Run();
        }

        public DelegateCommand RefreshCommand { get; set; }

        private async void QuickRefresh(object sender, EventArgs e)
        {
            if (SelectedForm != null && SelectedSite!=null && !IsPendingChanges)
            {
                var volumeUnits = _selectedUnitType == 0 ? VolumeUnits.Km : VolumeUnits.M;
                var newData = await HourlyData.LoadData(SelectedDate.ToLocal(), SelectedSite.Id);

                foreach (var form in FormList.Where(f => f.GetType() == SelectedForm.GetType()))
                {
                    if (!IsPendingChanges)
                    {
                        if (IsOldRead)
                        {
                            IsOldRead = false;
                        }
                        else
                        {
                            form.HighlightUpdates(newData, volumeUnits);
                        }
                    }
                }
            }
        }
        public bool IsPendingChanges { get { return FormList.Any(f => f.IsPendingChanges); } }
        public bool IsOldRead { get { return FormList.Any(f => f.IsOldRead); } private set { foreach (var form in FormList) form.IsOldRead = value; } }

        private int _serieId;
        private async void RefreshAll()
        {
            if (SelectedSite == null) return;

            Lock();

            var data = await HourlyData.LoadData(SelectedDate.ToLocal(), SelectedSite.Id);
            _serieId = data.Serie.Id;

            _inputState = data.InputState.State;
            OnPropertyChanged(() => InputState);

            InputStateInfo = data.InputState.ChangeDate.HasValue
                ? $"{data.InputState.UserName} ({data.InputState.ChangeDate:dd.MM.yyyy HH:mm})"
                : string.Empty;
            OnPropertyChanged(() => InputStateInfo);


            _messageList = data.MsgList.SelectMany(o => o.Value).SelectMany(p => p.Value).ToList();


            var selectedEntityType = SelectedForm?.EntityType; // чтобы восстановить выбранную вкладку после обновления
            var volumeUnits = _selectedUnitType == 0 ? VolumeUnits.Km : VolumeUnits.M;
            FormList = new List<QuickForm>
            {
                new CompStationsViewModel(data, volumeUnits) {IsReadOnly = !IsInputAllowed},
                new CompShopsViewModel(data, volumeUnits) {IsReadOnly = !IsInputAllowed},
                new CompUnitsViewModel(data, volumeUnits) {IsReadOnly = !IsInputAllowed},
                new MeasLinesViewModel(data, volumeUnits) {IsReadOnly = !IsInputAllowed},
                new DistrStationsViewModel(data, volumeUnits) {IsReadOnly = !IsInputAllowed},
                new DistrStationOutletsViewModel(data, volumeUnits) {IsReadOnly = !IsInputAllowed},
                new ReducingStationsViewModel(data, volumeUnits) {IsReadOnly = !IsInputAllowed},
                new ValvesViewModel(data, volumeUnits) {IsReadOnly = !IsInputAllowed},
                new MeasPointsViewModel(data, volumeUnits) {IsReadOnly = !IsInputAllowed}
            };
            FormList = FormList.Where(x => x.Items.Count > 0).ToList();
            OnPropertyChanged(() => FormList);

            SelectedForm = FormList.SingleOrDefault(x => x.EntityType == selectedEntityType) ?? FormList.FirstOrDefault();

            RunCheckingsCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(() => IsInputStateChangeAllowed);
            OnPropertyChanged(() => IsInputAllowed);
            OnPropertyChanged(() => IsAllGood);

            Unlock();
        }
        

        public List<QuickForm> FormList { get; set; }

        public QuickForm SelectedForm
        {
            get { return _selectedForm; }
            set
            {
                if (SetProperty(ref _selectedForm, value))
                {
                    OnPropertyChanged(() => MessageList);
                    OnPropertyChanged(() => MessageCount);
                }
            }
        }

        public bool IsAllGood
            =>
                FormList.All(
                    f =>
                        f.ValidationStatus != EntityValidationStatus.Error &&
                        f.ValidationStatus != EntityValidationStatus.NotChecked);



        #region DATE

        private DateTime _selectedDate;

        /// <summary>
        /// Выбранная дата. 
        /// Должна быть задана всегда, NULL недопустим. 
        /// Поэтому первым делом инициализируется в конструкторе.
        /// </summary>
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if (SetProperty(ref _selectedDate, value.ToLocal()))
                {
                    RefreshAll();
                }
            }
        }

        #endregion

        #region SITE

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
                    RefreshAll();
                }
            }
        }

        #endregion

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

        #endregion

        
        public bool IsInputAllowed => InputState == ManualInputState.Input && _editPermission;

        public DelegateCommand RunCheckingsCommand { get; set; }

        private async void RunCheckings()
        {
            if (_serieId == 0) return;
            Lock();
            var cList = FormList.SelectMany(f => f.CheckList).Select(i => new PerformCheckingParameterSet
            {
                SerieId = _serieId,
                EntityId = i,
                SaveHistory = true
            }).ToList();
            
            await new SeriesDataServiceProxy().PerformCheckingAsync(cList);
            Unlock();
            RefreshAll();
        }


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
                        return IsAllGood && _editPermission;

                    case ManualInputState.Approved:
                        // Сбросить статус "Подтверждено" может только пользователь ПДС
                        return UserProfile.Current.Site.IsEnterprise && _editPermission;

                    default:
                        return false;
                }
            }
        }

        
        private async void SetInputState(ManualInputState targetState)
        {
            
            await new ManualInputServiceProxy().SetInputStateAsync(
                new SetManualInputStateParameterSet
                {
                    SerieId = _serieId,
                    SiteId = _selectedSite.Id,
                    State = targetState
                });

            RefreshAll();
        }



        #region COPY PREV VALUES

        public DelegateCommand CopyAllCommand { get; set; }

        public DelegateCommand CopyCurrentTabCommand { get; set; }

        private void CopyAll()
        {
            if (FormList != null) FormList.ForEach(f => f.CopyValues());
        }

        private void CopyCurrentTab()
        {
            if(SelectedForm != null) SelectedForm.CopyValues();
        }
        
        #endregion


        #region UNIT TYPE

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
                    RefreshAll();
                }
            }
        }

        #endregion


        #region MESSAGE PANEL 

        private bool _showMessagePanel;

        public bool ShowMessagePanel
        {
            get { return _showMessagePanel; }
            set { SetProperty(ref _showMessagePanel, value); }
        }


        private List<PropertyValueMessageDTO> _messageList;
        private QuickForm _selectedForm;

        public List<PropertyValueMessageDTO> MessageList
            => _messageList?.Where(m => m.EntityType == SelectedForm?.EntityType).ToList();

        public int? MessageCount => MessageList?.Count;

        #endregion

    }


   
    
 
}