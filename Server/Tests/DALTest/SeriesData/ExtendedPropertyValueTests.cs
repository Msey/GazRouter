using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.Dictionaries.Sources;
using GazRouter.DAL.SeriesData.ExtendedPropertyValues;
using GazRouter.DAL.SeriesData.PropertyValues;
using GazRouter.DAL.SeriesData.Series;
using GazRouter.DTO.Bindings.PropertyBindings;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.ExtendedPropertyValues;
using GazRouter.DTO.SeriesData.PropertyValues;
using GazRouter.DTO.SeriesData.Series;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.SeriesData
{
   
    [TestClass]
    public class ExtendedPropertyValue : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void ExtendedPropertyValueTest()
        {
            var sourceId = new GetSourcesListQuery(Context).Execute().First().Id;

            var seriesId = new AddSeriesCommand(Context).Execute(
                new AddSeriesParameterSet
                    {
                        KeyDate = DateTime.Now,
                        Description = "TestSeries",
                        PeriodTypeId = PeriodType.Twohours
                    });


            var siteId =
                CreateSite();

            var propertyTypeId = PropertyType.PressureInlet;

            new SetPropertyValueCommand(Context).Execute(new SetPropertyValueParameterSet
                {
                    SeriesId = seriesId,
                    EntityId = siteId,
                    PropertyTypeId = propertyTypeId,
                    Value = 123.0
                });

            var valuesList =
                new GetExtendedPropertyValuesListQuery(Context).Execute(
                    new GetExtendedPropertyValuesParameterSet
                        {
                            SeriesId = seriesId,
                            SourceId = sourceId,
                            PageNumber = 0,
                            PageSize = 10,
                            EntityType = new List<EntityType>()
                        });

            Assert.IsTrue(valuesList != null && valuesList.TotalCount > 0);
        }
    }
}