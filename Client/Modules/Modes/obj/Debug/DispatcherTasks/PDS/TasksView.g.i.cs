﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Modules\Modes\DispatcherTasks\PDS\TasksView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "CDA6CF3FD3562C263ECEF9C3F981FE54"
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


namespace GazRouter.Modes.DispatcherTasks.PDS {
    
    
    public partial class TasksView : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.UserControl TaskViewControl;
        
        internal System.Windows.Style HieghtHeader;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Telerik.Windows.Controls.RadGridView mainGridView;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Modes;component/DispatcherTasks/PDS/TasksView.xaml", System.UriKind.Relative));
            this.TaskViewControl = ((System.Windows.Controls.UserControl)(this.FindName("TaskViewControl")));
            this.HieghtHeader = ((System.Windows.Style)(this.FindName("HieghtHeader")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.mainGridView = ((Telerik.Windows.Controls.RadGridView)(this.FindName("mainGridView")));
        }
    }
}

