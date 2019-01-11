using System.Linq;
using System.Windows;
using GazRouter.Flobus.Visuals;
using Telerik.Windows.Diagrams.Core;
using AdornerService = GazRouter.Flobus.Services.AdornerService;
using ResizingService = GazRouter.Flobus.Services.ResizingService;
using SelectionService = GazRouter.Flobus.Services.SelectionService;
using ServiceLocator = GazRouter.Flobus.Services.ServiceLocator;

namespace GazRouter.Flobus.Tools
{
    public class ManipulationTool : ToolBase, IMouseListener
    {
        internal const string ToolNameNwse = "Resizing ToolNWSE";
        internal const string ToolNameSenw = "Resizing ToolSenw";
        private readonly ResizeDirection _resizeDirection;
        private ServiceLocator _serviceLocator;
        private bool _isInitialized;

        public ManipulationTool(string toolName) : base(toolName)
        {
            switch (toolName)
            {
                case ToolNameSenw:
                    _resizeDirection = ResizeDirection.SouthEastNorthWest;
                    Cursor = DiagramCursors.SizeNWSE;
                    break;
            }
        }

        private AdornerService AdornerService => _serviceLocator.AdornerService;
        private SelectionService SelectionService => _serviceLocator.SelectionService;
        private ResizingService ResizingService => _serviceLocator.ResizingService;

        public override void Initialize(ServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public bool MouseDown(PointerArgs e)
        {
            if (!IsActive)
            {
                return false;
            }

            ResizingService.InitializeResize(SelectionService.SelectedItems.OfType<ShapeWidgetBase>(), 0,
                AdornerService.AdornerBounds,
                _resizeDirection, e.TransformedPoint);
            _isInitialized = true;
            Graph.IsMouseCaptured = true;

            return true;
        }

        public bool MouseMove(PointerArgs e)
        {
            if (!IsActive)
            {
                return false;
            }
            if (!_isInitialized)
            {
                if (ShouldActivatePrimaryTool(e.TransformedPoint))
                {
                    ToolService.ActivatePrimaryTool();
                }
                return false;
            }
            if (ResizingService.IsResizing || StartResize(e.TransformedPoint))
            {
                ResizingService.Resize(e.TransformedPoint);
            }
            return true;
        }

        public bool MouseUp(PointerArgs e)
        {
            if (!IsActive)
            {
                return false;
            }
            _isInitialized = false;

            if (ResizingService.IsResizing)
            {
                ResizingService.CompleteResize(AdornerService.AdornerBounds, e.TransformedPoint);
            }

            ToolService.ActivatePrimaryTool();
            return false;
        }

        private bool ShouldActivatePrimaryTool(Point transformedPosition)
        {
            return !AdornerService.BottomRight().AroundPoint(transformedPosition, AdornerService.ResizeActivationRadius);
        }

        private bool StartResize(Point currentPoint)
        {
            return ResizingService.StartResize(currentPoint);
        }
    }
}