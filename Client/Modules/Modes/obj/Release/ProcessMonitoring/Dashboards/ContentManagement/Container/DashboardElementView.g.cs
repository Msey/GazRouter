﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Modules\Modes\ProcessMonitoring\Dashboards\ContentManagement\Container\DashboardElementView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B4DF10D64F9C89A3921C9C8D35E8CCA5"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container;
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


namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container {
    
    
    public partial class DashboardElementView : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.CheckBox Mode;
        
        internal GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container.DashboardElementContainer DashboardElementContainer;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Modes;component/ProcessMonitoring/Dashboards/ContentManagement/Container/Dashboa" +
                        "rdElementView.xaml", System.UriKind.Relative));
            this.Mode = ((System.Windows.Controls.CheckBox)(this.FindName("Mode")));
            this.DashboardElementContainer = ((GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container.DashboardElementContainer)(this.FindName("DashboardElementContainer")));
        }
    }
}
