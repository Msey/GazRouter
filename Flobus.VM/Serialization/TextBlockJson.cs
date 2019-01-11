using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using GazRouter.Flobus.VM.Model;
using Newtonsoft.Json;

namespace GazRouter.Flobus.VM.Serialization
{
    public class TextBlockJson
    {
        public TextBlockJson()
        {
            FontSize = 12;
            TextColor = Colors.Black;
        }

        public TextBlockJson(TextBlock td) : this()
        {
            Position = td.Position;
            Text = td.Text;
            Height = td.Height;
            Width = td.Width;
            FontBold = td.FontBold;
            FontItalic = td.FontItalic;
            FontSize = td.FontSize;
            TextColor = td.TextColor;
            TextAngle = td.TextAngle;
        }

        public int TextAngle { get; set; }

        public Point Position { get; set; }
        public string Text { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        
        public Color TextColor { get; set; }

        public bool FontBold { get; set; } 

        public bool FontItalic { get; set; }

         [DefaultValue(12)]
         [JsonProperty(DefaultValueHandling =  DefaultValueHandling.IgnoreAndPopulate)]
        public double FontSize { get; set; }

    }
}