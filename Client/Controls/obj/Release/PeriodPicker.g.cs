﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Controls\PeriodPicker.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "03DD9547A7EF5A02B4E5AA0715447111"
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


namespace GazRouter.Controls {
    
    
    public partial class PeriodPicker : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.UserControl selector;
        
        internal System.Windows.Controls.TextBox txtDates;
        
        internal Telerik.Windows.Controls.RadButton btnSelect;
        
        internal System.Windows.Controls.Primitives.Popup popup;
        
        internal Telerik.Windows.Controls.RadContextMenu RadContextMenuMenu;
        
        internal Telerik.Windows.Controls.RadDateTimePicker beginDate;
        
        internal Telerik.Windows.Controls.RadDateTimePicker endDate;
        
        internal System.Windows.Controls.TextBlock messageCount;
        
        internal System.Windows.Controls.TextBlock daysCount;
        
        internal Telerik.Windows.Controls.RadButton ApplyButton;
        
        internal Telerik.Windows.Controls.RadButton CloseButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Controls;component/PeriodPicker.xaml", System.UriKind.Relative));
            this.selector = ((System.Windows.Controls.UserControl)(this.FindName("selector")));
            this.txtDates = ((System.Windows.Controls.TextBox)(this.FindName("txtDates")));
            this.btnSelect = ((Telerik.Windows.Controls.RadButton)(this.FindName("btnSelect")));
            this.popup = ((System.Windows.Controls.Primitives.Popup)(this.FindName("popup")));
            this.RadContextMenuMenu = ((Telerik.Windows.Controls.RadContextMenu)(this.FindName("RadContextMenuMenu")));
            this.beginDate = ((Telerik.Windows.Controls.RadDateTimePicker)(this.FindName("beginDate")));
            this.endDate = ((Telerik.Windows.Controls.RadDateTimePicker)(this.FindName("endDate")));
            this.messageCount = ((System.Windows.Controls.TextBlock)(this.FindName("messageCount")));
            this.daysCount = ((System.Windows.Controls.TextBlock)(this.FindName("daysCount")));
            this.ApplyButton = ((Telerik.Windows.Controls.RadButton)(this.FindName("ApplyButton")));
            this.CloseButton = ((Telerik.Windows.Controls.RadButton)(this.FindName("CloseButton")));
        }
    }
}

