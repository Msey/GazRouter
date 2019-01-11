using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Repairs;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PlanTypes;
using GazRouter.DTO.Dictionaries.RepairExecutionMeans;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.Repairs.Plan;

namespace GazRouter.Repair.Dialogs
{
    public class AddEditExternalConditionViewModel : AddEditViewModelBase<RepairPlanBaseDTO, int>
    {
        public AddEditExternalConditionViewModel(Action<int> actionBeforeClosing, RepairPlanBaseDTO repair, int year)
            : base(actionBeforeClosing, repair)
        {
            _planType = PlanType.Planned;

            AllowedDateRangeStart = new DateTime(year, 1, 1);
            AllowedDateRangeEnd = new DateTime(year, 12, 31);

            SelectedEntity = new CommonEntityDTO
            {
                Id = repair.EntityId,
                EntityType = repair.EntityType,
                Name = repair.EntityName,
                ShortPath = repair.EntityName
            };
            
            SetValidationRules();

            SaveCommand.RaiseCanExecuteChanged();
        }

        public AddEditExternalConditionViewModel(Action<int> actionBeforeClosing, int year)
            : base(actionBeforeClosing)
        {
            Model.StartDate = DateTime.Today.AddYears(year - DateTime.Now.Year);
            Model.EndDate = StartDate.AddDays(1);
            _planType = PlanType.Planned;

            AllowedDateRangeStart = new DateTime(year, 1, 1);
            AllowedDateRangeEnd = new DateTime(year, 12, 31);
            SetValidationRules();

            SaveCommand.RaiseCanExecuteChanged();
        }
        
        private readonly PlanType? _planType;
        
   
        protected override string CaptionEntityTypeName
        {
            get { return "внешних условий"; }
        }

        public List<EntityType> AllowedType
        {
            get
            {
                return new List<EntityType>
                {
                    EntityType.DistrStation,
                    EntityType.MeasStation
                };
            }
        }

       

        
        
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
                if (_selectedEntity != value)
                {
                    _selectedEntity = value;
                    OnPropertyChanged(() => SelectedEntity);
                }
            }
        }

        /// <summary>
        /// Выбран ли объект
        /// </summary>
        public bool IsEntitySelected
        {
            get { return _selectedEntity != null; }
        }

        #endregion

        
        #region Dates

        private const int DefaultDateDelta = 1;

        /// <summary>
        /// Плановая дата начала проведения ремонтных работ
        /// </summary>
        public DateTime StartDate
        {
            get { return Model.StartDate; }
            set
            {
                Model.StartDate = value;
              
                OnPropertyChanged(() => StartDate);
                if (Model.StartDate >= Model.EndDate)
                    EndDate = Model.StartDate.AddDays(DefaultDateDelta) > AllowedDateRangeEnd
                        ? AllowedDateRangeEnd
                        : Model.StartDate.AddDays(DefaultDateDelta);

                OnPropertyChanged(() => Duration);
            }
        }
        

        /// <summary>
        /// Плановая дата завершения ремонтных работ
        /// </summary>
        public DateTime EndDate
        {
            get { return Model.EndDate; }
            set
            {
                Model.EndDate = value;
                OnPropertyChanged(() => EndDate);
                if (Model.EndDate <= Model.StartDate)
                    StartDate = Model.EndDate.AddDays(-DefaultDateDelta) < AllowedDateRangeStart
                        ? AllowedDateRangeStart
                        : Model.EndDate.AddDays(-DefaultDateDelta);

                OnPropertyChanged(() => Duration);
            }
        }



        private DateTime _allowedDateRangeStart;
        /// <summary>
        /// Начало допустимого диапазона выбора дат
        /// </summary>
        public DateTime AllowedDateRangeStart
        {
            get { return _allowedDateRangeStart; }
            set
            {
                _allowedDateRangeStart = value;
                OnPropertyChanged(() => AllowedDateRangeStart);
            }
        }



        private DateTime _allowedDateRangeEnd;
        /// <summary>
        /// Конец допустимого диапазона выбора дат
        /// </summary>
        public DateTime AllowedDateRangeEnd
        {
            get { return _allowedDateRangeEnd; }
            set
            {
                _allowedDateRangeEnd = value;
                OnPropertyChanged(() => AllowedDateRangeEnd);
            }
        }

        /// <summary>
        /// Продолжительность работ, ч
        /// </summary>
        public int Duration
        {
            get
            {
                return (int)(EndDate - StartDate).TotalHours;
            }
        }
        

        #endregion

        

        private void SetValidationRules()
        {
           
            AddValidationFor(() => Description)
                .When(() => string.IsNullOrEmpty(Description))
                .Show("Введите текстовое описание работ");
            
        }

        
        
        
        

        /// <summary>
        /// Описание работ
        /// </summary>
        public string Description
        {
            get { return Model.Description; }
            set
            {
                Model.Description = value;
                OnPropertyChanged(() => Description);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        

        /// <summary>
        /// Комментарий ГТП
        /// </summary>
        public string DescriptionGtp
        {
            get { return Model.DescriptionGtp; }
            set
            {
                Model.DescriptionGtp = value;
                OnPropertyChanged(() => DescriptionGtp);
            }
        }
        

        #region Save

       

        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }

        protected override Task<int> CreateTask
        {
            get
            {
                return new RepairsServiceProxy().AddRepairAsync(
                    new AddRepairParameterSet
                    {
                        StartDate = StartDate,
                        EndDate = EndDate,
                        Description = Description,
                        DescriptionGtp = DescriptionGtp,
                        EntityId = SelectedEntity.Id,
                        PlanType = _planType,
                        IsExternalCondition = true,
                        ExecutionMeans = ExecutionMeans.Internal,
                        RepairType = 100
                    });
            }
        }

        protected override Task UpdateTask
        {
            get
            {
                return new RepairsServiceProxy().EditRepairAsync(
                    new EditRepairParameterSet
                    {

                        StartDate = StartDate,
                        EndDate = EndDate,
                        Description = Description,
                        DescriptionGtp = DescriptionGtp,
                        EntityId = SelectedEntity.Id,
                        Id = Model.Id,
                        PlanType = _planType,
                        IsExternalCondition = true,
                        ExecutionMeans = ExecutionMeans.Internal,
                        RepairType = 100

                    });
            }
        }
       
        #endregion

        
        
    }
}