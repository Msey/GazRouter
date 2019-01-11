using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GazRouter.Common;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.Flobus.Dialogs;
using GazRouter.Flobus.Extensions;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.Misc;
using GazRouter.Flobus.Primitives;
using GazRouter.Flobus.Tools;
using GazRouter.Flobus.UiEntities.FloModel;
using GazRouter.Flobus.Utilites;
using GazRouter.Flobus.Visuals;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using NLog;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Diagrams.Core;
using Telerik.Windows.Media.Imaging;
using ICommand = System.Windows.Input.ICommand;
using PositionChangedEventArgs = GazRouter.Flobus.EventArgs.PositionChangedEventArgs;
using PropertyMetadata = System.Windows.PropertyMetadata;
using ServiceLocator = GazRouter.Flobus.Services.ServiceLocator;
using VirtualizationService = GazRouter.Flobus.Services.VirtualizationService;

namespace GazRouter.Flobus
{
    [TemplatePart(Name = PartItemsHost, Type = typeof(SchemaSurface))]
    [TemplatePart(Name = PartSelectionRectangle, Type = typeof(Rectangle))]
    [TemplatePart(Name = PartMainPanel, Type = typeof(Panel))]
    [TemplatePart(Name = PartContextMenu, Type = typeof(RadContextMenu))]
    [TemplatePart(Name = PartItemInformationAdorner, Type = typeof(ItemInformationAdorner))]
    public partial class Schema : Control, ISupportContextMenu
    {
        public static readonly DependencyProperty GoToTreeCommandProperty =
            DependencyProperty.Register("GotoTreeCommand", typeof(ICommand), typeof(Schema),
                new PropertyMetadata(null));

        public static readonly DependencyProperty GoToTrendCommandProperty =
            DependencyProperty.Register(
                "GoToTrendCommand",
                typeof(DelegateCommand<GoToTrendCommandParameter>),
                typeof(Schema),
                new PropertyMetadata(null));

        public static readonly DependencyProperty SchemaSourceProperty = DependencyProperty.Register(
            nameof(SchemaSource),
            typeof(ISchemaSource),
            typeof(Schema),
            new PropertyMetadata(null, OnSchemaSourcePropertyChanged));

        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
            nameof(Zoom),
            typeof(double),
            typeof(Schema),
            new FrameworkPropertyMetadata(1d, OnZoomPropertyChanged));

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedObject",
            typeof(ISearchable),
            typeof(Schema),
            new PropertyMetadata(null, (o, args) => ((Schema) o).OnSelectedObjectChanged()));

        public static readonly DependencyProperty StandardColorsModeProperty =
            DependencyProperty.Register(
                nameof(StandardColorsMode),
                typeof(bool),
                typeof(Schema),
                new PropertyMetadata(false, OnStandardColorsModePropertyChanged));
                
        public static readonly DependencyProperty VisibleLayersProperty = DependencyProperty.Register(
            nameof(VisibleLayers),
            typeof(SchemeLayers),
            typeof(Schema),
            new PropertyMetadata(SchemeLayers.All, OnVisibleLayersChanged));

        public static readonly DependencyProperty MousePositionProperty = DependencyProperty.Register(
            "MousePosition", typeof(Point), typeof(Schema), new PropertyMetadata(default(Point)));

        public static event EventHandler DialogWindowCall;
        public static void InvokeDialogWindowCall(object sender)
        {
            if (DialogWindowCall != null)
            {
                try { DialogWindowCall.Invoke(sender, null); } catch { }
            }
        }
        private static void CleanUpHandlers()
        {
            try
            {
                var dds = DialogWindowCall.GetInvocationList();
                foreach (var d in dds)
                    DialogWindowCall -= (EventHandler)d;
            }
            catch { }
        }

        private const string PartItemsHost = "ItemsHost";
        private const string PartSelectionRectangle = "SelectionRectangle";
        private const string PartContextMenu = "ContextMenu";
        private const string PartMainPanel = "MainPanel";
        private const string PartItemInformationAdorner = "ItemInformationAdorner";
        private const string PartBackgroundGrid = "BackgroundGrid";

        private readonly DiagramTransformationService _transformationService;
        private ItemInformationAdorner _itemInformationAdorner;

        private bool _supressSaveparams;

        private RadContextMenu _contextMenu;

        private TextBlock _schemeInfo;

        private SchemaSurface _itemsHost;
        private Rectangle _rectangleSelectionVisual;
        private Panel _mainPanel;

        private bool _suppressVisualChildrenChanged;
        private bool _isDiagramClicked;
        private Point _lastMouseDownPoint;
        private Point _lastMouseMovePoint;

        private PipelineManipulationAdorner _pipelineManipulationAdorner;
        private RadBusyIndicator _busyIndicator;

        static Schema()
        {
            DiagramConstants.MaximumZoom = 2;
            RegisterCommands();
        }

        /// <summary>
        ///     Конструктор
        /// </summary>
        public Schema()
        {
            DefaultStyleKey = typeof(Schema);

            _transformationService = new DiagramTransformationService();

//            _controller = new GraphController(this, new GenericContainerGenerator<RadDiagramItem>(this));
            ServiceLocator = new ServiceLocator(this);
            ServiceLocator.SelectionService.SelectionChanged += OnSelectionServiceSelectionChanged;
//            var toolList = ((ToolService) _toolService).ToolList;

            /*         toolList.Clear();
            toolList.Add(new PointerTool());
            toolList.Add(new PanningTool());*/
            SizeChanged += OnSizeChanged;
            ServiceLocator.DraggingService.Dragging += OnDraggingServiceOnDragging;
            ServiceLocator.DraggingService.StartDragging += OnDraggingServiceStartDragging;
            ServiceLocator.DraggingService.CompleteDragging += OnDraggingServiceCompleteDragging;

            ServiceLocator.ResizingService.Resizing += OnResizingServiceResizing;
//            MouseRightButtonUp += RectMouseRightButtonUp;
            InitCommands();

            Loaded += OnLoaded;
        }
        
        public bool IsServiceManipulating => ServiceLocator.DraggingService.IsDragging ||
                                             ServiceLocator.ManipulationPointService.IsManipulating;

        public Point MousePosition
        {
            get { return (Point) GetValue(MousePositionProperty); }
            set { SetValue(MousePositionProperty, value); }
        }

        public int Snap { get; } = 10;

        public DelegateCommand<GoToTrendCommandParameter> GoToTrendCommand
        {
            get { return (DelegateCommand<GoToTrendCommandParameter>) GetValue(GoToTrendCommandProperty); }
            set { SetValue(GoToTrendCommandProperty, value); }
        }

        public ICommand GotoTreeCommand
        {
            get { return (ICommand) GetValue(GoToTreeCommandProperty); }
            set { SetValue(GoToTreeCommandProperty, value); }
        }

        public Action<CommonEntityDTO, PropertyType, DateTime?> GotoTrend { get; set; }

        public DateTime? KeyDate { get; set; }

        /// <summary>
        ///     Отображаемая модель
        /// </summary>
        public ISchemaSource SchemaSource
        {
            get { return (ISchemaSource) GetValue(SchemaSourceProperty); }

            set { SetValue(SchemaSourceProperty, value); }
        }

        public int SchemeId { get; set; }

        public Dictionary<Guid, PipelineWidget> Pipelines { get; } = new Dictionary<Guid, PipelineWidget>();

        /// <summary>
        ///     Масштаб отображения схемы
        /// </summary>
        public double Zoom
        {
            get { return (double) GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        public ISearchable SelectedItem
        {
            get { return (ISearchable) GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public bool StandardColorsMode
        {
            get { return (bool) GetValue(StandardColorsModeProperty); }
            set { SetValue(StandardColorsModeProperty, value); }
        }
        
        public SchemeLayers VisibleLayers
        {
            get { return (SchemeLayers) GetValue(VisibleLayersProperty); }
            set { SetValue(VisibleLayersProperty, value); }
        }

        public ISchemaItem SelectedWidget { get; private set; }

        public double ItemsSurfaceActualWidth => _mainPanel?.ActualWidth ?? 0;

        public double ItemsSurfaceActualHeight => _mainPanel?.ActualHeight ?? 0;

        public Point ItemsSurfaceCenter => new Point(ItemsSurfaceActualWidth/2, ItemsSurfaceActualHeight/2);

        public bool IsLoaded { get; private set; }

        internal BackgroundGrid BackgroundGrid { get; private set; }
        internal WidgetBase FoundItem { get; set; }
        protected internal ServiceLocator ServiceLocator { get; }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            RemoveHandler(KeyDownEvent, new KeyEventHandler(OnKeyDownHandled));
            AddHandler(KeyDownEvent, new KeyEventHandler(OnKeyDownHandled), true);

            RemoveHandler(KeyUpEvent, new KeyEventHandler(OnKeyUpHandled));
            AddHandler(KeyUpEvent, new KeyEventHandler(OnKeyUpHandled), true);

            _itemsHost?.Children.Clear();

            _itemsHost = GetTemplateChild(PartItemsHost) as SchemaSurface;
            _rectangleSelectionVisual = GetTemplateChild(PartSelectionRectangle) as Rectangle;

            ResolveAdorners();

            BackgroundGrid = GetTemplateChild(PartBackgroundGrid) as BackgroundGrid;
            if (BackgroundGrid != null)
            {
                BackgroundGrid.Schema = this;
            }

            _schemeInfo = GetTemplateChild("SchemaInfo") as TextBlock;

            _busyIndicator = GetTemplateChild("BusyIndicator") as RadBusyIndicator;

            _contextMenu = GetTemplateChild(PartContextMenu) as RadContextMenu;
            if (_contextMenu != null)
            {
                _contextMenu.Opening += _contextMenu_Opening;
            }

            var backgroundGrid = GetTemplateChild("BackgroundGrid") as BackgroundGrid;
            if (backgroundGrid != null)
            {
                backgroundGrid.Schema = this;
            }
            _mainPanel = (Panel) GetTemplateChild(PartMainPanel);
            _mainPanel.SizeChanged += (sender, args) => UpdateViewport();

            InitializePanAndZoomTransformation();
            
            AddItemsToCanvas();

            //            CirclePointer = new CirclePointerAdorner(this);

            if (SchemaSource != null)
            {
                FillModel(SchemaSource);
            }
        }

        public Rect CalculateEnclosingBounds()
        {
            var enclosingBounds = Rect.Empty;
            foreach (var widget in Items.Where(w => w.VirtualizationState != ItemVisibility.Collapsed))
            {
                var bounds = widget.Bounds;
                if (bounds.X == 0 && bounds.Y == 0 && bounds.Width == 0 && bounds.Height == 0)
                {
                    continue;
                }

                enclosingBounds.Union(bounds);
            }

            return enclosingBounds;
        }

        public WriteableBitmap CreateDiagramImage(Rect enclosingBounds, Size returnImageSize)
        {
            VirtualizationService.ForceRealization(Items);
            UpdateLayout();

            var image = BitmapUtils.CreateWriteableBitmap(_itemsHost, enclosingBounds, returnImageSize, Background,
                new Thickness());

            VirtualizationService.UpdateVirtualization(Items, Viewport);
            UpdateLayout();

            return image;
        }

        public void CreatePipelineWidget([NotNull] IPipeline pipeLine)
        {
            var pw = new PipelineWidget(pipeLine, this) {StandardColorsMode = StandardColorsMode};

            Items.Add(pw);
            Pipelines.Add(pw.Id, pw);

            AddItemToCanvas(pw);

            foreach (var valve in pipeLine.Valves)
            {
                pw.AddValve(valve);
            }

            foreach (var ds in pipeLine.DistributingStations)
            {
                if (!SchemaSource.IsDistributionStationHidden(ds.Id))
                    pw.AddDistributingStation(ds);
            }

            foreach (var mp in pipeLine.MeasuringLines)
            {
                if (!SchemaSource.IsMeasuringLineHidden(mp.Id))
                    pw.AddMeasuringLine(mp);
            }

            foreach (var rs in pipeLine.ReducingStations)
            {
                if (!SchemaSource.IsReducingStationHidden(rs.Id))
                    pw.AddReducingStation(rs);
            }
            /*  
       * все вернуть при первой возможности
       *


         // Маркеры для газопровода
         foreach (var pm in pipeLine.PipelineMarkers)
         {
             _widgetList.Add(new PipelineMarkerWidget(pm, this));
         }*/
        }

        public void Export()
        {
            var dialog = new SaveFileDialog {DefaultExt = "png", Filter = "Файл PNG (*.png) | *.png"};
            if (!(dialog.ShowDialog() ?? false))
            {
                return;
            }
            using (var fileStream = dialog.OpenFile())
            {
                var bitmap = new WriteableBitmap(_itemsHost, null);
                bitmap.Invalidate();
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(fileStream);
            }
        }

        public void FillMenu(RadContextMenu menu, Point mousePosition, Schema schema)
        {
            if (SchemaSource == null)
            {
                return;
            }
            if (IsReadOnly)
            {
                return;
            }

            menu.AddCommand("Добавить газопровод...", new DelegateCommand<Point?>(
                AddPipelineFromMenu,
                position =>
                {
                    return SchemaSource.Dto.PipelineList.Any(dto => SchemaSource.Pipelines.All(c => c.Id != dto.Id));
                }),
                mousePosition);
            menu.AddCommand("Добавить КЦ...", new DelegateCommand<Point?>(
                AddCompShopFromMenu,
                eventArg =>
                    SchemaSource.Dto.CompShopList.Any(dto => SchemaSource.CompressorShops.All(c => c.Id != dto.Id))),
                mousePosition);

            menu.AddCommand("Добавить линейный объект...", new DelegateCommand<Point?>(AddPolyLineFromMenu, p => p.HasValue),
                mousePosition);

            menu.AddCommand("Добавить обратный клапан...", new DelegateCommand<Point?>(AddCheckValveFromMenu, p => p.HasValue),
                mousePosition);

            menu.AddCommand("Добавить текст...", new DelegateCommand<Point?>(AddTextBlockFromMenu, p => p.HasValue),
                mousePosition);

            Dictionary<IPipelineOmElement, string> hiddenMeasuringLines = schema.SchemaSource.GetHiddenMeasuringLines(Guid.Empty);
            if (hiddenMeasuringLines != null && hiddenMeasuringLines.Count > 0)
            {
                RadMenuItem restoreML = new RadMenuItem();
                restoreML.Header = "Восстановить замерные линии";
                foreach (var key in hiddenMeasuringLines.Keys)
                {
                    RadMenuItem mp = new RadMenuItem();
                    mp.Header = hiddenMeasuringLines[key];
                    mp.Command = new Microsoft.Practices.Prism.Commands.DelegateCommand(() =>
                    {
                        Guid pipeId = schema.SchemaSource.RestoreMeasuringLine(key.Id);
                        if (pipeId != Guid.Empty)
                        {
                            Pipelines[pipeId].AddMeasuringLine(key);
                        }
                    });
                    restoreML.Items.Add(mp);
                }
                menu.Items.Add(restoreML);
            }

            Dictionary<IDistrStation, string> hiddenDistributionStations = schema.SchemaSource.GetHiddenDistributionStations(Guid.Empty);
            if (hiddenDistributionStations != null && hiddenDistributionStations.Count > 0)
            {
                RadMenuItem restoreML = new RadMenuItem();
                restoreML.Header = "Восстановить ГРС";
                foreach (var key in hiddenDistributionStations.Keys)
                {
                    RadMenuItem mp = new RadMenuItem();
                    mp.Header = hiddenDistributionStations[key];
                    mp.Command = new Microsoft.Practices.Prism.Commands.DelegateCommand(() =>
                    {
                        Guid pipeId = schema.SchemaSource.RestoreDistributionStation(key.Id);
                        if (pipeId != Guid.Empty)
                        {
                            Pipelines[pipeId].AddDistributingStation(key);
                        }
                    });
                    restoreML.Items.Add(mp);
                }
                menu.Items.Add(restoreML);
            }

            Dictionary<IPipelineOmElement, string> hiddenReducingStations = schema.SchemaSource.GetHiddenReducingStations(Guid.Empty);
            if (hiddenReducingStations != null && hiddenReducingStations.Count > 0)
            {
                RadMenuItem restoreML = new RadMenuItem();
                restoreML.Header = "Восстановить ПРГ";
                foreach (var key in hiddenReducingStations.Keys)
                {
                    RadMenuItem mp = new RadMenuItem();
                    mp.Header = hiddenReducingStations[key];
                    mp.Command = new Microsoft.Practices.Prism.Commands.DelegateCommand(() =>
                    {
                        Guid pipeId = schema.SchemaSource.RestoreReducingStation(key.Id);
                        if (pipeId != Guid.Empty)
                        {
                            Pipelines[pipeId].AddReducingStation(key);
                        }
                    });
                    restoreML.Items.Add(mp);
                }
                menu.Items.Add(restoreML);
            }
        }

        public void OnManipulationPointActivated(PipelineEditPointControl manipulationPoint)
        {
            if (IsReadOnly)
            {
                return;
            }
            var toolService = ServiceLocator.ToolService;
            var tool = toolService.FindTool(PipelineManipulationTool.ToolName) as PipelineManipulationTool;
            if (tool != null)
            {
                tool.ActiveManipulationPoint = manipulationPoint;
                toolService.ActivateTool(PipelineManipulationTool.ToolName);
            }
        }
        
        public Point PointToCanvas(UIElement uiElement, Point point)
        {
            return uiElement.TransformToVisual(_itemsHost).Transform(point);
        }

        public void RemoveCompressorShop(CompressorShopWidget widget)
        {
            Items.Remove(widget);
            SchemaSource.RemoveCompressorShops(widget.Id);
            RemoveItemFromCanvas(widget);
        }

        public void RemoveTextWidget(TextWidget textWidget)
        {
            Items.Remove(textWidget);
            if (textWidget.IsSelected)
            {
                SelectionService.ClearSelection();
            }
            SchemaSource?.RemoveTextBlock(textWidget.DataContext as ITextBlock);
            RemoveItemFromCanvas(textWidget);
        }

        public void RemovePolyLineWidget(PolyLineWidget lineWidget)
        {
            Items.Remove(lineWidget);
            if (lineWidget.IsSelected)
            {
                SelectionService.ClearSelection();
            }
            SchemaSource?.RemovePolyLine(lineWidget.DataContext as IPolyLine);
            RemoveItemFromCanvas(lineWidget);
        }

        public void RemoveTextWidget(CheckValveWidget checkvalveWidget)
        {
            Items.Remove(checkvalveWidget);
            if (checkvalveWidget.IsSelected)
            {
                SelectionService.ClearSelection();
            }
            SchemaSource?.RemoveCheckValve(checkvalveWidget.DataContext as ICheckValve);
            RemoveItemFromCanvas(checkvalveWidget);
        }

        public void RemovePipelineWidget(PipelineWidget pipelineWidget)
        {
            Items.Remove(pipelineWidget);
            Pipelines.Remove(pipelineWidget.Id);
            if (pipelineWidget.IsSelected)
            {
                SelectionService.ClearSelection();
            }
            SchemaSource?.RemovePipeline(pipelineWidget.Data);
            RemoveItemFromCanvas(pipelineWidget);

            // pw
            foreach (var vlv in pipelineWidget.Data.Valves)
            {
                RemovePipelineElement(vlv);
            }

            // ГРС
            foreach (var ds in pipelineWidget.Data.DistributingStations)
            {
                RemovePipelineElement(ds);
            }

            // Замерные линии
            foreach (var mp in pipelineWidget.Data.MeasuringLines)
            {
                RemovePipelineElement(mp);
            }

            // Подсказки по соединениям газопроводов
            foreach (var pchw in pipelineWidget.Data.PipelineConnections)
            {
                RemovePipelineElement(pchw);

                //                widget.Destroy();
            }
            // Обновление подсказок других газопроводов
            foreach (var pipeline in Pipelines.Values)
            {
                foreach (var connectionHint in pipeline.Items.OfType<PipelineConnectionHintWidget>())
                {
                    if (((IPipelineConnectionHint) connectionHint.DataContext).DestinationPipileneId ==
                        pipelineWidget.Id)
                    {
//                        pipeline.Items.Re
                    }
                }
            }

            // ПРГ
            foreach (var rs in pipelineWidget.Data.ReducingStations)
            {
                RemovePipelineElement(rs);
            }
/*
                   // Маркеры для газопровода
                   foreach (var pm in SchemaSource.PipelineMarkers.Where(c => c.Section.Pipe.Id == pw.Data.Id))
                   {
                       var widget = _widgetList.Single(w => w.GetData() == pm);
                       _widgetList.Remove(widget);

                   }*/
        }

        /// <summary>
        ///     Сохранение выбранного виджета
        /// </summary>
        /// <param name="selection">Выбранный виджет</param>
        public void SetSelectedWidget(ISchemaItem selection)
        {
            if (SelectedWidget != null && SelectedWidget != selection)
            {
                SelectedWidget.IsSelected = false;
            }
            SelectedWidget = selection;
        }

        internal void AddItemToCanvas(UIElement element, int zindex = 0)
        {
            if (_itemsHost != null && !_itemsHost.Children.Contains(element))
            {
                if (zindex != 0)
                {
                    element.SetValue(Canvas.ZIndexProperty, zindex);
                }
                _itemsHost.Children.Add(element);

                if (!_suppressVisualChildrenChanged)
                {
                    OnVisualChildrenChanged();
                }
            }
        }

        internal Brush GetCanvasBackground()
        {
            return _itemsHost.Background;
        }

        internal Point GetPositonOnCanvas(MouseEventArgs args)
        {
            return args.GetPosition(_itemsHost);
        }

        internal void RemoveItemFromCanvas(UIElement element)
        {
            if (_itemsHost != null && _itemsHost.Children.Contains(element))
            {
                _itemsHost.Children.Remove(element);
                if (!_suppressVisualChildrenChanged)
                {
                    OnVisualChildrenChanged();
                }
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (FoundItem != null)
            {
                FoundItem.IsFound = false;
                FoundItem = null;
            }

            _isDiagramClicked = true;

            var position = _lastMouseDownPoint = e.GetPosition(this);
            var transformedPosition = _transformationService.TranformToOriginal(position);

            ServiceLocator.ToolService.MouseDown(new PointerArgs(position, transformedPosition));
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            var position = e.GetPosition(this);

            //            ((IMouseListener)_toolService.FindTool(PanningTool.ToolName)).MouseUp(new PointerArgs(position, _transformationService.TranformToOriginal(position)));

            ServiceLocator.ToolService.MouseUp(new PointerArgs(position,
                _transformationService.TranformToOriginal(position)));
            UpdateFocusedElement();
            /* ReleaseMouseCapture();
            _isMouseDown = false;*/
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var position = e.GetPosition(this);

            if (position == _lastMouseDownPoint || position == _lastMouseMovePoint)
            {
                return;
            }

            _lastMouseMovePoint = position;

            var transformedPosition = _transformationService.TranformToOriginal(position);
            MousePosition = new Point((int) transformedPosition.X, (int) transformedPosition.Y);
            ServiceLocator.ToolService.MouseMove(new PointerArgs(position, transformedPosition));
        }

        private static void OnCanDeleteCommandExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var scheme = sender as Schema;

            if (scheme != null)
            {
                e.CanExecute = scheme.SelectedWidget != null;
            }
        }

        private static void OnDeleteCommandExecuted(object sender, ExecutedRoutedEventArgs ea)
        {
            var scheme = sender as Schema;

            if (scheme == null)
            {
                return;
            }

            RadWindow.Confirm(new DialogParameters
            {
                Header = "Внимание!",
                Content = @"Вы уверены, что хотите удалить объект со схемы?",
                Closed = (s, e) =>
                {
                    if (e.DialogResult.HasValue && e.DialogResult.Value)
                    {
                        scheme.RemoveCompressorShop((CompressorShopWidget) scheme.SelectedWidget);
                    }
                }
            });
        }

        private static void OnSchemaSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var scheme = (Schema) d;
            scheme.FillModel(e.NewValue as ISchemaSource);
        }

        private static void OnZoomPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var schema = (Schema) d;
            schema?.OnZoomChanged(Convert.ToDouble(e.NewValue));
        }

        private static void OnStandardColorsModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Schema) d).OnStandardColorsModeChanged();
        }
        
        private static void OnVisibleLayersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var schema = (Schema) d;
            schema?.UpdateVisibility();
        }

        private void OnStandardColorsModeChanged()
        {
            foreach (var pipeline in Pipelines.Values)
            {
                pipeline.StandardColorsMode = StandardColorsMode;
            }
        }

        // TODO: Добавить сюда функции выделения группы элементов и перетаскивания их

        private void RemoveWidgetFromCanvas(IWidget widget)
        {
            var element = widget as UIElement;
            if (element != null)
            {
                RemoveItemFromCanvas(element);
            }
        }

        private void OnDraggingServiceCompleteDragging(object sender, PositionChangedEventArgs e)
        {
            PublishDiagramEvent(DiagramEvent.Drag, e);
        }

        private void OnDraggingServiceStartDragging(object sender, CancelingPositionChangedEventArgs e)
        {
            PublishDiagramEvent(DiagramEvent.Dragging, e);
/*            if (!e.Cancel && !IsServiceManipulating)
            {
                ServiceLocator.SegmentationService.MapShapes(Shapes);
            }*/
        }

        private void OnDraggingServiceOnDragging(object sender, PositionChangedEventArgs e)
        {
            ServiceLocator.AdornerService.UpdateAdornerBounds(e.NewPosition.Substract(e.OldPosition));
            PublishDiagramEvent(DiagramEvent.DragDelta, e);
        }

        private void OnSelectionServiceSelectionChanged(object sender, DiagramSelectionChangedEventArgs e)
        {
            RefreshManipulationAdorner();
        }

        private void RefreshManipulationAdorner()
        {
            ServiceLocator.AdornerService.UpdateAdornerBounds(ServiceLocator.SelectionService.GetSelectionBounds());
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            IsLoaded = true;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs sizeChangedEventArgs)
        {
            Clip = new RectangleGeometry {Rect = new Rect(0, 0, ActualWidth, ActualHeight)};
        }

        private void OnKeyDownHandled(object sender, KeyEventArgs e)
        {
            if (ShowValveKmHotkey(e))
            {
                ShowHidePipelineElementsKm(true);
                e.Handled = true;
            }
        }

        private void OnResizingServiceResizing(object sender, ResizingEventArgs e)
        {
            ServiceLocator.AdornerService.UpdateAdornerBounds(e.NewBounds);
            PublishDiagramEvent(DiagramEvent.ResizeDelta, e);
        }

        private void OnKeyUpHandled(object sender, KeyEventArgs e)
        {
            if (ShowValveKmHotkey(e))
            {
                ShowHidePipelineElementsKm(false);
                e.Handled = true;
            }
        }

        private void UpdateFocusedElement()
        {
            if (_isDiagramClicked)
            {
                Focus();
                _isDiagramClicked = false;
            }
        }

        private bool ShowValveKmHotkey(KeyEventArgs e)
        {
            return e.Key == Key.K && Keyboard.Modifiers == ModifierKeys.None;
        }

        private void ShowHidePipelineElementsKm(bool show)
        {
            foreach (var pipelineWidget in Pipelines.Values)
            {
                pipelineWidget.ShowHideElementsKm(show);
            }
        }

        private void UpdatePipelineManipulationAdorner(Rect adornerBounds)
        {
            if (_pipelineManipulationAdorner == null)
            {
                return;
            }

            var isManipulationAdornerVisible = SelectionService.SelectedItems.All(d => d.IsManipulationAdornerVisible);
            var manipulating = !adornerBounds.IsEmpty && adornerBounds != new Rect() && isManipulationAdornerVisible;

            _pipelineManipulationAdorner.Update(SelectionService.SelectedItems, manipulating);
        }

        private void UpdateInformationTip(object tip)
        {
            if (_itemInformationAdorner != null)
            {
                _itemInformationAdorner.InformationTip = tip;
            }
        }

        private void UpdateItemInformationAdornerInformationTipVisibility(bool isVisible)
        {
            if (_itemInformationAdorner != null)
            {
                _itemInformationAdorner.IsInformationTipVisible = isVisible;
            }
        }

        private void UpdateItemInformationAdornerPosition()
        {
            if (_itemInformationAdorner == null)
            {
                return;
            }
            var rawSelectionBounds = SelectionService.GetSelectionBounds();
            var transformedBounds = _transformationService.TranformToCurrent(rawSelectionBounds);

            var selectionBounds = transformedBounds == Rect.Empty ? new Rect(0, 0, 0, 0) : transformedBounds;
            var adornerX = Math.Floor(selectionBounds.X);
            var adornerY = Math.Floor(selectionBounds.Y);
            var adornerWidth = Math.Ceiling(selectionBounds.Width + (selectionBounds.X - adornerX));
            var adornerHeight = Math.Ceiling(selectionBounds.Height + (selectionBounds.Y - adornerY));

            _itemInformationAdorner.Width = adornerWidth;

            _itemInformationAdorner.Height = adornerHeight;
            _itemInformationAdorner.SetLocation(adornerX, adornerY);
        }

        private void SaveSchemeParams()
        {
            if (_supressSaveparams)
            {
                return;
            }
            if (SchemeId == 0)
            {
                return;
            }
            var s = new SchemaParams
            {
                Zoom = Zoom,
                Position = Position
            };
            IsolatedStorageManager.SaveSchemaParams(SchemeId, s);
            LogManager.GetCurrentClassLogger().Debug($"Save schema params: zoom= {s.Zoom}, position= {s.Position} ");
        }

        private void AddItemsToCanvas()
        {
            if (_itemsHost == null)
            {
                return;
            }

            _itemsHost.Children.Clear();

            foreach (var item in Items.ToList())
            {
                AddItemToCanvas(item);
            }
        }

        private void AddTextBlockFromMenu(Point? point)
        {
            var textBlock = SchemaSource.AddTextBlock("Текст", point.Value);
            CreateTextBoxWidget(textBlock);
        }
        private void AddPolyLineFromMenu(Point? point)
        {
            var polyline = SchemaSource.AddPolyLine(point.Value);
            CreatePolyLineWidget(polyline);
        }
        private void AddCheckValveFromMenu(Point? point)
        {
            var check_valve = SchemaSource.AddCheckValve(point.Value);
            CreateCheckValveWidget(check_valve);
        }
        private void AddCompShopFromMenu(Point? arg)
        {
            Debug.Assert(arg != null, "arg != null");
            var position = arg.Value;

            var vm = new CompShopAddDialogViewModel(dto =>
            {
                var cs = SchemaSource.AddCompressorShops(dto, position);
                CreateCompressorShopWidget(cs);
            })
            {
                CompShopList =
                    SchemaSource.Dto.CompShopList.Where(dto => SchemaSource.CompressorShops.All(c => c.Id != dto.Id))
                        .ToList()
            };
            var dlg1 = new CompShopAddDialog {DataContext = vm};
            dlg1.ShowDialog();
        }

        private void AddPipelineFromMenu(Point? arg)
        {
            Debug.Assert(arg != null, "arg != null");
            var position = arg.Value;
            var dlg = new PipelineAddDialog();

            var pipelineAddDialogViewModel = new PipelineAddDialogViewModel(dto =>
            {
                if (SchemaSource == null)
                {
                    return;
                }

                var pipeline = SchemaSource.AddPipeline(dto, position);

                CreatePipelineWidget(pipeline);
            })
            {
                PipelineList =
                    SchemaSource.Dto.PipelineList.Where(dto => SchemaSource.Pipelines.All(c => c.Id != dto.Id))
                        .OrderBy(p => p.Type)
                        .ThenBy(p => p.Name)
                        .ToList()
            };
            dlg.DataContext = pipelineAddDialogViewModel;
            dlg.ShowDialog();
        }

        private void ResolveAdorners()
        {
            _manipulationAdorner = GetTemplateChild("ManipulationAdorner") as ManipulationAdorner;
            if (_manipulationAdorner != null)
            {
                _manipulationAdorner.Schema = this;
                _manipulationAdorner.ApplyTemplate();
            }

            _pipelineManipulationAdorner =
                GetTemplateChild("PipelineManipulationAdorner") as PipelineManipulationAdorner;
            if (_pipelineManipulationAdorner != null)
            {
                _pipelineManipulationAdorner.Schema = this;
            }

            if (_itemInformationAdorner != null)
            {
                _itemInformationAdorner.IsAdditionalContentVisibleChanged -=
                    ItemInformationAdornerOnIsAdditionalContentVisibleChanged;
                _itemInformationAdorner.Schema = null;
            }
            _itemInformationAdorner = GetTemplateChild(PartItemInformationAdorner) as ItemInformationAdorner;
            if (_itemInformationAdorner != null)
            {
                _itemInformationAdorner.Schema = this;
                _itemInformationAdorner.IsAdditionalContentVisibleChanged +=
                    ItemInformationAdornerOnIsAdditionalContentVisibleChanged;
            }
        }

        private void ItemInformationAdornerOnIsAdditionalContentVisibleChanged(object sender, System.EventArgs eventArgs)
        {
            IsAdditionalContentVisible = _itemInformationAdorner.IsAdditionalContentVisible;
        }

        private void _contextMenu_Opening(object sender, RadRoutedEventArgs e)
        {
            InitContextMenu(e);
        }

        private void InitializePanAndZoomTransformation()
        {
            _transformationService.ApplyTransform(_mainPanel, _manipulationAdorner);

            _suppressViewportUpdate = true;
            PanInternal(Position);
            _suppressViewportUpdate = false;
            ZoomInternal(Zoom, new Point(0, 0), ZoomType.Absolute);
            _mainPanel.RenderTransformOrigin = new Point();
        }

        private void UpdateViewport()
        {
            if (_mainPanel == null)
            {
                return;
            }

            var screenAvailable = new Rect(0, 0, _mainPanel.ActualWidth, _mainPanel.ActualHeight);
            var viewportInOriginalCoordinates = _transformationService.TranformToOriginal(screenAvailable);

            Viewport = viewportInOriginalCoordinates;
            VirtualizationService.UpdateVirtualization(Items, viewportInOriginalCoordinates);
        }

        private void CreateCompressorShopWidget(ICompressorShop cs)
        {
            var compressorShopShape = new CompressorShopWidget(this) {DataContext = cs};

            Items.Add(compressorShopShape);
         
            AddItemToCanvas(compressorShopShape, (int) WidgetZOrder.CompressorShop);
        }

        private void CreateTextBoxWidget(ITextBlock tb)
        {
            var widget = new TextWidget(this) {DataContext = tb};

            Items.Add(widget);

            AddItemToCanvas(widget, (int) WidgetZOrder.CompressorShop);
        }

        private void CreatePolyLineWidget(IPolyLine pl)
        {
            var widget = new PolyLineWidget(pl,this) { DataContext = pl };
            Items.Add(widget);

            AddItemToCanvas(widget, (int)WidgetZOrder.CompressorShop);
        }
        private void CreateCheckValveWidget(ICheckValve pl)
        {
            var widget = new CheckValveWidget(this) { DataContext = pl };
            Items.Add(widget);
            AddItemToCanvas(widget, (int)WidgetZOrder.CompressorShop);
        }

        private void RemovePipelineElement(IPipelineElement vlv)
        {
            var widget = Items.FirstOrDefault(w => w.DataContext == vlv);
            if (widget != null)
            {
                Items.Remove(widget);
                RemoveWidgetFromCanvas(widget);
            }
        }

        public void RemovedPipelineChild(PipelineOmElementWidgetBase vlv)
        {
            if (!_suppressVisualChildrenChanged)
            {
                OnVisualChildrenChanged();
            }
        }

        private async void FillModel(ISchemaSource source)
        {
            Debug.WriteLine("Заполняем схему");
            
            Clear();

            if (source == null)
            {
                return;
            }
            if (_busyIndicator != null)
            {
                _busyIndicator.BusyContent = "Отрисовка схемы...";
                _busyIndicator.IsBusy = true;
            }
            // Выводим информация о версии схемы
            if (_schemeInfo != null)
            {
                _schemeInfo.Text = source.SchemeInfo.ToString();
            }
            SchemeId = source.SchemeInfo.SchemeId;
            _suppressVisualChildrenChanged = true;

            var schemaParams = IsolatedStorageManager.LoadSchemaParams(SchemeId);

            var autofit = false;
            if (schemaParams != null)
            {
                LogManager.GetCurrentClassLogger()
                    .Trace($" Restore position {schemaParams.Position} and zoom {schemaParams.Zoom}");
                _supressSaveparams = true;
                Zoom = schemaParams.Zoom;
                Position = schemaParams.Position;
                _supressSaveparams = false;
            }
            else
            {
                autofit = true;
            }

            // Создаем элементы схемы:
            Debug.WriteLine("Создаем элементы схемы:");
            if (_busyIndicator != null)
            {
                _busyIndicator.BusyContent = "Отрисовка газопроводов...";
            }

            // Газопроводы
            Debug.WriteLine("Газопроводы");

            int task_returner = 0;

            for (var index = 0; index < source.Pipelines.Count; index++)
            {
                var pipeline = source.Pipelines[index];
                CreatePipelineWidget(pipeline);
                if (index % 20 == 0)
                {
                    await TaskEx.Yield();
                }
            }


            if (_busyIndicator != null)
            {
                _busyIndicator.BusyContent = "Отрисовка компресорных цехов...";
            }
            task_returner = 0;
            Debug.WriteLine("КЦ");
            // КЦ
            foreach (var cs in source.CompressorShops)
            {
                CreateCompressorShopWidget(cs);
                if (task_returner % 20 == 0)
                {
                    await TaskEx.Yield();
                }
                task_returner++;
            }

            if (_busyIndicator != null)
            {
                _busyIndicator.BusyContent = "Отрисовка соединений газопроводов...";
            }
            task_returner = 0;
            //            if (!IsReadOnly)
            {
                foreach (var pipeline in source.Pipelines)
                {
                    // Подсказки по соединениям газопроводов
                    foreach (var c in pipeline.PipelineConnections)
                    {
                        Pipelines[pipeline.Id].AddPipelineConnectionHint(c);
                    }
                    if (task_returner % 20 == 0)
                    {
                        await TaskEx.Yield();
                    }
                    task_returner++;
                }
            }



            if (_busyIndicator != null)
            {
                _busyIndicator.BusyContent = "Отрисовка текстовых меток...";
            }
            task_returner = 0;
            foreach (var cs in source.TextBlocks)
            {
                CreateTextBoxWidget(cs);

                if (task_returner % 20 == 0)
                {
                    await TaskEx.Yield();
                }
                task_returner++;
            }

            if (_busyIndicator != null)
            {
                _busyIndicator.BusyContent = "Отрисовка линий...";
            }
                        
            task_returner = 0;
            foreach (var pl in source.PolyLines)
            {
                CreatePolyLineWidget(pl);
                if (task_returner % 20 == 0)
                {
                    await TaskEx.Yield();
                }
                task_returner++;
            }

            task_returner = 0;
            foreach (var pl in source.CheckValves)
            {
                CreateCheckValveWidget(pl);
                if (task_returner % 20 == 0)
                {
                    await TaskEx.Yield();
                }
                task_returner++;
            }

            if (autofit)
            {
                AutoFit();
            }
            
            _suppressVisualChildrenChanged = false;

            OnVisualChildrenChanged();

            if (_busyIndicator != null)
            {
                _busyIndicator.IsBusy = false;
            }


        }

            /// <summary>
            ///     Удаляет все элементы со схемы
            /// </summary>
        private void Clear()
        {
            _itemsHost?.Children.Clear();
            Pipelines.Clear();

            foreach (var w in Items)
            {
                var schemaItem = (ISchemaItem) w;
                if (schemaItem != null && schemaItem.IsSelected)
                {
                    SelectionService.DeselectItem(schemaItem);
                }
                RemoveWidgetFromCanvas(w);
            }
            Items.Clear();
            OnVisualChildrenChanged();
        }

        private void InitCommands()
        {
        }

        private void InitContextMenu(RadRoutedEventArgs e)
        {
            var menu = (RadContextMenu) e.Source;
            if (menu == null)
            {
                return;
            }
            menu.Items.Clear();

            var el = menu.GetClickedElement<PipelineEditPointControl>() ??
                     menu.GetClickedElement<WidgetBase>() as ISupportContextMenu ?? menu.GetClickedElement<Schema>();
            var mousePosition = TransformToVisual(_itemsHost).Transform(menu.MousePosition);

            el.FillMenu(menu, mousePosition, this);
            if (!menu.Items.Any())
            {
                e.Handled = true;
            }
        }

        private void OnSelectedObjectChanged()
        {
            if (SelectedItem?.Position == null)
            {
                return;
            }
            BringIntoView(SelectedItem.Position, Zoom);
        }

        private void UpdateVisibility()
        {
            foreach (var widget in Items)
            {
                widget.Visibility = VisibleLayers.HasFlag(widget.Layer) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}