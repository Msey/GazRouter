using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Dialogs.EntityPicker;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Entities;
using GazRouter.DTO.ObjectModel.Entities.IsInputOff;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;


namespace GazRouter.ManualInput.Settings.InputOffEntities
{
    [RegionMemberLifetime(KeepAlive = true)]
    public class InputOffEntitiesViewModel : LockableViewModel
    {
        public InputOffEntitiesViewModel(bool isReadOnly)
        {
            RefreshCommand = new DelegateCommand(Load);
            AddCommand = new DelegateCommand(Add, () => !isReadOnly);
            RemoveCommand = new DelegateCommand(Remove, () => !isReadOnly && SelectedEntity != null);

            Init();
        }


        public List<SiteDTO> SiteList { get; set; }

        private async void Init()
        {
            SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                new GetSiteListParameterSet
                {
                    EnterpriseId = UserProfile.Current.Site.Id
                });
            OnPropertyChanged(() => SiteList);

            SelectedSite = SiteList.FirstOrDefault();
        } 


        private SiteDTO _selectedSite;
        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                if (SetProperty(ref _selectedSite, value))
                {
                    Load();
                }
            }
        }

        public List<CommonEntityDTO> EntityList { get; set; }

        public DelegateCommand RefreshCommand { get; set; }

        private async void Load()
        {
            if (SelectedSite == null) return;
            Lock();

            EntityList = await new ObjectModelServiceProxy().GetEntityListAsync(
                new GetEntityListParameterSet
                {
                    SiteId = SelectedSite.Id,
                    ShowVirtual = true,
                    IsInputOff = true
                });
            OnPropertyChanged(() => EntityList);
            
            Unlock();
        }


        private CommonEntityDTO _selectedEntity;
        public CommonEntityDTO SelectedEntity
        {
            get { return _selectedEntity; }
            set
            {
                if(SetProperty(ref _selectedEntity, value))
                {
                    RemoveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        public DelegateCommand AddCommand { get; set; }

        private EntityPickerDialogViewModel _picker;
        private void Add()
        {
            var allowedTypes = new List<EntityType>
            {
                EntityType.CompShop,
                EntityType.CompStation,
                EntityType.CompUnit,
                EntityType.DistrStation,
                EntityType.DistrStationOutlet,
                EntityType.MeasStation,
                EntityType.MeasLine,
                EntityType.ReducingStation,
                EntityType.Valve
            };

            _picker = new EntityPickerDialogViewModel(async () =>
            {
                if (_picker.SelectedItem != null)
                {
                    await new ObjectModelServiceProxy().SetIsInputOffAsync(
                        new SetIsInputOffParameterSet
                        {
                            EntityId = _picker.SelectedItem.Id,
                            IsInputOff = true
                        });
                    Load();
                }
            }, allowedTypes, true);
            var v = new EntityPickerDialogView { DataContext = _picker };
            v.ShowDialog();
        }


        public DelegateCommand RemoveCommand { get; set; }

        private async void Remove()
        {
            await new ObjectModelServiceProxy().SetIsInputOffAsync(
                new SetIsInputOffParameterSet
                {
                    EntityId = SelectedEntity.Id,
                    IsInputOff = false
                });
            Load();
        }

    }
}