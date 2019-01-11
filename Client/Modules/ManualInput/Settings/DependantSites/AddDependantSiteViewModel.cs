using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DTO.ManualInput.DependantSites;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.Prism.Commands;

namespace GazRouter.ManualInput.Settings.DependantSites
{
    public class AddDependantSiteViewModel : DialogViewModel
    {
        private Guid _siteId;

        public AddDependantSiteViewModel(List<SiteDTO> siteList, Guid siteId, Action closeCallback)
            : base(closeCallback)
        {
            SaveCommand = new DelegateCommand(Save);

            _siteId = siteId;
            SiteList = siteList.Where(s => s.Id != siteId && s.DependantSiteIdList.Count == 0).ToList();
            OnPropertyChanged(() => SiteList);
        }
       

        public List<SiteDTO> SiteList { get; set; }

        public SiteDTO SelectedSite { get; set; }

        public DelegateCommand SaveCommand { get; set; }


        private async void Save()
        {
            Lock();

            await new ManualInputServiceProxy().AddDependantSiteAsync(
                new AddRemoveDependantSiteParameterSet
                {
                    SiteId = _siteId,
                    DependantSiteId = SelectedSite.Id
                });
           
            Unlock();

            DialogResult = true;
        }
    }
}
