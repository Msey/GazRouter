using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GazRouter.Application;
using GazRouter.Application.Wrappers;
using GazRouter.Common;
using GazRouter.Controls.Attachment;
using GazRouter.DataProviders.EventLog;
using GazRouter.DTO.EventLog;
using GazRouter.DTO.EventLog.Attachments;
using GazRouter.Modes.EventLog.Dialogs;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Data.PropertyGrid;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using System.Threading.Tasks;

namespace GazRouter.Modes.EventLog
{
    public class EventAttachmentViewModel : EventTabViewModelBase
    {
        private List<AttachmentWrapper> _allItems = new List<AttachmentWrapper>();
        public Action Quote;

        public EventAttachmentViewModel()
        {
            IsEditPermission = Authorization2.Inst.IsEditable(LinkType.Log);
            _filter =
                IsolatedStorageManager.Get<CommentAndAttachmentFilterEnum?>("EventLogCommentAndAttachmentFilter") ??
                CommentAndAttachmentFilterEnum.All;
            
            AddCommand = new DelegateCommand<string>(Add, dtoType => Type == EventListType.List);

            EditCommand = new DelegateCommand(Edit,

                () => SelectedItem != null && Type == EventListType.List && CheckCanUseCommand());
            DeleteCommand = new DelegateCommand(Remove,
                () => SelectedItem != null && Type == EventListType.List && CheckCanUseCommand());
        }

        #region Commands

        public DelegateCommand<string> AddCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }
        public DelegateCommand SelectCommand { get; set; }
        public DelegateCommand<AttachmentWrapper> OpenAttachmentCommand { get; set; }

        private bool CheckCanUseCommand()
        {
            if (SelectedItem != null)
                return (DateTime.Now - SelectedItem.Dto.CreateDate) <= TimeSpan.FromMinutes(15) &&
                       (UserProfile.Current.Site.IsEnterprise || SelectedItem.Dto.SiteId == UserProfile.Current.Site.Id);
            return false;
        }

        private void Remove()
        {
            MessageBoxProvider.Confirm("Удалить объект?", b =>
            {
                if (b)
                    Delete();
            }, "Подтверждение удаления");
        }


        protected void RaiseCommands()
        {
            AddCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }

        private void Add(string type)
        {
            if (type == "Comment")
            {
                var viewModel = new AddEventCommentViewModel(EventDTO.Id, id => RefreshAfterAdd());
                var view = new AddEventCommentView { DataContext = viewModel };
                view.ShowDialog();
            }
            else
            {
                var vm = new AddEditAttachmentViewModel(async obj =>
                {
                    var x = (AddEditAttachmentViewModel)obj;
                    if (x.DialogResult.HasValue && x.DialogResult.Value)
                    {
                        await new EventLogServiceProxy().AddEventAttachmentAsync(
                            new AddEventAttachmentParameterSet
                            {
                                Description = x.Description,
                                Data = x.FileData,
                                FileName = x.FileName,
                                EventId = EventDTO.Id
                            });
                        RefreshAfterAdd();
                    }
                });
                var v = new AddEditAttachmentView { DataContext = vm };
                v.ShowDialog();
            }
        }

        private void Edit()
        {
            if (SelectedItem.IsComment)
            {
                var viewModel = new AddEventCommentViewModel(SelectedItem.Dto, id => RefreshAfterAdd());
                var view = new AddEventCommentView { DataContext = viewModel };
                view.ShowDialog();
            }
            else
            {
                var vm = new AddEditAttachmentViewModel(async obj =>
                {
                    var x = (AddEditAttachmentViewModel)obj;
                    if (x.DialogResult.HasValue && x.DialogResult.Value)
                    {
                        await new EventLogServiceProxy().EditEventAttachmentAsync(
                            new EditEventAttachmentParameterSet
                            {
                                EventAttachmentId = SelectedItem.Dto.Id,
                                Description = x.Description,
                                Data = x.FileData,
                                FileName = x.FileName
                            });
                        RefreshAfterAdd();
                    }
                }, SelectedItem.Dto);
                var v = new AddEditAttachmentView { DataContext = vm };
                v.ShowDialog();
            }
        }
    

        private async void Delete()
        {
            try
            {
                Behavior.TryLock();
                await new EventLogServiceProxy().DeleteEventAttachementAsync(SelectedItem.Dto.Id);
                Refresh(true);
            }
            finally
            {
                Behavior.TryUnlock();
            }
            
        }


        #endregion


        private bool _isEditPermission;

        public bool IsEditPermission
        {
            get { return _isEditPermission; }
            set
            {
                _isEditPermission = value;
                OnPropertyChanged(() => IsEditPermission);
            }
        }


        #region FilterItems

        public IEnumerable<KeyValuePair<string, CommentAndAttachmentFilterEnum>> FilterList
        {
            get
            {
                yield return new KeyValuePair<string, CommentAndAttachmentFilterEnum>("Все", CommentAndAttachmentFilterEnum.All);
                yield return new KeyValuePair<string, CommentAndAttachmentFilterEnum>("Моё подразделение", CommentAndAttachmentFilterEnum.Division);
                yield return new KeyValuePair<string, CommentAndAttachmentFilterEnum>("Только мои", CommentAndAttachmentFilterEnum.My);
            }
        }


        private CommentAndAttachmentFilterEnum _filter;
        public CommentAndAttachmentFilterEnum Filter
        {
            get { return _filter; }
            set
            {
                if (SetProperty(ref _filter, value))
                {
                    IsolatedStorageManager.Set("EventLogCommentAndAttachmentFilter", value);
                    OnPropertyChanged(() => Items);
                }
            }
        }

        #endregion


        public List<AttachmentWrapper> Items
        {
            get
            {
                switch (Filter)
                {
                    case CommentAndAttachmentFilterEnum.Division:
                        return _allItems.Where(c => c.Dto.SiteId == UserProfile.Current.Site.Id).ToList();
                        
                    case CommentAndAttachmentFilterEnum.My:
                        return _allItems.Where(c => c.Dto.UserId == UserProfile.Current.Id).ToList();
                        
                    default:
                        return _allItems;
                }
            }
        }


        
        private AttachmentWrapper _selectedItem;
        public AttachmentWrapper SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                    RaiseCommands();
            }
        }

       





        #region Refresh

        protected override void Refresh(bool refreshParent = false)
        {
            if (refreshParent && ReloadParent != null)
            {
                ReloadParent.Invoke();
            }
            else
            {
                LoadData();
            }
        }

        protected void RefreshAfterAdd()
        {
            if (EventDTO.IsQuote)
            {
                Refresh(true);
            }
            else
            {
                Quote();
            }
        }


        protected override async void LoadData()
        {
            if (EventDTO == null) return;
            try
            {
                Behavior.TryLock();
                _allItems =
                    (await new EventLogServiceProxy().GetEventAttachmentListAsync(EventDTO.Id)).Select(
                        a => new AttachmentWrapper(a)).ToList();
                OnPropertyChanged(() => Items);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        public async Task<List<EventAttachmentDTO>> LoadData(EventDTO dto)
        {
            return await new EventLogServiceProxy().GetEventAttachmentListAsync(dto.Id);
        }

        #endregion

        public override string Header
        {
            get { return "Примечания и вложения"; }
        }

        public Visibility MainMenu
        {
            get { return Type == EventListType.List ? Visibility.Visible : Visibility.Collapsed; }
        }
    }

    #region Wrapper

    public class AttachmentWrapper : DtoWrapperBase<EventAttachmentDTO>
    {
        public AttachmentWrapper(EventAttachmentDTO dto) : base(dto) {}

        public bool IsComment => string.IsNullOrEmpty(Dto.FileName);

        public bool IsAttachment => !string.IsNullOrEmpty(Dto.FileName);
    }

    #endregion
}