using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PowerUnitTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.PowerUnits;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.PowerUnit
{
	public class AddEditPowerUnitViewModel : AddEditViewModelBase<PowerUnitDTO, Guid>
    {
		protected override string CaptionEntityTypeName
        {
            get { return "электроагрегата"; }
        }


        private string _entityName;
        /// <summary>
        /// Наименование электроагрегата
        /// </summary>
        public string EntityName
        {
            get { return _entityName; }
			set 
            { 
                if (SetProperty(ref _entityName, value))
                    SaveCommand.RaiseCanExecuteChanged(); 
            }
        }

        
		
		private double _kilometer;
        /// <summary>
        /// Километр установки электроагрегата на газопроводе 
        /// (задается только при установке электроагрегата на газопровод)
        /// </summary>
		public double Kilometer
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

        

		
        /// <summary>
        /// Список типов двигателей
        /// </summary>
        public Dictionary<string, EngineGroup> EngineGroupList => new Dictionary<string, EngineGroup>
        {
            {"Газотурбинный", EngineGroup.Turbine},
            {"Газопоршневой", EngineGroup.Reciprocating}
        };

	    /// <summary>
        /// Выбранный тип двигателя
        /// </summary>
	    public EngineGroup EngineGroup
	    {
	        get { return _engineGroup; }
	        set
	        {
                _engineGroup = value;
                OnPropertyChanged(() => PowerUnitTypeList);
	            PowerUnitType = null;
	        }
	    }
        
	    

        /// <summary>
        /// Список типов агрегатов, зависит от выбранного типа двигателя
        /// </summary>
		public List<PowerUnitTypeDTO> PowerUnitTypeList
		{
			get { return ClientCache.DictionaryRepository.PowerUnitTypes.Where(pu => pu.EngineGroup == _engineGroup).ToList(); }
		}


        private PowerUnitTypeDTO _powerUnitType;
        /// <summary>
        /// Выбранный тип агрегата
        /// </summary>
        public PowerUnitTypeDTO PowerUnitType
        {
            get { return _powerUnitType; }
            set
            {
                if (SetProperty(ref _powerUnitType, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

		

	    private double _turbineConsumption;
        /// <summary>
        /// Расход газа на работу турбодетандера, м3/с
        /// </summary>
        public double TurbineConsumption
        {
            get { return _turbineConsumption; }
            set
            {
                if (SetProperty(ref _turbineConsumption, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private int _turbineRuntime;
        /// <summary>
        /// Время работы турбодетандера, с
        /// </summary>
        public int TurbineRuntime
        {
            get { return _turbineRuntime; }
            set
            {
                if (SetProperty(ref _turbineRuntime, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        private double _operatingTimeFactor;
        /// <summary>
        /// Коэф. учитывающий наработку агрегата 
        /// (1 - при наработке до проведения кап. ремонта, 1.02 - при наработке после кап. ремонта)
        /// </summary>
        public bool OperatingTimeFactor
        {
            get { return _operatingTimeFactor == 1.02; }
            set
            {
                var k = value ? 1.02 : 1;
                if (SetProperty(ref _operatingTimeFactor, k))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool ParentIsPipeline => _parentDto.EntityType == EntityType.Pipeline;


	    private readonly CommonEntityDTO _parentDto;

        public AddEditPowerUnitViewModel(Action<Guid> actionBeforeClosing,
            CommonEntityDTO parentDto)
            : base(actionBeforeClosing)
        {
            _parentDto = parentDto;
	        _engineGroup = EngineGroup.Turbine;
            _operatingTimeFactor = 1;

			SetValidationRules();
        }

		public AddEditPowerUnitViewModel(Action<Guid> actionBeforeClosing, PowerUnitDTO powerUnitDto, CommonEntityDTO parentDto)
            : base(actionBeforeClosing, powerUnitDto)
		{
            _parentDto = parentDto;
			
		    _engineGroup = ClientCache.DictionaryRepository.PowerUnitTypes.FirstOrDefault(pu => pu.Id == powerUnitDto.PowerUnitTypeId).EngineGroup;
            _powerUnitType = ClientCache.DictionaryRepository.PowerUnitTypes.FirstOrDefault(t => t.Id == powerUnitDto.PowerUnitTypeId);
            _entityName = powerUnitDto.Name;
            _kilometer = powerUnitDto.Kilometr;
		    _operatingTimeFactor = powerUnitDto.OperatingTimeFactor;
		    _turbineConsumption = powerUnitDto.TurbineConsumption;
		    _turbineRuntime = powerUnitDto.TurbineRuntime;

            SetValidationRules();
            
		}

		private void SetValidationRules()
		{
            AddValidationFor(() => EntityName)
                .When(() => string.IsNullOrEmpty(EntityName))
                .Show("Не указано наименование");

            AddValidationFor(() => PowerUnitType)
                .When(() => PowerUnitType == null)
                .Show("Не выбран тип электроагрегата");

            AddValidationFor(() => TurbineConsumption)
                .When(() => TurbineConsumption < 0 || TurbineConsumption > 1.5)
                .Show("Недопустимое значение (интервал допустимых значений от 0 до 1.5)");

            AddValidationFor(() => TurbineRuntime)
                .When(() => TurbineRuntime < 0 || TurbineRuntime > 600)
                .Show("Недопустимое значение (интервал допустимых значений от 0 до 600)");


            var t1 = _parentDto as PipelineDTO;
			if (t1 != null)
			{
                AddValidationFor(() => Kilometer)
					.When(() => Kilometer < t1.KilometerOfStartPoint || Kilometer > t1.KilometerOfEndPoint)
					.Show(string.Format("Недопустимое значение (интервал допустимых значений от {0} до {1})", t1.KilometerOfStartPoint, t1.KilometerOfEndPoint));
			}
		}

		private EngineGroup _engineGroup;

		protected override Task UpdateTask
		{
			get
			{
				var paramSet = new EditPowerUnitParameterSet
				{
					Id = Model.Id,
					Name = _entityName,
					PowerUnitTypeId = _powerUnitType.Id,
					Kilometer = _kilometer,
					OperatingTimeFactor = _operatingTimeFactor,
					TurbineConsumption = _turbineConsumption,
					TurbineRuntime = _turbineRuntime
				};
				switch (_parentDto.EntityType)
				{
					case EntityType.CompStation:
						paramSet.PowerPlantId = _parentDto.Id;
						break;
					default:
						paramSet.PipelineId = _parentDto.Id;
						break;
				}
				return new ObjectModelServiceProxy().EditPowerUnitAsync(
					paramSet);
			}
		}

		protected override Task<Guid> CreateTask
		{
			get
			{
				var paramSet = new AddPowerUnitParameterSet
				{
					Name = _entityName,
					PowerUnitTypeId = _powerUnitType.Id,
					Kilometer = _kilometer,
					OperatingTimeFactor = _operatingTimeFactor,
					TurbineConsumption = _turbineConsumption,
					TurbineRuntime = _turbineRuntime
				};

				if (_parentDto.EntityType == EntityType.PowerPlant)
				{
					paramSet.PowerPlantId = _parentDto.Id;
				}
				else if (_parentDto.EntityType == EntityType.Pipeline)
				{
					paramSet.PipelineId = _parentDto.Id;
				}
				return new ObjectModelServiceProxy().AddPowerUnitAsync(
					paramSet);
			}
		}

		protected override bool OnSaveCommandCanExecute()
		{
            ValidateAll();
			return !HasErrors;
		}
	}
}