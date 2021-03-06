﻿#pragma checksum "C:\Users\k.suvorov\Source\Workspaces\Iuspt\MainSolution\Client\Modules\Modes\ExcelReports\ExcelReportMainView.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "ADA348847321A00256EE79929B705174"
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
using Telerik.Windows.Controls.Spreadsheet.Controls;


namespace GazRouter.Modes.ExcelReports {
    
    
    public partial class ExcelReportMainView : System.Windows.Controls.UserControl {
        
        internal Telerik.Windows.Controls.BooleanToVisibilityConverter Bool2Visibility;
        
        internal Telerik.Windows.Controls.InvertedBooleanToVisibilityConverter InvertedBool2Visibility;
        
        internal Telerik.Windows.Controls.ContainerBindingCollection BindingsCollection;
        
        internal Telerik.Windows.Controls.RadTreeView tree;
        
        internal Telerik.Windows.Controls.RadContextMenu ContextMenu;
        
        internal System.Windows.Controls.Grid detailGrid;
        
        internal System.Windows.Controls.CheckBox chbEditMode;
        
        internal System.Windows.Controls.StackPanel stpKeyDate;
        
        internal Telerik.Windows.Controls.RadButton updateBtn;
        
        internal Telerik.Windows.Controls.RadButton printBtn;
        
        internal Telerik.Windows.Controls.RadRibbonView ribbonView;
        
        internal Telerik.Windows.Controls.RadRibbonSplitButton PasteButton;
        
        internal Telerik.Windows.Controls.RadButtonGroup FontStylesGroup;
        
        internal Telerik.Windows.Controls.RadRibbonSplitButton MergeAndCenterButton;
        
        internal Telerik.Windows.Controls.RadRibbonSplitButton AccountingNumberFormatButton;
        
        internal Telerik.Windows.Controls.RadRibbonDropDownButton InsertCellsButton;
        
        internal Telerik.Windows.Controls.RadRibbonDropDownButton DeleteCellsButton;
        
        internal Telerik.Windows.Controls.RadRibbonDropDownButton FormatButton;
        
        internal Telerik.Windows.Controls.RadRibbonDropDownButton FillButton;
        
        internal Telerik.Windows.Controls.RadRibbonDropDownButton ClearButton;
        
        internal Telerik.Windows.Controls.Spreadsheet.Controls.ThemeGallery ThemeGallery;
        
        internal Telerik.Windows.Controls.Spreadsheet.Controls.ColorGallery ColorGallery;
        
        internal Telerik.Windows.Controls.Spreadsheet.Controls.FontGallery FontGallery;
        
        internal Telerik.Windows.Controls.RadRibbonDropDownButton PrintAreaButton;
        
        internal Telerik.Windows.Controls.RadRibbonDropDownButton BreaksButton;
        
        internal Telerik.Windows.Controls.RadRibbonDropDownButton FreezePanesButton;
        
        internal Telerik.Windows.Controls.RadRibbonSplitButton BringForwardButton;
        
        internal Telerik.Windows.Controls.RadRibbonSplitButton SendBackwardButton;
        
        internal Telerik.Windows.Controls.RadRibbonDropDownButton RotateButton;
        
        internal Telerik.Windows.Controls.RadRibbonContextualGroup PictureTools;
        
        internal Telerik.Windows.Controls.Spreadsheet.Controls.RadSpreadsheetFormulaBar formulaBar;
        
        internal Telerik.Windows.Controls.RadSpreadsheet radSpreadsheet;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/Modes;component/ExcelReports/ExcelReportMainView.xaml", System.UriKind.Relative));
            this.Bool2Visibility = ((Telerik.Windows.Controls.BooleanToVisibilityConverter)(this.FindName("Bool2Visibility")));
            this.InvertedBool2Visibility = ((Telerik.Windows.Controls.InvertedBooleanToVisibilityConverter)(this.FindName("InvertedBool2Visibility")));
            this.BindingsCollection = ((Telerik.Windows.Controls.ContainerBindingCollection)(this.FindName("BindingsCollection")));
            this.tree = ((Telerik.Windows.Controls.RadTreeView)(this.FindName("tree")));
            this.ContextMenu = ((Telerik.Windows.Controls.RadContextMenu)(this.FindName("ContextMenu")));
            this.detailGrid = ((System.Windows.Controls.Grid)(this.FindName("detailGrid")));
            this.chbEditMode = ((System.Windows.Controls.CheckBox)(this.FindName("chbEditMode")));
            this.stpKeyDate = ((System.Windows.Controls.StackPanel)(this.FindName("stpKeyDate")));
            this.updateBtn = ((Telerik.Windows.Controls.RadButton)(this.FindName("updateBtn")));
            this.printBtn = ((Telerik.Windows.Controls.RadButton)(this.FindName("printBtn")));
            this.ribbonView = ((Telerik.Windows.Controls.RadRibbonView)(this.FindName("ribbonView")));
            this.PasteButton = ((Telerik.Windows.Controls.RadRibbonSplitButton)(this.FindName("PasteButton")));
            this.FontStylesGroup = ((Telerik.Windows.Controls.RadButtonGroup)(this.FindName("FontStylesGroup")));
            this.MergeAndCenterButton = ((Telerik.Windows.Controls.RadRibbonSplitButton)(this.FindName("MergeAndCenterButton")));
            this.AccountingNumberFormatButton = ((Telerik.Windows.Controls.RadRibbonSplitButton)(this.FindName("AccountingNumberFormatButton")));
            this.InsertCellsButton = ((Telerik.Windows.Controls.RadRibbonDropDownButton)(this.FindName("InsertCellsButton")));
            this.DeleteCellsButton = ((Telerik.Windows.Controls.RadRibbonDropDownButton)(this.FindName("DeleteCellsButton")));
            this.FormatButton = ((Telerik.Windows.Controls.RadRibbonDropDownButton)(this.FindName("FormatButton")));
            this.FillButton = ((Telerik.Windows.Controls.RadRibbonDropDownButton)(this.FindName("FillButton")));
            this.ClearButton = ((Telerik.Windows.Controls.RadRibbonDropDownButton)(this.FindName("ClearButton")));
            this.ThemeGallery = ((Telerik.Windows.Controls.Spreadsheet.Controls.ThemeGallery)(this.FindName("ThemeGallery")));
            this.ColorGallery = ((Telerik.Windows.Controls.Spreadsheet.Controls.ColorGallery)(this.FindName("ColorGallery")));
            this.FontGallery = ((Telerik.Windows.Controls.Spreadsheet.Controls.FontGallery)(this.FindName("FontGallery")));
            this.PrintAreaButton = ((Telerik.Windows.Controls.RadRibbonDropDownButton)(this.FindName("PrintAreaButton")));
            this.BreaksButton = ((Telerik.Windows.Controls.RadRibbonDropDownButton)(this.FindName("BreaksButton")));
            this.FreezePanesButton = ((Telerik.Windows.Controls.RadRibbonDropDownButton)(this.FindName("FreezePanesButton")));
            this.BringForwardButton = ((Telerik.Windows.Controls.RadRibbonSplitButton)(this.FindName("BringForwardButton")));
            this.SendBackwardButton = ((Telerik.Windows.Controls.RadRibbonSplitButton)(this.FindName("SendBackwardButton")));
            this.RotateButton = ((Telerik.Windows.Controls.RadRibbonDropDownButton)(this.FindName("RotateButton")));
            this.PictureTools = ((Telerik.Windows.Controls.RadRibbonContextualGroup)(this.FindName("PictureTools")));
            this.formulaBar = ((Telerik.Windows.Controls.Spreadsheet.Controls.RadSpreadsheetFormulaBar)(this.FindName("formulaBar")));
            this.radSpreadsheet = ((Telerik.Windows.Controls.RadSpreadsheet)(this.FindName("radSpreadsheet")));
        }
    }
}

