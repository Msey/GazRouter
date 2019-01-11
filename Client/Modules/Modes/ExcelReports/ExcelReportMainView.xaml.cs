using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Resources;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Dialogs.EntityPicker;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.Flobus.Misc;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Elements;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;
using Microsoft.Practices.Prism.Commands;
using NLog.Internal;
using NLog.Layouts;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Data.PropertyGrid;
using Telerik.Windows.Controls.Spreadsheet;
using Telerik.Windows.Controls.Spreadsheet.Worksheets;
using Telerik.Windows.Documents.Spreadsheet.Commands;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Utils.Extensions;
using DelegateCommand = Telerik.Windows.Controls.DelegateCommand;
using UriBuilder = GazRouter.DataProviders.UriBuilder;
namespace GazRouter.Modes.ExcelReports
{
    public partial class ExcelReportMainView
    {
        private ExcelReportMainViewModel _model;
        private XlsxFormatProvider _formatProvider;
        private EntityPickerDialogViewModel _epvm;
        static ExcelReportMainView()
        {
            WorkbookFormatProvidersManager.RegisterFormatProvider(new XlsxFormatProvider());
        }
        public ExcelReportMainView()
        {
            InitializeComponent();
            _formatProvider = new XlsxFormatProvider();
            this.Loaded += ExcelReportMainView_Loaded;
        }
        /// <summary>
        /// Команда добавления ЭЛЕМЕНТА отображения СУЩНОСТИ
        /// </summary>
        private void AddEntityElement()
        {
            _epvm = new EntityPickerDialogViewModel(() =>
            {
                if (_epvm.DialogResult.HasValue && _epvm.DialogResult.Value && _epvm.SelectedItem != null)
                {
                    var editor = this.radSpreadsheet.ActiveWorksheetEditor;
                    var cells = editor?.Worksheet.Cells[editor?.Selection.ActiveRange.SelectedCellRange];
                    cells?.SetValue(_epvm.SelectedItem.Id.Convert().ToString().Replace("-", "").ToUpper());
                }
            },
            new List<EntityType>
            {
                EntityType.CompShop,
                EntityType.DistrStation,
                EntityType.MeasLine,
                EntityType.CompUnit,
                EntityType.DistrStationOutlet,
                EntityType.MeasPoint,
                EntityType.ReducingStation,
                EntityType.Valve
            });

            var ep = new EntityPickerDialogView { DataContext = _epvm };
            ep.ShowDialog();
        }
        private void ExcelReportMainView_Loaded(object sender, RoutedEventArgs e)
        {
            _model = this.DataContext as ExcelReportMainViewModel;
            _model.PropertyChanged += PropertyChanged;
            _model.ReportChanged += () => {
                if (this.radSpreadsheet.ActiveWorksheetEditor != null)
                    this.radSpreadsheet.ActiveWorksheetEditor.ShowRowColumnHeadings = _model.IsEditMode;
            };
            this.radSpreadsheet.WorkbookChanged += SaveWorkbook;
            this.radSpreadsheet.WorkbookContentChanged += SaveWorkbook;
            this.chbEditMode.Checked += ChbEditModeOnChecked;
            this.chbEditMode.Unchecked += ChbEditModeOnChecked;
            this.printBtn.Click += PrintBtn_Click;



            var addAction = new Action<object>(parameter => AddEntityElement());
            var addPredicate = new Predicate<object>(parameter =>
            {
                if (!_model.IsEditMode) return false;
                var editor = this.radSpreadsheet.ActiveWorksheetEditor;
                var cells = editor?.Worksheet.Cells[editor?.Selection.ActiveRange.SelectedCellRange];
                return cells != null;
            });
            var addItem = new RadMenuItem()
            {
                Header = "Добавить",
                Command = new SelectionDependentCommand(this.radSpreadsheet, addAction, addPredicate)
            };
            this.radSpreadsheet.WorksheetEditorContextMenu.Items.Add(addItem);


            var identifyAction = new Action<object>(parameter => IdentifyElement());
            var identifyPredicate = new Predicate<object>(parameter => _model.IsEditMode);
            var identifyItem = new RadMenuItem()
            {
                Header = "Идентифицировать",
                Command = new SelectionDependentCommand(this.radSpreadsheet, identifyAction, identifyPredicate)
            };
            this.radSpreadsheet.WorksheetEditorContextMenu.Items.Add(identifyItem);
            var item = this.radSpreadsheet.WorksheetEditorContextMenu.Items[0];
            SetEditMode(_model.IsEditMode);
        }
        private async void IdentifyElement()
        {
            var editor = this.radSpreadsheet.ActiveWorksheetEditor;
            var cells = editor?.Worksheet.Cells[editor.Selection.ActiveRange.SelectedCellRange];
            if (cells == null) return;
            var rawValue = cells.GetValue().Value.GetResultValueAsString(CellValueFormat.GeneralFormat);
            var dto = await _model.EvaluateStringAsync(rawValue);
            var content = dto == null ? @"Объект не найден" : dto.ShortPath;
            var dprms = new DialogParameters { Content = content, Header = "Идентификация объекта", IconTemplate = new DataTemplate()};
            RadWindow.Alert(dprms);
        }
        private void PrintBtn_Click(object sender, RoutedEventArgs e)
        {
            PrintWhatSettings settings = new PrintWhatSettings(ExportWhat.ActiveSheet, true);
            this.radSpreadsheet.Print(settings);
        }
        private void ChbEditModeOnChecked(object sender, RoutedEventArgs routedEventArgs)
        {
            SetEditMode(this.chbEditMode.IsChecked.GetValueOrDefault());
        }
        private void SetEditMode(bool isEditMode)
        {
            _model.IsEditMode = isEditMode;
            this.ribbonView.Visibility = isEditMode ? Visibility.Visible : Visibility.Collapsed;
            this.updateBtn.Visibility = isEditMode ? Visibility.Visible : Visibility.Collapsed;
            this.formulaBar.Visibility = isEditMode ? Visibility.Visible : Visibility.Collapsed;
            this.stpKeyDate.Visibility = !isEditMode ? Visibility.Visible : Visibility.Collapsed;

            if (this.radSpreadsheet.ActiveWorksheetEditor != null)
                this.radSpreadsheet.ActiveWorksheetEditor.ShowRowColumnHeadings = isEditMode;

        }
        private void SaveWorkbook(object source, EventArgs eventArgs)
        {
            SaveWorkbook();
        }
        private void SaveWorkbook()
        {
            try
            {
                if (_model.IsEditMode && this.radSpreadsheet.Workbook != null)
                {
                    _model.SetContent(_formatProvider.Export(this.radSpreadsheet.Workbook), true);
                }
            }
            catch (Exception e)
            {
            }
        }
        private void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {

            if (e.PropertyName == "Content")
            {
                try
                {
                    ToggleEventHandlers(true);
                    if (_model.Content != null && _model.Content.Length > 0)
                    {
                        this.radSpreadsheet.Workbook = _formatProvider.Import(_model.Content);
                    }
                    else
                    {
                        this.radSpreadsheet.Workbook = new Workbook();
                    }
                    ToggleEventHandlers(false);
                }
                catch (Exception)
                {
                }
            }
            if (e.PropertyName == "SelectedItem")
            {
                this.detailGrid.Visibility = _model.DetailGridVisibility;
            }
        }
        private void ToggleEventHandlers(bool off)
        {
            if (off)
            {
                this.radSpreadsheet.WorkbookChanged -= SaveWorkbook;
                this.radSpreadsheet.WorkbookContentChanged -= SaveWorkbook;
            }
            else
            {
                this.radSpreadsheet.WorkbookChanged += SaveWorkbook;
                this.radSpreadsheet.WorkbookContentChanged += SaveWorkbook;
            }
        }
        private void RadTreeView_PreviewDragStarted(object sender, RadTreeViewDragEventArgs e)
		{
			if (e.DraggedItems.Count == 1 && e.DraggedItems[0] is FolderItemViewModel)
				e.Handled = true;
		}
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var model = this.DataContext as ExcelReportMainViewModel;

            var item = model?.SelectedItem as ExcelReportItemViewModel ;
            var timeStamp = model?.KeyDate;
            if (timeStamp.HasValue && item != null)
            {
                HtmlPage.Window.Navigate(UriBuilder.GetXlsxUri(timeStamp.Value, item.Id));
            }
        }
    }
    public class SelectionDependentCommand : DelegateCommand
    {
        private readonly RadSpreadsheet _radSpreadsheet;
        private RadWorksheetEditor worksheetEditor;
        public SelectionDependentCommand(RadSpreadsheet radSpreadsheet, Action<object> action , Predicate<object> predicate ) : base(action, predicate)
        {
            _radSpreadsheet = radSpreadsheet;
            this._radSpreadsheet.ActiveSheetEditorChanged += RadSpreadsheetOnActiveSheetEditorChanged;
        }
        private void RadSpreadsheetOnActiveSheetEditorChanged(object sender, EventArgs eventArgs)
        {
            if (this.worksheetEditor != null)
            {
                this.worksheetEditor.Selection.SelectionChanged -= SelectionOnSelectionChanged;
            }

            this.worksheetEditor = this._radSpreadsheet.ActiveWorksheetEditor;
            if (this.worksheetEditor != null)
            {
                this.worksheetEditor.Selection.SelectionChanged += SelectionOnSelectionChanged;
            }
        }
        private void SelectionOnSelectionChanged(object sender, EventArgs eventArgs)
        {
            this.InvalidateCanExecute();
        }
    }
}
