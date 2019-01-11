using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.BoilerTypes;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Boilers;
using GazRouter.DTO.ObjectModel.Pipelines;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.Boiler
{
	public class AddEditBoilerViewModel : AddEditViewModelBase<BoilerDTO, Guid>
    {
		protected override string CaptionEntityTypeName
        {
            get { return "котел"; }
        }

		
		public bool ParentIsPipeline
		{
			get { return _parentEntity.EntityType == EntityType.Pipeline; }
		}


        private readonly CommonEntityDTO _parentEntity;
        private bool _isSmall;
        private double _kilometer;
        private string _boilerGroup;
        private BoilerTypeDTO _boilerType;
        private double _heatLossFactor;
        private double _heatSupplySystemLoad;
        
        
		
        /// <summary>
        /// Километр установки котла на газопроводе
        /// (используется только при добавлении котла на газопровод)
        /// </summary>
		public double Kilometer
        {
			get { return _kilometer; }
            set
            {
				if (SetProperty(ref _kilometer, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }


        /// <summary>
        /// Список групп котлоагрегатов
        /// </summary>
        public List<string> BoilerGroupList
        {
            get
            {
                return ClientCache.DictionaryRepository.BoilerTypes
                    .Where(b => b.IsSmall == _isSmall)
                    .Select(g => g.GroupName)
                    .Distinct()
                    .ToList();
            }
        }

        
        /// <summary>
        /// Выбранная группа котлоагрегатов
        /// </summary>
        public string BoilerGroup
        {
            get { return _boilerGroup; }
            set
            {
                if (SetProperty(ref _boilerGroup, value))
                {
                    OnPropertyChanged(() => BoilerGroup);
                    OnPropertyChanged(() => BoilerTypeList);
                }
            }
        }


        /// <summary>
        /// Список типов котлоагрегатов
        /// </summary>
        public List<BoilerTypeDTO> BoilerTypeList
        {
            get { return ClientCache.DictionaryRepository.BoilerTypes.Where(b => b.GroupName == BoilerGroup && b.IsSmall == _isSmall).ToList(); }

        }
		
        /// <summary>
        /// Выбранный тип котлоагрегата
        /// </summary>
		public BoilerTypeDTO BoilerType
		{
			get { return _boilerType; }
			set
			{
				if (SetProperty(ref _boilerType, value))
				    SaveCommand.RaiseCanExecuteChanged();
			}
		}

        /// <summary>
        /// Котел малой мощности
        /// </summary>
	    public bool IsSmall
	    {
	        get { return _isSmall; }
	    }
        
        
        /// <summary>
        /// Коэф. внутрикотельных потерь
        /// </summary>
	    public double HeatLossFactor
	    {
	        get { return _heatLossFactor; }
            set { SetProperty(ref _heatLossFactor, value); }
	    }
        

        /// <summary>
        /// Присоединенная нагрузка системы теплоснабжения, Гкал/ч
        /// </summary>
        public double HeatSupplySystemLoad
        {
            get { return _heatSupplySystemLoad; }
            set { SetProperty(ref _heatSupplySystemLoad, value); }
        }



        

        public AddEditBoilerViewModel(Action<Guid> actionBeforeClosing, CommonEntityDTO parentEntity)
            : base(actionBeforeClosing)
        {
            
            _parentEntity = parentEntity;
            _isSmall = parentEntity.EntityType != EntityType.BoilerPlant;
			
            SetValudationRules();
        }

        public AddEditBoilerViewModel(Action<Guid> actionBeforeClosing, BoilerDTO boilerDto, CommonEntityDTO parentEntity)
            : base(actionBeforeClosing, boilerDto)
		{
            _parentEntity = parentEntity;
            _isSmall = parentEntity.EntityType != EntityType.BoilerPlant;

            Name = boilerDto.Name;
            Kilometer = boilerDto.Kilometr;

            BoilerType = ClientCache.DictionaryRepository.BoilerTypes.Single(t => t.Id == boilerDto.BoilerTypeId);
            BoilerGroup = BoilerType.GroupName;
            
            
            HeatLossFactor = boilerDto.HeatLossFactor;
            HeatSupplySystemLoad = boilerDto.HeatSupplySystemLoad;
            

            SetValudationRules();
		}



		protected override Task UpdateTask
		{
			get
			{
				var paramSet = new EditBoilerParameterSet
					               {
						               Id = Model.Id,
						               Name = Name,
						               BoilerTypeId = _boilerType.Id,
						               Kilometer = _kilometer,
						               HeatLossFactor = _heatLossFactor,
						               HeatSupplySystemLoad = _heatSupplySystemLoad
					               };
				switch (_parentEntity.EntityType)
				{
					case EntityType.BoilerPlant:
						paramSet.BoilerPlantId = _parentEntity.Id;
						break;
					case EntityType.DistrStation:
						paramSet.DistStationId = _parentEntity.Id;
						break;
					case EntityType.MeasStation:
						paramSet.MeasStationId = _parentEntity.Id;
						break;
					default:
						paramSet.PipelineId = _parentEntity.Id;
						break;
				}

				return new ObjectModelServiceProxy().EditBoilerAsync(paramSet);
			}

		}

		protected override Task<Guid> CreateTask
        {
			get
			{
				var paramSet = new AddBoilerParameterSet
					               {
						               Name = Name,
						               BoilerTypeId = _boilerType.Id,
						               Kilometer = _kilometer,
						               HeatLossFactor = _heatLossFactor,
						               HeatSupplySystemLoad = _heatSupplySystemLoad
					               };

				switch (_parentEntity.EntityType)
				{
					case EntityType.BoilerPlant:
						paramSet.BoilerPlantId = _parentEntity.Id;
						break;
					case EntityType.DistrStation:
						paramSet.DistStationId = _parentEntity.Id;
						break;
					case EntityType.MeasStation:
						paramSet.MeasStationId = _parentEntity.Id;
						break;
					default:
						paramSet.PipelineId = _parentEntity.Id;
						break;
				}
				return new ObjectModelServiceProxy().AddBoilerAsync(paramSet);
			}
        }

		protected override bool OnSaveCommandCanExecute()
		{
            ValidateAll();
			return !HasErrors;
		}

	    private void SetValudationRules()
	    {
            AddValidationFor(() => BoilerGroup)
                .When(() => BoilerGroup == null)
                .Show("Не выбрана группа");

            AddValidationFor(() => BoilerType)
                .When(() => BoilerType == null)
                .Show("Не выбран тип");

            AddValidationFor(() => Name)
                .When(() => string.IsNullOrEmpty(Name))
                .Show("Не указано наименование");


            var t1 = _parentEntity as PipelineDTO;
            if (t1 != null)
            {
                AddValidationFor(() => Kilometer)
                    .When(() => Kilometer < t1.KilometerOfStartPoint || Kilometer >  t1.KilometerOfEndPoint)
                    .Show(string.Format("Недопустимое значение (интервал допустимых значений от {0} до {1})", t1.KilometerOfStartPoint, t1.KilometerOfEndPoint));
            }

            AddValidationFor(() => HeatLossFactor)
                .When(() => !_isSmall && (HeatLossFactor < 0 || HeatLossFactor > 0.1))
                .Show("Недопустимое значение (интервал допустимых значений от 0 до 0.1)");
            

            AddValidationFor(() => HeatSupplySystemLoad)
                .When(() => !_isSmall && (HeatSupplySystemLoad < 0 || HeatSupplySystemLoad > 10))
                .Show("Недопустимое значение (интервал допустимых значений от 0 до 10)");
	    }
	}
    
}