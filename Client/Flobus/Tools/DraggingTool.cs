using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.Flobus.FloScheme;
using Telerik.Windows.Diagrams.Core;
using DraggingService = GazRouter.Flobus.Services.DraggingService;
using SelectionService = GazRouter.Flobus.Services.SelectionService;
using ServiceLocator = GazRouter.Flobus.Services.ServiceLocator;
using SnappingService = GazRouter.Flobus.Services.SnappingService;

namespace GazRouter.Flobus.Tools
{
    public class DraggingTool : ToolBase, IMouseListener
    {
        public const string ToolName = "Dragging Tool";

        private ServiceLocator _serviceLocator;
        private Point _startDragPoint;

        public DraggingTool() : base(ToolName)
        {
            Cursor = DiagramCursors.Dragging;
        }

        private DraggingService DraggingService => _serviceLocator.DraggingService;

        private SnappingService SnappingService => _serviceLocator.SnappingService;

        private SelectionService SelectionService => _serviceLocator.SelectionService;

        public override void Initialize(ServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public bool MouseDown(PointerArgs e)
        {
            _startDragPoint = e.TransformedPoint;
            DraggingService.InitializeDrag(e.TransformedPoint);

            return false;
        }

        public bool MouseMove(PointerArgs e)
        {
            if (!IsActive)
            {
                return false;
            }

            var isDraggingEnabled = SelectionService.SelectedItems.All(d => d.IsDraggingEnabled);
            if (isDraggingEnabled && DraggingService.CanDrag(e.TransformedPoint))
            {
                if (DraggingService.IsDragging || StartDrag(e.TransformedPoint))
                {
                    var snappedPosition = e.TransformedPoint;
                    var shapesToSnap = SelectionService.SelectedShapes;
                    if (shapesToSnap.Any())
                    {
                        snappedPosition = SnappingService.SnapItems(shapesToSnap, e.TransformedPoint);
                    }
                    {
                        DraggingService.Drag(snappedPosition);
                    }
                }
            }

            return true;
        }

        public bool MouseUp(PointerArgs e)
        {
            if (IsActive)
            {
                if (DraggingService.IsDragging)
                {
                    DraggingService.CompleteDrag();
                    SnappingService.ClearSnap();
                }
                ToolService.ActivatePrimaryTool();
            }

            return false;
        }

        private bool StartDrag(Point currentPoint)
        {
            IEnumerable<ISchemaItem> items;
            items = SelectionService.SelectedItems;
//            items = new List<ISchemaItem>() { (ISchemaItem) _serviceLocator.HitTestService.ItemUnderMouse};
            var dragStarted = DraggingService.StartDrag(items, currentPoint);
            if (dragStarted)
            {
                var hitItem = _serviceLocator.HitTestService.GetTopItemNearPoint(_startDragPoint, 10);
                _serviceLocator.SnappingService.InitializeSnap(hitItem, Graph.IsSnapToGridEnabled, _startDragPoint);
            }
            return dragStarted;
        }
    }
}