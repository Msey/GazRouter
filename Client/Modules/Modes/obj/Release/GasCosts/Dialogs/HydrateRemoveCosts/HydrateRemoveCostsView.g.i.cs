﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Modules\Modes\GasCosts\Dialogs\HydrateRemoveCosts\HydrateRemoveCostsView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "EDDF1ADA57850F698155BBBD73D56A1E"
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


namespace GazRouter.Modes.GasCosts.Dialogs.HydrateRemoveCosts {
    
    
    public partial class HydrateRemoveCostsView : Telerik.Windows.Controls.RadWindow {
        
        internal Telerik.Windows.Controls.BooleanToVisibilityConverter Bool2Visibility;
        
        internal Telerik.Windows.Controls.InvertedBooleanToVisibilityConverter InvertedBool2Visibility;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Modes;component/GasCosts/Dialogs/HydrateRemoveCosts/HydrateRemoveCostsView.xaml", System.UriKind.Relative));
            this.Bool2Visibility = ((Telerik.Windows.Controls.BooleanToVisibilityConverter)(this.FindName("Bool2Visibility")));
            this.InvertedBool2Visibility = ((Telerik.Windows.Controls.InvertedBooleanToVisibilityConverter)(this.FindName("InvertedBool2Visibility")));
            this.SelectedEntityControl = ((GazRouter.Controls.EntityPicker)(this.FindName("SelectedEntityControl")));
        }
    }
}

