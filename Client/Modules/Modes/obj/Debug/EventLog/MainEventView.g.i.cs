﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Modules\Modes\EventLog\MainEventView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "EB0E6F129CB86B008E945C0117662843"
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


namespace GazRouter.Modes.EventLog {
    
    
    public partial class MainEventView : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.UserControl MainEventViewControl;
        
        internal Telerik.Windows.Controls.RadButton StartTraceBtn;
        
        internal Telerik.Windows.Controls.RadButton EndTraceBtn;
        
        internal Telerik.Windows.Controls.RadGridView RadGridView;
        
        internal Telerik.Windows.Controls.RadContextMenu ContextMenu;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Modes;component/EventLog/MainEventView.xaml", System.UriKind.Relative));
            this.MainEventViewControl = ((System.Windows.Controls.UserControl)(this.FindName("MainEventViewControl")));
            this.StartTraceBtn = ((Telerik.Windows.Controls.RadButton)(this.FindName("StartTraceBtn")));
            this.EndTraceBtn = ((Telerik.Windows.Controls.RadButton)(this.FindName("EndTraceBtn")));
            this.RadGridView = ((Telerik.Windows.Controls.RadGridView)(this.FindName("RadGridView")));
            this.ContextMenu = ((Telerik.Windows.Controls.RadContextMenu)(this.FindName("ContextMenu")));
        }
    }
}
