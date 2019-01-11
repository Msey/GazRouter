using System.Linq;
using GazRouter.DAL.ObjectModel.Segment.Pressure;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.Segment;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class PipeSegmentTest : DalTestBase
    {
        [TestMethod ,TestCategory(Stable)]
        public void FullTestPipeSegment()
        {
            var pipelineId = CreatePipeline(PipelineType.Branch);
            const double kilometerOfStartAfterCreation = 23;
            const double kilometerOfEndAfterCreation = 24;
            const double pressure = 10;
            var pipeSegmentId =
                new AddPressureSegmentCommand(Context).Execute(new AddPressureSegmentParameterSet
                    {
                        PipelineId = pipelineId,
                        KilometerOfStartPoint = kilometerOfStartAfterCreation,
                        KilometerOfEndPoint = kilometerOfEndAfterCreation,
                        Pressure = pressure
                    });

            var pipeSegmentList =
                new GetPressureSegmentListQuery(Context).Execute(pipelineId).First(p => p.Id == pipeSegmentId);
            Assert.IsNotNull(pipeSegmentList);
            Assert.AreNotEqual(default(int), pipeSegmentId);

            const double kilometerOfStartAfterEditing = 24;
            const double kilometerOfEndAfterEditing = 25;
            new EditPressureSegmentCommand(Context).Execute(new EditPressureSegmentParameterSet
                {
                    SegmentId = pipeSegmentId,
                    PipelineId = pipelineId,
                    KilometerOfStartPoint = kilometerOfStartAfterEditing,
                    KilometerOfEndPoint = kilometerOfEndAfterEditing,
                    Pressure = pressure
                });
            Assert.AreNotEqual(default(int), pipeSegmentId);

            new DeletePressureSegmentCommand(Context).Execute(pipeSegmentId);
        }
    }
}
