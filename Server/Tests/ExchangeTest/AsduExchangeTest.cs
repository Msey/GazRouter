using System;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.Service.Exchange.Lib;
using GazRouter.Service.Exchange.Lib.Asdu;
using GazRouter.Service.Exchange.Lib.Export;
using GazRouter.Service.Exchange.Lib.Run;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace ExchangeTest
{
    [TestClass]
    public class AsduExchangeTest : TransactionTestsBase { 

        [TestMethod, TestCategory(UnStable)]
        public void Pt2HTest()
        {

            var periodType = PeriodType.Twohours;
            var delta = TimeSpan.FromMinutes(5);
            var divisor = periodType.ToTimeSpan().Ticks;
            var now = DateTime.Today.AddHours(14);
            Assert.IsFalse(Math.Abs(now.Ticks%divisor) > delta.Ticks);
            now = DateTime.Today.AddHours(13);
            Assert.IsTrue(Math.Abs(now.Ticks%divisor) > delta.Ticks);
            now = DateTime.Today.AddHours(13).AddMinutes(56);
            Assert.IsTrue(Math.Abs(now.Ticks%divisor) > delta.Ticks);
            now = DateTime.Today.AddHours(13).AddMinutes(62);
            Assert.IsFalse(Math.Abs(now.Ticks%divisor) > delta.Ticks);

            now = DateTime.Today.AddHours(9).AddMinutes(46);
            var exporter = new AsduExchangeObjectExporter(Context, now, periodType);
            var eo = exporter.Build();



        }

        //[TestMethod, TestCategory(Stable)]
        //public void PeriodConditionTest()
        //{

        //    var interval = 12;
        //    var periodType = PeriodType.Twohours;
        //    var now = TimeSpan.FromHours(DateTime.Now.Hour);

        //    for (int i = 0; i < 1000; i++)
        //    {
        //        var time = now + TimeSpan.FromMinutes(i*interval);
        //        Assert.IsTrue(i % 10 != 0 || TimeSpan.FromTicks(time.Ticks % (periodType.ToTimeSpan().Ticks)) <= TimeSpan.FromMinutes(3));
        //    }
        //}

    }
}