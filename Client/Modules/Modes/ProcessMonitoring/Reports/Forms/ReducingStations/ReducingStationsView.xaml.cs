using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace GazRouter.Modes.ProcessMonitoring.Reports.Forms.ReducingStations
{
    public partial class ReducingStationsView
    {
        public ReducingStationsView()
        {
            InitializeComponent();
        }

        private void RadMaskedNumericInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var v = (sender as Telerik.Windows.Controls.RadMaskedNumericInput);
            if (v.Value == null && e.PlatformKeyCode == 189) v.Value = -0;
        }
    }
}
