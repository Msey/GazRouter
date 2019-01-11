using GazRouter.Application;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Utils.Units;

namespace GazRouter.Flobus.Primitives
{
    public class SchemeColorData
    {
        public SchemeColorData()
        {
            switch (UserProfile.Current.UserSettings.SchemeColorMode)
            {
                case SchemaMode.Classic:
                    {
                        SchemeBackgroundColor = Color.FromArgb(0xFF, 0xF5, 0xF5, 0xF5);
                        TextColor = Colors.Black;
                        BorderColor = Colors.Black;
                        PipeLineFrameColor = Colors.Transparent;
                        break;
                    }
                case SchemaMode.Inverted:
                    {
                        SchemeBackgroundColor = Colors.Black;
                        TextColor = Colors.White;
                        BorderColor = Colors.White;
                        PipeLineFrameColor = Colors.White;
                        break;
                    }
                default:
                    {
                        SchemeBackgroundColor = Color.FromArgb(0xFF, 0xF5, 0xF5, 0xF5);
                        TextColor = Colors.Black;
                        BorderColor = Colors.Black;
                        PipeLineFrameColor = Colors.Transparent;
                        break;
                    }
            }
        }
        public Color SchemeBackgroundColor { get; set; }
        public Color TextColor { get; set; }
        public Color BorderColor { get; set; }
        public Color PipeLineFrameColor { get; set; }
    }
}
