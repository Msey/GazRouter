using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Common.ViewModel;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
namespace GazRouter.Modes.GasCosts
{
    public class RadGridViewColumnsBinding : PropertyChangedBase
    {
        public static readonly DependencyProperty ColumnsCollectionProperty =
            DependencyProperty.RegisterAttached("ColumnsCollection", typeof(IEnumerable<GasCostColumnDefinition>),
                                                typeof(RadGridViewColumnsBinding),
                                                new PropertyMetadata(OnColumnsCollectionChanged));
        public static readonly DependencyProperty DefaultHeaderCellStyleProperty = 
            DependencyProperty.RegisterAttached("DefaultHeaderCellStyle", typeof(Style), 
                typeof(RadGridViewColumnsBinding), new PropertyMetadata(default(Style)));
        public static readonly DependencyProperty LongHeaderCellStyleProperty = 
            DependencyProperty.RegisterAttached("LongHeaderCellStyle", typeof(Style), 
                typeof(RadGridViewColumnsBinding), new PropertyMetadata(default(Style)));
        public static readonly DependencyProperty AppendToExistingColumnsProperty = 
            DependencyProperty.RegisterAttached("AppendToExistingColumns", typeof(bool), 
                typeof(RadGridViewColumnsBinding), new PropertyMetadata(false));
        private static void OnColumnsCollectionChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var gridView = o as GridViewDataControl;
            if (gridView == null) return;

            if (!GetAppendToExistingColumns(o))
               gridView.Columns.Clear();

            if (e.NewValue == null) return;
            var collection = e.NewValue as IEnumerable<GasCostColumnDefinition>;

            if (collection == null) return;

            foreach (var col in collection)
            {
                var column = GetColumn(o, col);
                gridView.Columns.Add(column);
                col.PropertyChanged += (s, args) =>
                {
                    column.IsVisible = ((GasCostColumnDefinition)s).IsVisible;
                };
            }

            var observableCollection = collection as ObservableCollection<GasCostColumnDefinition>;

            if (observableCollection == null) return;
            observableCollection.CollectionChanged += (sender, arguments) =>
            {
                switch (arguments.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (GasCostColumnDefinition col in arguments.NewItems)
                        {
                            var column = GetColumn(o,col);
                            gridView.Columns.Add(column);
                            column.DisplayIndex = arguments.NewStartingIndex;
                            col.PropertyChanged += (s, args) =>
                            {
                                column.IsVisible = ((GasCostColumnDefinition)s).IsVisible;
                            };
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (GasCostColumnDefinition col in arguments.OldItems)
                        {
                            var column = gridView.Columns[col.UniqueName];
                            if (column != null)
                                gridView.Columns.Remove(column);
                        }
                        break;
                }
            };
        }
        private static GridViewDataColumn GetColumn(DependencyObject dependencyObject, GasCostColumnDefinition col)
        {
            var column = new GridViewDataColumn
                             {
                                 UniqueName = col.UniqueName,
                                 CellStyleSelector = col.CellStyleSelector,
                                 DataMemberBinding = col.DataMemberBinding,
                                 Header = col.Header,
                                 TextAlignment = col.TextAligment,
                                 HeaderTextAlignment = col.HeaderTextAlignment,
                                 IsFilterable = col.IsFilterable,
                                 IsGroupable = col.IsGroupable,
                                 IsReadOnly = col.IsReadOnly,
                                 IsResizable = col.IsResizable,
                                 IsSortable = col.IsSorteable,
                                 IsVisible = col.IsVisible,
                                 CellTemplateSelector = col.CellTemplateSelector,
                                 CellTemplate = col.CellTemplate, 
                                 MaxWidth = col.MaxWidth,
                                 // Width = col.Width
                             };

            if (col.Width != GridViewLength.Auto) column.Width = col.Width;
            const int minLongHeaderLength = 50;
            var style = col.HeaderCellStyle ?? (col.Header.Length >= minLongHeaderLength
                ? GetLongCellStyleNewInstance(dependencyObject as UIElement) 
                : GetDefaultCellStyleNewInstance(dependencyObject as UIElement));
            if (!string.IsNullOrEmpty(col.HeaderToolTip))
                style.Setters.Add(new Setter(ToolTipService.ToolTipProperty, col.HeaderToolTip));
            column.HeaderCellStyle = style;
            return column;
        }
        private static Style GetDefaultCellStyleNewInstance(UIElement element)
        {
            var style = 
             new Style(typeof(GridViewHeaderCell))
            {
                BasedOn = GetDefaultHeaderCellStyle(element)
            };
            return style;
        }
        private static Style GetLongCellStyleNewInstance(UIElement element)
        {
            var style =
             new Style(typeof(GridViewHeaderCell))
             {
                 BasedOn = GetLongHeaderCellStyle(element)
             };
            return style;
        }
        public static void SetColumnsCollection(DependencyObject o, ObservableCollection<GasCostColumnDefinition> value)
        {
            o.SetValue(ColumnsCollectionProperty, value);
        }
        public static ObservableCollection<GasCostColumnDefinition> GetColumnsCollection(DependencyObject o)
        {
            return o.GetValue(ColumnsCollectionProperty) as ObservableCollection<GasCostColumnDefinition>;
        }
        public static Style GetDefaultHeaderCellStyle(UIElement element)
        {
            return (Style)element.GetValue(DefaultHeaderCellStyleProperty);
        }
        public static void SetDefaultHeaderCellStyle(UIElement element, Style value)
        {
            element.SetValue(DefaultHeaderCellStyleProperty, value);
        }
        public static Style GetLongHeaderCellStyle(UIElement element)
        {
            return (Style)element.GetValue(LongHeaderCellStyleProperty);
        }
        public static void SetLongHeaderCellStyle(UIElement element, Style value)
        {
            element.SetValue(LongHeaderCellStyleProperty, value);
        }
        public static bool GetAppendToExistingColumns(DependencyObject element)
        {
            return (bool)element.GetValue(AppendToExistingColumnsProperty);
        }
        public static void SetAppendToExistingColumns(DependencyObject element, bool value)
        {
            element.SetValue(AppendToExistingColumnsProperty, value);
        }
    }
}