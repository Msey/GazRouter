﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Controls\Dialogs\DatePicker\DatePickerDialogView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3318F1E47FA74B66A609518673C098DE"
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


namespace GazRouter.Controls.Dialogs.DatePicker {
    
    
    public partial class DatePickerDialogView : Telerik.Windows.Controls.RadWindow {
        
        internal Telerik.Windows.Controls.RadDatePicker datePicker;
        
        internal Telerik.Windows.Controls.RadButton SaveButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Controls;component/Dialogs/DatePicker/DatePickerDialogView.xaml", System.UriKind.Relative));
            this.datePicker = ((Telerik.Windows.Controls.RadDatePicker)(this.FindName("datePicker")));
            this.SaveButton = ((Telerik.Windows.Controls.RadButton)(this.FindName("SaveButton")));
        }
    }
}

