﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Flobus\Dialogs\AddCommentWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "C6C2EA9F4B592635CF539BB6AE58FE60"
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


namespace GazRouter.Flobus.Dialogs {
    
    
    public partial class AddCommentWindow : Telerik.Windows.Controls.RadWindow {
        
        internal Telerik.Windows.Controls.RadWindow MainWindow;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Flobus;component/Dialogs/AddCommentWindow.xaml", System.UriKind.Relative));
            this.MainWindow = ((Telerik.Windows.Controls.RadWindow)(this.FindName("MainWindow")));
            this.SaveButton = ((Telerik.Windows.Controls.RadButton)(this.FindName("SaveButton")));
        }
    }
}

