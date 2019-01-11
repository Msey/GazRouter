using System;
using System.Windows;
using GazRouter.Flobus.Interfaces;

namespace ClientTests.Flobus
{
    public class PipelineElementStub : IPipelineOmElement
    {
        public double Km { get; set; }
        public Guid Id { get; } = Guid.NewGuid();
        public Point ContainerPosition { get; set; }
    }
}