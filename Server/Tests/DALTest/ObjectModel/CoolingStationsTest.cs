using System.Linq;
using GazRouter.DAL.Dictionaries.Regions;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.CoolingStations;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.CoolingStations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class CoolingStationsTest : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void FullTestCoolingStation()
        {
            var siteId = CreateSite();
            var region = new GetRegionListQuery(Context).Execute().First();
            var compStationId =
                new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet
                {
                    ParentId = siteId,
                    Name = "TestCompStation",
                    RegionId = region.Id
                });

            var coolingStationId = new AddCoolingStationCommand(Context).Execute(new AddCoolingStationParameterSet { Name = "TestCoolingStationName1", ParentId = compStationId });

            new EditCoolingStationCommand(Context).Execute(new EditCoolingStationParameterSet { Id = coolingStationId, Name = "TestCoolingStationName2" });

            var list = new GetCoolingStationsListQuery(Context).Execute(new GetCoolingStationListParameterSet(){SiteId = siteId});
            Assert.IsTrue(list.Count > 0);

            new DeleteCoolingStationCommand(Context).Execute(new DeleteEntityParameterSet { Id = coolingStationId , EntityType = EntityType.CoolingStation});
        }
    }
}
