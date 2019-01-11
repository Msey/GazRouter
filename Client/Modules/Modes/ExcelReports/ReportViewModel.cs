using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GazRouter.Application.Helpers;
using GazRouter.Common.Events;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ExcelReports;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ExcelReports;
using GazRouter.DTO.ObjectModel;
using GazRouter.Modes.Infopanels;
using GazRouter.Modes.Infopanels.Tree;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;
using CellValueType = GazRouter.DTO.ExcelReports.CellValueType;
using ServiceLocator = Microsoft.Practices.ServiceLocation.ServiceLocator;
namespace GazRouter.Modes.ExcelReports
{
    /// <summary>
    /// 
    /// при переключении в дереве на отчете, создается новая View и новый ViewModel 
    /// новый ViewModel - сразу грузит объект из базы;
    /// 
    /// </summary>
    public class ReportViewModel : LockableViewModel
    {
#region constructor
        public ReportViewModel()
        {
            _isEditMode = false;
            _keyDate = SeriesHelper.GetCurrentSession();
            _formatProvider = new XlsxFormatProvider();
            InitCommands();
            PropertyChanged += OnPropertiesChanged;
            TreeVisibility = true;
            Workbook = GetEmptyWorkbook();
            IsEditMode = false;
        }
#endregion
#region commands
        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand UpdateContentCommand { get; set; }
        public bool CanUpdate()
        {
            return IsEditMode.GetValueOrDefault() && IsEditable;
        }
        private async void SaveWrapper()
        {
//            Behavior.TryLock();
            await SaveContent();
//            Behavior.TryUnlock();
        }
        private void InitCommands()
        {
            RefreshCommand = new DelegateCommand(Refresh, () => true);
            UpdateContentCommand = new DelegateCommand(SaveWrapper, CanUpdate);
        }
#endregion
#region const
        private IEnumerable<string> Regexs
        {
            get
            {
                yield return @"#SQL#([^@]*(@TIMESTAMP\s*(([\+\-])([^\+\-\']+))?)?.*)";
                yield return @"#CELL_GS#([^#]*)#([^#]*)#([^#]*)#(@TIMESTAMP\s*(([+-])([^+-]+))?)";
                yield return @"#CELL_GS_CH#([^#]*)#([^#]*)#([^#]*)#(@TIMESTAMP\s*(([+-])([^+-]+))?)";
                yield return @"#CELL_GS_CH_ENT#(@TIMESTAMP\s*(([+-])([^+-]+))?)";
                yield return @"#CELL_GS_ENT#(@TIMESTAMP\s*(([+-])([^+-]+))?)";
                yield return @"#CELL_PV#(.*)#(\d*)#(\d*)#(@TIMESTAMP\s*(([+-])([^+-]+))?)";
                yield return @"#CELL_STN_ENT#([^#]*)#([^#]*)#([^#]*)#([^#]*)#(@TIMESTAMP\s*(([+-])([^+-]+))?)";
                yield return @"#CELL_STN_OBJ#([^#]*)#([^#]*)#([^#]*)#([^#]*)#([^#]*)#(@TIMESTAMP\s*(([+-])([^+-]+))?)";
                yield return @"#CELL_STN_SITE#([^#]*)#([^#]*)#([^#]*)#([^#]*)#([^#]*)#(@TIMESTAMP\s*(([+-])([^+-]+))?)";
                yield return @"@TIMESTAMP\s*(([+-])([^+-]+))?";
            }
        }
        #endregion
#region variables
        // public PanelPermission Permission { set; get; }

        private readonly XlsxFormatProvider _formatProvider;
        public Action Print;
        private int _itemId;
        private ItemBase _itemBase;
        public bool WorkbookContentChanged { get; set; }
#endregion
#region property
        private byte[] _editContent;
        private byte[] EditContent
        {
            get { return _editContent; }
            set { SetProperty(ref _editContent, value); }
        }
        public bool IsTwoHoursSelected
        {
            get { return PeriodType == PeriodType.Twohours; }
        }
        private bool _isEditable;
        public bool IsEditable
        {
            get { return _isEditable; }
            set
            {
                _isEditable = value;
                OnPropertyChanged(() => IsEditable);
            }
        }
        private bool? _isEditMode;
        public bool? IsEditMode
        {
            get { return _isEditMode; }
            set
            {
                if (SetProperty(ref _isEditMode, value))
                    UpdateContentCommand?.RaiseCanExecuteChanged();
            }
        }
        private DateTime? _keyDate;
        public DateTime? KeyDate
        {
            get { return _keyDate; }
            set
            {
                SetProperty(ref _keyDate, value);
                Refresh();
            }
        }
        private PeriodType _periodType;
        public PeriodType PeriodType
        {
            get { return _periodType; }
            set
            {
                if (_periodType == value) return;
                _periodType = value;
                OnPropertyChanged(() => IsTwoHoursSelected);
            }
        }
        private bool _treeVisibility;
        public bool TreeVisibility
        {
            get { return _treeVisibility; }
            set
            {
                SetProperty(ref _treeVisibility, value);
                ServiceLocator.Current.GetInstance<IEventAggregator>()
                    .GetEvent<VisualStateChangedEvent>().Publish(value);
            }
        }
        private Workbook _workbook;
        public Workbook Workbook
        {
            get { return _workbook; }
            set
            {
                if (_workbook == value) return;
                _workbook = value;
                OnPropertyChanged(() => Workbook);
            }
        }

        public Func<string, string> SaveMessage;
#endregion
#region events          
        public void OnPropertiesChanged(object sender, PropertyChangedEventArgs args)
        {
            switch (args.PropertyName)
            {
                case nameof(IsEditMode):
                    if (IsEditMode.GetValueOrDefault()) Workbook = GetWorkbook();
                    else EditModeToOff();
                    break;
            }
        }
#endregion
#region methods
        public void SetChanged()
        {
            if (WorkbookContentChanged) return;
            //
            WorkbookContentChanged = true;
            _itemBase.IsChanged = () => true;
        }
        public async Task SetItem(ItemBase itemBase)
        {
            _itemBase = itemBase;
            _itemBase.IsChanged = () => false;
            _itemBase.Save = SaveContent;
            WorkbookContentChanged = false;
            //
            IsEditMode = false;
            var dto = ((ReportItem)itemBase).Dto;
            if (_itemId == dto.Id && PeriodType == dto.PeriodTypeId) return;
            //
            if (dto.PeriodTypeId == PeriodType.Day) _keyDate = _keyDate?.Date;
            _itemId     = dto.Id;
            PeriodType  = dto.PeriodTypeId;
            EditContent = await LoadEditContent();
            Workbook    = GetWorkbook();

            CalcCells();
        }
        private async void CalcCells()
        {
            var dto = await GetExcelReportContent();
            ChangeCellsValue(Workbook, dto);
        }
        private async void Refresh()
        {
            var dto = await GetExcelReportContent();
            Workbook = ViewExcel(dto);
        }
        private async Task<ExcelReportContentDTO> GetExcelReportContent()
        {
            var cellsToChange = CellsToChange();
            return await
                 new ExcelReportServiceProxy().EvaluateExcelReportAsync(
                     new EvaluateExcelReportContentParameterSet
                     {
                         CellsToChange = cellsToChange.ToList(),
                         KeyDate = (DateTime)KeyDate,
                         PeriodType = PeriodType,
                         ReportId = _itemId,
                     });
        }
        private Workbook ViewExcel(ExcelReportContentDTO dto = null)
        {
            if (EditContent == null) return GetEmptyWorkbook();
            var workbook = GetExistingWorkbook();
            ChangeCellsValue(workbook, dto);
            return workbook;
        }
        private async Task<byte[]> LoadEditContent() {

            var dto = await new ExcelReportServiceProxy().GetExcelReportContentAsync(_itemId);
            return dto?.Content;
        }
        private async Task SaveContent()
        {
            var newContent = _formatProvider.Export(Workbook);
            var parameters = new ExcelReportContentDTO
            {
                ReportId = _itemId,
                Content = newContent
            };
            await new ExcelReportServiceProxy().UpdateExcelReportContentAsync(parameters);
            EditContent = newContent;
            _itemBase.IsChanged = () => false;
            WorkbookContentChanged = false;
        }
        private void EditModeToOff()
        {
            if (!IsEditMode.GetValueOrDefault() && WorkbookContentChanged)
                MessageBoxProvider.Confirm(SaveMessage(_itemBase.Name),
                    result =>
                    {
                        if (result) SaveWrapper();
                        Refresh();
                    });
            else
                Refresh();
        }
        private Workbook GetWorkbook()
        {
            var workbook = EditContent == null ? GetEmptyWorkbook() : GetExistingWorkbook();
            return workbook;
        }
        private Workbook GetEmptyWorkbook()
        {
            var workbook = new Workbook();
            return workbook;
        }
        private Workbook GetExistingWorkbook()
        {
            var workbook = _formatProvider.Import(EditContent);
            return workbook;
        }
#endregion
#region calc_cells
        private void ChangeCellsValue(Workbook workbook, ExcelReportContentDTO dto = null)
        {
            var dateTimeFormat = this.PeriodType == PeriodType.Twohours ?
                         new CellValueFormat("dd.MM.yyyy HH:mm") :
                         new CellValueFormat("dd.MM.yyyy");
            var sheets = workbook.Worksheets;
            var templateValues = dto.ChangedCells ?? new List<SerializableTuple4<int, int, int, CellValue>>();
            foreach (var tuple4 in templateValues)
            {
                var sheet = tuple4.Item1;
                var row = tuple4.Item2;
                var column = tuple4.Item3;
                var cell = sheets[sheet].Cells[row, column];
                var valueToChange = tuple4.Item4;
                switch (valueToChange?.ValueType)
                {
                    case CellValueType.Number:
                        cell.SetValue(valueToChange.Number);
                        break;
                    case CellValueType.DateTime:
                        cell.SetValue(valueToChange.DateTime);
                        cell.SetFormat(dateTimeFormat);
                        break;
                    case CellValueType.Text:
                        cell.SetValue(valueToChange.RawValue);
                        break;
                    case CellValueType.Error:
                        cell.SetValue(valueToChange.RawValue);
                        break;
                    default:
                        cell.SetValue(string.Empty);
                        break;
                }
            }
        }
        private IEnumerable<SerializableTuple4<int, int, int, string>> CellsToChange()
        {
            var sheets = GetWorkbook().Worksheets;
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var s = 0; s < sheets.Count; s++)
            {
                var sheet = sheets[s];
                var range = sheet.UsedCellRange;
                var cells = sheet.Cells;
                for (var r = 0; r < range.RowCount; r++)
                for (var c = 0; c < range.ColumnCount; c++)
                {
                    var cell = cells[r, c];
                    var cellValue = cell.GetValue().Value.RawValue;
                    var isMatch = Regexs.Any(reg => Regex.IsMatch(cellValue, reg));
                    if (isMatch) yield return 
                                new SerializableTuple4<int, int, int, string>(s, r, c, cellValue);
                }
            }
        }
        public async Task<CommonEntityDTO> EvaluateStringAsync(string input)
        {
            return await new ExcelReportServiceProxy().EvaluateStringAsync(input);
        }
#endregion
#region helpers
        private static void Log(string m1, string m2)
        {
            ServiceLocator.Current.GetInstance<IEventAggregator>()
                        .GetEvent<AddLogEntryEvent>().Publish(new Tuple<string, string>(m1, m2));
        }
#endregion
    }
}
#region trash
//            var cellsToChange = CellsToChange();
//            var dto = await GetExcelReportContent(cellsToChange);
//            ChangeCellsValue(Workbook, dto);

//            var dto = await
//                new ExcelReportServiceProxy().EvaluateExcelReportAsync(
//                    new EvaluateExcelReportContentParameterSet
//                    {
//                        CellsToChange = cellsToChange.ToList(),
//                        KeyDate = (DateTime)KeyDate,
//                        PeriodType = PeriodType,
//                        ReportId = _itemId,
//                    });

//                case nameof(Workbook):
//                    if (IsEditMode ?? false) Unprotect(Workbook);
//                    else Protect(Workbook);
//                    break; 
#endregion