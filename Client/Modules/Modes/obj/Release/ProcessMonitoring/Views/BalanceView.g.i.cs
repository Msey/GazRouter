﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Modules\Modes\ProcessMonitoring\Views\BalanceView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "14BFFF05233F9E12DFCE3D945841E5D9"
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


namespace GazRouter.Modes.ProcessMonitoring.Views {
    
    
    public partial class BalanceView : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Telerik.Windows.Controls.GridViewColumnGroup Intake;
        
        internal Telerik.Windows.Controls.GridViewColumnGroup Consumer;
        
        internal Telerik.Windows.Controls.GridViewColumnGroup Transit;
        
        internal Telerik.Windows.Controls.GridViewColumnGroup STN;
        
        internal Telerik.Windows.Controls.GridViewColumnGroup IZAP;
        
        internal Telerik.Windows.Controls.GridViewColumnGroup ZAP;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Modes;component/ProcessMonitoring/Views/BalanceView.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.Intake = ((Telerik.Windows.Controls.GridViewColumnGroup)(this.FindName("Intake")));
            this.Consumer = ((Telerik.Windows.Controls.GridViewColumnGroup)(this.FindName("Consumer")));
            this.Transit = ((Telerik.Windows.Controls.GridViewColumnGroup)(this.FindName("Transit")));
            this.STN = ((Telerik.Windows.Controls.GridViewColumnGroup)(this.FindName("STN")));
            this.IZAP = ((Telerik.Windows.Controls.GridViewColumnGroup)(this.FindName("IZAP")));
            this.ZAP = ((Telerik.Windows.Controls.GridViewColumnGroup)(this.FindName("ZAP")));
        }
    }
}

