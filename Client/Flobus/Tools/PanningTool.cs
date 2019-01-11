using System.Windows;
using Telerik.Windows.Diagrams.Core;

namespace GazRouter.Flobus.Tools
{
    /// <summary>
    ///Инструмент для паронамирования схемы
    /// 
    /// </summary>
    public class PanningTool : PrimaryMouseTool
    {
        private Point _originalGraphPosition = new Point(double.NaN, double.NaN);
        private Point _currentGraphPosition = new Point(double.NaN, double.NaN);
        private Point _updatedGraphPosition = new Point(double.NaN, double.NaN);
        /// <summary>
        /// The name of the panning tool.
        /// 
        /// </summary>
        public const string ToolName = "Panning Tool";

        private Point _currentPoint;

        
        public PanningTool()
            : base(ToolName)
        {
            Cursor = DiagramCursors.Panning;
        }

        /// <inheritdoc/>
        public override bool MouseMove(PointerArgs e)
        {
            if (IsActive && HitItem == null && ToolService.IsMouseDown)
            {
                Graph.IsMouseCaptured = true;
                Point point = e.Point;
                _updatedGraphPosition = new Point(_currentGraphPosition.X + point.X - _currentPoint.X,
                    _currentGraphPosition.Y + point.Y - _currentPoint.Y);
                Graph.Pan(_updatedGraphPosition);
                _currentPoint = point;
                _currentGraphPosition = _updatedGraphPosition;
                return true;
            }
            return base.MouseMove(e);
        }

        /// <inheritdoc/>
        public override bool MouseDown(PointerArgs e)
        {
            _originalGraphPosition = Graph.Position;
            _currentGraphPosition = _originalGraphPosition;
            InitialPoint = _currentPoint = e.Point;
            return base.MouseDown(e);
        }

        /// <inheritdoc/>
        public override bool MouseUp(PointerArgs e)
        {
            if (IsActive)
            {
                ToolService.ActivatePrimaryTool();
            }
            return base.MouseUp(e);
        }

        /// <inheritdoc/>
        public override bool KeyDown(KeyArgs key)
        {
            if (!IsActive)
                return false;
            bool flag = base.KeyDown(key);
            if (ToolService.IsControlDown)
                return flag;
            ToolService.ActivatePrimaryTool();
            return true;
        }

        /// <inheritdoc/>
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            _originalGraphPosition = new Point(double.NaN, double.NaN);
            _currentGraphPosition = new Point(double.NaN, double.NaN);
            _updatedGraphPosition = new Point(double.NaN, double.NaN);
        }
    }
}