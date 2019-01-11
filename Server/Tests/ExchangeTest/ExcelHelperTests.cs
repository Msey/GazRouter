using System;
using GazRouter.DAL.ExcelReport;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.Service.Exchange.Lib.Excel.CellEvaluators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace ExchangeTest
{
    [TestClass()]
    public class ExcelHelperTests : TransactionTestsBase
    {

        [TestMethod(), TestCategory("UnStable")]
        public void ProcessAndSaveToFileTest()
        {
            //using (var stream = FileTools.OpenOrCreate("1.xlsx"))
            //{
            //    var timeStamp = new DateTime(2014, 5, 1,  14, 0, 0);
            //    ExcelHelper.ProcessAndSave(stream, timeStamp, "11.xlsx", Context);
            //}
        }

        [TestMethod(), TestCategory("UnStable")]
        public void RunProcTest()
        {
            var result = new SqlCellEvaluator(DateTime.Now, PeriodType.Twohours, Context).GetValue(@"#SQL#TO_CHAR(TO_DATE('@TIMESTAMP-2/24', 'dd.MM.yyyy hh24:mi:ss'),  'dd.MM.yyyy hh24:mi:ss')");
            Assert.IsNotNull(result);
        }


    }
}