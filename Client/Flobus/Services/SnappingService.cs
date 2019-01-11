using System.Collections.Generic;
using System.Windows;
using GazRouter.Flobus.Visuals;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Services
{
    public class SnappingService : SchemaServiceBase
    {
        private Point _previousMousePosition;
        private WidgetBase _mainItem;
        private bool _isSnapToGridEnabled;

        public SnappingService(Schema schema) : base(schema)
        {
        }

        private int Snap => Graph.Snap;

        public void InitializeSnap(WidgetBase mainItem, bool isSnapToGridEnabled, Point initialPosiiton)
        {
            _mainItem = mainItem;
            _isSnapToGridEnabled = isSnapToGridEnabled;
            _previousMousePosition = initialPosiiton;
        }

        public void ClearSnap()
        {
            _mainItem = null;
        }

        public Point SnapItems(IEnumerable<CompressorShopWidget> selectedItems, Point newPosition)
        {
            var draggingOffset = newPosition.Substract(_previousMousePosition);
            var snappingOffset = CalculateSnappingOffset(selectedItems, draggingOffset);
            var finalOffset = draggingOffset.Add(snappingOffset);

            var snappedPosition = _previousMousePosition.Add(finalOffset);
            _previousMousePosition = snappedPosition;
            return snappedPosition;
        }

        public Point SnapPoint(Point point)
        {
            return point.Snap(Snap, Snap);
        }

        private Point CalculateSnappingOffset(IEnumerable<WidgetBase> selectedItems, Point offset)
        {
            var snapping = new Point(0, 0);
            if (_isSnapToGridEnabled && _mainItem != null)
            {
                var futurePosition = _mainItem.Position.Add(offset);
                var snappedPosition = SnapPoint(futurePosition);
                snapping = snappedPosition.Substract(futurePosition);
            }
            return snapping;
        }
    }
}