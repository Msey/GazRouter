using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;

namespace GazRouter.DataServices.BL
{
    /// <summary>
    ///     Сегменты по давлению должны покрывать весь газопровод по всей длине ( не должно быть дырок, пересечений; по каждому
    ///     газопроводу должен быть хотя бы один сегмент ЛПУ)
    /// </summary>
    public class PressureSegmentRule : SegmentsRule<PressureSegmentDTO>
    {
        public PressureSegmentRule(DictionaryRepositoryDTO dictionaries, Dictionary<Guid, PipelineDTO> pipelines,
            List<PressureSegmentDTO> pressureSegments)
            : base(dictionaries, pipelines, pressureSegments)
        {
        }

        public override InconsistencyType InconsistencyType
        {
            get { return InconsistencyType.PressureSegmentError; }
        }
    }
}