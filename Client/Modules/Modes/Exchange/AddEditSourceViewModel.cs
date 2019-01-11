using System;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Bindings;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DTO.Bindings.Sources;
using GazRouter.DTO.DataExchange.DataSource;

namespace GazRouter.Modes.Exchange
{
    public class AddEditSourceViewModel : AddEditViewModelBase<Source, int>
    {
        public AddEditSourceViewModel(Action<int> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            Init();
        }


        public AddEditSourceViewModel(Action<int> actionBeforeClosing, DataSourceDTO es)
            : base(actionBeforeClosing, es)
        {
            Init();
	        Description = es.Description;
			IsHidden = es.IsHidden;
			IsReadonly = es.IsReadonly;
			SourceId = es.SourceId;
			SourceName = es.SourceName;
			SystemName = es.SystemName;
            Id = es.SourceId;
            IsReadonly = es.IsReadonly;
        }


        private void Init()
        {
            IsReadonly = false;
        }

        protected override bool OnSaveCommandCanExecute()
        {
            return true;
            //return !this.IsReadonly && !string.IsNullOrEmpty(this.SourceName) && !string.IsNullOrEmpty(this.SystemName) ;
        }

        public string SourceName
        {
            get { return _sourceName; }
            set
            {
                _sourceName = value; 
                OnPropertyChanged(() => SourceName);
                OnSaveCommandCanExecute();
            }
        }

        protected override Task<int> CreateTask
        {
            get
            {
                return new DataExchangeServiceProxy().AddDataSourceAsync(new AddDataSourceParameterSet
                {
                    Description = Description,
                    Name = SourceName,
                    SysName = SystemName
                });
            }
        }

        protected override Task UpdateTask
        {
            get
            {
                return new DataExchangeServiceProxy().EditDataSourceAsync(new EditDataSourceParameterSet
                {
                    Description = Description,
                    Id = (int) SourceId,
                    Name = SourceName,
                    SysName = SystemName,
                });
            }
        }

        public bool IsHidden { get; set; }
        private int? _sourceId;
        private string _systemName;
        private string _sourceName;
        private bool _isReadonly;

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

        protected override string CaptionEntityTypeName
        {
            get { return " источника"; }
        }

        public string SystemName
        {
            get { return _systemName; }
            set
            {
                _systemName = value; 
                OnPropertyChanged(() => SystemName);
                OnSaveCommandCanExecute();
            }
        }

        public bool IsReadonly
        {
            get { return _isReadonly; }
            set
            {
                _isReadonly = value; 
                OnPropertyChanged(() => IsReadonly);
                OnSaveCommandCanExecute();
            }
        }
    }
}