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

namespace GazRouter.ObjectModel.Model.Dialogs.AddEdit.Pipelines.InletOutletCompShop
{
    public class AddEditPipelineInletOutletCompShopViewModel : AddEditPipelineObjectViewModel<PipelineDTO, Guid>
    {
        private readonly PipelineTypesDTO _newPipelineType;
        private readonly PipelineDTO _parentPipeline;
        private readonly int _sortorder;
        private readonly int _gasTransportSystemId;

        public AddEditPipelineInletOutletCompShopViewModel(Action<Guid> actionBeforeClosing, PipelineType newPipelineType, 
            PipelineDTO parentPipeline, int gasTransportSystemId, int sortorder)
            : base(actionBeforeClosing)
        {
		    _newPipelineType =
		        ClientCache.DictionaryRepository.PipelineTypes[newPipelineType];
		    _gasTransportSystemId = gasTransportSystemId;
			_sortorder = sortorder;

            _parentPipeline = parentPipeline;
            PipelineName = string.Format("{0}:", parentPipeline.Name);
            switch (newPipelineType)
            {
                case PipelineType.CompressorShopInlet:
                    Name = "Входной г/п";
                    break;
                case PipelineType.CompressorShopOutlet:
                    Name = "Выходной г/п";
                    break;
            }

            SetValidationRules();
        }

        public AddEditPipelineInletOutletCompShopViewModel(Action<Guid> actionBeforeClosing, PipelineDTO model, PipelineDTO parentPipeline)
            : base(actionBeforeClosing, model)
        {
            _newPipelineType =
                ClientCache.DictionaryRepository.PipelineTypes[model.Type];
            Name = model.Name;
            KilometerOfStartPoint = model.KilometerOfStartPoint;
            KilometerOfEndPoint = model.KilometerOfEndPoint;
            KilometerOfEndPointConn = model.Type == PipelineType.CompressorShopInlet ? model.KilometerOfBeginConn :  model.KilometerOfEndConn;

            PipelineName = string.Format("{0}:", parentPipeline.Name);
            _parentPipeline = parentPipeline;
            EndEntityId = model.Type == PipelineType.CompressorShopInlet ? model.EndEntityId : model.BeginEntityId;

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
                        DestEntityId = _newPipelineType.PipelineType == PipelineType.CompressorShopInlet ? _parentPipeline.Id : EndEntityId.Value,
                        Kilometr = _newPipelineType.PipelineType == PipelineType.CompressorShopInlet ? KilometerOfEndPointConn : null
                    },
                    EndConnParameters = new AddPipelineConnParameterSet
                    {
                        EndTypeId = PipelineEndType.EndType,
                        DestEntityId = _newPipelineType.PipelineType == PipelineType.CompressorShopInlet ? EndEntityId.Value : _parentPipeline.Id,
                        Kilometr = _newPipelineType.PipelineType == PipelineType.CompressorShopInlet ? null : KilometerOfEndPointConn
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
                        KilometerOfEnd = KilometerOfEndPoint.Value,
                        KilometerOfStart = KilometerOfStartPoint.Value,
                        GasTransportSystemId = Model.SystemId
                    },
                    StartConnParameters = new AddPipelineConnParameterSet
                    {
                        EndTypeId =  PipelineEndType.StartType,
                        DestEntityId = _newPipelineType.PipelineType == PipelineType.CompressorShopInlet ? _parentPipeline.Id : EndEntityId.Value,
                        Kilometr = _newPipelineType.PipelineType == PipelineType.CompressorShopInlet ? KilometerOfEndPointConn : null
                    },
                    EndConnParameters = new AddPipelineConnParameterSet
                    {
                        EndTypeId = PipelineEndType.EndType,
                        DestEntityId = _newPipelineType.PipelineType == PipelineType.CompressorShopInlet ? EndEntityId.Value : _parentPipeline.Id,
                        Kilometr = _newPipelineType.PipelineType == PipelineType.CompressorShopInlet ? null : KilometerOfEndPointConn
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
                   && KilometerOfEndPointConn >= _parentPipeline.KilometerOfStartPoint
                   && KilometerOfEndPointConn <= _parentPipeline.KilometerOfEndPoint;
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
            AddValidationFor(() => KilometerOfEndPointConn)
                .When(() => KilometerOfEndPointConn < _parentPipeline.KilometerOfStartPoint || KilometerOfEndPointConn > (_parentPipeline.KilometerOfEndPoint))
                .Show(string.Format("Значение должно быть в диапозоне от {0} до {1}", _parentPipeline.KilometerOfStartPoint, _parentPipeline.KilometerOfEndPoint));
        }

        #endregion

        public List<EntityType> StartAllowedTypes
        {
            get
            {
                return new List<EntityType> { EntityType.CompShop };
            }
        }

        public string PipelineName { get; set; }
    }
}