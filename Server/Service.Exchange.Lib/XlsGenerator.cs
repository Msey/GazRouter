using GazRouter.Log;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.IO;

namespace GazRouter.Service.Exchange.Lib
{

    public sealed class XlsGenerator
    {
        private readonly MyLogger Logger;

        private Func<string, string, object> getData;

        public XlsGenerator(Func<string, string, object> getData, MyLogger logger)
        {
            this.getData = getData;
            this.Logger = logger;
        }

        public bool Process(string templateFileName, string outFileName)
        {
            try
            {
                var hssfworkbook = new HSSFWorkbook(new FileStream(templateFileName, FileMode.Open, FileAccess.Read));

                hssfworkbook.ForceFormulaRecalculation = true;

                foreach (ISheet sheet in hssfworkbook)
                {
                    foreach (IRow row in sheet)
                    {
                        foreach (var cell in row)
                        {
                            CheckAndReplaceCell(cell);
                        }
                    }
                }

                using (FileStream file = new FileStream(outFileName, FileMode.Create))
                {
                    hssfworkbook.Write(file);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error in XlsGenerator: {e}");
            }

            return true;
        }


        private void CheckAndReplaceCell(ICell cell)
        {
            try
            {
                if (cell.CellType != CellType.String)
                    return;

                var v = cell.StringCellValue;

                if (v.Contains("{{") && v.Contains("}}"))
                {
                    if (v.Contains("REPORT_DATE"))
                    {
                        cell.SetCellValue(DateTime.Now);
                    }
                    else if (v.Contains(";"))
                    {
                        var parameters = v.Split(';');
                        parameters[0] = parameters[0].Replace("{{", string.Empty).Replace("}}", string.Empty).Trim(' ');
                        parameters[1] = parameters[1].Replace("{{", string.Empty).Replace("}}", string.Empty).Trim(' ');

                        object val = getData(parameters[0], parameters[1]);
                        if (val == null)
                        {
                            cell.SetCellValue((string)null);
                        }
                        else if (val is double)
                        {
                            cell.SetCellValue(Convert.ToDouble(val));
                        }
                        else if (val is DateTime)
                        {
                            cell.SetCellValue(Convert.ToDateTime(val));
                        }
                        else
                        {
                            cell.SetCellValue(val.ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error in XlsGenerator: {e}");
            }
        }
    }
}
