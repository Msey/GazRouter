using System.Collections.Generic;
using GazRouter.Common.ViewModel;

namespace GazRouter.DataExchange.ASUTP
{
    public class ItemBase : PropertyChangedBase
    {
        public ItemBase()
        {
            Children = new List<ItemBase>();
            IsExpanded = true;
        }

        public List<ItemBase> Children { get; set; }

        public virtual string Name { get; set; }

        public bool IsExpanded { get; set; }

        public bool IsEditable { get; set; }
    }
}