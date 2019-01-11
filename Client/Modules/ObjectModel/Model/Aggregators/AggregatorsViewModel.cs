using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Dialogs.ObjectDetails.Measurings;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Aggregators;
using GazRouter.ObjectModel.Model.Dialogs.AddEdit.Objects.Aggregator;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.ObjectModel.Model.Aggregators
{
    public class AggregatorsViewModel : LockableViewModel
    {
        private ItemBase _selectedItem;
        public bool EditPermission { get; set; }

        public AggregatorsViewModel()
        {
            EditPermission = Authorization2.Inst.IsEditable(LinkType.ObjectModel);



            RefreshCommand = new DelegateCommand(Refresh);

            AddCommand = new DelegateCommand(Add, () => EditPermission);
            EditCommand = new DelegateCommand(Edit, () => EditPermission && (SelectedItem?.EditAllowed ?? false));
            DeleteCommand = new DelegateCommand(Delete, () => EditPermission && (SelectedItem?.EditAllowed ?? false));

            Measurings = new MeasuringsViewModel {ShowTrend = true};

            Refresh();

        }


        public DelegateCommand RefreshCommand { get; set; }

        public DelegateCommand AddCommand { get; set; }

        public DelegateCommand EditCommand { get; set; }

        public DelegateCommand DeleteCommand { get; set; }

        private void UpdateCommands()
        {
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }



        public List<ItemBase> Items { get; set; }

        public ItemBase SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    var aggr = value as AggrItem;
                    if (aggr != null)
                        Measurings.SetEntity(aggr.Dto.Id, aggr.Dto.EntityType);
                    OnPropertyChanged(() => IsAggrSelected);
                    UpdateCommands();
                }
            }
        }

        public bool IsAggrSelected => SelectedItem is AggrItem;

        private async void Refresh()
        {
            Lock();

            var aggrList = await new ObjectModelServiceProxy().GetAggregatorListAsync(null);

            Items = new List<ItemBase>();
            foreach (var type in aggrList.GroupBy(a => a.AggregatorTypeName))
            {
                var typeItem = new TypeItem
                {
                    Name = type.Key,
                    Childs = type.Select(a => new AggrItem(a)).ToList()
                };
                Items.Add(typeItem);
            }

            OnPropertyChanged(() => Items);

            Unlock();
        }


        public MeasuringsViewModel Measurings { get; set; }



        private void Add()
        {
            var vm = new AddEditAggregatorViewModel((x) => Refresh());
            var v = new AddEditAggregatorView {DataContext = vm};
            v.ShowDialog();
        }

        private void Edit()
        {
            var aggr = SelectedItem as AggrItem;
            if (aggr != null)
            {
                var vm = new AddEditAggregatorViewModel((x) => Refresh(), aggr.Dto);
                var v = new AddEditAggregatorView {DataContext = vm};
                v.ShowDialog();
            }
        }

        private async void Delete()
        {
            var aggr = SelectedItem as AggrItem;
            if (aggr != null)
            {
                await new ObjectModelServiceProxy().DeleteEntityAsync(
                    new DeleteEntityParameterSet
                    {
                        Id = aggr.Dto.Id,
                        EntityType = aggr.Dto.EntityType
                    });
                Refresh();
            }
        }


    }

    public class ItemBase
    {
        public virtual string Name { get; set; }

        public virtual ImageSource Image { get; set; }

        public virtual bool EditAllowed => false;
    }

    public class AggrItem : ItemBase
    {
        public AggregatorDTO Dto { get; set; }

        public AggrItem(AggregatorDTO dto)
        {
            Dto = dto;
        }

        public override string Name => Dto.Name;

        public override ImageSource Image => new BitmapImage(new Uri("/Common;component/Images/16x16/calculate.png", UriKind.Relative));

        public override bool EditAllowed => !Dto.IsSystem;
    }

    public class TypeItem : ItemBase
    {
        public override ImageSource Image => new BitmapImage(new Uri("/Common;component/Images/16x16/folder.png", UriKind.Relative));

        public List<AggrItem> Childs { get; set; }
    }
}