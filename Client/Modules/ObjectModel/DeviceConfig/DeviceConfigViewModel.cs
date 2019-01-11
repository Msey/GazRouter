using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.BoilerTypes;
using GazRouter.DTO.Dictionaries.EmergencyValveTypes;
using GazRouter.DTO.Dictionaries.HeaterTypes;
using GazRouter.DTO.Dictionaries.PowerUnitTypes;
using GazRouter.DTO.Dictionaries.RegulatorTypes;
using GazRouter.ObjectModel.DeviceConfig.Dialogs;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Common;

namespace GazRouter.ObjectModel.DeviceConfig
{
    public class DeviceConfigViewModel : LockableViewModel, IPageTitle
    {
        public DeviceConfigViewModel()
        {
            var isEditPermission = Authorization2.Inst.IsEditable(LinkType.DeviceConfig);

            PropertyChanged += (s, e) => { if(e.PropertyName == nameof(SelectedDeviceType)) OnPropertyChanged(() => PageTitle);};

            AddCommand = new DelegateCommand(Add, () => isEditPermission);
            EditCommand = new DelegateCommand(Edit, () => SelectedItem != null && isEditPermission);
            DeleteCommand = new DelegateCommand(Delete, () => SelectedItem != null && isEditPermission);
            RefreshCommand = new DelegateCommand(Refresh);
            PublishCommand = new DelegateCommand(PublishChanges, () => isEditPermission);
            ShowChangesCommand = new DelegateCommand(ShowChanges);

            vmiDict = new Dictionary<DeviceType, IDeviceConfigViewModelImplementation>()
            {
                { _dtPowerUnit, new PowerUnitTypeViewModelImplementation() },
                { _dtBoiler, new BoilerTypeViewModelImplementation() },
                { _dtRegulator, new RegulatorTypeViewModelImplementation() },
                { _dtEmergencyValve, new EmergencyValveTypeViewModelImplementation() },
                { _dtHeater, new HeaterTypeViewModelImplementation() },
            };
        }
        
        public string PageTitle => "Test Title v2: " + SelectedDeviceType?.Name;

        public string TestTitle {  get { return "Test Title"; } }

        private Dictionary<DeviceType, IDeviceConfigViewModelImplementation> vmiDict;
        private IDeviceConfigViewModelImplementation ViewModelImplementation { get { return vmiDict[SelectedDeviceType]; } }

        private static DeviceType _dtPowerUnit = DeviceType.FromName("Типы электроагрегатов");
        private static DeviceType _dtBoiler = DeviceType.FromName("Типы котлов");
        private static DeviceType _dtRegulator = DeviceType.FromName("Типы кранов-регуляторов");
        private static DeviceType _dtEmergencyValve = DeviceType.FromName("Типы предохранительных клапанов");
        private static DeviceType _dtHeater = DeviceType.FromName("Типы подогревателей газа");
        private DeviceType _selectedDeviceType = _dtPowerUnit;


        public IEnumerable<DeviceType> DeviceTypes => vmiDict.Keys;
        public DeviceType SelectedDeviceType
        {
            get { return _selectedDeviceType; }
            set
            {
                if(value != _selectedDeviceType)
                {
                    _selectedDeviceType = value;
                    SelectedItem = null;
                    Refresh();
                }
            }
        }

        private ObservableCollection<object> _items;
        public ObservableCollection<object> Items
        {
            get
            {
                if (_items == null) Refresh();
                return _items;
            }
        }

        private object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    RaiseCommands();
                    OnPropertyChanged(() => SelectedItem);
                }
            }
        }

        public BaseDictionaryDTO SelectedDTO => (SelectedItem as DtoWrapper)?.Dto;

        protected void RaiseCommands()
        {
            AddCommand?.RaiseCanExecuteChanged();
            EditCommand?.RaiseCanExecuteChanged();
            DeleteCommand?.RaiseCanExecuteChanged();
            RefreshCommand?.RaiseCanExecuteChanged();
        }

        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand PublishCommand { get; set; }
        public DelegateCommand ShowChangesCommand { get; set; }

        protected void Add()
        {
            ViewModelImplementation.Add(_ => Refresh());
        }

        protected void Edit()
        {
            ViewModelImplementation.Edit(_ => Refresh(), SelectedDTO);
        }

        protected async void Refresh()
        {
            Lock();
            try {
                _items = new ObservableCollection<object>(await ViewModelImplementation.GetObjects());

                OnPropertyChanged(() => SelectedDeviceType);
                OnPropertyChanged(() => Items);
            }
            finally
            {
                Unlock();
            }
        }

        protected async void PublishChanges()
        {
            Lock();
            try
            {
                await ClientCache.DictionaryRepository.Load(true).ConfigureAwait(true);
                OnPropertyChanged(() => SelectedDeviceType);
                OnPropertyChanged(() => Items);
            }
            finally
            {
                Unlock();
            }
        }

        private void ShowChanges()
        {
            var vm = new ChangeLogViewModel(_selectedDeviceType, DeviceTypes);
            var v = new ChangeLogView { DataContext = vm};
            v.ShowDialog();
        }
        
        protected void Delete()
        {
            MessageBoxProvider.Confirm("Удалить " + SelectedDTO.Name,
                async confirmed =>
                {
                    if (confirmed)
                    {
                        await ViewModelImplementation.Remove(SelectedDTO.Id);
                        Refresh();
                    }
                });
        }
    }

    public class DeviceType
    {
        public string Name { get; set; }

        internal static DeviceType FromName(string name)
        {
            return new DeviceType() { Name = name };
        }
    }

    public class DtoWrapper
    {
        protected BaseDictionaryDTO _dto;
        public DtoWrapper(BaseDictionaryDTO dto)
        {
            this._dto = dto;
        }

        [Browsable(false)]
        [Display(AutoGenerateField = false)]
        public BaseDictionaryDTO Dto => _dto;
    }

    public class DtoWrapperT<T> : DtoWrapper where T : BaseDictionaryDTO
    {
        public DtoWrapperT(T dto) : base(dto)
        { }

        protected T dto => (T)_dto;

        [Browsable(false)]
        [Display(Name = "№", Order = -200)]
        public int Id => dto.Id;

        [Display(Name = "Наименование", Order = -100)]
        public string Name => dto.Name;

        private bool Equals(DtoWrapperT<T> other)
        {
            return Id == other.Id;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((DtoWrapperT<T>)obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }


    public class PowerUnitTypeWrapper : DtoWrapperT<PowerUnitTypeDTO>
    {
        public PowerUnitTypeWrapper(PowerUnitTypeDTO dto) : base(dto)
        {
        }

        [Display(Name = "Тип двигателя")]
        public string EngineType => dto.EngineGroupName;

        [Display(Name = "Номинальная электрическая мощность, кВт")]
        public double RatedPower => dto.RatedPower;

        [Display(Name = "Норматив расхода условного топлива, кг у.т./Гкал")]
        public string FuelConsumptionRate => dto.FuelConsumptionRate.ToString("0.###");

        [Display(Name = "Группа двигателя")]
        public string EngineGroup => dto.EngineTypeName;

        [Display(Name = "Комментарий")]
        public string Description => dto.Description;
    }

    public class BoilerTypeWrapper : DtoWrapperT<BoilerTypeDTO>
    {
        public BoilerTypeWrapper(BoilerTypeDTO dto) : base(dto)
        {
        }

        [Display(Name = "Номинальный КПД")]
        public string EfficiencyRated => dto.RatedEfficiencyFactor.ToString("0.###");

        [Display(Name = "Номинальная теплопроизводительность, Гкал/ч")]
        public string HeatProductivityRated => dto.RatedHeatingEfficiency.ToString("0.###");

        [Display(Name = "Группа")]
        public string Group => dto.GroupName;

        [Display(Name = "Котел малой мощности?")]
        public bool IsSmall => dto.IsSmall;

        [Display(Name = "Площадь нагрева котла, м2")]
        public string HeatingArea => dto.HeatingArea.ToString("0.###");

        [Display(Name = "Комментарий")]
        public string Description => dto.Description;
    }

    public class RegulatorTypeWrapper : DtoWrapperT<RegulatorTypeDTO>
    {
        public RegulatorTypeWrapper(RegulatorTypeDTO dto) : base(dto)
        {
        }

        [Display(Name = "Номинальный расход газа, тыс. м3/ч")]
        public string GasConsumptionRate => dto.GasConsumptionRate.ToString("0.###");

        [Display(Name = "Комментарий")]
        public string Description => dto.Description;
    }

    public class EmergencyValveTypeWrapper : DtoWrapperT<EmergencyValveTypeDTO>
    {
        public EmergencyValveTypeWrapper(EmergencyValveTypeDTO dto) : base(dto)
        {
        }

        [Display(Name = "Внутренний диаметр")]
        public string InnerDiameter => dto.InnerDiameter.ToString("0.###");

        [Display(Name = "Комментарий")]
        public string Description => dto.Description;
    }

    public class HeaterTypeWrapper : DtoWrapperT<HeaterTypeDTO>
    {
        public HeaterTypeWrapper(HeaterTypeDTO dto) : base(dto)
        {
        }

        [Display(Name = "Номинальный расход газа, тыс. м3/ч")]
        public string GasConsumptionRate => dto.GasConsumptionRate.ToString("0.###");

        [Display(Name = "Номинальный КПД")]
        public string EfficiencyRated
        {
            get
            {
                if (dto.EffeciencyFactorRated == null) return string.Empty;
                return Convert.ToDouble(dto.EffeciencyFactorRated).ToString("0.###");
            }
        }

        [Display(Name = "Комментарий")]
        public string Description => dto.Description;
    }

    internal interface IDeviceConfigViewModelImplementation
    {
        void Add(Action<int> actionBeforeClosing);
        void Edit(Action<int> actionBeforeClosing, BaseDictionaryDTO selectedDTO);
        Task<IEnumerable<object>> GetObjects();
        Task Remove(int id);
    }

    internal class PowerUnitTypeViewModelImplementation : IDeviceConfigViewModelImplementation
    {
        public void Add(Action<int> actionBeforeClosing)
        {
            var viewModel = new AddPowerUnitViewModel(actionBeforeClosing);
            var view = new AddPowerUnitView() { DataContext = viewModel };
            view.ShowDialog();
        }

        public void Edit(Action<int> actionBeforeClosing, BaseDictionaryDTO selectedDTO)
        {
            var viewModel = new AddPowerUnitViewModel(actionBeforeClosing, (PowerUnitTypeDTO)selectedDTO);
            var view = new AddPowerUnitView() { DataContext = viewModel };
            view.ShowDialog();
        }

        public async Task<IEnumerable<object>> GetObjects()
        {
            return (await new ObjectModelServiceProxy().GetPowerUnitTypesAsync()).Select(x => new PowerUnitTypeWrapper(x));
        }

        public async Task Remove(int id)
        {
            await new ObjectModelServiceProxy().RemovePowerUnitTypeAsync(id);
        }
    }

    internal class BoilerTypeViewModelImplementation : IDeviceConfigViewModelImplementation
    {
        public void Add(Action<int> actionBeforeClosing)
        {
            var viewModel = new AddBoilerTypeViewModel(actionBeforeClosing);
            var view = new AddBoilerTypeView() { DataContext = viewModel };
            view.ShowDialog();
        }

        public void Edit(Action<int> actionBeforeClosing, BaseDictionaryDTO selectedDTO)
        {
            var viewModel = new AddBoilerTypeViewModel(actionBeforeClosing, (BoilerTypeDTO)selectedDTO);
            var view = new AddBoilerTypeView() { DataContext = viewModel };
            view.ShowDialog();
        }

        public async Task<IEnumerable<object>> GetObjects()
        {
            return (await new ObjectModelServiceProxy().GetBoilerTypesAsync()).Select(x => new BoilerTypeWrapper(x));
        }

        public async Task Remove(int id)
        {
            await new ObjectModelServiceProxy().RemoveBoilerTypeAsync(id);
        }
    }

    internal class RegulatorTypeViewModelImplementation : IDeviceConfigViewModelImplementation
    {
        public void Add(Action<int> actionBeforeClosing)
        {
            var viewModel = new AddRegulatorTypeViewModel(actionBeforeClosing);
            var view = new AddRegulatorTypeView() { DataContext = viewModel };
            view.ShowDialog();
        }

        public void Edit(Action<int> actionBeforeClosing, BaseDictionaryDTO selectedDTO)
        {
            var viewModel = new AddRegulatorTypeViewModel(actionBeforeClosing, (RegulatorTypeDTO)selectedDTO);
            var view = new AddRegulatorTypeView() { DataContext = viewModel };
            view.ShowDialog();
        }

        public async Task<IEnumerable<object>> GetObjects()
        {
            return (await new ObjectModelServiceProxy().GetRegulatorTypesAsync()).Select(x => new RegulatorTypeWrapper(x));
        }

        public async Task Remove(int id)
        {
            await new ObjectModelServiceProxy().RemoveRegulatorTypeAsync(id);
        }
    }

    internal class EmergencyValveTypeViewModelImplementation : IDeviceConfigViewModelImplementation
    {
        public void Add(Action<int> actionBeforeClosing)
        {
            var viewModel = new AddEmergencyValveTypeViewModel(actionBeforeClosing);
            var view = new AddEmergencyValveTypeView() { DataContext = viewModel };
            view.ShowDialog();
        }

        public void Edit(Action<int> actionBeforeClosing, BaseDictionaryDTO selectedDTO)
        {
            var viewModel = new AddEmergencyValveTypeViewModel(actionBeforeClosing, (EmergencyValveTypeDTO)selectedDTO);
            var view = new AddEmergencyValveTypeView() { DataContext = viewModel };
            view.ShowDialog();
        }


        public async Task<IEnumerable<object>> GetObjects()
        {
            return (await new ObjectModelServiceProxy().GetEmergencyValveTypesAsync()).Select(x => new EmergencyValveTypeWrapper(x));
        }

        public async Task Remove(int id)
        {
            await new ObjectModelServiceProxy().RemoveEmergencyValveTypeAsync(id);
        }
    }

    internal class HeaterTypeViewModelImplementation : IDeviceConfigViewModelImplementation
    {
        public void Add(Action<int> actionBeforeClosing)
        {
            var viewModel = new AddHeaterTypeViewModel(actionBeforeClosing);
            var view = new AddHeaterTypeView() { DataContext = viewModel };
            view.ShowDialog();
        }

        public void Edit(Action<int> actionBeforeClosing, BaseDictionaryDTO selectedDTO)
        {
            var viewModel = new AddHeaterTypeViewModel(actionBeforeClosing, (HeaterTypeDTO)selectedDTO);
            var view = new AddHeaterTypeView() { DataContext = viewModel };
            view.ShowDialog();
        }

        public async Task<IEnumerable<object>> GetObjects()
        {
            return (await new ObjectModelServiceProxy().GetHeaterTypesAsync()).Select(x => new HeaterTypeWrapper(x));
        }

        public async Task Remove(int id)
        {
            await new ObjectModelServiceProxy().RemoveHeaterTypeAsync(id);
        }
    }
}
