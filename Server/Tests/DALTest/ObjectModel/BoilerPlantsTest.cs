using System.Linq;
using GazRouter.DAL.ObjectModel.BoilerPlants;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.BoilerPlants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ObjectModel
{
    [TestClass]
	public class BoilerPlantsTest : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
		public void FullTestBoilerPlants()
        {
	        var boiler =
				new AddBoilerPlantCommand(Context).Execute(new AddBoilerPlantParameterSet
			                                                 {
				                                                 Name = "Test123"
			                                                 });
            var boilersList =
				new GetBoilerPlantListQuery(Context).Execute(null).First(p => p.Id == boiler);
            Assert.IsNotNull(boilersList);
	        var plant = new GetBoilerPlantByIdQuery(Context).Execute(boiler);
			Assert.IsNotNull(plant);
			new EditBoilerPlantCommand(Context).Execute(new EditBoilerPlantParameterSet
                {
					Id = boiler,
					Name = "Test12345"
                });
			plant =
				new GetBoilerPlantByIdQuery(Context).Execute(boiler);
			Assert.IsNotNull(plant);
			Assert.AreEqual(plant.Name, "Test12345");
			new DeleteBoilerPlantCommand(Context).Execute(new DeleteEntityParameterSet { Id = boiler });
        }
    }
}
