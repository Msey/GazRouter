using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.ReducingStations;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.ReducingStation
{
    public class AddEditReducingStationViewModel : AddEditViewModelBase<ReducingStationDTO>
    {
        private readonly int _sortorder;
        private bool _firstEditLoading;

        public AddEditReducingStationViewModel(Action<Guid> actionBeforeClosing, ReducingStationDTO model)
            : base(actionBeforeClosing, model)
        {
            _firstEditLoading = true;
            KilometerIsEnabled = true;
			Id = model.Id;
			Kilometer = model.Kilometer;
			Name = model.Name;
			PipelineId = model.PipelineId;
            SiteId = model.ParentId.Value;
            SelectedPipelineId = model.PipelineId;
        }

		public AddEditReducingStationViewModel(Action<Guid> actionBeforeClosing, Guid siteId, int sortorder)
            : base(actionBeforeClosing)
        {
            SiteId = siteId;
			_sortorder = sortorder;
        }

        #region ListPipeline

        public List<EntityType> PipeLines
        {
            get
            {
                return new List<EntityType>
                    {
                        EntityType.Pipeline
                    };
            }
        }

        protected Guid SiteId { get; set; }
        protected Guid PipelineId { get; set; }

        private List<PipelineDTO> _listPipeline = new List<PipelineDTO>();

        public List<PipelineDTO> ListPipeline
        {
            get { return _listPipeline; }
            set
            {
                if (SetProperty(ref _listPipeline, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region SelectedPipelineId

        private double? _kilometer;
        private Guid _selectedPipelineId;

        public Guid SelectedPipelineId
        {
            get { return _selectedPipelineId; }
            set
            {
                if (_selectedPipelineId == value) return;
                _selectedPipelineId = value;
                PipelineId = _selectedPipelineId;
                SaveCommand.RaiseCanExecuteChanged();
                OnPropertyChanged(() => SelectedPipelineId);
                LoadKilometers();
            }
        }

        #endregion

        #region Kilometer

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

        private double _kilometerStart;
        private double _kilometerEnd;

        private void SetValidationRules()
        {
            AddValidationFor(() => Kilometer).When(() => Kilometer < _kilometerStart || Kilometer > _kilometerEnd).
                Show(string.Format("Значение должно быть в диапозоне от {0} до {1}", _kilometerStart, _kilometerEnd));
        }

        
        private async void LoadKilometers()
        {
            ClearValidations();

            try
            {
                Behavior.TryLock();
                var pipe = await new ObjectModelServiceProxy().GetPipelineByIdAsync(_selectedPipelineId);
                if (!_firstEditLoading)
                {
                    Kilometer = pipe.KilometerOfStartPoint;
                }
                _kilometerStart = pipe.KilometerOfStartPoint;
                _kilometerEnd = pipe.KilometerOfEndPoint;
                KilometerIsEnabled = true;

                _firstEditLoading = false;
                SetValidationRules();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private bool _kilometerIsEnabled;
        public bool KilometerIsEnabled
        {
            get { return _kilometerIsEnabled; }
            set { SetProperty(ref _kilometerIsEnabled, value); }
        }

        #endregion

        protected override string CaptionEntityTypeName
        {
            get { return "пункта редуцирования"; }
        }


		protected override Task UpdateTask
		{
			get
			{
				return new ObjectModelServiceProxy().EditReducingStationAsync(
					new EditReducingStationParameterSet
					{
						ReducingStationId = Model.Id,
						SiteId = SiteId,
						MainPipelineId = PipelineId,
						Kilometer = Kilometer.Value,
						Name = Name
					});
			}
		}

		protected override Task<Guid> CreateTask
		{
			get
			{
				return new ObjectModelServiceProxy().AddReducingStationAsync(
					new AddReducingStationParameterSet
					{
						SiteId = SiteId,
						MainPipelineId = PipelineId,
						Kilometer = Kilometer.Value,
						Name = Name,
						SortOrder = _sortorder
					});
			}
		}

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name)
                   && Kilometer.HasValue
                   && Kilometer.Value >= 0
                   && SiteId != Guid.Empty
                   && PipelineId != Guid.Empty;
        }
    }
}