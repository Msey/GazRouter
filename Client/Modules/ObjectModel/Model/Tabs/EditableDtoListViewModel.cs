using System;
using System.ServiceModel;
using GazRouter.Common;
using GazRouter.DTO;
using GazRouter.DTO.Infrastructure.Faults;
using Telerik.Windows.Controls;

namespace GazRouter.ObjectModel.Model.Tabs
{
    public abstract class EditableDtoListViewModel<TItem, TId> : DtoListViewModelBase<TItem, TId>
        where TItem : BaseDto<TId>
        where TId : struct
    {
        protected EditableDtoListViewModel()
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.ObjectModel);
            // 
            AddCommand = new DelegateCommand(Add, obj => editPermission);
            EditCommand = new DelegateCommand(Edit, obj => SelectedItem != null && 
                                                    editPermission);
            RemoveCommand = new DelegateCommand(Remove, obj => SelectedItem != null && 
                                                        editPermission);
        }

        private void Remove(object obj)
        {
            Remove();
        }

        private void Edit(object obj)
        {
            Edit();
        }

        private void Add(object obj)
        {
            Add();
        }

        public DelegateCommand AddCommand { get; }
        public DelegateCommand EditCommand { get; }
        public DelegateCommand RemoveCommand { get; }

        protected abstract void Add();
        protected abstract void Edit();
        protected abstract void Delete();

        protected override void RaiseCommands()
        {
            AddCommand.InvalidateCanExecute();
            EditCommand.InvalidateCanExecute();
            RemoveCommand.InvalidateCanExecute();
        }

        private void Remove()
        {
          MessageBoxProvider.Confirm("Удалить объект?", confirmed =>
            {
                if (confirmed)
                {
                    Delete();
                }
            }, "Подтверждение удаления");
        }

        protected bool DeleteCallback(Exception ex)
        {
            if (ex == null)
            {
                Refresh();
                return true;
            }

            var faultException = ex as FaultException<FaultDetail>;
            if (faultException != null && faultException.Detail.FaultType == FaultType.IntegrityConstraint)
            {
                MessageBoxProvider.Alert(faultException.Detail.Message, "Ошибка");
                return true;
            }
            return false;
        }
    }
}