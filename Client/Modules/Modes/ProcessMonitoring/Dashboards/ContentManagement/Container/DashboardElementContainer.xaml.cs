using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using GazRouter.Common.Cache;
using GazRouter.Controls.Dialogs;
using GazRouter.Controls.Dialogs.EntityPicker;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Elements;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Misc;
using GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Model;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Telerik.Windows.Controls;
using EntityPickerDialogView = GazRouter.Controls.Dialogs.EntityPicker.EntityPickerDialogView;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement.Container
{
    public partial class DashboardElementContainer : IContextMenuGenerator
    {
#region constructor
        public DashboardElementContainer()
        {
            _isInit = false;
            InitializeComponent();
            // 
            Loaded += Init;
            IsEditMode = false;
        }
        public void Init(object obj, RoutedEventArgs routedEvent)
        {
            if (_isInit) return;
            //DefaultStyleKey = typeof(DashboardElementContainer);
            _elements = new List<ElementViewBase>();
            _selectedElements = new List<ElementViewBase>();
            InitCommands();
            if (_scroll == null || _canvas == null) return;
            _scroll.KeyDown += OnKeyDown;
            _canvas.KeyDown += OnKeyDown;
            if (_toolbar != null)
            {
                _toolbar.Visibility = Visibility.Collapsed;
                _alignLeftBtn.Click += (s, e) => AlignSelectedElements(1);
                _alignTopBtn.Click += (s, e) => AlignSelectedElements(2);
                _alignRightBtn.Click += (s, e) => AlignSelectedElements(3);
                _alignBottomBtn.Click += (s, e) => AlignSelectedElements(4);
                _brushBtn.Click += _brushBtn_Click;
            }
            if (_back != null)
            {
                _back.MouseLeftButtonDown -= m_back_MouseLeftButtonDown;
                _back.MouseLeftButtonUp -= m_back_MouseLeftButtonUp;
                _back.MouseMove -= m_back_MouseMove;
                _back.MouseRightButtonDown -= OnMouseRightButtonUp;
                _canvas.MouseRightButtonDown -= OnMouseRightButtonUp;
            }
            _back = new Rectangle
            {
                Fill = new SolidColorBrush(Color.FromArgb(0xff, 0xf5, 0xf5, 0xf5)),
                StrokeThickness = 0,
                Width = _canvas.Width + 1000,
                Height = _canvas.Height + 1000
            };
            _canvas.Children.Add(_back);
            _back.MouseLeftButtonDown += m_back_MouseLeftButtonDown;
            _back.MouseLeftButtonUp += m_back_MouseLeftButtonUp;
            _back.MouseMove += m_back_MouseMove;
            _back.MouseRightButtonUp += OnMouseRightButtonUp;
            _canvas.MouseRightButtonUp += OnMouseRightButtonUp;
            _selection = new Polygon
            {
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Color.FromArgb(0xff, 0xdc, 0x14, 0x3c)),
                StrokeDashArray = new DoubleCollection { 1, 1 },
                Visibility = Visibility.Collapsed
            };
            _canvas.Children.Add(_selection);
            Canvas.SetZIndex(_selection, 22222);
            Tooltip = new TooltipWidget(this);
            TinyTrend = new TinyTrend(this)
            {
                Height = 80,
                Width = 120
            };
            SelectedSerieIndex = 0;
            CreateDashboardElements();
            _isInit = true;
        }
        #endregion
        #region variables
        private static bool _isInit; 
        private Rectangle _back;
        private List<ElementViewBase> _elements;
        private List<ElementViewBase> _selectedElements;
        private ElementViewBase _brushElement;
        private bool _mouseButtonPressed;
        // Групповое выделение
        private Polygon _selection;
        private Point _mouseStartPosition;
        /// <summary> Цвет фона дашборда </summary>
        public new Brush Background
        {
            get { return _back.Fill; }
            set { _back.Fill = value; }
        }
        private int _selectedSerieIndex;
        /// <summary>
        /// индекс выбранного сеанса, по всем дашбордам
        /// </summary>
        public int SelectedSerieIndex
        {
            get { return _selectedSerieIndex; }
            set 
            {
                _selectedSerieIndex = value;
                UpdateElements();
            }
        }
        /// <summary>
        /// Тултип для элементов дашборда. Элементы могут его вызывать, 
        /// чтобы показывать информацию о себе.
        /// </summary>
        public TooltipWidget Tooltip { get; private set; }
        /// <summary>
        /// Маленький трендик. 
        /// Отображается как тултип при навелении на значение параметра.
        /// </summary>
        public TinyTrend TinyTrend { get; private set; }
#endregion
        private static void OnIsEditModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!_isInit) return;
            //
            var dashboard = d as DashboardElementContainer;
            if (dashboard == null) return;
            dashboard.UpdateElements();
            dashboard.AdjustCanvasSize();
            var isEditMode = (bool)e.NewValue;
            dashboard._toolbar.Visibility = isEditMode ? Visibility.Visible : Visibility.Collapsed;
            //
            if (!isEditMode) dashboard.ClearMenu();
        }
#region dproperty
        public static readonly DependencyProperty LayoutProperty
            = DependencyProperty.Register("Layout", typeof(DashboardLayout), typeof(DashboardElementContainer), new PropertyMetadata(null, OnLayoutPropertyChanged));        
        public DashboardLayout Layout
        {
            get { return (DashboardLayout)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }
        private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!_isInit) return;

            var dashboard = d as DashboardElementContainer;
            if (dashboard?.Data == null) return;
            //
            dashboard.DestroyElements();
            dashboard.CreateDashboardElements();    
        }
        public static readonly DependencyProperty ScaleProperty
            = DependencyProperty.Register("Scale", typeof(int), typeof(DashboardElementContainer), new PropertyMetadata(100, OnScalePropertyChanged));
        public int Scale
        {
            get { return (int)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }
        private static void OnScalePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!_isInit) return;

            var dashboard = d as DashboardElementContainer;
            if (dashboard == null) return;
            var newScale = double.Parse(e.NewValue.ToString());
            //dashboard._scale.CenterX = dashboard._scroll.HorizontalOffset + dashboard._scroll.ViewportWidth / 2;
            //dashboard._scale.CenterY = dashboard._scroll.VerticalOffset + dashboard._scroll.ViewportHeight/2;
            dashboard._scale.ScaleX = newScale / 100;
            dashboard._scale.ScaleY = newScale / 100;
            dashboard.AdjustCanvasSize();
        }
        public static readonly DependencyProperty DataProperty
            = DependencyProperty.Register("Data", typeof(DashboardData), typeof(DashboardElementContainer), new PropertyMetadata(null, OnDataPropertyChanged));
        public DashboardData Data
        {
            get
            {
                return (DashboardData)GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }
        private static void OnDataPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!_isInit) return;

            var dashboard = d as DashboardElementContainer;
            if(dashboard?.Data != null)
                dashboard.UpdateElementsData();
        }
        /// <summary>
        /// Режим работы дашборда
        /// </summary>
        public static readonly DependencyProperty IsEditModeProperty
            = DependencyProperty.Register("IsEditMode", typeof(bool), typeof(DashboardElementContainer), new PropertyMetadata(false, OnIsEditModePropertyChanged));
        public bool IsEditMode
        {
            get { return (bool)GetValue(IsEditModeProperty); }
            set
            {
                SetValue(IsEditModeProperty, value);
            }
        }
        public static readonly DependencyProperty ToTrendCommandProperty =
            DependencyProperty.Register("ToTrendCommand", typeof(DelegateCommand<ToTrendCommandParameter>), typeof(DashboardElementContainer), new PropertyMetadata(null));
        public DelegateCommand<ToTrendCommandParameter> ToTrendCommand
        {
            get { return (DelegateCommand<ToTrendCommandParameter>)GetValue(ToTrendCommandProperty); }
            set { SetValue(ToTrendCommandProperty, value); }
        }
#endregion
#region ContextMenu
        public void ClearMenu()
        {
            _contextMenu.Items.Clear();
        }
#region commands
        private void InitCommands()
        {
            // Инициализация команды добавления нового элемента на дашборд
            _addEntityElement = new DelegateCommand<MouseButtonEventArgs>(OnAddEntityElement);
            _addTextElement = new DelegateCommand<MouseButtonEventArgs>(OnAddTextElement);
            _addLineElement = new DelegateCommand<MouseButtonEventArgs>(OnAddLineElement);
            _addValueElement = new DelegateCommand<MouseButtonEventArgs>(OnAddPropertyElement);
            _addShapeElement = new DelegateCommand<MouseButtonEventArgs>(OnAddShapeElement);
        }
        private DelegateCommand<MouseButtonEventArgs> _addTextElement;
        private DelegateCommand<MouseButtonEventArgs> _addEntityElement;
        private DelegateCommand<MouseButtonEventArgs> _addLineElement;
        private DelegateCommand<MouseButtonEventArgs> _addValueElement;
        private DelegateCommand<MouseButtonEventArgs> _addShapeElement;
#endregion
        public void FillMenu(RadContextMenu menu, MouseButtonEventArgs e)
        {
            if (IsEditMode)
            {
                //menu.AddTestCommand(e, "Добавить объект...", _addEntityElement);
                //menu.AddTestCommand(e, "Добавить параметр...", _addEntityElement);
                //menu.AddTestCommand(e, "Добавить линию...", _addEntityElement);
                //menu.AddTestCommand(e, "Добавить текст...", _addEntityElement);
                //menu.AddTestCommand(e, "Добавить тренд...", _addEntityElement);


                var addGroup = new RadMenuItem
                {
                    Header = "Добавить",
                    //Command = _addEntityElement
                };
                menu.Items.Add(addGroup);
                addGroup.Items.Add(new RadMenuItem
                {
                    Header = "Объект...",
                    Command = _addEntityElement,
                    CommandParameter = e
                });

                addGroup.Items.Add(new RadMenuItem
                {
                    Header = "Параметр...",
                    Command = _addValueElement,
                    CommandParameter = e,
                });

                addGroup.Items.Add(new RadMenuItem
                {
                    Header = "Линию...",
                    Command = _addLineElement,
                    CommandParameter = e,
                });

                addGroup.Items.Add(new RadMenuItem
                {
                    Header = "Фигуру...",
                    Command = _addShapeElement,
                    CommandParameter = e,
                });

                addGroup.Items.Add(new RadMenuItem
                {
                    Header = "Текст...",
                    Command = _addTextElement,
                    CommandParameter = e,
                    
                });

                addGroup.Items.Add(new RadMenuItem
                {
                    Header = "Тренд...",
                    Command = _addEntityElement,
                    CommandParameter = e,
                    IsEnabled = false
                });


                //menu.Items.Add(new RadMenuItem()
                //{
                //    Header = "Параметры...",
                //    Command = _changeDashboardSettings,
                //    CommandParameter = e,
                //});
            }

        }
        private void InitContextMenu(object sender, MouseButtonEventArgs e)
        {
            _contextMenu.Items.Clear();
            var obj = sender as IContextMenuGenerator;
            obj?.FillMenu(_contextMenu, e);
            if (!_contextMenu.Items.Any()) e.Handled = true;
        }
        private EntityPickerDialogViewModel _epvm;
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();
#region events
        void _brushBtn_Click(object sender, RoutedEventArgs args)
        {
            if (_selectedElements.Count > 0)
            {
                var minX = _selectedElements.Min(e => e.Position.X);
                _brushElement = _selectedElements.Single(e => e.Position.X == minX);

                foreach (var els in _selectedElements)
                    els.Deselect();
                _selectedElements.Clear();

                
                if(_brushElement.Select())
                    _selectedElements.Add(_brushElement);
            }
        }
        public void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_contextMenu != null)
                InitContextMenu(sender == _back ? this : sender, e);
        }
        /// <summary>
        /// Команда добавления ЭЛЕМЕНТА отображения СУЩНОСТИ
        /// </summary>
        /// <param name="eventArg"></param>
        private void OnAddEntityElement(MouseButtonEventArgs eventArg)
        {
            _epvm = new EntityPickerDialogViewModel(() =>
            {
                var pos = eventArg.GetPosition(Canvas);
                var em = new EntityElementModel
                {
                    EntityId = _epvm.SelectedItem.Id, 
                    EntityName = _epvm.SelectedItem.ShortPath, 
                    EntityType = _epvm.SelectedItem.EntityType,
                    Position = new Point(pos.X, pos.Y), 
                    Z = _elements.Count > 0 ? _elements.Max(e => e.Z) + 10 : 10,
                    VisiblePropertyTypeList = DefaultDisplaySettings.GetDefaultPropertyTypeDisplaySettingsList(_epvm.SelectedItem.EntityType)
                    
                };

                new EntityElementView(em, this);
                Layout.ElementList.Add(em);
                AdjustCanvasSize();
            }, 
            new List<EntityType>
            {
                EntityType.CompShop, 
                EntityType.DistrStation, 
                EntityType.MeasLine, 
                EntityType.CompUnit, 
                EntityType.DistrStationOutlet,
                EntityType.MeasPoint,
                EntityType.ReducingStation,
                EntityType.Valve,
                EntityType.MeasStation,
                EntityType.Aggregator
            });
            var ep = new EntityPickerDialogView {DataContext = _epvm};
            ep.ShowDialog();
        }
        private SelectEntityPropertyViewModel _psvm;
        /// <summary>
        /// Команда добавления ЭЛЕМЕНТА отображения ПАРАМЕТРА
        /// </summary>
        /// <param name="eventArg"></param>
        private void OnAddPropertyElement(MouseButtonEventArgs eventArg)
        {
            _psvm = new SelectEntityPropertyViewModel(() =>
            {
                var pos = eventArg.GetPosition(Canvas);
                var em = new PropertyElementModel
                {
                    EntityId = _psvm.SelectedItem.Id,
                    EntityName = _psvm.SelectedItem.ShortPath,
                    EntityType = _psvm.SelectedItem.EntityType,
                    Position = new Point(pos.X, pos.Y),
                    Z = _elements.Count > 0 ? _elements.Max(e => e.Z) + 10 : 10,
                    PropertyType = _psvm.SelectedEntityProperty.PropertyType,
                    Comment = _psvm.SelectedItem.ShortPath 
                    + Environment.NewLine 
                    + ClientCache.DictionaryRepository.PropertyTypes.Single(pt => pt.PropertyType == _psvm.SelectedEntityProperty.PropertyType).Name

                };

                new PropertyElementView(em, this);
                Layout.ElementList.Add(em);
                AdjustCanvasSize();
            }, null, /*для примера - Только Давление*/new List<PhysicalType>{PhysicalType.Pressure});

            var ps = new SelectEntityProperty { DataContext = _psvm };
            ps.Show();
        }
        /// <summary>
        /// Команда добавления ЭЛЕМЕНТА отображения ТЕКСТА
        /// </summary>
        /// <param name="eventArg"></param>
        private void OnAddTextElement(MouseButtonEventArgs eventArg)
        {
            var pos = eventArg.GetPosition(Canvas);
            var em = new TextElementModel
            {
                Position = new Point(pos.X, pos.Y),
                Z = _elements.Count > 0 ? _elements.Max(e => e.Z) + 10 : 10,
                Text = "{Отредактируйте текст}"
            };
            new TextElementView(em, this);
            Layout.ElementList.Add(em);
            AdjustCanvasSize();
        }
        /// <summary>
        /// Команда добавления ЭЛЕМЕНТА отображения ЛИНИИ
        /// </summary>
        /// <param name="eventArg"></param>
        private void OnAddLineElement(MouseButtonEventArgs eventArg)
        {
            var pos = eventArg.GetPosition(Canvas);
            var em = new LineElementModel
            {
                PointList = new List<LineElementPointModel>
                {
                    new LineElementPointModel { Position = new Point(pos.X - 50, pos.Y - 50), DeleteAllowed = false},
                    new LineElementPointModel { Position = new Point(pos.X + 50, pos.Y + 50), DeleteAllowed = false}
                },
                Z = _elements.Count > 0 ? _elements.Max(e => e.Z) + 10 : 10,
            };
            new LineElementView(em, this);
            Layout.ElementList.Add(em);
            AdjustCanvasSize();

        }
        /// <summary>
        /// Команда добавления ЭЛЕМЕНТА отображения ФИГУРЫ
        /// </summary>
        /// <param name="eventArg"></param>
        private void OnAddShapeElement(MouseButtonEventArgs eventArg)
        {
            var pos = eventArg.GetPosition(Canvas);
            var em = new ShapeElementModel
            {
                Position = pos,
                Width = 30,
                Height = 30,
                Z = _elements.Count > 0 ? _elements.Max(e => e.Z) + 10 : 10,
            };

            var v = new ShapeElementView(em,this,true);
            Layout.ElementList.Add(em);
            AdjustCanvasSize();

        }
#endregion
#endregion
        public Canvas Canvas
        {
            get { return _canvas; }
        }
        public void AdjustCanvasSize()
        {
            if (_elements == null) return;
            //
            var maxX = _elements.Count > 0 ? _elements.Max(e => e.Position.X + e.Width) * 1.05 : 0;
            var maxY = _elements.Count > 0 ? _elements.Max(e => e.Position.Y + e.Height) * 1.05 : 0;

            var scale = ((double) Scale)/100;

            if (maxX * scale < _scroll.ViewportWidth) maxX = _scroll.ViewportWidth / scale;
            if (maxY * scale < _scroll.ViewportHeight) maxY = _scroll.ViewportHeight / scale;

            if (IsEditMode)
            {
                Canvas.Width = maxX * 2;
                Canvas.Height = maxY * 2;
            }
            else
            {
                Canvas.Width = maxX;
                Canvas.Height = maxY;
            }
        }
#region Elements
        private void CreateDashboardElements()
        {
            if (Layout != null)
            {
                foreach (var elem in Layout.ElementList)
                {
                    if (elem.GetType() == typeof(EntityElementModel))
                    {
                        new EntityElementView((EntityElementModel)elem, this);
                    }
                    if (elem.GetType() == typeof(TextElementModel))
                    {
                        new TextElementView((TextElementModel)elem, this);
                    }
                    if (elem.GetType() == typeof(LineElementModel))
                    {
                        new LineElementView((LineElementModel)elem, this);
                    }
                    if (elem.GetType() == typeof(ShapeElementModel))
                    {
                        new ShapeElementView((ShapeElementModel)elem, this, false);
                    }
                    if (elem.GetType() == typeof(PropertyElementModel))
                    {
                        new PropertyElementView((PropertyElementModel)elem, this);
                    }
                    //var e = new EntityElementView(layout, this);
                    //if (Layout.Measurings != null)
                    //    e.UpdateData();
                }
                AdjustCanvasSize();
            }
            IsTabStop = true;
            Focus();
        }
        private void UpdateElementsData()
        {
            if (Data != null)
                _elements?.ForEach(e => e.UpdateData());
        }
        public void AddElement(ElementViewBase elementView)
        {
            _elements.Add(elementView);
            elementView.MouseRightButtonUp += OnMouseRightButtonUp;
            elementView.MouseLeftButtonDown += element_MouseLeftButtonDown;
            elementView.ElementMove += element_Move;
            elementView.MouseLeftButtonUp += element_MouseLeftButtonUp;
            
        }
        public void RemoveElement(ElementViewBase elementView)
        {
            _elements.Remove(elementView);
            Layout.ElementList.Remove(elementView.ElementModel);
            elementView.MouseRightButtonUp -= OnMouseRightButtonUp;
            elementView.MouseLeftButtonDown -= element_MouseLeftButtonDown;
            elementView.ElementMove -= element_Move;
            elementView.MouseLeftButtonUp -= element_MouseLeftButtonUp;
            elementView.Destroy();
        }
        public void MoveElementBackward(ElementViewBase elementView)
        {
            elementView.Z = 10;
            foreach(var el in Layout.ElementList.Where(e => e != elementView.ElementModel))
                el.Z += 10;
        }
        public void MoveElementForward(ElementViewBase elementView)
        {
            elementView.Z = Layout.ElementList.Where(e => e != elementView.ElementModel).Max(e => e.Z) + 10;
        }
        private IEnumerable<ElementViewBase> FindElements(Rect selectionRect)
        {
            //_elements.Where(e => selectionRect.Contains(e.Position))

            return
                _elements.Where(
                    e =>
                        selectionRect.Contains(e.Position) &&
                        selectionRect.Contains(new Point(e.Position.X + e.Width, e.Position.Y + e.Height)));
        }
        public void StartDragElements()
        {
            foreach (var e in _selectedElements)
            {
               e.StartDrag();
            }
        }
        public void EndDragElements()
        {
            foreach (var e in _selectedElements)
            {
                e.EndDrag();
            }
            AdjustCanvasSize();
        }
        public void MoveSelectedElements(Point offset)
        {
            foreach (var e in _selectedElements)
            {
                e.Move(offset.X, offset.Y);
            }
        }
        public void RemoveSelectedElements()
        {
            foreach (var e in _selectedElements)
            {
                RemoveElement(e);
            }
        }
        private void DestroyElements()
        {
            _elements?.ForEach(e => e.Destroy());
        }
        private void UpdateElements()
        {
            foreach (var e in _elements)
                e.UpdatePosition();
        }
        private void AlignSelectedElements(int direction)
        {
            if (_selectedElements.Count == 0) return;
             switch (direction)
            {
                case 1: //left
                    var targetLeft = _selectedElements.Min(e => e.Position.X);
                    _selectedElements.ForEach(e => e.Move(targetLeft - e.Position.X, 0));
                    break;
                case 2: //top
                    var targetTop = _selectedElements.Min(e => e.Position.Y);
                    _selectedElements.ForEach(e => e.Move(0, targetTop - e.Position.Y));
                    break;
                case 3: //right
                    var targetRight = _selectedElements.Max(e => e.Position.X + e.Width);
                    _selectedElements.ForEach(e => e.Move(targetRight - (e.Position.X + e.Width), 0));
                    break;
                case 4: //bottom
                    var targetBottom = _selectedElements.Max(e => e.Position.Y + e.Height);
                    _selectedElements.ForEach(e => e.Move(0, targetBottom - (e.Position.Y + e.Height)));
                    break;
            }
        }
#endregion
#region EVENT_HANDLERS
        void m_back_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseButtonPressed)
            {
                if (IsEditMode)
                {
                    var pt = e.GetPosition(_canvas);

                    _selection.Points = new PointCollection {
                                _mouseStartPosition,
                                new Point(pt.X, _mouseStartPosition.Y),
                                pt,
                                new Point(_mouseStartPosition.X, pt.Y)
                            };
                    
                }
                else
                {
                    var curMousePos = e.GetPosition(null);
                    double newHorOffset = _scroll.HorizontalOffset + (_mouseStartPosition.X - curMousePos.X);
                    double newVertOffset = _scroll.VerticalOffset + (_mouseStartPosition.Y - curMousePos.Y);
                    //double newHorOffset = _scroll.HorizontalOffset + Zoom * (_mousePosBuff.X - curMousePos.X);
                    //double newVertOffset = _scroll.VerticalOffset + Zoom * (_mousePosBuff.Y - curMousePos.Y);

                    if (newHorOffset < 0)
                        newHorOffset = 0;
                    if (newVertOffset < 0)
                        newVertOffset = 0;

                    _scroll.ScrollToHorizontalOffset(newHorOffset);
                    _scroll.ScrollToVerticalOffset(newVertOffset);
                    _scroll.UpdateLayout();
                    _mouseStartPosition = curMousePos;
                }
            }
        }
        void m_back_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _mouseButtonPressed = false;
            _back.ReleaseMouseCapture();

            if (IsEditMode)
            {
                _selection.Visibility = Visibility.Collapsed;
                // снимаем текущее выделение
                foreach (var els in _selectedElements)
                    els.Deselect();
                _selectedElements.Clear();

                // находим выбранные элементы и выделяем их
                var list = FindElements(new Rect(_mouseStartPosition, e.GetPosition(_canvas)));
                foreach (var el in list)
                    if (el.Select())
                        _selectedElements.Add(el);

            }
        }
        void m_back_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mouseButtonPressed = true;
            _back.CaptureMouse();

            foreach (var element in _selectedElements)
                element.Deselect();

            if (IsEditMode)
            {
                _mouseStartPosition = e.GetPosition(_canvas);
                _selection.Points = new PointCollection {
                                            _mouseStartPosition,
                                            _mouseStartPosition,
                                            _mouseStartPosition,
                                            _mouseStartPosition
                                        };
                _selection.Visibility = Visibility.Visible;
            }
            else
            {
                _mouseStartPosition = e.GetPosition(null);
            }

        }
        /// <summary>
        /// m_selectionTrigger - необходим для реализации следующего поведения:
        /// Если выделено несколько элементов то при щелчке на один из выбранных элементов
        /// выделение с других элементов должно сниматься, а выделение на этом элементе должно
        /// остаться. Но если мы нажали на этот элемент и потащили его, то передвигаются 
        /// все выбранные элементы и выделение с других элементов в этом случае не снимается.
        /// Флаг устанвливается в обработчике ElementMouseDown, затем если произошло движение
        /// элемента, то он снимается в обработчике ElementMove, если он остается в установленном состоянии, 
        /// то в обработчике ElementMouseUp выделение со всех элементов снимается, кроме выбранного.
        /// </summary>
        bool _selectionTrigger;
        void element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var element = sender as ElementViewBase;

            // Проверить выбран ли элемент
            if (_selectedElements.Contains(element))
                if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
                {
                    element.Deselect();
                    _selectedElements.Clear();
                }
                else _selectionTrigger = true;
            else
            {
                // при щелчке кнопка Ctrl не нажата, то снимаем текущее выделение
                if ((Keyboard.Modifiers & ModifierKeys.Control) == 0)
                {
                    foreach (var els in _selectedElements)
                        els.Deselect();
                    _selectedElements.Clear();
                }

                if (_brushElement != null)
                {
                    element.ElementModel.CopyStyle(_brushElement.ElementModel);
                    element.UpdateContents();
                    _brushElement = null;
                    return;
                }

                if (element.Select())
                    _selectedElements.Add(element);
            }
        }
        void element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var element = sender as ElementViewBase;

            // ... и при щелчке кнопка Ctrl не нажата, то снимаем выделение
            if (_selectionTrigger)
            {
                foreach (var els in _selectedElements)
                    els.Deselect();
                _selectedElements.Clear();

                if (element.Select())
                    _selectedElements.Add(element);
            }
        }
        void element_Move(object sender, Point offset)
        {
            foreach (var e in _selectedElements)
            {
                e.Move(offset.X, offset.Y);
            }
            _selectionTrigger = false;
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    MoveSelectedElements(new Point(-1, 0));
                    break;

                case Key.Up:
                    MoveSelectedElements(new Point(0, -1));
                    break;

                case Key.Right:
                    MoveSelectedElements(new Point(1, 0));
                    break;

                case Key.Down:
                    MoveSelectedElements(new Point(0, 1));
                    break;

                case Key.Delete:
                    RemoveSelectedElements();
                    break;



                case Key.L:
                    AlignSelectedElements(1);
                    break;
                case Key.T:
                    AlignSelectedElements(2);
                    break;
                case Key.R:
                    AlignSelectedElements(3);
                    break;
                case Key.B:
                    AlignSelectedElements(4);
                    break;
            }
            e.Handled = true;
        }
#endregion
    }
    public class ToTrendCommandParameter
    {
        public ToTrendCommandParameter(CommonEntityDTO entity, PropertyType propertyType)
        {
            Entity = entity;
            PropertyType = propertyType;
        }
        public CommonEntityDTO Entity { get; set; }
        public PropertyType PropertyType { get; set; }
    }
}
