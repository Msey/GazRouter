using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;
using GazRouter.DTO.Dictionaries.PipelineEndType;
using GazRouter.DTO.ObjectModel.PipelineConns;
using GazRouter.DTO.ObjectModel.Pipelines;

namespace GazRouter.DataServices.BL
{
    /// <summary>
    ///     Для каждого подключения начала газопровода километр подключения был в диапазоне начало - конец газопровода, к
    ///     которому он подключается
    /// </summary>
    public class KmEndConnectionRule : KmConnectionRule
    {
        public KmEndConnectionRule(DictionaryRepositoryDTO dictionaries, Dictionary<Guid, PipelineDTO> pipelines,
            List<PipelineConnDTO> pipelineConn, PipelineEndType pipelineEndType)
            : base(dictionaries, pipelines, pipelineConn, pipelineEndType)
        {
        }

        public override InconsistencyType InconsistencyType => InconsistencyType.KmEndConnectionError;
    }
}