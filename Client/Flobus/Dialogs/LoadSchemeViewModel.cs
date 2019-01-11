using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Appearance;
using GazRouter.DTO.Appearance.Versions;
using Microsoft.Practices.Prism;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Flobus.Dialogs
{
    public class LoadSchemeViewModel : DialogViewModel<Action<SchemeVersionItemDTO>>
    {
        private AddSchemaViewModel _model;
        private SchemeParameterSet _newItem;
        private SchemeVersionItemDTO _selectedItem;

        public LoadSchemeViewModel(Action<SchemeVersionItemDTO> closeCallback, bool editMode)
            : base(closeCallback)
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.ObjectModel);
            //
            EditMode = editMode;
            CommentCommand = new DelegateCommand(CommentCommandExecute, 
                                                 ()=> CommentCommandCanExecute() && editPermission);
            DeleteCommand = new DelegateCommand(Delete, () => SelectedItem != null && editPermission);
            AddCommand = new DelegateCommand(AddCommandExecute, () => editPermission);
            PublishSchemeVersionCommand = new DelegateCommand(() => PublishSchemeVersionCommandExecute(false),
                    () => SelectedItem != null && !SelectedItem.IsPublished && editPermission);
            UnPublishSchemeVersionCommand = new DelegateCommand(() => PublishSchemeVersionCommandExecute(true),
                    () => SelectedItem != null && SelectedItem.IsPublished && editPermission);
            LoadCommand = new DelegateCommand(() => { DialogResult = true; }, () => SelectedItem != null);
            NewItem = new SchemeParameterSet();
            Load();
        }

        public event EventHandler<SchemeVersionDeletedEventArgs> SchemeVersionDeleted;

        public bool EditMode { get; }

        public SchemeVersionItemDTO SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    RaiseCanExecuteCommands();
                }
            }
        }

        public SchemeParameterSet NewItem
        {
            get { return _newItem; }
            set
            {
                if (Equals(value, _newItem))
                {
                    return;
                }
                _newItem = value;
                OnPropertyChanged(() => NewItem);
            }
        }

        public ObservableCollection<SchemeVersionItemDTO> Items { get; } = new ObservableCollection<SchemeVersionItemDTO>();

        public DelegateCommand DeleteCommand { get; }
        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand PublishSchemeVersionCommand { get; }
        public DelegateCommand UnPublishSchemeVersionCommand { get; }
        public DelegateCommand LoadCommand { get; }

        public DelegateCommand CommentCommand { get; }

        protected override void InvokeCallback(Action<SchemeVersionItemDTO> closeCallback)
        {
            closeCallback(SelectedItem);
        }

        private bool CommentCommandCanExecute()
        {
            return SelectedItem != null;
        }

        private void CommentCommandExecute()
        {
            RadWindow.Prompt(new DialogParameters
            {
                Content =
                    "Комментарий",
                Closed = OnClosed,
                Header = "Добавление комментария"
            });
        }

        private async void OnClosed(object sender, WindowClosedEventArgs e)
        {
            await
                new SchemeServiceProxy().CommentSchemeVersionAsync(new CommentSchemeVersionParameterSet
                {
                    Comment = e.PromptResult,
                    SchemeVersionId = SelectedItem.Id
                });

            Load();
        }

        private async void PublishSchemeVersionCommandExecute(bool unpublish)
        {
            var schemeId = SelectedItem.Id;
            if (unpublish)
            {
                await new SchemeServiceProxy().UnPublishSchemeVersionAsync(SelectedItem.Id);
            }
            else
            {
                await new SchemeServiceProxy().PublishSchemeVersionAsync(SelectedItem.Id);
            }
            await Load();
            SelectedItem = Items.FirstOrDefault(dto => dto.Id == schemeId);
        }

        private void AddCommandExecute()
        {
            _model = new AddSchemaViewModel(() =>
            {
                if (_model.DialogResult ?? false)
                {
                    SelectedItem = new SchemeVersionItemDTO {Id = _model.Id};
                    DialogResult = true;
                }
            });
            var dialog = new AddSchemaDialog {DataContext = _model};
            dialog.ShowDialog();
        }

        private async Task Load()
        {
            Behavior.TryLock();
            try
            {
                List<SchemeVersionItemDTO> result;
                if (EditMode)
                {
                    result = await new SchemeServiceProxy().GetSchemeVersionListAsync();
                }
                else
                {
                    result = await new SchemeServiceProxy().GetPublishedSchemeVersionListAsync();
                }

                Items.Clear();
                Items.AddRange(result);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private void Delete()
        {
            var dp = new DialogParameters
            {
                CancelButtonContent = "Отмена",
                Closed = (s, e) =>
                {
                    if (e.DialogResult.HasValue && e.DialogResult.Value)
                    {
                        DeleteSchemeVersion();
                    }
                },
                Content = "Вы уверены что хотите удалить версию?",
                Header = "Удаление версии",
                OkButtonContent = "Ок"
            };

            RadWindow.Confirm(dp);
        }

        private async void DeleteSchemeVersion()
        {
            var id = SelectedItem.Id;
            await new SchemeServiceProxy().DeleteSchemeVersionAsync(id);
            Items.Remove(SelectedItem);
            SelectedItem = null;
            OnSchemeVersionDeleted(new SchemeVersionDeletedEventArgs {ScheveVeresionId = id});
        }

        private void RaiseCanExecuteCommands()
        {
            LoadCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
            PublishSchemeVersionCommand.RaiseCanExecuteChanged();
            UnPublishSchemeVersionCommand.RaiseCanExecuteChanged();
            CommentCommand.RaiseCanExecuteChanged();
        }

        private void OnSchemeVersionDeleted(SchemeVersionDeletedEventArgs e)
        {
            SchemeVersionDeleted?.Invoke(this, e);
        }
    }
}