using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GazRouter.Controls.Dialogs;
using GazRouter.Controls.Dialogs.DictionaryPicker;
using GazRouter.DTO.Dictionaries;

namespace GazRouter.Controls
{
    public partial class DictionaryPicker
    {
        public DictionaryPicker()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty SelectedItemProperty =
    DependencyProperty.Register("SelectedItemProperty", typeof(BaseDictionaryDTO), typeof(DictionaryPicker),
                                new PropertyMetadata(null, OnSelectedItemPropertyChanged));

        public static readonly DependencyProperty SelectedItemIdProperty =
 DependencyProperty.Register("SelectedItemIdProperty", typeof(int?), typeof(DictionaryPicker),
                             new PropertyMetadata(null, OnSelectedItemIdPropertyChanged));

        private static void OnSelectedItemIdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DictionaryPicker)d;
            control.SelectItemById((int?)e.NewValue);
        }

        private void SelectItemById(int? id)
        {
             if (id.HasValue)
            {
                if (SelectedItem != null && SelectedItem.Id == id)
                {
                    return;
                }
                SelectedItem = Dictionary.FirstOrDefault(p => p.Id == id);
            }
            else
            {
                SelectedItem = null;
            }
        }

        public BaseDictionaryDTO SelectedItem
        {
            get { return (BaseDictionaryDTO)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

		public IEnumerable<BaseDictionaryDTO> Dictionary
		{
            get { return (IEnumerable<BaseDictionaryDTO>)GetValue(DictionaryProperty); }
			set { SetValue(DictionaryProperty, value); }
		}

		public static readonly DependencyProperty DictionaryProperty =
            DependencyProperty.Register("Dictionary", typeof(IEnumerable<BaseDictionaryDTO>), typeof(DictionaryPicker), new PropertyMetadata(null, OnDictionaryChanged));

    	private static void OnDictionaryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    	{
    	}

        public int? SelectedItemId
        {
            get { return (int?)GetValue(SelectedItemIdProperty); }
            set { SetValue(SelectedItemIdProperty, value); }
        }

        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (DictionaryPicker)d;
            if (e.NewValue != null)
            {
                var dto = ((BaseDictionaryDTO) e.NewValue);
                control.txtName.Text = dto.Name;
                control.SelectedItemId = dto.Id;
            }
            else
            {
                control.txtName.Text = string.Empty;
                control.SelectedItemId = default(int);
            }
        }

        private DictionaryPickerDialogViewModel _viewModel;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
        	_viewModel = DialogHelper.ShowDictionaryPicker(CloseCallback, Dictionary);
        }

        private void CloseCallback()
        {
            if (_viewModel.DialogResult ?? false)
            {
                SelectedItem = _viewModel.SelectedItem;
            }
        }
    }
}
