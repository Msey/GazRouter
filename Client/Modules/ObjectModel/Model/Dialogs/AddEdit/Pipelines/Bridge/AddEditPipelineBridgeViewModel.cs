using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;
using GazRouter.Application;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineEndType;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.PipelineConns;
using GazRouter.DTO.ObjectModel.Pipelines;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines.Bridge
{
    public class AddEditPipelineBridgeViewModel : AddEditPipelineObjectViewModel<PipelineDTO, Guid>
    {
        private readonly PipelineTypesDTO _newPipelineType;
        private readonly PipelineDTO _parentPipeline;
        private readonly int _sortorder;
        private readonly int _gasTransportSystemId;

        public AddEditPipelineBridgeViewModel(Action<Guid> actionBeforeClosing, PipelineType newPipelineType, 
            PipelineDTO parentPipeline, int gasTransportSystemId, int sortorder)
            : base(actionBeforeClosing)
        {
		    _newPipelineType =
		        ClientCache.DictionaryRepository.PipelineTypes[newPipelineType];
		    _gasTransportSystemId = gasTransportSystemId;
			_sortorder = sortorder;

            _parentPipeline = parentPipeline;
            PipelineName = string.Format("{0}:", parentPipeline.Name);
            Name = "Т/п между";

            SetValidationRules();
        }

        public AddEditPipelineBridgeViewModel(Action<Guid> actionBeforeClosing, PipelineDTO model, PipelineDTO parentPipeline)
            : base(actionBeforeClosing, model)
        {
            _newPipelineType =
                ClientCache.DictionaryRepository.PipelineTypes[model.Type];
            Name = model.Name;
            KilometerOfStartPoint = model.KilometerOfStartPoint;
            KilometerOfEndPoint = model.KilometerOfEndPoint;
            KilometerOfStartPointConn = model.KilometerOfBeginConn;
            KilometerOfEndPointConn = model.KilometerOfEndConn;

            PipelineName = string.Format("{0}:", parentPipeline.Name);
            _parentPipeline = parentPipeline;
            EndEntityId = model.EndEntityId;

            if (EndEntityId.HasValue)//на данный момент в БД существуют перемычки с незаданным целевым газопроводом
                GetPipeline(EndEntityId.Value);
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
                        KilometerOfEnd = KilometerOfEndPoint.Value ,
                        KilometerOfStart = KilometerOfStartPoint.Value,
                        GasTransportSystemId = _gasTransportSystemId,
                        SortOrder = _sortorder
                    },
                    StartConnParameters = new AddPipelineConnParameterSet
                                              {
                                                  EndTypeId = PipelineEndType.StartType,
                                                  DestEntityId = _parentPipeline.Id,
                                                  Kilometr = KilometerOfStartPointConn
                                              },
                    EndConnParameters = new AddPipelineConnParameterSet
                                            {
                                                EndTypeId = PipelineEndType.EndType,
                                                DestEntityId = EndEntityId.Value,
                                                Kilometr = KilometerOfEndPointConn
                                            }

                };
                return new ObjectModelServiceProxy().AddPipelineAsync(parameters);
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
                        KilometerOfEnd = KilometerOfEndPoint.Value ,
                        KilometerOfStart = KilometerOfStartPoint.Value,
                        GasTransportSystemId = Model.SystemId
                    },
                    StartConnParameters = new AddPipelineConnParameterSet
                    {
                        EndTypeId = PipelineEndType.StartType,
                        DestEntityId = _parentPipeline.Id,
                        Kilometr = KilometerOfStartPointConn 
                    },
                    EndConnParameters = new AddPipelineConnParameterSet
                    {
                        EndTypeId = PipelineEndType.EndType,
                        DestEntityId = EndEntityId.Value,
                        Kilometr = KilometerOfEndPointConn
                    }
                };
                return new ObjectModelServiceProxy().EditPipelineAsync(parameters);
            }
        }

        protected override string CaptionEntityTypeName
        {
            get { return _newPipelineType.Name; }
        }

        #region SaveCommand

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name)
                   && KilometerOfStartPoint >= 0 
                   && KilometerOfStartPoint < KilometerOfEndPoint
                   && KilometerOfEndPoint > 0
                   && EndEntityId.HasValue
                   && KilometerOfStartPointConn >= _parentPipeline.KilometerOfStartPoint
                   && KilometerOfStartPointConn <= (_parentPipeline.KilometerOfEndPoint)
                   && _tmpPipeline != null
                   && KilometerOfEndPointConn >= _tmpPipeline.KilometerOfStartPoint
                   && KilometerOfEndPointConn <= (_tmpPipeline.KilometerOfEndPoint);
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
            AddValidationFor(() => KilometerOfStartPointConn)
                .When(() => KilometerOfStartPointConn < _parentPipeline.KilometerOfStartPoint || KilometerOfStartPointConn > (_parentPipeline.KilometerOfEndPoint))
                .Show(string.Format("Значение должно быть в диапозоне от {0} до {1}", _parentPipeline.KilometerOfStartPoint, _parentPipeline.KilometerOfEndPoint));

            if (_tmpPipeline != null)
            {
                AddValidationFor(() => KilometerOfEndPointConn)
               .When(() => KilometerOfEndPointConn < _tmpPipeline.KilometerOfStartPoint || KilometerOfEndPointConn > (_tmpPipeline.KilometerOfEndPoint))
               .Show(string.Format("Значение должно быть в диапозоне от {0} до {1}", _tmpPipeline.KilometerOfStartPoint, _tmpPipeline.KilometerOfEndPoint));
            }
        }

        #endregion

        public List<EntityType> StartAllowedTypes
        {
            get
            {
                return new List<EntityType> { EntityType.Pipeline };
            }
        }

        public string PipelineName { get; set; }

        private string _selectedPipelineName = "Газопровод не выбран";
        public string SelectedPipelineName
        {
            get { return _selectedPipelineName; }
            set { SetProperty(ref _selectedPipelineName, value); }
        }

        private PipelineDTO _tmpPipeline;
        public override sealed CommonEntityDTO EndEntity
        {
            get { return base.EndEntity; }
            set
            {
                if (base.EndEntity == value)
                    return;

                base.EndEntity = value;
                GetPipeline(EndEntity.Id);
            }
        }

        private async void GetPipeline(Guid id)
        {
            ClearValidations();

            try
            {
                Behavior.TryLock();
                _tmpPipeline = await new ObjectModelServiceProxy().GetPipelineByIdAsync(id);
                
                if (!IsEdit)
                    KilometerOfEndPointConn = _tmpPipeline.KilometerOfStartPoint;
                IsEnabledKilometerOfEndPointConn = true;
                SelectedPipelineName = string.Format("{0}:", _tmpPipeline.Name);
                SelectedPipelineNameBrush = new SolidColorBrush(Colors.Black);

                SetValidationRules();
                SaveCommand.RaiseCanExecuteChanged();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
        

        private bool _isEnabledKilometerOfEndPointConn;
        public bool IsEnabledKilometerOfEndPointConn
        {
            get { return _isEnabledKilometerOfEndPointConn; }
            set { SetProperty(ref _isEnabledKilometerOfEndPointConn, value); }
        }

        private Brush _selectedPipelineNameBrush = new SolidColorBrush(Colors.Black);
        public Brush SelectedPipelineNameBrush
        {
            get { return _selectedPipelineNameBrush; }
            set { SetProperty(ref _selectedPipelineNameBrush, value); }
        }
    }
}