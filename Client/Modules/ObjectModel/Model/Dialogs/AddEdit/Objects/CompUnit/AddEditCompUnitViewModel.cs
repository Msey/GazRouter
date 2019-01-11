using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GazRouter.Application;
using GazRouter.Controls.Dialogs.PipingVolumeCalculator;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.CompUnitSealingTypes;
using GazRouter.DTO.Dictionaries.CompUnitTypes;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.Dictionaries.SuperchargerTypes;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.ObjectModel.Model.Dialogs.Auxi;
using Microsoft.Practices.Prism.Commands;
using PipingVolumeCalculatorView = GazRouter.Controls.Dialogs.PipingVolumeCalculator.PipingVolumeCalculatorView;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.CompUnit
{
    public class AddEditCompUnitViewModel : AddEditViewModelBase<CompUnitDTO, Guid>
    {
        private EngineClass _engineClass;

        #region CompUnitTypeList

        public List<CompUnitTypeDTO> CompUnitTypeList
        {
            get
            {
                return
                    ClientCache.DictionaryRepository.CompUnitTypes.Where(t => t.EngineClassId == _engineClass).ToList();
            }
        }

        #endregion

        #region SelectedCompUnitType

        private BaseDictionaryDTO _selectedCompUnitType;

        public BaseDictionaryDTO SelectedCompUnitType
        {
            get { return _selectedCompUnitType; }
            set
            {
                if (SetProperty(ref _selectedCompUnitType, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        private int _compUnitNum;

        public int CompUnitNum
        {
            get { return _compUnitNum; }
            set
            {
                if (SetProperty(ref _compUnitNum, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #region IsVirtual

        private bool _isVirtual;

        public bool IsVirtual
        {
            get { return _isVirtual; }
            set { SetProperty(ref _isVirtual, value); }
        }

        #endregion

        #region SuperchargerTypeList

        public List<SuperchargerTypeDTO> SuperchargerTypeList
        {
            get { return ClientCache.DictionaryRepository.SuperchargerTypes; }
        }

        #endregion

        #region SelectedSuperchargerType

        private BaseDictionaryDTO _selectedSuperchargerType;

        public BaseDictionaryDTO SelectedSuperchargerType
        {
            get { return _selectedSuperchargerType; }
            set
            {
                if (SetProperty(ref _selectedSuperchargerType, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        private bool _hasRecoveryBoiler;

        public bool HasRecoveryBoiler
        {
            get { return _hasRecoveryBoiler; }
            set { SetProperty(ref _hasRecoveryBoiler, value); }
        }

        public List<CompUnitSealingTypeDTO> SealingTypeList
        {
            get { return ClientCache.DictionaryRepository.CompUnitSealingTypes; }
        }

        private CompUnitSealingTypeDTO _selectedSealingType;

        /// <summary>
        /// Тип уплотнения
        /// </summary>
        public CompUnitSealingTypeDTO SelectedSealingType
        {
            get { return _selectedSealingType; }
            set
            {
                if (SetProperty(ref _selectedSealingType, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }


        private int _sealingCount;

        /// <summary>
        /// Кол-во уплотнений
        /// </summary>
        public int SealingCount
        {
            get { return _sealingCount; }
            set
            {
                if (SetProperty(ref _sealingCount, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private double _startValveConsumption;

        /// <summary>
        /// Расход импульсного газа на работу ЗРА при пуске агрегата, м³
        /// </summary>
        public double StartValveConsumption
        {
            get { return _startValveConsumption; }
            set
            {
                _startValveConsumption = value;
                OnPropertyChanged(() => StartValveConsumption);
            }
        }

        private double _stopValveConsumption;

        /// <summary>
        /// Расход импульсного газа на работу ЗРА при останове агрегата, м³
        /// </summary>
        public double StopValveConsumption
        {
            get { return _stopValveConsumption; }
            set
            {
                _stopValveConsumption = value;
                OnPropertyChanged(() => StopValveConsumption);
            }
        }

        #region Values

        private double _turbineStarterConsumption;

        public double TurbineStarterConsumption
        {
            get { return _turbineStarterConsumption; }
            set { SetProperty(ref _turbineStarterConsumption, value); }
        }

        private double _dryMotoringConsumption;

        public double DryMotoringConsumption
        {
            get { return _dryMotoringConsumption; }
            set { SetProperty(ref _dryMotoringConsumption, value); }
        }

        private double _injectionProfileVolume;

        public double InjectionProfileVolume
        {
            get { return _injectionProfileVolume; }
            set { SetProperty(ref _injectionProfileVolume, value); }
        }

        private double _bleedingRate;

        public double BleedingRate
        {
            get { return _bleedingRate; }
            set { SetProperty(ref _bleedingRate, value); }
        }

        #endregion

        #region SaveCommand

        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }

        #endregion SaveCommand

        public DelegateCommand OpenValveSwitchCalculatorCommand { get; private set; }
        public DelegateCommand OpenPipingVolumeCalculatorCommand { get; private set; }

        private readonly Guid _compShopId;
        private List<ValveSwitches> _valveSwitchList;
        private List<PipeSection> _profilePiping;


        public AddEditCompUnitViewModel(Action<Guid> actionBeforeClosing, CompUnitDTO compUnitDto)
            : base(actionBeforeClosing, compUnitDto)
        {
            _engineClass = compUnitDto.EngineClass;
            Id = Model.Id;
            Name = Model.Name;
            _compShopId = compUnitDto.ParentId.Value;
            CompUnitNum = compUnitDto.CompUnitNum;
            TurbineStarterConsumption = Model.TurbineStarterConsumption;
            DryMotoringConsumption = Model.DryMotoringConsumption;
            InjectionProfileVolume = Model.InjectionProfileVolume;
            BleedingRate = Model.BleedingRate;
            SealingCount = Model.SealingCount;
            SelectedSealingType = Model.SealingType.HasValue
                ? SealingTypeList.Single(s => s.SealingType == Model.SealingType)
                : null;
            IsVirtual = Model.IsVirtual;
            HasRecoveryBoiler = Model.HasRecoveryBoiler;
            SelectedCompUnitType =
                ClientCache.DictionaryRepository.CompUnitTypes.Single(p => p.Id == compUnitDto.CompUnitTypeId);
            SelectedSuperchargerType = SuperchargerTypeList.Single(t => t.Id == Model.SuperchargerTypeId);
            StartValveConsumption = Model.StartValveConsumption;
            StopValveConsumption = Model.StopValveConsumption;

            if (!string.IsNullOrEmpty(Model.ValveConsumptionDetails))
            {
                var xmlSerializer = new XmlSerializer(typeof (List<ValveSwitches>));
                var reader = new StringReader(Model.ValveConsumptionDetails);
                _valveSwitchList = (List<ValveSwitches>) xmlSerializer.Deserialize(reader);
            }

            if (!string.IsNullOrEmpty(Model.InjectionProfilePiping))
            {
                var xmlSerializer = new XmlSerializer(typeof (List<PipeSection>));
                var reader = new StringReader(Model.InjectionProfilePiping);
                _profilePiping = (List<PipeSection>) xmlSerializer.Deserialize(reader);
            }

            SetValidationRules();

            OpenValveSwitchCalculatorCommand = new DelegateCommand(OnOpenValveSwitchCalculator);
            OpenPipingVolumeCalculatorCommand = new DelegateCommand(OnOpenPipingVolumeCalculator);
        }


        public AddEditCompUnitViewModel(Action<Guid> actionBeforeClosing, Guid compShopId, EngineClass engineClass)
            : base(actionBeforeClosing)
        {
            _engineClass = engineClass;
            _compShopId = compShopId;
            _compUnitNum = 1;
            SetValidationRules();

            OpenValveSwitchCalculatorCommand = new DelegateCommand(OnOpenValveSwitchCalculator);
            OpenPipingVolumeCalculatorCommand = new DelegateCommand(OnOpenPipingVolumeCalculator);
        }

        /// <summary>
        /// Открывает калькулятор расхода импульсного газа на работу ЗРА при пуске/останове ГПА
        /// </summary>
        private void OnOpenValveSwitchCalculator()
        {
            var vm = new CompUnitValveSwitchCalculatorViewModel((start, stop, list) =>
            {
                StartValveConsumption = start;
                StopValveConsumption = stop;
                _valveSwitchList = list;
            },
                _valveSwitchList);
            var v = new CompUnitValveSwitchCalculatorView {DataContext = vm};
            v.ShowDialog();
        }

        /// <summary>
        /// Открывает калькулятор геометрического объема газопроводов
        /// </summary>
        private void OnOpenPipingVolumeCalculator()
        {
            var vm = new PipingVolumeCalculatorViewModel(_profilePiping, list =>
            {
                InjectionProfileVolume = list.Sum(p => p.Volume);
                _profilePiping = list;
            });
            var v = new PipingVolumeCalculatorView {DataContext = vm};
            v.ShowDialog();
        }


        protected override string CaptionEntityTypeName
        {
            get { return "ГПА"; }
        }

        private void SetValidationRules()
        {
            AddValidationFor(() => Name)
                .When(() => string.IsNullOrEmpty(Name))
                .Show("Введите наименование");

            AddValidationFor(() => SelectedCompUnitType)
                .When(() => SelectedCompUnitType == null)
                .Show("Не выбран тип ГПА");

            AddValidationFor(() => SelectedSuperchargerType)
                .When(() => SelectedSuperchargerType == null)
                .Show("Не выбран тип нагнетателя");

            AddValidationFor(() => InjectionProfileVolume)
                .When(() => InjectionProfileVolume < 0 || InjectionProfileVolume > 10000)
                .Show("Недопустимое значние (интервал допустимых значений от 0 до 10000)");

            AddValidationFor(() => TurbineStarterConsumption)
                .When(() => TurbineStarterConsumption < 0 || TurbineStarterConsumption > 10000)
                .Show("Недопустимое значние (интервал допустимых значений от 0 до 10000)");

            AddValidationFor(() => DryMotoringConsumption)
                .When(() => DryMotoringConsumption < 0 || DryMotoringConsumption > 10000)
                .Show("Недопустимое значние (интервал допустимых значений от 0 до 10000)");

            AddValidationFor(() => BleedingRate)
                .When(() => BleedingRate < 0 || BleedingRate > 100)
                .Show("Недопустимое значние (интервал допустимых значений от 0 до 100)");

            AddValidationFor(() => SealingCount)
                .When(() => SealingCount < 0 || SealingCount > 10)
                .Show("Недопустимое значние (интервал допустимых значений от 1 до 10)");

            AddValidationFor(() => SelectedSealingType)
                .When(() => SelectedSealingType == null)
                .Show("Не выбран тип уплотнений");
        }


        protected override Task UpdateTask
        {
            get
            {
                var valveSerializer = new XmlSerializer(typeof (List<ValveSwitches>));
                var valveWriter = new StringWriter();
                valveSerializer.Serialize(valveWriter, _valveSwitchList);

                var pipingSerializer = new XmlSerializer(typeof (List<PipeSection>));
                var pipingWriter = new StringWriter();
                pipingSerializer.Serialize(pipingWriter, _profilePiping);

                return new ObjectModelServiceProxy().EditCompUnitAsync(
                    new EditCompUnitParameterSet
                    {
                        Id = Model.Id,
                        Name = Name,
                        ParentId = _compShopId,
                        CompUnitNum = CompUnitNum,
                        CompUnitTypeId = SelectedCompUnitType.Id,
                        SuperchargerTypeId = SelectedSuperchargerType.Id,
                        IsVirtual = IsVirtual,
                        HasRecoveryBoiler = HasRecoveryBoiler,
                        DryMotoringConsumption = DryMotoringConsumption,
                        InjectionProfileVolume = InjectionProfileVolume,
                        TurbineStarterConsumption = TurbineStarterConsumption,
                        BleedingRate = BleedingRate,
                        SealingCount = SealingCount,
                        SealingType = SelectedSealingType.SealingType,
                        StartValveConsumption = StartValveConsumption,
                        StopValveConsumption = StopValveConsumption,
                        ValveConsumptionDetails = valveWriter.ToString(),
                        InjectionProfilePiping = pipingWriter.ToString()
                    });
            }
        }

        protected override Task<Guid> CreateTask
        {
            get
            {
                var valveSerializer = new XmlSerializer(typeof (List<ValveSwitches>));
                var valveWriter = new StringWriter();
                valveSerializer.Serialize(valveWriter, _valveSwitchList);

                var pipingSerializer = new XmlSerializer(typeof (List<PipeSection>));
                var pipingWriter = new StringWriter();
                pipingSerializer.Serialize(pipingWriter, _profilePiping);

                return new ObjectModelServiceProxy().AddCompUnitAsync(
                    new AddCompUnitParameterSet
                    {
                        Name = Name,
                        ParentId = _compShopId,
                        CompUnitNum = CompUnitNum,
                        CompUnitTypeId = SelectedCompUnitType.Id,
                        SuperchargerTypeId = SelectedSuperchargerType.Id,
                        IsVirtual = IsVirtual,
                        HasRecoveryBoiler = HasRecoveryBoiler,
                        DryMotoringConsumption = DryMotoringConsumption,
                        InjectionProfileVolume = InjectionProfileVolume,
                        TurbineStarterConsumption = TurbineStarterConsumption,
                        BleedingRate = BleedingRate,
                        SealingCount = SealingCount,
                        SealingType = SelectedSealingType.SealingType,
                        StartValveConsumption = StartValveConsumption,
                        StopValveConsumption = StopValveConsumption,
                        ValveConsumptionDetails = valveWriter.ToString(),
                        InjectionProfilePiping = pipingWriter.ToString()
                    });
            }
        }
    }
}