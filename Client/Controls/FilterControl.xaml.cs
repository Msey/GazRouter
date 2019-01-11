using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Data;

namespace GazRouter.Controls
{
    public partial class FilterControl: IFilteringControl
    {
		public FilterControl()
        {
            InitializeComponent();
        }

		private GridViewBoundColumnBase _column;
        private FilterDescriptor _filter;

        /// <summary>
        /// Gets or sets a value indicating whether the filtering is active.
        /// </summary>
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="IsActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(
                "IsActive",
                typeof(bool),
				typeof(FilterControl),
                new PropertyMetadata(false));

        public string Value
        {
            get { return findtext.Text; }
            set { findtext.Text = value; }
        }


        public TextBox FindTextBox
        {
            get { return findtext; }
        }

        public void Prepare(GridViewColumn column)
        {
			_column = column as GridViewBoundColumnBase;
			if (_column == null)
			{
				return;
			}

			if (_filter == null)
            {
                CreateFilters();
            }

			_filter.Value = Value;
        }

        private void CreateFilters()
        {
            string dataMember = _column.DataMemberBinding.Path.Path;
            
            _filter = new FilterDescriptor(dataMember
                , FilterOperator.Contains
                , null,false,typeof(string));
        }

        private void OnFilter(object sender, RoutedEventArgs e)
        {
            _filter.Value = Value;

			if (!_column.DataControl.FilterDescriptors.Contains(_filter))
            {
				_column.DataControl.FilterDescriptors.Add(_filter);
            }
            
            IsActive = true;            
        }
        
        private void OnClear(object sender, RoutedEventArgs e)
        {
			if (_column.DataControl.FilterDescriptors.Contains(_filter))
            {
				_column.DataControl.FilterDescriptors.Remove(_filter);
            }
			Value = string.Empty;
			IsActive = false;
        }

        private void findtext_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            _filter.Value = Value;

            if (!_column.DataControl.FilterDescriptors.Contains(_filter))
            {
                _column.DataControl.FilterDescriptors.Add(_filter);
            }

            IsActive = true;
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            FilteringDropDown down = this.ParentOfType<FilteringDropDown>();
            if (down != null)
            {
                down.IsDropDownOpen = false;
            }
        }
    }
}
