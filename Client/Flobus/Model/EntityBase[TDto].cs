using System;
using System.Linq;
using GazRouter.DTO.ObjectModel;

namespace GazRouter.Flobus.Model
{
    public abstract class EntityBase<TDto> : EntityBase<TDto, Guid> where TDto : CommonEntityDTO
    {
        protected EntityBase(TDto dto) : base(dto)
        {
        }

        public override string ShortPath => Dto.ShortPath;

        public override string TypeName
            => ClientCache.DictionaryRepository.EntityTypes.Single(et => et.EntityType == Dto.EntityType).Name;
    }
}