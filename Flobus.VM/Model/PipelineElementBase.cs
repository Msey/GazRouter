using System;
using GazRouter.DTO;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.Model;
using JetBrains.Annotations;

namespace GazRouter.Flobus.VM.Model
{
    public abstract class PipelineElementBase<T, TId> : EntityBase<T, TId>, IPipelineElement where T : NamedDto<TId>
    {
        protected PipelineElementBase(T dto, [NotNull] Pipeline pipe)
            : base(dto)
        {
            if (pipe == null)
            {
                throw new ArgumentNullException(nameof(pipe));
            }
            Pipe = pipe;
        }

        /// <summary>
        ///     Газопровод, на котором находится элемент
        /// </summary>
        public Pipeline Pipe { get; internal set; }

        /// <summary>
        ///     Километр установки крана на газопроводе
        /// </summary>
        public abstract double Km { get; }
    }
}