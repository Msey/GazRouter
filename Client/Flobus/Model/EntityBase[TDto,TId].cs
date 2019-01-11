using System.Linq;
using System.Windows;
using GazRouter.Common.Cache;
using GazRouter.DTO;
using GazRouter.DTO.ObjectModel;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Flobus.Model
{
    /// <summary>
    ///     Базовый класс для всех технологических объектов модели
    /// </summary>
    public abstract class EntityBase<TDto, TId> : SchemeObject where TDto : NamedDto<TId>
    {
        private Point _position;

        protected EntityBase(TDto dto)
        {
            Dto = dto;
        }

        public TDto Dto { get; }

        public TId Id => Dto.Id;

        public string Name => Dto.Name;

        public virtual string ShortPath => (Dto as CommonEntityDTO)?.ShortPath ?? string.Empty;

        public virtual string TypeName
            =>
                Dto is CommonEntityDTO
                    ? ClientCache.DictionaryRepository.EntityTypes.Single(
                        et => et.EntityType == (Dto as CommonEntityDTO).EntityType).Name
                    : string.Empty;

        /// <summary>
        ///     Позиция объекта
        /// </summary>
        public Point Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        protected static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();
    }
}