using System;
using System.Threading.Tasks;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Dashboards;
using GazRouter.DTO.Dashboards.Folders;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.AddEditFolder
{
    public class AddEditFolderViewModel : AddEditViewModelBase<FolderDTO, int>
    {
        private readonly int? _folderId;

        public AddEditFolderViewModel(int? folderId, Action<int> callback)
            : base(callback)
        {
            _folderId = folderId;
          
        }

        public AddEditFolderViewModel(FolderDTO model, Action<int> callback)
            : base(callback, model)
        {
            _folderId = model.Id;
            Name = model.Name;
        }
        
        protected override bool OnSaveCommandCanExecute()
        {
            return !string.IsNullOrEmpty(Name)
                && Name != Model.Name;
        }

        protected override Task<int> CreateTask => new DashboardServiceProxy().AddFolderAsync(
            new AddFolderParameterSet
            {
                Name     = Name,
                ParentId = _folderId
            });

        protected override Task UpdateTask => new DashboardServiceProxy().EditFolderAsync(
            new EditFolderParameterSet
            {
                Name = Name,
                FolderId = Model.Id,
                ParentId = Model.ParentId,
                SortOrder = Model.SortOrder
            });
        protected override string CaptionEntityTypeName => "папки";
    }
}