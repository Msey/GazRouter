using System;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.DataExchange.ExchangeProperty;

namespace GazRouter.DataExchange.ASUTP
{
    public class PropertyItem : PropertyChangedBase
    {
        private readonly ExchangePropertyDTO _dto;
        private readonly Func<ExchangePropertyDTO, string, bool> _saveAction;

        public PropertyItem(string name, ExchangePropertyDTO dto, Func<ExchangePropertyDTO, string, bool> saveAction)
        {
            _dto = dto;
            Name = name;
            _saveAction = saveAction;
        }

        public string Name { get; }

        public string ExtId
        {
            get { return _dto.ExtId; }
            set
            {
                if (_dto.ExtId != value && !(string.IsNullOrEmpty(_dto.ExtId) && string.IsNullOrEmpty(value)))
                {
                    if (_saveAction(_dto, value))
                    {
                        _dto.ExtId = value;
                    }

                    OnPropertyChanged(() => ExtId);
                }
            }
        }

        public double? Coeff
        {
            get { return _dto.Coeff; }
            set
            {
                var oldValue = _dto.Coeff;
                _dto.Coeff = value;
                if (!_saveAction(_dto, _dto.ExtId))
                    _dto.Coeff = oldValue;
                
                OnPropertyChanged(() => Coeff);
            }
        }
    }
}