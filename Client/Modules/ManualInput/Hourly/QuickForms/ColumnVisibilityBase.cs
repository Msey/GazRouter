using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Microsoft.Practices.ObjectBuilder2;

namespace GazRouter.ManualInput.Hourly.QuickForms
{
    public class ColumnVisibilityBase : ViewModelBase
    {
        public ColumnVisibilityBase(IEnumerable<PropertyType> propTypes)
        {
            Visibility = new Dictionary<PropertyType, bool>();
            propTypes.ForEach(p => Visibility.Add(p, true));
        }

        public Dictionary<PropertyType, bool> Visibility { get; set; }
    }
}