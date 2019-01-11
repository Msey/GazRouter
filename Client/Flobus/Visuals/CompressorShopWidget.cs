using System;
using System.Windows;
using System.Windows.Data;
using GazRouter.Controls.Dialogs.ObjectDetails;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.Flobus.FloScheme;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.Misc;
using GazRouter.Flobus.UiEntities.FloModel;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Flobus.Visuals
{
    public sealed class CompressorShopWidget : ShapeWidgetBase, ISupportContextMenu
    {
        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
            nameof(Caption), typeof(string), typeof(CompressorShopWidget), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty IdProperty = DependencyProperty.Register(
            nameof(Id), typeof(Guid), typeof(CompressorShopWidget), new PropertyMetadata(default(Guid)));

        private const string PartContextMenu = "ContextMenu";

        private RadContextMenu _contextMenu;

        internal CompressorShopWidget(Schema schema) : base(schema)
        {
            CreateBindings();
        }

        public string Caption
        {
            get { return (string) GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public Guid Id
        {
            get { return (Guid) GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public override void Select()
        {
            VisualStateManager.GoToState(this, VisualStates.Selected, false);
            Schema.SetSelectedWidget(this);
        }

        public override void Update()
        {
        }

        public void FillMenu(RadContextMenu menu, Point mousePosition, Schema schema)
        {
            if (!Schema.IsReadOnly)
            {
                menu.AddCommand("Показать в дереве", new DelegateCommand(
                    () => schema.GotoTreeCommand.Execute(Id),
                    () => schema.GotoTreeCommand != null && schema.GotoTreeCommand.CanExecute(Id)));

                menu.AddCommand("Удалить", new DelegateCommand(
                    () => RadWindow.Confirm(new DialogParameters
                    {
                        Header = "Внимание!",
                        Content = @"Вы уверены, что хотите удалить КC со схемы?",
                        Closed = (sender, e1) =>
                        {
                            if (e1.DialogResult.HasValue && e1.DialogResult.Value)
                            {
                                Schema.RemoveCompressorShop(this);
                            }
                        }
                    })));
            }
            menu.AddCommand("Паспорт...", new DelegateCommand(() =>
            {
                new ObjectDetailsView {DataContext = new ObjectDetailsViewModel(Id, EntityType.CompShop)}
                    .ShowDialog();
            }));


            if (Schema.IsRepair)
            {

                menu.AddCommand("Запланировать ремонт...",
                new DelegateCommand(
                    () =>
                    {
                        Schema.InvokeDialogWindowCall(Id);
                    }));
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_contextMenu != null)
            {
                _contextMenu.Opened -= ContextMenuOnOpened;
            }
            _contextMenu = GetTemplateChild(PartContextMenu) as RadContextMenu;

            if (_contextMenu != null)
            {
                _contextMenu.Opened += ContextMenuOnOpened;
            }

            Update();
        }

        protected override void CreateBindings()
        {
            base.CreateBindings();
      /*      SetBinding(IdProperty, new Binding(nameof(ICompressorShop.Id)));
            SetBinding(CaptionProperty, new Binding(nameof(ICompressorShop.ShortPath)));
            SetBinding(IsFoundProperty, new Binding(nameof(ISearchable.IsFound)) {Mode = BindingMode.OneWay});
      */      SetBinding(ContentProperty, new Binding(nameof(ICompressorShop.Data)) {Mode = BindingMode.OneWay});
        }

        private void ContextMenuOnOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            //  Select();
        }
    }
}