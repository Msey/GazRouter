using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Balances.Commercial.Dialogs.ClearValues;
using GazRouter.Balances.Commercial.Dialogs.DivideVolume;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Balances;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Balances.InputStates;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceItems;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.Prism.Regions;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Balances.Commercial.SiteInput
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class SiteInputViewModel : LockableViewModel
    {
        private readonly bool _writePermision;

        public SiteInputViewModel()
        {
            _writePermision = Authorization2.Inst.IsEditable(LinkType.SiteInput);

            _selectedUnitType = IsolatedStorageManager.Get<int?>("VolumeInputUnits") ?? 1;
            _selectedInputMode = IsolatedStorageManager.Get<InputMode?>("CommercialSelectedInputMode") ??
                                 InputMode.MonthValue;

            Intake = new TableViewModel(@base => { });
            Transit = new TableViewModel(@base => { });
            Consumers = new TableViewModel(@base => { });
            OperConsumers = new TableViewModel(@base => { });
            
            RefreshCommand = new DelegateCommand(Load);

            LoadSiteList();

            SelectedMonth = SeriesHelper.GetPastDispDay().MonthStart();

            AutoDivideCommand = new DelegateCommand(AutoDivide, () => _writePermision);
            ClearValuesCommand = new DelegateCommand(ClearValues, () => _writePermision);
        }


        #region SITE_LIST

        public List<SiteDTO> SiteList { get; set; }

        private SiteDTO _selectedSite;

        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                _selectedSite = value;
                OnPropertyChanged(() => SelectedSite);
                Load();
            }
        }

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

        #endregion


        public IEnumerable<InputMode> InputModeList => Enum.GetValues(typeof (InputMode)).OfType<InputMode>();

        private InputMode _selectedInputMode;

        public InputMode SelectedInputMode
        {
            get { return _selectedInputMode; }
            set
            {
                if (SetProperty(ref _selectedInputMode, value))
                {
                    OnPropertyChanged(() => IsDayInputMode);
                    IsolatedStorageManager.Set("CommercialSelectedInputMode", value);

                    SetSelectedDay();
                }
            }
        }

        public bool IsDayInputMode => _selectedInputMode == InputMode.DayValue;


        private DateTime _selectedMonth;

        public DateTime SelectedMonth
        {
            get { return _selectedMonth; }
            set
            {
                if (SetProperty(ref _selectedMonth, value))
                {
                    SetSelectedDay();
                }
            }
        }

        public IEnumerable<int> DayList
        {
            get
            {
                for (var d = 1; d <= _selectedMonth.DaysInMonth(); d++)
                    yield return d;
            }
        }

        private int _selectedDay;

        public int SelectedDay
        {
            get { return _selectedDay; }
            set
            {
                if (SetProperty(ref _selectedDay, value))
                    Load();
            }
        }

        private void SetSelectedDay()
        {
            if (_selectedInputMode == InputMode.DayValue)
            {
                OnPropertyChanged(() => DayList);
                var pastDay = SeriesHelper.GetPastDispDay();
                _selectedDay = _selectedMonth.Month == pastDay.Month && _selectedMonth.Year == pastDay.Year
                    ? pastDay.Day
                    : 1;
                OnPropertyChanged(() => SelectedDay);
                Load();
            }
            else
                Load();
        }


        #region UNITS

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
                    Load();
                }
            }
        }

        private int Coef => _selectedUnitType == 0 ? 1 : 1000;

        private string VolumeFormat => _selectedUnitType == 0 ? "#,0.000" : "#,0";

        #endregion


        public TableViewModel Intake { get; set; }

        public TableViewModel Transit { get; set; }

        public TableViewModel Consumers { get; set; }

        public TableViewModel OperConsumers { get; set; }


        public IEnumerable<TableViewModel> TabList
        {
            get
            {
                yield return Intake;
                yield return Transit;
                yield return Consumers;
                yield return OperConsumers;
            }
        }


        private CommercialData _data;

        #region LOAD

        public DelegateCommand RefreshCommand { get; set; }

        private async void Load()
        {
            if (SelectedSite == null) return;

            Lock();

            var date = SelectedInputMode == InputMode.DayValue
                ? new DateTime(SelectedMonth.Year, SelectedMonth.Month, SelectedDay)
                : SelectedMonth;
            _data = await CommercialData.GetData(SelectedInputMode, date, SelectedSite.SystemId, SelectedSite.Id);


            _inputState = _data.InputState.State;
            OnPropertyChanged(() => InputState);

            InputStateInfo = _data.InputState.ChangeDate.HasValue
                ? $"{_data.InputState.UserName} ({_data.InputState.ChangeDate:dd.MM.yyyy HH:mm})"
                : string.Empty;
            OnPropertyChanged(() => InputStateInfo);

            OnPropertyChanged(() => IsReadOnly);
            OnPropertyChanged(() => IsInputStateChangeAllowed);


            foreach (var tab in TabList)
            {
                tab.IsReadOnly = IsReadOnly;
                tab.ValueFormat = VolumeFormat;
            }
            

            // ПОСТУПЛЕНИЕ,ТРАНЗИТ
            Intake.Items = new List<ItemBase>();
            Transit.Items = new List<ItemBase>();
            

            var intakeItem = new SummaryItem
            {
                Name = SelectedSite.Name,
                ImgSrc = new BitmapImage(new Uri("/Common;component/Images/16x16/object2.png", UriKind.Relative)),
                FontWeight = FontWeights.Bold
            };
            Intake.Items.Add(intakeItem);

            var transitItem = new SummaryItem
            {
                Name = SelectedSite.Name,
                ImgSrc = new BitmapImage(new Uri("/Common;component/Images/16x16/object2.png", UriKind.Relative)),
                FontWeight = FontWeights.Bold
            };
            Transit.Items.Add(transitItem);

            foreach (var station in _data.GetMeasStationList())
            {
                var stationItem = new SummaryItem
                {
                    Name = station.Name,
                    ImgSrc =
                        new BitmapImage(new Uri("/Common;component/Images/16x16/EntityTypes/meas_line.png",
                            UriKind.Relative)),
                    Measured = _data.Measurings.GetOrDefault(station.Id)*Coef
                };
                var balItem = station.BalanceSignId == Sign.In ? BalanceItem.Intake : BalanceItem.Transit;
                foreach (var owner in _data.GetOwnerList(SelectedSite.SystemId))
                {
                    var ownerItem = new OwnerItem(station.Id, owner, balItem, SetValue)
                    {
                        Current = _data.GetFactValue(station.Id, owner.Id, balItem, Coef),
                        Plan = _data.GetPlanValue(station.Id, owner.Id, balItem, Coef)
                    };

                    if (ownerItem.GetVisibility(SelectedSite.SystemId))
                        stationItem.AddChild(ownerItem);
                }
                if (station.BalanceSignId == Sign.In)
                    intakeItem.AddChild(stationItem);
                else
                    transitItem.AddChild(stationItem);
            }
            Intake.UpdateItems();
            Transit.UpdateItems();


            // ПОТРЕБИТЕЛИ 
            Consumers.Items = new List<ItemBase>();
            var consumersItem = new SummaryItem
            {
                Name = SelectedSite.Name,
                ImgSrc = new BitmapImage(new Uri("/Common;component/Images/16x16/object2.png", UriKind.Relative)),
                FontWeight = FontWeights.Bold
            };
            Consumers.Items.Add(consumersItem);

            foreach (var station in _data.DistrStations.DistrStations)
            {
                var stationItem = new SummaryItem
                {
                    Name = station.Name,
                    ImgSrc =
                        new BitmapImage(new Uri("/Common;component/Images/16x16/EntityTypes/distr_station.png",
                            UriKind.Relative)),
                    Measured = _data.Measurings.GetOrDefault(station.Id)*Coef
                };
                foreach (var consumer in _data.GetConsumerList(station.Id))
                {
                    var consumerItem = new SummaryItem
                    {
                        Name = consumer.Name
                    };

                    foreach (var owner in _data.GetOwnerList(SelectedSite.SystemId))
                    {
                        var ownerItem = new OwnerItem(consumer.Id, owner, BalanceItem.Consumers, SetValue)
                        {
                            Current = _data.GetFactValue(consumer.Id, owner.Id, BalanceItem.Consumers, Coef),
                            Plan = _data.GetPlanValue(consumer.Id, owner.Id, BalanceItem.Consumers, Coef)
                        };

                        if (ownerItem.GetVisibility(SelectedSite.SystemId))
                            consumerItem.AddChild(ownerItem);
                    }

                    if (consumerItem.Childs.Any())
                        stationItem.AddChild(consumerItem);
                }

                if (stationItem.Childs.Any())
                    consumersItem.AddChild(stationItem);
            }
            Consumers.UpdateItems();


            //// СТН
            //AuxCosts.Items = new List<ItemBase>();
            //var siteItem = new SummaryItem
            //{
            //    Name = SelectedSite.Name,
            //    Measured = 0
            //};
            //foreach (var owner in _data.Owners.Where(o => o.DisableList.All(d => d.EntityId != SelectedSite.Id)))
            //{
            //    var ownerItem = new InputItem(SelectedSite.Id, owner.Id, owner.Name,
            //        _data.GetValue(SelectedSite.Id, owner.Id, Coef),
            //        _data.GetPrevValue(SelectedSite.Id, owner.Id, Coef),
            //        SetValue);
            //    siteItem.AddChild(ownerItem);
            //}
            //AuxCosts.Items.Add(siteItem);



            // ПЭН
            OperConsumers.Items = new List<ItemBase>();
            var operConsumersItem = new SummaryItem
            {
                Name = SelectedSite.Name,
                ImgSrc = new BitmapImage(new Uri("/Common;component/Images/16x16/object2.png", UriKind.Relative)),
                FontWeight = FontWeights.Bold
            };
            OperConsumers.Items.Add(operConsumersItem);
            foreach (var operConsumer in _data.OperConsumers.Where(c => c.ParentId == SelectedSite.Id))
            {
                var operConsumerItem = new SummaryItem
                {
                    Name = operConsumer.Name,
                    Measured = _data.Measurings.GetOrDefault(operConsumer.Id)*Coef
                };
                foreach (var owner in _data.GetOwnerList(SelectedSite.SystemId))
                {
                    var ownerItem = new OwnerItem(operConsumer.Id, owner, BalanceItem.OperConsumers, SetValue)
                    {
                        Current = _data.GetFactValue(operConsumer.Id, owner.Id, BalanceItem.OperConsumers, Coef),
                        Plan = _data.GetPlanValue(operConsumer.Id, owner.Id, BalanceItem.OperConsumers, Coef)
                    };

                    if (ownerItem.GetVisibility(SelectedSite.SystemId))
                        operConsumerItem.AddChild(ownerItem);
                }
                operConsumersItem.AddChild(operConsumerItem);
            }
            OperConsumers.UpdateItems();

            Unlock();
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

        public bool IsReadOnly => !_writePermision || _inputState != ManualInputState.Input;

    #endregion



        private async void SetInputState(ManualInputState targetState)
        {
            if (SelectedSite == null || _data == null) return;
            await new BalancesServiceProxy().SetInputStateAsync(
                new SetInputStateParameterSet
                {
                    ContractId = _data.ContractDto.Id,
                    SiteId = SelectedSite.Id,
                    State = targetState
                });

            Load();
        }


        private async void SetValue(Guid pointId, int ownerId, BalanceItem balItem, double? value)
        {
            await new BalancesServiceProxy().SetBalanceValueAsync(
                new SetBalanceValueParameterSet
                {
                    ContractId = _data.ContractDto.Id,
                    EntityId = pointId,
                    GasOwnerId = ownerId,
                    BalanceItem = balItem,
                    BaseValue = value / Coef
                });
        }


        public DelegateCommand AutoDivideCommand { get; set; }

        private void AutoDivide()
        {
            var vm =
                new DivideVolumeViewModel(
                    _data.Owners.Where(o => o.SystemList.Any(s => s == SelectedSite.SystemId)).ToList(), SelectedSite.Id,
                    _data.ContractDto.Id, Load);
            var v = new DivideVolumeView {DataContext = vm};
            v.ShowDialog();
        }


        #region УДАЛЕНИЕ ЗНАЧЕНИЙ

        public DelegateCommand ClearValuesCommand { get; set; }
        private void ClearValues()
        {
            var vm = new ClearValuesViewModel(_data.ContractDto.Id, SelectedSite.Id, _data.Owners, Load);
            var v = new ClearValuesView { DataContext = vm };
            v.ShowDialog();
        }

        #endregion

    }
}