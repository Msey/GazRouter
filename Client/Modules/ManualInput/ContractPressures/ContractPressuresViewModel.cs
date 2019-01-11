using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Browser;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.Controls;
using GazRouter.Controls.InputStory;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DataProviders.Integro;
using GazRouter.DTO.DataExchange.ASUTPImport;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.Integro;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.Integro;
using GazRouter.DTO.ManualInput.InputStates;
using GazRouter.DTO.SeriesData.EntityValidationStatus;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.ManualInput.ContractPressures;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.ManualInput.ContractPressures
{
    public class ContractPressuresViewModel : LockableViewModel
    {
        private DateTime _SelectedDate;
        public DateTime SelectedDate
        {
            get { return _SelectedDate; }
            set
            {
                value = new DateTime(value.Year, value.Month,DateTime.DaysInMonth(value.Year, value.Month));
                if (SetProperty(ref _SelectedDate, value))
                {
                    LoadData();
                }
            }
        }

        public List<SiteDTO> SiteList { get; private set;}
        private SiteDTO _SelectedSite;
        public SiteDTO SelectedSite
        {
            get { return _SelectedSite; }
            set
            {
                if(SetProperty(ref _SelectedSite, value))
                {
                    LoadData();
                }
            }
        }

        public List<ContractPressureViewModel> DataItems { get; private set; }

        private ContractPressureViewModel _SelectedDataItem;
        public ContractPressureViewModel SelectedDataItem
        {
            get { return _SelectedDataItem; }
            set
            {
                if (SetProperty(ref _SelectedDataItem, value))
                {
                    LoadChangingHistory(value);
                }
            }
        }

        public List<ContractPressureHistoryViewModel> HistoryDataItems { get; private set; }

        private List<ContractPressureViewModel> _ChangedDataItems;

        private DelegateCommand _RefreshDataItemsCommand;
        public DelegateCommand RefreshDataItemsCommand => _RefreshDataItemsCommand;

        private DelegateCommand _SaveChangesCommand;
        public DelegateCommand SaveChangesCommand => _SaveChangesCommand;

        public ContractPressuresViewModel()
        {
            DateTime dtNow = DateTime.Now;
            _SelectedDate = new DateTime(dtNow.Year, dtNow.Month,DateTime.DaysInMonth(dtNow.Year, dtNow.Month));
            SiteList = new List<SiteDTO>();
            DataItems = new List<ContractPressureViewModel>();
            HistoryDataItems = new List<ContractPressureHistoryViewModel>();
            _ChangedDataItems = new List<ContractPressureViewModel>();
            _RefreshDataItemsCommand = new DelegateCommand(LoadData);
            _SaveChangesCommand = new DelegateCommand(SaveChanges, () => _ChangedDataItems.Count > 0);
            LoadSites();
        }

        private async void LoadSites()
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
            OnPropertyChanged(() => SiteList);
            SelectedSite = SiteList[0];
        }
        private async void LoadData()
        {
            if (_ChangedDataItems.Count > 0)
            {
                if (MessageBox.Show("Есть несохранённые изменения. Отменить все изменения и продолжить?", "Внимание!", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    DropChanges();
                }
                else return;
            }
            Behavior.TryLock("Загрузка данных");
            try
            {
                Guid? lastSelectedItemId = SelectedDataItem?.DistrStationOutletId;
                SelectedDataItem=null;
                foreach (var Item in DataItems)
                    Item.PropertyChanged -= VM_PropertyChanged;
                //if (SelectedSite == null)
                //{
                //    DataItems = new List<ContractPressureViewModel>();
                //}
                //else
                {
                    var DtoDataItems = await new ManualInputServiceProxy().GetContractPressureListAsync(new GetContractPressureListQueryParameterSet()
                    {
                        SiteId = SelectedSite == null ? null : (Guid?)SelectedSite.Id,
                        TargetMonth = SelectedDate,
                    });
                    DataItems = await Task.Factory.StartNew(() =>
                    {
                        return DtoDataItems.Select(dto =>
                        {
                            var VM = new ContractPressureViewModel(dto);
                            VM.PropertyChanged += VM_PropertyChanged;
                            return VM;
                        }).ToList();
                    });
                    if (lastSelectedItemId.HasValue)
                        SelectedDataItem = DataItems.FirstOrDefault(i => i.DistrStationOutletId == lastSelectedItemId.Value);
                }
            }
            finally
            {
                OnPropertyChanged(() => DataItems);
                Behavior.TryUnlock();
            }
        }
        
        private async void LoadChangingHistory(ContractPressureViewModel DataItem)
        {
            Behavior.TryLock("Загрузка данных");
            HistoryDataItems.Clear();
            try
            {
                if (DataItem == null)
                {
                    DataItems = new List<ContractPressureViewModel>();
                }
                else
                {
                    var DtoHistoryDataItems = await new ManualInputServiceProxy().GetContractPressureHistoryListAsync(DataItem.DistrStationOutletId);
                    HistoryDataItems = await Task.Factory.StartNew(() =>
                    {
                        return DtoHistoryDataItems.Select(dto => new ContractPressureHistoryViewModel(dto)).ToList();
                    });
                }
            }
            finally
            {
                OnPropertyChanged(() => HistoryDataItems);
                Behavior.TryUnlock();
            }
        }

        private void VM_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var DataItem = (ContractPressureViewModel)sender;
            if (!DataItem.HasErrors)
            {
                _ChangedDataItems.Add(DataItem);
                if (_ChangedDataItems.Count == 1)
                    _SaveChangesCommand.RaiseCanExecuteChanged();
            }
        }

        private async void SaveChanges()
        {
            Behavior.TryLock("Сохранение изменений");
            try
            {
                await new ManualInputServiceProxy().AddEditContractPressuresAsync(_ChangedDataItems.Select(vm => new AddEditContractPressureParameterSet() { distr_station_outlet_id=vm.DistrStationOutletId, contract_pressure=vm.Pressure }).ToList());
                DropChanges();
            }
            finally
            {
                Behavior.TryUnlock();
            }
            LoadData();
        }

        private void DropChanges()
        {
            _ChangedDataItems.Clear();
            _SaveChangesCommand.RaiseCanExecuteChanged();
        }
    }

    public class ContractPressureViewModel : ValidationViewModel
    {
        public Guid SiteId => _DTO.SiteId;
        
        public Guid DistrStationId => _DTO.DistrStationId;

        public string DistrStationName => _DTO.DistrStationName;

        public Guid DistrStationOutletId => _DTO.DistrStationOutletId;

        public string DistrStationOutletName => _DTO.DistrStationOutletName;

        public double? Pressure
        {
            get { return _DTO.Pressure.HasValue? (double?)UserProfile.ToUserUnits(_DTO.Pressure.Value, GazRouter.DTO.Dictionaries.PhisicalTypes.PhysicalType.Pressure) :null; }
            set
            {
                if (value != Pressure)
                {
                    if (value.HasValue)
                    {
                        _DTO.Pressure = UserProfile.ToServerUnits(value.Value, GazRouter.DTO.Dictionaries.PhisicalTypes.PhysicalType.Pressure);
                        ValidateAll();
                    }
                    else
                        _DTO.Pressure = value;
                    OnPropertyChanged(() => Pressure);
                }
            }
        }

        public DateTime? ChangeDate => _DTO.ChangeDate;

        private readonly ContractPressureDTO _DTO;
        public ContractPressureViewModel(ContractPressureDTO Context)
        {
            _DTO = Context;
            AddValidationFor(() => Pressure)
               .When(() => Pressure.HasValue && ( Pressure.Value < ValueRangeHelper.PressureRange.Min.Kgh || Pressure.Value > ValueRangeHelper.PressureRange.Max.Kgh))
               .Show(ValueRangeHelper.PressureRange.ViolationMessage);
        }
    }

    public class ContractPressureHistoryViewModel: ViewModelBase
    {
        public double? Pressure
        {
            get { return _DTO.Pressure.HasValue ? (double?)UserProfile.ToUserUnits(_DTO.Pressure.Value, GazRouter.DTO.Dictionaries.PhisicalTypes.PhysicalType.Pressure) : null; }
            set
            {
                if (value.HasValue)
                {
                    _DTO.Pressure = UserProfile.ToServerUnits(value.Value, GazRouter.DTO.Dictionaries.PhisicalTypes.PhysicalType.Pressure);
                    OnPropertyChanged(() => Pressure);
                }
                _DTO.Pressure = value;
                OnPropertyChanged(() => Pressure);
            }
        }

        public DateTime ChangeDate => _DTO.ChangeDate;

        public string UserName => _DTO.UserName;

        public string UserSiteName => _DTO.UserSiteName;

        private readonly ContractPressureHistoryDTO _DTO;
        public ContractPressureHistoryViewModel(ContractPressureHistoryDTO Context)
        {
            _DTO = Context;
        }
    }
}
