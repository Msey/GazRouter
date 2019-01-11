using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.Flobus.FloScheme;
using Telerik.Windows.Diagrams.Core;
using IGraph = GazRouter.Flobus.Model.IGraph;

namespace GazRouter.Flobus.Services
{
    public class AdornerService : SchemaServiceBase
    {
        private const int InflateValue = 7;
        private IEnumerable<ISchemaItem> _selectedItems;
        private Rect _adornerBounds = Rect.Empty;
        private Rect _inflatedAdornerBounds = Rect.Empty;
        private bool _shouldPublish;

        public AdornerService(IGraph graph) : base(graph)
        {
        }

        public Rect AdornerBounds
        {
            get { return _adornerBounds; }
            private set
            {
                if (_adornerBounds != value)
                {
                    _adornerBounds = value;
                    _inflatedAdornerBounds = value.InflateRect(InflateValue);
                    _shouldPublish = true;
                }

                if (_shouldPublish)
                {
                    Graph.PublishDiagramEvent(DiagramEvent.SelectionBoundsChanged, null);
                    _shouldPublish = false;
                }
            }
        }

        public Rect InflatedAdornerBounds
        {
            get
            {
                if (_selectedItems == null || _selectedItems.Count() <= 1)
                {
                    return AdornerBounds;
                }
                return _inflatedAdornerBounds;
            }
        }

        public double ResizeActivationRadius
        {
            get
            {
                var resizeActivationRadius = Graph.GetAdornerPartResolver();
                return resizeActivationRadius?.GetResizeActivationRadius() ?? 0;
            }
        }

        public void UpdateAdornerBounds(Rect newBounds)
        {
            var minSize = DiagramConstants.MinimumAdornerSize;
            AdornerBounds = new Rect(newBounds.TopLeft(),
                new Size(Math.Max(minSize, newBounds.Width), Math.Max(minSize, newBounds.Height)));
        }

        public void UpdateAdornerBounds(Point deltaPosition)
        {
            var rect = new Rect(
                AdornerBounds.X + deltaPosition.X,
                AdornerBounds.Y + deltaPosition.Y,
                Math.Max(DiagramConstants.MinimumAdornerSize, AdornerBounds.Height),
                Math.Max(DiagramConstants.MinimumAdornerSize, AdornerBounds.Width));
            AdornerBounds = rect;
        }

        /// <summary>
        ///     Возвращает правую нижнюю точку изменения размера
        /// </summary>
        /// <returns></returns>
        public Point BottomRight()
        {
            var resolver = Graph.GetAdornerPartResolver();
            if (resolver != null)
            {
                var offset = resolver.GetBottomRightResizeHandleOffset();
                return InflatedAdornerBounds.BottomRight(0, pointOffset: offset);
            }
            return new Point(double.NaN, double.NaN);
        }
    }
}