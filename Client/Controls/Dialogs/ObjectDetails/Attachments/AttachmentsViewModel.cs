using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Browser;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Attachment;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Attachments;
using Microsoft.Practices.Prism.Commands;


namespace GazRouter.Controls.Dialogs.ObjectDetails.Attachments
{
    public class AttachmentsViewModel : ValidationViewModel
    {
        private Guid? _entityId;
        private bool _isActive;
        private bool _isReadOnly;
        private AttachmentDTO<int, Guid> _selectedAttachment;

        public AttachmentsViewModel()
        {
            OpenAttachmentCommand = new DelegateCommand<object>(OpenAttachment);

            RefreshCommand = new DelegateCommand(Refresh);
            AddCommand = new DelegateCommand(Add, () => _entityId.HasValue);
            RemoveCommand = new DelegateCommand(Remove, () => _selectedAttachment != null);


            Refresh();
        }

        /// <summary>
        /// »дентификатор объекта, дл€ которого отображаютс€ прикрепленные документы
        /// </summary>
        public Guid? EntityId
        {
            get { return _entityId;}
            set
            {
                if(SetProperty(ref _entityId, value))
                {
                    Refresh();
                    AddCommand.RaiseCanExecuteChanged();
                }
            }
        }

        /// <summary>
        /// ≈сли установлен в True, то при каждом изменении EntityId обновл€етс€ список вложений
        /// Ёто сделано дл€ того, чтобы грузить данные только в том случае когда вкладка с вложени€ми активна
        /// </summary>
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (SetProperty(ref _isActive, value))
                {
                    Refresh();
                }
            }
        }

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { SetProperty(ref _isReadOnly, value); }
        }

        public List<AttachmentDTO<int, Guid>> AttachmentList { get; set; }

        public AttachmentDTO<int, Guid> SelectedAttachment
        {
            get { return _selectedAttachment; }
            set
            {
                if(SetProperty(ref _selectedAttachment, value))
                    RemoveCommand.RaiseCanExecuteChanged();
            }
        }

        public bool HasAttachments
        {
            get { return AttachmentList != null && AttachmentList.Any(); }
        }

        public DelegateCommand<object> OpenAttachmentCommand { get; set; }

        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand RemoveCommand { get; set; }


        private async void Refresh()
        {
            if (!_isActive) return;
            try
            {
                Behavior.TryLock();

                AttachmentList = _entityId.HasValue
                    ? await new ObjectModelServiceProxy().GetEntityAttachmentListAsync(_entityId)
                    : new List<AttachmentDTO<int, Guid>>();
                
                
                OnPropertyChanged(() => AttachmentList);
                OnPropertyChanged(() => HasAttachments);
            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }

        private void OpenAttachment(object x)
        {
            var blobId = x as Guid?;
            if(blobId.HasValue)
                HtmlPage.Window.Navigate(DataProviders.UriBuilder.GetBlobHandlerUri(blobId.Value), "_blank");

        }

        private void Add()
        {
            if (!_entityId.HasValue) return;

            var vm = new AddEditAttachmentViewModel(async obj =>
            {
                var x = (AddEditAttachmentViewModel)obj;
                if (x.DialogResult.HasValue && x.DialogResult.Value)
                {
                    await new ObjectModelServiceProxy().AddEntityAttachmentAsync(
                        new AddAttachmentParameterSet<Guid>
                        {
                            Description = x.Description,
                            Data = x.FileData,
                            FileName = x.FileName,
                            ExternalId = _entityId.Value
                        });
                    Refresh();
                }
            });
            var v = new AddEditAttachmentView { DataContext = vm };
            v.ShowDialog();
        }


        private async void Remove()
        {
            if (_selectedAttachment == null) return;
            await new ObjectModelServiceProxy().RemoveEntityAttachmentAsync(_selectedAttachment.Id);
            Refresh();
        }
    }
}