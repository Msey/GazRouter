using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Visuals;
using Telerik.Windows.Diagrams.Core;
using IGraph = GazRouter.Flobus.Model.IGraph;

namespace GazRouter.Flobus.Services
{
    /// <summary>
    ///     Сервис для определения объектов находящихся под курсором
    /// </summary>
    public class HitTestService : SchemaServiceBase
    {
        public HitTestService(IGraph graph) : base(graph)
        {
        }

        public IWidget ItemUnderMouse
        {
            get
            {
                var widget =
                    Graph.Widgets.FirstOrDefault(x => x.IsMouseOver && x.IsEnabled && x.Visibility == Visibility.Visible);
                var pipelineWidget = widget as IPipelineWidget;
                if (pipelineWidget != null)
                {
                    return pipelineWidget.PipelineElementUnderMouse ?? widget;
                }
                return widget;
            }
        }

        public IEnumerable<IWidget> GetItemsNearPoint(Point point, double delta)
        {
            return Graph.Widgets.Where(x => x.IsEnabled && x.Bounds.InflateRect(delta).Contains(point, 0));
        }

        public WidgetBase GetTopItemNearPoint(Point point, double delta)
        {
            var items = GetItemsNearPoint(point, delta);
            return items.OrderBy(x => x.ZIndex).LastOrDefault() as WidgetBase;
        }

        public IEnumerable<ISchemaItem> GetItemsUnderRect(Rect rect)
        {
            return GetShapesUnderRect(rect).Union(GetPipelinesUnderRect(rect));
        }

        private IEnumerable<ISchemaItem> GetPipelinesUnderRect(Rect rect)
        {
            var foundPipelines = new List<IPipelineWidget>();

            foreach (
                var pipelineWidget in
                    Graph.Widgets.OfType<IPipelineWidget>()
                        .Where(p => p.Visibility == Visibility.Visible && p.IsEnabled))
            {
                if (rect.Contains(pipelineWidget.Bounds))
                {
                    foundPipelines.Add(pipelineWidget);
                    continue;
                }

                var points = new List<Point>(pipelineWidget.Points.Select(p => p.Position));
                if (rect.IntersectsLine(points))
                {
                    foundPipelines.Add(pipelineWidget);
                }
            }

            return foundPipelines;
        }

        private IEnumerable<ISchemaItem> GetShapesUnderRect(Rect rect)
        {
            return
                Graph.Widgets.Where(
                    x =>
                        !(x is IPipelineWidget) && x.IsEnabled && x.Visibility == Visibility.Visible &&
                        x.Bounds.IntersectsWith(rect)).Cast<ISchemaItem>();
        }
    }
}