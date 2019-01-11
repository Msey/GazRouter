using DataExchange.Integro.ASSPOOTI;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DataProviders.Integro;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.Integro;
using GazRouter.DTO.DataExchange.Integro.Enum;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.Integro;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GazRouter.DataExchange.Integro.ASSPOOTI
{
    public class AddEditSummaryViewModel : DialogViewModel
    {
        private Guid _id;
        private SystemItem systemItem;
        private SummaryExchTaskDTO model;

        public AddEditSummaryViewModel(Action closeCallback, SystemItem systemItem) : this(closeCallback, null, systemItem)
        {
        }

        public AddEditSummaryViewModel(Action closeCallback, SummaryExchTaskDTO summaryModel, SystemItem systemItem) : base(closeCallback)
        {
            model = summaryModel;
            if (model == null)
                model = new SummaryExchTaskDTO();
            InitData();
            this.systemItem = systemItem;
            _id = model != null && model.Summary != null ? model.Summary.Id : Guid.Empty;
            _name = model?.Summary?.Name;
            Descriptor = model?.Summary?.Descriptor;
            TransformFileName = model?.Summary?.TransformFileName;            
            var periodType = model != null ? model.Summary?.PeriodType : PeriodType.Twohours;
            SelectedPeriodType = PeriodTypes.SingleOrDefault(p => p.PeriodType == periodType);
            IsUsedInExchange = (int)SummaryStatusTypes.Used == model?.Summary?.StatusId;
            //
            if (model.ExchangeTask != null)
            {
                ExchangeTaskId = model.ExchangeTask.Id;
                //Name = model.ExchangeTask.Name;
                //SelectedPeriodType = PeriodTypes.SingleOrDefault(p => p.PeriodType == dto.PeriodTypeId);
                //SelectedExchangeType = ExchangeTypeList.SingleOrDefault(et => et.ExchangeType == dto.ExchangeTypeId);
                FileNameMask = model.ExchangeTask.FileNameMask;
                SelectedExcludeHour = model.ExchangeTask.ExcludeHours;
                //IsCritical = dto.IsCritical;
                IsTransform = model.ExchangeTask.IsTransform;
                Transformation = model.ExchangeTask.Transformation;
                //SelectedTransportType = TransportTypeList.SingleOrDefault(t => t.TransportType == dto.TransportTypeId);
                //IsSql = dto.IsSql;
                //SqlProcedureName = dto.SqlProcedureName;
            }
            IsAsspooti = systemItem.SourceType == MappingSourceType.ASSPOOTI;
            SaveCommand = new DelegateCommand(Save, OnSaveCommandCanExecute);
        }

        private void InitData()
        {
            PeriodTypes = new List<PeriodTypeDTO> {
                ClientCache.DictionaryRepository.PeriodTypes.Single(pt => pt.PeriodType == PeriodType.Twohours),
                ClientCache.DictionaryRepository.PeriodTypes.Single(pt => pt.PeriodType == PeriodType.Day),
                ClientCache.DictionaryRepository.PeriodTypes.Single(pt => pt.PeriodType == PeriodType.Month),
            };

            ExcludeHourList = new List<string> { "00","02","04","06","08","10","12","14","16","18","20","22" };
            //SessionDataTypes = new List<SessionDataTypeDTO>{
            //    new SessionDataTypeDTO() { Item = SessionDataType.RT },
            //    new SessionDataTypeDTO() { Item = SessionDataType.UB }
            //};
        }

        private int ExchangeTaskId = 0;

        public bool IsAsspooti { get; set; }

        public bool IsExcludeHourVisibility { get { return IsAsspooti && SelectedPeriodType != null && SelectedPeriodType.PeriodType == PeriodType.Twohours;  } }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (SetProperty(ref _name, value))
                    RefreshCommands();
            }
        }

        private string _descriptor;
        public string Descriptor
        {
            get { return _descriptor; }
            set
            {
                if (SetProperty(ref _descriptor, value))
                    RefreshCommands();
            }
        }

        private string _transformFileName;
        public string TransformFileName
        {
            get { return _transformFileName; }
            set
            {
                if (SetProperty(ref _transformFileName, value))
                    RefreshCommands();
            }
        }

        private string _fileNameMask;
        public string FileNameMask
        {
            get { return _fileNameMask; }
            set
            {
                if (SetProperty(ref _fileNameMask, value))
                    RefreshCommands();
            }
        }

        /// <summary>
        /// Для АССПООТИ час выгрузки сводки
        /// </summary>
        public List<string> ExcludeHourList { get; private set; }

        private string _excludeHours;
        /// <summary>
        /// Для АССПООТИ час выгрузки сводки
        /// </summary>
        public string SelectedExcludeHour
        {
            get { return _excludeHours; }
            set
            {
                if (SetProperty(ref _excludeHours, value))
                    RefreshCommands();
            }
        }

        private bool isTransform;
        public bool IsTransform
        {
            get { return isTransform; }
            set
            {
                if (SetProperty(ref isTransform, value))
                    RefreshCommands();
            }
        }

        private bool isUsedInExchange;
        public bool IsUsedInExchange
        {
            get { return isUsedInExchange; }
            set
            {
                if (SetProperty(ref isUsedInExchange, value))
                    RefreshCommands();
            }
        }
        
        /// <summary>
        /// Список типов периодов
        /// </summary>
        public List<PeriodTypeDTO> PeriodTypes { get; private set; }


        private PeriodTypeDTO _selectedPeriodType;
        /// <summary>
        /// Выбранный тип периода
        /// </summary>
        public PeriodTypeDTO SelectedPeriodType
        {
            get { return _selectedPeriodType; }
            set
            {
                if (SetProperty(ref _selectedPeriodType, value))
                {
                    SetSessionDataType();
                    SelectedExcludeHour = null;
                    OnPropertyChanged(() => IsExcludeHourVisibility);                    
                    RefreshCommands();
                }

            }
        }

        private List<SessionDataTypeDTO> sessionDataTypes;
        /// <summary>
        /// Список типов 
        /// </summary>
        public List<SessionDataTypeDTO> SessionDataTypes
        {
            get { return sessionDataTypes; }
            set
            {
                SetProperty(ref sessionDataTypes, value);
            }
        }

        private SessionDataTypeDTO _selectedSessionDataType;
        /// <summary>
        /// Выбранный тип периода
        /// </summary>
        public SessionDataTypeDTO SelectedSessionDataType
        {
            get { return _selectedSessionDataType; }
            set
            {
                if (SetProperty(ref _selectedSessionDataType, value))
                {
                    RefreshCommands();
                }

            }
        }


        private string _transformation;
        /// <summary>
        /// Текст трансформации (XSLT)
        /// </summary>
        public string Transformation
        {
            get { return _transformation; }
            set
            {
                if (SetProperty(ref _transformation, value))
                    RefreshCommands();
            }
        }

        public DelegateCommand SaveCommand { get; set; }

        private void RefreshCommands()
        {
            if (SaveCommand != null)
                SaveCommand.RaiseCanExecuteChanged();
        }

        protected bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(_name);
        }

        private void SetSessionDataType()
        {
            switch (systemItem.SourceType)
            {
                case MappingSourceType.ASDU_ESG:
                    SetSessionAsdu();
                    break;
                case MappingSourceType.ASSPOOTI:
                    SetSessionAsspooti();
                    break;
                default:
                    SetSessionAsdu();
                    break;
            }
            SelectedSessionDataType = SessionDataTypes.FirstOrDefault(f => f.Item == model?.Summary?.SessionDataType);
        }

        private void SetSessionAsdu()
        {
            switch (SelectedPeriodType.PeriodType)
            {
                case PeriodType.Twohours:
                    SessionDataTypes = new List<SessionDataTypeDTO>{
                        new SessionDataTypeDTO() { Item = SessionDataType.RT },
                        new SessionDataTypeDTO() { Item = SessionDataType.UB } };
                    break;
                case PeriodType.Day:
                    SessionDataTypes = new List<SessionDataTypeDTO>{
                        new SessionDataTypeDTO() { Item = SessionDataType.RT },
                        new SessionDataTypeDTO() { Item = SessionDataType.UB } };
                    break;
                case PeriodType.Month:
                    SessionDataTypes = new List<SessionDataTypeDTO>{
                        new SessionDataTypeDTO() { Item = SessionDataType.PRO},
                        new SessionDataTypeDTO() { Item = SessionDataType.PL} };
                    break;
                default:
                    SessionDataTypes = new List<SessionDataTypeDTO>();
                    break;
            }
        }

        private void SetSessionAsspooti()
        {
            SessionDataTypes = new List<SessionDataTypeDTO>{
                        new SessionDataTypeDTO() { Item = SessionDataType.РТ2 },
                        new SessionDataTypeDTO() { Item = SessionDataType.РТ24 } };
        }
        private async void Save()
        {
            if (Guid.Empty.Equals(_id))
            {
                _id = Guid.NewGuid(); 
            }
            var param = new SummaryExchTaskParamSet();
            param.SummatyParam = new AddEditSummaryParameterSet
            {
                Id = _id,
                Name = Name,
                Descriptor = Descriptor,
                TransformFileName = TransformFileName,
                PeriodType = SelectedPeriodType.PeriodType,
                SystemId = systemItem.Id,
                SessionType = SelectedSessionDataType.Name,
                ExchangeTaskId = ExchangeTaskId
            };
            if (model?.Summary?.StatusId != (int)SummaryStatusTypes.Deleted)
            {
                param.SummatyParam.StatusTypeId = (int)(IsUsedInExchange ? SummaryStatusTypes.Used : SummaryStatusTypes.NotUsed);
            }

            param.ExchTaskParam = new EditExchangeTaskParameterSet
            {
                Id = ExchangeTaskId,
                Name = Name,
                DataSourceId = (int)systemItem.SourceType,// (int)DataExchangeSources.Asduesg,
                PeriodTypeId = SelectedPeriodType.PeriodType,
                ExchangeTypeId = ExchangeType.Export,
                FileNameMask = FileNameMask,
                IsCritical = false,
                IsTransform = true, // xslt -преобразование
                Transformation = Transformation,
                IsSql = false,
                SqlProcedureName = string.Empty,
                ExcludeHours = SelectedExcludeHour
            };
            var summaryGuid = await new IntegroServiceProxy().SaveSummaryExchTaskAsync(param);

            //await new IntegroServiceProxy().AddEditSummaryAsync(
            //        new AddEditSummaryParameterSet
            //        {
            //            Id = _id,
            //            Name = Name,
            //            Descriptor = Descriptor,
            //            TransformFileName = TransformFileName,
            //            PeriodType = SelectedPeriodType.PeriodType,
            //            SystemId = _selectedSytemId,
            //            SessionType = SelectedSessionDataType.Name

            //        });
            DialogResult = true;
        }

        private SessionDataTypeDTO GetSessionDataTypeDTO(string type)
        {
            SessionDataType value;             
            return new SessionDataTypeDTO() { Item = (Enum.TryParse(type, out value) ? value : SessionDataType.RT) };
        }

        private SessionDataTypeDTO GetSessionDataTypeDTO(SessionDataType? sessionDataType)
        {
            SessionDataType value;
            return new SessionDataTypeDTO() { Item = sessionDataType ?? SessionDataType.RT };
        }
        public string Caption => $"{(Guid.Empty.Equals(_id) ? "Создание" : "Изменение")} сводки";
    }

    public class SessionDataTypeDTO
    {
        public string Name { get { return Item.ToString(); } }

        public SessionDataType Item { get; set; }
    }
}
