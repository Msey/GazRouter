﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Modules\Repair\PlanTableView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "B9C40089A7093E5F65212822EB0D61D5"
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


namespace GazRouter.Repair {
    
    
    public partial class PlanTableView : System.Windows.Controls.UserControl {
        
        internal Telerik.Windows.Controls.ContainerBindingCollection BindingsCollection;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Telerik.Windows.Controls.RadGridView Grid;
        
        internal Telerik.Windows.Controls.RadContextMenu GridContextMenu;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Repair;component/PlanTableView.xaml", System.UriKind.Relative));
            this.BindingsCollection = ((Telerik.Windows.Controls.ContainerBindingCollection)(this.FindName("BindingsCollection")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.Grid = ((Telerik.Windows.Controls.RadGridView)(this.FindName("Grid")));
            this.GridContextMenu = ((Telerik.Windows.Controls.RadContextMenu)(this.FindName("GridContextMenu")));
        }
    }
}
