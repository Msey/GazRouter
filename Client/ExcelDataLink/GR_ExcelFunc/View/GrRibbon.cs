using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ExcelDna.Integration;
using ExcelDna.Integration.CustomUI;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GR_ExcelFunc.Model;


namespace GR_ExcelFunc.View
{
    /// <summary>
    /// в классе содержатся процедуры, выполняющиеся при нажатии кнопок в риббоне
    /// </summary>
    [ComVisible(true)]
    public class GrRibbon : ExcelRibbon
    {
        public void OnShowTaskPane(IRibbonControl control)
        {
            //MessageBox.Show("Ид контрола: " + control.Id);
            
            TaskPaneManager.ShowTaskPane();
            
        }

        public void OnShowPropertyDialog(IRibbonControl control)
        {
            WinFormManager.ShowForm();
        }
        public void OnDeleteTaskPane(IRibbonControl control)
        {
            TaskPaneManager.DeleteTaskPane();
        }

        public static void FillEnterprises()
        {
            var v = new ExcelReference(0, 0);
            v.SetValue("Предприятия");
        }


        public static void FillSites()
        {
            FillExcelSheetFromSelectedCell(new ExcelData().SiteList());
        }

        public static void FillCompShops()
        {
            FillExcelSheetFromSelectedCell(new ExcelData().CompShopList());
        }

        public static void FillCompStations()
        {
            FillExcelSheetFromSelectedCell(new ExcelData().CompStationList());
        }
        public static void FillCompUnits()
        {
            FillExcelSheetFromSelectedCell(new ExcelData().CompUnitList());
        }

        public static void FillDistrStations()
        {
            FillExcelSheetFromSelectedCell(new ExcelData().DistrStationList());
        }
        public static void FillExcelSheetFromSelectedCell(object[,] data)
        {
            ExcelReference selection = (ExcelReference)XlCall.Excel(XlCall.xlfSelection);
            
            var resultRows = selection.RowFirst + data.GetLength(0);
            var resultCols = selection.ColumnFirst + data.GetLength(1);
            var v = new ExcelReference(selection.RowFirst, resultRows - 1, selection.ColumnFirst, resultCols - 1);
            v.SetValue(data);
        }
        public void OnKsButtonPressed(IRibbonControl control)
        {
            MessageBox.Show("Ид контрола: " + control.Id);
            FillEnterprises();
        }


        public static void DoTest()
        {
            dynamic app = ExcelDnaUtil.Application;
            dynamic wb = app.ActiveWorkbook;
            dynamic ws = app.ActiveSheet;
            ws.Name = "Объекты ИУС ПТП";
            dynamic range = ws.Range["A1"];
            range.Formula = "=ShortObjectPathById(\"4BF48F005A2A4F9E809C1DED7D160D70\")";

        }
    }
    [ComVisible(true)]
    public class FillExcel
    {
        public static void FillData(DateTime start, DateTime end, Guid id, PropertyType property, PeriodType period)
        {
            try
            {
                dynamic app = ExcelDnaUtil.Application;
                dynamic wb = app.ActiveWorkbook;
                dynamic ws = app.ActiveSheet;
                //ws.Name = "Объекты ИУС ПТП";
                //dynamic range = ws.Range["A1"];
                //range.Formula = "=ShortObjectPathById(\"4BF48F005A2A4F9E809C1DED7D160D70\")";

                var t1 = DateTime.Now;

                var data = new ExcelData().PropertyData(start, end, id, property, period);

                var t2 = DateTime.Now - t1;
                var startRow = 1;
                var startColumn = 1;
                if (app.ActiveCell != null)
                {
                    startRow = app.ActiveCell.Row;
                    startColumn = app.ActiveCell.Column;
                }

                for (var i = 0; i < data.GetLength(0); i++)
                {
                    ws.Cells(i + startRow, startColumn).Value = data[i, 0];
                    ws.Cells(i + startRow, startColumn + 1).Value = data[i, 1];
                }
                ws.Cells(1, startColumn + 2).Value = t2.ToString();
            }
            catch (Exception err)
            {
                MessageBox.Show("Ошибка: " + err.Message);
            }

        }
    }
}
