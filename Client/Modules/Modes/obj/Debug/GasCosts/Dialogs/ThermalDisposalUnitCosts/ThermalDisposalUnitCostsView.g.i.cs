﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Modules\Modes\GasCosts\Dialogs\ThermalDisposalUnitCosts\ThermalDisposalUnitCostsView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "25884C41C0D26900B4E17EE558C02D91"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using GazRouter.Controls;
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


namespace GazRouter.Modes.GasCosts.Dialogs.ThermalDisposalUnitCosts {
    
    
    public partial class ThermalDisposalUnitCostsView : Telerik.Windows.Controls.RadWindow {
        
        public GazRouter.Controls.EntityPicker SelectedEntityControl;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Modes;component/GasCosts/Dialogs/ThermalDisposalUnitCosts/ThermalDisposalUnitCos" +
                        "tsView.xaml", System.UriKind.Relative));
            this.SelectedEntityControl = ((GazRouter.Controls.EntityPicker)(this.FindName("SelectedEntityControl")));
        }
    }
}

