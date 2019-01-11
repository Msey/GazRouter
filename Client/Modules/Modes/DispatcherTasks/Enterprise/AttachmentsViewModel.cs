using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Browser;
using GazRouter.DataProviders.DispatcherTask;
using GazRouter.DTO.Dictionaries.StatusTypes;
using GazRouter.DTO.DispatcherTasks.Attachments;
using GazRouter.Modes.DispatcherTasks.Dialogs;
using Microsoft.Practices.Prism.Commands;
using UriBuilder = GazRouter.DataProviders.UriBuilder;
using GazRouter.DataProviders.Authorization;
using System.Linq;

namespace GazRouter.Modes.DispatcherTasks.PDS
{
    public class AttachmentsViewModel : TaskDetailsViewModelBase<TaskAttachmentDTO>
    {
        private TaskAttachmentDTO _selectedItem;

        private bool _isActive;
        private bool _loaded;

        public AttachmentsViewModel()
        {
            AddCommand = new DelegateCommand(Add,
                () =>
                    Status != null && Status.IsLastVersion &&
                    Status.StatusTypeId != StatusType.Annuled &&
                    Status.StatusTypeId != StatusType.Performed);

            DeleteCommand = new DelegateCommand(Delete,
                () =>
                    Status != null && Status.IsLastVersion &&
                    Status.StatusTypeId != StatusType.Annuled &&
                    Status.StatusTypeId != StatusType.Performed &&
                    SelectedItem != null);

            EditCommand = new DelegateCommand(Edit,
                () =>
                    Status != null && Status.IsLastVersion &&
                    Status.StatusTypeId != StatusType.Annuled &&
                    Status.StatusTypeId != StatusType.Performed &&
                    SelectedItem != null);

            OpenAttachmentCommand = new DelegateCommand<TaskAttachmentDTO>(OnOpenAttachmentCommandExecuted);
        }

        public override Visibility IsTabVisible => Visibility.Visible;

        public DelegateCommand<TaskAttachmentDTO> OpenAttachmentCommand { get; protected set; }

        public TaskAttachmentDTO SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaiseCommands();
                OnPropertyChanged(() => SelectedItem);
            }
        }

        public bool IsActive
        {
            set
            {
                if (_isActive == value)
                {
                    return;
                }
                _isActive = value;
                if (_loaded)
                {
                    return;
                }
                _loaded = true;
                Refresh();
            }
        }

        public override string Header => "Вложенные файлы";

        public override void Refresh()
        {
            ReloadData();
        }

        protected async void ReloadData()
        {
            Items.Clear();
            SelectedItem = null;
            if (Status != null)
            {
                Behavior.TryLock();
                try
                {
                    var users = await new UserManagementServiceProxy().GetUsersAsync();
                    var list = await new DispatcherTaskServiceProxy().GetTaskAttachementListAsync(Status.TaskId);

                    foreach (var item in list)
                        item.CreateUserDescription = users.FirstOrDefault((u) => u.Id == item.CreateUserId).Description;

                    GetListCallback(list, null);
                }
                finally
                {
                    Behavior.TryUnlock();
                }
            }
        }

        protected bool GetListCallback(List<TaskAttachmentDTO> list, Exception ex)
        {
            if (ex == null)
            {
                Items.Clear();
                foreach (var item in list)
                {
                    Items.Add(item);
                }

                OnPropertyChanged(() => Items);
            }
            return ex == null;
        }

        protected override void RaiseCommands()
        {
            AddCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }

        protected override void Add()
        {
            OMDialogHelper.AddFileAttachment(Status.TaskId, id => Refresh());
        }
        protected override void Edit()
        {
            OMDialogHelper.EditFileAttachment(SelectedItem, id => Refresh());
        }
        protected void Delete()
        {
            OMDialogHelper.DeleteFileAttachment(SelectedItem.Id, Refresh);
        }

        private static void OnOpenAttachmentCommandExecuted(TaskAttachmentDTO attachment)
        {
            HtmlPage.Window.Navigate(UriBuilder.GetBlobHandlerUri(attachment.BlobId), "_blank");
        }
    }
}