using GazRouter.Common.ViewModel;
using GazRouter.DTO.DataExchange.Asdu;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Utils.Extensions;

namespace DataExchange.Integro.ASSPOOTI
{
    public class MappingPropertyItem : PropertyChangedBase
    {
        private AsduPropertyDTO asduPropertyDTO;
        public MappingPropertyItem(string name, AsduPropertyDTO dto)
        {
            asduPropertyDTO = dto;
            Name = name;
        }

        public string Name { get; }

        public Guid EntityId { get { return asduPropertyDTO.EntityId; } }

        public string ParameterGid
        {
            get { return asduPropertyDTO.ParameterGid?.Convert().ToString().Replace("-", "").ToUpper(); }
            set
            {
                if (value != null)
                {
                    var oldVal = asduPropertyDTO.ParameterGid;
                    Guid? newVal = null;
                    if (!string.IsNullOrEmpty(value))
                    {
                        var g = new Guid();
                        if (Guid.TryParse(value, out g))
                            newVal = g.Convert();
                    }

                    if (oldVal == newVal) return;

                    asduPropertyDTO.ParameterGid = newVal;
                    OnPropertyChanged(() => ParameterGid);
                }
            }
        }

        public PropertyType PropertyTypeId { get { return asduPropertyDTO.PropertyTypeId; } }

        public Guid? EntityGid { get; set; }

    }
}
