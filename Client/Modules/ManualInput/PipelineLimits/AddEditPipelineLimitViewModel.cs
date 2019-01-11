using GazRouter.Common.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using GazRouter.ManualInput.CompUnitTests.ChartDigitizer;
using GazRouter.DTO.ManualInput.PipelineLimits;
using GazRouter.Application;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.Application.Helpers;
using Utils.Units;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.ManualInput.PipelineLimits
{
    public class AddEditPipelineLimitViewModel : AddEditViewModelBase<PipelineLimitDTO, int>
    {
        private readonly int _id;
        private readonly Guid _pipelineId;
        private string _description;
        private double? _begin;
        private double? _end;
        private double? _maxvalue;
        private double? _minvalue;
        private Pressure _pressure;

        public string PressureMessage => $"Максимальное давление, {UserProfile.UserUnitName(PhysicalType.Pressure)}:";

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

        #region constructors
        public AddEditPipelineLimitViewModel(Action<int> closeCallback, PipelineLimitDTO model)
            : base(closeCallback, model)
        {
            _isAdd = false;
            _id = Model.Id;
            _pipelineId = Model.PipelineId;
            _begin = Model.Begin;
            _end = Model.End;
            _maxvalue = Model.MaxAllowableKm;
            _minvalue = Model.MinAllowableKm;
            _pressure = Model.MaxPressure;
            _description = Model.Description;

            LoadPipelines();     
        }

        public AddEditPipelineLimitViewModel(Action<int> closeCallback, Guid unitId)
            : base(closeCallback)
        {
            _isAdd = true;
            _pipelineId = unitId;
            
            LoadPipelines();
        }

        #endregion
        #region Properties
        
        private bool _isAdd;
        public bool IsAdd
        {
            get { return _isAdd; }
            set
            {
                SetProperty(ref _isAdd, value);
            }
        }

        private CommonEntityDTO _selectedPipeline;

        public CommonEntityDTO SelectedPipeline
        {
            get { return _selectedPipeline; }
            set
            {
                if (SetProperty(ref _selectedPipeline, value))
                {
                    LoadPipelines();
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public List<EntityType> AllowedType => new List<EntityType>
        {
            EntityType.Pipeline
        };

        /// <summary>
        /// Описание распоряжения
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                if (SetProperty(ref _description, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }


        /// <summary>
        /// Километр начала
        /// </summary>
        public double? Begin
        {
            get { return _begin; }
            set
            {
                if (SetProperty(ref _begin, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Километр конца
        /// </summary>
        public double? End
        {
            get { return _end; }
            set
            {
                if (SetProperty(ref _end, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Максимальное давление
        /// </summary>
        public double MaxPressure
        {
            get { return UserProfile.ToUserUnits(_pressure.Kgh, PhysicalType.Pressure); }
            set
            {
                if (SetProperty(ref _pressure, Pressure.FromKgh(UserProfile.ToServerUnits(value, PhysicalType.Pressure))))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion
        
        /// <summary>
        /// Добавление нового испытания
        /// </summary>
        protected override Task<int> CreateTask
        {
            get
            {
                var pSet = new AddPipelineLimitParameterSet
                {
                    PipelineId = SelectedPipeline.Id,
                    Begin = Begin,
                    End = End,
                    MaxPressure = _pressure.Kgh,
                    Description = Description,
                    UserName = UserProfile.Current.Login
                };
                
                return new ManualInputServiceProxy().AddPipelineLimitAsync(pSet);
            }
        }

        /// <summary>
        /// Редактирование
        /// </summary>
        protected override Task UpdateTask
        {
            get
            {
                var pSet = new EditPipelineLimitParameterSet
                {
                    EntityId = _id,
                    PipelineId = Model.PipelineId,
                    Begin = Begin,
                    End = End,
                    MaxPressure = _pressure.Kgh,
                    Description = Description,
                    UserName = UserProfile.Current.Login
                };
                return new ManualInputServiceProxy().EditPipelineLimitAsync(pSet);
            }
        }


        private async void LoadPipelines()
        {
            if (_selectedPipeline == null && _pipelineId == Guid.Empty) return;
            try
            {
                Behavior.TryLock();
                Guid pipe_id = _selectedPipeline != null ? _selectedPipeline.Id : _pipelineId;

                var pipeline = await new ObjectModelServiceProxy().GetPipelineByIdAsync(pipe_id);
                _minvalue = pipeline.KilometerOfStartPoint;
                _maxvalue = pipeline.KilometerOfEndPoint;
                Begin = _minvalue;
                End = _maxvalue;
                if (SelectedPipeline == null)
                {
                    SelectedPipeline = pipeline;
                    OnPropertyChanged(() => SelectedPipeline);
                }
            }
            finally
            {
                ClearValidations();
                SetValidationRules();
                ValidateAll();

                Behavior.TryUnlock();
            }
        }
        protected void SetValidationRules()
        {
            AddValidationFor(() => Begin)
                .When(() => SelectedPipeline != null && Begin < 0)
                .Show("Некорректное значение");
            AddValidationFor(() => End)
                .When(() => SelectedPipeline != null && End < 0)
                .Show("Некорректное значение");

            AddValidationFor(() => Begin)
                .When(() => Begin < _minvalue || Begin > _maxvalue)
                .Show(string.Format("Значение не соответствует километражу газопровода ( {0} - {1} км.)", _minvalue, _maxvalue));
            AddValidationFor(() => End)
                .When(() => End < _minvalue || End > _maxvalue)
                .Show(string.Format("Значение не соответствует километражу газопровода ( {0} - {1} км.)", _minvalue, _maxvalue));

            AddValidationFor(() => End)
                .When(() => End < Begin)
                .Show("Километр конца участка должен быть больше километра начала");

            AddValidationFor(() => MaxPressure)
                .When(() => _pressure < ValueRangeHelper.PressureRange.Min || _pressure > ValueRangeHelper.PressureRange.Max)
                .Show(ValueRangeHelper.PressureRange.ViolationMessage);
        }


        public DelegateCommand DigitizeCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var vm = new ChartDigitizerViewModel(null);
                    var v = new ChartDigitizerView { DataContext = vm };
                    v.ShowDialog();
                });
            }
        }
        protected override string CaptionEntityTypeName
        {
            get { return " ограничения по давлению"; }
        }

        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }
    }
}