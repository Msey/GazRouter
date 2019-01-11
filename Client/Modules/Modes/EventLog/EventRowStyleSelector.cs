using System.Windows;
using GazRouter.DTO.EventLog;
using Telerik.Windows.Controls;

namespace GazRouter.Modes.EventLog
{
    public class EventRowStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var obj = (EventDTO)item;

            if (obj.IsEmergency && !obj.IsQuote) return NonAckEmergencyEventStyle;

            if(obj.IsEmergency) return EmergencyEventStyle;

            if (!obj.IsQuote) return NonAckEventStyle;

            return base.SelectStyle(item, container);
        }

        public Style EmergencyEventStyle { get; set; }

        public Style NonAckEventStyle { get; set; }

        public Style NonAckEmergencyEventStyle { get; set; }
    }
}
