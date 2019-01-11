using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Telerik.Windows.Controls;
using PropertyMetadata = Telerik.Windows.PropertyMetadata;

namespace GazRouter.ActionsRolesUsers.Controls
{
    public class DynamicTreeListView : RadTreeListView
    {
        private const string _rowTemplate =
            @"<telerik:GridViewDataColumn xmlns:telerik=""http://schemas.telerik.com/2008/xaml/presentation"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" DataMemberBinding=""{{Binding Path=Values[{0}]}}"" IsReadOnly=""True"" IsFilterable=""False"" IsSortable=""False"" TextWrapping=""Wrap"" UniqueName=""{2}"">
                            <telerik:GridViewDataColumn.Header>
                                <TextBlock Margin=""5,3,5,0"" Text=""{1}"" />
                            </telerik:GridViewDataColumn.Header>
                            <telerik:GridViewDataColumn.CellTemplate>
                                <DataTemplate>
                                    <CheckBox IsEnabled=""{{Binding Path=IsEnabled}}"" HorizontalAlignment=""Center"" IsChecked=""{{Binding Path=Values[{0}].IsChecked, Mode=TwoWay}}"" Visibility=""{{Binding Visibility}}"" />
                                </DataTemplate>
                            </telerik:GridViewDataColumn.CellTemplate>    
                        </telerik:GridViewDataColumn>";


        private GridViewColumn CreateColumn(string colName)
        {
            string xaml = string.Format(_rowTemplate, colName, colName, colName);

            var column = (GridViewColumn) XamlReader.Load(xaml);

            return column;
        }

        #region ColumnNamesSource

        public static readonly DependencyProperty ColumnNamesSourceProperty =
            DependencyProperty.Register("ColumnNamesSource", typeof (IEnumerable<string>), typeof (DynamicTreeListView),
                                        new PropertyMetadata(ColumnNamesSourceChanged));

        private readonly List<GridViewColumn> _dynamicColumns = new List<GridViewColumn>();

        public IEnumerable<string> ColumnNamesSource
        {
            get { return (IEnumerable<string>) GetValue(ColumnNamesSourceProperty); }
            set { SetValue(ColumnNamesSourceProperty, value); }
        }

        private void ColumnNamesSourceChanged()
        {
            Columns.RemoveItems(_dynamicColumns);
            _dynamicColumns.Clear();

            if (ColumnNamesSource == null || !ColumnNamesSource.Any())
                return;

            string[] names = ColumnNamesSource.ToArray();
            foreach (string name in names)
                _dynamicColumns.Add(CreateColumn(name));

            Columns.AddRange(_dynamicColumns);
        }

        private static void ColumnNamesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((DynamicTreeListView) d).ColumnNamesSourceChanged();
        }

        #endregion ColumnNamesSource
    }
}