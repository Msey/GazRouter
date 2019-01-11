using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.Controls.Trends;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EngineClasses;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.Pipelines;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.CompShop
{
	public class AddEditCompShopViewModel : AddEditViewModelBase<CompShopDTO, Guid>
    {
        #region ListPipeline

        private List<PipelineDTO> _listPipeline = new List<PipelineDTO>();

        public List<PipelineDTO> ListPipeline
        {
            get { return _listPipeline; }
            set
            {
                SetProperty(ref _listPipeline, value);
            }
        }

        #endregion

        #region SelectedPipeline

        private PipelineDTO _selectedPipeline;

        public PipelineDTO SelectedPipeline
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

        #region IsVirtual

        private bool _isVirtual;

        public bool IsVirtual
        {
            get { return _isVirtual; }
            set
            {
                if (SetProperty(ref _isVirtual, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

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

        private bool _firstEditLoading;
        private double _kilometerStart;
        private double _kilometerEnd;

        private void SetValidationRules()
        {
            AddValidationFor(() => KmOfConn).When(() => KmOfConn < _kilometerStart || KmOfConn > _kilometerEnd).
                Show(string.Format("Значение должно быть в диапозоне от {0} до {1}", _kilometerStart, _kilometerEnd));
            AddValidationFor(() => PipingVolume).When(() => PipingVolume < 0 || PipingVolume > 100000).Show("Значение должно быть в диапозоне от 0 до 100000");
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

        #region PipingVolume

        private double _pipingVolume;

        public double PipingVolume
        {
            get { return _pipingVolume; }
            set
            {
                if (SetProperty(ref _pipingVolume, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private double _pipingVolumeIn;

        public double PipingVolumeIn
        {
            get { return _pipingVolumeIn; }
            set
            {
                if (SetProperty(ref _pipingVolumeIn, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private double _pipingVolumeOut;

        public double PipingVolumeOut
        {
            get { return _pipingVolumeOut; }
            set
            {
                if (SetProperty(ref _pipingVolumeOut, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region ListEngineClasses

        private List<EngineClassDTO> _listEngineClasses = new List<EngineClassDTO>();

        public List<EngineClassDTO> ListEngineClasses
        {
            get { return _listEngineClasses; }
            set
            {
                if (SetProperty(ref _listEngineClasses, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region SelectedEngineClass

        private EngineClassDTO _selectedEngineClass;

        public EngineClassDTO SelectedEngineClass
        {
            get { return _selectedEngineClass; }
            set
            {
                if (SetProperty(ref _selectedEngineClass, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region SaveCommand

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name);
        }

        #endregion SaveCommand

        private readonly Guid _stationId;

		public AddEditCompShopViewModel(Action<Guid> actionBeforeClosing, CompShopDTO compShopDto)
            : base(actionBeforeClosing, compShopDto)
        {
            _stationId = compShopDto.ParentId.Value;
            KilometerIsEnabled = true;
            _firstEditLoading = true;
			Id = compShopDto.Id;
			if (compShopDto.KmOfConn.HasValue) KmOfConn = compShopDto.KmOfConn.Value;
			Name = compShopDto.Name;
			IsVirtual = compShopDto.IsVirtual;
			if (compShopDto.PipingVolume.HasValue) PipingVolume = compShopDto.PipingVolume.Value;
            if (compShopDto.PipingVolumeIn.HasValue) PipingVolumeIn = compShopDto.PipingVolumeIn.Value;
            if (compShopDto.PipingVolumeOut.HasValue) PipingVolumeOut = compShopDto.PipingVolumeOut.Value;
            LoadPipelines();
            LoadEngineClasses();
        }

		public AddEditCompShopViewModel(Action<Guid> actionBeforeClosing, Guid compStationId, int sortorder)
            : base(actionBeforeClosing)
        {
            _stationId = compStationId;
			_sortorder = sortorder;
            LoadPipelines();
            LoadEngineClasses();
        }

		private int _sortorder;

        protected override string CaptionEntityTypeName
        {
            get { return "КЦ"; }
        }

        private async void LoadPipelines()
        {
            try
            {
                Behavior.TryLock();
                ListPipeline = await new ObjectModelServiceProxy().GetPipelineListAsync(
                    new GetPipelineListParameterSet
                    {
                        PipelineTypes = new List<PipelineType>{PipelineType.Main, PipelineType.Distribution}
                    });
                
                SelectedPipeline = ListPipeline.FirstOrDefault(p => Equals(p.Id, Model.PipelineId)) ??
                                   ListPipeline.First();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private void LoadEngineClasses()
        {
            ListEngineClasses = ClientCache.DictionaryRepository.EngineClasses;
            _selectedEngineClass = ListEngineClasses.FirstOrDefault(dto => dto.EngineClass == Model.EngineClass) ?? ListEngineClasses.First();
            OnPropertyChanged(() => ListEngineClasses);
            OnPropertyChanged(() => SelectedEngineClass);
        }

        protected override Task UpdateTask
        {
	        get
	        {
                return new ObjectModelServiceProxy().EditCompShopAsync(
                    new EditCompShopParameterSet
                    {
                        Id = Model.Id,
                        Name = Name,
                        ParentId = _stationId,
                        PipelineId = SelectedPipeline.Id,
                        KmOfConn = KmOfConn,
                        PipingVolume = PipingVolume,
                        PipingVolumeIn = PipingVolumeIn,
                        PipingVolumeOut = PipingVolumeOut,
                        IsVirtual = IsVirtual,
                        EngineClassId = SelectedEngineClass.EngineClass
                    });
	        }
        }

        protected override Task<Guid> CreateTask
        {
	        get
	        {
                return
                    new ObjectModelServiceProxy().AddCompShopAsync(
                        new AddCompShopParameterSet
                        {
                            Name = Name,
                            ParentId = _stationId,
                            PipelineId = SelectedPipeline.Id,
                            KmOfConn = KmOfConn,
                            PipingVolume = PipingVolume,
                            PipingVolumeIn = PipingVolumeIn,
                            PipingVolumeOut = PipingVolumeOut,
                            IsVirtual = IsVirtual,
                            EngineClassId = SelectedEngineClass.EngineClass,
                            SortOrder = _sortorder
                        });
	        }
        }
    }


 
}