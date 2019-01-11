using GazRouter.DTO.Repairs.Workflow;
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

namespace GazRouter.Repair.RepWorks
{
    public class SetStatusItem
    {
        public SetStatusItem(ICommand cmd, WorkStateDTO state)
        {
            Command = cmd;
            State = state;
        }

        public ICommand Command { get; set; }

        public WorkStateDTO State { get; set; }
    }


}
