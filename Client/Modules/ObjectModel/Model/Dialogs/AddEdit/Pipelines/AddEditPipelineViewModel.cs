using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineEndType;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.PipelineConns;
using GazRouter.DTO.ObjectModel.Pipelines;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines
{
    public class AddEditPipelineViewModel : AddEditLinearObjectViewModel<PipelineDTO, Guid>
    {
        private readonly PipelineTypesDTO _newPipelineType;
        private readonly int _sortorder;
        private readonly int _gasTransportSystemId;

        public AddEditPipelineViewModel(Action<Guid> actionBeforeClosing, PipelineDTO model)
            : base(actionBeforeClosing, model)
        {
            _newPipelineType =
                ClientCache.DictionaryRepository.PipelineTypes[model.Type];
            Name = model.Name;
            KilometerOfStartPoint = model.KilometerOfStartPoint;
            KilometerOfEndPoint = model.KilometerOfEndPoint;
            KilometerOfStartPointConn = model.KilometerOfBeginConn;
            KilometerOfEndPointConn = model.KilometerOfEndConn;

            StartEntityId = model.BeginEntityId;
            EndEntityId = model.EndEntityId;

            InitAllowedTypes(model.Type);

            SetValidationRules();
        }

		public AddEditPipelineViewModel(Action<Guid> actionBeforeClosing, PipelineType newPipelineType, PipelineDTO parentPipeline, int gasTransportSystemId, int sortorder)
            : base(actionBeforeClosing)
        {

            if (newPipelineType == PipelineType.Main )
                throw new ArgumentOutOfRangeException("newPipelineType", "Нельзя создавать магистральный газопровод");

		    _newPipelineType =
		        ClientCache.DictionaryRepository.PipelineTypes[newPipelineType];
		    _gasTransportSystemId = gasTransportSystemId;
			_sortorder = sortorder;

		    if (newPipelineType == PipelineType.CompressorShopOutlet || newPipelineType == PipelineType.Inlet)
		    {
		        EndEntity = parentPipeline;
		    }
            else if (newPipelineType == PipelineType.Booster)
            {
                StartEntity = parentPipeline;
                EndEntity = parentPipeline;
            }
		    else
		    {
		        StartEntity = parentPipeline;
		    }

            InitAllowedTypes(newPipelineType);
            
            SetValidationRules();
        }

        private void InitAllowedTypes(PipelineType newPipelineType)
        {
            switch (newPipelineType)
            {
                case PipelineType.Main:
                case PipelineType.Looping:
                case PipelineType.Bridge:
                case PipelineType.Booster:
                    StartAllowedTypes = new List<EntityType>
                        {
                            EntityType.Pipeline
                        };
                    EndAllowedTypes = new List<EntityType>
                        {
                            EntityType.Pipeline
                        };
                    break;
                case PipelineType.Distribution:
                    StartAllowedTypes = new List<EntityType>
                        {
                            EntityType.Pipeline
                        };
                    EndAllowedTypes = new List<EntityType>
                        {
                            EntityType.Pipeline,
                            EntityType.DistrStation
                        };
                    break;
                case PipelineType.Branch:
                    StartAllowedTypes = new List<EntityType>
                        {
                            EntityType.Pipeline
                        };
                    EndAllowedTypes = new List<EntityType>
                        {
                            EntityType.DistrStation
                        };
                    break;
                case PipelineType.Inlet:
                    StartAllowedTypes = new List<EntityType>();
                    EndAllowedTypes = new List<EntityType>
                        {
                            EntityType.Pipeline
                        };
                    break;
                case PipelineType.CompressorShopInlet:
                    StartAllowedTypes = new List<EntityType>
                        {
                            EntityType.Pipeline
                        };
                    EndAllowedTypes = new List<EntityType>
                        {
                            EntityType.CompShop
                        };
                    break;
                case PipelineType.CompressorShopOutlet:
                    StartAllowedTypes = new List<EntityType>
                        {
                            EntityType.CompShop
                        };
                    EndAllowedTypes = new List<EntityType>
                        {
                            EntityType.Pipeline
                        };
                    break;
                case PipelineType.CompressorShopBridge:
                    StartAllowedTypes = new List<EntityType>
                        {
                            EntityType.CompShop
                        };
                    EndAllowedTypes = new List<EntityType>
                        {
                            EntityType.CompShop
                        };
                    break;
                case PipelineType.RefiningDeviceChamber:
                    StartAllowedTypes = new List<EntityType>();
                    EndAllowedTypes = new List<EntityType>
                        {
                            EntityType.Pipeline
                        };
                    break;
            }
        }

        protected override Task UpdateTask
        {
            get
            {
                var parameters = new EditPipelineWithConnsParameterSet
                {
                    PipelineParameters = new EditPipelineParameterSet
                    {
                        Id = Model.Id,
                        PipelineTypeId = _newPipelineType.PipelineType,
                        Name = Name,
                        KilometerOfEnd = KilometerOfEndPoint.Value,
                        KilometerOfStart = KilometerOfStartPoint.Value,
                        GasTransportSystemId = Model.SystemId
                    },
                    StartConnParameters = StartEntity == null
                        ? null
                        : new AddPipelineConnParameterSet
                        {
                            EndTypeId = PipelineEndType.StartType,
                            DestEntityId = StartEntity.Id,
                            Kilometr =
                                StartEntity.EntityType != EntityType.CompShop
                                    ? KilometerOfStartPointConn.Value
                                    : (double?) null
                        },
                    EndConnParameters = EndEntity == null
                        ? null
                        : new AddPipelineConnParameterSet
                        {
                            EndTypeId = PipelineEndType.EndType,
                            DestEntityId = EndEntity.Id,
                            Kilometr =
                                EndEntity.EntityType != EntityType.DistrStation
                                    ? KilometerOfEndPointConn.Value
                                    : (double?) null
                        }
                };
                return new ObjectModelServiceProxy().EditPipelineAsync(parameters);
            }
        }

        protected override Task<Guid> CreateTask
        {
            get
            {
                var parameters = new AddPipelineWithConnsParameterSet
                {
                    PipelineParameters = new AddPipelineParameterSet
                    {
                        PipelineTypeId = _newPipelineType.PipelineType,
                        Name = Name,
                        KilometerOfEnd  = KilometerOfEndPoint.Value,
                        KilometerOfStart = KilometerOfStartPoint.Value,
                        GasTransportSystemId = _gasTransportSystemId,
                        SortOrder = _sortorder
                    },
                    StartConnParameters = StartEntity == null
                                              ? null
                                              : new AddPipelineConnParameterSet
                                              {
                                                  EndTypeId = PipelineEndType.StartType,
                                                  DestEntityId = StartEntity.Id,
                                                  Kilometr = StartEntity.EntityType != EntityType.CompShop ? KilometerOfStartPointConn.Value : (double?)null
                                              },
                    EndConnParameters = EndEntity == null
                                            ? null
                                            : new AddPipelineConnParameterSet
                                            {
                                                EndTypeId = PipelineEndType.EndType,
                                                DestEntityId = EndEntity.Id,
                                                Kilometr = EndEntity.EntityType != EntityType.DistrStation ? KilometerOfEndPointConn.Value : (double?)null
                                            }

                };
                return new ObjectModelServiceProxy().AddPipelineAsync(parameters);
            }
        }

        protected override string CaptionEntityTypeName
        {
            get { return string.Format("объекта {0}", _newPipelineType.Name); }
        }

        #region SaveCommand

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name)
                   && KilometerOfStartPoint.HasValue
                   && KilometerOfStartPoint.Value >= 0
                   && KilometerOfEndPoint.HasValue
                   && KilometerOfEndPoint.Value > 0;
        }

        #endregion SaveCommand

        #region SetValidationRules

        public void SetValidationRules()
        {
            AddValidationFor(() => KilometerOfStartPoint).When(() => KilometerOfStartPoint < 0 || KilometerOfStartPoint > 10000).Show("Значение должно быть в диапозоне от 0 до 10000");
            AddValidationFor(() => KilometerOfStartPoint)
                .When(() => KilometerOfStartPoint > KilometerOfEndPoint)
                .Show("Км. начала не может быть больше км. конца.");
            AddValidationFor(() => KilometerOfEndPoint).When(() => KilometerOfEndPoint < 0 || KilometerOfEndPoint > 10000).Show("Значение должно быть в диапозоне от 0 до 10000");
            AddValidationFor(() => KilometerOfEndPoint)
                .When(() => KilometerOfStartPoint > KilometerOfEndPoint)
                .Show("Км. начала не может быть больше км. конца.");
        }

        #endregion

        private CommonEntityDTO _startEntity;
        public CommonEntityDTO StartEntity
        {
            get { return _startEntity; }
            set
            {
                _startEntity = value;
                OnPropertyChanged(() => StartEntity);
                OnPropertyChanged(() => StartKilometrConnIsEnabled);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private CommonEntityDTO _endEntity;
        public CommonEntityDTO EndEntity
        {
            get { return _endEntity; }
            set
            {
                _endEntity = value;
                OnPropertyChanged(() => EndEntity);
                OnPropertyChanged(() => EndKilometrConnIsEnabled);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private Guid? _startEntityId;
        public Guid? StartEntityId
        {
            get { return _startEntityId; }
            set
            {
                _startEntityId = value;
                OnPropertyChanged(() => StartEntityId);
            }
        }

        private Guid? _endEntityId;
        public Guid? EndEntityId
        {
            get { return _endEntityId; }
            set
            {
                _endEntityId = value;
                OnPropertyChanged(() => EndEntityId);
            }
        }

        public List<EntityType> StartAllowedTypes { get; private set; }

        public List<EntityType> EndAllowedTypes { get; private set; }

        #region KilometerOfStart

        public override sealed double? KilometerOfStartPoint
        {
            get { return base.KilometerOfStartPoint; }
            set
            {
                if (base.KilometerOfStartPoint == value)
                    return;

                base.KilometerOfStartPoint = value;
                if (_newPipelineType.PipelineType == PipelineType.Booster)
                {
                    _kilometerOfStartConn = KilometerOfStartPoint;
                    OnPropertyChanged(() => KilometerOfStartPointConn);
                }
                OnPropertyChanged(() => KilometerOfEndPoint);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private double? _kilometerOfStartConn = 0;
        public double? KilometerOfStartPointConn
        {
            get { return _kilometerOfStartConn; }
            set
            {
                if (SetProperty(ref _kilometerOfStartConn, value))
                {
                    OnPropertyChanged(() => KilometerOfEndPointConn);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region KilometerOfEnd

        public override sealed double? KilometerOfEndPoint
        {
            get { return base.KilometerOfEndPoint; }
            set
            {
                if (base.KilometerOfEndPoint == value)
                    return;

                base.KilometerOfEndPoint = value;
                if (_newPipelineType.PipelineType == PipelineType.Booster)
                {
                    _kilometerOfEndConn = KilometerOfEndPoint;
                    OnPropertyChanged(() => KilometerOfEndPointConn);
                }
                OnPropertyChanged(() => KilometerOfStartPoint);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private double? _kilometerOfEndConn = 0;
        public double? KilometerOfEndPointConn
        {
            get { return _kilometerOfEndConn; }
            set
            {
                if (SetProperty(ref _kilometerOfEndConn, value))
                {
                    OnPropertyChanged(() => KilometerOfStartPointConn);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion


        public bool StartKilometrConnIsEnabled
        {
            get { return StartEntity != null; }
        }

        public bool EndKilometrConnIsEnabled
        {
            get { return EndEntity != null; }
        }

        public bool IsMainPipeline
        {
            get { return _newPipelineType.PipelineType == PipelineType.Main; }
        }
    }
}