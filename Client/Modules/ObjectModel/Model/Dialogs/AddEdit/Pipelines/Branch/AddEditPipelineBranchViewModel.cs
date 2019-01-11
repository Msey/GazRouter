using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineEndType;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.PipelineConns;
using GazRouter.DTO.ObjectModel.Pipelines;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines.Branch
{
    public class AddEditPipelineBranchViewModel : AddEditPipelineObjectViewModel<PipelineDTO, Guid>
    {
        private readonly PipelineTypesDTO _newPipelineType;
        private readonly PipelineDTO _parentPipeline;
        private readonly int _sortorder;
        private readonly int _gasTransportSystemId;

        public AddEditPipelineBranchViewModel(Action<Guid> actionBeforeClosing, PipelineType newPipelineType, 
            PipelineDTO parentPipeline, int gasTransportSystemId, int sortorder)
            : base(actionBeforeClosing)
        {
		    _newPipelineType =
		        ClientCache.DictionaryRepository.PipelineTypes[newPipelineType];
		    _gasTransportSystemId = gasTransportSystemId;
			_sortorder = sortorder;

            _parentPipeline = parentPipeline;
            PipelineName = string.Format("{0}:", parentPipeline.Name);
            Name = "г/п отвод на";

            SetValidationRules();
        }

        public AddEditPipelineBranchViewModel(Action<Guid> actionBeforeClosing, PipelineDTO model, PipelineDTO parentPipeline)
            : base(actionBeforeClosing, model)
        {
            _newPipelineType =
                ClientCache.DictionaryRepository.PipelineTypes[model.Type];
            Name = model.Name;
            KilometerOfStartPoint = model.KilometerOfStartPoint;
            KilometerOfEndPoint = model.KilometerOfEndPoint;
            KilometerOfStartPointConn = model.KilometerOfBeginConn;

            PipelineName = string.Format("{0}:", parentPipeline.Name);
            _parentPipeline = parentPipeline;
            EndEntityId = model.EndEntityId;

            SetValidationRules();
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
                                                DestEntityId = EndEntityId.Value
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
                        KilometerOfEnd= KilometerOfEndPoint.Value ,
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
                        DestEntityId = EndEntityId.Value
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
                   && KilometerOfStartPointConn <= (_parentPipeline.KilometerOfEndPoint);
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
        }

        #endregion

        public List<EntityType> StartAllowedTypes
        {
            get
            {
                return new List<EntityType> { EntityType.DistrStation };
            }
        }

        public string PipelineName { get; set; }
    }
}