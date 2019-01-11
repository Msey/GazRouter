using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Segment;

namespace GazRouter.DataServices.BL
{
    public abstract class SegmentsRule<T> : RuleBase where T : BaseSegmentDTO
    {
        private readonly Dictionary<Guid, PipelineDTO> _pipelines;
        private readonly List<T> _baseSegments;

        protected SegmentsRule(DictionaryRepositoryDTO dictionaries, Dictionary<Guid, PipelineDTO> pipelines,
            List<T> baseSegments)
            : base(dictionaries)
        {
            _pipelines = pipelines;
            _baseSegments = baseSegments;
        }

        public override List<Guid> Validate()
        {
            var errors = new List<Guid>();

            CheckPipelineHasSegments(errors);

            foreach (var segment in _baseSegments)
            {
                var pipeline = _pipelines[segment.PipelineId];

                if (!errors.Contains(pipeline.Id))
                {
                    var segments = _baseSegments.Where(ds => ds.PipelineId == pipeline.Id).ToList();

                    if (segments.Count == 1)
                    {
                        ValidateSingleSegment(errors, segment, pipeline);
                    }

                    else
                    {
                        var segmentsOrdered = segments.OrderBy(s => s.KilometerOfStartPoint).ToList();

                        if (segment == segmentsOrdered.First())
                        {
                            ValidateFirstSegment(errors, segment, segmentsOrdered, pipeline);
                        }

                        if (segment == segmentsOrdered.Last())
                        {
                            ValidateLastSegment(errors, segment, segmentsOrdered, pipeline);
                        }

                        if (segment != segmentsOrdered.First() && segment != segmentsOrdered.Last())
                        {
                            ValidateOtherSegment(errors, segment, segmentsOrdered, pipeline);
                        }
                    }
                }
            }

            return errors;
        }

        #region Проверки на валидность

        /// <summary>
        ///     Функция-проверка на наличие хотя бы одного сегмента данного типа на газопроводе
        /// </summary>
        /// <param name="errors">Список идентификаторов невалидных газопроводов</param>
        public void CheckPipelineHasSegments(List<Guid> errors)
        {
            foreach (var pipelineId in _pipelines.Keys)
            {
                if (_baseSegments.All(s => s.PipelineId != pipelineId))
                {
                    errors.Add(pipelineId);
                }
            }
        }

        /// <summary>
        ///     Проверка валидности газопровода в случае единственного сегмента на газопроводе
        ///     (сегмент единственный на газопроводе -> км начала сегмента равен км начала газопровода и км конца равен км конца
        ///     газопровода)
        /// </summary>
        /// <param name="errors">Список идентификаторов невалидных газопроводов</param>
        /// <param name="segment">Сегмент на газопроводе (единственный)</param>
        /// <param name="pipeline">Рассматриваемый газопровод</param>
        public void ValidateSingleSegment(List<Guid> errors, T segment, PipelineDTO pipeline)
        {
            var kmEnd = segment.KilometerOfEndPoint;
            var kmStart = segment.KilometerOfStartPoint;
            var kmStartPipeline = pipeline.KilometerOfStartPoint;
            var kmEndPipeline = pipeline.KilometerOfStartPoint + pipeline.Length;

            if ((decimal) kmEnd != (decimal) kmEndPipeline || (decimal) kmStart != (decimal) kmStartPipeline)
            {
                errors.Add(pipeline.Id);
            }
        }

        /// <summary>
        ///     Проверка на валидность первого по км сегмента
        ///     (сегмент - первый по км на газопроводе -> км начала равен км начала газопровода и км конца равен км начала
        ///     последующего сегмента)
        /// </summary>
        /// <param name="errors">Список идентификаторов невалидных газопроводов</param>
        /// <param name="segment">Проверяемый сегмент</param>
        /// <param name="segments">Упорядоченный список всех сегментов на газопроводе по км начала</param>
        /// <param name="pipeline">Рассматриваемый газопровод</param>
        public void ValidateFirstSegment(List<Guid> errors, T segment, List<T> segments, PipelineDTO pipeline)
        {
            var kmEnd = segment.KilometerOfEndPoint;
            var kmStart = segment.KilometerOfStartPoint;
            var kmStartPipeline = pipeline.KilometerOfStartPoint;

            if ((decimal) kmStart != (decimal) kmStartPipeline ||
                (decimal) kmEnd != (decimal) segments[1].KilometerOfStartPoint)
            {
                errors.Add(pipeline.Id);
            }
        }

        /// <summary>
        ///     Проверка на валидность последнего по км сегмента
        ///     (сегмент - последний по км -> км конца равен км конца газопровода и км начала равен км конца предыдущего)
        /// </summary>
        /// <param name="errors">Список идентификаторов невалидных газопроводов</param>
        /// <param name="segment">Проверяемый сегмент</param>
        /// <param name="segments">Упорядоченный список всех сегментов на газопроводе по км начала</param>
        /// <param name="pipeline">Рассматриваемый газопровод</param>
        public void ValidateLastSegment(List<Guid> errors, T segment, List<T> segments, PipelineDTO pipeline)
        {
            var kmEnd = segment.KilometerOfEndPoint;
            var kmEndPipeline = pipeline.KilometerOfStartPoint + pipeline.Length;

            if ((decimal) kmEnd != (decimal) kmEndPipeline)
            {
                errors.Add(pipeline.Id);
            }
        }

        /// <summary>
        ///     Проверка на валидность сегмента
        ///     (км начала равен км конца предыдущего сегмента и км конца равен км начала последующего сегмента)
        /// </summary>
        /// <param name="errors">Список идентификаторов невалидных газопроводов</param>
        /// <param name="segment">Проверяемый сегмент</param>
        /// <param name="segments">Упорядоченный список всех сегментов на газопроводе по км начала</param>
        /// <param name="pipeline">Рассматриваемый газопровод</param>
        public void ValidateOtherSegment(List<Guid> errors, T segment, List<T> segments, PipelineDTO pipeline)
        {
            var index = segments.IndexOf(segment);
            var kmEnd = segment.KilometerOfEndPoint;
            var kmStartNext = segments[index + 1].KilometerOfStartPoint;

            if ((decimal) kmEnd != (decimal) kmStartNext)
            {
                errors.Add(pipeline.Id);
            }
        }

        #endregion
    }
}