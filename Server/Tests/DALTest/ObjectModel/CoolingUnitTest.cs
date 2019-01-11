using System;
using System.Linq;
using GazRouter.DAL.Dictionaries.CoolingUnitTypes;
using GazRouter.DAL.Dictionaries.Regions;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.CoolingStations;
using GazRouter.DAL.ObjectModel.CoolingUnit;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.CoolingStations;
using GazRouter.DTO.ObjectModel.CoolingUnit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class CoolingUnitTest : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void FullTestCoolingStation()
        {
            var siteId = CreateSite();
            var region = new GetRegionListQuery(Context).Execute().First();
			var coolingUnitTypeId = new GetCoolingUnitTypeListQuery(Context).Execute().FirstOrDefault();
			Assert.IsNotNull(coolingUnitTypeId);
            var compStationId =
                new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet
                {
                    ParentId = siteId,
                    Name = "TestCompStation",
                    RegionId = region.Id
                });

            var coolingStationId = new AddCoolingStationCommand(Context).Execute(new AddCoolingStationParameterSet { Name = "TestCoolingStationName1", ParentId = compStationId });

			var coolingUnitId =
			  new AddCoolingUnitCommand(Context).Execute(new AddCoolingUnitParameterSet
			  {
				  Name = "Test123",
				  CoolintUnitType = coolingUnitTypeId.Id,
				  ParentId = coolingStationId
			  });
			Assert.AreNotEqual(Guid.Empty, coolingUnitId);

			var coolingUnit =
				new GetCoolingUnitListQuery(Context).Execute(new GetCoolingUnitListParameterSet()).First(p => p.Id == coolingUnitId);
			Assert.IsNotNull(coolingUnit);
			Assert.AreEqual(coolingUnit.Name, "Test123");
			new EditCoolingUnitCommand(Context).Execute(new EditCoolingUnitParameterSet
			{
				Id = coolingUnitId,
				Name = "Test123123",
				CoolintUnitType = coolingUnitTypeId.Id
			});
			coolingUnit =
				new GetCoolingUnitListQuery(Context).Execute(new GetCoolingUnitListParameterSet()).First(p => p.Id == coolingUnitId);
			Assert.IsNotNull(coolingUnit);
			Assert.AreEqual(coolingUnit.Name, "Test123123");

			new DeleteCoolingUnitCommand(Context).Execute(new DeleteEntityParameterSet { Id = coolingUnit.Id });


            new DeleteCoolingStationCommand(Context).Execute(new DeleteEntityParameterSet { Id = coolingStationId , EntityType = EntityType.CoolingStation});
        }
    }
}
