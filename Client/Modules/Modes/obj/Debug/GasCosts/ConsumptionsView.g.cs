﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Modules\Modes\GasCosts\ConsumptionsView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C095796953DA3AB6F62E862BB4AC585C"
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


namespace GazRouter.Modes.GasCosts {
    
    
    public partial class ConsumptionsView : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Telerik.Windows.Controls.RadTreeListView MainGrid;
        
        internal Telerik.Windows.Controls.RadContextMenu GridContextMenu;
        
        internal Telerik.Windows.Controls.RadTreeListView MainGridDuplicate;
        
        internal Telerik.Windows.Controls.RadContextMenu GridContextMenuDuplicate;
        
        internal Telerik.Windows.Controls.RadButton AddStnButton;
        
        internal Telerik.Windows.Controls.RadGridView NormGrid;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Modes;component/GasCosts/ConsumptionsView.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.MainGrid = ((Telerik.Windows.Controls.RadTreeListView)(this.FindName("MainGrid")));
            this.GridContextMenu = ((Telerik.Windows.Controls.RadContextMenu)(this.FindName("GridContextMenu")));
            this.MainGridDuplicate = ((Telerik.Windows.Controls.RadTreeListView)(this.FindName("MainGridDuplicate")));
            this.GridContextMenuDuplicate = ((Telerik.Windows.Controls.RadContextMenu)(this.FindName("GridContextMenuDuplicate")));
            this.AddStnButton = ((Telerik.Windows.Controls.RadButton)(this.FindName("AddStnButton")));
            this.NormGrid = ((Telerik.Windows.Controls.RadGridView)(this.FindName("NormGrid")));
            this.ContextMenu = ((Telerik.Windows.Controls.RadContextMenu)(this.FindName("ContextMenu")));
        }
    }
}

