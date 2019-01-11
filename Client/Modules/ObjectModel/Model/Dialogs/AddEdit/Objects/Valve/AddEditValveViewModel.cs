using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Dictionaries.ValvePurposes;
using GazRouter.DTO.Dictionaries.ValveTypes;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;
using GazRouter.DTO.ObjectModel.Valves;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.Valve
{
    public class AddEditValveViewModel : AddEditViewModelBase<ValveDTO>
    {
        protected PipelineDTO ParentPipeline;
        private List<DiameterSegmentDTO> _diameterSegments;

        public AddEditValveViewModel(Action<Guid> actionBeforeClosing, ValveDTO model, PipelineDTO parentPipeline)
            : base(actionBeforeClosing, model)
        {
            //Mapper.Map(model, this);
            ParentPipeline = parentPipeline;
            SelectedCompShopId = model.CompShopId;
            Kilometer = model.Kilometer;
            LoadValvePurposes();
            SelectedTypeValve =
                ClientCache.DictionaryRepository.ValveTypes.Single(
                    v => v.Id == model.ValveTypeId);

            SelectedValvePurpose =
                ClientCache.DictionaryRepository.ValvePurposes.Single(
                    p => p.Id == (int)model.ValvePurposeId);


            Bypass1TypeId = model.Bypass1TypeId;
            Bypass2TypeId = model.Bypass2TypeId;
            Bypass3TypeId = model.Bypass3TypeId;

            IsControlPoint = model.IsControlPoint;

            SetValidationRules();

            ShowNamingHint = new DelegateCommand(() => IsNamingHintVisible = !IsNamingHintVisible);

            LoadSegments();
        }


		public AddEditValveViewModel(Action<Guid> actionBeforeClosing, PipelineDTO parentPipeline, int sortorder)
            : base(actionBeforeClosing)
        {
            ParentPipeline = parentPipeline;
            if (ParentPipeline.Type == PipelineType.CompressorShopInlet)
                SelectedCompShopId = ParentPipeline.EndEntityId;
            if (ParentPipeline.Type == PipelineType.CompressorShopOutlet)
                SelectedCompShopId = ParentPipeline.BeginEntityId;
            LoadValvePurposes();
            SelectedTypeValve = ClientCache.DictionaryRepository.ValveTypes.First();
            
            SetValidationRules();
			_sortorder = sortorder;
            ShowNamingHint = new DelegateCommand(() => IsNamingHintVisible = !IsNamingHintVisible);
            
            LoadSegments();
        }

		private int _sortorder;

        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }


		protected override Task UpdateTask
		{
			get
			{
				return new ObjectModelServiceProxy().EditValveAsync(
					new EditValveParameterSet
						{
							Id = Model.Id,
							Name = Name,
							Kilometr = Kilometer.Value,
							Bypass1TypeId = Bypass1TypeId,
                            Bypass2TypeId = Bypass2TypeId,
                            Bypass3TypeId = Bypass3TypeId,
							PipelineId = ParentPipeline.Id,
							ValveTypeId = SelectedTypeValve.Id,
							ValvePurposeId = SelectedValvePurpose.ValvePurpose,
							CompShopId = SelectedCompShopId,
                            IsControlPoint = IsControlPoint
						});
			}
		}

		protected override Task<Guid> CreateTask
		{
			get
			{
				return new ObjectModelServiceProxy().AddValveAsync(
					new AddValveParameterSet
					{
						Name = Name,
						Kilometr = Kilometer.Value,
                        Bypass1TypeId = Bypass1TypeId,
                        Bypass2TypeId = Bypass2TypeId,
                        Bypass3TypeId = Bypass3TypeId,
						PipelineId = ParentPipeline.Id,
						ValveTypeId = SelectedTypeValve.Id,
						ValvePurposeId = SelectedValvePurpose.ValvePurpose,
						CompShopId = SelectedCompShopId,
						SortOrder = _sortorder,
                        IsControlPoint = IsControlPoint
                    });
			}
		}

        #region ListValvePurpose

        private List<ValvePurposeDTO> _listValvePurpose = new List<ValvePurposeDTO>();

        public List<ValvePurposeDTO> ListValvePurpose
        {
            get
            {
                switch (ParentPipeline.Type)
                {
                    case PipelineType.Main:
                        return _listValvePurpose.Where(p =>
                            p.ValvePurpose == ValvePurpose.Linear ||
                            p.ValvePurpose == ValvePurpose.InletProtectiveCompShop ||
                            p.ValvePurpose == ValvePurpose.TransversalCompShop ||
                            p.ValvePurpose == ValvePurpose.OutletProtectiveCompShop).ToList();
                    case PipelineType.CompressorShopInlet:
                        return _listValvePurpose.Where(p => p.ValvePurpose == ValvePurpose.InletCompShop).ToList();
                    case PipelineType.CompressorShopOutlet:
                        return _listValvePurpose.Where(p => p.ValvePurpose == ValvePurpose.OutletCompShop).ToList();
                    case PipelineType.RefiningDeviceChamber:
                        return _listValvePurpose.Where(p => 
                        p.ValvePurpose == ValvePurpose.ReceivingRefiningDevice ||
                        p.ValvePurpose == ValvePurpose.StartingRefiningDevice).ToList();
                    default:
                        return _listValvePurpose.Where(p => p.ValvePurpose == ValvePurpose.Linear).ToList();
                }
            }
            set { SetProperty(ref _listValvePurpose, value); }
        }

        #endregion

        #region SelectedCompShopId

        private Guid? _selectedCompShopId;

        public Guid? SelectedCompShopId
        {
            get { return _selectedCompShopId; }
            set
            {
                if (SetProperty(ref _selectedCompShopId, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public List<EntityType> AllowedType
        {
            get
            {
                return new List<EntityType>
                {
                    EntityType.CompShop
                };
            }
        }

        #endregion

        #region SelectedValvePurpose

        private ValvePurposeDTO _selectedValvePurpose;

        public ValvePurposeDTO SelectedValvePurpose
        {
            get { return _selectedValvePurpose; }
            set
            {
                if (SetProperty(ref _selectedValvePurpose, value))
                {
                    OnPropertyChanged(() => IsCsEnabled);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private void LoadValvePurposes()
        {
            var purposeList =
                (ClientCache.DictionaryRepository.ValvePurposes);
            ListValvePurpose = purposeList.OrderBy(l => l.Name).ToList();
            _selectedValvePurpose = ListValvePurpose.FirstOrDefault(dto => dto.ValvePurpose == Model.ValvePurposeId);

            if (_selectedValvePurpose == null && ListValvePurpose.Count == 1)
            {
                _selectedValvePurpose = ListValvePurpose.Single();
            }

            OnPropertyChanged(() => ListValvePurpose);
            OnPropertyChanged(() => SelectedValvePurpose);
        }

        #endregion

        #region Kilometer

        private double? _kilometer;
        private ValveTypeDTO _selectedTypeValve;

        public double? Kilometer
        {
            get { return _kilometer; }
            set
            {
                if (SetProperty(ref _kilometer, value))
                {
                    if (Kilometer.HasValue
                        && Kilometer.Value >= ParentPipeline.KilometerOfStartPoint
                        && Kilometer.Value <= ParentPipeline.KilometerOfStartPoint + ParentPipeline.Length)
                    {
                        if (_diameterSegments == null) return;
                        var seg =
                            _diameterSegments.FirstOrDefault(
                                s => s.KilometerOfStartPoint <= Kilometer && s.KilometerOfEndPoint >= Kilometer);
                        if (seg != null)
                            SelectedTypeValve = ListTypeValve.Single(v => v.DiameterConv == seg.DiameterConv);
                        
                        CheckDiameter();
                    }
                    
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region BypassTypes
        

        private int? _bypass1TypeId;
        public int? Bypass1TypeId
        {
            get { return _bypass1TypeId; }
            set
            {
                if (SetProperty(ref _bypass1TypeId, value))
                {
                    if (!_bypass1TypeId.HasValue)
                        Bypass2TypeId = null;
                    OnPropertyChanged(() => HasBypass1);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool HasBypass1
        {
            get { return Bypass1TypeId.HasValue; }
        }

        private int? _bypass2TypeId;
        public int? Bypass2TypeId
        {
            get { return _bypass2TypeId; }
            set
            {
                if (SetProperty(ref _bypass2TypeId, value))
                {
                    if (!_bypass2TypeId.HasValue)
                        Bypass3TypeId = null;
                    OnPropertyChanged(() => HasBypass2);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool HasBypass2
        {
            get { return Bypass2TypeId.HasValue; }
        }

        private int? _bypass3TypeId;
        public int? Bypass3TypeId
        {
            get { return _bypass3TypeId; }
            set
            {
                if (SetProperty(ref _bypass3TypeId, value))
                    SaveCommand.RaiseCanExecuteChanged();
                
            }
        }

        private bool _isNamingHintVisible;
        private bool _isTypeValveSelectorEnable;

        #endregion

        private bool _isControlPoint;
        public bool IsControlPoint
        {
            get { return _isControlPoint;}
            set
            {
                if (SetProperty(ref _isControlPoint, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }


        protected override string CaptionEntityTypeName
        {
            get { return "кранового узла"; }
        }
        
        protected void SetValidationRules()
        {
            AddValidationFor(() => Name)
                .When(() => string.IsNullOrEmpty(Name))
                .Show("Укажите наименование кранового узла");

            AddValidationFor(() => SelectedTypeValve)
                .When(() => SelectedTypeValve == null)
                .Show("Укажите тип кранового узла");

            AddValidationFor(() => SelectedValvePurpose)
                .When(() => SelectedValvePurpose == null)
                .Show("Укажите назначение кранового узла");

            AddValidationFor(() => SelectedCompShopId)
                .When(() => IsCsEnabled && SelectedCompShopId == null)
                .Show("Укажите компрессорный цех, к охранной зоне которого относится крановый узел");



            AddValidationFor(() => Kilometer)
                .When(() => !Kilometer.HasValue)
                .Show("Укажите километр установки крана на газопроводе");

            AddValidationFor(() => Kilometer)
                .When(() => Kilometer.HasValue 
                    && (Kilometer.Value < ParentPipeline.KilometerOfStartPoint 
                    || Kilometer.Value > ParentPipeline.KilometerOfStartPoint + ParentPipeline.Length))
                .Show(string.Format("Недопустимое значение километра установки крана на газопроводе ({0} - {1})", 
                ParentPipeline.KilometerOfStartPoint, ParentPipeline.KilometerOfStartPoint + ParentPipeline.Length));



            AddValidationFor(() => Bypass1TypeId)
                .When(
                    () =>
                        Bypass1TypeId.HasValue && SelectedTypeValve != null &&
                        ListTypeValve.Single(t => t.Id == Bypass1TypeId.Value).DiameterConv >
                        SelectedTypeValve.DiameterConv)
                .Show("Диаметр байпасной линии не может быть больше диаметра основного крана");

            AddValidationFor(() => Bypass2TypeId)
                .When(
                    () =>
                        Bypass2TypeId.HasValue && SelectedTypeValve != null &&
                        ListTypeValve.Single(t => t.Id == Bypass2TypeId.Value).DiameterConv >
                        SelectedTypeValve.DiameterConv)
                .Show("Диаметр байпасной линии не может быть больше диаметра основного крана");

            AddValidationFor(() => Bypass3TypeId)
                .When(
                    () =>
                        Bypass3TypeId.HasValue && SelectedTypeValve != null &&
                        ListTypeValve.Single(t => t.Id == Bypass3TypeId.Value).DiameterConv >
                        SelectedTypeValve.DiameterConv)
                .Show("Диаметр байпасной линии не может быть больше диаметра основного крана");
        }

        public DelegateCommand ShowNamingHint { get; private set; }
        
        public bool IsNamingHintVisible
        {
            get { return _isNamingHintVisible; }
            set { SetProperty(ref _isNamingHintVisible, value); }
        }

        public bool IsNamingHintButtonVisible
        {
            get { return ParentPipeline.Type == PipelineType.Bridge; }
        }

        #region ValveType

        public List<ValveTypeDTO> ListTypeValve
        {
            get { return ClientCache.DictionaryRepository.ValveTypes; }
        }

        public ValveTypeDTO SelectedTypeValve
        {
            get { return _selectedTypeValve; }
            set
            {
                if (SetProperty(ref _selectedTypeValve, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                    CheckDiameter();
                }
            }
        }

        public bool IsTypeValveSelectorEnable
        {
            get { return _isTypeValveSelectorEnable; }
            set
            {
                SetProperty(ref _isTypeValveSelectorEnable, value);
            }
        }

        public bool IsCsEnabled
        {
            get
            {
                if (SelectedValvePurpose == null) return false;
                switch (SelectedValvePurpose.ValvePurpose)
                {
                    case ValvePurpose.InletProtectiveCompShop:
                    case ValvePurpose.TransversalCompShop:
                    case ValvePurpose.OutletProtectiveCompShop:
                    case ValvePurpose.InletCompShop:
                    case ValvePurpose.OutletCompShop:
                        return true;
                    default:
                        return false;
                }
            }
        }
        
        #endregion


        #region CheckDiameter

        private async void LoadSegments()
        {
            try
            {
                Behavior.TryLock();
                _diameterSegments = await new ObjectModelServiceProxy().GetDiameterSegmentListAsync(ParentPipeline.Id);
                IsTypeValveSelectorEnable = true;
            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }


        public string WarningMessage { get; set; }
        public bool IsWarningMessageVisible
        {
            get { return !string.IsNullOrEmpty(WarningMessage); }
        }

        private void CheckDiameter()
        {
            if (_diameterSegments == null) return; 
            var seg = _diameterSegments.FirstOrDefault(s => s.KilometerOfStartPoint <= Kilometer && s.KilometerOfEndPoint >= Kilometer);
            if (seg == null)
                WarningMessage =
                    "Для выбранного километра установки крана отсутвует сегмент по диаметру. Необходимо ввести сегмент в списке сегментов газопровода.";
            else
            {
                WarningMessage = seg.DiameterConv != SelectedTypeValve.DiameterConv
                    ? string.Format(
                        "Выбранный диаметр крана не соответсвует диаметру сегмента газопровода. Необходимо выбрать диаметр крана соответсвующий диаметру сегмента ({0}), либо изменить диаметр сегмента.",
                        seg.DiameterConv)
                    : "";

            }
            OnPropertyChanged(() => WarningMessage);
            OnPropertyChanged(() => IsWarningMessageVisible);
        }

        #endregion
    }
}