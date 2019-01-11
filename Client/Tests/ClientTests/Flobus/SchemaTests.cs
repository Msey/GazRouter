using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows;
using GazRouter.Flobus;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.Model;
using GazRouter.Flobus.Visuals;
using GazRouter.Flobus.VM.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Telerik.Windows.Diagrams.Core;

namespace ClientTests.Flobus
{
    [TestClass]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class SchemaTests
    {
        private Schema _schema;
        private Mock<ISchemaSource> _mockSchemaSource;
        private List<IPipeline> _pipelines;
        private List<ICompressorShop> _compressorShops;

        [TestInitialize]
        public void TestInitialize()
        {
            _compressorShops = new List<ICompressorShop>();
            _schema = new Schema();
            _mockSchemaSource = new Mock<ISchemaSource>();
            _pipelines = new List<IPipeline>();
            _mockSchemaSource.SetupGet(source => source.Pipelines).Returns(_pipelines);
            _mockSchemaSource.SetupGet(source => source.CompressorShops).Returns(_compressorShops);
            _mockSchemaSource.SetupGet(source => source.SchemeInfo).Returns(new SchemeVersion());
        }

        [TestMethod]
        public void BindingPipelineTest()
        {
            var mockPipeline = CreateMockPipeline();
            var guid = Guid.NewGuid();
            mockPipeline.SetupGet(c => c.Id).Returns(guid);
            var pipelineViewModel = mockPipeline.Object;
            _pipelines.Add(pipelineViewModel);

            _schema.SchemaSource = _mockSchemaSource.Object;

            Assert.AreEqual(1, _schema.Items.OfType<PipelineWidget>().Count(), "Количество газопроводов");

            var pipelineWidget = _schema.Items.OfType<PipelineWidget>().First();
            Assert.AreEqual(new Point(100, 100), pipelineWidget.StartPoint);
            Assert.AreEqual(new Point(500, 400), pipelineWidget.EndPoint);
            Assert.AreEqual(guid, pipelineWidget.Id);
        }

        [TestMethod]
        public void BindingIntermediatePointsTest()
        {
            var mockPipeline = CreateMockPipeline();
            var pipelineViewModel = mockPipeline.Object;
            _pipelines.Add(pipelineViewModel);
            var intermediatePointPos = new Point(400, 100);
            var point = new GeometryPoint(400, intermediatePointPos);
            mockPipeline.SetupGet(p => p.IntermediatePoints)
                .Returns(new List<IGeometryPoint> {point});

            _schema.SchemaSource = _mockSchemaSource.Object;
            var pipelineWidget = _schema.Items.OfType<PipelineWidget>().First();
            pipelineWidget.Move(new Vector(10, 10));

            mockPipeline.VerifySet(c => c.StartPoint = new Point(110, 110));
            Assert.AreEqual(new Point(510, 410), pipelineViewModel.EndPoint);
            Assert.AreEqual(new Point(410, 110), point.Position);
        }

        [TestMethod]
        public void BindingPipelineWithMeasuringLineTest()
        {
            var mockPipeline = CreateMockPipeline();
            var mockMeasuringLine = new Mock<IPipelineOmElement>();
            mockMeasuringLine.SetupGet(p => p.Km).Returns(100);
            mockPipeline.SetupGet(pipeline => pipeline.MeasuringLines)
                .Returns(new List<IPipelineOmElement> {mockMeasuringLine.Object});
            _pipelines.Add(mockPipeline.Object);

            _schema.SchemaSource = _mockSchemaSource.Object;

            var pipelineWidget = _schema.Items.OfType<PipelineWidget>().First();
            Assert.AreEqual(1, pipelineWidget.Items.OfType<MeasuringLineWidget>().Count());
        }

        [TestMethod]
        public void BindingPipelineWithValveTest()
        {
            var mockPipeline = CreateMockPipeline();

            var mockValve = new Mock<IValve>();
            mockValve.SetupGet(p => p.Km).Returns(400);
            var valve = mockValve.Object;
            mockPipeline.SetupGet(pipeline => pipeline.Valves).Returns(new List<IValve> {valve});
            var pipelineViewModel = mockPipeline.Object;
            _pipelines.Add(pipelineViewModel);

            _schema.SchemaSource = _mockSchemaSource.Object;

            var pipelineWidget = _schema.Items.OfType<PipelineWidget>().First();
            Assert.AreEqual(1, pipelineWidget.Items.OfType<ValveWidget>().Count());
            var valveWidget = pipelineWidget.Items.OfType<ValveWidget>().First();
            Assert.AreEqual(new Point(500, 225), valveWidget.Position);
        }

/*        [TestMethod]
        public void BindingCompressorShopTest()
        {
            var mockCompShop = new Mock<ICompressorShop>();
            var point = new Point(200, 200);
            mockCompShop.SetupProperty(cs => cs.Position, point);
            var compressorShop = mockCompShop.Object;
            _compressorShops.Add(compressorShop);
            _schema.SchemaSource = _mockSchemaSource.Object;

            Assert.AreEqual(1, _schema.Items.OfType<CompressorShopWidget>().Count());

            var compShopWisget = _schema.Items.OfType<CompressorShopWidget>().First();
            Assert.AreEqual(point, compShopWisget.Position);

            point = new Point(300, 300);
            compShopWisget.Position = point;

            Assert.AreEqual(point, compressorShop.Position);
        }*/

        private static Mock<IPipeline> CreateMockPipeline()
        {
            var mockPipeline = new Mock<IPipeline>();
            mockPipeline.SetupProperty(p => p.StartPoint, new Point(100, 100));
            mockPipeline.SetupProperty(p => p.EndPoint, new Point(500, 400));
            mockPipeline.SetupGet(pipeline => pipeline.KmBegining).Returns(100);
            mockPipeline.SetupGet(pipeline => pipeline.KmEnd).Returns(500);
            mockPipeline.Setup(p => p.AddPoint(It.IsAny<double>(), It.IsAny<Point>()))
                .Returns((double km, Point p) => new GeometryPoint(km, p));

            return mockPipeline;
        }
    }
}