using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
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
using GazRouter.Repair.Attachment;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Controls;
using GazRouter.DTO.Repairs.Workflow;
using GazRouter.Application;
using GazRouter.Repair.Agreement;
using GazRouter.Repair.PrintForms;
using GazRouter.Repair.RepWorks;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.FormatProviders.OpenXml.Docx;
using Telerik.Windows.Documents.FormatProviders.Rtf;

namespace GazRouter.Repair.ReqWorks.Dialogs
{
    public class AddEditRepairViewModel : AddEditViewModelBase<RepairPlanBaseDTO, int>
    {
        private readonly int _year;
        private const int MaxCorrectTransfer = 2000;
        RepairPlanBaseDTO _repair;

        EditRepairParameterSet _updateSet = null;

        private DateTime? _dateStartFact;
        public DateTime? StartDateFact
        {
            get { return _dateStartFact; }
            set
            {
                SetProperty(ref _dateStartFact, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        private DateTime? _dateEndFact;
        public DateTime? EndDateFact
        {
            get { return _dateEndFact; }
            set
            {
                SetProperty(ref _dateEndFact, value);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private int _planTypeCode = 0;

        public string Status
        {
            get
            {
                if (_repair != null && _repair.WFWState != null)
                    return string.Format("Статус согласования: {0}.  Статус работ: {1}.", WorkStateDTO.GetState(_repair.WFWState.WFState), WorkStateDTO.GetState(_repair.WFWState.WState));
                else return "";
            }
        }



        public AddEditRepairViewModel(Action<int> actionBeforeClosing, RepairPlanBaseDTO repair, int year, bool showFiles = false)
            : base(actionBeforeClosing, repair)
        {
            LoadDictionaries();

            _year = year;

            _repair = repair;


            //_planType = PlanType.Planned;
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

            if (ChangesAllowed)
            {
                _startDateSched = repair.DateStartSched.HasValue ? repair.DateStartSched.Value : repair.StartDate.Date.AddHours(8);//repair.DateStartSched.Value == new DateTime(1, 1, 1) ? repair.StartDate : repair.DateStartSched.Value : repair.StartDate;
                _endDateSched = repair.DateEndSched.HasValue ? repair.DateEndSched.Value : repair.EndDate.Date.AddHours(18);//repair.DateEndSched.Value == new DateTime(1, 1, 1) ? repair.EndDate : repair.DateEndSched.Value : repair.EndDate;
            }
            else
            {
                _startDateSched = repair.DateStartSched.Value;
                _endDateSched = repair.DateEndSched.Value;
            }

            WorkflowHistory = new WorkflowHistoryViewModel(Id);

            SelectedFireType = new FireworksDTO() { Firetype = repair.Firework };





            StartDateFact = repair.DateStartFact;
            EndDateFact = repair.DateEndFact;

            SelectedEntity = new CommonEntityDTO
            {
                Id = repair.EntityId,
                EntityType = repair.EntityType,
                Name = repair.EntityName,
                ShortPath = repair.EntityName
            };
            _selectedRepairType = RepairTypeList.Single(rt => rt.Id == repair.RepairTypeId);
            _selectedExecutionMeans = ExecutionMeansList.Single(em => em.ExecutionMeans == repair.ExecutionMeans);

            ResolutionDate = repair.ResolutionDate;
            ResolutionDateCpdd = repair.ResolutionDateCpdd;
            ResolutionNum = repair.ResolutionNum;
            ResolutionNumCpdd = repair.ResolutionNumCpdd;

            GazpromPlanID = repair.GazpromPlanID;
            ConsumersState = repair.ConsumersState;
            GazpromPlanDate = repair.GazpromPlanDate;

            Duration = repair.Duration.HasValue && repair.Duration.Value != 0 ? repair.Duration.Value : Converters.DurationFromDates.TotalHours(StartDate, EndDate);

            LoadWorks();

            SetStatusCommand = new DelegateCommand<WorkStateDTO>(SetStatus);

            //SelectedFireType = new FireworksDTO() { Firetype = repair.Firework };


            //ProcessCommand = new DelegateCommand(ProcessWorks, () => SelectedRepair != null && IsChangesAllowed && IsEditPermission && false);

            SetValidationRules();
            ValidateAll();

            _updateSet = updateSet;

            CurrentState = repair.WFWState;

            ChangesAllowed = true;
            //if (CurrentState.WFState == WorkStateDTO.WorkflowStates.Draft)
            //{
            //    ChangesAllowed = true;
            //}
            //else
            //{
            //    ChangesAllowed = false;
            //}

            if (UserProfile.Current.Site.IsEnterprise)
            {
                IsPds = true;
            }


            //if (UserProfile.Current.Site.IsEnterprise)
            //{
            //    IsPds = true;

            //    if (CurrentState.WState < WorkStateDTO.WorkStates.Current)
            //    {
            //        ChangesAllowed = true;
            //    }
            //    else
            //    {
            //        ChangesAllowed = false;
            //    }
            //}

            //if (!UserProfile.Current.Site.IsEnterprise && (CurrentState.WFState != WorkStateDTO.WorkflowStates.Undefined || CurrentState.WFState != WorkStateDTO.WorkflowStates.Draft))
            //{
            //    ChangesAllowed = false;
            //    IsSetStatusAllowed = false;

            //    AgreementList.isEditingAllowed = false;
            //}

            //BuildStatusMenu();

            _DocCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(PrintDoc);
            _AgreeFaxCommand = new Microsoft.Practices.Prism.Commands.DelegateCommand(PrintFax);
        }

        public async void SetEntityId(Guid eId)
        {
            var entityDto = await new ObjectModelServiceProxy().GetEntityByIdAsync(eId);

            SelectedEntity = entityDto;//.Result;
        }
        public AddEditRepairViewModel(Action<int> actionBeforeClosing, Guid eId, int year)
            : base(actionBeforeClosing)
        {
            LoadDictionaries();

            _year = year;
            SetEntityId(eId);

            _startDate = new DateTime(year, 1, 1);
            _endDate = _startDate.AddDays(1);
            _partsDeliveryDate = new DateTime(year, 1, 1);

            _planType = PlanType.Unplanned;

            SetValidationRules();
        }


        public AddEditRepairViewModel(Action<int> actionBeforeClosing, int year)
            : base(actionBeforeClosing)
        {
            LoadDictionaries();

            _year = year;

            _startDate = new DateTime(year, 1, 1);
            _endDate = _startDate.AddDays(1);
            _partsDeliveryDate = new DateTime(year, 1, 1);

            _planType = PlanType.Unplanned;



            SetValidationRules();
        }

        private string _gazpromPlanID = "";
        public string GazpromPlanID
        {
            get { return _gazpromPlanID; }
            set
            {
                if (SetProperty(ref _gazpromPlanID, value))
                {
                    OnPropertyChanged(() => GazpromPlanID);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private DateTime? _gazpromPlanDate;
        public DateTime? GazpromPlanDate
        {
            get { return _gazpromPlanDate; }
            set
            {
                if (SetProperty(ref _gazpromPlanDate, value))
                {
                    OnPropertyChanged(() => GazpromPlanDate);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _consumersState = "";
        public string ConsumersState
        {
            get { return _consumersState; }
            set
            {
                if (SetProperty(ref _consumersState, value))
                {
                    OnPropertyChanged(() => ConsumersState);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private DateTime? _resolutionDate;
        public DateTime? ResolutionDate
        {
            get { return _resolutionDate; }
            set
            {
                if (SetProperty(ref _resolutionDate, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private DateTime? _resolutionDateCpdd;
        public DateTime? ResolutionDateCpdd
        {
            get { return _resolutionDateCpdd; }
            set
            {
                if (SetProperty(ref _resolutionDateCpdd, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _resolutionNum;
        public string ResolutionNum
        {
            get { return _resolutionNum; }
            set
            {
                if (SetProperty(ref _resolutionNum, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private string _resolutionNumCpdd;
        public string ResolutionNumCpdd
        {
            get { return _resolutionNumCpdd; }
            set
            {
                if (SetProperty(ref _resolutionNumCpdd, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }



        private PlanType? _planType;

        private PlanTypeDTO _selectedPlanType;
        public PlanTypeDTO SelectedPlanType
        {
            get { return _selectedPlanType; }
            set
            {
                if (SetProperty(ref _selectedPlanType, value))
                {
                    switch (_selectedPlanType.Id)
                    {
                        case 1: _planType = PlanType.Planned; break;
                        case 2: _planType = PlanType.Unplanned; break;
                        case 3: _planType = PlanType.Emergency; break;
                        default: _planType = PlanType.Unplanned; break;
                    }
                }
            }
        }

        private async void LoadDictionaries()
        {
            var t = await new RepairsServiceProxy().GetRepairPlanTypesAsync();
            PlanTypes = t;
        }

        private List<PlanTypeDTO> _planTypes;
        public List<PlanTypeDTO> PlanTypes
        {
            get { return _planTypes; }
            set
            {
                if (SetProperty(ref _planTypes, value))
                {
                    if (_repair != null)
                    {
                        SelectedPlanType = PlanTypes.First(o => o.Id == _repair.PlanType);
                    }
                }
            }
        }

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
                    SaveCommand.RaiseCanExecuteChanged();
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
                if (SetProperty(ref _selectedExecutionMeans, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region RepairWorks

        public RepairWorkList RepairWorkList { get; set; }


        private void LoadWorks()
        {
            RepairWorkList = new RepairWorkList { WorksChanged = SaveCommand.RaiseCanExecuteChanged };
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
                _workListWarningMessages.ForEach(m => sb.AppendLine(m));
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
                if (SetProperty(ref _startDate, value.Date))
                {
                    if (_startDate > _endDate)
                    {
                        _endDate = _startDate.AddDays(1);
                        OnPropertyChanged(() => EndDate);
                    }
                    OnPropertyChanged(() => Duration);
                    RecalcDuration();
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
                    RecalcDuration();
                    OnPropertyChanged(() => Duration);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        /// <summary>
        /// Продолжительность работ
        /// </summary>
        //ublic TimeSpan Duration => new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 18, 0, 0) - new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, 8, 0, 0);
        private void RecalcDuration()
        {
            try
            {
                Duration = Converters.DurationFromDates.TotalHours(StartDate, EndDate);
            }
            catch { }
        }

        private int _duration;
        /// <summary>
        /// Продолжительность работ
        /// </summary>
        public int Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                int total = Converters.DurationFromDates.TotalHours(StartDate, EndDate) + 24;
                if (total < value)
                    value = total;
                if (SetProperty(ref _duration, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private DateTime _startDateSched;
        /// <summary>
        /// Плановая дата начала проведения ремонтных работ
        /// </summary>
        public DateTime StartDateSched
        {
            get { return _startDateSched; }
            set
            {
                if (SetProperty(ref _startDateSched, value))
                {
                    if (_startDateSched > _endDateSched)
                    {
                        _endDateSched = _startDateSched.AddDays(1);
                        OnPropertyChanged(() => EndDateSched);
                    }
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        private DateTime _endDateSched;
        /// <summary>
        /// Плановая дата завершения ремонтных работ
        /// </summary>
        public DateTime EndDateSched
        {
            get { return _endDateSched; }
            set
            {
                if (SetProperty(ref _endDateSched, value))
                {
                    if (_endDateSched < _startDateSched)
                    {
                        _startDateSched = _endDateSched.AddDays(-1);
                        OnPropertyChanged(() => StartDateSched);
                    }
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }



        /// <summary>
        /// Начало допустимого диапазона выбора дат
        /// </summary>
        public DateTime AllowedDateRangeStart => new DateTime(_year, 1, 1);

        /// <summary>
        /// Конец допустимого диапазона выбора дат
        /// </summary>
        public DateTime AllowedDateRangeEnd => new DateTime(_year + 1, 12, 31);



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

            AddValidationFor(() => SelectedPlanType)
                .When(() => SelectedEntity != null && SelectedPlanType == null)
                .Show("Не указан вид работ");

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
                .When(() => BleedAmount == 0 && RepairWorkList != null && RepairWorkList.HasBleedWork)
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
                if (SetProperty(ref _maxTransferWinter, value))
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

        #region Attachments
        public bool HasID => Model != null && Model.Id > 0;
        public RepairAttachmentsViewModel Attachments => new RepairAttachmentsViewModel(Model.Id, false);
        #endregion

        #region Save

        public EditRepairParameterSet updateSet
        {
            get
            {
                return new EditRepairParameterSet
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
                            }).ToList(),


                    WorkflowState = (int)TargetState.WFState,
                    RepairState = (int)TargetState.WState,

                    DateEndFact = _dateEndFact,
                    DateStartFact = _dateStartFact,

                    DateEndShed = _endDateSched,
                    DateStartShed = _startDateSched,

                    FireworkType = (int)SelectedFireType.Firetype,

                    ResolutionDate = ResolutionDate,
                    ResolutionDateCpdd = ResolutionDateCpdd,
                    ResolutionNum = ResolutionNum,
                    ResolutionNumCpdd = ResolutionNumCpdd,
                   
                    GazpromPlanID = GazpromPlanID,
                    ConsumersState = ConsumersState,
                    GazpromPlanDate = GazpromPlanDate,
                    Duration = Duration,
                };
            }
        }
        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return CheckWorks() && !HasErrors;
        }

        protected override Task UpdateTask
        {
            get
            {
                return new RepairsServiceProxy().EditRepairAsync(ChangesAllowed ? updateSet : _updateSet);
            }
        }

        private async Task<int> Create()
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


                            ,

                DateEndShed = _endDateSched,
                DateStartShed = _startDateSched,

                FireworkType = (int)SelectedFireType.Firetype,
                GazpromPlanID = GazpromPlanID,
                ConsumersState = ConsumersState,
                GazpromPlanDate = GazpromPlanDate,
                Duration = Duration,
            };
            var tid = await new RepairsServiceProxy().AddRepairAsync(paramSet);

            await new RepairsServiceProxy().ChangeWorkflowStateAsync(new ChangeRepairWfParametrSet
            {
                RepairId = tid,
                WFState = WorkStateDTO.WorkflowStates.Draft,
                WState = WorkStateDTO.WorkStates.Undefined
            });

            return tid;
        }

        protected override Task<int> CreateTask
        {
            get
            {

                return Create();
                //    var paramSet = new AddRepairParameterSet
                //    {
                //        BleedAmount = BleedAmount,
                //        SavingAmount = SavingAmount,
                //        CapacityWinter = CapacityWinter,
                //        CapacitySummer = CapacitySummer,
                //        CapacityTransition = CapacityTransition,
                //        MaxTransferWinter = MaxTransferWinter,
                //        MaxTransferSummer = MaxTransferSummer,
                //        MaxTransferTransition = MaxTransferTransition,
                //        CalculatedTransfer = CalculatedTransfer,
                //        StartDate = StartDate,
                //        EndDate = EndDate,
                //        Description = Description,
                //        DescriptionGtp = DescriptionGtp,
                //        EntityId = SelectedEntity.Id,
                //        IsCritical = IsTransferRelation,
                //        PlanType = _planType,
                //        RepairType = SelectedRepairType.Id,
                //        ExecutionMeans = SelectedExecutionMeans.ExecutionMeans,
                //        IsExternalCondition = false,
                //        PartsDeliveryDate = PartsDeliveryDate,
                //        RepairWorks = RepairWorkList
                //            .Where(w => w.IsSelected)
                //            .Select(w =>
                //                new RepairWorkParameterSet
                //                {
                //                    WorkTypeId = w.Dto.Id,
                //                    KilometerStart = w.KilometerStart,
                //                    KilometerEnd = w.KilometerEnd
                //                }).ToList()


                //                ,

                //        DateEndShed = _endDateSched,
                //        DateStartShed = _startDateSched,

                //        FireworkType = (int)SelectedFireType.Firetype
                //    };
                //    var tid = new RepairsServiceProxy().AddRepairAsync(paramSet);

                //    new RepairsServiceProxy().ChangeWorkflowStateAsync(new ChangeRepairWfParametrSet
                //    {
                //        RepairId = tid.Id,
                //        WFState = WorkStateDTO.WorkflowStates.Draft,
                //        WState = WorkStateDTO.WorkStates.Undefined
                //    });

                //    return tid;
            }
        }

        #endregion

        #region Fireworks

        public List<FireworksDTO> FireTypeList => FireworksDTO.GetList();

        private FireworksDTO _selectedFireType = new FireworksDTO() { Firetype = FireworksDTO.FireTypes.OtherWork };
        public FireworksDTO SelectedFireType
        {
            get { return _selectedFireType; }
            set { _selectedFireType = value;
                OnPropertyChanged(() => SelectedFireType);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region AgreementList

        public AgreementListViewModel AgreementList => new AgreementListViewModel(Model.Id, true, true);

        #endregion

        #region Workflow

        private WorkflowHistoryViewModel _workflowHistory = new WorkflowHistoryViewModel(null);
        public WorkflowHistoryViewModel WorkflowHistory
        {
            get { return _workflowHistory; }
            set { if (SetProperty(ref _workflowHistory, value)) OnPropertyChanged(() => WorkflowHistory); }
        }

        public bool _changesAllowed = true;
        public bool ChangesAllowed
        {
            get { return _changesAllowed; }
            set
            {
                _changesAllowed = value;
                OnPropertyChanged(() => ChangesAllowed);
            }
        }

        public bool _statusAllowed = true;
        public bool IsSetStatusAllowed
        {
            get { return _statusAllowed; }
            set
            {
                _changesAllowed = value;
                OnPropertyChanged(() => _statusAllowed);
            }
        }

        private string _currentStateCaption = "";
        public string CurrentStateCaption {
            get
            { return _currentStateCaption; }
            set
            {
                if (SetProperty(ref _currentStateCaption, value))
                {
                    OnPropertyChanged(() => CurrentStateCaption);
                }
            }
        }
        private WorkStateDTO _currentState = new WorkStateDTO();
        public WorkStateDTO CurrentState
        {
            get
            { return _currentState; }
            set
            {
                if (SetProperty(ref _currentState, value))
                {
                    OnPropertyChanged(() => CurrentState);
                    TargetState = CurrentState;
                    CurrentStateCaption = CurrentState.Name;
                }
            }
        }

        private WorkStateDTO _targetState = new WorkStateDTO();
        public WorkStateDTO TargetState
        {
            get
            { return _targetState; }
            set
            {
                if (SetProperty(ref _targetState, value))
                {
                    if (_updateSet != null)
                    {
                        if (TargetState.WState == WorkStateDTO.WorkStates.Current && _dateStartFact == null)
                        {
                            _dateStartFact = DateTime.Now;
                            _updateSet.DateStartFact = _dateStartFact;
                        }
                        if (TargetState.WState == WorkStateDTO.WorkStates.Completed && _dateEndFact == null)
                        {
                            _dateEndFact = DateTime.Now;
                            _updateSet.DateEndFact = DateTime.Now;
                        }
                    }
                    CurrentStateCaption = _targetState.Name;
                }
            }
        }

        public DelegateCommand<WorkStateDTO> SetStatusCommand { get; set; }
        public List<SetStatusItem> SetStatusItemList { get; set; }

        public void BuildStatusMenu()
        {

            var temp = CurrentState.GetTransfers(UserProfile.Current.Site.IsEnterprise);
            SetStatusItemList = temp.Select(s => new SetStatusItem(SetStatusCommand, s)).ToList();
            //foreach (var tr in temp)
            //{
            //    result.Add(new SetStatusItem(SetStatusCommand) {  });
            //}


        }

        private void SetStatus(WorkStateDTO targetStatus)
        {
            RadWindow.Confirm(new DialogParameters
            {
                Header = "Изменение статуса ремонтных работ",
                Content = "Подтвердите пожалуйста установку статуса \"" +
                    targetStatus.Name
                + "\"",
                Closed = (o, even) =>
                {
                    if (even.DialogResult.HasValue && even.DialogResult.Value)
                    {
                        TargetState = targetStatus;
                        _updateSet.WorkflowState = (int)targetStatus.WFState;
                        _updateSet.RepairState = (int)targetStatus.WState;
                    }
                }
            });
        }

        private bool _isPds = false;
        public bool IsPds
        {
            get { return _isPds; }
            set
            {
                if (SetProperty(ref _isPds, value))
                    OnPropertyChanged(() => IsPds);
            }
        }

        private Microsoft.Practices.Prism.Commands.DelegateCommand _DocCommand;
        public Microsoft.Practices.Prism.Commands.DelegateCommand DocCommand => _DocCommand;

        public async void PrintDoc()
        {
            var agreeds = await AgreementList.GetList();
            if (IsPds)
            {
                //var viewModel = new PdsToCpddFaxViewModel(_repair, SelectedEntity, RepairWorkList, agreeds);
                //var view = new PdsToCpddFaxView { DataContext = viewModel };
                //view.ShowDialog();
                var DocFormatter = new PdsToCpddFaxDocFormatter(_repair, SelectedEntity, RepairWorkList, agreeds);
                ShowPrintForm(DocFormatter);
            }
            else
            {
                //var viewModel = new LpuToPdsFaxViewModel(_repair, SelectedEntity, RepairWorkList, agreeds);
                //var view = new LpuToPdsFaxView { DataContext = viewModel };
               // view.ShowDialog();
                var DocFormatter = new LpuToPdsFaxDocFormatter(_repair, SelectedEntity, RepairWorkList, agreeds);
                ShowPrintForm(DocFormatter);
            }
        }

        private Microsoft.Practices.Prism.Commands.DelegateCommand _AgreeFaxCommand;
        public Microsoft.Practices.Prism.Commands.DelegateCommand AgreeFaxCommand => _AgreeFaxCommand;

        public async void PrintFax()
        {
            var agreeds = await AgreementList.GetList();
            //var viewModel = new PdsToLpuFaxViewModel(RDocument, _repair, SelectedEntity, RepairWorkList, agreeds);
            //var view = new PdsToLpuFaxView { DataContext = viewModel };
            //view.ShowDialog();
            var DocFormatter = new PdsToLpuFaxDocFormatter(_repair, SelectedEntity, RepairWorkList, agreeds);
            ShowPrintForm(DocFormatter);
        }

        private void ShowPrintForm(IFaxDocFormatter DocFormatter)
        {
            var viewModel = new PrintFormViewModel(DocFormatter);
            var view = new PrintFormView() { DataContext = viewModel };
            view.ShowDialog();
        }

    #endregion.



}


    public class SetStatusItem 
    {
        public WorkStateDTO state { get; set; }
        public DelegateCommand<WorkStateDTO> Command { get; set; }
        public string Caption { get; set; }

        public SetStatusItem(DelegateCommand<WorkStateDTO> setStatusCommand, WorkStateDTO s)
        {
            Command = setStatusCommand;
            state = new WorkStateDTO() { WFState = s.WFState, WState = s.WState };
            Caption = s.Caption;
        }
    }
}