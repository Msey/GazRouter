using System;
using GazRouter.DAL.SeriesData.Series;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.SeriesData.Series;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.SeriesData
{
    [TestClass]
    public class SeriesTest : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void SeriesList()
        {
            {
                var fakedate = new DateTime(1913, 1, 1);
                var seriesId = new AddSeriesCommand(Context).Execute(new AddSeriesParameterSet { KeyDate = fakedate, Description = "TestSeries", PeriodTypeId = PeriodType.Twohours });
                var list = new GetSeriesListQuery(Context).Execute(new GetSeriesListParameterSet { PeriodType = PeriodType.Twohours, PeriodStart = fakedate, PeriodEnd = fakedate});
                var list1 = new GetSeriesQuery(Context).Execute(new GetSeriesParameterSet { TimeStamp = fakedate, PeriodType = PeriodType.Twohours});
                
                Assert.IsTrue(list != null && list.Count > 0);
                Assert.IsTrue(list1 != null);
                
                new DeleteSeriesCommand(Context).Execute(seriesId);
            }
        }
    }
}