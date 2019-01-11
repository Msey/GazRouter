using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;

namespace GazRouter.DataServices.BL
{
    /// <summary>
    ///     Сегменты по диаметру должны покрывать весь газопровод по всей длине ( не должно быть дырок, пересечений; по каждому
    ///     газопроводу должен быть хотя бы один сегмент ЛПУ)
    /// </summary>
    public class DiameterSegmentRule : SegmentsRule<DiameterSegmentDTO>
    {
        public DiameterSegmentRule(DictionaryRepositoryDTO dictionaries, Dictionary<Guid, PipelineDTO> pipelines,
            List<DiameterSegmentDTO> diameterSegments)
            : base(dictionaries, pipelines, diameterSegments)
        {
        }

        public override InconsistencyType InconsistencyType => InconsistencyType.DiameterSegmentError;
    }
}