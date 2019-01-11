using System.Windows.Browser;
using GazRouter.DataProviders;
using GazRouter.DataProviders.DispatcherTask;
using GazRouter.DTO.DispatcherTasks.Attachments;
using GazRouter.Modes.DispatcherTasks.Dialogs;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.Modes.DispatcherTasks.LPU
{
    public class AttachmentsLPUViewModel : RecordCommentsAndAttachmentsViewModelBase<TaskAttachmentDTO>
    {
        public AttachmentsLPUViewModel()
        {
            OpenAttachmentCommand = new DelegateCommand<TaskAttachmentDTO>(OnOpenAttachmentCommandExecuted);
        }

        public DelegateCommand<TaskAttachmentDTO> OpenAttachmentCommand { get; protected set; }

        public override string Header => "Вложенные файлы";

        public override async void ReloadData()
        {
            Items.Clear();
            SelectedItem = null;
            if (TaskRecord != null)
            {
                Behavior.TryLock();
                try
                {
                    var list = await  new DispatcherTaskServiceProxy().GetTaskAttachementListAsync(TaskRecord.TaskId);
                    GetListCallback(list);
                }
                finally
                {
                    Behavior.TryUnlock();
                }
            }
        }

        protected override void Add()
        {
            OMDialogHelper.AddFileAttachment(TaskRecord.TaskId, id => Refresh());
        }

        protected override void Edit()
        {
            OMDialogHelper.EditFileAttachment(SelectedItem, id => Refresh());
        }

        protected override void Delete()
        {
            OMDialogHelper.DeleteFileAttachment(SelectedItem.Id, Refresh);
        }

        private void OnOpenAttachmentCommandExecuted(TaskAttachmentDTO attachment)
        {
            HtmlPage.Window.Navigate(UriBuilder.GetBlobHandlerUri(attachment.BlobId), "_blank");
        }

        public override bool CanEdit()
        {
            return SelectedItem.CreateUserId == Application.UserProfile.Current.Id;
        }
    }
}