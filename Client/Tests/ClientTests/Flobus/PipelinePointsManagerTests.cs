using System.Windows;
using GazRouter.Flobus.Model;
using GazRouter.Flobus.Visuals;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable UnusedMember.Global

namespace ClientTests.Flobus
{
    [TestClass]
    public class PipelinePointsManagerTests
    {
        [TestMethod, Tag("C")]
        public void ПриДобавленииПромежуточнойТочкиЛежащейНаЛинииГазопроводаДолженВычислитьсяКм()
        {
            var startPoint = new Point(100, 100);
            var endPoint = new Point(200, 100);

            var ppm = new PipelinePointsManager(startPoint, endPoint, 100, 300);
            var point = ppm.AddIntermediatePoint(new Point(150, 100));
            Assert.AreEqual(3, ppm.Count);
            Assert.AreEqual(200, point.Km);
            Assert.AreEqual(PointType.Intermediate, point.Type);
        }

        [TestMethod]
        public void ТочкиНаГоризонтальнойЛинии()
        {
            var startPoint = new Point(100, 100);
            var endPoint = new Point(200, 100);

            var ppm = new PipelinePointsManager(startPoint, endPoint, 100, 200);

            Assert.AreEqual(2, ppm.Count);
            Assert.AreEqual(startPoint, ppm.Start.Position);
            Assert.AreEqual(endPoint, ppm.End.Position);
        }

        [TestMethod]
        public void ЕслиТочкиНаДиагоналиДолжнаДобавитьсяТочкаПоворота()
        {
            var ppm = new PipelinePointsManager(new Point(100, 100), new Point(200, 200), 100, 200);

            Assert.AreEqual(3, ppm.Count);
            Assert.AreEqual(new Point(200, 100), ppm.Points[1].Position);
            Assert.AreEqual(PointType.Turn, ppm.Points[1].Type);
        }

        [TestMethod]
        public void ДобавляемПромежуточнуюТочкуВМестоПоворота()
        {
            var ppm = new PipelinePointsManager(new Point(200, 200), new Point(-200, -200), 100, 300);

            var point = ppm.AddIntermediatePoint(new Point(-200, 200));
            Assert.AreEqual(3, ppm.Count);
            Assert.AreEqual(200, point.Km);
            Assert.AreEqual(new Point(-200, 200), ppm.Points[1].Position);
            Assert.AreEqual(PointType.Intermediate, ppm.Points[1].Type);
        }

        [TestMethod]
        public void ДобавляемИнфраструктурнуюТочкуНаКм()
        {
            var ppm = new PipelinePointsManager(new Point(400, 200), new Point(-200, -200), 100, 300);

            var point = ppm.FindOrCreateInfraPoint(200, true);
            Assert.AreEqual(4, ppm.Count);
            Assert.AreEqual(200, point.Km);
            Assert.AreEqual(new Point(-100, 200), ppm.Points[1].Position);
            Assert.AreEqual(PointType.Infra, ppm.Points[1].Type);
        }

        [TestMethod]
        public void ПослеВторогСдвигаНАчальнойТочкиГазопродовДолженОставатьсяОртогональным()
        {
            var ppm = new PipelinePointsManager(new Point(23781, 2032), new Point(23781, 1951), 0, 038);
            ppm.FindOrCreateInfraPoint(0.019, true);

            ppm.Start.Position = new Point(23741, 2032);
            Assert.AreEqual(true, ppm.IsOrthogonal);
            ppm.Start.Position = new Point(23731, 2010);
            Assert.AreEqual(true, ppm.IsOrthogonal);
        }

        [TestMethod, Tag("PPM")]
        public void MoveStartPointTest()
        {
            var ppm = new PipelinePointsManager(new Point(200, 200), new Point(-200, -200), 100, 500);
            ppm.AddIntermediatePoint(new Point(-200, 200));
            var valveBeforIntermediate = ppm.FindOrCreateInfraPoint(200, true);
            var valveAfterIntermediate = ppm.FindOrCreateInfraPoint(400, true);
            var v3 = ppm.FindOrCreateInfraPoint(275, true);

            ppm.Start.Position = new Point(400, 400);

            Assert.AreEqual(7, ppm.Count);
            Assert.AreEqual(new Point(0, 400), valveBeforIntermediate.Position, "Эта точка должна перемеcтиться");
            Assert.AreEqual(new Point(-200, 0), valveAfterIntermediate.Position, "Эта точка не должна перемещаться");
            Assert.AreEqual(new Point(-200, 300), v3.Position, "Эта точка должна перемеcтиться");
        }

        [TestMethod]
        public void MoveEndPointTest()
        {
            var ppm = new PipelinePointsManager(new Point(200, 200), new Point(-200, -200), 100, 500);
            ppm.AddIntermediatePoint(new Point(-200, 200));
            var valveBeforIntermediate = ppm.FindOrCreateInfraPoint(200, true);
            var valveAfterIntermediate = ppm.FindOrCreateInfraPoint(400, true);
            var v3 = ppm.FindOrCreateInfraPoint(325, true);

            ppm.End.Position = new Point(-400, -400);

            Assert.AreEqual(7, ppm.Count);
            Assert.AreEqual(new Point(0, 200), valveBeforIntermediate.Position, "Эта точка не должна перемещаться");
            Assert.AreEqual(new Point(-400, 0), valveAfterIntermediate.Position, "Эта точка должна перемеcтиться");
            Assert.AreEqual(new Point(-300, 200), v3.Position, "Эта точка должна перемеcтиться");
        }

        [TestMethod]
        public void ЕслиСделатьГазопроводПрямымТочкаПоворотаДолжнаУдалиться()
        {
            var ppm = new PipelinePointsManager(new Point(100, 100), new Point(300, 100), 100, 300);
            var point = ppm.AddIntermediatePoint(new Point(200, 200), 200);
            Assert.AreEqual(5, ppm.Count);
            point.Position = new Point(200, 100);

            Assert.AreEqual(3, ppm.Count);
        }

        [TestMethod, Tag("C")]
        public void MoveIntermediatePointTest()
        {
            var ppm = new PipelinePointsManager(new Point(200, 200), new Point(-200, -200), 100, 500);

            var valveBeforIntermediate = ppm.FindOrCreateInfraPoint(200, true);
            var valveAfterIntermediate = ppm.FindOrCreateInfraPoint(400, true);

            var point = ppm.AddIntermediatePoint(new Point(200, -200), 300);
            Assert.AreEqual(new Point(200, 0), valveBeforIntermediate.Position, "Эта точка должна перемеcтиться");
            Assert.AreEqual(new Point(0, -200), valveAfterIntermediate.Position, "Эта точка должна перемеcтиться");
            point.Position = new Point(-200, 200);
            Assert.AreEqual(new Point(0, 200), valveBeforIntermediate.Position, "Эта точка должна перемеcтиться");
            Assert.AreEqual(new Point(-200, 0), valveAfterIntermediate.Position, "Эта точка должна перемеcтиться");
        }

        [TestMethod]
        public void MinLenghtTest()
        {
            var ppm = new PipelinePointsManager(new Point(200, 200), new Point(201, 201), 100, 500);

            Assert.AreEqual(new Point(201, 200 + ppm.MinLength - 1), ppm.End.Position);
        }

        [TestMethod, Tag("C")]
        public void ArrangeTest()
        {
            var ppm = new PipelinePointsManager(new Point(100, 100), new Point(300, 100), 100, 500);
            var point = ppm.AddIntermediatePoint(new Point(200, 105), 300);
            Assert.AreEqual(5, ppm.Count);
            ppm.Arrange();
            Assert.AreEqual(3, ppm.Count);
        }
    }
}