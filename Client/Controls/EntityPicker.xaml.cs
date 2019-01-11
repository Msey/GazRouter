using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GazRouter.Common;
using GazRouter.Controls.Dialogs;
using GazRouter.Controls.Dialogs.EntityPicker;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Entities;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows;
using PropertyMetadata = System.Windows.PropertyMetadata;

namespace GazRouter.Controls
{
    public partial class EntityPicker
    {
        public EntityPicker()
        {
            InitializeComponent();
            _selectFavorites = new DelegateCommand<Guid?>(SelectFavorite);
            LoadFavorites();
        }


        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItemProperty", typeof (CommonEntityDTO), typeof (EntityPicker),
                new PropertyMetadata(null, OnSelectedItemPropertyChanged));


        public static readonly DependencyProperty SelectedItemIdProperty =
            DependencyProperty.Register("SelectedItemIdProperty", typeof (Guid?), typeof (EntityPicker),
                new PropertyMetadata(null, OnSelectedItemIdPropertyChanged));

        private static void OnSelectedItemIdPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (EntityPicker) d;
            control.SelectItemById((Guid?) e.NewValue);
        }

        private void SelectItemById(Guid? id)
        {
            if (!id.HasValue)
            {
                SelectedItem = null;
            }
            else
            {
                if (SelectedItem == null || SelectedItem.Id != id)
                {
                    SelectEntityById(id.Value);
                }
            }
        }

        private async void SelectEntityById(Guid id)
        {
            var entityDto = await new ObjectModelServiceProxy().GetEntityByIdAsync(id);
            SelectedItem = entityDto;
        }

        public CommonEntityDTO SelectedItem
        {
            get { return (CommonEntityDTO) GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public Guid? SelectedItemId
        {
            get { return (Guid?) GetValue(SelectedItemIdProperty); }
            set { SetValue(SelectedItemIdProperty, value); }
        }

        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (EntityPicker) d;
            control.OnSelectedItemPropertyChanged((CommonEntityDTO) e.NewValue);
        }

        private void OnSelectedItemPropertyChanged(CommonEntityDTO newValue)
        {
            if (newValue != null)
            {
                var dto = newValue;
                txtName.Text = dto.ShortPath;
                SelectedItemId = dto.Id;
                SaveFavorites(dto.Id);
            }
            else
            {
                txtName.Text = string.Empty;
                SelectedItemId = null;
            }
            UpdateBtnDeleteVisibility();
        }

        private EntityPickerDialogViewModel _viewModel;

        private void SearchClick(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
                return;
            _viewModel = DialogHelper.ShowEntityPicker(CloseCallback, AllowedTypes.ToList());
        }

        private void Button_Delete_Click(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
                return;
            SelectedItem = null;
        }

        private void CloseCallback()
        {
            if (_viewModel.DialogResult ?? false)
            {
                SelectedItem = _viewModel.SelectedItem;
                SaveFavorites(SelectedItem.Id);
                FavoritesItems.Add(SelectedItem);
                IsFavoritesItemsChanged();
            }
        }


        public List<EntityType> AllowedTypes
        {
            get { return (List<EntityType>) GetValue(AllowedTypesProperty); }
            set { SetValue(AllowedTypesProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool) GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static readonly DependencyProperty AllowedTypesProperty =
            DependencyProperty.Register("AllowedTypesProperty", typeof (List<EntityType>), typeof (EntityPicker),
                new PropertyMetadata(new List<EntityType>(), OnAllowedTypesPropertyChanged));

        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly",
            typeof (bool), typeof (EntityPicker), new PropertyMetadata(default(bool), ISReadOnlyChangedCallback));

        public static readonly DependencyProperty CanUserDeleteProperty = DependencyProperty.Register("CanUserDelete",
            typeof (bool), typeof (EntityPicker), new PropertyMetadata(default(bool), CanUserDeleteChangedCallback));

        private static void CanUserDeleteChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((EntityPicker) dependencyObject).CanUserDeleteChanged();
        }

        private void CanUserDeleteChanged()
        {
            UpdateBtnDeleteVisibility();
        }

        private void UpdateBtnDeleteVisibility()
        {
            btnDelete.Visibility = CanUserDelete && SelectedItem != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool CanUserDelete
        {
            get { return (bool) GetValue(CanUserDeleteProperty); }
            set { SetValue(CanUserDeleteProperty, value); }
        }

        private static void ISReadOnlyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((EntityPicker) dependencyObject).IsReadOnlyChanged();
        }

        private void IsReadOnlyChanged()
        {
            btnSelect.Visibility = IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
        }

        private static void OnAllowedTypesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EntityPicker) d).LoadFavorites();
        }

        public ObservableCollection<CommonEntityDTO> FavoritesItems { get; protected set; }

        private async void LoadFavorites()
        {
            var favorites = await new ObjectModelServiceProxy().GetEntityListAsync(
                new GetEntityListParameterSet
                {
                    EntityIdList = IsolatedStorageManager.FavoritesList
                });
            FavoritesItems =
                new ObservableCollection<CommonEntityDTO>(
                    favorites.Where(
                        t => IsolatedStorageManager.FavoritesList.Contains(t.Id) && AllowedTypes.Contains(t.EntityType)));
            IsFavoritesItemsChanged();
        }

        private void SaveFavorites(Guid id)
        {
            var list = IsolatedStorageManager.FavoritesList.ToList();
            if (list.Contains(id))
            {
                list.Remove(id);
            }
            list.Insert(0, id);
            if (list.Count > 25)
                while (list.Count > 25)
                    list.RemoveAt(25);
            IsolatedStorageManager.FavoritesList = list;
        }

        public DelegateCommand<Guid?> SelectFavorites
        {
            get { return _selectFavorites; }
        }

        private readonly DelegateCommand<Guid?> _selectFavorites;

        private void SelectFavorite(Guid? obj)
        {
            if (obj.HasValue)
            {
                SelectedItem = FavoritesItems.FirstOrDefault(t => t.Id == obj.Value);
                RadContextMenuMenu.IsOpen = false;
            }
        }

        private void IsFavoritesItemsChanged()
        {
            FavoritesMenuMenu.Visibility = FavoritesItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TreeSearchClick(object sender, RadRoutedEventArgs e)
        {
            if (IsReadOnly)
                return;
            DialogHelper.ShowTreeEntityPicker(TreeCloseCallback, AllowedTypes.ToList());
        }

        private void TreeCloseCallback(CommonEntityDTO entity)
        {
            SelectedItem = entity;
            SaveFavorites(SelectedItem.Id);
            FavoritesItems.Add(SelectedItem);
            IsFavoritesItemsChanged();
        }
    }
}