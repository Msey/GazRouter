using System;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Dashboards;
using GazRouter.DTO.Dashboards.Dashboard;
namespace GazRouter.Modes.ProcessMonitoring.Dashboards.AddEditDashboard
{
    public class AddEditDashboardViewModel : AddEditViewModelBase<DashboardDTO, int>
    {
        private readonly int? _folderId;
        private readonly Guid _site;
		
        public AddEditDashboardViewModel(int? folderId, 
                                         Guid site,
                                         Action<int> closeCallback) : base(closeCallback)
        {
            _folderId = folderId;
            _site    = site;
        }
        public AddEditDashboardViewModel(DashboardDTO model, Action<int> closeCallback)
            : base(closeCallback, model)
        {
            Name = model.DashboardName;
        }
        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name)
             && Name != Model.DashboardName;
        }
        protected override Task<int> CreateTask => new DashboardServiceProxy()
            .AddDashboardWithPermissionAsync(new AddDashboardPermissionParameterSet
        {
            FolderId = _folderId,
            DashboardName = Name,
            Site = _site,
            PeriodTypeId = DTO.Dictionaries.PeriodTypes.PeriodType.Twohours,
        });
        protected override Task UpdateTask => new DashboardServiceProxy().EditDashboardAsync(
            new EditDashboardParameterSet
            {
                DashboardName = Name,
                DashboardId = Model.Id,
                FolderId = Model.FolderId,
                SortOrder = Model.SortOrder,
                PeriodTypeId = DTO.Dictionaries.PeriodTypes.PeriodType.Twohours,
            });
        protected override string CaptionEntityTypeName => "инфопанели";
	}
}
#region trash
//        public Task<int> method_name()
//        {
//
//            new DashboardServiceProxy().AddDashboardAsync(
//                new AddDashboardParameterSet
//                {
//                    FolderId = _folderId,
//                    DashboardName = Name,
//                    PeriodTypeId = DTO.Dictionaries.PeriodTypes.PeriodType.Twohours,
//                };
//        }
#endregion
