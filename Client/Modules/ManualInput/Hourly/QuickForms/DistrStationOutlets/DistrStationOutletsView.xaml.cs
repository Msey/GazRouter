﻿using System.Windows.Input;
using Telerik.Windows.Controls;

namespace GazRouter.ManualInput.Hourly.QuickForms.DistrStationOutlets
{
    public partial class DistrStationOutletsView
    {
        public DistrStationOutletsView()
        {
            InitializeComponent();
        }

        private void RadMaskedNumericInput_KeyDown(object sender, KeyEventArgs e)
        {
            var v = (sender as RadMaskedNumericInput);
            if (v.Value == null && (e.PlatformKeyCode == 189 || e.Key == Key.Subtract)) v.Value = -0;
        }
    }
}
