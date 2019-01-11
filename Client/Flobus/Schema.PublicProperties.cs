using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using GazRouter.Flobus.Visuals;
using JetBrains.Annotations;

namespace GazRouter.Flobus
{
    public partial class Schema
    {
        public static readonly DependencyProperty IsAdditionalContentVisibleProperty = DependencyProperty.Register(
            nameof(IsAdditionalContentVisible), typeof(bool), typeof(Schema), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsInformationAdornerVisibileProperty = DependencyProperty.Register(
            nameof(IsInformationAdornerVisibile), typeof(bool), typeof(Schema), new PropertyMetadata(true));

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(
            "IsReadOnly", typeof(bool), typeof(Schema), new PropertyMetadata(true));
        
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position", typeof(Point), typeof(Schema),
            new PropertyMetadata(new Point(0, 0), OnPositionPropertyChanged));

        public static readonly DependencyProperty ShowInfoProperty = DependencyProperty.Register(
            "ShowInfo", typeof(bool), typeof(Schema), new PropertyMetadata(false));

        public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register(
            "Viewport", typeof(Rect), typeof(Schema), new PropertyMetadata(new Rect(), OnViewportPropertyChanged));

        private Rect _viewport;

        public bool IsInformationAdornerVisibile
        {
            get { return (bool) GetValue(IsInformationAdornerVisibileProperty); }
            set { SetValue(IsInformationAdornerVisibileProperty, value); }
        }

        public ObservableCollection<WidgetBase> Items { get; } = new ObservableCollection<WidgetBase>();

        public bool ShowInfo
        {
            get { return (bool) GetValue(ShowInfoProperty); }
            set { SetValue(ShowInfoProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool) GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        private static bool _isRepair = false;
        public static bool IsRepair
        {
            get { return _isRepair; }
            set { _isRepair = value;
                if (!value)
                    CleanUpHandlers();
            }
        }

        public bool IsAdditionalContentVisible
        {
            get { return (bool) GetValue(IsAdditionalContentVisibleProperty); }
            set { SetValue(IsAdditionalContentVisibleProperty, value); }
        }

        public Rect Viewport
        {
            get { return _viewport; }
            set
            {
                if (_viewport != value)
                {
                    SetValue(ViewportProperty, value);
                }
            }
        }

        public Point Position
        {
            get { return (Point) GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        /*
                public PipelineWidget AddPipeline(Point startPoint, Point endPoint, double kmStart, double kmEnd)
                {
                    var pipeWidget = new PipelineWidget(this, startPoint.Round(), endPoint.Round(), kmStart, kmEnd);

                    AddPipeline(pipeWidget);
                    return pipeWidget;

                }
        */

        /*  public void AddPipeline([NotNull] PipelineWidget pipeWidget)
        {

            DetachPipelineEvents(pipeWidget);
           AttachPipelineEvents(pipeWidget);
            AddDiagramItem(pipeWidget);
            AddItemToCanvas(pipeWidget);


        }*/

        public void AddCompressorShop([NotNull] CompressorShopWidget compressorShop)
        {
            if (compressorShop == null)
            {
                throw new ArgumentNullException(nameof(compressorShop));
            }

            AddDiagramItem(compressorShop);
        }

        public void AddDiagramItem(WidgetBase item)
        {
            if (!Items.Contains(item))
            {
                Items.Add(item);
            }
        }

        private static void OnViewportPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var schema = d as Schema;
            if (schema != null)
            {
                schema._viewport = (Rect) e.NewValue;
                schema.OnViewportChanged((Rect) e.OldValue, (Rect) e.NewValue);
            }
        }

        private void AttachPipelineEvents(PipelineWidget pipelineWidget)
        {
            pipelineWidget.PropertyChanged += OnPipelinePropertyChanged;
        }

        private void DetachPipelineEvents(PipelineWidget pipelineWidget)
        {
            pipelineWidget.PropertyChanged -= OnPipelinePropertyChanged;
        }

        private void OnPipelinePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var pipeline = (PipelineWidget) sender;
            switch (e.PropertyName)
            {
                case nameof(PipelineWidget.IsSelected):
                    if (pipeline.IsSelected)
                    {
                        ServiceLocator.SelectionService.SelectItem(pipeline, true);
                    }
                    else
                    {
                        ServiceLocator.SelectionService.DeselectItem(pipeline);
                    }
                    break;
                case nameof(PipelineWidget.StartPoint):
                case nameof(PipelineWidget.EndPoint):
                    ServiceLocator.VirtualzationService.Virtualize(new[] {pipeline});
                    break;
                case nameof(PipelineWidget.Bounds):
                    ServiceLocator.VirtualzationService.Virtualize(new[] {pipeline});
                    if (pipeline.IsSelected && !IsServiceManipulating)
                    {
                        RefreshManipulationAdorner();
                    }
                    break;
            }
        }
    }
}