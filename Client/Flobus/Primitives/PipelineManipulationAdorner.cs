using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Model;
using GazRouter.Flobus.Visuals;

namespace GazRouter.Flobus.Primitives
{
    public class PipelineManipulationAdorner : AdornerBase
    {
        /// <summary>
        ///     Identifies the IsPipelineAdornerActive dependency property.
        /// </summary>
        public static readonly DependencyProperty IsPipelineAdornerActiveProperty =
            DependencyProperty.Register(
                "IsPipelineAdornerActive",
                typeof (bool),
                typeof (PipelineManipulationAdorner),
                new PropertyMetadata(false, OnIsPipelineAdornerActiveChanged));

        public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register(
            nameof(StartPoint), typeof (Point), typeof (PipelineManipulationAdorner), new PropertyMetadata(new Point()));

        public static readonly DependencyProperty EndPointProperty = DependencyProperty.Register(
            "EndPoint", typeof (Point), typeof (PipelineManipulationAdorner), new PropertyMetadata(new Point()));

        private Canvas _pipelineManipulationPointsSurface;

        public Point StartPoint
        {
            get { return (Point) GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        public Point EndPoint
        {
            get { return (Point) GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        public bool IsPipelineAdornerActive
        {
            get { return (bool) GetValue(IsPipelineAdornerActiveProperty); }
            set { SetValue(IsPipelineAdornerActiveProperty, value); }
        }

        public List<PipelineEditPointControl> PipelineEditors { get; } = new List<PipelineEditPointControl>();

        /// <summary>
        ///     Gets the active connection that is currently adorned.
        /// </summary>
        protected PipelineWidget ActivePipeline { get; private set; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _pipelineManipulationPointsSurface =
                GetTemplateChild("PipelineManipulationPointsSurface") as Canvas;

            if (_pipelineManipulationPointsSurface != null)
            {
                _pipelineManipulationPointsSurface.Unloaded += OnPipelineManipulationPointsSurfaceUnloaded;
            }
        }

        public void Update(IEnumerable<ISchemaItem> items, bool show)
        {
            var connection = items.FirstOrDefault() as PipelineWidget;
            if (connection == null || connection.Visibility == Visibility.Collapsed)
            {
                Clear();
                Visibility = Visibility.Collapsed;
                _pipelineManipulationPointsSurface?.Children.Clear();
                return;
            }

            var isSingleSelectedPipeline = items.Count() == 1;
            IsPipelineAdornerActive = isSingleSelectedPipeline && connection.IsEnabled;

            Visibility = show ? Visibility.Visible : Visibility.Collapsed;

            if (IsPipelineAdornerActive)
            {
                ActivePipeline = connection;
                ActivePipeline.Adorner = this;
                /*
                                if (ActivePipeline.ManipulationPoints.Count >= 2)
                                {
                                    ActivePipeline.ManipulationPoints[0].PipelinePoint = new NewPipelinePoint(ActivePipeline,ActivePipeline.StartPoint, ActivePipeline.KmBegining, PointType.First);
                                    ActivePipeline.ManipulationPoints[1].PipelinePoint = new NewPipelinePoint(ActivePipeline, ActivePipeline.EndPoint, ActivePipeline.KmEnd, PointType.Last);
                                }
                */

                UpdatePipelineEditPoints();
            }
            else
            {
                Clear();
            }
        }

        private static void OnIsPipelineAdornerActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (PipelineManipulationAdorner) d;
            self.OnIsPipelineAdornerActiveChanged();
        }

        private void OnIsPipelineAdornerActiveChanged()
        {
            if (!IsPipelineAdornerActive)
            {
                ActivePipeline.Adorner = null;
                ActivePipeline = null;
            }
        }

        private void UpdatePipelineEditPoints()
        {
            if (ActivePipeline == null)
            {
                Clear();
            }
            else
            {
                CreateEditPoints();

                if (ActivePipeline.ManipulationPoints.Count >= 2)
                {
                    StartPoint = ActivePipeline.ManipulationPoints[0].Position;
                    EndPoint = ActivePipeline.ManipulationPoints[1].Position;
                }
            }
        }

        private void CreateEditPoints()
        {
            if (ActivePipeline == null)
            {
                return;
            }
            if (_pipelineManipulationPointsSurface == null)
            {
                return;
            }
            _pipelineManipulationPointsSurface.Children.Clear();

            // we start from scratch every time
            PipelineEditors.Clear();

            var cachedStartPoint = ActivePipeline.StartPoint;
            var cachedEndPoint = ActivePipeline.EndPoint;

            // the endpoints first
            /*      AddEditorPoint(ActivePipeline.Points.Start); //new NewPipelinePoint(ActivePipeline,cachedStartPoint, ActivePipeline.KmBegining, PointType.First));
            AddEditorPoint(ActivePipeline.Points.End);// new NewPipelinePoint(ActivePipeline, cachedEndPoint, ActivePipeline.KmEnd, PointType.Last));*/

            foreach (
                var point in ActivePipeline.Points.Where(p => p.Type != PointType.Turn))
            {
                AddEditorPoint(point);
            }
        }

/*
        private void AddEditorPoint(PointType type, Point p)
        {
            var editor = new PipelineEditPointControl(type, p, ActivePipeline);
            PipelineEditors.Add(editor);
            editor.UpdateDisplayElement();
            _pipelineManipulationPointsSurface.Children.Add(editor);
        }
*/

        private void AddEditorPoint(IPipelinePoint p)
        {
            var editor = new PipelineEditPointControl(p, ActivePipeline) {PipelinePoint = p};
            PipelineEditors.Add(editor);
            editor.UpdateDisplayElement();
            _pipelineManipulationPointsSurface.Children.Add(editor);
        }

        private void Clear()
        {
        }

        private void OnPipelineManipulationPointsSurfaceUnloaded(object sender, RoutedEventArgs e)
        {
            if (_pipelineManipulationPointsSurface != null)
            {
                _pipelineManipulationPointsSurface.Children.Clear();
                _pipelineManipulationPointsSurface.Unloaded -= OnPipelineManipulationPointsSurfaceUnloaded;
            }
        }
    }
}