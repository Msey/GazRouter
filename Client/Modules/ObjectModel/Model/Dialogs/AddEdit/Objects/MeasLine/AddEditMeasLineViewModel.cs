using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.MeasLine;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.MeasLine
{
    public class AddEditMeasLineViewModel : AddEditViewModelBase<MeasLineDTO>
    {
        private readonly Guid _parentId;
        private bool _firstEditLoading;

        public AddEditMeasLineViewModel(Action<Guid> actionBeforeClosing, MeasLineDTO model)
            : base(actionBeforeClosing, model)
        {
            _firstEditLoading = true;
            KilometerIsEnabled = true;
            _parentId = model.ParentId.Value;
			Id = model.Id;
			KmOfConn = model.KmOfConn;
			Name = model.Name;
            BalanceName = model.BalanceName;
            GetSelectedPipeline();
        }

		public AddEditMeasLineViewModel(Action<Guid> actionBeforeClosing, Guid parentId, int sortorder)
            : base(actionBeforeClosing)
        {
            _parentId = parentId;
			_sortorder = sortorder;
        }

		private int _sortorder;

        private async void GetSelectedPipeline()
        {
            try
            {
                Behavior.TryLock();
                SelectedPipeLine = await new ObjectModelServiceProxy().GetEntityByIdAsync(Model.PipelineId);
            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }
        

        protected override string CaptionEntityTypeName
        {
            get { return "замерной линии ГИС"; }
        }

        #region SaveCommand

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name);
        }

        #endregion SaveCommand

        #region KmOfConn

        private double _kmOfConn;

        public double KmOfConn
        {
            get { return _kmOfConn; }
            set
            {
                if (SetProperty(ref _kmOfConn, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private double _kilometerStart;
        private double _kilometerEnd;

        private void SetValidationRules()
        {
            AddValidationFor(() => KmOfConn).When(() => KmOfConn < _kilometerStart || KmOfConn > _kilometerEnd).
                Show(string.Format("Значение должно быть в диапозоне от {0} до {1}", _kilometerStart, _kilometerEnd));
        }

        private async void LoadKilometers()
        {
            ClearValidations();
            try
            {
                Behavior.TryLock();
                var pipe = await new ObjectModelServiceProxy().GetPipelineByIdAsync(_selectedPipeline.Id);
                if (!_firstEditLoading)
                {
                    KmOfConn = pipe.KilometerOfStartPoint;
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

        public string BalanceName { get; set; }

        protected override Task UpdateTask
		{
			get
			{
				return new ObjectModelServiceProxy().EditMeasLineAsync(
					new EditMeasLineParameterSet
					{
						Id = Model.Id,
						Name = Name,
                        BalanceName = BalanceName,
						ParentId = _parentId,
						PipelineId = SelectedPipeLine.Id,
						KmOfConn = KmOfConn
					});
			}
		}

        protected override Task<Guid> CreateTask
		{
			get
			{
				return new ObjectModelServiceProxy().AddMeasLineAsync(
					new AddMeasLineParameterSet
					{
						Name = Name,
                        BalanceName = BalanceName,
                        ParentId = _parentId,
						PipelineId = SelectedPipeLine.Id,
						KmOfConn = KmOfConn,
						SortOrder = _sortorder
					});
			}
		}

        public List<EntityType> AllowedType
        {
            get
            {
                return new List<EntityType>
                {
                    EntityType.Pipeline
                };
            }
        }

        #region SelectedDestination

        private CommonEntityDTO _selectedPipeline;

        public CommonEntityDTO SelectedPipeLine
        {
            get { return _selectedPipeline; }
            set
            {
                if (SetProperty(ref _selectedPipeline, value))
                {
                    LoadKilometers();
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion
    }
}