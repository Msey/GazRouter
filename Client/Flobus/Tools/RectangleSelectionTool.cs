using System;
using System.Linq;
using System.Windows;
using Telerik.Windows.Diagrams.Core;
using HitTestService = GazRouter.Flobus.Services.HitTestService;
using SelectionService = GazRouter.Flobus.Services.SelectionService;
using ServiceLocator = GazRouter.Flobus.Services.ServiceLocator;

namespace GazRouter.Flobus.Tools
{
    public class RectangleSelectionTool : ToolBase, IMouseListener
    {
        public const string ToolName = "RectanbleSelection Tool";

        private Rect _rectangle;
        private Rect _rectangleTransformed;
        private Point _initialPointTransformed;
        private bool _clearSelectionOnNewSelectionFlag;
        private ServiceLocator _serviceLocator;

        public RectangleSelectionTool() : base(ToolName)
        {
            Cursor = DiagramCursors.RectSelection;
        }

        private HitTestService HitTestService => _serviceLocator.HitTestService;

        private SelectionService SelectionService => _serviceLocator.SelectionService;

        public override void Initialize(ServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public bool MouseDown(PointerArgs e)
        {
            InitialPoint = e.Point;
            _initialPointTransformed = e.TransformedPoint;
            if (HitTestService.ItemUnderMouse == null)
            {
                Graph.IsMouseCaptured = true;
            }
            return false;
        }

        public bool MouseMove(PointerArgs e)
        {
            if (IsActive)
            {
                if (!InitialPoint.HasValue)
                {
                    InitialPoint = e.Point;
                }

                var width = Math.Abs(InitialPoint.Value.X - e.Point.X);
                var height = Math.Abs(InitialPoint.Value.Y - e.Point.Y);
                var left = Math.Min(InitialPoint.Value.X, e.Point.X);
                var top = Math.Min(InitialPoint.Value.Y, e.Point.Y);

                _rectangle = new Rect(left, top, width, height);

                if (!_clearSelectionOnNewSelectionFlag && SelectionService.SelectedItemsCount != 0)
                {
                    SelectionService.ClearSelection();
                    _clearSelectionOnNewSelectionFlag = true;
                }

                var widthTransformed = Math.Abs(_initialPointTransformed.X - e.TransformedPoint.X);
                var heightTransformed = Math.Abs(_initialPointTransformed.Y - e.TransformedPoint.Y);
                var leftTransformed = Math.Min(_initialPointTransformed.X, e.TransformedPoint.X);
                var topTransformed = Math.Min(_initialPointTransformed.Y, e.TransformedPoint.Y);
                _rectangleTransformed = new Rect(leftTransformed, topTransformed, widthTransformed, heightTransformed);

                Graph.UpdateRectSelection(_rectangleTransformed);

                return true;
            }
            return false;
        }

        public bool MouseUp(PointerArgs e)
        {
            if (IsActive)
            {
                if (ToolService.IsShiftDown)
                {
                    if (!_rectangle.IsEmpty)
                    {
                        Graph.BringIntoView(_rectangleTransformed);
                    }
                }
                else
                {
                    if (!_rectangle.IsEmpty)
                    {
                        OnRectangleCreated();
                    }
                }
                Graph.UpdateRectSelection(Rect.Empty);
                ToolService.ActivatePrimaryTool();
                _clearSelectionOnNewSelectionFlag = false;

                return true;
            }
            return false;
        }

        private void OnRectangleCreated()
        {
            if (_rectangle.Width < 5 && _rectangle.Height < 5)
            {
                return;
            }

            var hits = HitTestService.GetItemsUnderRect(_rectangleTransformed).ToList();
            if (hits.Count > 0)
            {
                SelectionService.SelectItems(hits);
            }
            else
            {
                SelectionService.ClearSelection();
            }
        }
    }
}