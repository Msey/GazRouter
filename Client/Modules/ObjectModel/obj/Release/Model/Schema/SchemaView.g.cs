﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Modules\ObjectModel\Model\Schema\SchemaView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4CC0D4BDAA1B03EBF1DFEE29C12E3C0A"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using GazRouter.Flobus;
using GazRouter.Flobus.Misc;
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


namespace GazRouter.ObjectModel.Model.Schema {
    
    
    public partial class SchemaView : System.Windows.Controls.UserControl {
        
        internal GazRouter.Flobus.Misc.Scale2PercentConverter Scale2PecentCnv;
        
        internal GazRouter.Flobus.Schema Scheme;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/ObjectModel;component/Model/Schema/SchemaView.xaml", System.UriKind.Relative));
            this.Scale2PecentCnv = ((GazRouter.Flobus.Misc.Scale2PercentConverter)(this.FindName("Scale2PecentCnv")));
            this.Scheme = ((GazRouter.Flobus.Schema)(this.FindName("Scheme")));
        }
    }
}

