using System;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.PipelineEndType;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.PipelineConns;
using GazRouter.DTO.ObjectModel.Pipelines;

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines.Booster
{
    public class AddEditPipelineBoosterViewModel : AddEditPipelineObjectViewModel<PipelineDTO, Guid>
    {
        private readonly PipelineTypesDTO _newPipelineType;
        private readonly PipelineDTO _parentPipeline;
        private readonly int _sortorder;
        private readonly int _gasTransportSystemId;

        public AddEditPipelineBoosterViewModel(Action<Guid> closeCallback, PipelineDTO parentPipeline, 
            int gasTransportSystemId, int sortorder)
            : base(closeCallback)
        {
            _newPipelineType =
                ClientCache.DictionaryRepository.PipelineTypes[ PipelineType.Booster];
            _parentPipeline = parentPipeline;
            _gasTransportSystemId = gasTransportSystemId;
            _sortorder = sortorder;

            KilometerOfStartPoint = parentPipeline.KilometerOfStartPoint;
            KilometerOfEndPoint = parentPipeline.KilometerOfEndPoint;
            StartEntity = parentPipeline;
            EndEntity = parentPipeline;

            SetValidationRules();
        }

        public AddEditPipelineBoosterViewModel(Action<Guid> closeCallback, PipelineDTO model, PipelineDTO parentPipeline)
            : base(closeCallback, model)
        {
            _newPipelineType =
                ClientCache.DictionaryRepository.PipelineTypes[model.Type];
            _parentPipeline = parentPipeline;
            Name = model.Name;
            KilometerOfStartPoint = model.KilometerOfStartPoint;
            KilometerOfEndPoint = model.KilometerOfEndPoint;

            SetValidationRules();
        }

        #region SetValidationRules

        public void SetValidationRules()
        {
            AddValidationFor(() => KilometerOfStartPoint)
                .When(() => KilometerOfStartPoint < _parentPipeline.KilometerOfStartPoint || KilometerOfStartPoint > (_parentPipeline.KilometerOfEndPoint ))
                .Show(string.Format("Значение должно быть в диапозоне от {0} до {1}", _parentPipeline.KilometerOfStartPoint, _parentPipeline.KilometerOfEndPoint ));

            AddValidationFor(() => KilometerOfEndPoint)
                .When(() => KilometerOfEndPoint < _parentPipeline.KilometerOfStartPoint || KilometerOfEndPoint > _parentPipeline.KilometerOfEndPoint)
                .Show(string.Format("Значение должно быть в диапозоне от {0} до {1}", _parentPipeline.KilometerOfStartPoint, _parentPipeline.KilometerOfEndPoint));

            AddValidationFor(() => KilometerOfStartPoint)
               .When(() => KilometerOfStartPoint > KilometerOfEndPoint)
               .Show("Км. начала не может быть больше км. окончания");

            AddValidationFor(() => KilometerOfEndPoint)
               .When(() => KilometerOfEndPoint < KilometerOfStartPoint)
               .Show("Км. окончания не может быть меньше км. начала");
         }

        #endregion

        #region Tasks

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
                        KilometerOfStart = KilometerOfStartPoint.Value,
                        KilometerOfEnd = KilometerOfEndPoint.Value,
                        GasTransportSystemId = _gasTransportSystemId,
                        SortOrder = _sortorder
                    },
                    StartConnParameters = new AddPipelineConnParameterSet
                    {
                        EndTypeId = PipelineEndType.StartType,
                        DestEntityId = StartEntity.Id,
                        Kilometr = KilometerOfStartPoint
                    },
                    EndConnParameters = new AddPipelineConnParameterSet
                    {
                        EndTypeId = PipelineEndType.EndType,
                        DestEntityId = EndEntity.Id,
                        Kilometr = KilometerOfEndPoint
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
                        DestEntityId = Model.BeginEntityId.Value,
                        Kilometr = KilometerOfStartPoint
                    },
                    EndConnParameters = new AddPipelineConnParameterSet
                    {
                        EndTypeId = PipelineEndType.EndType,
                        DestEntityId = Model.EndEntityId.Value,
                        Kilometr = KilometerOfEndPoint
                    }
                };
                return new ObjectModelServiceProxy().EditPipelineAsync(parameters);
            }
        }

        #endregion

        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name)
                   && KilometerOfStartPoint >= 0
                   && KilometerOfStartPoint < KilometerOfEndPoint
                   && KilometerOfEndPoint > 0
                   && KilometerOfStartPoint >= _parentPipeline.KilometerOfStartPoint
                   && KilometerOfEndPoint <= _parentPipeline.KilometerOfEndPoint;
        }

        protected override string CaptionEntityTypeName
        {
            get { return _newPipelineType.Name; }
        }

        
    }
}
