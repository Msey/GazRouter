using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GazRouter.Flobus.Extensions;

namespace GazRouter.Flobus.Primitives
{
    public class AdornerBase : Control
    {
        private Point _position;
        public AdornerBase()
        {
            DefaultStyleKey = GetType();

            RenderTransformOrigin=new Point(0.5, 0.5);

            var scaleTranform = new TransformGroup();
            scaleTranform.Children.Add(new ScaleTransform());
            scaleTranform.Children.Add(new SkewTransform());
            scaleTranform.Children.Add(new RotateTransform());
            scaleTranform.Children.Add(new TranslateTransform());
            Rotation = scaleTranform.Children[2] as RotateTransform;

            var transform = new TransformGroup();
            transform.Children.Add(scaleTranform);
            transform.Children.Add(new SkewTransform());
            transform.Children.Add(new RotateTransform());
            transform.Children.Add(new TranslateTransform());

            RenderTransform = transform;
        }

        /// <summary>
        /// Gets the rotation.
        /// </summary>
        /// <value>The rotation.</value>
        public RotateTransform Rotation { get; }

        public Schema Schema { get; set; }

        /// <summary>
        /// Gets or sets the position of the control.
        /// </summary>
        public Point Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (value != _position)
                {
                    _position = value;
                    this.SetLocation(_position);
                }
            }
        }

        internal void Move(Point newPosition)
        {
            if (!double.IsInfinity(newPosition.X) && !double.IsInfinity(newPosition.Y))
            {
                Position = newPosition;
            }
        }
        internal void Resize(double width, double height)
        {
            if (!double.IsInfinity(width) && !double.IsInfinity(height))
            {
                if (Width != width || Height != height)
                {
                    Width = width;
                    Height = height;
                }
            }
        }

    }
}