using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ExcelReports;
using GazRouter.DTO.Dashboards.Dashboard;
using GazRouter.DTO.Dictionaries.PeriodTypes;
namespace GazRouter.Modes.ExcelReports
{
    public class AddEditExcelReportsViewModel : AddEditViewModelBase<DashboardDTO, int>
    {
#region constructor
        public AddEditExcelReportsViewModel(int? folderId, Guid site, Action<int> closeCallback) : base(closeCallback)
        {
            _folderId  = folderId;
            _site      = site;
            LoadPeriodTypes();
        }
        public AddEditExcelReportsViewModel(Action<int> closeCallback, DashboardDTO model)
            : base(closeCallback, model)
        {
            Name = model.DashboardName;
            PeriodType = ClientCache.DictionaryRepository.PeriodTypes.SingleOrDefault(pt => pt.PeriodType == model.PeriodTypeId);
            LoadPeriodTypes();
        }
#endregion
        private readonly Guid _site;
        private readonly int? _folderId;
		private PeriodTypeDTO _periodType;
        private void LoadPeriodTypes()
        {
			ListPeriodTypes = ClientCache.DictionaryRepository.PeriodTypes
                .Where(dto => dto.PeriodType == DTO.Dictionaries.PeriodTypes.PeriodType.Twohours || 
                              dto.PeriodType == DTO.Dictionaries.PeriodTypes.PeriodType.Day)
                                 .ToList();
        }
#region ListPeriodTypes
        private List<PeriodTypeDTO> _listPeriodTypes = new List<PeriodTypeDTO>();
        public List<PeriodTypeDTO> ListPeriodTypes
        {
            get { return _listPeriodTypes; }
            set
            {
                if (_listPeriodTypes == value) return;
                _listPeriodTypes = value;
                OnPropertyChanged(() => ListPeriodTypes);
            }
        }
#endregion
        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name)  &&
                   PeriodType != null           &&
                   (Name != Model.DashboardName || 
                   PeriodType.PeriodType != Model.PeriodTypeId);
        }
        protected override Task<int> CreateTask => new ExcelReportServiceProxy()
            .AddExcelWithPermissionAsync(
            new AddDashboardPermissionParameterSet
            {
                DashboardName  = Name,
                Site          = _site,
                FolderId       = _folderId,
                PeriodTypeId   = PeriodType?.PeriodType ?? DTO.Dictionaries.PeriodTypes.PeriodType.None,
            });
        protected override Task UpdateTask => new ExcelReportServiceProxy().EditDashboardAsync(
            new EditDashboardParameterSet
            {
                FolderId = Model.FolderId,
                DashboardName = Name,
                PeriodTypeId = PeriodType.PeriodType,
                SortOrder = Model.SortOrder,
                DashboardId = Model.Id,
            });
        public PeriodTypeDTO PeriodType
        {
            get { return _periodType; }
            set {

                if (SetProperty(ref _periodType, value))
                {
                    _periodType = value;
                    OnPropertyChanged(() => PeriodType);
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }
        protected override string CaptionEntityTypeName => "отчеты";
    }
}

//public AddEditExcelReportsViewModel(int? folderId, Action<int> closeCallback)
//            : base(closeCallback)
//        {
//    _folderId = folderId;
//    LoadPeriodTypes();
//}