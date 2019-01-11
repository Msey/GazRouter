using System;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.DataExchange.ASUTP
{
    public class BindableItem : ItemBase
    {

        private readonly ExchangeEntityDTO _dto;
        private readonly Func<ExchangeEntityDTO, string, bool> _saveAction;

        public BindableItem(ExchangeEntityDTO dto, Func<ExchangeEntityDTO, string, bool> saveAction)
        {
            _dto = dto;
            _saveAction = saveAction;
            IsExpanded = true;
        }

        public Guid EntityId => _dto.EntityId;

        public override string Name => _dto.EntityName;

        public string ExtId
        {
            get { return _dto.ExtId; }
            set
            {
                if (_dto.ExtId != value)
                {
                    if (_saveAction(_dto, value))
                    {
                        _dto.ExtId = value;
                    }

                    OnPropertyChanged(() => ExtId);
                    OnPropertyChanged(() => IsActive);
                    OnPropertyChanged(() => IsActiveEnabled);
                }
            }
        }

        public bool IsActive
        {
            get { return _dto.IsActive; }
            set
            {
                if (_dto.IsActive != value)
                {
                    _dto.IsActive = value;
                    OnPropertyChanged(() => IsActive);
                    _saveAction(_dto, _dto.ExtId);
                }
            }
        }

        public bool IsActiveEnabled => !string.IsNullOrEmpty(_dto.ExtId);

        public EntityType EntityType => _dto.EntityTypeId;
    }
}