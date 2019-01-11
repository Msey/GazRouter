using System.Linq;
using GazRouter.DAL.Dictionaries.Diameters;
using GazRouter.DAL.Dictionaries.PipelineGroups;
using GazRouter.DAL.ObjectModel.Segment.Diameter;
using GazRouter.DAL.ObjectModel.Segment.PipelineGroup;
using GazRouter.DAL.ObjectModel.Segment.Pressure;
using GazRouter.DAL.ObjectModel.Segment.Site;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.Segment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace DALTest.ObjectModel
{
    [TestClass]
    public class SegmentTest : DalTestBase
    {
        [TestMethod, TestCategory(Stable)]
        public void SiteSegmentTest()
        {
            var siteId = CreateSite();
            var pipelineId = CreatePipeline(PipelineType.Branch);

            var segmentId =
                new AddSiteSegmentCommand(Context).Execute(new AddSiteSegmentParameterSet
                {
                    SiteId = siteId,
                    PipelineId = pipelineId,
                    KilometerOfStartPoint = 23.0,
                    KilometerOfEndPoint = 24.0
                });

            var segment =
                new GetSiteSegmentListQuery(Context).Execute(pipelineId).SingleOrDefault(p => p.Id == segmentId);
            Assert.IsNotNull(segment);
            Assert.AreEqual(segment.KilometerOfStartPoint, 23.0);
            Assert.AreEqual(segment.KilometerOfEndPoint, 24.0);

            
            new EditSiteSegmentCommand(Context).Execute(new EditSiteSegmentParameterSet
            {
                SegmentId = segmentId,
                SiteId = siteId,
                PipelineId = pipelineId,
                KilometerOfStartPoint = 66.0,
                KilometerOfEndPoint = 77.0
            });

            segment =
                new GetSiteSegmentListQuery(Context).Execute(pipelineId).SingleOrDefault(p => p.Id == segmentId);
            Assert.IsNotNull(segment);
            Assert.AreEqual(segment.KilometerOfStartPoint, 66.0);
            Assert.AreEqual(segment.KilometerOfEndPoint, 77.0);

            new DeleteSiteSegmentCommand(Context).Execute(segmentId);
            segment =
                new GetSiteSegmentListQuery(Context).Execute(pipelineId).SingleOrDefault(p => p.Id == segmentId);
            Assert.IsNull(segment);


        }

        [TestMethod, TestCategory(Stable)]
        public void GroupSegmentTest()
        {
            var pipelineId = CreatePipeline(PipelineType.Branch);

            var pipelineGroupId = new GetPipelineGroupListQuery(Context).Execute().First().Id;

            var segmentId = new AddGroupSegmentCommand(Context).Execute(new AddGroupSegmentParameterSet
                                                            {
                                                                PipelineGroupId = pipelineGroupId,
                                                                PipelineId = pipelineId,
                                                                KilometerOfStartPoint = 23.0,
                                                                KilometerOfEndPoint = 24.0
                                                            });
            var segment =
                new GetGroupSegmentListQuery(Context).Execute(pipelineId).SingleOrDefault(p => p.Id == segmentId);
            Assert.IsNotNull(segment);
            Assert.AreEqual(segment.PipelineGroupId, pipelineGroupId);
            Assert.AreEqual(segment.KilometerOfStartPoint, 23.0);
            Assert.AreEqual(segment.KilometerOfEndPoint, 24.0);

            new EditGroupSegmentCommand(Context).Execute(new EditGroupSegmentParameterSet
            {
                SegmentId = segmentId,
                PipelineGroupId = pipelineGroupId,
                PipelineId = pipelineId,
                KilometerOfStartPoint = 66.0,
                KilometerOfEndPoint = 77.0
            });
            segment =
                new GetGroupSegmentListQuery(Context).Execute(pipelineId).SingleOrDefault(p => p.Id == segmentId);
            Assert.IsNotNull(segment);
            Assert.AreEqual(segment.PipelineGroupId, pipelineGroupId);
            Assert.AreEqual(segment.KilometerOfStartPoint, 66.0);
            Assert.AreEqual(segment.KilometerOfEndPoint, 77.0);

            
            new DeleteGroupSegmentCommand(Context).Execute(segmentId);
            segment =
                new GetGroupSegmentListQuery(Context).Execute(pipelineId).SingleOrDefault(p => p.Id == segmentId);
            Assert.IsNull(segment);
        }


        [TestMethod, TestCategory(Stable)]
        public void PressureSegmentTest()
        {
            var pipelineId = CreatePipeline(PipelineType.Branch);
            
            var segmentId = new AddPressureSegmentCommand(Context).Execute(new AddPressureSegmentParameterSet
            {
                Pressure = 10.0,
                PipelineId = pipelineId,
                KilometerOfStartPoint = 23.0,
                KilometerOfEndPoint = 24.0
            });
            var segment =
                new GetPressureSegmentListQuery(Context).Execute(pipelineId).SingleOrDefault(p => p.Id == segmentId);
            Assert.IsNotNull(segment);
            Assert.AreEqual(segment.Pressure, 10.0);
            Assert.AreEqual(segment.KilometerOfStartPoint, 23.0);
            Assert.AreEqual(segment.KilometerOfEndPoint, 24.0);

            new EditPressureSegmentCommand(Context).Execute(new EditPressureSegmentParameterSet
            {
                SegmentId = segmentId,
                Pressure = 20.0,
                PipelineId = pipelineId,
                KilometerOfStartPoint = 66.0,
                KilometerOfEndPoint = 77.0
            });
            segment =
                new GetPressureSegmentListQuery(Context).Execute(pipelineId).SingleOrDefault(p => p.Id == segmentId);
            Assert.IsNotNull(segment);
            Assert.AreEqual(segment.Pressure, 20.0);
            Assert.AreEqual(segment.KilometerOfStartPoint, 66.0);
            Assert.AreEqual(segment.KilometerOfEndPoint, 77.0);


            new DeletePressureSegmentCommand(Context).Execute(segmentId);
            segment =
                new GetPressureSegmentListQuery(Context).Execute(pipelineId).SingleOrDefault(p => p.Id == segmentId);
            Assert.IsNull(segment);
        }


        [TestMethod, TestCategory(Stable)]
        public void DiameterSegmentTest()
        {
            var pipelineId = CreatePipeline(PipelineType.Branch);

            var diameterId = new GetDiameterListQuery(Context).Execute().First().Id;
            var extDiameterId = new GetExternalDiameterListQuery(Context).Execute().First(ed=>ed.InternalDiameterId == diameterId).Id;

            var segmentId = new AddDiameterSegmentCommand(Context).Execute(new AddDiameterSegmentParameterSet
            {
                DiameterId = diameterId,
                ExternalDiameterId = extDiameterId,
                PipelineId = pipelineId,
                KilometerOfStartPoint = 23.0,
                KilometerOfEndPoint = 24.0
            });
            var segment =
                new GetDiameterSegmentListQuery(Context).Execute(pipelineId).SingleOrDefault(p => p.Id == segmentId);
            Assert.IsNotNull(segment);
            Assert.AreEqual(segment.DiameterId, diameterId);
            Assert.AreEqual(segment.ExternalDiameterId, extDiameterId);
            Assert.AreEqual(segment.KilometerOfStartPoint, 23.0);
            Assert.AreEqual(segment.KilometerOfEndPoint, 24.0);

            new EditDiameterSegmentCommand(Context).Execute(new EditDiameterSegmentParameterSet
            {
                SegmentId = segmentId,
                DiameterId = diameterId,
                ExternalDiameterId = extDiameterId,
                PipelineId = pipelineId,
                KilometerOfStartPoint = 66.0,
                KilometerOfEndPoint = 77.0
            });
            segment =
                new GetDiameterSegmentListQuery(Context).Execute(pipelineId).SingleOrDefault(p => p.Id == segmentId);
            Assert.IsNotNull(segment);
            Assert.AreEqual(segment.KilometerOfStartPoint, 66.0);
            Assert.AreEqual(segment.KilometerOfEndPoint, 77.0);


            new DeleteDiameterSegmentCommand(Context).Execute(segmentId);
            segment =
                new GetDiameterSegmentListQuery(Context).Execute(pipelineId).SingleOrDefault(p => p.Id == segmentId);
            Assert.IsNull(segment);
        }
    }
}
