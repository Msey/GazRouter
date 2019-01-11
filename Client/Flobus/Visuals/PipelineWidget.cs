using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GazRouter.Controls.Dialogs.ObjectDetails;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.Flobus.Dialogs;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.Misc;
using GazRouter.Flobus.Model;
using GazRouter.Flobus.Primitives;
using GazRouter.Flobus.UiEntities;
using GazRouter.Flobus.UiEntities.FloModel;
using GazRouter.Flobus.Utilites;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Controls;
using Telerik.Windows.Diagrams.Core;
using CoreUtils = Telerik.Windows.Diagrams.Core.Utils;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using DistanceToPointComparer = GazRouter.Flobus.Utilites.DistanceToPointComparer;
using Orientation = System.Windows.Controls.Orientation;
using PositionChangedEventArgs = GazRouter.Flobus.EventArgs.PositionChangedEventArgs;

namespace GazRouter.Flobus.Visuals
{
    /// <summary>
    ///     Визуальный компонент для газопровода
    /// </summary>
    public class PipelineWidget : WidgetBase, ISupportContextMenu, IPipelineWidget
    {
        public static readonly DependencyProperty MarkupsProperty = DependencyProperty.Register(
            nameof(Markups), typeof(ICollection<PipelineMarkup>), typeof(PipelineWidget),
            new PropertyMetadata(null, OnMarkupPropertyChanged));

        public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register(
            nameof(StrokeThickness), typeof(double), typeof(PipelineWidget), new PropertyMetadata(2d));

        public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register(
            nameof(Stroke), typeof(Color), typeof(PipelineWidget),
            new PropertyMetadata(Colors.Black, OnStrokePropertyChanged));

        public static readonly DependencyProperty StartPointProperty = DependencyProperty.Register(
            nameof(StartPoint), typeof(Point), typeof(PipelineWidget),
            new PropertyMetadata(new Point(), OnStartPointPropertyChanged));

        public static readonly DependencyProperty EndProperty = DependencyProperty.Register(
            nameof(EndPoint), typeof(Point), typeof(PipelineWidget),
            new PropertyMetadata(new Point(), OnEndPointPropertyChanged));

        public static readonly DependencyProperty IdProperty = DependencyProperty.Register(
            nameof(Id), typeof(Guid), typeof(PipelineWidget), new PropertyMetadata(default(Guid)));

        public static readonly DependencyProperty StandardColorsModeProperty = DependencyProperty.Register(
            nameof(StandardColorsMode), typeof(bool), typeof(PipelineWidget),
            new PropertyMetadata(false, OnStandardColorsModePropertyChanged));        

        protected static readonly Style ToolTipStyle =
            System.Windows.Application.Current.Resources["WidgetToolTip"] as Style;

        private readonly bool _init;
        private readonly List<Path> _markupElements = new List<Path>();
        private readonly IList<PipelineElementWidget> _items = new List<PipelineElementWidget>();

        private Path _deferredPath;
        private Geometry _geometry;

        private Path _geometryPath;
        private int _markupsPlaceIndex;

        public PipelineWidget(IPipeline pipeline, Schema scm) : base(scm)

        {
            if (pipeline == null)
            {
                throw new ArgumentNullException(nameof(pipeline));
            }

            DefaultStyleKey = typeof(PipelineWidget);
            BindVisibilityToVirtualizationVisibility();
            LayoutUpdated += PipelineWidget_LayoutUpdated;
            Id = pipeline.Id;
            var startPoint = pipeline.StartPoint.Round();
            var endPoint = pipeline.EndPoint.Round();
            PipelinePontsManager = new PipelinePointsManager(startPoint, endPoint, pipeline.KmBegining, pipeline.KmEnd,
                pipeline.IntermediatePoints, pipeline.OverlaySegments);
            KmBegining = pipeline.KmBegining;
            KmEnd = pipeline.KmEnd;
            _init = true;
            StartPoint = startPoint;
            EndPoint = endPoint;
            _init = false;
            Position = CoreUtils.GetTopLeftPoint(Points.Select(p => p.Position));
            OnPropertyChanged(nameof(StartPoint));
            OnPropertyChanged(nameof(EndPoint));
            PipelinePontsManager.Start.PositionChanged += StartOnPositionChanged;
            PipelinePontsManager.End.PositionChanged += EndOnPositionChanged;
            //this.KeyDown += PipelineWidget_KeyDown;
            //Canvas.KeyDown += PipelineWidget_KeyDown;

            Data = pipeline;
            var c = new Binding
            {
                //   Source = pipeline,
                Path = new PropertyPath(nameof(IPipeline.StartPoint)),
                Mode = BindingMode.TwoWay
            };
            SetBinding(StartPointProperty, c);
            c = new Binding
            {
                // Source = pipeline,
                Path = new PropertyPath(nameof(IPipeline.EndPoint)),
                Mode = BindingMode.TwoWay
            };

            SetBinding(EndProperty, c);

            KmBegining = pipeline.KmBegining;
            KmEnd = pipeline.KmEnd;
            
            Initialize();

            if (((GazRouter.Flobus.Model.EntityBase<GazRouter.DTO.ObjectModel.Pipelines.PipelineDTO, System.Guid>)(pipeline)).Dto.Type == DTO.Dictionaries.PipelineTypes.PipelineType.RefiningDeviceChamber)
            {
                AddPlug(pipeline.KmEnd);
            }
        }
                
        public event EventHandler GeometryChanged;

        public double StrokeThickness
        {
            get { return (double) GetValue(StrokeThicknessProperty); }
            set { SetValue(StrokeThicknessProperty, value); }
        }

        public Color Stroke
        {
            get { return (Color) GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public double KmBegining { get; set; }
        public double KmEnd { get; set; }

        public IPipeline Data
        {
            get { return (IPipeline) DataContext; }
            protected set
            {
                DataContext = value;
                //     Points ;
                //  PipelinePoints = value.Points.Cast<IPipelinePoint>().ToList();
//                CreateBindings();
            }
        }

        public Guid Id
        {
            get { return (Guid) GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }


        /// <summary>
        ///     The connection adorner which helps manipulating this connection.
        /// </summary>
        public PipelineManipulationAdorner Adorner { get; set; }

        public IEnumerable<PipelineElementWidget> Items => _items;
        public Canvas Canvas { get; private set; }

        public int PointsCount => PipelinePontsManager.Count;

        public ICollection<PipelineMarkup> Markups
        {
            get { return (ICollection<PipelineMarkup>) GetValue(MarkupsProperty); }
            set { SetValue(MarkupsProperty, value); }
        }

        public bool StandardColorsMode
        {
            get { return (bool) GetValue(StandardColorsModeProperty); }
            set { SetValue(StandardColorsModeProperty, value); }
        }

        public IEnumerable<IPipelinePoint> Points => PipelinePontsManager.Points;

        public IList<IPipelineEditPoint> ManipulationPoints
        {
            get
            {
                if (Adorner == null)
                {
                    var list = new List<IPipelineEditPoint>
                    {
                        new PipelineEditPointControl(PipelinePontsManager.Start, this),
                        new PipelineEditPointControl(PipelinePontsManager.End, this)
                    };
                    return list;
                }
                return Adorner.PipelineEditors.Cast<IPipelineEditPoint>().ToList();
            }
        }

        public Point StartPoint
        {
            get { return (Point) GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        public Point EndPoint
        {
            get { return (Point) GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        public IWidget PipelineElementUnderMouse
        {
            get { return _items.FirstOrDefault(i => i.IsMouseOver); }
        }

        public override Rect Bounds
        {
            get
            {
                var actualSize = ActualSize;

                if (Geometry == null || Geometry.Bounds.IsEmpty || actualSize.IsEmpty)
                {
                    return new Rect(Position, actualSize);
                }
                var size = Geometry.Bounds.ToSize();
                var position = Geometry.Bounds.TopLeft().Add(Position);
                if (size.Width.IsEqual(0) && actualSize.Width.IsNotEqual(0))
                {
                    size.Width = actualSize.Width;
                }
                if (size.Height.IsEqual(0) && actualSize.Height.IsNotEqual(0))
                {
                    size.Height = actualSize.Height;
                }
                return new Rect(position, size);
            }
        }

        internal IPipelinePointManager PipelinePontsManager { get; }

        protected IGraphServiceLocator ServiceLocator { get; private set; }

        private Geometry Geometry
        {
            get { return _geometry; }
            set
            {
                if (_geometry != value)
                {
                    _geometry = value;
                    OnPropertyChanged(nameof(Bounds));
                }
            }
        }

        public override void Update()
        {
            //  Stroke = new SolidColorBrush(Schema.StandardColorsMode ? Data.StandardColor : Data.Color);
            //     StrokeThickness = Data.Thickness;
            Schema.UpdateAdorners();
            UpdateGeometry();
            UpdateDeferredGeometry(null);

            /*   if (Data != null)
            {
                SetToolTip(
                    _geometryPath,
                    $"{Data.Name}\n({Data.PipeTypeName})\nкм. начала: {Data.KmBegining}\nкм.конца: {Data.KmEnd}");
            }*/
        }
        
        
        public void UpdateDefferedGeometry(Point startPoint, Point endPoint, Point[] middlePoints)
        {
            var list = middlePoints.ToList();
            list.Insert(0, startPoint);

            list.Add(endPoint);

            var segments = PipelinePontsManager.CreateGeometrySegments(list);
            var pipelineGeometry = CreatePipelineGeometry(segments[0].Start, segments[segments.Count - 1].End,
                segments.Skip(1).Select(s => s.Start).ToList(), false, false);
            UpdateDeferredGeometry(pipelineGeometry);
        }

        public IPipelinePoint AddPoint(Point p)
        {
            var seg = FindSegment(p);

            if (seg != null)
            {
                p = seg.Orientation == Orientation.Horizontal
                    ? new Point(p.Round().X, seg.BeginingPoint.Position.Y)
                    : new Point(seg.BeginingPoint.Position.X, p.Round().Y);
                var km = seg.Point2Km(p);

                var pipelinePoint = Data != null
                    ? PipelinePontsManager.CreatePoint(Data.AddPoint(km, p), PointType.Intermediate)
                    : PipelinePontsManager.CreatePoint(PointType.Intermediate, km, p);
                PipelinePontsManager.Add(pipelinePoint);
                Schema.UpdateAdorners();
                return pipelinePoint;
            }
            return null;
        }
        
        public void RemovePoint(IPipelinePoint point)
        {
            Data?.RemovePoint(point.Km);
            PipelinePontsManager.RemovePoint(point);
            Update();
            Schema.UpdateAdorners();
        }

        public void MovePoint(IPipelinePoint point, Point prev_point)
        {
            PipelinePontsManager.MovePoint(point, FindSegment(prev_point).BeginingPoint);
        }

        public void RecalculateIntermediateKm(IPipelinePoint point)
        {
            PipelinePontsManager.RecalculateIntermediateKm(point);
        }

        public void Move(Vector offset)
        {
            PipelinePontsManager.Move(offset);
        }

        public override void Deselect()
        {
            if (Schema.IsReadOnly)
            {
//                Points.ForEach(p => p.Visibility = Visibility.Collapsed);
            }
        }

        public override void Select()
        {
            Schema.SetSelectedWidget(this);
        }
        
        public void FillMenu(RadContextMenu menu, Point mousePosition, Schema schema)
        {
            if (!Schema.IsReadOnly)
            {
                menu.AddCommand("Добавить точку", new DelegateCommand<Point?>(
                    arg =>
                    {
                        Debug.Assert(arg != null, "arg != null");
                        var pt = mousePosition;

                        AddPoint(pt);
                    }, arg =>
                    {
                        var seg = FindSegment(mousePosition);
                        return seg != null && seg.EndPoint.Km - seg.BeginingPoint.Km > 0.001;
                    }), menu.MousePosition);
                menu.AddCommand("Стиль...", new DelegateCommand(
                    () => { new PipelineStyleEditDialog {DataContext = this}.Show(); }));


                Dictionary<IPipelineOmElement, string> hiddenMeasuringLines  = schema.SchemaSource.GetHiddenMeasuringLines(Data.Id);
                if (hiddenMeasuringLines != null && hiddenMeasuringLines.Count> 0)
                {
                    RadMenuItem restoreML = new RadMenuItem();
                    restoreML.Header = "Восстановить замерные линии";
                    foreach (var key in hiddenMeasuringLines.Keys)
                    {
                        RadMenuItem mp = new RadMenuItem();
                        mp.Header = hiddenMeasuringLines[key];
                        //mp.Tag = ml;
                        mp.Command = new DelegateCommand(() =>
                        {
                            if (schema.SchemaSource.RestoreMeasuringLine(key.Id) != Guid.Empty) 
                            {
                                AddMeasuringLine(key);
                            }
                        });
                        restoreML.Items.Add(mp);
                    }
                    menu.Items.Add(restoreML);
                   
                }

                Dictionary<IDistrStation, string> hiddenDistributionStations = schema.SchemaSource.GetHiddenDistributionStations(Data.Id);
                if (hiddenDistributionStations != null && hiddenDistributionStations.Count > 0)
                {
                    RadMenuItem restoreML = new RadMenuItem();
                    restoreML.Header = "Восстановить ГРС";
                    foreach (var key in hiddenDistributionStations.Keys)
                    {
                        RadMenuItem mp = new RadMenuItem();
                        mp.Header = hiddenDistributionStations[key];
                        //mp.Tag = ml;
                        mp.Command = new DelegateCommand(() =>
                        {
                            if (schema.SchemaSource.RestoreDistributionStation(key.Id) != Guid.Empty)
                            {
                                AddDistributingStation(key);
                            }
                        });
                        restoreML.Items.Add(mp);
                    }
                    menu.Items.Add(restoreML);
                }

                Dictionary<IPipelineOmElement, string> hiddenReducingStations = schema.SchemaSource.GetHiddenReducingStations(Data.Id);
                if (hiddenReducingStations != null && hiddenReducingStations.Count > 0)
                {
                    RadMenuItem restoreML = new RadMenuItem();
                    restoreML.Header = "Восстановить ПРГ";
                    foreach (var key in hiddenReducingStations.Keys)
                    {
                        RadMenuItem mp = new RadMenuItem();
                        mp.Header = hiddenReducingStations[key];
                        //mp.Tag = ml;
                        mp.Command = new DelegateCommand(() =>
                        {
                            if (schema.SchemaSource.RestoreReducingStation(key.Id) != Guid.Empty)
                            {
                                AddReducingStation(key);
                            }
                        });
                        restoreML.Items.Add(mp);
                    }
                    menu.Items.Add(restoreML);
                }
                


                //ata.
                //FloModel.FloModelHelper.MeasuringLineJsonsDict.v

                menu.AddCommand("Удалить", new DelegateCommand(
                    () => RadWindow.Confirm(new DialogParameters
                    {
                        Header = "Внимание!",
                        Content = @"Вы уверены, что хотите удалить газопровод со схемы?",
                        Closed = (sender, e1) =>
                        {
                            if (e1.DialogResult.HasValue && e1.DialogResult.Value)
                            {
                                Schema.RemovePipelineWidget(this);
                            }
                        }
                    })));
                //menu.AddCommand("Выровнять", new DelegateCommand<Point?>(ArrangePipiline, p => p.HasValue), mousePosition);
                menu.AddSeparator();
            }

            if (Data == null)
            {
                return;
            }

            if (!Schema.IsReadOnly)
            {
                menu.AddCommand("Показать в дереве", new DelegateCommand(
                    () => Schema.GotoTreeCommand.Execute(Id),
                    () => Schema.GotoTreeCommand != null && Schema.GotoTreeCommand.CanExecute(Id)));
            }
            menu.AddCommand("Паспорт...",
                new DelegateCommand(
                    () =>
                    {
                        new ObjectDetailsView {DataContext = new ObjectDetailsViewModel(Id, EntityType.Pipeline)}
                            .ShowDialog();
                    }));

            if (Schema.IsRepair)
            {
               
                menu.AddCommand("Запланировать ремонт...",
                new DelegateCommand(
                    () =>
                    {
                        Schema.InvokeDialogWindowCall(Data.Id);
                    }));
            }           
        }

        public void OnManipulationPointActivated(PipelineEditPointControl manipulationPoint)
        {
            Schema.OnManipulationPointActivated(manipulationPoint);
        }

        public PipelineSegment FindSegment(Point p)
        {
            return PipelinePontsManager.FindSegment(p);
        }

        [Obsolete("Не используется")]
        public double MaxAllowedKm(IPipelinePoint pipelinePoint)
        {
            return PipelinePontsManager.MaxAlloweKm(pipelinePoint);
        }

        public void AddMeasuringLine(IPipelineOmElement mp)
        {
            if (mp.Km > 1340 && mp.Km < 1350)
            {
                var a = false;
            }
            AddItem(new MeasuringLineWidget(this, mp));
        }

        public void AddPlug(double km)
        {
            AddItem(new PlugWidget(this, km));
        }

        public void AddPipelinePoint(IPipelinePoint point, bool raiseGeometryChanged = true)
        {
            PipelinePontsManager.Add(point);
            if (raiseGeometryChanged)
            {
                OnGeometryChanged();
            }
        }

        [Obsolete("Не используется")]
        public double MinAllowedKm(IPipelinePoint pipelinePoint)
        {
            return PipelinePontsManager.MinAllowedKm(pipelinePoint); //?.Km ?? KmBegining) + 0.01;
        }

        public void NotifyPipelineSegmentChanged(IPipelinePoint ptBegining, IPipelinePoint ptEnd, IPipelinePoint point,
            SegmentChangedReason remove)
        {
        }

        public void SegmentChanged(double kmStart, double kmEnd, double angle)
        {
            foreach (var pipelineElementWidget in Items)
            {
                if (pipelineElementWidget.Km >= kmStart && pipelineElementWidget.Km <= kmEnd)
                {
                    pipelineElementWidget.Angle = angle;
                }
            }
        }

        public void MoveAlong(IPipelinePoint pipelinePoint, Vector offset)
        {
            PipelinePontsManager.MoveAlong(pipelinePoint, offset);
        }

        public void ShowHideElementsKm(bool show)
        {
            foreach (var pipelineElementWidget in Items)
            {
                pipelineElementWidget.ShowHideKm(show);
            }
        }

        public void AddReducingStation(IPipelineOmElement rs)
        {
            AddItem(new ReducingStationWidget(this, rs));
        }

        public void AddDistributingStation(IDistrStation ds)
        {
            AddItem(new DistributingStationWidget(this, ds));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _geometryPath = GetTemplateChild("GeometryPath") as Path;

            BindColors();
            _deferredPath = GetTemplateChild("DeferredPath") as Path;
            Canvas = GetTemplateChild("Panel") as Canvas;
            if (Canvas != null)
            {
                _markupsPlaceIndex = Canvas.Children.IndexOf(_geometryPath) + 1;
            }
            AddItemsToCanvas();
            UpdateMarkups();
            Update();
        }
                
        public void AddPipelineConnectionHint(IPipelineConnectionHint pch)
        {
            if (pch.Km > 1340 && pch.Km < 1350)
            {
                var a = false;
            }
            AddItem(new PipelineConnectionHintWidget(this, pch.Km) {DataContext = pch});
        }

        internal void AddValve(IValve valve)
        {
            if (valve.Km > 1340 && valve.Km < 1350)
            {
                var a = false;
            }
            AddItem(new ValveWidget(this, valve));
        }

        internal double GetAngle(IPipelinePoint point)
        {
            if (point.Type == PointType.Intermediate)
            {
                return 0;
            }
            return Support2D.Radians2Degrees(PipelinePontsManager.FindGeometrySegment(point).Angle);
        }

        internal IPipelinePoint FindOrCreateInfraPoint(double km)
        {
            var point = PipelinePontsManager.FindPoint(km);
            if (point == null)
            {
                var pos = PipelinePontsManager.Km2Point(km);
                point = PipelinePontsManager.CreatePoint(Data.AddPoint(km, pos), PointType.Infra);

                PipelinePontsManager.Add(point);
            }
            else
            {
                PipelinePontsManager.MakeInfra(point);
            }
            return point;
            /* var point = PipelinePontsManager.FindPoint(km);
             Point pos;
             if (point == null)
             {
                 pos = PipelinePontsManager.Km2Point(km);
             }
             else
             {
                 pos = point.Position ;
                 if (point.Type== PointType.Intermediate)
                      PipelinePontsManager.RemovePoint( point);
             }
             point = PipelinePontsManager.CreatePoint(Data.AddPoint(km, pos), PointType.Infra);

             PipelinePontsManager.Add(point);
             return point;*/
        }

        internal void UpdateGeometry()
        {
            if (Schema.ServiceLocator.ManipulationPointService.IsManipulating)
            {
                return;
            }

            Position = CoreUtils.GetTopLeftPoint(AllPoints());
            
            PipelinePontsManager.EnsureRectangularLine();
                        
            Geometry = CreatePipelineGeometry(PipelinePontsManager.Start.Position.Substract(Position),
                PipelinePontsManager.End.Position.Substract(Position),
                PipelinePontsManager.GeometryPoints.Select(c => c.Substract(Position)).ToList(), false , true);
            if (_geometryPath != null)
            {
                _geometryPath.Data = Geometry;
            }
            //            FetchZPipelines(false).ForEach(c=>c.Update());
            if (Markups == null)
            {
                return;
            }
            var index = 0;
            foreach (var markup in Markups)
            {
                var markupEl = _markupElements[index];
                index++;
                if (markup.EndKm < KmBegining || markup.StartKm > KmEnd)
                {
                    continue;
                }

                var points = PipelinePontsManager.GetPointsOnKm(markup.StartKm, markup.EndKm).Select(c => c.Position.Substract(Position)).ToList();

                var start = markup.StartKm > KmBegining ?
                   PipelinePontsManager.Km2Point(markup.StartKm).Substract(Position) : PipelinePontsManager.Start.Position.Substract(Position);
                var end = markup.EndKm < KmEnd ?
                   PipelinePontsManager.Km2Point(markup.EndKm).Substract(Position) : PipelinePontsManager.End.Position.Substract(Position);

                markupEl.Data = CreatePipelineGeometry(start, end, points, false, false);
            }
        }
        
        protected void OnGeometryChanged()
        {
            GeometryChanged?.Invoke(this, System.EventArgs.Empty);
        }

        protected override void CreateBindings()
        {
            SetBinding(IsFoundProperty, new Binding(nameof(ISearchable.IsFound)) {Mode = BindingMode.TwoWay});
            SetBinding(MarkupsProperty, new Binding(nameof(IPipeline.Markups)) {Mode = BindingMode.OneWay});
            //            SetBinding(StrokeProperty, new Binding("Color"));
            //            SetBinding(IdProperty, new Binding("Id"));
            //            SetBinding(StrokeThicknessProperty, new Binding("Thickness"));
        }

        protected void Initialize()
        {
            // Мониторинг точек для перерисовки кривых
            //           Data.SegmentChanged += PointsSegmentChanged;

            /*    // Создание точек PipelineEditPointControl
            _points = new List<IPipelineEditPoint>();

            {
                foreach (
                    PipelineEditPointControl pipelinePoint in Data.Points.Select(pt => new PipelineEditPointControl(pt, Schema, this) {Visibility = Visibility.Collapsed}))
                {
                    _points.Add(pipelinePoint);
                    Schema.AddItemToCanvas(pipelinePoint, (int)WidgetZOrder.PipelinePoint);
                }
            }*/
        }

        protected override void OnMouseRightButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonUp(e);

            //todo: вынести  это в SelectionService
            Select();
        }

        protected override void OnPositionChanged(Point position)
        {
            base.OnPositionChanged(position);

            foreach (var item in _items)
            {
                item.UpdatePosition();
            }
        }

        protected void SetToolTip(UIElement element, object tooltip)
        {
            ToolTipService.SetToolTip(element, new ToolTip {Content = tooltip, Style = ToolTipStyle});
        }

        private static void OnMarkupPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            ((PipelineWidget) o).OnMarkupChanged(args.OldValue as ICollection<PipelineMarkup>,
                args.NewValue as ICollection<PipelineMarkup>);
        }

        private static void OnStandardColorsModePropertyChanged(DependencyObject o,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((PipelineWidget) o).BindColors();
        }

        private static void OnEndPointPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = d as PipelineWidget;
            p?.OnEndPointChanged((Point) e.NewValue, (Point) e.OldValue);
        }

        private static void OnStartPointPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var p = d as PipelineWidget;
            p?.OnStartPointChanged((Point) e.NewValue, (Point) e.OldValue);
        }

        private static void OnStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        private void ArrangePipiline(Point? point)
        {
            if (!point.HasValue)
            {
                return;
            }

            PipelinePontsManager.Arrange();
        }

        private void BindColors()
        {
            if (_geometryPath == null)
            {
                return;
            }
            if (StandardColorsMode)
            {
                var expression = _geometryPath.GetBindingExpression(Shape.StrokeProperty);
                if (expression != null)
                {
                    _geometryPath.ClearValue(Shape.StrokeProperty);
                }
                _geometryPath.Stroke = new SolidColorBrush(Colors.Black);
            }
            else
            {
                _geometryPath.SetBinding(Shape.StrokeProperty, new Binding("Stroke")
                {
                    Mode = BindingMode.OneWay,
                    RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
                    Converter = new ColorToBrushConverter()
                });
            }
        }

        private void OnEndPointChanged(Point newValue, Point oldValue)
        {
            if (_init)
            {
                return;
            }
            Position = CoreUtils.GetTopLeftPoint(Points.Select(p => p.Position));
            PipelinePontsManager.End.Position = newValue;
            OnPropertyChanged(nameof(EndPoint));
            // RecalcInfraPoints(oldValue, newValue, PointType.Last );
        }

        private void OnStartPointChanged(Point newValue, Point oldValue)
        {
            if (_init)
            {
                return;
            }
            Position = CoreUtils.GetTopLeftPoint(Points.Select(p => p.Position));
            PipelinePontsManager.Start.Position = newValue;
            OnPropertyChanged(nameof(StartPoint));
        }

        private IList<Point> AllPoints()
        {
            var points = Points.Where(p => p.Type == PointType.Intermediate).Select(p => p.Position).ToList();
            points.Insert(0, StartPoint);
            points.Add(EndPoint);
            return points;
        }

        private void PipelineWidget_LayoutUpdated(object sender, System.EventArgs e)
        {
        }

        private void EndOnPositionChanged(object sender, PositionChangedEventArgs e)
        {
            EndPoint = e.NewPosition;
        }

        private void StartOnPositionChanged(object sender, PositionChangedEventArgs e)
        {
            StartPoint = e.NewPosition;
        }

        private void AddItemsToCanvas()
        {
            if (Canvas == null)
            {
                return;
            }

            foreach (var item in _items)
            {
                Canvas.Children.Add(item);
            }
        }

        /// <summary>
        ///     возвращает все газопроводы лежащие на схеме выше или ниже нашего
        /// </summary>
        /// <param name="below"></param>
        /// <returns></returns>
        private IList<PipelineWidget> FetchZPipelines(bool below)
        {
            var list = new List<PipelineWidget>();
            var pipes = Schema.Pipelines.Where(c => c.Value.Bounds.IntersectsWith(Bounds)).Select(c => c.Value).ToList();
            var zeta = pipes.IndexOf(this);
            if (below)
            {
                for (var k = zeta - 1; k >= 0; k--)
                {
                    list.Add(pipes[k]);
                }
            }
            else
            {
                for (var k = zeta + 1; k < pipes.Count; k++)
                {
                    list.Add(pipes[k]);
                }
            }
            return list;
        }

        /*
        private void AddPoint(IPipelinePoint pipelinePoint)
        {
            
            if (FindPoint(pipelinePoint) == null)
            {
                bool inserted = false;
                for (int i = 0; i < Points.Count; i++)
                {
                    var colPoint = Points[i];
                    if (((IPipelinePoint) pipelinePoint).Km < colPoint.Km)
                    {
                        Points.Insert(i, pipelinePoint);
                        inserted = true;
                        break;
                    }
                }

                if (!inserted)
                    Points.Add(pipelinePoint);
                ((IPipelinePoint) pipelinePoint).PointChanged += OnPointChanged;

               
                var segment = FindSegment(pipelinePoint);

              

                // Выровнять точку относительно соседних точек
                const int sensitivity = 20;
                if (true)
                    if (Math.Abs((double) (segment.BeginingPoint.Position.X - segment.EndPoint.Position.X)) < sensitivity ||
                        Math.Abs((double) (segment.BeginingPoint.Position.Y - segment.EndPoint.Position.Y)) < sensitivity)
                        pipelinePoint.Align(segment.BeginingPoint, sensitivity);

                NotifyPipelineSegmentChanged(segment.BeginingPoint, segment.EndPoint, pipelinePoint, SegmentChangedReason.Add);

            }
        }
*/

/*
        private IPipelinePoint FindPoint(IPipelinePoint pipelinePoint)
        {
            var km = ((IPipelinePoint) pipelinePoint).Km;
            foreach (var pt in Points)
            {
                if (pt.Km == km) return pt;
                if (pt.Km > km) return null;
            }
            return null;
        }
*/

/*
        private void OnPointChanged(IntermediatePipelinePoint point)
        {
            var segment = FindSegment(point);

            NotifyPipelineSegmentChanged(segment.BeginingPoint, segment.EndPoint, point, SegmentChangedReason.Move);
        }
*/

        private void UpdateDeferredGeometry(Geometry newGeometry)
        {
            if (_deferredPath != null)
            {
                _deferredPath.Data = newGeometry;
            }
        }

        private Geometry CreatePipelineGeometry(Point start, Point end, List<Point> points, bool drawBridges, bool split)
        {
            List<PointSegment> gaps = split ?
                PipelinePontsManager.OverlaySegments.Select(seg => seg.Substract(Position)).ToList()
                : new List<PointSegment>();

            var lists = new List<List<Point>>();
            if (gaps.Count > 0)
            {
                var point_lists = PipelinePontsManager.GetPointsSeparateGaps();
                foreach (var list in point_lists)
                    lists.Add(list.Select(c => c.Position.Substract(Position)).ToList());
            }
            PolylineSpecification spec = new PolylineSpecification
            {
                StartPoint = start,
                EndPoint = end,
                Points = points,
                SplitPoints = lists,
                Gaps = gaps,
                DrawBridges = drawBridges,
                Crossings = drawBridges ? GetCrossings() : null,
            };
            return GeometryExtensions.CreateLineGeometry(spec);
        }

        /// <summary>
        ///     Возвращает информации о пересечение данного газопровода с другими газопроводами
        /// </summary>
        private CrossingsData GetCrossings()
        {
            var intersections = new List<Point>();
            var intersection = new Point();
            var points = AllPoints();
            var crossingsData = new CrossingsData(PipelinePontsManager.GeometrySegments.Count);
            const double gapRadius = 5d;

            const double gapRadiusSquared = gapRadius*gapRadius;
            if (Schema == null)
            {
                return crossingsData;
            }

            var pipelines = FetchZPipelines(true);
            if (pipelines.Count == 0)
            {
                for (var k = 0; k < PipelinePontsManager.GeometrySegments.Count; k++)
                {
                    var segment = PipelinePontsManager.GeometrySegments[k];
                    crossingsData.SegmentCrossings[k].Add(segment.Start);
                    crossingsData.SegmentCrossings[k].Add(segment.End);
                }
            }
            else
            {
                for (var segmentIndex = 0; segmentIndex < PipelinePontsManager.GeometrySegments.Count; segmentIndex++)
                {
                    var segment = PipelinePontsManager.GeometrySegments[segmentIndex];
                    var segmentStartPoint = segment.Start;
                    var segmentEndPoint = segment.End;
                    if (segmentStartPoint == segmentEndPoint)
                    {
                        continue;
                    }

                    foreach (var pipeline in pipelines)
                    {
                        if (pipeline.Visibility == Visibility.Visible)
                        {
                            var bounds = pipeline.Bounds;

                            if (bounds.IntersectsWith(Bounds))
                            {
                                for (var k = 0; k < pipeline.PipelinePontsManager.GeometrySegments.Count; k++)
                                {
                                    var otherSegment = pipeline.PipelinePontsManager.GeometrySegments[k];
                                    if (segment.Orientation == otherSegment.Orientation)
                                    {
                                        continue;
                                    }
                                    var otherSegmentStartPoint = otherSegment.Start;
                                    var otherSegmentEndPoint = otherSegment.End;
                                    if (otherSegmentStartPoint == otherSegmentEndPoint)
                                    {
                                        continue;
                                    }

                                    if (CoreUtils.SegmentIntersect(segmentStartPoint, segmentEndPoint,
                                        otherSegmentStartPoint,
                                        otherSegmentEndPoint, ref intersection)
                                        && CoreUtils.DistanceSquared(segmentStartPoint, intersection) > gapRadiusSquared
                                        && CoreUtils.DistanceSquared(segmentEndPoint, intersection) > gapRadiusSquared
                                        &&
                                        CoreUtils.DistanceSquared(otherSegmentStartPoint, intersection) >
                                        gapRadiusSquared
                                        &&
                                        CoreUtils.DistanceSquared(otherSegmentEndPoint, intersection) > gapRadiusSquared
                                        )
                                    {
                                        intersections.Add(intersection);
                                    }
                                }
                            }
                        }
                    }

                    var comparer = new DistanceToPointComparer(segmentStartPoint);
                    intersections.Sort(comparer);

                    var currentSegmentCrossing = crossingsData.SegmentCrossings[segmentIndex];

                    currentSegmentCrossing.Add(segmentStartPoint);
                    foreach (var p in intersections)
                    {
                        var unit = segmentEndPoint.Delta(segmentStartPoint);
                        unit.Normalize();

                        var rho = new Vector(unit.X*gapRadius, unit.Y*gapRadius);

                        //конец дуги
                        currentSegmentCrossing.Add(new Point(p.X - rho.X, p.Y - rho.Y));

                        //начало дуги
                        currentSegmentCrossing.Add(new Point(p.X + rho.X, p.Y + rho.Y));
                    }

                    currentSegmentCrossing.Add(segmentEndPoint);

                    for (var i = 1; i < currentSegmentCrossing.Count - 2; i++)
                    {
                        var p1 = currentSegmentCrossing[i];
                        var p2 = currentSegmentCrossing[i + 1];

                        if (comparer.Compare(p1, p2) > 0 || p1.Distance(p2) < 5d)
                        {
                            currentSegmentCrossing.RemoveAt(i);
                            currentSegmentCrossing.RemoveAt(i);
                            i--;
                        }
                    }
                    intersections.Clear();
                    intersection = new Point();
                }
            }

            var delta = new Vector(Position.X, Position.Y);
            foreach (var crossings in crossingsData.SegmentCrossings)
            {
                for (var index = 0; index < crossings.Count; index++)
                {
                    var p = crossings[index];
                    crossings[index] = new Point(p.X - delta.X, p.Y - delta.Y);
                }
            }
            return crossingsData;
        }

        private void AddItem(PipelineElementWidget pipelineElementWidget)
        {
            _items.Add(pipelineElementWidget);

            Canvas?.Children.Add(pipelineElementWidget);
        }

        public void DeleteItem(PipelineElementWidget pipelineElementWidget)
        {
            _items.Remove(pipelineElementWidget);

            Canvas?.Children.Remove(pipelineElementWidget);
        }

        private void OnMarkupChanged(ICollection<PipelineMarkup> oldValue, ICollection<PipelineMarkup> newValue)
        {
            var oldCollection = oldValue as INotifyCollectionChanged;
            if (oldCollection != null)
            {
                oldCollection.CollectionChanged -= OnCollectionChanged;
            }
            UpdateMarkups();
            var newCollection = newValue as INotifyCollectionChanged;
            if (newCollection != null)
            {
                newCollection.CollectionChanged += OnCollectionChanged;
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            UpdateMarkups();
        }

        private void UpdateMarkups()
        {
            if (Canvas == null)
            {
                return;
            }
            foreach (var markupElement in _markupElements)
            {
                Canvas.Children.Remove(markupElement);
            }
            _markupElements.Clear();
            if (Markups == null)
            {
                return;
            }
            foreach (var markup in Markups)
            {
                var path = new Path {Stroke = new SolidColorBrush(markup.Color), StrokeThickness = StrokeThickness};
                SetToolTip(path, markup.Tooltip);
                _markupElements.Add(path);
                Canvas.Children.Insert(_markupsPlaceIndex, path);                
            }
            UpdateGeometry();
        }
    }
}