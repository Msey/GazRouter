using System.Linq;
using System.Windows;
using GazRouter.Flobus;
using GazRouter.Flobus.Model;
using GazRouter.Flobus.Services;
using GazRouter.Flobus.Visuals;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

// ReSharper disable UnusedMember.Global

namespace ClientTests.Flobus.Services
{
    [TestClass]
    public class DraggingServiceTests
    {
        [TestMethod]
        public void DragPipelineTest()

        {
            var s = new Schema();
            var service = new DraggingService(s);

            var pipeline = new PipelineWidget(new PipelineStub
            {
                StartPoint = new Point(100, 200),
                EndPoint = new Point(700, 400),
                KmBegining = 0,
                KmEnd = 300
            }, s);
            var intermediatePointMock1 = new Mock<IPipelinePoint>();
            intermediatePointMock1.SetupGet(point => point.Km).Returns(100);
            intermediatePointMock1.SetupGet(point => point.Type).Returns(PointType.Intermediate);
            intermediatePointMock1.SetupProperty(p => p.Position, new Point(300, 200));
            pipeline.AddPipelinePoint(intermediatePointMock1.Object);
                //IntermediatePipelinePoint.CreateIntermediate( 100, new Point(300, 200)));
            var intermediatePointMock2 = new Mock<IPipelinePoint>();
            intermediatePointMock2.SetupGet(point => point.Km).Returns(200);
            intermediatePointMock2.SetupGet(point => point.Type).Returns(PointType.Intermediate);
            intermediatePointMock2.SetupProperty(point => point.Position, new Point(500, 400));
            pipeline.AddPipelinePoint(intermediatePointMock2.Object);
                // IntermediatePipelinePoint.CreateIntermediate( 200, new Point(500, 400)));

            service.InitializeDrag(new Point(200, 200));
            service.StartDrag(new[] {pipeline}, new Point(210, 200));
            service.Drag(new Point(210, 200));

            Assert.AreEqual(new Point(110, 200), pipeline.StartPoint);
            Assert.AreEqual(new Point(710, 400), pipeline.EndPoint);
            Assert.AreEqual(new Point(310, 200), pipeline.Points.Single(c => c.Km == 100).Position);
            Assert.AreEqual(new Point(510, 400), pipeline.Points.Single(c => c.Km == 200).Position);
        }

        [TestMethod]
        public void DragValveTest()
        {
            var s = new Schema();
            var service = new DraggingService(s);

            var pipeline = new PipelineWidget(new PipelineStub
            {
                StartPoint = new Point(200, 200),
                EndPoint = new Point(400, 200),
                KmBegining = 100,
                KmEnd = 200
            }, new Schema());

            var valve = new ValveWidget(pipeline, new PipelineTests.ValveStub {Km = 150});
            //  pipeline.AddPipelinePoint(valve);

            service.InitializeDrag(new Point(300, 200));
            var endDragPoint = new Point(350, 200);
            service.StartDrag(new[] {valve}, endDragPoint);
            service.Drag(endDragPoint);

            Assert.AreEqual(endDragPoint, valve.Position);
        }

        [TestMethod, Tag("C")]
        public void DragMeasuringLineTest()
        {
            var s = new Schema();
            var service = new DraggingService(s);

            var pipeline =
                new PipelineWidget(
                    new PipelineStub
                    {
                        StartPoint = new Point(200, 200),
                        EndPoint = new Point(400, 200),
                        KmBegining = 100,
                        KmEnd = 200
                    }, new Schema());

            var widget = new MeasuringLineWidget(pipeline, new PipelineElementStub() {Km = 150 });
            //  pipeline.AddPipelinePoint(valve);

            service.InitializeDrag(new Point(300, 200));
            var endDragPoint = new Point(350, 200);
            service.StartDrag(new[] {widget}, endDragPoint);
            service.Drag(endDragPoint);

            Assert.AreEqual(endDragPoint, widget.Position);
        }
    }
}