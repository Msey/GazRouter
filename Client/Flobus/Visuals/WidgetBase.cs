using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Primitives;
using GazRouter.Flobus.Utilites;
using JetBrains.Annotations;
using Telerik.Windows.Diagrams.Core;
using static System.Double;
using CommonExtensions = GazRouter.Flobus.Extensions.CommonExtensions;

namespace GazRouter.Flobus.Visuals
{
    /// <summary>
    ///     Базовый класс для всех элементов схемы
    /// </summary>
    public abstract class WidgetBase : ContentControl, INotifyPropertyChanged, ISchemaItem
    {
        public static readonly DependencyProperty IsFoundProperty = DependencyProperty.Register(
            nameof(IsFound), typeof(bool), typeof(WidgetBase), new PropertyMetadata(false, IsFoundPropertyChanged));

        public static readonly DependencyProperty ZIndexProperty = DependencyProperty.Register(
            nameof(ZIndex), typeof(int), typeof(WidgetBase), new PropertyMetadata(-1, OnZIndexPropertyChanged));

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            nameof(IsSelected), typeof(bool), typeof(WidgetBase),
            new PropertyMetadata(default(bool), OnIsSelectedPropertyChanged));

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            nameof(Position), typeof(Point), typeof(WidgetBase),
            new PropertyMetadata(new Point(NaN, NaN), OnPositionPropertyChanged));

        private Size _lastActualSize;
        private Schema schema;

        public WidgetBase([NotNull] Schema schema)
        {
            CreateBindings();

            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }
            DefaultStyleKey = GetType();
            IsManipulationAdornerVisible = true;
            Schema = schema;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsFound
        {
            get { return (bool) GetValue(IsFoundProperty); }
            set { SetValue(IsFoundProperty, value); }
        }

        public virtual bool Deleteble()
        {
            return false;
        }

        public virtual bool DeleteCommand()
        {
            return false;
        }

        public virtual Rect Bounds => new Rect(Position, ActualSize);

        public bool IsMouseOver { get; set; }

        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public Point Position
        {
            get { return (Point) GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public ItemVisibility VirtualizationState
        {
            get { return SchemaSurface.GetItemVisibility(this); }
            set { SchemaSurface.SetItemVisibility(this, value); }
        }

        public int ZIndex
        {
            get { return (int) GetValue(ZIndexProperty); }
            set { SetValue(ZIndexProperty, value); }
        }

        public SchemeLayers Layer => SchemeLayers.Base;
        public bool IsManipulationAdornerVisible { get; set; }
        public bool IsDraggingEnabled => true;
        public bool IsResizingEnabled { get; set; }
        public Schema Schema { get; }

        protected Size ActualSize
        {
            get
            {
                var newWidth = IsNaN(Width) ? ActualWidth : Math.Max(Width, MinWidth);
                var newHeight = IsNaN(Height) ? ActualHeight : Math.Max(Height, MinHeight);

                if (newWidth > 0 && newHeight > 0)
                {
                    _lastActualSize = new Size(newWidth, newHeight);
                }
                return _lastActualSize;
            }
        }

        public virtual void Update()
        {
        }

        public virtual void Select()
        {
            throw new NotImplementedException();
        }

        public virtual void Deselect()
        {
        }

        protected virtual void CreateBindings()
        {
            SetBinding(IsFoundProperty, new Binding(nameof(IsFound)) { Mode = BindingMode.TwoWay });
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            IsMouseOver = true;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            IsMouseOver = false;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            CaptureMouse();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            ReleaseMouseCapture();
        }

        protected virtual void OnPositionChanged(Point position)
        {
            CommonExtensions.SetLocation(this, position);
        }

        protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                Focus();
            }

            UpdateVisualStates();
        }

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void BindVisibilityToVirtualizationVisibility()
        {
            var visibilityBinding = new Binding
            {
                Path = new PropertyPath(SchemaSurface.ItemVisibilityProperty),
                Source = this,
                Mode = BindingMode.TwoWay,
                Converter = ItemVisibilityToVisibilityConverter.Instance
            };
            SetBinding(VisibilityProperty, visibilityBinding);
        }

        private static void IsFoundPropertyChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((WidgetBase) dependencyObject).IsFoundChanged();
        }

        private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = (WidgetBase) d;
            owner.OnIsSelectedChanged((bool) e.OldValue, (bool) e.NewValue);
        }

        private static void OnZIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WidgetBase) d).OnZIndexChanged((int) e.NewValue);
        }

        private static void OnPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var owner = (WidgetBase) d;

            var newPoint = (Point) e.NewValue;
            var oldPoint = (Point) e.OldValue;
            if (newPoint != oldPoint)
            {
                owner.OnPositionChanged(newPoint);
            }
        }

        private void IsFoundChanged()
        {
            if (IsFound)
            {
                Schema.BringIntoCenterView(Position);
                Schema.FoundItem = this;
            }
            VisualStateManager.GoToState(this, IsFound ? FoundStates.Found : FoundStates.NotFound, false);
        }

        private void OnZIndexChanged(int newValue)
        {
            Canvas.SetZIndex(this, newValue);
            OnPropertyChanged(nameof(ZIndex));
        }

        private void UpdateVisualStates()
        {
            VisualStateManager.GoToState(this, IsSelected ? VisualStates.Selected : VisualStates.Unselected, false);
        }
    }
}