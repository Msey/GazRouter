using System;
using GazRouter.DataExchange.ASUTP;
using GazRouter.DTO.DataExchange.Asdu;
using GazRouter.DTO.DataExchange.ExchangeEntity;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Utils.Extensions;

namespace DataExchange.ASDU
{
    public class BindableItem : ItemBase
    {
        private readonly AsduEntityDTO _dto;
        private readonly Func<AsduEntityDTO, bool> _saveAction;

        public BindableItem(AsduEntityDTO dto, Func<AsduEntityDTO, bool> saveAction = null)
        {
            _dto = dto;
            _saveAction = saveAction ?? (dto1 => true);

            IsExpanded = true;
        }

        public Guid EntityId
        {
            get { return _dto.EntityId; }
        }

        public override string Name
        {
            get { return _dto.EntityName; }
        }

        //public Guid? ParameterGid
        //{
        //    get { return _dto.ParameterGid; }
        //    set
        //    {
        //        if (_dto.ParameterGid != value)
        //        {
        //            var oldVal = _dto.ParameterGid;

        //            _dto.ParameterGid = value;
        //            if (!_saveAction(_dto))
        //            {
        //                _dto.ParameterGid = oldVal;
        //            }

        //            OnPropertyChanged(() => ParameterGid);
        //            OnPropertyChanged(() => IsActive);
        //            OnPropertyChanged(() => IsActiveEnabled);
        //        }
        //    }
        //}

        public bool IsActive
        {
            get { return _dto.IsActive; }
            set
            {
                if (_dto.IsActive != value)
                {
                    _dto.IsActive = value;
                    OnPropertyChanged(() => IsActive);
                    _saveAction(_dto);
                }
            }
        }

        public bool IsActiveEnabled
        {
            get { return _dto.ParameterGid != null; }
        }

        public EntityType EntityType
        {
            get { return _dto.EntityTypeId; }
        }

        public string EntityGid
        {
            get { return _dto.EntityGid?.Convert().ToString().Replace("-", "").ToUpper(); }
            set
            {
                if (value != null)
                {
                    var oldVal = _dto.EntityGid;

                    Guid? newVal = null;
                    if (!string.IsNullOrEmpty(value))
                    {
                        var g = new Guid();
                        if (Guid.TryParse(value, out g))
                            newVal = g.Convert();
                    }

                    if (oldVal == newVal) return;

                    _dto.EntityGid = newVal;
                    if (!_saveAction(_dto))
                    {
                        _dto.EntityGid = oldVal;
                    }
                    else
                    {
                        _dto.EntityGid = newVal;
                    }


                    OnPropertyChanged(() => EntityGid);
                    OnPropertyChanged(() => IsActive);
                    OnPropertyChanged(() => IsActiveEnabled);
                }
            }
        }
    }
}