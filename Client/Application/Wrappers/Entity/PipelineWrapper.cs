using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.Pipelines;

namespace GazRouter.Application.Wrappers.Entity
{
    public class PipelineWrapper : EntityWrapperBase<PipelineDTO>
    {
        public PipelineWrapper(PipelineDTO dto, bool displaySystem) : base(dto, displaySystem)
        {
            AddProperty("Тип газопровода", dto.TypeName);
            AddProperty("Км. начала", dto.KilometerOfStartPoint.ToString("0.###"));
            AddProperty("Км. окончания", dto.KilometerOfEndPoint.ToString("0.###"));
            AddProperty("Длина, км", dto.Length.ToString("0.###"));
        }

        [Display(Name = "Тип газопровода", Order = 10)]
        public string TypeName => _dto.TypeName;

        [Display(Name = "Км. начала", Order = 20)]
        public double KilometerOfStartPoint => _dto.KilometerOfStartPoint;

        [Display(Name = "Км. окончания", Order = 30)]
        public double KilometerOfEndPoint => _dto.KilometerOfEndPoint;

        [Display(Name = "Длина, км", Order = 40)]
        public double Length => _dto.KilometerOfEndPoint;

        [Browsable(false)]
        public PipelineType Type => _dto.Type;

        [Browsable(false)]
        public Guid? BeginEntityId => _dto.BeginEntityId;

        [Browsable(false)]
        public Guid? EndEntityId => _dto.EndEntityId;
    }
}