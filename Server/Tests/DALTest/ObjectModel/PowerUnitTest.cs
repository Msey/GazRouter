using System;
using System.Linq;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.Dictionaries.PowerUnitTypes;
using GazRouter.DAL.ObjectModel.PowerUnits;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.PowerUnits;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class PowerUnitTest : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void FullTestPowerUnit()
        {
	        var powerUnitTypeId = new GetPowerUnitTypeListQuery(Context).Execute().FirstOrDefault();
            var pipelineId = CreatePipeline(PipelineType.Branch);
            const double kilometerOfStartAfterCreation = 23;
			var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
	        var powerunitis =
		        new AddPowerUnitCommand(Context).Execute(new AddPowerUnitParameterSet
			                                                 {
				                                                 Name = "Test123",
				                                                 GasTransportSystemId = 1,
				                                                 PowerUnitTypeId = powerUnitTypeId.Id,
				                                                 PipelineId = pipelineId,
				                                                 Kilometer = kilometerOfStartAfterCreation
			                                                 });

            var pipeSegmentList =
				new GetPowerUnitListQuery(Context).Execute(new GetPowerUnitListParameterSet {SystemId = gastransport.Id }).First(p => p.Id == powerunitis);
            Assert.IsNotNull(pipeSegmentList);
			Assert.AreNotEqual(Guid.Empty, powerunitis);

            const double kilometerOfEndAfterEditing = 25;
            new EditPowerUnitCommand(Context).Execute(new EditPowerUnitParameterSet
                {
					Id = powerunitis,
					Name = "Test123",
					GasTransportSystemId = 1,
					PowerUnitTypeId = powerUnitTypeId.Id,
					PipelineId = pipelineId,
					Kilometer = kilometerOfEndAfterEditing
                });
			Assert.AreNotEqual(Guid.Empty, powerunitis);

			new DeletePowerUnitCommand(Context).Execute(new DeleteEntityParameterSet { Id = powerunitis });
        }
    }
}
