using System;
using System.Linq;
using GazRouter.DAL.Dictionaries.Regions;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.PowerPlants;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.Regions;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.PowerPlants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class PowerPlantsTest : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void FullTestPowerPlants()
        {
			Guid siteId = CreateSite();
			RegionDTO region = new GetRegionListQuery(Context).Execute().First();
			Guid compStationId =
				new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet
				{
					ParentId = siteId,
					Name = "TestCompStation",
					RegionId = region.Id
				});
			Assert.AreNotEqual(Guid.Empty, compStationId);
	        var powerPlantsis =
		        new AddPowerPlantCommand(Context).Execute(new AddPowerPlantParameterSet
			                                                 {
				                                                 Name = "Test123",
																 ParentId = compStationId
			                                                 });
			Assert.AreNotEqual(Guid.Empty, powerPlantsis);
            var plantsList =
				new GetPowerPlantListQuery(Context).Execute(null).First(p => p.Id == powerPlantsis);
			Assert.IsNotNull(plantsList);
			Assert.AreNotEqual(Guid.Empty, plantsList.Id);
			plantsList =
				new GetPowerPlantByIdQuery(Context).Execute(powerPlantsis);
			Assert.IsNotNull(plantsList);
			Assert.AreNotEqual(Guid.Empty, plantsList.Id);

            new EditPowerPlantCommand(Context).Execute(new EditPowerPlantParameterSet
                {
					Id = powerPlantsis,
					Name = "Test123"
                });
			Assert.AreNotEqual(Guid.Empty, powerPlantsis);

			new DeletePowerPlantCommand(Context).Execute(new DeleteEntityParameterSet { Id = powerPlantsis });
			new DeleteCompStationCommand(Context).Execute(new DeleteEntityParameterSet
			{
				EntityType = EntityType.CompStation,
				Id = compStationId
			});
        }
    }
}
