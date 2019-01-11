using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries;
using Telerik.Windows.Data;

namespace GazRouter.Controls.Dialogs.DictionaryPicker
{
    public class DictionaryPickerDialogViewModel : PickerDialogViewModelBase<BaseDictionaryDTO>
    {

        public DictionaryPickerDialogViewModel(Action closeCallback, IEnumerable<BaseDictionaryDTO> listOfItem)
            : base(closeCallback)
        {
        	List = listOfItem;
            Init();
        }

        private IEnumerable<BaseDictionaryDTO> List { get; set; }

        protected override void OnItemsLoading(object sender, VirtualQueryableCollectionViewItemsLoadingEventArgs args)
		{
            base.OnItemsLoading(sender, args);
            Behavior.TryLock();
            var res = string.IsNullOrEmpty(NamePart) ? List :
				List.Where(p => p.Name.IndexOf(NamePart, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
			if (res.Count() != Items.VirtualItemCount)
			{
				Items.VirtualItemCount = res.Count();
			}
			Items.ItemsLoaded += BindingListItemsLoaded;
			Items.Load(args.StartIndex, res);
		}

		private void BindingListItemsLoaded(object sender, VirtualQueryableCollectionViewItemsLoadedEventArgs e)
		{
			Items.ItemsLoaded -= BindingListItemsLoaded;
			Items.Refresh();
            Behavior.TryUnlock();
		}

        public override int PageSize
        {
            get { return 10; }
        }

        protected override void ApplyFilter()
        {
            CreateItems();
        }
    }
}