using System.Windows.Media;

namespace GazRouter.Flobus.Interfaces
{
    public class PipelineMarkup
    {
        public double StartKm { get; set; }
        public double EndKm { get; set; }
        public object Data { get; set; }
        public Color Color { get; set; }
        public string Tooltip { get; set; }
    }
}