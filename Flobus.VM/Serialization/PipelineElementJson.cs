using System;
using System.Windows;

namespace GazRouter.Flobus.VM.Serialization
{
    public abstract class PipelineElementJson
    {
        public PipelineElementJson()
        { }

        protected PipelineElementJson(Guid id, int textAngle, Point container, bool hidden = false)
        {
            Id = id;
            TextAngle = textAngle;
            Hidden = hidden;
            ContainerPosition = container;
        }

        public Guid Id { get; set; }
        public int TextAngle { get; set; }
        public bool Hidden { get; set; }
        public Point ContainerPosition { get; set; }
    }
}