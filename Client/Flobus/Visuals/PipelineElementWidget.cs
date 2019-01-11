using System;
using System.Windows;
using GazRouter.Common.Events;
using GazRouter.Flobus.Extensions;
using GazRouter.Flobus.Model;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Events;
using Telerik.Windows.Diagrams.Core;
using Orientation = System.Windows.Controls.Orientation;
using PositionChangedEventArgs = GazRouter.Flobus.EventArgs.PositionChangedEventArgs;
using ServiceLocator = Microsoft.Practices.ServiceLocation.ServiceLocator;
namespace GazRouter.Flobus.Visuals
{
    /// <summary>
    ///     Базовый класс для всех элементов на газопроводе
    /// </summary>
    public abstract class PipelineElementWidget : WidgetBase
    {
        public static readonly DependencyProperty AngleProperty = DependencyProperty.Register(
            nameof(Angle), typeof(double), typeof(PipelineElementWidget),
            new PropertyMetadata(default(double), OnAnglePropertyChanged));

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
            nameof(Caption), typeof(string), typeof(PipelineElementWidget),
            new PropertyMetadata(string.Empty, CaptionPropertyChanged));

        public static readonly DependencyProperty KmProperty = DependencyProperty.Register(
            nameof(Km), typeof(double), typeof(PipelineElementWidget), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            nameof(Orientation), typeof(Orientation), typeof(PipelineElementWidget),
            new PropertyMetadata(Orientation.Horizontal, OnOrientationPropertyChanged));


        protected PipelineElementWidget([NotNull] PipelineWidget pipelineWidget, double km)
            : base(pipelineWidget.Schema)
        {
            if (pipelineWidget == null)
            {
                throw new ArgumentNullException(nameof(pipelineWidget));
            }
            Pipeline = pipelineWidget;
            Km = km;
            var pipelineSegment = pipelineWidget.PipelinePontsManager.FindSegment(km, true);
            if (pipelineSegment != null)
            {
                Orientation = pipelineSegment.Orientation;
                Angle = pipelineSegment.Angle;
            }
            else
            {
                Orientation = Orientation.Horizontal;
            }
            if (pipelineSegment == null)
            {
                IsError = true; // todo: ?
                return;
            }
            PipelinePoint = pipelineWidget.FindOrCreateInfraPoint(km);
            PipelinePoint.PositionChanged += OnPipelinePointOnPositionChanged;
            UpdateAngleAndPosition(PipelinePoint);
            pipelineWidget.GeometryChanged += (sender, args) => CalcPositionAndAngle();
        }

#region property
        public bool IsError { get; set; }
        public Orientation Orientation
        {
            get { return (Orientation) GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }
        public PointType Type => PointType.Infra;
        public double Km
        {
            get { return (double) GetValue(KmProperty); }
            set { SetValue(KmProperty, value); }
        }
        public string Caption
        {
            get { return (string) GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }
        public IPipelinePoint PipelinePoint { get; }
        public double Angle
        {
            get { return (double) GetValue(AngleProperty); }
            set { SetValue(AngleProperty, Math.Round(value, 0)); }
        }
        public PipelineWidget Pipeline { get; }
        protected virtual bool MakeInfra => true;
#endregion

#region methods
        public abstract void ShowHideKm(bool show);
        internal void UpdatePosition()
        {
            this.SetLocation(Position.Substract(Pipeline.Position));
        }
        protected virtual void CaptionChanged()
        {
        }
        protected virtual void UpdateContentLayout()
        {
        }
        protected void CalcPositionAndAngle()
        {
            throw new NotSupportedException();
            /*    var point = Pipeline.Points.FindPoint(Km);
            if (point == null)
            {
                var geometryPoint = Pipeline.Data.AddPoint(Km, Pipeline.Km2Point(Km));
                Pipeline.Points.Add(new PipelinePoint(Pipeline, geometryPoint));
            }
            else
            {
                Position = point.Position;

            }*/
        }
        protected override void OnPositionChanged(Point position)
        {
            //            base.OnPositionChanged(position);


            if (PipelinePoint != null)
            {
                PipelinePoint.Position = position;
                UpdatePosition();
            }
            
        }
        protected void UpdateAngleAndPosition(IPipelinePoint point)
        {
            Position = point.Position;

            var segment = Pipeline.PipelinePontsManager.FindSegment(Km, true);
            Orientation = segment.Orientation;
            Angle = segment.Angle;
        }
        protected virtual void OnAngleChanged()
        {
            Orientation = Angle%180 == 0 ? Orientation.Horizontal : Orientation.Vertical;
        }
        protected virtual void OnOrientationChanged()
        {
            UpdateContentLayout();
        }
        private static void CaptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PipelineElementWidget) d).CaptionChanged();
        }
        private static void OnAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PipelineElementWidget) d).OnAngleChanged();
        }
        private static void OnOrientationPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((PipelineElementWidget) o).OnOrientationChanged();
        }
        private void OnPipelinePointOnPositionChanged(object sender, PositionChangedEventArgs args)
        {
            UpdateAngleAndPosition((IPipelinePoint) sender);
        }
#endregion
    }
}