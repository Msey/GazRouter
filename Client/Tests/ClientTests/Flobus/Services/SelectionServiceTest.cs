using System.Windows;
using GazRouter.Flobus;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Services;
using GazRouter.Flobus.Visuals;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
// ReSharper disable UnusedMember.Global

namespace ClientTests.Flobus.Services
{
    [TestClass]
    public class SelectionServiceTest
    {
        [TestMethod]
        public void SelectBasicTast()
        {
      //      var mock = new Mock<ISchemaItem>();
      
            ISchemaItem schemaItem = new PipelineWidget(new PipelineStub {StartPoint= new Point(100, 100), EndPoint=new Point(200, 100), KmBegining=100, KmEnd=200 },  new Schema());
            Assert.AreEqual(false, schemaItem.IsSelected);
            new  SelectionService().SelectItem(schemaItem);
            Assert.AreEqual(true, schemaItem.IsSelected);

        }
    }
}