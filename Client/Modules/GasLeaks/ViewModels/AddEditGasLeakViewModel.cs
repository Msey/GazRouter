using System;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.GasLeaks;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.GasLeaks;
using GazRouter.DTO.ObjectModel;
using Utils.Extensions;

namespace GazRouter.GasLeaks.ViewModels
{
    public class AddEditGasLeakViewModel : AddEditViewModelBase<LeakDTO, int>
    {
        #region c-tors

        public AddEditGasLeakViewModel(Action<int> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            SetValidationRules();
            _discoverDate = DateTime.Now;
            ValidateAll();
        }


        public AddEditGasLeakViewModel(Action<int> actionBeforeClosing, ExtLeakDTO leak)
            : base(actionBeforeClosing, leak)
        {
			ContactName = leak.ContactName;
			Description = leak.Description;
			DiscoverDate = leak.DiscoverDate;
			Id = leak.Id;
			Kilometer = leak.Kilometer;
			Place = leak.Place;
			Reason = leak.Reason;
			RepairActivity = leak.RepairActivity;
			RepairPlanDate = leak.RepairPlanDate;
			RepairPlanFact = leak.RepairPlanFact;
			VolumeDay = leak.VolumeDay;
            SelectedEntity = new CommonEntityDTO
            {
                Id = leak.EntityId,
                Name = leak.EntityName,
                ShortPath = leak.EntityShortPath,
                EntityType = leak.EntityType
            };
            SetValidationRules();
            ValidateAll();
        }

        #endregion

        #region Entity

        private string _contactName;
        private string _description;
        private DateTime _discoverDate;
        private CommonEntityDTO _selectedEntity;
        private string _place;
        private double _kilometer;

        private string _reason;
        private string _repairActivity;
        private DateTime? _repairPlanDate;
        private DateTime? _repairPlanFact;
        private double _volumeDay;
        

        /// <summary>
        /// Выбранный объект
        /// </summary>
        public CommonEntityDTO SelectedEntity
        {
            get { return _selectedEntity; }
            set
            {
                _selectedEntity = value;
                OnPropertyChanged(() => SelectedEntity);
                OnPropertyChanged(() => IsEntitySelected);
                OnPropertyChanged(() => SelectedEntityIsPipeline);
                SaveCommand.RaiseCanExecuteChanged();

                if (value != null && value.EntityType == EntityType.Pipeline)
                {
                    // Если выбран газопровод, то нужно загрузить данные по нему из объектной модели,
                    // такие как км. начала и км. конца, а вводимый километр места утечки на газопроводе
                    // должен быть в допустимом диапазоне.

                    UpdatePipelineInfo(value.Id);
                }
            }
        }


        private async void UpdatePipelineInfo(Guid id)
        {
            try
            {
                Behavior.TryLock();
                var pipe = await new ObjectModelServiceProxy().GetPipelineByIdAsync(id);
                ClearValidations();
                SetValidationRules();

                AddValidationFor(() => Kilometer)
                    .When(
                        () =>
                            SelectedEntityIsPipeline &&
                            (Kilometer < pipe.KilometerOfStartPoint || Kilometer > pipe.KilometerOfEndPoint))
                    .Show(string.Format("Допустимое значение километра ({0} - {1})", pipe.KilometerOfStartPoint,
                        pipe.KilometerOfEndPoint));
                ValidateAll();

                SaveCommand.RaiseCanExecuteChanged();
            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }


        public bool IsEntitySelected
        {
            get { return SelectedEntity != null; }
        }

        /// <summary>
        /// Выбранный объект - это газопровод
        /// Если выбран газопровод, то отображать поле ввода для километра, 
        /// если точечный объект, то поле для ввода текствоого описания места 
        /// </summary>
        public bool SelectedEntityIsPipeline
        {
            get
            {
                return IsEntitySelected && SelectedEntity.EntityType == EntityType.Pipeline;
            }
        }


        /// <summary>
        /// Текстовое описание места утечки для точечных объектов
        /// </summary>
        public string Place
        {
            get { return _place; }
            set
            {
                if (_place == value) return;
                _place = value;
                OnPropertyChanged(() => Place);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Километр утечки на газопроводе
        /// </summary>
        public double Kilometer
        {
            get { return _kilometer; }
            set
            {
                if (_kilometer == value) return;
                _kilometer = value;
                OnPropertyChanged(() => Kilometer);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }




        /// <summary>
        /// Причина утечки
        /// </summary>
        public string Reason
        {
            get { return _reason; }
            set
            {
                if (_reason == value) return;
                _reason = value;
                OnPropertyChanged(() => Reason);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        
        /// <summary>
        /// Объем утечки газа в сутки
        /// </summary>
        public double VolumeDay
        {
            get { return _volumeDay; }
            set
            {
                _volumeDay = value;
                OnPropertyChanged(() => VolumeDay);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        
        /// <summary>
        /// Действия по устранению утечки
        /// </summary>
        public string RepairActivity
        {
            get { return _repairActivity; }
            set
            {
                _repairActivity = value;
                OnPropertyChanged(() => RepairActivity);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }


        /// <summary>
        /// Примечание
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(() => Description);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        
        /// <summary>
        /// ФИО ответсвенного лица
        /// </summary>
        public string ContactName
        {
            get { return _contactName; }
            set
            {
                _contactName = value;
                OnPropertyChanged(() => ContactName);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }


        /// <summary>
        /// Дата обнаружения утечки
        /// </summary>
        public DateTime DiscoverDate
        {
            get { return _discoverDate; }
            set
            {
                _discoverDate = value;
                OnPropertyChanged(() => DiscoverDate);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Плановая дата устранения утечки
        /// </summary>
        public DateTime? RepairPlanDate
        {
            get { return _repairPlanDate; }
            set
            {
                _repairPlanDate = value;
                OnPropertyChanged(() => RepairPlanDate);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Фактическия дата устранения утечки
        /// </summary>
        public DateTime? RepairPlanFact
        {
            get { return _repairPlanFact; }
            set
            {
                _repairPlanFact = value;
                OnPropertyChanged(() => RepairPlanFact);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }


        

        #endregion Entity

        protected override string CaptionEntityTypeName
        {
            get { return " утечки"; }
        }

        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }


        /// <summary>
        /// Редактирование
        /// </summary>
        protected override Task UpdateTask
        {
            get
            {
               return new GasLeaksServiceProxy().EditLeakAsync(
                new EditLeakParameterSet
                    {
                        LeakId = Model.Id,
                        Place = Place,
                        Kilometer = Kilometer,
                        Reason = Reason,
                        VolumeDay = VolumeDay,
                        RepairActivity = RepairActivity,
                        Description = Description,
                        ContactName = ContactName,
                        DiscoverDate = DiscoverDate.ToLocal(),
                        RepairPlanDate = RepairPlanDate.ToLocal(),
                        RepairPlanFact = RepairPlanFact.ToLocal(),
                        EntityId = SelectedEntity.Id
                    });
            }
        }


        /// <summary>
        /// Добавление новой утечки
        /// </summary>
        protected override Task<int> CreateTask
        {
            get
            {
                return new GasLeaksServiceProxy().AddLeakAsync(
                    new AddLeakParameterSet
                    {
                        Place = Place,
                        Kilometer = Kilometer,
                        Reason = Reason,
                        VolumeDay = VolumeDay,
                        RepairActivity = RepairActivity,
                        Description = Description,
                        ContactName = ContactName,
                        DiscoverDate = DiscoverDate.ToLocal(),
                        RepairPlanDate = RepairPlanDate.ToLocal(),
                        RepairPlanFact = RepairPlanFact.ToLocal(),
                        EntityId = SelectedEntity.Id,
                    });
            }
        }

        protected void SetValidationRules()
        {
            AddValidationFor(() => SelectedEntity)
                .When(() => SelectedEntity == null)
                .Show("Не выбран объект");

            AddValidationFor(() => DiscoverDate)
                .When(() => DiscoverDate > DateTime.Now)
                .Show("Дата обнаружения не может быть больше текущей даты");

            AddValidationFor(() => VolumeDay)
                .When(() => VolumeDay <= 0)
                .Show("Некорректное значение");

            AddValidationFor(() => Kilometer)
                .When(() => SelectedEntityIsPipeline && Kilometer < 0)
                .Show("Некорректное значение");

            AddValidationFor(() => RepairPlanDate)
                .When(() => RepairPlanDate < DiscoverDate)
                .Show("Дата устранения не может быть меньше даты обнаружения");

            AddValidationFor(() => RepairPlanFact)
                .When(() => RepairPlanFact < DiscoverDate)
                .Show("Дата устранения не может быть меньше даты обнаружения");

            AddValidationFor(() => RepairPlanFact)
                .When(() => RepairPlanFact > DateTime.Now)
                .Show("Фактическая дата устранения не может быть больше текущей даты");

        }

    }
}