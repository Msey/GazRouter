using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;

namespace GazRouter.Common.Ui.Behaviors
{
    public class RadGridMultiSelectBehavior : Behavior<RadGridView>
    {
        private RadGridView Grid
        {
            get
            {
                return AssociatedObject;
            }
        }

        public INotifyCollectionChanged SelectedItems
        {
            get { return (INotifyCollectionChanged)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(INotifyCollectionChanged), typeof(RadGridMultiSelectBehavior), new PropertyMetadata(OnSelectedItemsPropertyChanged));


        private static void OnSelectedItemsPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            var collection = args.NewValue as INotifyCollectionChanged;
            if (collection != null)
            {
                collection.CollectionChanged += ((RadGridMultiSelectBehavior)target).ContextSelectedItemsCollectionChanged;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            Grid.SelectedItems.CollectionChanged += GridSelectedItemsCollectionChanged;
        }

        void ContextSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UnsubscribeFromEvents();

            Transfer(SelectedItems as IList, Grid.SelectedItems);

            SubscribeToEvents();
        }

        void GridSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            UnsubscribeFromEvents();

            Transfer(Grid.SelectedItems, SelectedItems as IList);

            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            Grid.SelectedItems.CollectionChanged += GridSelectedItemsCollectionChanged;

            if (SelectedItems != null)
            {
                SelectedItems.CollectionChanged += ContextSelectedItemsCollectionChanged;
            }
        }

        private void UnsubscribeFromEvents()
        {
            Grid.SelectedItems.CollectionChanged -= GridSelectedItemsCollectionChanged;

            if (SelectedItems != null)
            {
                SelectedItems.CollectionChanged -= ContextSelectedItemsCollectionChanged;
            }
        }

        public static void Transfer(IList source, IList target)
        {
            if (source == null || target == null)
                return;

            target.Clear();

            foreach (var o in source)
            {
                target.Add(o);
            }
        }
    }
}
