using System.Linq;
using GazRouter.DAL.Dictionaries.ValveTypes;
using GazRouter.DAL.ObjectModel.Segment.Diameter;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.Segment;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class PipeDiameterSegmentTest : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void FullTestPipeSegment()
        {
            var pipelineId = CreatePipeline(PipelineType.Branch);
            const double kilometerOfStartAfterCreation = 23;
            const double kilometerOfEndAfterCreation = 24;
			var diameter = new GetValveTypesListQuery(Context).Execute().FirstOrDefault();
			Assert.IsNotNull(diameter);
            var pipeSegmentId =
				new AddDiameterSegmentCommand(Context).Execute(new AddDiameterSegmentParameterSet
                    {
                        PipelineId = pipelineId,
                        KilometerOfStartPoint = kilometerOfStartAfterCreation,
                        KilometerOfEndPoint = kilometerOfEndAfterCreation,
						DiameterId = diameter.Id
                    });

            var pipeSegmentList =
				new GetDiameterSegmentListQuery(Context).Execute(pipelineId).First(p => p.Id == pipeSegmentId);
            Assert.IsNotNull(pipeSegmentList);
            Assert.AreNotEqual(default(int), pipeSegmentId);

            const double kilometerOfStartAfterEditing = 24;
            const double kilometerOfEndAfterEditing = 25;
			new EditDiameterSegmentCommand(Context).Execute(new EditDiameterSegmentParameterSet
                {
                    SegmentId = pipeSegmentId,
                    PipelineId = pipelineId,
                    KilometerOfStartPoint = kilometerOfStartAfterEditing,
                    KilometerOfEndPoint = kilometerOfEndAfterEditing,
					DiameterId = diameter.Id
                });
            Assert.AreNotEqual(default(int), pipeSegmentId);

			new DeleteDiameterSegmentCommand(Context).Execute(pipeSegmentId);
        }
    }
}
