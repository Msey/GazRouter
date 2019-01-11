using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DTO.Bindings.Sources;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.TransportTypes;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Modes.Exchange
{
    public class AddEditExchangeSettingViewModel : AddEditViewModelBase<ExchangeTaskDTO, int>
    {
        private readonly bool _isClone;
        private readonly DataExchangeServiceProxy _provider = new DataExchangeServiceProxy();

        public AddEditExchangeSettingViewModel(Action<int> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            Init();
        }


        public AddEditExchangeSettingViewModel(Action<int> actionBeforeClosing, ExchangeTaskDTO es, bool isClone = false)
            : base(actionBeforeClosing, es)
        {
            _isClone = isClone;
            Init();
			Email = es.TransportAddress;
			FileMask = es.FileNameMask;
			IsTransform = es.IsTransform;
			Name = es.Name;
			PeriodTypeId = es.PeriodTypeId;
			SettingData = es.Transformation;
			SourceId = es.DataSourceId;
            Id = es.Id;
            Description = es.Name;
        }


        private void Init()
        {
            PeriodTypes = ClientCache.DictionaryRepository.PeriodTypes;
            Sources = ExchangesViewModel.SharedSourceList;
            UploadCommand = new DelegateCommand(Upload, () => IsTransform);
            CheckCommand = new DelegateCommand(Check, () => !string.IsNullOrEmpty(SettingData) && IsTransform);
            SelectedTransportType = (int) TransportType.Folder;
            _isTransform = true;
            _xmlAsAttachement = true;
        }


        private void Check()
        {
            var content = IsValid(SettingData) ? "Пройдена!" : "Не пройдена! (выберите корректный xsl-файл)";
            
            RadWindow.Alert(new DialogParameters
            {
                Content = content,
                Header =  "Проверка файла",
                OkButtonContent = "Закрыть",
            });
        }

        public bool IsValid(string xmlText)
        {
            try
            {
                using (var reader = XmlReader.Create(new StringReader(xmlText)))
                {
                   while (reader.Read())
                   {
                   }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private void Upload()
        {
            var dlg = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Document Files|*.xsl; *.xml"
            };
            if (dlg.ShowDialog() != true)
            {
                return;
            }
            FileName = dlg.File.Name;
            using (var fileStream = dlg.File.OpenText())
            {
                SettingData = fileStream.ReadToEnd();
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_fileName == value) return;
                _fileName = value;
                OnPropertyChanged(() => FileName);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public List<Source> Sources { get; set; }
        public List<PeriodTypeDTO> PeriodTypes { get; set; }

        public bool IsTransform
        {
            get { return _isTransform; }
            set
            {
                if (value.Equals(_isTransform)) return;
                _isTransform = value;
                OnPropertyChanged(() => IsTransform);
                UploadCommand.RaiseCanExecuteChanged();
                CheckCommand.RaiseCanExecuteChanged();
            }
        }


        protected override bool OnSaveCommandCanExecute()
        {
            return PeriodTypeId.HasValue && !string.IsNullOrEmpty(FileMask); //&& ScheduleTime.HasValue;
        }

        private EditExchangeTaskParameterSet CreateExchangeTaskParameterSet()
        {
            string transportAddress = null;
            string transportLogin = null;
            string transportPassword = null;
            TransportType? transportTypeId = null;
            if (IsFtpVisible)
            {
                transportTypeId = TransportType.Ftp;
                transportAddress = FtpAddress ;
                transportLogin =  FtpLogin ;
                transportPassword = FtpPassword ;
            }
            if (IsEmailVisible)
            {
                transportTypeId = TransportType.Email;
                transportAddress = Email;
            }
            if (IsSmbVisible)
            {
                transportTypeId = TransportType.Folder;
                transportAddress = FolderPath;
            }
            return new EditExchangeTaskParameterSet
                {
                    Id = Id,
                    PeriodTypeId = PeriodTypeId.Value,
                    DataSourceId = (int) SourceId,
                    TransportAddress = transportAddress,
                    TransportLogin = transportLogin,
                    TransportPassword = transportPassword,
                    FileNameMask = FileMask,
                    IsTransform = IsTransform,
                    Transformation = SettingData,
                    ExchangeTypeId = ExchangeType.Export,
                    Name =  Description,
                    TransportTypeId = transportTypeId,
                };
        }

        protected override Task<int> CreateTask
        {
            get
            {
                var par = CreateExchangeTaskParameterSet();
                return _provider.AddExchangeTaskAsync(par);
            }
        }

        protected override Task UpdateTask
        {
            get
            {
                var parameters = CreateExchangeTaskParameterSet();
                if (_isClone) return _provider.AddExchangeTaskAsync(parameters);

                return _provider.EditExchangeTaskAsync(parameters);
            }
        }

        public string SettingData
        {
            get { return _settingData; }
            set
            {
                if (_settingData == value) return;
                _settingData = value;
                OnPropertyChanged(() => SettingData);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private PeriodType? _periodTypeId;
        public PeriodType? PeriodTypeId
        {
            get { return _periodTypeId; }
            set
            {
                if (_periodTypeId == value) return;
                _periodTypeId = value;
                OnPropertyChanged(() => PeriodTypeId);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        private int? _sourceId;
        private string _fileName;
        private string _settingData;
        private int _selectedTransportType;
        private bool _isSmbVisible;
        private bool _isFtpVisible;
        private bool _isEmailVisible;
        private bool _xmlAsAttachement;
        private bool _isTransform;
        private string _fileMask;

        public int? SourceId
        {
            get { return _sourceId; }
            set
            {
                if (_sourceId == value) return;
                _sourceId = value;
                OnPropertyChanged(() => SourceId);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        public string Description { get; set; }

        public string FileMask
        {
            get { return _fileMask; }
            set
            {
                if (value == _fileMask) return;
                _fileMask = value;
                OnPropertyChanged(() => FileMask);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        protected override string CaptionEntityTypeName
        {
            get { return " задания"; }
        }

        public string FtpAddress { get; set; }

        public string FtpLogin { get; set; }

        public string FtpPassword { get; set; }

        public string Email { get; set; }

        public string FolderPath { get; set; }

        public string XslPath { get; set; }


        public DelegateCommand UploadCommand { get; set; }

        public DelegateCommand CheckCommand { get; set; }
        public DelegateCommand DownloadCommand { get; set; }

        public TimeSpan? ScheduleTime
        {
            get; set;
        }

        public IEnumerable<KeyValuePair<int, string>> TransportTypeList
        {
            get
            {
                yield return new KeyValuePair<int, string>((int)TransportType.Folder, "smb");
                yield return new KeyValuePair<int, string>((int)TransportType.Ftp, "ftp | sftp | ftps");
                yield return new KeyValuePair<int, string>((int) TransportType.Email, "email");
            }
        }

        public int SelectedTransportType
        {
            get { return _selectedTransportType; }
            set
            {
                _selectedTransportType = value;
                IsSmbVisible = value == (int) TransportType.Folder;
                IsFtpVisible = value == (int) TransportType.Ftp;
                IsEmailVisible = value == (int) TransportType.Email;
            }
        }

        public bool IsEmailVisible
        {
            get { return _isEmailVisible; }
            set
            {
                if (value.Equals(_isEmailVisible)) return;
                _isEmailVisible = value;
                OnPropertyChanged(() => IsEmailVisible);
            }
        }

        public bool IsFtpVisible
        {
            get { return _isFtpVisible; }
            set
            {
                if (value.Equals(_isFtpVisible)) return;
                _isFtpVisible = value;
                OnPropertyChanged(() => IsFtpVisible);
            }
        }

        public bool IsSmbVisible
        {
            get { return _isSmbVisible; }
            set
            {
                if (value.Equals(_isSmbVisible)) return;
                _isSmbVisible = value;
                OnPropertyChanged(() => IsSmbVisible);
            }
        }

        public bool XmlAsAttachement
        {
            get { return _xmlAsAttachement; }
            set
            {
                if (value.Equals(_xmlAsAttachement)) return;
                _xmlAsAttachement = value;
                OnPropertyChanged(() => XmlAsAttachement);
            }
        }

        public string FileMaskTooltip
        {
            get
            {
                return @"
                d - День месяца, в диапазоне от 1 до 31 
                dd - День месяца, в диапазоне от 01 до 31 
                ddd - Сокращенное название дня недели 
                dddd - Полное название дня недели 
                h - Час в 12-часовом формате от 1 до 12 
                hh - Час в 12-часовом формате от 01 до 12 
                H - Час в 24-часовом формате от 0 до 23 
                HH - Час в 24-часовом формате от 00 до 23 
                m - Минуты, в диапазоне от 0 до 59
                mm - Минуты, в диапазоне от 00 до 59
                M - Месяц, в диапазоне от 1 до 12
                MM - Месяц, в диапазоне от 01 до 12
                MMM - Сокращенное название месяца
                MMMM - Полное название месяца";
            }
        }
    }
}