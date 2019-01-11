using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using Telerik.Windows.Documents.Spreadsheet.FormatProviders.OpenXml.Xlsx;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Documents.Spreadsheet.PropertySystem;

namespace GazRouter.Common
{
    public class ExcelReport
    {
        private readonly Workbook _workbook;
        private Worksheet _worksheet;
        int _currentRow;
        private int _currentCol;
        private const string TableCell = "tablecell";
        private const string TableHeaderCell = "tableheadercell";

        public int currentRow => _currentRow;
        public int currentColumn => _currentCol;

        public ExcelReport(string sheetName = null)
        {
            _workbook = new Workbook();
            _worksheet = _workbook.Worksheets.Add();
            if (!string.IsNullOrEmpty(sheetName)) _worksheet.Name = sheetName;

            CreateStyles();
        }
        public ExcelReport(Dictionary<string, CellStyle> CustomStylesDic, string sheetName = null):
            this(sheetName)
        {
            foreach (var CustomStyleEntry in CustomStylesDic)
            {
                var CustomStyle = CustomStyleEntry.Value;
                var AddedStyle =_workbook.Styles.Add(CustomStyleEntry.Key);
                AddedStyle.BeginUpdate();
                AddedStyle.BottomBorder = CustomStyle.BottomBorder;
                AddedStyle.LeftBorder = CustomStyle.LeftBorder;
                AddedStyle.RightBorder = CustomStyle.RightBorder;
                AddedStyle.TopBorder = CustomStyle.TopBorder;
                AddedStyle.DiagonalDownBorder = CustomStyle.DiagonalDownBorder;
                AddedStyle.DiagonalUpBorder = CustomStyle.DiagonalUpBorder;
                AddedStyle.Fill = CustomStyle.Fill;
                AddedStyle.FontFamily = CustomStyle.FontFamily;
                AddedStyle.FontSize = CustomStyle.FontSize;
                AddedStyle.ForeColor = CustomStyle.ForeColor;
                AddedStyle.Format = CustomStyle.Format;
                AddedStyle.HorizontalAlignment = CustomStyle.HorizontalAlignment;
                AddedStyle.VerticalAlignment = CustomStyle.VerticalAlignment;
                AddedStyle.IncludeAlignment = CustomStyle.IncludeAlignment;
                AddedStyle.IncludeBorder = CustomStyle.IncludeBorder;
                AddedStyle.IncludeFill = CustomStyle.IncludeFill;
                AddedStyle.IncludeFont = CustomStyle.IncludeFont;
                AddedStyle.IncludeNumber = CustomStyle.IncludeNumber;
                AddedStyle.IncludeProtection = CustomStyle.IncludeProtection;
                AddedStyle.Indent = CustomStyle.Indent;
                AddedStyle.IsBold = CustomStyle.IsBold;
                AddedStyle.IsItalic = CustomStyle.IsItalic;
                AddedStyle.Underline = CustomStyle.Underline;
                AddedStyle.IsWrapped = CustomStyle.IsWrapped;
                AddedStyle.IsLocked = CustomStyle.IsLocked;
                AddedStyle.EndUpdate();
            }
        }

        private void CreateStyles()
        {
     
            var cellStyle = _workbook.Styles.Add(TableHeaderCell);
            cellStyle.BeginUpdate();

            var border = new CellBorder(CellBorderStyle.Hair, new ThemableColor(Colors.Black));
            cellStyle.BottomBorder = border;
            cellStyle.TopBorder = border;
            cellStyle.LeftBorder = border;
            cellStyle.RightBorder = border;


            cellStyle.Fill = new PatternFill(PatternType.Solid, Color.FromArgb(255, 255, 201, 14), Color.FromArgb(255, 255, 201, 14));
            cellStyle.IsBold = true;
            cellStyle.IsWrapped = true;
            cellStyle.EndUpdate();


            cellStyle = _workbook.Styles.Add(TableCell);
            cellStyle.BeginUpdate();

          
            cellStyle.BottomBorder = border;
            cellStyle.TopBorder = border;
            cellStyle.LeftBorder = border;
            cellStyle.RightBorder = border;

            cellStyle.IsWrapped = true;
            cellStyle.EndUpdate();
        }

        private CellSelection CurrentCell
        {
            get { return _worksheet.Cells[_currentRow, _currentCol]; }
        }

        public ExcelReport Write(object value, bool move_next = true)
        {
            if (value == null) CurrentCell.SetValue(string.Empty);
            else switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:  CurrentCell.SetValue((bool)value); break;
                case TypeCode.Double:   CurrentCell.SetValue((double)value); break;
                case TypeCode.DateTime: CurrentCell.SetValue((DateTime)value); break;
                case TypeCode.String: CurrentCell.SetValue((string)value); break;
                default: CurrentCell.SetValue(value.ToString()); break;
            }
            if (move_next) _currentCol++;
            return this;
        }

        public ExcelReport WriteCell(object value, bool move_next = true)
        {
            return WriteCell(value, TableCell, move_next);
        }
        public ExcelReport WriteCell(object value,string CellStyleName, bool move_next = true)
        {
            CurrentCell.SetStyleName(CellStyleName);
            return Write(value, move_next);
        }

        public ExcelReport WriteHeader(object value, int width)
        {
            return WriteHeader(value, width, TableHeaderCell);
        }
        public ExcelReport WriteHeader(object value, int width, string CellStyleName)
        {
            CurrentCell.SetStyleName(CellStyleName);
            _worksheet.Columns[_currentCol].SetWidth(new ColumnWidth(width, true));
            return Write(value);
        }

        public ExcelReport Move(int rowIndex, int columnIndex, string sheetName = null)
        {
            if(!string.IsNullOrEmpty(sheetName))
            {
                var v = _workbook.Worksheets.GetByName(sheetName);
                if (v == null)
                {
                    v = _workbook.Worksheets.Add();
                    v.Name = sheetName;
                }
                _worksheet = v;
            }
            _currentCol = columnIndex;
            _currentRow = rowIndex;
            return this;
        }

        public ExcelReport MoveNextCell()
        {
            return Move(_currentRow, _currentCol + 1);
        }

        public ExcelReport NewRow(int count = 1, int skipColumn = 0)
        {
            return Move(_currentRow + count, skipColumn);
        }

        public ExcelReport SkipColumn(int count = 1)
        {
            return Move(_currentRow, _currentCol + count);
        }

        public void Save(Stream stream)
        {
            new XlsxFormatProvider().Export(_workbook, stream);
        }

        //

        public ExcelReport SetSolidFill(Color color)
        {
            CurrentCell.SetFill(PatternFill.CreateSolidFill(color));
            return this;
        }

        public ExcelReport SetForeColor(Color color)
        {
            CurrentCell.SetForeColor(new ThemableColor(color));
            return this;
        }

        public ExcelReport SetFontSize(double value)
        {
            CurrentCell.SetFontSize(value);
            return this;
        }

        public ExcelReport SetIsBold(bool value)
        {
            CurrentCell.SetIsBold(value);
            return this;
        }

        //

        public ExcelReport SetCustomFill(IFill Fill)
        {
            CurrentCell.SetFill(Fill);
            return this;
        }

        public ExcelReport SetVerticalAlignment(RadVerticalAlignment Alignment)
        {
            CurrentCell.SetVerticalAlignment(Alignment);
            return this;
        }

        public ExcelReport SetHorizontaAlignment(RadHorizontalAlignment Alignment)
        {
            CurrentCell.SetHorizontalAlignment(Alignment);
            return this;
        }

        public bool MergeCells(int r1, int c1, int r2, int c2)
        {
            CellSelection MergedCells;
            return MergeCells(r1, c1, r2, c2, out MergedCells);
        }

        public bool MergeCells(int r1, int c1, int r2, int c2, out CellSelection MergedCells)
        {
            CellIndex C1 = new CellIndex(r1, c1);
            CellIndex C2 = new CellIndex(r2, c2);
            MergedCells = _worksheet.Cells[C1, C2];
            return MergedCells.Merge();
        }

        public bool MergeCells(int right, int down=0)
        {
            CellSelection MergedCells;
            return MergeCells(_currentRow, _currentCol, _currentRow+down, _currentCol+right, out MergedCells);
        }

        public bool MergeCells(int right, int down, out CellSelection MergedCells)
        {
            return MergeCells(_currentRow, _currentCol, _currentRow + down, _currentCol + right, out MergedCells);
        }

        public CellSelection SelectCells(int r1, int c1, int r2, int c2)
        {
            CellIndex C1 = new CellIndex(r1, c1);
            CellIndex C2 = new CellIndex(r2, c2);
            return _worksheet.Cells[C1, C2];
        }

        public CellSelection SelectCellsFromCurrent(int right, int down = 0)
        {
            return SelectCells(_currentRow, _currentCol, _currentRow + down, _currentCol + right);
        }

        public void SetAutoFilterRange(int r1, int c1, int r2, int c2)
        {
            _worksheet.Filter.FilterRange = new CellRange(r1, c1, r2, c2);
        }
        //
    }
}