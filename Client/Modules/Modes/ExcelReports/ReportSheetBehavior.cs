using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using GazRouter.Controls.Dialogs.EntityPicker;
using GazRouter.DTO.Dictionaries.EntityTypes;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Spreadsheet;
using Telerik.Windows.Controls.Spreadsheet.Worksheets;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Documents.Spreadsheet.Model.Protection;
using Utils.Extensions;
namespace GazRouter.Modes.ExcelReports
{
    public class ReportSheetBehavior : Behavior<UIElement>
    {
#region const
        private const string LockPassword = "8ec4787e-fbfa-4078-b60a-19dc6841d036";
#endregion
#region variables
        private ReportViewModel _model;
        private EntityPickerDialogViewModel _epvm;
        private XlsxFormatProvider _formatProvider;
        private bool IsEditMode => _model.IsEditMode ?? false;
        private ReportSheet _reportSheet;
#endregion
#region events
        protected override void OnAttached()
        {
            base.OnAttached();
            //
            WorkbookFormatProvidersManager.RegisterFormatProvider(new XlsxFormatProvider());
            _formatProvider = new XlsxFormatProvider();
            _reportSheet = AssociatedObject as ReportSheet;
            _reportSheet.Loaded += Init;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            _reportSheet.Loaded -= Init;
        }
        private void OnWorkbookContentChanged(object o, EventArgs args)
        {
            if (IsEditMode) _model.SetChanged();
        }
        private void OnActiveSheetEditorChanged(object sender, EventArgs e)
        {
            SetWorkbookEditMode(IsEditMode);
        }
        private void OnChbEditModeChenged(object sender, RoutedEventArgs routedEventArgs)
        {
            var checkBox = sender as CheckBox;
            if (checkBox == null) return;
            SetWorkbookEditMode(checkBox.IsChecked.GetValueOrDefault());
        }
#endregion
#region methods
        private void Init(object sender, RoutedEventArgs e)
        {
            _model = _reportSheet.DataContext as ReportViewModel;
            if (_model == null) return;
            //
            _reportSheet.chbEditMode.Checked += OnChbEditModeChenged;
            _reportSheet.chbEditMode.Unchecked += OnChbEditModeChenged;
            _reportSheet.BtnPrint.Click += (o, args) => { Print(); };
            var addAction = new Action<object>(parameter => AddEntityElement());
            var addItem = new RadMenuItem
            {
                Header = "Добавить",
                Command = new SelectionDependentCommand(_reportSheet.radSpreadsheet, addAction, GetAddPredicate())
            };
            _reportSheet.radSpreadsheet.WorksheetEditorContextMenu.Items.Add(addItem);
            var identifyAction = new Action<object>(parameter => IdentifyElement());
            var identifyPredicate = new Predicate<object>(parameter => IsEditMode);
            var identifyItem = new RadMenuItem
            {
                Header = "Идентифицировать",
                Command = new SelectionDependentCommand(_reportSheet.radSpreadsheet, identifyAction, identifyPredicate)
            };
            _reportSheet.radSpreadsheet.WorkbookContentChanged += OnWorkbookContentChanged;
            _reportSheet.radSpreadsheet.WorksheetEditorContextMenu.Items.Add(identifyItem);
            _reportSheet.radSpreadsheet.ActiveSheetEditorChanged += OnActiveSheetEditorChanged;
            SetWorkbookEditMode(false);
//            Loaded -= Init; todo:
        }
        private void Print()
        {
            var settings = new PrintWhatSettings(ExportWhat.ActiveSheet, true);
            _reportSheet.radSpreadsheet.Print(settings);
        }
        /// <summary> Команда добавления ЭЛЕМЕНТА отображения СУЩНОСТИ </summary>
        private void AddEntityElement()
        {
            _epvm = new EntityPickerDialogViewModel(() =>
            {
                if (_epvm.DialogResult.HasValue && _epvm.DialogResult.Value && _epvm.SelectedItem != null)
                {
                    var editor = _reportSheet.radSpreadsheet.ActiveWorksheetEditor;
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
        private async void IdentifyElement()
        {
            var editor = _reportSheet.radSpreadsheet.ActiveWorksheetEditor;
            var cells = editor?.Worksheet.Cells[editor.Selection.ActiveRange.SelectedCellRange];
            if (cells == null) return;
            var rawValue = cells.GetValue().Value.GetResultValueAsString(CellValueFormat.GeneralFormat);
            var dto = await _model.EvaluateStringAsync(rawValue);
            var content = dto == null ? @"Объект не найден" : dto.ShortPath;
            var dprms = new DialogParameters { Content = content, Header = "Идентификация объекта", IconTemplate = new DataTemplate() };
            RadWindow.Alert(dprms);
        }
        private Predicate<object> GetAddPredicate()
        {
            return parameter =>
            {
                if (!IsEditMode) return false;
                var editor = _reportSheet.radSpreadsheet.ActiveWorksheetEditor;
                var cells = editor?.Worksheet.Cells[editor?.Selection.ActiveRange.SelectedCellRange];
                return cells != null;
            };
        }
        private void SetWorkbookEditMode(bool isEditMode)
        {
            if (_reportSheet.radSpreadsheet.ActiveWorksheetEditor == null) return;
            if (isEditMode) SetWorkbookToEditMode();
            else SetWorkbookToReadMode();
        }
        private void SetWorkbookToEditMode()
        {
            _reportSheet.ribbonView.Height = 100;
            _reportSheet.formulaBar.Visibility = Visibility.Visible;
            _reportSheet.radSpreadsheet.ActiveWorksheetEditor.ShowRowColumnHeadings = true;
            Unprotect(_reportSheet.radSpreadsheet.Workbook);
        }
        private void SetWorkbookToReadMode()
        {
            _reportSheet.ribbonView.Height = 0;
            _reportSheet.formulaBar.Visibility = Visibility.Collapsed;
            _reportSheet.radSpreadsheet.ActiveWorksheetEditor.ShowRowColumnHeadings = false;
            Protect(_reportSheet.radSpreadsheet.Workbook);
        }
        public void Protect(Workbook workbook)
        {
            if (!workbook.IsProtected) workbook.Protect(LockPassword);
            foreach (var sheet in workbook.Worksheets)
                if (!sheet.IsProtected)
                    sheet.Protect(LockPassword, WorksheetProtectionOptions.Default);
        }
        public void Unprotect(Workbook workbook)
        {
            if (workbook.IsProtected) workbook.Unprotect(LockPassword);
            foreach (var sheet in workbook.Worksheets)
                if (sheet.IsProtected)
                    sheet.Unprotect(LockPassword);
        }
#endregion
    }
    public class SelectionDependentCommand : DelegateCommand
    {
        private readonly RadSpreadsheet _radSpreadsheet;
        private RadWorksheetEditor _worksheetEditor;

        public SelectionDependentCommand(RadSpreadsheet radSpreadsheet, Action<object> action, Predicate<object> predicate)
            : base(action, predicate)
        {
            this._radSpreadsheet = radSpreadsheet;
            this._radSpreadsheet.ActiveSheetEditorChanged += this.RadSpreadsheetActiveSheetEditorChanged;
        }
        private void RadSpreadsheetActiveSheetEditorChanged(object sender, EventArgs e)
        {
            if (this._worksheetEditor != null)
            {
                this._worksheetEditor.Selection.SelectionChanged -= this.Selection_SelectionChanged;
            }
            this._worksheetEditor = this._radSpreadsheet.ActiveWorksheetEditor;
            if (this._worksheetEditor != null)
            {
                this._worksheetEditor.Selection.SelectionChanged += this.Selection_SelectionChanged;
            }
        }
        private void Selection_SelectionChanged(object sender, EventArgs e)
        {
            this.InvalidateCanExecute();
        }
    }
}
