﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Modules\ObjectModel\Model\Tabs\Segments\Site\SiteSegmentView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "57511421D854D1584DB72AEB44694FB7"
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


namespace GazRouter.ObjectModel.Model.Tabs.Segments.Site {
    
    
    public partial class SiteSegmentView : System.Windows.Controls.UserControl {
        
        internal Telerik.Windows.Controls.RadGridView GridView;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/ObjectModel;component/Model/Tabs/Segments/Site/SiteSegmentView.xaml", System.UriKind.Relative));
            this.GridView = ((Telerik.Windows.Controls.RadGridView)(this.FindName("GridView")));
            this.ContextMenu = ((Telerik.Windows.Controls.RadContextMenu)(this.FindName("ContextMenu")));
        }
    }
}

