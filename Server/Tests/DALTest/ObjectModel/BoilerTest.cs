using System;
using System.Linq;
using GazRouter.DAL.Dictionaries.BoilerTypes;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.ObjectModel.Boilers;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Boilers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class BoilerTest : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void FullTestBoiler()
        {
	        var boilerTypeId = new GetBoilerTypeListQuery(Context).Execute().FirstOrDefault();
            var pipelineId = CreatePipeline(PipelineType.Branch);
            const double kilometerOfStartAfterCreation = 23;
			var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();

	        var boilerId =
		        new AddBoilerCommand(Context).Execute(new AddBoilerParameterSet
			                                                 {
				                                                 Name = "Test123",
				                                                 BoilerTypeId = boilerTypeId.Id,
				                                                 PipelineId = pipelineId,
																 Kilometer = kilometerOfStartAfterCreation,
																 GasTransportSystemId = gastransport.Id
			                                                 });
            var boilersList =
                new GetBoilerListQuery(Context).Execute(new GetBoilerListParameterSet { SystemId = gastransport.Id }).First(p => p.Id == boilerId);
            Assert.IsNotNull(boilersList);
            Assert.AreNotEqual(Guid.Empty, boilerId);

            const double kilometerOfEndAfterEditing = 25;
            new EditBoilerCommand(Context).Execute(new EditBoilerParameterSet
                {
                    Id = boilerId,
					Name = "Test123",
					BoilerTypeId = boilerTypeId.Id,
					PipelineId = pipelineId,
					Kilometer = kilometerOfEndAfterEditing
                });
            Assert.AreNotEqual(Guid.Empty, boilerId);

            var tmpBoiler = new GetBoilerByIdQuery(Context).Execute(boilerId);
            Assert.AreEqual(tmpBoiler.Id, boilerId);

            new DeleteBoilerCommand(Context).Execute(new DeleteEntityParameterSet { Id = boilerId });
        }
    }
}
