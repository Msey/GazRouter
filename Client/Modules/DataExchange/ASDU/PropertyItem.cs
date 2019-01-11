using System;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.DataExchange.Asdu;
using GazRouter.DTO.DataExchange.ExchangeProperty;
using Utils.Extensions;

namespace DataExchange.ASDU
{
    public class PropertyItem : PropertyChangedBase
    {
        private readonly AsduPropertyDTO _dto;
        private readonly Func<AsduPropertyDTO, bool> _saveAction;

        public PropertyItem(string name, AsduPropertyDTO dto, Func<AsduPropertyDTO, bool> saveAction)
        {
            _dto = dto;
            Name = name;
            _saveAction = saveAction;
        }

        public string Name { get; }

        public string ParameterGid
        {
            get { return _dto.ParameterGid?.Convert().ToString().Replace("-", "").ToUpper(); }
            set
            {
                if (value != null)
                {
                    var oldVal = _dto.ParameterGid;
                    Guid? newVal = null;
                    if (!string.IsNullOrEmpty(value))
                    {
                        var g = new Guid();
                        if (Guid.TryParse(value, out g))
                            newVal = g.Convert();
                    }

                    if (oldVal == newVal) return;

                    _dto.ParameterGid = newVal;

                    if (!_saveAction(_dto))
                    {
                        _dto.ParameterGid = oldVal;
                    }

                    OnPropertyChanged(() => ParameterGid);
                }
            }
        }
    }
}