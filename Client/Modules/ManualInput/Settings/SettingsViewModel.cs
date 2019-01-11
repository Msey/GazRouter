using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.ManualInput.Settings.DependantSites;
using GazRouter.ManualInput.Settings.EntityProperties;
using GazRouter.ManualInput.Settings.InputOffEntities;
using GazRouter.ManualInput.Settings.SerieChecks;
using GazRouter.Modes.GasCosts2;
using Microsoft.Practices.Prism.Regions;
namespace GazRouter.ManualInput.Settings
{
    [RegionMemberLifetime(KeepAlive = true)]
    public class SettingsViewModel : LockableViewModel
    {
        public SettingsViewModel()
        {
            var isReadOnly   = !Authorization2.Inst.IsEditable(LinkType.DataCoollect);
            SerieChecks      = new SerieChecksViewModel(isReadOnly);
            EntityProperties = new EntityPropertiesViewModel(isReadOnly);
            DependantSites   = new DependantSitesViewModel(isReadOnly);
            InputOffEntities = new InputOffEntitiesViewModel(isReadOnly);
            StateVisibility  = new StateVisibilityViewModel(isReadOnly);
        }

        public SerieChecksViewModel SerieChecks { get; set; }

        public EntityPropertiesViewModel EntityProperties { get; set; }

        public DependantSitesViewModel DependantSites { get; set; }

        public InputOffEntitiesViewModel InputOffEntities { get; set; }

        public StateVisibilityViewModel StateVisibility { get; set; }
    }
    

}