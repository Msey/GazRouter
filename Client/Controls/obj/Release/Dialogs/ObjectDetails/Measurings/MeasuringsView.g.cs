﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Controls\Dialogs\ObjectDetails\Measurings\MeasuringsView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "610BFC1D1273297A3DF781056555E706"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace GazRouter.Controls.Dialogs.ObjectDetails.Measurings {
    
    
    public partial class MeasuringsView : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.UserControl PropertiesValuesControl;
        
        internal System.Windows.Controls.Grid TheGrid;
        
        internal System.Windows.Controls.TextBlock date;
        
        internal System.Windows.Controls.TextBlock value;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Controls;component/Dialogs/ObjectDetails/Measurings/MeasuringsView.xaml", System.UriKind.Relative));
            this.PropertiesValuesControl = ((System.Windows.Controls.UserControl)(this.FindName("PropertiesValuesControl")));
            this.TheGrid = ((System.Windows.Controls.Grid)(this.FindName("TheGrid")));
            this.date = ((System.Windows.Controls.TextBlock)(this.FindName("date")));
            this.value = ((System.Windows.Controls.TextBlock)(this.FindName("value")));
        }
    }
}

