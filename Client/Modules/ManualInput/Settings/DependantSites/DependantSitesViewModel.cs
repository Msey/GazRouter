using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ManualInput.DependantSites;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;


namespace GazRouter.ManualInput.Settings.DependantSites
{
    [RegionMemberLifetime(KeepAlive = true)]
    public class DependantSitesViewModel : LockableViewModel
    {
        private SiteItem _selectedSite;

        public DependantSitesViewModel(bool isReadOnly)
        {
            AddCommand = new DelegateCommand(Add, () => SelectedSite != null && !SelectedSite.IsDependant && !isReadOnly);
            RemoveCommand = new DelegateCommand(Remove, () => SelectedSite != null && SelectedSite.IsDependant && !isReadOnly);

            Refresh();
        }

        public List<SiteItem> SiteList { get; set; }

        public SiteItem SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                if (SetProperty(ref _selectedSite, value))
                    UpdateCommands();
            }
        }


        public DelegateCommand AddCommand { get; set; }

        private void Add()
        {
            var vm = new AddDependantSiteViewModel(SiteList.Select(s => s.Dto).ToList(), SelectedSite.Dto.Id, Refresh);
            var v = new AddDependantSiteView {DataContext = vm};
            v.ShowDialog();
        }

        public DelegateCommand RemoveCommand { get; set; }
        private async void Remove()
        {
            Lock();
            await new ManualInputServiceProxy().RemoveDependantSiteAsync(
                new AddRemoveDependantSiteParameterSet
                {
                    SiteId = SiteList.Single(s => s.Dto.DependantSiteIdList.Contains(SelectedSite.Dto.Id)).Dto.Id,
                    DependantSiteId = SelectedSite.Dto.Id
                });
            Unlock();

            Refresh();
        }


        private void UpdateCommands()
        {
            AddCommand.RaiseCanExecuteChanged();
            RemoveCommand.RaiseCanExecuteChanged();
        }


        private async void Refresh()
        {
            if (!UserProfile.Current.Site.IsEnterprise) return;

            Lock();

            var siteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                new GetSiteListParameterSet
                {
                    EnterpriseId = UserProfile.Current.Site.Id
                });

            SiteList = new List<SiteItem>();

            foreach (var site in siteList.Where(s => siteList.All(x => !x.DependantSiteIdList.Contains(s.Id))))
            {
                var siteItem = new SiteItem(site);
                siteItem.DependantSites.AddRange(
                    siteList.Where(s => site.DependantSiteIdList.Contains(s.Id))
                        .Select(s => new SiteItem(s) {IsDependant = true}));
                SiteList.Add(siteItem);
            }
            OnPropertyChanged(() => SiteList);

            UpdateCommands();
            
            Unlock();
        }

    }


    public class SiteItem
    {
        public SiteItem(SiteDTO dto)
        {
            Dto = dto;
            DependantSites = new List<SiteItem>();
        }

        public SiteDTO Dto { get; set; }

        public bool IsDependant { get; set; }
        
        public List<SiteItem> DependantSites { get; set; }
    }
}