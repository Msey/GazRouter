using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using System.Windows.Media;

namespace DataExchange.ASDU
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


        public bool  IsEnabled { get; set; }

        public ItemBase Parent { get; set; }

        public virtual ImageSource Image { get; set; }

        public virtual bool EditAllowed => false;

        public string GetParentName()
        {
            string res = string.Empty;
            if (Parent != null)
                res = Parent.GetParentName() + "." + Name;
            else
                res = Name;
            return res;
        }

    }
}