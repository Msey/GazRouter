using System.Linq;
using GazRouter.DAL.Dictionaries.GasTransportSystem;
using GazRouter.DAL.ObjectModel.Entities;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Pipelines;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class PipelinesTests : TransactionTestsBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void FullTest()
        {
            const double kilometerOfStartAfterCreation = 0.23;
            var gastransport = new GetGasTransportSystemListQuery(Context).Execute().First();
            var pipelineId =
                new AddPipelineCommand(Context).Execute(new AddPipelineParameterSet
                    {
                        Name = "Pipeline1",
                        PipelineTypeId = PipelineType.Bridge,
                        Hidden = true,
                        SortOrder = 1,
                        KilometerOfStart = kilometerOfStartAfterCreation,
                        KilometerOfEnd = 0.43,
                        GasTransportSystemId = gastransport.Id
                    });

            Assert.AreNotEqual(default(int), pipelineId);

            const string bulshitName = "asdfasdfasfdasfdf";
            const double kilometerOfStartAfterEditing = 0.24;
            const double editedKmOnEnd = 0.3;
            new EditPipelinesCommand(Context).Execute(new EditPipelineParameterSet
                {
                    Id = pipelineId,
                    Name = bulshitName,
                    PipelineTypeId = PipelineType.Branch,
                    KilometerOfStart = kilometerOfStartAfterEditing,
                    KilometerOfEnd = editedKmOnEnd,
GasTransportSystemId = gastransport.Id
                });
			new SetSortOrderCommand(Context).Execute(new SetSortOrderParameterSet{Id=pipelineId,SortOrder = 10});
            var editedPipline =
                new GetPipelineByIdQuery(Context).Execute(pipelineId);

            Assert.AreEqual(bulshitName, editedPipline.Name);
            Assert.AreEqual(kilometerOfStartAfterEditing, editedPipline.KilometerOfStartPoint);
            Assert.AreEqual(editedKmOnEnd, editedPipline.KilometerOfEndPoint);
	        Assert.AreEqual(10, editedPipline.SortOrder);
            new DeletePipelineCommand(Context).Execute(new DeleteEntityParameterSet
                {
                    Id = pipelineId,
                    EntityType = EntityType.Pipeline
                });

            var delitedPipline =
                new GetPipelineListQuery(Context).Execute(null).FirstOrDefault(p => p.Id == pipelineId);

            Assert.IsNull(delitedPipline);
        }
    }
}