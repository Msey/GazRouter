using System.Windows;
using System.Windows.Media;
using GazRouter.Common.ViewModel;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.VM.Serialization;

namespace GazRouter.Flobus.VM.Model
{
    public class TextBlock : PropertyChangedBase, ITextBlock
    {
        private Color _textColor = Colors.Black;
        private double _fontSize = 12;
        private bool _fontItalic;
        private bool _fontBold;
        private Point _position;
        private string _text;
        private double _height = 30;
        private double _width = 50;
        private int _textAngle;

        public TextBlock()
        {
        }

        public TextBlock(TextBlockJson tb)
        {
            Position = tb.Position;
            Text = tb.Text;
            Width = tb.Width;
            Height = tb.Height;
            FontBold = tb.FontBold;
            FontItalic=tb.FontItalic;
            FontSize = tb.FontSize;
            TextColor= tb.TextColor;
            TextAngle = tb.TextAngle;
        }

        public Color TextColor
        {
            get { return _textColor; }
            set { SetProperty(ref _textColor, value); }
        }

        public double FontSize
        {
            get { return _fontSize; }
            set { SetProperty(ref _fontSize, value); }
        }

        public bool FontItalic
        {
            get { return _fontItalic; }
            set { SetProperty(ref _fontItalic, value); }
        }

        public bool FontBold
        {
            get { return _fontBold; }
            set { SetProperty(ref _fontBold, value); }
        }

        public Point Position
        {
            get { return _position; }
            set { SetProperty(ref _position, value); }
        }

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        public int TextAngle
        {
            get { return _textAngle; }
            set { SetProperty(ref _textAngle, value); }
        }

        public double Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }

        public double Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }
        }
    }
}