using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.Dictionaries.InconsistencyTypes;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;

namespace GazRouter.DataServices.BL
{
    /// <summary>
    ///     Сегменты по группам не должны пересекаться между собой
    /// </summary>
    public class GroupSegmentRule : RuleBase
    {
        private readonly Dictionary<Guid, PipelineDTO> _pipelines;
        private readonly List<GroupSegmentDTO> _groupSegments;

        public GroupSegmentRule(DictionaryRepositoryDTO dictionaries, Dictionary<Guid, PipelineDTO> pipelines,
            List<GroupSegmentDTO> groupSegments)
            : base(dictionaries)
        {
            _pipelines = pipelines;
            _groupSegments = groupSegments;
        }

        public override InconsistencyType InconsistencyType
        {
            get { return InconsistencyType.PipleneGroupSegmentError; }
        }

        public override List<Guid> Validate()
        {
            var errors = new List<Guid>();

            foreach (var groupSegment in _groupSegments)
            {
                var pipeline = _pipelines[groupSegment.PipelineId];
                var segments = _groupSegments.Where(ds => ds.PipelineId == pipeline.Id);
                var segmentsOrdered = segments.OrderBy(s => s.KilometerOfStartPoint).ToList();

                if (!errors.Contains(pipeline.Id))
                {
                    if (segmentsOrdered.Count == 1)
                    {
                        ValidateSingleSegment(errors, groupSegment, pipeline);
                    }

                    else
                    {
                        var addPipeline = IsHaveSegmentsCommonKmStartOrKmEnd(errors, groupSegment, segmentsOrdered,
                            pipeline);
                        if (addPipeline)
                        {
                            continue;
                        }

                        if (groupSegment == segmentsOrdered.First())
                        {
                            addPipeline = ValidateFirstSegment(errors, groupSegment, segmentsOrdered, pipeline);
                            if (addPipeline)
                            {
                                continue;
                            }
                        }

                        if (groupSegment == segmentsOrdered.Last())
                        {
                            addPipeline = ValidateLastSegment(errors, groupSegment, segmentsOrdered, pipeline);
                            if (addPipeline)
                            {
                                continue;
                            }
                        }

                        if (groupSegment != segmentsOrdered.First() && groupSegment != segmentsOrdered.Last())
                        {
                            ValidateOtherSegment(errors, groupSegment, segmentsOrdered, pipeline);
                        }
                    }
                }
            }

            return errors;
        }

        /// <summary>
        ///     Проверка валидности газопровода в случае единственного сегмента на газопроводе
        ///     (сегмент единственный на газопроводе -> км начала сегмента равен или больше км начала газопровода и км конца равен
        ///     или меньше км конца газопровода)
        /// </summary>
        /// <param name="errors">Список идентификаторов невалидных газопроводов</param>
        /// <param name="segment">Сегмент на газопроводе (единственный)</param>
        /// <param name="pipeline">Рассматриваемый газопровод</param>
        public void ValidateSingleSegment(List<Guid> errors, GroupSegmentDTO segment, PipelineDTO pipeline)
        {
            var kmEnd = segment.KilometerOfEndPoint;
            var kmStart = segment.KilometerOfStartPoint;
            var length = pipeline.Length;
            var kmStartPipeline = pipeline.KilometerOfStartPoint;
            var kmEndPipeline = kmStartPipeline + length;

            if ((decimal) kmEnd > (decimal) kmEndPipeline || (decimal) kmStart < (decimal) kmStartPipeline)
            {
                errors.Add(pipeline.Id);
            }
        }

        /// <summary>
        ///     Проверка сегментов на общий км начала или общий км конца
        /// </summary>
        /// <param name="errors">Список идентификаторов невалидных газопроводов</param>
        /// <param name="segment">Проверяемый сегмент</param>
        /// <param name="segments">Список всех сегментов на газопроводе</param>
        /// <param name="pipeline">Рассматриваемый газопровод</param>
        /// <returns>Был ли добавлен газопровод в список невалидных газопроводов</returns>
        public bool IsHaveSegmentsCommonKmStartOrKmEnd(List<Guid> errors, GroupSegmentDTO segment,
            List<GroupSegmentDTO> segments,
            PipelineDTO pipeline)
        {
            var kmStart = segment.KilometerOfStartPoint;
            var kmEnd = segment.KilometerOfEndPoint;
            var addPipeline = false;

            if (
                segments.FindAll(
                    s => s.KilometerOfStartPoint == kmStart).Count > 1
                ||
                segments.FindAll(s => s.KilometerOfEndPoint == kmEnd).Count > 1)
            {
                errors.Add(pipeline.Id);
                addPipeline = true;
            }

            return addPipeline;
        }

        /// <summary>
        ///     Проверка на валидность первого по км сегмента
        ///     (сегмент - первый по км на газопроводе -> км начала равен или больше км начала газопровода и км конца равен или
        ///     меньше км начала последующего сегмента)
        /// </summary>
        /// <param name="errors">Список идентификаторов невалидных газопроводов</param>
        /// <param name="segment">Проверяемый сегмент</param>
        /// <param name="segments">Упорядоченный список всех сегментов на газопроводе по км начала</param>
        /// <param name="pipeline">Рассматриваемый газопровод</param>
        /// <returns>Был ли добавлен газопровод в список невалидных газопроводов</returns>
        public bool ValidateFirstSegment(List<Guid> errors, GroupSegmentDTO segment, List<GroupSegmentDTO> segments,
            PipelineDTO pipeline)
        {
            var addPipeline = false;
            var kmStart = segment.KilometerOfStartPoint;
            var kmEnd = segment.KilometerOfEndPoint;
            var kmStartPipeline = pipeline.KilometerOfStartPoint;
            var kmStartNextSegment = segments[1].KilometerOfStartPoint;

            if ((decimal) kmStart >= (decimal) kmStartPipeline &&
                (decimal) kmEnd <= (decimal) kmStartNextSegment)
            {
            }

            else
            {
                errors.Add(pipeline.Id);
                addPipeline = true;
            }

            return addPipeline;
        }

        /// <summary>
        ///     Проверка на валидность последнего по км сегмента
        ///     (сегмент - последний по км -> км конца равен или меньше км конца газопровода и км начала равен или больше км конца
        ///     предыдущего)
        /// </summary>
        /// <param name="errors">Список идентификаторов невалидных газопроводов</param>
        /// <param name="segment">Проверяемый сегмент</param>
        /// <param name="segments">Упорядоченный список всех сегментов на газопроводе по км начала</param>
        /// <param name="pipeline">Рассматриваемый газопровод</param>
        /// <returns>Был ли добавлен газопровод в список невалидных газопроводов</returns>
        public bool ValidateLastSegment(List<Guid> errors, GroupSegmentDTO segment, List<GroupSegmentDTO> segments,
            PipelineDTO pipeline)
        {
            var addPipeline = false;
            var kmEnd = segment.KilometerOfEndPoint;
            var kmEndPipeline = pipeline.KilometerOfStartPoint + pipeline.Length;
            var kmStart = segment.KilometerOfStartPoint;
            var kmEndPrevious = segments[segments.Count - 2].KilometerOfEndPoint;

            if ((decimal) kmEnd <= (decimal) kmEndPipeline && (decimal) kmStart >= (decimal) kmEndPrevious)
            {
            }

            else
            {
                errors.Add(pipeline.Id);
                addPipeline = true;
            }

            return addPipeline;
        }

        /// <summary>
        ///     Проверка на валидность сегмента
        ///     (км начала равен или больше км конца предыдущего сегмента и км конца равен или меньше км начала последующего
        ///     сегмента)
        /// </summary>
        /// <param name="errors">Список идентификаторов невалидных газопроводов</param>
        /// <param name="segment">Проверяемый сегмент</param>
        /// <param name="segments">Упорядоченный список всех сегментов на газопроводе по км начала</param>
        /// <param name="pipeline">Рассматриваемый газопровод</param>
        public void ValidateOtherSegment(List<Guid> errors, GroupSegmentDTO segment, List<GroupSegmentDTO> segments,
            PipelineDTO pipeline)
        {
            var index = segments.IndexOf(segment);
            var kmStart = segment.KilometerOfStartPoint;
            var kmEnd = segment.KilometerOfEndPoint;
            var kmEndPrevious = segments[index - 1].KilometerOfEndPoint;
            var kmStartNext = segments[index + 1].KilometerOfStartPoint;

            if ((decimal) kmEndPrevious <= (decimal) kmStart && (decimal) kmStartNext >= (decimal) kmEnd)
            {
            }
            else
            {
                errors.Add(pipeline.Id);
            }
        }
    }
}