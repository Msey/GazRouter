using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace DataExchange.ASDU
{
    internal sealed class TreeLinkDrawHelper
    {
        private readonly RadTreeListView _treeLeft;
        private readonly RadTreeListView _treeRight;
        private readonly Canvas _drawingRoot;
        private readonly Func<bool> _isInLinkDrawMode;
        private readonly LineGeometry _linkIndicatorLine;
        private readonly EllipseGeometry _linkIndicatorStart;
        private readonly EllipseGeometry _linkIndicatorEnd;
        private readonly Path _linkIndicatorPath;

        private bool IsInLinkDrawingMode => _isInLinkDrawMode != null && _isInLinkDrawMode();

        public TreeLinkDrawHelper(RadTreeListView treeLeft, RadTreeListView treeRight, Canvas drawingRoot, Func<bool> isInLinkDrawMode)
        {
            _treeLeft = treeLeft;
            _treeRight = treeRight;
            _drawingRoot = drawingRoot;
            _isInLinkDrawMode = isInLinkDrawMode;
            _linkIndicatorStart = new EllipseGeometry
            {
                RadiusX = 3,
                RadiusY = 3,
                Center = new Point(0, 0)
            };

            _linkIndicatorEnd = new EllipseGeometry
            {
                RadiusX = 3,
                RadiusY = 3,
                Center = new Point(0, 0)
            };

            _linkIndicatorLine = new LineGeometry
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 0)
            };

            var geomGroup = new GeometryGroup();
            geomGroup.Children.Add(_linkIndicatorLine);
            geomGroup.Children.Add(_linkIndicatorStart);
            geomGroup.Children.Add(_linkIndicatorEnd);

            _linkIndicatorPath = new Path
            {
                Stroke = new SolidColorBrush(GazRouter.Common.GoodStyles.Colors.Purple),
                StrokeThickness = 1,
                Fill = new SolidColorBrush(GazRouter.Common.GoodStyles.Colors.Purple),
                Data = geomGroup,
                Visibility = Visibility.Collapsed,
                Opacity = 0.8
            };
            _drawingRoot.Children.Add(_linkIndicatorPath);

            _treeLeft.LayoutUpdated += OnTreeLayoutUpdated;
            _treeRight.LayoutUpdated += OnTreeLayoutUpdated;

            _treeLeft.SelectionChanged += OnTreeSelectionChanged;
            _treeRight.SelectionChanged += OnTreeSelectionChanged;
        }

        private void OnTreeSelectionChanged(object sender, SelectionChangeEventArgs selectionChangeEventArgs)
        {
            if (IsInLinkDrawingMode && sender is RadTreeListView)
            {
                var tlv = sender as RadTreeListView;
                if (tlv.SelectedItem != null)
                {
                    tlv.ScrollIntoView(tlv.SelectedItem);
                }
            }
        }

        private void OnTreeLayoutUpdated(object sender, EventArgs eventArgs)
        {
            DrawLinkIndicator();
        }

        private void DrawLinkIndicator()
        {
            _linkIndicatorPath.Visibility = Visibility.Collapsed;

            if (!IsInLinkDrawingMode)
            {
                return;
            }

            if (_treeLeft.SelectedItem != null && _treeRight.SelectedItem != null)
            {
                var iusCell = _treeLeft.GetRowForItem(_treeLeft.SelectedItem)?.Cells?.First();
                if (iusCell == null)
                {
                    return;
                }

                GeneralTransform gt;
                try
                {
                    gt = iusCell.TransformToVisual(_drawingRoot);
                }
                catch (ArgumentException)
                {
                    // Nevermind, controls are not loaded yet/already unloaded
                    return;
                }
                var offset = gt.Transform(new Point(iusCell.ActualWidth, iusCell.ActualHeight / 2));
                _linkIndicatorStart.Center = new Point(offset.X, offset.Y);
                _linkIndicatorLine.StartPoint = new Point(offset.X, offset.Y);

                var asduCell = _treeRight.GetRowForItem(_treeRight.SelectedItem)?.Cells?.First();
                if (asduCell == null)
                {
                    return;
                }
                try
                {
                    gt = asduCell.TransformToVisual(_drawingRoot);
                }
                catch (ArgumentException)
                {
                    // Nevermind, controls are not loaded yet/already unloaded
                    return;
                }
                offset = gt.Transform(new Point(0, asduCell.ActualHeight / 2));
                _linkIndicatorEnd.Center = new Point(offset.X, offset.Y);
                _linkIndicatorLine.EndPoint = new Point(offset.X, offset.Y);
                _linkIndicatorPath.Visibility = Visibility.Visible;
            }
        }
    }
}