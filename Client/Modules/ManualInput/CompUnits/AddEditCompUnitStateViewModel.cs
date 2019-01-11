using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DTO.Dictionaries.CompUnitFailureCauses;
using GazRouter.DTO.Dictionaries.CompUnitFailureFeatures;
using GazRouter.DTO.Dictionaries.CompUnitRepairTypes;
using GazRouter.DTO.Dictionaries.CompUnitStopTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.ManualInput.CompUnitStates;
using Utils.Extensions;


namespace GazRouter.ManualInput.CompUnits
{
    public class AddEditCompUnitStateViewModel : AddEditViewModelBase<CompUnitStateDTO, int>
    {
        private readonly CompUnitStateDTO _currentState;

        public AddEditCompUnitStateViewModel(CompUnitStateDTO currentState, Action<int> actionBeforeClosing, bool isEdit)
            : base(actionBeforeClosing)
        {
            IsEdit = isEdit;
            _currentState = currentState;

            if (IsEdit)
            {
                ChangeStateDate = _currentState.StateChangeDate;
                SelectedState = CompUnitStateList.First();
                SelectedStopType = _currentState.StopType.HasValue
                    ? ClientCache.DictionaryRepository.CompUnitStopTypes.Single(
                        t => t.CompUnitStopType == _currentState.StopType)
                    : null;

                if (_currentState.State == CompUnitState.Repair)
                {
                    RepairCompletionDate = _currentState.CompletionDatePlan;
                    SelectedRepairType = RepairTypeList.Single(t => t.UnitRepairType == _currentState.RepairType);
                }

                if (_currentState.State == CompUnitState.Reserve)
                {
                    RepairNext = _currentState.IsRepairNext;
                }

                if (_currentState.StopType != CompUnitStopType.Planned && _currentState.FailureDetails != null)
                {
                    IsCritical = currentState.FailureDetails.IsCritical;
                    SelectedFailureCause =
                        ClientCache.DictionaryRepository.CompUnitFailureCauses.Single(
                            c => c.CompUnitFailureCause == _currentState.FailureDetails.FailureCause);
                    SelectedFailureFeature =
                        ClientCache.DictionaryRepository.CompUnitFailureFeatures.Single(
                            c => c.CompUnitFailureFeature == _currentState.FailureDetails.FailureFeature);
                    FailureExternalView = _currentState.FailureDetails.FailureExternalView;
                    FailureCauseDescription = _currentState.FailureDetails.FailureCauseDescription;
                    WorkPerformed = _currentState.FailureDetails.FailureWorkPerformed;
                }
                
            }
            else
            {
                ChangeStateDate = DateTime.Now;
                //RepairCompletionDate = ChangeStateDate;
            }
            
            SetValidationRules();
            SaveCommand.RaiseCanExecuteChanged();
        }
        
        
        private DateTime _changeStateDate;
        /// <summary>
        /// Дата изменения состояния агрегата
        /// </summary>
        public DateTime ChangeStateDate
        {
            get { return _changeStateDate; }
            set
            {
                if (SetProperty(ref _changeStateDate, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }
        
        /// <summary>
        /// Список возможных состояний крана
        /// </summary>
        public List<StateBaseDTO> CompUnitStateList
        {
            get
            {
                if(IsEdit)
                    return ClientCache.DictionaryRepository.CompUnitStates.Where(s => s.Id == (int)_currentState.State).ToList();

                if (_currentState != null)
                    return
                        ClientCache.DictionaryRepository.CompUnitStates.Where(
                            s => s.Id != (int) _currentState.State && s.Id != (int) CompUnitState.Undefined).ToList();

                return
                    ClientCache.DictionaryRepository.CompUnitStates.Where(s => s.Id != (int) CompUnitState.Undefined)
                        .ToList();

            }
        }


        
        private StateBaseDTO _selectedState;
        
        /// <summary>
        /// Состояние крана
        /// </summary>
        public StateBaseDTO SelectedState
        {
            get { return _selectedState; }
            set
            {
                if (SetProperty(ref _selectedState, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                    OnPropertyChanged(() => IsReserve);
                    OnPropertyChanged(() => IsRepair);
                    OnPropertyChanged(() => IsStop); 
                }
                
            }
        }

        /// <summary>
        /// Новое состояние - резерв
        /// </summary>
        public bool IsReserve
        {
            get { return SelectedState != null && _selectedState.Id == (int)CompUnitState.Reserve; }
        }

        /// <summary>
        /// Новое состояние - ремонт
        /// </summary>
        public bool IsRepair
        {
            get { return SelectedState != null && _selectedState.Id == (int)CompUnitState.Repair; }
        }
        

        /// <summary>
        /// Изменение состояние - останов ГПА
        /// </summary>
        public bool IsStop
        {
            get
            {
                return IsEdit
                    ? _currentState?.StopType.HasValue ?? false
                    : SelectedState != null && _currentState?.State == CompUnitState.Work;
            }
        }



        /// <summary>
        /// Тип останова ГПА. отображается только в случае останова (IsStop == true)
        /// </summary>
        public List<CompUnitStopTypeDTO> StopTypeList
        {
            get { return ClientCache.DictionaryRepository.CompUnitStopTypes; }
        }



        private CompUnitStopTypeDTO _selectedStopType;
        /// <summary>
        /// Выбранный тип останова (плановый, аварийный, вынужденный)
        /// </summary>
        public CompUnitStopTypeDTO SelectedStopType
        {
            get { return _selectedStopType; }
            set
            {
                if (SetProperty(ref _selectedStopType, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                    OnPropertyChanged(() => IsFailure);
                }
            }
        }



        #region REPAIR

        private DateTime? _repairCompletionDate;
        /// <summary>
        /// Плановая дата завершения ремонта
        /// </summary>
        public DateTime? RepairCompletionDate
        {
            get { return _repairCompletionDate; }
            set
            {
                if(SetProperty(ref _repairCompletionDate, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }


        /// <summary>
        /// Виды ремонтных работ
        /// </summary>
        public List<CompUnitRepairTypeDTO> RepairTypeList
        {
            get { return ClientCache.DictionaryRepository.CompUnitRepairTypes; }
        }


        private CompUnitRepairTypeDTO _selectedRepairType;
        /// <summary>
        /// Выбранный вид ремонтных работ
        /// </summary>
        public CompUnitRepairTypeDTO SelectedRepairType
        {
            get { return _selectedRepairType; }
            set
            {
                if(SetProperty(ref _selectedRepairType, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion


        #region RESERVE
        /// <summary>
        /// Резерв с последующим ремонтом
        /// </summary>
        public bool RepairNext { get; set; }

        #endregion


        /// <summary>
        /// Вынужденный или аварийный останов
        /// </summary>
        public bool IsFailure
        {
            get { return IsStop && SelectedStopType != null && SelectedStopType.CompUnitStopType != CompUnitStopType.Planned; }
        }

        #region FAILURE 

        /// <summary>
        /// Влияние на транспорт газа
        /// </summary>
        public bool IsCritical { get; set; }


        /// <summary>
        /// Список признаков отказа
        /// </summary>
        public List<CompUnitFailureFeatureDTO> FailureFeatureList
        {
            get { return ClientCache.DictionaryRepository.CompUnitFailureFeatures; }
        }


        private CompUnitFailureFeatureDTO _selectedFailureFeature;
        /// <summary>
        /// Выбранный признак отказа
        /// </summary>
        public CompUnitFailureFeatureDTO SelectedFailureFeature
        {
            get { return _selectedFailureFeature; }
            set
            {
                if (SetProperty(ref _selectedFailureFeature, value))
                    SaveCommand.RaiseCanExecuteChanged();    
            }
        }


        /// <summary>
        /// Список причин отказа
        /// </summary>
        public List<CompUnitFailureCauseDTO> FailureCauseList
        {
            get { return ClientCache.DictionaryRepository.CompUnitFailureCauses; }
        }


        private CompUnitFailureCauseDTO _selectedFailureCause;
        /// <summary>
        /// Выбранная причина отказа
        /// </summary>
        public CompUnitFailureCauseDTO SelectedFailureCause
        {
            get { return _selectedFailureCause; }
            set
            {
                if (SetProperty(ref _selectedFailureCause, value))
                    SaveCommand.RaiseCanExecuteChanged(); 
            }
        }

        
        private string _failureExternalView;
        /// <summary>
        /// Внешнее проявление отказа
        /// </summary>
        public string FailureExternalView
        {
            get { return _failureExternalView; }
            set
            {
                if (SetProperty(ref _failureExternalView, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }


        private string _failureCauseDescription;
        /// <summary>
        /// Описание причины отказа
        /// </summary>
        public string FailureCauseDescription
        {
            get { return _failureCauseDescription; }
            set
            {
                if (SetProperty(ref _failureCauseDescription, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }


        private string _workPerformed;
        /// <summary>
        /// Выполненые работы по устранению причин отказа
        /// </summary>
        public string WorkPerformed
        {
            get { return _workPerformed; }
            set
            {
                if (SetProperty(ref _workPerformed, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }


        private void SetValidationRules()
        {
            AddValidationFor(() => SelectedState)
                .When(() => SelectedState == null)
                .Show("Не выбрано новое состояние");

            AddValidationFor(() => SelectedStopType)
                .When(() => IsStop && SelectedStopType == null)
                .Show("Не выбран тип останова");

            AddValidationFor(() => SelectedRepairType)
                .When(() => IsRepair && SelectedRepairType == null)
                .Show("Не выбран вид ремонтных работ");


            AddValidationFor(() => RepairCompletionDate)
                .When(() => IsRepair && RepairCompletionDate.HasValue && RepairCompletionDate < ChangeStateDate) 
                .Show("Плановая дата завешения ремонта не может быть меньше даты изменения состояния");



            AddValidationFor(() => SelectedFailureFeature)
                .When(() => IsFailure && SelectedFailureFeature == null)
                .Show("Укажите признак отказа");

            AddValidationFor(() => SelectedFailureCause)
                .When(() => IsFailure && SelectedFailureCause == null)
                .Show("Укажите причину отказа");


            var sessionPeriod = SeriesHelper.GetCurrentSessionPeriod();

            //AddValidationFor(() => ChangeStateDate)
            //    .When(() => ChangeStateDate < SwitchDateRangeStart || ChangeStateDate > SwitchDateRangeEnd)
            //    .Show(
            //        string.Format(
            //            "Недопустимое время переключения. Должно быть в интервале между {0:dd.MM.yyyy HH:mm} и {1:dd.MM.yyyy HH:mm}",
            //            SwitchDateRangeStart, DateTime.Now));


        }
        

        protected override string CaptionEntityTypeName
        {
            get { return " состояния агрегата"; }
        }


        protected override Task<int> CreateTask
        {
            get
            {
                return
                    new ManualInputServiceProxy().AddCompUnitStateAsync(
                        new AddCompUnitStateParameterSet
                        {
                            StateChangeDate = ChangeStateDate.ToLocal(),
                            CompUnitId = _currentState.CompUnitId,
                            State = (CompUnitState) SelectedState.Id,
                            StopType =
                                SelectedStopType != null ? (CompUnitStopType?) SelectedStopType.CompUnitStopType : null,
                            IsRepairNext = RepairNext,
                            RepairCompletionDate = RepairCompletionDate.ToLocal(),
                            RepairType =
                                SelectedRepairType != null
                                    ? (CompUnitRepairType?) SelectedRepairType.UnitRepairType
                                    : null,
                            IsCritical = IsCritical,
                            FailureCause =
                                SelectedFailureCause != null
                                    ? (CompUnitFailureCause?) SelectedFailureCause.CompUnitFailureCause
                                    : null,
                            FailureFeature =
                                SelectedFailureFeature != null
                                    ? (CompUnitFailureFeature?) SelectedFailureFeature.CompUnitFailureFeature
                                    : null,
                            FailureExternalView = FailureExternalView,
                            FailureCauseDescription = FailureCauseDescription,
                            FailureWorkPerformed = WorkPerformed
                        });

            }
        }

        protected override Task UpdateTask
        {
            get
            {
                return
                    new ManualInputServiceProxy().EditCompUnitStateAsync(
                        new EditCompUnitStateParameterSet
                        {
                            StateId = _currentState.Id,
                            StopType =
                                SelectedStopType != null ? (CompUnitStopType?)SelectedStopType.CompUnitStopType : null,
                            IsRepairNext = RepairNext,
                            RepairCompletionDate = RepairCompletionDate.ToLocal(),
                            RepairType =
                                SelectedRepairType != null
                                    ? (CompUnitRepairType?)SelectedRepairType.UnitRepairType
                                    : null,
                            IsCritical = IsCritical,
                            FailureCause =
                                SelectedFailureCause != null
                                    ? (CompUnitFailureCause?)SelectedFailureCause.CompUnitFailureCause
                                    : null,
                            FailureFeature =
                                SelectedFailureFeature != null
                                    ? (CompUnitFailureFeature?)SelectedFailureFeature.CompUnitFailureFeature
                                    : null,
                            FailureExternalView = FailureExternalView,
                            FailureCauseDescription = FailureCauseDescription,
                            FailureWorkPerformed = WorkPerformed
                        });

            }
        }


        
        
    }
    
}
