using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Repair.PrintForms
{
    public class BitmapUtils
    {
        public static WriteableBitmap CreateWriteableBitmap(UIElement element, Rect enclosingBounds,
            Size returnImageSize, Brush backgroundBrush, Thickness margin)
        {
            var rectWithMargin = enclosingBounds.InflateRect(margin.Left, margin.Top, margin.Right, margin.Bottom);

            if (
                !(element != null && IsSizeValid(returnImageSize) && enclosingBounds.IsValidBounds() &&
                  rectWithMargin.IsValidBounds()))
            {
                return null;
            }

            if (returnImageSize.Width <= 0 || returnImageSize.Height <= 0)
            {
                returnImageSize = rectWithMargin.ToSize();
            }

            var scale = new ScaleTransform();
            if (rectWithMargin.Width > 0 || rectWithMargin.Height > 0)
            {
                scale.ScaleX = returnImageSize.Width / rectWithMargin.Width;
                scale.ScaleY = returnImageSize.Height / rectWithMargin.Height;
            }

            var scaleWidth = ((int)returnImageSize.Width);
            var scaleHeight = ((int)returnImageSize.Height);


            var transformation = new TransformGroup();
            var translate = new TranslateTransform { X = -rectWithMargin.X, Y = -rectWithMargin.Y };
            transformation.Children.Add(translate);
            transformation.Children.Add(scale);

            WriteableBitmap bitmap = new WriteableBitmap(scaleWidth, scaleHeight);
            if (backgroundBrush != null)
                bitmap.Render(
                    new Rectangle { Fill = backgroundBrush, Width = rectWithMargin.Width, Height = rectWithMargin.Height },
                    null);

            bitmap.Render(element, transformation);
            bitmap.Invalidate();
            return bitmap;

        }

        private static bool IsSizeValid(Size size)
        {
            return !size.Width.IsNanOrInfinity() && !size.Height.IsNanOrInfinity();
        }
    }
}
