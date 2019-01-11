using System.Windows;
using GazRouter.DTO.EventLog.EventRecipient;
using Telerik.Windows.Controls;

namespace GazRouter.Modes.EventLog
{
    public class RecepientsStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var obj = (EventRecepientDTO)item;

            if (obj.AckDate == null)
            {
                return AckGridRowStyle;
            }

            return base.SelectStyle(item, container);
        }

        public Style AckGridRowStyle { get; set; }
    }
}
