using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DispatcherTask;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.DispatcherTasks.Tasks;
using GazRouter.DTO.ObjectModel.Sites;

namespace GazRouter.Modes.DispatcherTasks.Dialogs.AddMultiRecordTask
{
    public class AddMultiRecordTaskViewModel : AddEditViewModelBase<TaskDTO, Guid>
    {

        public AddMultiRecordTaskViewModel(Action<Guid> actionBeforeClosing)
            : base(actionBeforeClosing)
        {
            CompletionDate = DateTime.Now;

            LoadSiteList();
        }



        private string _subject;

        public string Subject
        {
            get { return _subject; }
            set
            {
                if (SetProperty(ref _subject, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }


        private string _description;

        public string Description
        {
            get { return _description; }
            set
            {
                if (SetProperty(ref _description, value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }


        public DateTime CompletionDate { get; set; }



        public List<SiteSelectorItem> SiteList { get; set; }

        private async void LoadSiteList()
        {
            if (!UserProfile.Current.Site.IsEnterprise) return;

            Lock();

            var sites = await new ObjectModelServiceProxy().GetSiteListAsync(
                new GetSiteListParameterSet
                {
                    EnterpriseId = UserProfile.Current.Site.Id
                });

            SiteList = sites.Select(s => new SiteSelectorItem(s, SaveCommand.RaiseCanExecuteChanged)).ToList();
            OnPropertyChanged(() => SiteList);

            IsAllSitesChecked = false;

            Unlock();
        }


        public bool? IsAllSitesChecked
        {
            get
            {
                if (SiteList != null)
                {
                    if (SiteList.All(s => s.IsChecked)) return true;
                    if (SiteList.All(s => !s.IsChecked)) return false;
                }
                return null;
            }
            set
            {
                SiteList.ForEach(s => s.IsChecked = value ?? false);
                OnPropertyChanged(() => IsAllSitesChecked);
            }
        }


        protected override bool OnSaveCommandCanExecute()
        {
            return SiteList != null
                   && SiteList.Any(p => p.IsChecked)
                   && !string.IsNullOrEmpty(Description)
                   && !string.IsNullOrEmpty(Subject);
        }
        

        protected override Task<Guid> CreateTask =>
            new DispatcherTaskServiceProxy().AddMultiRecordTaskAsync(
                new AddTaskParameterSet
                {
                    Subject = Subject,
                    Description = Description,
                    CompletionDate = CompletionDate,
                    SiteIdList = SiteList.Where(s => s.IsChecked).Select(s => s.Dto.Id).ToList()
                });


        protected override string CaptionEntityTypeName => "задания для нескольких ЛПУ";
    }


    public class SiteSelectorItem : PropertyChangedBase
    {
        private readonly Action _updateAction;

        public SiteSelectorItem(SiteDTO dto, Action updateAction)
        {
            Dto = dto;
            _updateAction = updateAction;
        }

        public SiteDTO Dto { get; set; }


        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                if (SetProperty(ref _isChecked, value))
                    _updateAction?.Invoke();
            }
        }
    }
}
