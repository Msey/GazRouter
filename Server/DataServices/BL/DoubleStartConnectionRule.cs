using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;
using GazRouter.DTO.Dictionaries.PipelineEndType;
using GazRouter.DTO.ObjectModel.PipelineConns;

namespace GazRouter.DataServices.BL
{
    /// <summary>
    /// Подключение начала газопровода должно быть одно
    /// </summary>
    public class DoubleStartConnectionRule : RuleBase
    {
        private readonly List<PipelineConnDTO> _pipelineConns;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dictionaries"></param>
        /// <param name="pipelineConns"></param>
        public DoubleStartConnectionRule(DictionaryRepositoryDTO dictionaries, List<PipelineConnDTO> pipelineConns)
            : base(dictionaries)
        {
            _pipelineConns = pipelineConns;
        }

        public override List<Guid> Validate()
        {
            return
                (from conn in _pipelineConns where conn.EndTypeId == PipelineEndType.StartType select conn.PipelineId)
                    .ToList();
        }


        public override InconsistencyType InconsistencyType
        {
            get { return InconsistencyType.PipelineConnBeginError; }
        }
    }
}