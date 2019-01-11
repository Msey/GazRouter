using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DataServices.Infrastructure.Sessions;
using GazRouter.DAL.Core;
using GazRouter.DAL.ObjectModel.Entities;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ExcelReports;
using GazRouter.DTO.ObjectModel;
using GazRouter.Log;
using GazRouter.Service.Exchange.Lib.Excel.CellEvaluators;
using Utils.Extensions;
namespace GazRouter.Service.Exchange.Lib.Excel
{
    public class ExcelProcessor : IDisposable
    {
        private readonly DateTime _timeStamp;
        private readonly PeriodType _periodType;
        private readonly ExecutionContext _context;
        //private XLWorkbook _wb;

        public ExcelProcessor(DateTime timeStamp, PeriodType periodType, ExecutionContext context = null)
        {
            _timeStamp = timeStamp;
            _periodType = periodType;
            _context = context ?? DbContextHelper.OpenDbContext(SessionManager.SystemUserLogin, new MyLogger("exchangeLogger"));
        }
        public CommonEntityDTO EvaluateString(string value)
        {
            var m = Regex.Match(value, @"[A-F0-9]{32}");
            Guid guid;
            if (!Guid.TryParse(m.Value, out guid)) return null;

            guid = guid.Convert();
            return new GetEntityQuery(_context).Execute(guid);
        }
        public void Dispose()
        {
            _context.Dispose();
            //_wb.Dispose();
        }
        private IEnumerable<CellEvaluator> GetEvaluators()
        {
            var result = new List<CellEvaluator>()
            {
                new PvCellEvaluator(_timeStamp, _periodType, _context),
                //new PvFieldCellEvaluator(timeStamp, ctx),
                new StnObjCellEvaluator(_timeStamp, _periodType, _context),
                new StnEntCellEvaluator(_timeStamp, _periodType, _context),
                new StnSiteCellEvaluator(_timeStamp, _periodType, _context),
                new GsVolumeCellEvaluator(_timeStamp, _periodType, _context),
                new GsVolumeEntCellEvaluator(_timeStamp, _periodType, _context),
                //new GsVolumeChangeEntFieldEvaluator(timeStamp, ctx),
                //new GsVolumeEntFieldEvaluator(timeStamp, ctx),
                new GsVolumeChangeEntCellEvaluator(_timeStamp, _periodType, _context),
                new GsVolumeChangeCellEvaluator(_timeStamp, _periodType, _context),
                //new GsVolumeChangeFieldEvaluator(timeStamp, ctx),
                //new GsVolumeFieldEvaluator(timeStamp, ctx),
                new SqlCellEvaluator(_timeStamp, _periodType, _context),
                new TimeStampCellEvaluator(_timeStamp, _periodType, _context),
            };
            return result;
        }

        public List<SerializableTuple4<int, int, int, CellValue>> GetTemplateValues(IEnumerable<SerializableTuple4<int, int, int, string>> cellsToChange)
        {
            try
            {
                var cellEvaluators = GetEvaluators();

                var result1 = cellsToChange
                    .Select(c =>
                    {
                        var cellValue = c.Item4;
                        var ce = cellEvaluators.First(ce1 => ce1.IsMatch(cellValue));
                        try
                        {
                            return new SerializableTuple4<int, int, int, CellValue>(c.Item1, c.Item2, c.Item3, ce.GetValue(cellValue)); 
                        }
                        catch (Exception e)
                        {
                            return new SerializableTuple4<int, int, int, CellValue>(c.Item1, c.Item2, c.Item3,
                                new CellValue {ValueType = CellValueType.Error, RawValue = e.Message});
                        }
                    });

                return result1.ToList();
            }
            catch (Exception e)
            {
                var log = new MyLogger("mainLogger");
                log.Error(e.Message);
                return new List<SerializableTuple4<int, int, int, CellValue>>();
            }
        }
    }
}