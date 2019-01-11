using System;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Flobus;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.Visuals;
using GazRouter.Flobus.VM.Model;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace ClientTests.Flobus
{
    [TestClass]
    public class PipelineTests
    {
        [TestMethod, Tag("C")]
        public void ValveTest()
        {
            var mockPipeline = new Mock<IPipeline>();
            mockPipeline.SetupProperty(p => p.StartPoint, new Point(100, 100));
            mockPipeline.SetupProperty(p => p.EndPoint, new Point(200, 200));
            mockPipeline.SetupGet(p => p.KmBegining).Returns(100);
            mockPipeline.SetupGet(p => p.KmEnd).Returns(500);
            mockPipeline.Setup(p => p.AddPoint(It.IsAny<double>(), It.IsAny<Point>()))
                .Returns((double a, Point b) => new GeometryPoint(a, b));

            var pipeline = new PipelineWidget(
                mockPipeline.Object,
                new Schema());

            var valve = new ValveWidget(pipeline, new ValveStub {Km = 200});
            var valve2 = new ValveWidget(pipeline, new ValveStub {Km = 400});

            Assert.AreEqual(new Point(150, 100), valve.Position);
            mockPipeline.Verify(p => p.AddPoint(200, new Point(150, 100)));
            Assert.AreEqual(Orientation.Horizontal, valve.Orientation);
            Assert.AreEqual(new Point(200, 150), valve2.Position);
            Assert.AreEqual(Orientation.Vertical, valve2.Orientation);
            Assert.AreEqual(5, pipeline.PointsCount);
//            Assert.AreEqual(2, pipelineStub.IntermediatePoints.Count());
        }

        public class ValveStub : IValve
        {
            public double Km { get; set; }
            public Guid Id { get; }
            public int TextAngle { get; set; }
            public string Tooltip { get; } = string.Empty;
            public Point ContainerPosition { get; set; }
        }
    }
}