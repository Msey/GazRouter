using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ASDU;
using GazRouter.DTO.ASDU;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace DataExchange.ASDU
{
    public class AsduChangeRequestItem : PropertyChangedBase
    {
        private readonly OutboundContents _contents;

        public AsduChangeRequestItem(OutboundContents contents)
        {
            _contents = contents;
        }

        public int Key => _contents.Key;
        public string NodeType => _contents.Nodetype;
        public string Type => _contents.Type;
        public string Id => _contents.Id;
        public string Name => _contents.Name;
        public string ChangeState => _contents.Changestate;
        public string Value => _contents.Value;
        public string ValueAsdu => _contents.ValueAsdu;

        public string NodeTypeHumanReadable
        {
            get
            {
                var res = "неизвестно";
                switch (NodeType)
                {
                    case "ius_obj":
                        res = "Объект";
                        break;
                    case "atr_ius":
                        res = "Атрибут";
                        break;
                    case "asdu_obj":
                        res = "Объект М АСДУ";
                        break;
                }

                return res;
            }
        }

        public string Action
        {
            get
            {
                var res = "неизвестно";
                switch (ChangeState)
                {
                    case "update":
                        res = "Изменение";
                        break;
                    case "add":
                        res = "Добавление";
                        break;
                    case "delete":
                        res = "Удаление";
                        break;
                    case "nochange":
                        res = "Нет изменения";
                        break;
                }

                return res;
            }
        }
    }

    public class AsduChangeRequestViewModel : LockableViewModel
    {
        public AsduNsiDataViewModel NsiDataModel { get; }

        private LoadedFileWrapper _requestToSelect;

        public IList<AsduChangeRequestItem> RequestItems { get; private set; }

        public DelegateCommand AddRequestCommand { get; }
        public DelegateCommand EditRequestCommand { get; }
        public DelegateCommand DeleteRequestCommand { get; }
        public DelegateCommand CreateRequestXmlCommand { get; }
        public DelegateCommand SendRequestCommand { get; }
        public DelegateCommand ShowXmlCommand { get; private set; }
        public DelegateCommand DeleteOutboundContentCommand { get; }

        public ObservableCollection<LoadedFileWrapper> Requests { get; set; }

        private bool _isRequestsGridBusy;

        public bool IsRequestsGridBusy
        {
            get { return _isRequestsGridBusy; }
            set { SetProperty(ref _isRequestsGridBusy, value); }
        }

        private bool _isRequestItemsGridBusy;

        public bool IsRequestItemsGridBusy
        {
            get { return _isRequestItemsGridBusy; }
            set { SetProperty(ref _isRequestItemsGridBusy, value); }
        }

        public bool IsRequestSelected => SelectedRequest != null;

        public bool IsRequestItemSelected => SelectedRequestItem != null;

        public bool EditRequestEnabled => SelectedRequest?.Status != LoadedFileStatus.RequestSent;
        public bool DeleteRequestEnabled => SelectedRequest?.Status != LoadedFileStatus.RequestSent;
        public bool CreateRequestEnabled => SelectedRequest?.Status != LoadedFileStatus.RequestSent;

        public bool ShowXmlButtonEnabled => SelectedRequest?.Status == LoadedFileStatus.RequestXmlCreated ||
                                            SelectedRequest?.Status == LoadedFileStatus.XmlValidationError ||
                                            SelectedRequest?.Status == LoadedFileStatus.RequestSent;
        public bool SendRequestEnabled => SelectedRequest?.Status == LoadedFileStatus.RequestXmlCreated ||
                                          SelectedRequest?.Status == LoadedFileStatus.XmlValidationError;

        private AsduChangeRequestItem _selectedRequestItem;

        public AsduChangeRequestItem SelectedRequestItem
        {
            get { return _selectedRequestItem; }

            set
            {
                if (SetProperty(ref _selectedRequestItem, value))
                {
                    OnPropertyChanged(() => IsRequestItemSelected);
                }
            }
        }

        private LoadedFileWrapper _selectedRequest;

        public LoadedFileWrapper SelectedRequest
        {
            get { return _selectedRequest; }

            set
            {
                if (SetProperty(ref _selectedRequest, value))
                {
                    OnPropertyChanged(() => IsRequestSelected);
                    NsiDataModel.Clear();
                    NsiDataModel.IsDetailsViewEnabled = false;
                    NsiDataModel.PacketKey = SelectedRequest?.Key;
                    OnPropertyChanged(() => ShowXmlButtonEnabled);
                    OnPropertyChanged(() => SendRequestEnabled);
                    OnPropertyChanged(() => EditRequestEnabled);
                    OnPropertyChanged(() => DeleteRequestEnabled);
                    OnPropertyChanged(() => CreateRequestEnabled);
                    UpdateDetailsView();
                }
            }
        }

        private void UpdateDetailsView()
        {
            NsiDataModel.IsDataLoaded = false;
            NsiDataModel.IsDetailsViewEnabled = true;
        }

        public AsduChangeRequestViewModel(LoadedFileWrapper requestToSelect)
        {
            _requestToSelect = requestToSelect;
            NsiDataModel = new AsduNsiDataViewModel();
            AddRequestCommand = new DelegateCommand((a) => EditRequest(true));
            EditRequestCommand = new DelegateCommand((a) => EditRequest(false));
            DeleteRequestCommand = new DelegateCommand(DeleteRequest);
            CreateRequestXmlCommand = new DelegateCommand(CreateRequestXml);
            SendRequestCommand = new DelegateCommand(SendRequest);
            ShowXmlCommand = new DelegateCommand(ShowXml);
            DeleteOutboundContentCommand = new DelegateCommand(DeleteOutboundContent);

            LoadRequests();
        }

        private void SendRequest(object obj)
        {
            if (!SendRequestEnabled)
            {
                return;
            }

            MessageBoxProvider.Confirm(
                "Вы уверены, что хотите отправить заявку?" + (SelectedRequest.Status ==
                LoadedFileStatus.XmlValidationError
                    ? " Xml не прошел проверку!"
                    : ""), DoSendRequest);
        }

        private async void DoSendRequest(bool confirmed)
        {
            if (!confirmed)
            {
                return;
            }
            try
            {
                Behavior.TryLock("Отправка заявки");
                var sp = new ASDUServiceProxy();
                await sp.ManageAsduRequestAsync(new ManageRequestParams
                {
                    Key = SelectedRequest.Key,
                    Name = SelectedRequest.FileName,
                    Action = ManageRequestAction.Send
                });
                _requestToSelect = SelectedRequest;
                LoadRequests();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private void ShowXml(object obj)
        {
            var model = new AsduXmlFileViewModel(SelectedRequest);
            var acrv = new AsduXmlFileView() { DataContext = model };
            model.ShowXml();
            acrv.ShowDialog();
        }

        private async void CreateRequestXml(object obj)
        {
            if (SelectedRequest == null)
            {
                return;
            }

            try
            {
                Behavior.TryLock("Формирование заявки");
                var sp = new ASDUServiceProxy();
                await sp.ManageAsduRequestAsync(new ManageRequestParams
                {
                    Key = SelectedRequest.Key,
                    Action = ManageRequestAction.GenerateXml
                });
                _requestToSelect = SelectedRequest;
                LoadRequests();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private void DeleteOutboundContent(object obj)
        {
            if (SelectedRequestItem != null)
            {
                MessageBoxProvider.Confirm("Удалить выбранный элемент заявки?", async b =>
                {
//                    if (confirmed)
//                    {
//                        var sp = new ASDUServiceProxy();
//                        await sp.ManageAsduOutboundContentsAsync(
//                            new ManageRequestParams {Delete = true, Key = (SelectedRequestItem?.Key).ToString()});
//                        LoadOutboundContents();
//                    }
                });
            }
        }

        private void DeleteRequest(object obj)
        {
            if (SelectedRequest != null)
            {
                MessageBoxProvider.Confirm("Удалить выбранную заявку?", async b =>
                {
                    if (b)
                    {
                        var sp = new ASDUServiceProxy();
                        await sp.ManageAsduRequestAsync(new ManageRequestParams
                        {
                            Key = SelectedRequest.Key,
                            Action = ManageRequestAction.Delete
                        });
                        LoadRequests();
                    }
                });
            }
        }

        private void EditRequest(bool add)
        {
            if (!add && SelectedRequest == null)
            {
                return;
            }

            var dp = new DialogParameters
            {
                Header = "Заявка",
                Closed = (s, e) => RequestEdited(add, s, e),
                Content = "Введите имя заявки:",
                DefaultPromptResultValue = add ? "" : SelectedRequest.Name
            };

            RadWindow.Prompt(dp);
        }

        private async void RequestEdited(bool add, object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult.GetValueOrDefault())
            {
                var sp = new ASDUServiceProxy();
                await sp.ManageAsduRequestAsync(new ManageRequestParams
                {
                    Key = add ? null : SelectedRequest.Key,
                    Name = e.PromptResult,
                    Action = add ? ManageRequestAction.Create : ManageRequestAction.UpdateName
                });
                if (!add)
                {
                    _requestToSelect = SelectedRequest;
                }
                LoadRequests();
            }
        }

        private async void LoadRequests()
        {
            try
            {
                IsRequestsGridBusy = true;
                var sp = new ASDUServiceProxy();
                var tRequests =
                    sp.GetLoadedFilesAsync(new GetLoadedFilesParam {LoadedFilesType = LoadedFilesType.Output});
                await tRequests;
                Requests = new ObservableCollection<LoadedFileWrapper>(LoadedFileWrapper.FromLoadedFiles(tRequests.Result));
                OnPropertyChanged(() => Requests);
                SelectedRequest = Requests.FirstOrDefault(r => r.Key == _requestToSelect?.Key);
                _requestToSelect = null;
            }
            finally
            {
                IsRequestsGridBusy = false;
            }
        }
    }
}