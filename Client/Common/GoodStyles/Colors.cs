
using System.Windows.Media;


namespace GazRouter.Common.GoodStyles
{
    public static class Colors
    {
        public static Color Transparent { get; } = Color.FromArgb(0, 255, 255, 255);
        public static Color Black { get; } = Color.FromArgb(255, 0, 0, 0);
        public static Color Gray { get; } = Color.FromArgb(255, 128, 128, 128);
        public static Color Red { get; } = Color.FromArgb(0xff, 0xdc, 0x14, 0x3c);
        public static Color Green { get; } = Color.FromArgb(0xff, 0x2e, 0x8b, 0x57);
        public static Color Orange { get; } = Color.FromArgb(0xff, 255, 165, 0);
        public static Color SoftOrange { get; } = Color.FromArgb(0xff, 0xff, 0x57, 0x22);
        public static Color Dark { get; } = Color.FromArgb(0xff, 0x31, 0x31, 0x31);
        public static Color Cyan { get; } = Color.FromArgb(0xff, 0x00, 0xbc, 0xd4);
        public static Color NiceGreen { get; } = Color.FromArgb(0xff, 0x3c, 0x94, 0x8b);
        public static Color Purple { get; } = Color.FromArgb(0xff, 0xe0, 0x40, 0xfb);
        public static Color LightRed { get; } = Color.FromArgb(0xff, 0xf0, 0x80, 0x80);
        public static Color White { get; } = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
        public static Color Lime { get; } = Color.FromArgb(255, 0, 255, 0);

    }


    public static class Brushes
    {
        public static Brush Transparent { get; } = new SolidColorBrush(Colors.Transparent);
        public static Brush Black { get; } = new SolidColorBrush(Colors.Black);
        public static Brush Gray { get; } = new SolidColorBrush(Colors.Gray);
        public static Brush Red { get; } = new SolidColorBrush(Colors.Red);
        public static Brush Green { get; } = new SolidColorBrush(Colors.Green);
        public static Brush Orange { get; } = new SolidColorBrush(Colors.Orange);
        public static Brush Dark { get; } = new SolidColorBrush(Colors.Dark);
        public static Brush Cyan { get; } = new SolidColorBrush(Colors.Cyan);
        public static Brush NiceGreen { get; } = new SolidColorBrush(Colors.NiceGreen);
        public static Brush Lime { get; } = new SolidColorBrush(Colors.Lime);
        public static Brush Purple { get; } = new SolidColorBrush(Colors.Purple);
        public static Brush LightRed { get; } = new SolidColorBrush(Colors.LightRed);
        public static Brush White { get; } = new SolidColorBrush(Colors.White);
        public static Brush SoftOrange { get; } = new SolidColorBrush(Colors.SoftOrange);

    }



}
