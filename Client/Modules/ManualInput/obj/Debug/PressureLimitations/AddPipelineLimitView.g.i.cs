﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Modules\ManualInput\PressureLimitations\AddPipelineLimitView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "6332F90B14630B1D27FB360F274472F0"
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


namespace GazRouter.ManualInput.PressureLimitations {
    
    
    public partial class AddPressureLimitView : Telerik.Windows.Controls.RadWindow {
        
        internal Telerik.Windows.Controls.RadButton OKButton;
        
        internal Telerik.Windows.Controls.RadButton CancelButton;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/ManualInput;component/PressureLimitations/AddPipelineLimitView.xaml", System.UriKind.Relative));
            this.OKButton = ((Telerik.Windows.Controls.RadButton)(this.FindName("OKButton")));
            this.CancelButton = ((Telerik.Windows.Controls.RadButton)(this.FindName("CancelButton")));
        }
    }
}
