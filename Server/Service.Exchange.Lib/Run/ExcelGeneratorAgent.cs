using GazRouter.DAL.Core;
using GazRouter.DataServices.Infrastructure;
using GazRouter.Log;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;


namespace GazRouter.Service.Exchange.Lib.Run
{
    public static class ExcelGeneratorAgent
    {

        static readonly MyLogger Logger;

        static ExcelGeneratorAgent()
        {
            Logger = new MyLogger("excelGeneratorLogger");
        }

        public static void Run()
        {
            var connectionString                = AppSettingsManager.ConnectionString;
            var excelGeneratorTemplateDirectory = AppSettingsManager.ExcelGeneratorTemplateDirectory;
            var excelGeneratorOutputeDirectory  = AppSettingsManager.ExcelGeneratorOutputDirectory;            
            var excelInputFilenamePatterns      = AppSettingsManager.ExcelGeneratorInputName;
            var excelOutputFilenamePatterns     = AppSettingsManager.ExcelGeneratorOutputName;
            var queryStrings                    = AppSettingsManager.ExcelGeneratorQueryString;            

            for (int i = 0; i < queryStrings.Count; i++)
            {
                GenerateXlsFile(excelGeneratorTemplateDirectory, excelGeneratorOutputeDirectory, excelInputFilenamePatterns[i], excelOutputFilenamePatterns[i], connectionString, queryStrings[i]);
            }
        }

        private static void GenerateXlsFile(string excelGeneratorTemplateDirectory, string excelGeneratorOutputeDirectory, string templateInputName, string templateOutputName, string connectionString, string queryString)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(queryString, conn))
                    {

                        conn.Open();

                        using (var adapter = new OracleDataAdapter(cmd))
                        {
                            using (var dt = new DataTable())
                            {
                                adapter.Fill(dt);

                                var generator = new XlsGenerator((id, col) =>
                                {
                                    try
                                    {
                                        var row = dt.AsEnumerable().FirstOrDefault(r => (r["ID"].ToString() == id));
                                        if (row != null)
                                        {
                                            try
                                            {
                                                var rowItem = row[col];
                                                return rowItem;
                                            }
                                            catch (ArgumentException)
                                            {
                                                Logger.Error("Столбец " + col + " отсутствует в базе");
                                            }

                                        }
                                        return null;
                                    }
                                    catch (Exception e)
                                    {
                                        Logger.Error($"Error in XlsGenerator: {e}");
                                        return null;
                                    }
                                }, Logger);


                                string inFileName = Path.Combine(excelGeneratorTemplateDirectory, templateInputName);

                                if (File.Exists(inFileName))
                                {
                                    templateOutputName = templateOutputName
                                        .Replace("%year%", DateTime.Now.Year.ToString())
                                        .Replace("%month%", DateTime.Now.Month.ToString())
                                        .Replace("%day%", DateTime.Now.Day.ToString())
                                        .Replace("%hour%", DateTime.Now.Hour.ToString())
                                        .Replace("%minute%", DateTime.Now.Minute.ToString());

                                    var outFileName = Path.Combine(excelGeneratorOutputeDirectory, templateOutputName);
                                    Console.WriteLine($"{inFileName} -> {outFileName}");
                                    Logger.Info($"{inFileName} -> {outFileName}");

                                    generator.Process(inFileName, outFileName);

                                }
                                else
                                {
                                    Logger.Error("Oтсутствуeт файл Excel");
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Error in GenerateXlsFile: {e}");
            }
        }        
    }
}
