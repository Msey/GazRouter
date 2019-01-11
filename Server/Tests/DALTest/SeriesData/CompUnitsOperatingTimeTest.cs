using System;
using System.Linq;
using GazRouter.DAL.Dictionaries.CompUnitTypes;
using GazRouter.DAL.Dictionaries.EntityTypes;
using GazRouter.DAL.Dictionaries.SuperchargerTypes;
using GazRouter.DAL.ObjectModel.CompUnits;
using GazRouter.DAL.SeriesData.CompUnits;
using GazRouter.DAL.SeriesData.PropertyValues;
using GazRouter.DAL.SeriesData.Series;
using GazRouter.DTO;
using GazRouter.DTO.Dictionaries.CompUnitSealingTypes;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.SeriesData
{
    [TestClass]
    public class CompUnitsOperatingTimeTest : DalTestBase
    {
        [TestMethod, TestCategory(Stable)]
        public void GetData()
        {
            var seriesId1 = new AddSeriesCommand(Context).Execute(
                new AddSeriesParameterSet
                {
                    KeyDate = DateTime.Today,
                    Description = "TestSeriesOne",
                    PeriodTypeId = PeriodType.Twohours
                });

            var seriesId2 = new AddSeriesCommand(Context).Execute(
                new AddSeriesParameterSet
                {
                    KeyDate = DateTime.Today.AddHours(2),
                    Description = "TestSeriesTwo",
                    PeriodTypeId = PeriodType.Twohours
                });

            var seriesId3 = new AddSeriesCommand(Context).Execute(
                new AddSeriesParameterSet
                {
                    KeyDate = DateTime.Today.AddHours(4),
                    Description = "TestSeriesThree",
                    PeriodTypeId = PeriodType.Twohours
                });

            var seriesId4 = new AddSeriesCommand(Context).Execute(
                new AddSeriesParameterSet
                {
                    KeyDate = DateTime.Today.AddHours(6),
                    Description = "TestSeriesFour",
                    PeriodTypeId = PeriodType.Twohours
                });

            var compShopId = GetCompShop(Context);

            var unitTypeIds = new GetCompUnitTypeListQuery(Context).Execute();
            var unitTypeId1 = unitTypeIds[0].Id;
            var superchargerTypeId = new GetSuperchargerTypesQuery(Context).Execute().First().Id;
            var compUnitId =
                new AddCompUnitCommand(Context).Execute(new AddCompUnitParameterSet
                {
                    Name = "TestUnit",
                    ParentId = compShopId,
                    CompUnitTypeId = unitTypeId1,
                    SuperchargerTypeId = superchargerTypeId,
                    SealingType = CompUnitSealingType.Dry
                });

            new SetPropertyValueCommand(Context).Execute(new SetPropertyValueParameterSet
            {
                SeriesId = seriesId1,
                EntityId = compUnitId,
                PropertyTypeId = PropertyType.CompressorUnitState,
                Value = 1.0
            });

            new SetPropertyValueCommand(Context).Execute(new SetPropertyValueParameterSet
            {
                SeriesId = seriesId2,
                EntityId = compUnitId,
                PropertyTypeId = PropertyType.CompressorUnitState,
                Value = 2.0
            });

            new SetPropertyValueCommand(Context).Execute(new SetPropertyValueParameterSet
            {
                SeriesId = seriesId3,
                EntityId = compUnitId,
                PropertyTypeId = PropertyType.CompressorUnitState,
                Value = 3.0
            });

            new SetPropertyValueCommand(Context).Execute(new SetPropertyValueParameterSet
            {
                SeriesId = seriesId4,
                EntityId = compUnitId,
                PropertyTypeId = PropertyType.CompressorUnitState,
                Value = 42.0
            });

            //var list1 = new GetCompUnitsExtCommand(Context).Execute();
            //Assert.IsTrue(list1.Any());

            var list2 = new GetOperatingTimeQuery(Context).Execute(new DateIntervalParameterSet
            {
                BeginDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59)
            });
            Assert.IsTrue(list2.Any());
        }
    }
}
