﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Controls\EntityName\EntityNameBlock.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "3988EC02C54C8641610640A97C5BCBB2"
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


namespace GazRouter.Controls.EntityName {
    
    
    public partial class EntityNameBlock : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Image IconImg;
        
        internal System.Windows.Controls.TextBlock NameTxt;
        
        internal Telerik.Windows.Controls.RadContextMenu Menu;
        
        internal System.Windows.Controls.Border Highlight;
        
        internal System.Windows.Controls.TextBlock TypeTxt;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Controls;component/EntityName/EntityNameBlock.xaml", System.UriKind.Relative));
            this.IconImg = ((System.Windows.Controls.Image)(this.FindName("IconImg")));
            this.NameTxt = ((System.Windows.Controls.TextBlock)(this.FindName("NameTxt")));
            this.Menu = ((Telerik.Windows.Controls.RadContextMenu)(this.FindName("Menu")));
            this.Highlight = ((System.Windows.Controls.Border)(this.FindName("Highlight")));
            this.TypeTxt = ((System.Windows.Controls.TextBlock)(this.FindName("TypeTxt")));
        }
    }
}
