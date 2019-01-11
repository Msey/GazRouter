using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.EventLog;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.EventPriorities;
using GazRouter.DTO.EventLog;
using GazRouter.DTO.EventLog.EventRecipient;
using GazRouter.DTO.EventLog.TextTemplates;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.ObjectModel.Valves;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Prism.Commands;
using Utils.Extensions;
using GazRouter.DTO.Dictionaries.EventTypes;

namespace GazRouter.Modes.EventLog.Dialogs
{
    public class AddEditEventViewModel : AddEditViewModelBase<EventDTO, int>
    {
        private string _description;
        private Guid _entityId;
        private DateTime _eventDate;
        private DelegateCommand<string> _insertTextTemplateCommand;
        private double? _kilometer;
        private List<Guid> _neighborRecipientIdList;
        private CommonEntityDTO _selectedEntity;
        private bool _firstEditPipelineLoading;
        private List<Guid> _oldRecepients;
        private DelegateCommand _openTextTemplateEditorCommand;
        public AddEditEventViewModel(Action<int> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            EventDate = DateTime.Now;
            SelectedEventType = EventTypes.First();
            Init();
            SetValidationRules();
        }

        public AddEditEventViewModel(Action<int> actionBeforeClosing, EventDTO eventDTO)
            : base(actionBeforeClosing, eventDTO)
        {
            _firstEditPipelineLoading = true;
            Description = eventDTO.Description;
            EntityId = eventDTO.Entity.Id;
            if (eventDTO.EventDate.HasValue)
            {
                EventDate = eventDTO.EventDate.Value;
            }
            Id = eventDTO.Id;
            SelectedEventType = EventTypes.FirstOrDefault((e)=>e.Id == eventDTO.TypeId);
            Kilometer = eventDTO.Kilometer;
            Id = eventDTO.Id;
            Init();
            SetValidationRules();
        }

        public DelegateCommand<string> InsertTextTemplateCommand
        {
            get
            {
                return _insertTextTemplateCommand ?? (_insertTextTemplateCommand = new DelegateCommand<string>(tmpl =>
                {
                    if (!string.IsNullOrEmpty(Description))
                    {
                        Description += Environment.NewLine;
                    }
                    Description += tmpl;
                }));
            }
        }


        public DelegateCommand OpenTextTemplateEditorCommand
        {
            get
            {
                return _openTextTemplateEditorCommand ?? (_openTextTemplateEditorCommand = new DelegateCommand(() =>
                {
                    var vm = new TextTemplateEditorViewModel();
                    var v = new TextTemplateEditorView
                    {
                        DataContext = vm
                    };
                    v.Closed += (sender, args) => LoadTemplateList();
                    v.ShowDialog();
                }));
            }
        }

        public List<EventTextTemplate> TextTemplateList { get; set; }

        public bool IsTextTemplateListEmpty => TextTemplateList.Count == 0;

        public bool? IsAllSiteChecked
        {
            get
            {
                if (PossibleRecipientList.All(r => r.IsChecked))
                {
                    return true;
                }
                if (PossibleRecipientList.All(r => !r.IsChecked))
                {
                    return false;
                }
                return null;
            }
            set
            {
                PossibleRecipientList.ForEach(r => r.IsChecked = value ?? false);

                OnPropertyChanged(() => IsAllSiteChecked);
                OnPropertyChanged(() => IsNeighborSiteChecked);
            }
        }

        public bool IsNeighborSiteChecked
        {
            get
            {
                return
                    PossibleRecipientList
                        .Where(r => _neighborRecipientIdList.Any(n => n == r.SiteId))
                        .All(x => x.IsChecked);
            }
            set
            {
                PossibleRecipientList
                    .Where(r => _neighborRecipientIdList.Any(n => n == r.SiteId))
                    .ForEach(x => x.IsChecked = value);

                OnPropertyChanged(() => IsNeighborSiteChecked);
            }
        }

        public bool IsEnterprise => UserProfile.Current.Site.IsEnterprise;

        public override string Caption => IsEdit ? "Редактирование события" : "Регистрация события";

        public Guid EntityId
        {
            get { return _entityId; }
            set { SetProperty(ref _entityId, value); }
        }

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

        public DateTime EventDate
        {
            get { return _eventDate; }
            set
            {
                if (SetProperty(ref _eventDate, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public List<EventTypeDTO> EventTypes => ClientCache.DictionaryRepository.EventTypes.Where((e) => e.Id != 2 && e.Id != 5 && e.Id != 6).ToList<EventTypeDTO>(); 

        private EventTypeDTO _selectedEventType;
        public EventTypeDTO SelectedEventType
        {
            get { return _selectedEventType; }
            set
            {
                _selectedEventType = value;
                OnPropertyChanged(() => SelectedEventType);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        public ObservableCollection<EntityVm> PossibleRecipientList { get; private set; }

        public double KilometerMinValue { get; set; }
        public double KilometerMaxValue { get; set; }

        public double? Kilometer
        {
            get { return _kilometer; }
            set
            {
                if (SetProperty(ref _kilometer, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public CommonEntityDTO SelectedEntity
        {
            get { return _selectedEntity; }
            set
            {
                if (!SetProperty(ref _selectedEntity, value))
                {
                    return;
                }

                OnPropertyChanged(() => IsEntitySelected);
                OnPropertyChanged(() => SelectedEntityIsPipeline);

                if (value != null)
                {
                    CheckParentSite(value.Id);
                    UpdateStartEndPoints();
                }
                else
                {
                    Kilometer = null;
                    ValidateAll();
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsEntitySelected => SelectedEntity != null;

        public bool SelectedEntityIsPipeline => IsEntitySelected && SelectedEntity.EntityType == EntityType.Pipeline;

        public List<EntityType> AllowedTypes => new List<EntityType>
        {
            EntityType.Boiler,
            EntityType.CngFillingStation,
            EntityType.CompShop,
            EntityType.CompStation,
            EntityType.CompUnit,
            EntityType.Consumer,
            EntityType.CoolingStation,
            EntityType.DistrStation,
            EntityType.DistrStationOutlet,
            EntityType.PipelineGroup,
            EntityType.MeasLine,
            EntityType.MeasPoint,
            EntityType.MeasStation,
            EntityType.Pipeline,
            EntityType.PowerUnit,
            EntityType.ReducingStation,
            EntityType.Site,
            EntityType.Valve
        };

        protected override Task UpdateTask
        {
            get
            {
                var editRecepientsParameterSet = new EditRecepientsParameterSet
                {
                    RecepientsToDelete = _oldRecepients,
                    RecepientsToAdd = GetAddEventRecipientParameterSets()
                };

                if (!EventChanged)
                {
                    return new EventLogServiceProxy().EditRecepientsAsync(editRecepientsParameterSet);
                }

                var editEventParameterSet = new EditEventParameterSet
                {
                    Id = Id,
                    Text = Description,
                    EventDate = EventDate.ToLocal(),
                    EntityId = SelectedEntity.Id,
                    Kilometer = Kilometer,
                    TypeId = SelectedEventType.Id
                };
                var editEventAndRecepientsParameterSet = new EditEventAndRecepientsParameterSet
                {
                    Event = editEventParameterSet,
                    Recepients = editRecepientsParameterSet
                };

                return new EventLogServiceProxy().EditEventAndRecepientsAsync(editEventAndRecepientsParameterSet);
            }
        }

        protected bool EventChanged
            =>
                Model.Description != Description || Model.EventDate != EventDate || Model.Entity.Id != SelectedEntity.Id ||
                Model.TypeId != SelectedEventType.Id;

        protected override Task<int> CreateTask
        {
            get
            {
                var registerEventParameterSet = new RegisterEventParameterSet
                {
                    Text = Description,
                    EventDate = EventDate.ToLocal(),
                    EntityId = SelectedEntity.Id,
                    Kilometer = Kilometer,
                    TypeId = SelectedEventType.Id
                };
                var addEventRecipientParameterSets = GetAddEventRecipientParameterSets();
                return new EventLogServiceProxy().AddEventAndRecepientsAsync(new AddEventAndRecepientsParameterSet
                {
                    Event = registerEventParameterSet,
                    Recepients = addEventRecipientParameterSets
                });
            }
        }

        protected override string CaptionEntityTypeName
        {
            get { throw new NotSupportedException(); }
        }

        private bool RecepientsChanged
        {
            get
            {
                var newReceps = PossibleRecipientList.Where(r => r.IsChecked).Select(r => r.Id).ToList();
                return _oldRecepients.Any(r => !newReceps.Contains(r)) ||
                       newReceps.Any(r => !_oldRecepients.Contains(r));
            }
        }

        public void RaiseCommands()
        {
            SaveCommand.RaiseCanExecuteChanged();
            OnPropertyChanged(() => IsAllSiteChecked);
            OnPropertyChanged(() => IsNeighborSiteChecked);
        }

        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors && !(IsEdit && !(IsEdit && (EventChanged || RecepientsChanged)));
        }

        private async void Init()
        {
            PossibleRecipientList = new ObservableCollection<EntityVm>();

            _oldRecepients = new List<Guid>();

            _neighborRecipientIdList = new List<Guid>();
            Behavior.TryLock("Загрузка списка ЛПУ");
            List<CommonEntityDTO> result;
            try
            {
                result = await new ObjectModelServiceProxy().GetCurrentEnterpriseAndSitesAsync();
            }
            finally
            {
                Behavior.TryUnlock();
            }

            result.Where(en => en.Id != UserProfile.Current.Site.Id && en.EntityType == EntityType.Enterprise)
                .OrderBy(en => en.Name)
                .ToList()
                .ForEach(
                    en =>
                        PossibleRecipientList.Add(new EntityVm
                        {
                            SiteId = en.Id,
                            Name = en.Name,
                            OnCheckedChanged = RaiseCommands,
                            IsChecked = false
                        }));

            result.Where(en => en.Id != UserProfile.Current.Site.Id && en.EntityType != EntityType.Enterprise)
                .OrderBy(en => en.Name)
                .ToList()
                .ForEach(
                    en =>
                        PossibleRecipientList.Add(new EntityVm
                        {
                            SiteId = en.Id,
                            Name = en.Name,
                            OnCheckedChanged = RaiseCommands,
                            IsChecked = false
                        }));

            if (IsEdit)
            {
                GetEventRecipientList();
            }

            OnPropertyChanged(() => IsAllSiteChecked);
            OnPropertyChanged(() => IsNeighborSiteChecked);

            if (!UserProfile.Current.Site.IsEnterprise)
            {
                GetNeighborRecipientList();
            }

            LoadTemplateList();
        }

        private async void LoadTemplateList()
        {
            Behavior.TryLock("Загрузка списка шаблонов");
            List<EventTextTemplateDTO> list;
            try
            {
                list = await new EventLogServiceProxy().GetEventTextTemplateListAsync(UserProfile.Current.Site.Id);
            }
            finally
            {
                Behavior.TryUnlock();
            }
            TextTemplateList = list.Select(t => new EventTextTemplate(t, InsertTextTemplateCommand)).ToList();
            OnPropertyChanged(() => TextTemplateList);
            OnPropertyChanged(() => IsTextTemplateListEmpty);
        }

        private async void GetEventRecipientList()
        {
            Behavior.TryLock("Загрузка списка получателей события");
            List<EventRecepientDTO> receps;
            try
            {
                receps = await new EventLogServiceProxy().GetEventRecepientListAsync(Id);
            }
            finally
            {
                Behavior.TryUnlock();
            }
            receps.ForEach(r =>
            {
                var recepient = PossibleRecipientList.SingleOrDefault(pr => r.SiteId == pr.SiteId);
                if (recepient == null)
                {
                    return;
                }
                recepient.Id = r.Id;
                recepient.IsChecked = true;
                _oldRecepients.Add(r.Id);
            });
        }

        private List<AddEventRecipientParameterSet> GetAddEventRecipientParameterSets()
        {
            return PossibleRecipientList
                .Where(r => r.IsChecked)
                .Select(r => new AddEventRecipientParameterSet
                {
                    EventId = Id,
                    SiteId = r.SiteId,
                    Priority = EventPriority.Normal
                }).ToList();
        }

        private async void GetNeighborRecipientList()
        {
            Behavior.TryLock("Получаем список соседей");
            List<SiteDTO> result;
            try
            {
                result = await new ObjectModelServiceProxy().GetSiteListAsync(
                    new GetSiteListParameterSet
                    {
                        SiteId = UserProfile.Current.Site.Id
                    });
            }
            finally
            {
                Behavior.TryUnlock();
            }
            _neighborRecipientIdList = result.First().NeighbourSiteIdList;

            OnPropertyChanged(() => IsNeighborSiteChecked);
        }

        private void SetValidationRules()
        {
            AddValidationFor(() => EventDate)
                .When(() => EventDate > DateTime.Now)
                .Show("Дата события не может быть больше текущей даты");

            AddValidationFor(() => SelectedEntity)
                .When(OnCriteria)
                .Show("Объект не выбран");

            AddValidationFor(() => Description)
                .When(() => string.IsNullOrEmpty(Description))
                .Show("Отсутствует текст события");

            AddValidationFor(() => Kilometer)
                .When(
                    () =>
                        SelectedEntityIsPipeline &&
                        (Kilometer < KilometerMinValue ||
                         Kilometer > KilometerMaxValue))
                .Show($"Допустимое значение километра ({KilometerMinValue} - {KilometerMaxValue})");
        }

        private bool OnCriteria()
        {
            return SelectedEntity == null;
        }

        private async void UpdateStartEndPoints()
        {
            switch (SelectedEntity.EntityType)
            {
                case EntityType.Pipeline:
                    // Если выбран газопровод, то нужно загрузить данные по нему из объектной модели,
                    // такие как км. начала и км. конца, а вводимый километр места утечки на газопроводе
                    // должен быть в допустимом диапазоне.
                    Behavior.TryLock("Получаем данные о газопроводе");
                    PipelineDTO pipe; //,
                    try
                    {
                        pipe = await new ObjectModelServiceProxy().GetPipelineByIdAsync(SelectedEntity.Id);
                    }
                    finally
                    {
                        Behavior.TryUnlock();
                    }

                    KilometerMinValue = pipe.KilometerOfStartPoint;
                    KilometerMaxValue = pipe.KilometerOfStartPoint + pipe.Length;

                    if (_firstEditPipelineLoading)
                    {
                        _firstEditPipelineLoading = false;
                    }
                    else
                    {
                        Kilometer = pipe.KilometerOfStartPoint;
                    }

                    SaveCommand.RaiseCanExecuteChanged();

                    break;

                case EntityType.Valve:
                    Behavior.TryLock("Получаем данные о кране");
                    //Если выбран кран, то нужно загрузить данные по нему из объектной модели и выбирать километр из его DTO
                    ValveDTO dto;
                    try
                    {
                        dto = await new ObjectModelServiceProxy().GetValveByIdAsync(SelectedEntity.Id);
                    }
                    finally
                    {
                        Behavior.TryUnlock();
                    }

                    Kilometer = dto.Kilometer;

                    SaveCommand.RaiseCanExecuteChanged();
                    break;
            }
        }

        private async void CheckParentSite(Guid id)
        {
            var siteId = await new ObjectModelServiceProxy().FindSiteAsync(id);
            var entityVm = siteId.HasValue ? PossibleRecipientList.FirstOrDefault(vm => vm.SiteId == siteId) : null;

            if (entityVm != null)
            {
                entityVm.IsChecked = true;
            }
        }
    }
}