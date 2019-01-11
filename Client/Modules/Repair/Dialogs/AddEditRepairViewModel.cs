using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.Repairs;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PlanTypes;
using GazRouter.DTO.Dictionaries.RepairExecutionMeans;
using GazRouter.DTO.Dictionaries.RepairTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.DTO.Repairs.RepairWorks;
using GazRouter.DTO.SeriesData.PropertyValues;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Repair.Dialogs
{
    public class AddEditRepairViewModel : AddEditViewModelBase<RepairPlanBaseDTO, int>
    {
        private readonly int _year;
        private const int MaxCorrectTransfer = 2000;

        public AddEditRepairViewModel(Action<int> actionBeforeClosing, RepairPlanBaseDTO repair, int year)
            : base(actionBeforeClosing, repair)
        {
            _year = year;

            _planType = PlanType.Planned;
            _bleedAmount = repair.BleedAmount;
            _calculatedTransfer = repair.CalculatedTransfer;
            _capacitySummer = repair.CapacitySummer;
            _capacityTransition = repair.CapacityTransition;
            _capacityWinter = repair.CapacityWinter;
            _description = repair.Description;
            _descriptionGtp = repair.DescriptionGtp;
            _endDate = repair.EndDate;
            Id = repair.Id;
            _maxTransferSummer = repair.MaxTransferSummer;
            _maxTransferTransition = repair.MaxTransferTransition;
            _maxTransferWinter = repair.MaxTransferWinter;
            _partsDeliveryDate = repair.PartsDeliveryDate;
            _savingAmount = repair.SavingAmount;
            _startDate = repair.StartDate;
            _isTransferRelation = repair.IsCritical;
            
            _selectedEntity = new CommonEntityDTO
            {
                Id = repair.EntityId,
                EntityType = repair.EntityType,
                Name = repair.EntityName,
                ShortPath = repair.EntityName
            };
            _selectedRepairType = RepairTypeList.Single(rt => rt.Id == repair.RepairTypeId);
            _selectedExecutionMeans = ExecutionMeansList.Single(em => em.ExecutionMeans == repair.ExecutionMeans);

            LoadWorks();

            SetValidationRules();
            ValidateAll();
        }

        public AddEditRepairViewModel(Action<int> actionBeforeClosing, int year)
            : base(actionBeforeClosing)
        {
            _year = year;

            _startDate = new DateTime(year, 1, 1);
            _endDate = _startDate.AddDays(1);
            _partsDeliveryDate = new DateTime(year, 1, 1);
            _planType = PlanType.Planned;
            
            SetValidationRules();
        }

        private readonly PlanType? _planType;

        protected override string CaptionEntityTypeName => "ремонта";

        public List<EntityType> AllowedType => new List<EntityType>
        {
            EntityType.Pipeline,
            EntityType.DistrStation,
            EntityType.CompShop
        };

        #region SelectedEntity

        private CommonEntityDTO _selectedEntity;

        /// <summary>
        /// Выбранный объект
        /// </summary>
        public CommonEntityDTO SelectedEntity
        {
            get { return _selectedEntity; }
            set
            {
                if (SetProperty(ref _selectedEntity, value))
                {
                    SelectedRepairType = null;
                    OnPropertyChanged(() => IsEntitySelected);
                    OnPropertyChanged(() => IsPipelineSelected);
                    OnPropertyChanged(() => IsDistrStationSelected);
                    OnPropertyChanged(() => RepairTypeList);
                    OnPropertyChanged(() => RepairWorkList);

                    // Подгрузить список километров
                    if (IsPipelineSelected)
                        LoadKilometerList();

                    if (IsDistrStationSelected)
                    {
                        MaxTransferSummer = 0;
                        MaxTransferWinter = 0;
                        MaxTransferTransition = 0;

                        CapacitySummer = 0;
                        CapacityWinter = 0;
                        CapacityTransition = 0;

                        CalculatedTransfer = 0;
                    }
                }
            }
        }

        public List<KilometerItem> KilometerList { get; set; }

        private async void LoadKilometerList()
        {
            KilometerList = new List<KilometerItem>();
            
            try
            {
                Behavior.TryLock();
                var pipeline = await new ObjectModelServiceProxy().GetPipelineByIdAsync(_selectedEntity.Id);
                var valveList = await new ObjectModelServiceProxy().GetValveListAsync(
                    new GetValveListParameterSet { PipelineId = _selectedEntity.Id });

                KilometerList.Add(new PipeEdgeKilometer(pipeline.KilometerOfStartPoint));
                KilometerList.AddRange(valveList.Select(v => new ValveKilometer(v)));
                KilometerList.Add(new PipeEdgeKilometer(pipeline.KilometerOfEndPoint));
            }
            finally 
            {
                Behavior.TryUnlock();
            }
                


           
        }

        /// <summary>
        /// Выбран ли объект
        /// </summary>
        public bool IsEntitySelected => _selectedEntity != null;

        /// <summary>
        /// Выбранный объект - газопровод
        /// </summary>
        public bool IsPipelineSelected => _selectedEntity != null && _selectedEntity.EntityType == EntityType.Pipeline;

        /// <summary>
        /// Выбранный объект - ГРС
        /// </summary>
        public bool IsDistrStationSelected
            => _selectedEntity != null && _selectedEntity.EntityType == EntityType.DistrStation;

        #endregion

        #region RepairType

        /// <summary>
        /// Список видов ремонтных работ
        /// </summary>
        public List<RepairTypeDTO> RepairTypeList
        {
            get
            {
                if (SelectedEntity == null)
                {
                    return new List<RepairTypeDTO>();
                }
                return
                    ClientCache.DictionaryRepository.RepairTypes.Where(rt => rt.EntityType == SelectedEntity.EntityType)
                        .ToList();
            }
        }

        private RepairTypeDTO _selectedRepairType;

        /// <summary>
        /// Выбранный вид ремонтных работ
        /// </summary>
        public RepairTypeDTO SelectedRepairType
        {
            get { return _selectedRepairType; }
            set
            {
                if (SetProperty(ref _selectedRepairType, value))
                {
                    LoadWorks();
                    //SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region ExecutionMeans

        /// <summary>
        /// Список способов ведения работ
        /// </summary>
        public List<RepairExecutionMeansDTO> ExecutionMeansList => ClientCache.DictionaryRepository.RepairExecutionMeans;

        private RepairExecutionMeansDTO _selectedExecutionMeans;

        /// <summary>
        /// Выбранный способ ведения работ
        /// </summary>
        public RepairExecutionMeansDTO SelectedExecutionMeans
        {
            get { return _selectedExecutionMeans; }
            set
            {
                if(SetProperty(ref _selectedExecutionMeans, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region RepairWorks

        public RepairWorkList RepairWorkList { get; set; }


        private void LoadWorks()
        {
            RepairWorkList = new RepairWorkList {WorksChanged = SaveCommand.RaiseCanExecuteChanged};
            if (SelectedEntity != null && SelectedRepairType != null)
            {
                foreach (var wt in SelectedRepairType.RepairWorkTypes)
                {
                    var work = Model.Works.SingleOrDefault(w => w.WorkTypeId == wt.Id);
                    RepairWorkList.AddRepairWork(KilometerList, wt, work != null, work?.KilometerStart, work?.KilometerEnd);
                }
            }
            OnPropertyChanged(() => RepairWorkList);
        }

        private readonly List<string> _workListWarningMessages = new List<string>();

        public string WorkListWarningMessage
        {
            get
            {
                var sb = new StringBuilder();
                _workListWarningMessages.ForEach(m => sb.AppendLine( m));
                return sb.ToString();
            }
        }

        public bool IsWorkListWarningMessageVisible => _workListWarningMessages.Count > 0;

        private bool CheckWorks()
        {
            //_workListWarningMessages.Clear();

            //if (SelectedEntity == null || RepairWorkList == null)
            //{
            //    return false;
            //}

            //if (!RepairWorkList.Any(w => w.IsSelected))
            //{
            //    _workListWarningMessages.Add("● Не выбран ни один статус.");
            //}

            //var dicRepairWork = RepairWorkList.ToDictionary(c => c.WorkTypeId, v => v);
            //switch (SelectedEntity.EntityType)
            //{
            //    case EntityType.DistrStation:
            //        if (dicRepairWork[WorkType.DistrStationOvergazing].IsSelected && !dicRepairWork[WorkType.DistrStationDeactivating].IsSelected)
            //        {
            //            _workListWarningMessages.Add(
            //                "● Установлен статус \"стравливается\". При этом статус \"отключается\" не установлен.");
            //        }

            //        if (dicRepairWork[WorkType.DistrStationDeactivating].IsSelected && dicRepairWork[WorkType.DistrStationWorkViaTemporaryReductionUnit].IsSelected)
            //        {
            //            _workListWarningMessages.Add(
            //                "● Установлены взаимоисключающие статусы \"отключается\" и \"работа через временный узел редуцирования\".");
            //        }
            //        break;

            //    case EntityType.CompShop:
            //        if (RepairWorkList.Any(w => w.WorkTypeId == 212 && w.IsSelected)
            //            && RepairWorkList.Any(w => w.WorkTypeId == 211 && !w.IsSelected))
            //        {
            //            _workListWarningMessages.Add(
            //                "● Установлен статус \"стравливается контур цеха\". При этом статус \"отключается\" не установлен.");
            //        }

            //        if (RepairWorkList.Any(w => w.WorkTypeId == 214 && w.IsSelected)
            //            && RepairWorkList.Any(w => w.WorkTypeId == 213 && !w.IsSelected))
            //        {
            //            _workListWarningMessages.Add(
            //                "● Установлен статус \"стравливается охранная зона КЦ (кр.№19 - кр.№21)\". При этом статус \"отключается охранная зона КЦ (кр.№19 - кр.№21)\" не установлен.");
            //        }
            //        break;

            //    case EntityType.Pipeline:
            //        if (dicRepairWork[WorkType.PipelineDeactivating].IsSelected
            //            && dicRepairWork[WorkType.PipelineWithoutDeactivating].IsSelected )
            //        {
            //            _workListWarningMessages.Add(
            //                "● Установлены взаимоисключающие статусы \"отключается\" и \"без отключения\".");
            //        }

            //        if (dicRepairWork[WorkType.PipelineOvergazing].IsSelected
            //            && !dicRepairWork[WorkType.PipelineDeactivating].IsSelected)
            //        {
            //            _workListWarningMessages.Add(
            //                "● Установлен статус \"стравливается\". При этом статус \"отключается\" не установлен.");
            //        }

            //        if (RepairWorkList.Any(w => w.WorkTypeId == 345 && w.IsSelected)
            //            && RepairWorkList.Any(w => w.WorkTypeId == 346 && w.IsSelected))
            //        {
            //            _workListWarningMessages.Add(
            //                "● Установлены взаимоисключающие статусы \"частично выводится из гидравлики\" и \"выводится из гидравлики\".");
            //        }

            //        if (dicRepairWork[WorkType.PipelineDeactivating].IsSelected
            //            && RepairWorkList.Any(w => w.WorkTypeId == 345 && w.IsSelected))
            //        {
            //            _workListWarningMessages.Add(
            //                "● Установлены взаимоисключающие статусы \"отключается\" и \"частично выводится из гидравлики\".");
            //        }

            //        if (dicRepairWork[WorkType.PipelineDeactivating].IsSelected
            //            && RepairWorkList.Any(w => w.WorkTypeId == 346 && w.IsSelected))
            //        {
            //            _workListWarningMessages.Add(
            //                "● Установлены взаимоисключающие статусы \"отключается\" и \"выводится из гидравлики\".");
            //        }

            //        if (dicRepairWork[WorkType.PipelineDeactivating].IsSelected
            //            && RepairWorkList.Any(w => w.WorkTypeId == 347 && w.IsSelected))
            //        {
            //            _workListWarningMessages.Add(
            //                "● Установлены взаимоисключающие статусы \"отключается\" и \"снижение РРД на 30%\".");
            //        }

            //        if (dicRepairWork[WorkType.PipelineDeactivating].IsSelected
            //            && dicRepairWork[WorkType.PipelineOvergazing].IsSelected)
            //        {
            //            if (dicRepairWork[WorkType.PipelineDeactivating].KilometerStart !=
            //                dicRepairWork[WorkType.PipelineOvergazing].KilometerStart
            //                ||
            //                dicRepairWork[WorkType.PipelineDeactivating].KilometerEnd !=
            //                dicRepairWork[WorkType.PipelineOvergazing].KilometerEnd)
            //            {
            //                _workListWarningMessages.Add(
            //                    "● Для статусов \"отключается\" и \"стравливается\" выбранные километры начала и конца должны совпадать.");
            //            }
            //        }

            //        if (dicRepairWork[WorkType.PipelineDeactivating].IsSelected
            //            && dicRepairWork[WorkType.PipelineRepairing].IsSelected )
            //        {
            //            if (dicRepairWork[WorkType.PipelineDeactivating].KilometerStart >
            //               dicRepairWork[WorkType.PipelineRepairing].KilometerStart
            //                ||
            //                dicRepairWork[WorkType.PipelineDeactivating].KilometerEnd <
            //                dicRepairWork[WorkType.PipelineRepairing].KilometerEnd)
            //            {
            //                _workListWarningMessages.Add(
            //                    "● Участок со статусом \"ремонтируется\" должен находиться внутри участка со статусом \"отключается\".");
            //            }
            //        }

            //        if (
            //            RepairWorkList.Where(w => w.IsSelected)
            //                .Any(repairWork => RepairWorkList.Where(w => w.IsSelected)
            //                    .Any(w =>
            //                        w.KilometerEnd < repairWork.KilometerStart
            //                        || w.KilometerStart > repairWork.KilometerEnd)))
            //        {
            //            _workListWarningMessages.Add("● В списке несколько непересекающихся участков");
            //        }

            //        break;
            //}

            //OnPropertyChanged(() => WorkListWarningMessage);
            //OnPropertyChanged(() => IsWorkListWarningMessageVisible);
            //return !_workListWarningMessages.Any();

            return true;
        }

        #endregion

        
        
        #region Dates
        
        private DateTime _startDate;
        /// <summary>
        /// Плановая дата начала проведения ремонтных работ
        /// </summary>
        public DateTime StartDate
        {
            get { return _startDate; }
            set
            {
                if(SetProperty(ref _startDate, value.Date))
                {
                    if (_startDate > _endDate)
                    {
                        _endDate = _startDate.AddDays(1);
                        OnPropertyChanged(() => EndDate);
                    }
                    OnPropertyChanged(() => Duration);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        private DateTime _endDate;
        /// <summary>
        /// Плановая дата завершения ремонтных работ
        /// </summary>
        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                if (SetProperty(ref _endDate, value.Date))
                {
                    if (_endDate < _startDate)
                    {
                        _startDate = _endDate.AddDays(-1);
                        OnPropertyChanged(() => StartDate);
                    }
                    OnPropertyChanged(() => Duration);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        /// <summary>
        /// Продолжительность работ
        /// </summary>
        public TimeSpan Duration => EndDate - StartDate;



        /// <summary>
        /// Начало допустимого диапазона выбора дат
        /// </summary>
        public DateTime AllowedDateRangeStart => new DateTime(_year, 1, 1);
        
        /// <summary>
        /// Конец допустимого диапазона выбора дат
        /// </summary>
        public DateTime AllowedDateRangeEnd => new DateTime(_year, 12, 31);



        private DateTime _partsDeliveryDate;
        /// <summary>
        /// Дата поставки МТР
        /// </summary>
        public DateTime PartsDeliveryDate
        {
            get { return _partsDeliveryDate; }
            set
            {
                if (SetProperty(ref _partsDeliveryDate, value.Date))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        private void SetValidationRules()
        {
            AddValidationFor(() => SelectedEntity)
                .When(() => SelectedEntity == null)
                .Show("Объект не выбран");

            AddValidationFor(() => SelectedRepairType)
                .When(() => SelectedEntity != null && SelectedRepairType == null)
                .Show("Не указан вид ремонтных работ");

            AddValidationFor(() => SelectedExecutionMeans)
                .When(() => SelectedEntity != null && SelectedExecutionMeans == null)
                .Show("Не указан способ ведения работ");

            AddValidationFor(() => StartDate)
                .When(() => SelectedEntity != null && StartDate.Date < PartsDeliveryDate.Date)
                .Show("Дата начала работ не может быть меньше даты поставки МТР");

            AddValidationFor(() => StartDate)
                .When(
                    () =>
                        SelectedEntity != null && Model.Complex != null && !Model.Complex.IsLocal &&
                        StartDate < Model.Complex.StartDate)
                .Show(
                    $"Работа включена в корпоративный комплекс \"{Model.Complex?.ComplexName}\". Дата начала работ меньше даты начала комплекса {Model.Complex?.StartDate:dd.MM}");

            AddValidationFor(() => EndDate)
                .When(
                    () =>
                        SelectedEntity != null && Model.Complex != null && !Model.Complex.IsLocal &&
                        EndDate > Model.Complex.EndDate)
                .Show(
                    $"Работа включена в корпоративный комплекс \"{Model.Complex?.ComplexName}\". Дата окончания работ больше даты окончания комплекса ({Model.Complex?.EndDate:dd.MM})");

            AddValidationFor(() => Description)
                .When(() => SelectedEntity != null && string.IsNullOrEmpty(Description))
                .Show("Введите текстовое описание работ");

            AddValidationFor(() => BleedAmount)
                .When(() => BleedAmount < 0 || BleedAmount > 5)
                .Show("Некорректное значение");

            
            AddValidationFor(() => BleedAmount)
                .When(() => BleedAmount == 0 && RepairWorkList!= null && RepairWorkList.HasBleedWork)
                .Show(
                    "На вкладке состояние выбраны работы по стравливанию газа, при этом значение объема стравливаемого газа не введено.");

            AddValidationFor(() => BleedAmount)
                .When(() => BleedAmount > 0 && RepairWorkList != null && !RepairWorkList.HasBleedWork)
                .Show(
                    "Введено значение объема стравливаемого газа, при этом на вкладке состояние не выбраны работы по стравливанию газа.");

            AddValidationFor(() => SavingAmount)
                .When(() => BleedAmount == 0 && SavingAmount > 0)
                .Show("Указано значение вырабатываемого газа, при этом значение стравливаемого газа отсутсвует");

            AddValidationFor(() => SavingAmount)
                .When(() => SavingAmount < 0 || SavingAmount > 5)
                .Show("Некорректное значение");

            AddValidationFor(() => MaxTransferWinter)
                .When(() => MaxTransferWinter < 0 || MaxTransferWinter > MaxCorrectTransfer)
                .Show("Некорректное значение");

            AddValidationFor(() => MaxTransferSummer)
                .When(() => MaxTransferSummer < 0 || MaxTransferSummer > MaxCorrectTransfer)
                .Show("Некорректное значение");

            AddValidationFor(() => MaxTransferTransition)
                .When(() => MaxTransferTransition < 0 || MaxTransferTransition > MaxCorrectTransfer)
                .Show("Некорректное значение");

            AddValidationFor(() => CapacityWinter)
                .When(() => CapacityWinter < 0 || CapacityWinter > MaxCorrectTransfer)
                .Show("Некорректное значение");

            AddValidationFor(() => CapacitySummer)
                .When(() => CapacitySummer < 0 || CapacitySummer > MaxCorrectTransfer)
                .Show("Некорректное значение");

            AddValidationFor(() => CapacityTransition)
                .When(() => CapacityTransition < 0 || CapacityTransition > MaxCorrectTransfer)
                .Show("Некорректное значение");

            AddValidationFor(() => CalculatedTransfer)
                .When(() => CalculatedTransfer < 0 || CapacityTransition > MaxCorrectTransfer)
                .Show("Некорректное значение");
        }


        private string _description;
        /// <summary>
        /// Описание работ
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                if (SetProperty(ref _description, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _descriptionGtp;
        /// <summary>
        /// Комментарий ГТП
        /// </summary>
        public string DescriptionGtp
        {
            get { return _descriptionGtp; }
            set
            {
                if (SetProperty(ref _descriptionGtp, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        #region Regime

        private double _maxTransferWinter;
        /// <summary>
        /// Достигнутый объем транспорта газа на участке, млн.м3/сут (Зима)
        /// </summary>
        public double MaxTransferWinter
        {
            get { return _maxTransferWinter; }
            set
            {
                if(SetProperty(ref _maxTransferWinter, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private double _maxTransferSummer;
        /// <summary>
        /// Достигнутый объем транспорта газа на участке, млн.м3/сут (Лето)
        /// </summary>
        public double MaxTransferSummer
        {
            get { return _maxTransferSummer; }
            set
            {
                if (SetProperty(ref _maxTransferSummer, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        private double _maxTransferTransition;
        /// <summary>
        /// Достигнутый объем транспорта газа на участке, млн.м3/сут (Межсезонье)
        /// </summary>
        public double MaxTransferTransition
        {
            get { return _maxTransferTransition; }
            set
            {
                if (SetProperty(ref _maxTransferTransition, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private double _capacityWinter;
        /// <summary>
        /// Расчетная пропускная способность участка, млн.м3/сут (Зима)
        /// </summary>
        public double CapacityWinter
        {
            get { return _capacityWinter; }
            set
            {
                if (SetProperty(ref _capacityWinter, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private double _capacitySummer;
        /// <summary>
        /// Расчетная пропускная способность участка, млн.м3/сут (Лето)
        /// </summary>
        public double CapacitySummer
        {
            get { return _capacitySummer; }
            set
            {
                if (SetProperty(ref _capacitySummer, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        private double _capacityTransition;
        /// <summary>
        /// Расчетная пропускная способность участка, млн.м3/сут (Межсезонье)
        /// </summary>
        public double CapacityTransition
        {
            get { return _capacityTransition; }
            set
            {
                if (SetProperty(ref _capacityTransition, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        private double _calculatedTransfer;
        /// <summary>
        /// Расчетный объем транспорта газа на период проведения работ, млн.м3/сут.
        /// </summary>
        public double CalculatedTransfer
        {
            get { return _calculatedTransfer; }
            set
            {
                if (SetProperty(ref _calculatedTransfer, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        private double _bleedAmount;
        /// <summary>
        /// Объем стравливаемого газа, млн.м3
        /// </summary>
        public double BleedAmount
        {
            get { return _bleedAmount; }
            set
            {
                if (SetProperty(ref _bleedAmount, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        private double _savingAmount;
        /// <summary>
        /// Объем выработки газа, млн.м3
        /// </summary>
        public double SavingAmount
        {
            get { return _savingAmount; }
            set
            {
                if (SetProperty(ref _savingAmount, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion


        private bool _isTransferRelation;
        /// <summary>
        /// Влияет на транспорт газа
        /// </summary>
        public bool IsTransferRelation
        {
            get { return _isTransferRelation; }
            set
            {
                if (SetProperty(ref _isTransferRelation, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #region Save

        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return CheckWorks() && !HasErrors;
        }

        protected override Task UpdateTask
        {
            get
            {
                var paramSet = new EditRepairParameterSet
                {
                    BleedAmount = BleedAmount,
                    SavingAmount = SavingAmount,
                    CapacityWinter = CapacityWinter,
                    CapacitySummer = CapacitySummer,
                    CapacityTransition = CapacityTransition,
                    MaxTransferWinter = MaxTransferWinter,
                    MaxTransferSummer = MaxTransferSummer,
                    MaxTransferTransition = MaxTransferTransition,
                    CalculatedTransfer = CalculatedTransfer,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    Description = Description,
                    DescriptionGtp = DescriptionGtp,
                    EntityId = SelectedEntity.Id,
                    Id = Model.Id,
                    IsCritical = IsTransferRelation,
                    PlanType = _planType,
                    RepairType = SelectedRepairType.Id,
                    ExecutionMeans = SelectedExecutionMeans.ExecutionMeans,
                    IsExternalCondition = false,
                    PartsDeliveryDate = PartsDeliveryDate,
                    RepairWorks = RepairWorkList
                        .Where(w => w.IsSelected)
                        .Select(w =>
                            new RepairWorkParameterSet
                            {
                                WorkTypeId = w.Dto.Id,
                                KilometerStart = w.KilometerStart,
                                KilometerEnd = w.KilometerEnd
                            }).ToList()
                };
                return new RepairsServiceProxy().EditRepairAsync(paramSet);
            }
        }

        protected override Task<int> CreateTask
        {
            get
            {
                var paramSet = new AddRepairParameterSet
                {
                    BleedAmount = BleedAmount,
                    SavingAmount = SavingAmount,
                    CapacityWinter = CapacityWinter,
                    CapacitySummer = CapacitySummer,
                    CapacityTransition = CapacityTransition,
                    MaxTransferWinter = MaxTransferWinter,
                    MaxTransferSummer = MaxTransferSummer,
                    MaxTransferTransition = MaxTransferTransition,
                    CalculatedTransfer = CalculatedTransfer,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    Description = Description,
                    DescriptionGtp = DescriptionGtp,
                    EntityId = SelectedEntity.Id,
                    IsCritical = IsTransferRelation,
                    PlanType = _planType,
                    RepairType = SelectedRepairType.Id,
                    ExecutionMeans = SelectedExecutionMeans.ExecutionMeans,
                    IsExternalCondition = false,
                    PartsDeliveryDate = PartsDeliveryDate,
                    RepairWorks = RepairWorkList
                        .Where(w => w.IsSelected)
                        .Select(w =>
                            new RepairWorkParameterSet
                            {
                                WorkTypeId = w.Dto.Id,
                                KilometerStart = w.KilometerStart,
                                KilometerEnd = w.KilometerEnd
                            }).ToList()
                };
                return new RepairsServiceProxy().AddRepairAsync(paramSet);
            }
        }

        #endregion
    }
}