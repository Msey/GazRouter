using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DTO.DataExchange.DataSource;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.TransportTypes;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.DataExchange.CustomSource.Dialogs
{
    public class AddEditExchangeTaskViewModel : AddEditViewModelBase<ExchangeTaskDTO, int>
    {

        private string _email;


        private string _fileNameMask;

        private string _folderPath;


        private string _ftpAddress;

        private string _hostKey;
        private bool _isCritical;

        private bool _isSql;


        private bool _isTransform;
        private string _lag;


        private DataSourceDTO _selectedDataSource;


        private KeyValuePair<int, string> _selectedExchangeStatus;

        private ExchangeTypeDTO _selectedExchangeType;


        private PeriodTypeDTO _selectedPeriodType;

        private TransportTypeDTO _selectedTransportType;
        private string _sqlProcedureName;

        private string _transformation;

        public AddEditExchangeTaskViewModel(Action<int> actionBeforeClosing, int dataSourceId)
            : base(actionBeforeClosing)
        {
            var dto = new ExchangeTaskDTO {DataSourceId = dataSourceId};
            Init(dto);
        }


        public AddEditExchangeTaskViewModel(Action<int> actionBeforeClosing, ExchangeTaskDTO dto,
            bool changableExchangeType = true)
            : base(actionBeforeClosing, dto)
        {
            Init(dto, changableExchangeType);
        }

        public bool ChangableExchangeType { get; set; }

        public string SqlProcedureName
        {
            get { return _sqlProcedureName; }
            set
            {
                if (SetProperty(ref _sqlProcedureName, value))
                    OnPropertyChanged(() => SqlProcedureName);
            }
        }

        public bool IsSql
        {
            get { return _isSql; }
            set
            {
                if (SetProperty(ref _isSql, value))
                    OnPropertyChanged(() => IsSql);
            }
        }


        /// <summary>
        ///     Список источников данных
        /// </summary>
        public List<DataSourceDTO> DataSourceList { get; set; }

        /// <summary>
        ///     Выбранный источник данных
        /// </summary>
        public DataSourceDTO SelectedDataSource
        {
            get { return _selectedDataSource; }
            set
            {
                if (SetProperty(ref _selectedDataSource, value))
                    UpdateCommands();
            }
        }


        /// <summary>
        ///     Список типов периодов
        /// </summary>
        public IEnumerable<PeriodTypeDTO> PeriodTypeList
        {
            get
            {
                yield return ClientCache.DictionaryRepository.PeriodTypes.Single(
                    pt => pt.PeriodType == PeriodType.Twohours);
                yield return ClientCache.DictionaryRepository.PeriodTypes.Single(pt => pt.PeriodType == PeriodType.Day);
                //yield return ClientCache.DictionaryRepository.PeriodTypes.Single(pt => pt.PeriodType == PeriodType.Month);
            }
        }

        /// <summary>
        ///     Выбранный тип периода
        /// </summary>
        public PeriodTypeDTO SelectedPeriodType
        {
            get { return _selectedPeriodType; }
            set
            {
                if (SetProperty(ref _selectedPeriodType, value))
                    UpdateCommands();
            }
        }

        /// <summary>
        ///     Список типов обмена
        /// </summary>
        public List<ExchangeTypeDTO> ExchangeTypeList => ClientCache.DictionaryRepository.ExchangeTypes;

        /// <summary>
        ///     выбранный тип обмена
        /// </summary>
        public ExchangeTypeDTO SelectedExchangeType
        {
            get { return _selectedExchangeType; }
            set
            {
                if (SetProperty(ref _selectedExchangeType, value))
                {
                    OnPropertyChanged(() => IsImport);
                    OnPropertyChanged(() => IsExport);
                    UpdateCommands();
                }
            }
        }

        public bool IsImport => _selectedExchangeType != null &&
                                _selectedExchangeType.ExchangeType == ExchangeType.Import;

        public bool IsExport => _selectedExchangeType != null &&
                                _selectedExchangeType.ExchangeType == ExchangeType.Export;


        /// <summary>
        ///     Влияние на формировании серии данных
        /// </summary>
        public bool IsCritical
        {
            get { return _isCritical; }
            set
            {
                if (SetProperty(ref _isCritical, value))
                    OnPropertyChanged(() => IsCritical);
            }
        }

        /// <summary>
        ///     Маска файла
        /// </summary>
        public string FileNameMask
        {
            get { return _fileNameMask; }
            set
            {
                if (SetProperty(ref _fileNameMask, value))
                    UpdateCommands();
            }
        }

        /// <summary>
        ///     Выполнять трансформацию файла (XSLT)
        /// </summary>
        public bool IsTransform
        {
            get { return _isTransform; }
            set
            {
                if (SetProperty(ref _isTransform, value))
                    UpdateCommands();
            }
        }

        /// <summary>
        ///     Текст трансформации (XSLT)
        /// </summary>
        public string Transformation
        {
            get { return _transformation; }
            set
            {
                if (SetProperty(ref _transformation, value))
                    UpdateCommands();
            }
        }


        /// <summary>
        ///     Способ передачи (ftp, email и т.п.)
        /// </summary>
        public List<TransportTypeDTO> TransportTypeList => ClientCache.DictionaryRepository.TransportTypes;


        /// <summary>
        ///     Срабатывание таска (по событию , расписанию или отключен)
        /// </summary>
        public List<KeyValuePair<int, string>> ExchangeStatuses { get; } = new List<KeyValuePair<int, string>>
        {
            new KeyValuePair<int, string>((int) ExchangeStatus.Off, "Выключен"),
            new KeyValuePair<int, string>((int) ExchangeStatus.Event, "По событию"),
            new KeyValuePair<int, string>((int) ExchangeStatus.Scheduled, "По таймеру")
        };

        /// <summary>
        ///     Выбранный способ cрабатывания таска (по событию , расписанию или отключен)
        /// </summary>
        public KeyValuePair<int, string> SelectedExchangeStatus
        {
            get { return _selectedExchangeStatus; }
            set
            {
                if (SetProperty(ref _selectedExchangeStatus, value))
                {
                    UpdateCommands();
                    OnPropertyChanged(() => SelectedExchangeStatus);
                    OnPropertyChanged(() => IsScheduled);
                }
            }
        }

        /// <summary>
        ///     Выбранный способ передачи данных
        /// </summary>
        public TransportTypeDTO SelectedTransportType
        {
            get { return _selectedTransportType; }
            set
            {
                if (SetProperty(ref _selectedTransportType, value))
                {
                    UpdateCommands();
                    OnPropertyChanged(() => IsFtp);
                    OnPropertyChanged(() => IsEmail);
                    OnPropertyChanged(() => IsFolder);
                }
            }
        }

        public bool IsFtp => SelectedTransportType != null && SelectedTransportType.TransportType == TransportType.Ftp;

        public bool IsEmail => SelectedTransportType != null &&
                               SelectedTransportType.TransportType == TransportType.Email;

        public bool IsFolder => SelectedTransportType != null &&
                                SelectedTransportType.TransportType == TransportType.Folder;

        /// <summary>
        ///     Адрес FTP-сервера
        /// </summary>
        public string FtpAddress
        {
            get { return _ftpAddress; }
            set
            {
                if (SetProperty(ref _ftpAddress, value))
                    UpdateCommands();
            }
        }

        public string FtpLogin { get; set; }

        public string FtpPassword { get; set; }

        /// <summary>
        ///     Адрес email
        /// </summary>
        public string Email
        {
            get { return _email; }
            set
            {
                if (SetProperty(ref _email, value))
                    UpdateCommands();
            }
        }

        /// <summary>
        ///     Путь к папке
        /// </summary>
        public string FolderPath
        {
            get { return _folderPath; }
            set
            {
                if (SetProperty(ref _folderPath, value))
                    UpdateCommands();
            }
        }

        public string HostKey
        {
            get { return _hostKey; }
            set
            {
                if (SetProperty(ref _hostKey, value))
                    UpdateCommands();
            }
        }

        /// <summary>
        ///     Отправлять данные как вложение (только для email)
        /// </summary>
        public bool SendAsAttachment { get; set; }

        protected override Task<int> CreateTask
        {
            get
            {
                var set = new AddExchangeTaskParameterSet();
                PrepareParameterSet(set);
                return new DataExchangeServiceProxy().AddExchangeTaskAsync(set);
            }
        }

        protected override Task UpdateTask
        {
            get
            {
                var set = new EditExchangeTaskParameterSet
                {
                    Id = Model.Id,
                    Name = Name,
                    DataSourceId = Model.DataSourceId,
                    PeriodTypeId = Model.PeriodTypeId,
                    ExchangeTypeId = Model.ExchangeTypeId,
                    FileNameMask = Model.FileNameMask,
                    IsCritical = IsCritical,
                    IsTransform = IsTransform,
                    Transformation = Transformation,
                    IsSql = IsSql,
                    SqlProcedureName = SqlProcedureName,
                    ExchangeStatus = (ExchangeStatus) SelectedExchangeStatus.Key
                };
                PrepareParameterSet(set);
                return new DataExchangeServiceProxy().EditExchangeTaskAsync(set);
            }
        }


        protected override string CaptionEntityTypeName => " задания";


        public string XslPath { get; set; }


        public DelegateCommand UploadCommand { get; set; }

        public DelegateCommand CheckCommand { get; set; }
        public DelegateCommand DownloadCommand { get; set; }

        public bool FirstTabVisible { get; set; } = true;
        public bool SecondTabVisible { get; set; } = true;

        public string Lag
        {
            get { return _lag; }
            set
            {
                if (SetProperty(ref _lag, value))
                {
                    OnPropertyChanged(() => Lag);
                }
            }
        }

        public bool IsScheduled => SelectedExchangeStatus.Key == (int) ExchangeStatus.Scheduled;
        

        private async void Init(ExchangeTaskDTO dto, bool changableExchangeType = true)
        {
            ChangableExchangeType = changableExchangeType;

            Name = dto.Name;
            SelectedPeriodType = PeriodTypeList.SingleOrDefault(p => p.PeriodType == dto.PeriodTypeId);
            SelectedExchangeType = ExchangeTypeList.SingleOrDefault(et => et.ExchangeType == dto.ExchangeTypeId);
            FileNameMask = dto.FileNameMask;
            IsCritical = dto.IsCritical;
            IsTransform = dto.IsTransform;
            Transformation = dto.Transformation;
            SelectedTransportType = TransportTypeList.SingleOrDefault(t => t.TransportType == dto.TransportTypeId);
            IsSql = dto.IsSql;
            SqlProcedureName = dto.SqlProcedureName;
            SelectedExchangeStatus = ExchangeStatuses.SingleOrDefault(t => t.Key == (int) dto.ExchangeStatus);
            Lag = dto.Lag.ToString();

            if (dto.TransportTypeId == TransportType.Ftp)
            {
                FtpAddress = dto.TransportAddress;
                FtpLogin = dto.TransportLogin;
                FtpPassword = dto.TransportPassword;
                HostKey = dto.HostKey;
            }

            if (dto.TransportTypeId == TransportType.Email)
            {
                Email = dto.TransportAddress;
                SendAsAttachment = dto.SendAsAttachment;
            }

            if (dto.TransportTypeId == TransportType.Folder)
                FolderPath = dto.TransportAddress;


            DataSourceList = await new DataExchangeServiceProxy().GetDataSourceListAsync(null);

            var dataSourceId = dto.DataSourceId;
            SelectedDataSource = DataSourceList.SingleOrDefault(s => s.Id == dataSourceId);
            if (SelectedDataSource == null)
            {
                DataSourceList =
                    await new DataExchangeServiceProxy().GetDataSourceListAsync(
                        new GetDataSourceListParameterSet {GetReadonly = true});
                SelectedDataSource = DataSourceList.SingleOrDefault(s => s.Id == dataSourceId);
            }
            OnPropertyChanged(() => DataSourceList);

            SetValidationRules();
            ValidateAll();
        }


        private async void Init(int dataSourceId)
        {
            DataSourceList = await new DataExchangeServiceProxy().GetDataSourceListAsync(null);

            SelectedDataSource = DataSourceList.SingleOrDefault(s => s.Id == dataSourceId);
            if (SelectedDataSource == null)
            {
                DataSourceList =
                    await new DataExchangeServiceProxy().GetDataSourceListAsync(
                        new GetDataSourceListParameterSet {GetReadonly = true});
                SelectedDataSource = DataSourceList.SingleOrDefault(s => s.Id == dataSourceId);
            }
            OnPropertyChanged(() => DataSourceList);

            SetValidationRules();
            ValidateAll();
        }


        private void UpdateCommands()
        {
            SaveCommand.RaiseCanExecuteChanged();
        }


        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }


        private void PrepareParameterSet(AddExchangeTaskParameterSet set)
        {
            if (FirstTabVisible && SecondTabVisible)
            {
                set.Name = Name;
                set.DataSourceId = SelectedDataSource.Id;
                set.PeriodTypeId = SelectedPeriodType.PeriodType;
                set.ExchangeTypeId = SelectedExchangeType.ExchangeType;
                set.FileNameMask = FileNameMask;
                set.IsCritical = IsCritical;
                set.IsTransform = IsTransform;
                set.Transformation = Transformation;
                set.IsSql = IsSql;
                set.SqlProcedureName = SqlProcedureName;
                set.ExchangeStatus = (ExchangeStatus) SelectedExchangeStatus.Key;
            }

            if (SelectedExchangeType.ExchangeType == ExchangeType.Export)
            {
                set.TransportTypeId = SelectedTransportType.TransportType;

                if (IsFtp)
                {
                    set.TransportAddress = FtpAddress;
                    set.TransportLogin = FtpLogin;
                    set.TransportPassword = FtpPassword;
                    set.HostKey = HostKey;
                }
                if (IsEmail)
                {
                    set.TransportAddress = Email;
                    set.SendAsAttachment = SendAsAttachment;
                }
                if (IsFolder)
                    set.TransportAddress = FolderPath;

                int lag;
                if (int.TryParse(Lag, out lag))
                    set.Lag = lag;
            }
        }


        private void SetValidationRules()
        {
            if (FirstTabVisible && SecondTabVisible)
            {
                AddValidationFor(() => Name)
                    .When(() => string.IsNullOrEmpty(Name))
                    .Show("Введите наименование");

                AddValidationFor(() => SelectedPeriodType)
                    .When(() => SelectedPeriodType == null)
                    .Show("Не выбран период");

                AddValidationFor(() => SelectedExchangeType)
                    .When(() => SelectedExchangeType == null)
                    .Show("Не выбран тип");

                AddValidationFor(() => FileNameMask)
                    .When(() => string.IsNullOrEmpty(FileNameMask))
                    .Show("Укажите маску для имени файла");


                AddValidationFor(() => Transformation)
                    .When(() => IsTransform && string.IsNullOrEmpty(Transformation))
                    .Show("Введите текст XSLT трансформации");
            }


            AddValidationFor(() => SelectedTransportType)
                .When(() => IsExport && SelectedTransportType == null && IsExport)
                .Show("Выберите способ передачи данных");

            AddValidationFor(() => FtpAddress)
                .When(() => IsFtp && string.IsNullOrEmpty(FtpAddress) && IsExport)
                .Show("Введите адрес FTP-сервера");

            AddValidationFor(() => Email)
                .When(() => IsEmail && string.IsNullOrEmpty(Email) && IsExport)
                .Show("Введите адрес электронной почты");

            AddValidationFor(() => Email)
                .When(() => IsEmail && !string.IsNullOrEmpty(Email) && !Email.Contains("@") && IsExport)
                .Show("Недопустимый адрес электронной почты");


            AddValidationFor(() => FtpAddress)
                .When(() => IsFolder && string.IsNullOrEmpty(FolderPath) && IsExport)
                .Show("Введите путь к папке");
        }
    }
}