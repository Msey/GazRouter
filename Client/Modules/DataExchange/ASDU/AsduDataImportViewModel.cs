using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Browser;
using GazRouter.Application;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.DataExchange.Asdu;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.Prism.Commands;
using Utils.Extensions;
using UriBuilder = GazRouter.DataProviders.UriBuilder;
using GazRouter.DataProviders.ASDU;
using GazRouter.DTO.ASDU;
using System.Windows.Controls;
using DataExchange.ASDU;

namespace DataExchange.ASDU
{
    public class AsduDataImportViewModel : LockableViewModel
    {
        public AsduNsiDataViewModel NsiDataModel { get; }

        public AsduDataImportViewModel()
        {
            NsiDataModel = new AsduNsiDataViewModel();
            LoadFilesCommand = new DelegateCommand(LoadFiles);
            ExecuteImportCommand = new DelegateCommand(ExecuteImport);
            ReadFileCommand = new DelegateCommand(ReadFile);
            ApplyFileCommand = new DelegateCommand(ApplyFile);
            ShowXmlCommand = new DelegateCommand(ShowXml);
        }

        private void ShowXml()
        {
            var model = new AsduXmlFileViewModel(SelectedFile);
            var acrv = new AsduXmlFileView() {DataContext = model};
            model.ShowXml();
            acrv.ShowDialog();
        }


        public DelegateCommand LoadFilesCommand { get; private set; }
        public DelegateCommand ExecuteImportCommand { get; private set; }

        public DelegateCommand ReadFileCommand { get; private set; }
        public DelegateCommand ApplyFileCommand { get; private set; }

        public DelegateCommand ShowXmlCommand { get; private set; }


        public IList<LoadedFileWrapper> LoadedFiles { get; private set; }

        private LoadedFileWrapper _selectedFile;

        public LoadedFileWrapper SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                if (SetProperty(ref _selectedFile, value))
                {
                    NsiDataModel.Clear();
                    NsiDataModel.IsDetailsViewEnabled = false;
                    NsiDataModel.PacketKey = SelectedFile?.Key;
                    OnPropertyChanged(() => UploadFileButtonEnabled);
                    OnPropertyChanged(() => ReadFileButtonEnabled);
                    OnPropertyChanged(() => ApplyFileButtonEnabled);
                    OnPropertyChanged(() => ShowXmlButtonEnabled);
                    UpdateDetailsView();
                }
            }
        }

        private void UpdateDetailsView()
        {
            NsiDataModel.IsDataLoaded = false;
            NsiDataModel.IsDetailsViewEnabled = SelectedFile?.Status == LoadedFileStatus.Read ||
                                                SelectedFile?.Status == LoadedFileStatus.Applied;
        }


        public bool UploadFileButtonEnabled => SelectedFile?.Status == LoadedFileStatus.InDir;

        public bool ReadFileButtonEnabled => SelectedFile?.Status == LoadedFileStatus.XmlValidated ||
                                             SelectedFile?.Status == LoadedFileStatus.XmlValidationError; // TODO: Oracle bug

        public bool ApplyFileButtonEnabled => SelectedFile?.Status == LoadedFileStatus.Read;
        public bool ShowXmlButtonEnabled => SelectedFile != null;


        private string _fileContent;
        private string _fileName;


        private async void LoadFiles()
        {
            Behavior.TryLock();
            try
            {
                var sp = new ASDUServiceProxy();
                BusyMessage = string.Format("Выполняется загрузка списка файлов");
                var result =
                    await sp.GetLoadedFilesAsync(new GetLoadedFilesParam {LoadedFilesType = LoadedFilesType.Input});
                LoadedFiles = LoadedFileWrapper.FromLoadedFiles(result);

                OnPropertyChanged(() => LoadedFiles);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }


        private async void ExecuteImport()
        {
            //_fileContent = null;
            //_fileName = null;
            //if (!SelectFile())
            //{
            //    return;
            //}
            if (!UploadFileButtonEnabled)
            {
                return;
            }

            Behavior.TryLock();
            try
            {
                var sp = new ASDUServiceProxy();
                BusyMessage = string.Format("Выполняется загрузка файла");
                var result = await sp.ImportXmlFromMASDUAsync(new XmlFileForImport
                {
                    Filename = SelectedFile.FileName,
                    LoadFromDisk = true
                    //Xml = _fileContent
                });
            }
            finally
            {
                Behavior.TryUnlock();
            }

            LoadFiles();
        }

        private bool SelectFile()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                Filter = "Файлы XML (.xml)|*.xml|All Files (*.*)|*.*",
                FilterIndex = 1,
                Multiselect = false
            };

            bool? userClickedOK = openFileDialog1.ShowDialog();

            if (userClickedOK == true)
            {
                System.IO.Stream fileStream = openFileDialog1.File.OpenRead();

                using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream))
                {
                    _fileContent = reader.ReadToEnd();
                }

                fileStream.Close();
                _fileName = openFileDialog1.File.Name;
            }

            return userClickedOK.GetValueOrDefault();
        }

        private async void ReadFile()
        {
            if (!ReadFileButtonEnabled)
            {
                return;
            }

            Behavior.TryLock();
            try
            {
                var sp = new ASDUServiceProxy();
                BusyMessage = string.Format("Выполняется разбор файла");
                await sp.ManageAsduRequestAsync(new ManageRequestParams
                {
                    Key = SelectedFile.Key,
                    Action = ManageRequestAction.Read
                });
            }
            finally
            {
                Behavior.TryUnlock();
            }

            LoadFiles();
        }


        private async void ApplyFile()
        {
            if (!ApplyFileButtonEnabled)
            {
                return;
            }

            Behavior.TryLock();
            try
            {
                var sp = new ASDUServiceProxy();
                BusyMessage = "Выполняется применение файла";
                await sp.ManageAsduRequestAsync(new ManageRequestParams
                {
                    Key = SelectedFile.Key,
                    Action = ManageRequestAction.Apply
                });
            }
            finally
            {
                Behavior.TryUnlock();
            }

            LoadFiles();
        }
    }
}