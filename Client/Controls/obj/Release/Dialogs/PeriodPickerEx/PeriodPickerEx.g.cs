﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Controls\Dialogs\PeriodPickerEx\PeriodPickerEx.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "8ED5D53AD7F8C94AFA78F853EDC91B20"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using GazRouter.Controls.Dialogs.PeriodPickerEx;
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


namespace GazRouter.Controls.Dialogs.PeriodPickerEx {
    
    
    public partial class PeriodPickerEx : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.TextBox Txt;
        
        internal Telerik.Windows.Controls.RadButton BtnDrop;
        
        internal System.Windows.Controls.Primitives.Popup DropPart;
        
        internal GazRouter.Controls.Dialogs.PeriodPickerEx.PeriodPickerDropView DropView;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Controls;component/Dialogs/PeriodPickerEx/PeriodPickerEx.xaml", System.UriKind.Relative));
            this.Txt = ((System.Windows.Controls.TextBox)(this.FindName("Txt")));
            this.BtnDrop = ((Telerik.Windows.Controls.RadButton)(this.FindName("BtnDrop")));
            this.DropPart = ((System.Windows.Controls.Primitives.Popup)(this.FindName("DropPart")));
            this.DropView = ((GazRouter.Controls.Dialogs.PeriodPickerEx.PeriodPickerDropView)(this.FindName("DropView")));
        }
    }
}

