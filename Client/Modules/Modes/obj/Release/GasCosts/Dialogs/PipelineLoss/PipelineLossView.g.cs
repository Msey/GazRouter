﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Modules\Modes\GasCosts\Dialogs\PipelineLoss\PipelineLossView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D4B4F7F3601BB16FEDE5BBA00E7E8825"
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


namespace GazRouter.Modes.GasCosts.Dialogs.PipelineLoss {
    
    
    public partial class PipelineLossView : Telerik.Windows.Controls.RadWindow {
        
        internal Telerik.Windows.Controls.RadWindow Wnd;
        
        internal Telerik.Windows.Controls.InvertedBooleanToVisibilityConverter InvertedBooleanToVisibilityConverter;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Modes;component/GasCosts/Dialogs/PipelineLoss/PipelineLossView.xaml", System.UriKind.Relative));
            this.Wnd = ((Telerik.Windows.Controls.RadWindow)(this.FindName("Wnd")));
            this.InvertedBooleanToVisibilityConverter = ((Telerik.Windows.Controls.InvertedBooleanToVisibilityConverter)(this.FindName("InvertedBooleanToVisibilityConverter")));
        }
    }
}

