using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.Dictionaries.EntityTypes;
using GazRouter.DAL.SeriesData.PropertyValues;
using GazRouter.DAL.SeriesData.Series;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.SeriesData
{
    [TestClass]
    public class PropertyValueTests : DalTestBase
    {
        [TestMethod, TestCategory(Stable)]
        public void PropertyValueTest()
        {
            var csId = GetCompShop(Context);
            
            var seriesId = new AddSeriesCommand(Context).Execute(new AddSeriesParameterSet
            {
                PeriodTypeId = PeriodType.Twohours,
                KeyDate = new DateTime(2010, 1, 1),
                Description = string.Empty
            });
            new SetPropertyValueCommand(Context).Execute(new SetPropertyValueParameterSet
            {
                SeriesId = seriesId,
                EntityId = csId,
                PropertyTypeId = PropertyType.PressureInlet,
                Value = 10
            });

            var value = new GetPropertyValueQuery(Context).Execute(new GetPropertyValueParameterSet
            {
                EntityId = csId,
                PeriodTypeId = PeriodType.Twohours,
                PropertyTypeId = PropertyType.PressureInlet,
                Timestamp = new DateTime(2010, 1, 1)
            });
            Assert.IsTrue(value != null);

            var proplist =
                new GetPropertyValueListQuery(Context).Execute(new GetPropertyValueListParameterSet
                {
                    EntityId = csId,
                    PeriodTypeId = PeriodType.Twohours,
                    PropertyTypeId = PropertyType.PressureInlet,
                    StartDate = new DateTime(2010, 1, 1),
                    EndDate = new DateTime(2010, 1, 1).AddDays(1)
                });
            Assert.IsTrue(proplist.Count >= 1);


            var entlist = new GetEntityPropertyValueListQuery(Context).Execute(new GetEntityPropertyValueListParameterSet
            {
                PeriodType = PeriodType.Twohours,
                StartDate = new DateTime(2010, 1, 1),
                EndDate = new DateTime(2010, 1, 1).AddDays(1),
                EntityIdList = { csId }
            });
            AssertHelper.IsNotEmpty(entlist);
        }
    }
}