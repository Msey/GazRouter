using GazRouter.Common.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DataExchange.Integro.ASSPOOTI
{
    public class MappingEntityTreeItem : PropertyChangedBase
    {
        private LockableViewModel _viewModel;

        public MappingEntityTreeItem(LockableViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public ICollection<MappingEntityTreeItem> Childrens { get; set; }

        public string Name { get; set; }

        public Guid Id { get; set; }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    if (value)
                    {
                        _viewModel.Behavior.TryLock();
                        MappingHelper.LoadTreeItems(Id, new List<int> { 20, 21, 22 }, _viewModel).ContinueWith(x => {
                            Childrens = x.Result;
                            _viewModel.Behavior.TryUnlock();
                        });
                        //Deployment.Current.Dispatcher.InvokeAsync(async () =>
                        //{
                        //    Childrens = await MappingHelper.LoadTreeItems(Id, new List<int> { 20, 21, 22 });
                        //});
                    }
                    OnPropertyChanged(() => IsExpanded);
                }
            }
        }
    }
}
