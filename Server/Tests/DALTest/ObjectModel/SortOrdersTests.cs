using System;
using System.Linq;
using GazRouter.DAL.Dictionaries.Regions;
using GazRouter.DAL.ObjectModel.Consumers;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DAL.ObjectModel.Entities;
using GazRouter.DTO.Dictionaries.ConsumerTypes;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.DistrStations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class SortOrdersTests : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
		public void SortOrdersTest()
        {
			Guid newGuidSite = CreateSite();
            var regionId = new GetRegionListQuery(Context).Execute().First().Id;
            var distrStationId =
                new AddDistrStationCommand(Context).Execute(new AddDistrStationParameterSet
                                                                {
                                                                    Name = "Тест ГРС",
                                                                    ParentId = newGuidSite
                                                                });
            var consumerId =
                new AddConsumerCommand(Context).Execute(new AddConsumerParameterSet
                                                            {
																ConsumerType = (int)ConsumerType.Energetics,
                                                                ParentId = distrStationId,
                                                                Name = "TestConsumer",
                                                                RegionId = regionId
                                                            });
			
            var edited = new GetConsumerListQuery(Context).Execute(new GetConsumerListParameterSet { DistrStationId = distrStationId }).First(l => l.Id == consumerId);
            Assert.AreEqual("TestConsumer", edited.Name);
			Assert.AreEqual(0,edited.SortOrder);
			new SetSortOrderCommand(Context).Execute(new SetSortOrderParameterSet { Id = consumerId, SortOrder = 5 });
			edited = new GetConsumerListQuery(Context).Execute(new GetConsumerListParameterSet { DistrStationId = distrStationId }).First(l => l.Id == consumerId);
			Assert.AreEqual(5, edited.SortOrder);
            new DeleteConsumerCommand(Context).Execute(new DeleteEntityParameterSet { Id = consumerId });
        }
    }
}
