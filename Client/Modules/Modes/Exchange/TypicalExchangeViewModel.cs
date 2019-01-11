using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Browser;
using System.Windows.Controls;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.Controls;
using GazRouter.DataProviders.Bindings;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Bindings.ExchangeEntities;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Entities;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using UriBuilder = GazRouter.DataProviders.UriBuilder;

namespace GazRouter.Modes.Exchange
{
     [RegionMemberLifetime(KeepAlive = false)]
    public class TypicalExchangeViewModel : LockableViewModel
    {
        private List<EntityTypeDTO> _entityTypeList;
        private List<EnterpriseDTO> _exchangeEnterprises;
        private EnterpriseDTO _selectedEnterprise;
        private CommonEntityDTO _selectedEntity;
        private EntityTypeDTO _selectedEntityType;
        private CommonEntityDTO _selectedPickedEntity;
        private DateTime? _selectedTimeStamp;

        public TypicalExchangeViewModel()
        {
            DownloadCommand = new DelegateCommand(DownloadFile, () => PickedEntityList.Any() && SelectedTimeStamp.HasValue && SelectedEnterprise != null);
            PickItemCommand = new DelegateCommand(Pick, () => SelectedEnterprise != null && SelectedEntity != null);
            UnPickItemCommand = new DelegateCommand(UnPick, () => SelectedEnterprise != null && SelectedPickedEntity != null);
            ImportCommand = new DelegateCommand(Import);
            EntityList = new RangeEnabledObservableCollection<CommonEntityDTO>();
            PickedEntityList = new RangeEnabledObservableCollection<CommonEntityDTO>();
            EntityTypes = ClientCache.DictionaryRepository.EntityTypes;
        }

         private async void Import()
         {
            var dlg = new OpenFileDialog
            {
                Multiselect = false,
                Filter = "Document Files|*.xsl; *.xml"
            };
            if (dlg.ShowDialog() != true)
            {
                return;
            }
            using (var fileStream = dlg.File.OpenText())
            {
                var xml = fileStream.ReadToEnd();
                await _provider.ImportTypicalExchangeAsync(xml);
            }
         }

         public List<EntityTypeDTO> EntityTypes { get; set; }

        public RangeEnabledObservableCollection<CommonEntityDTO> PickedEntityList { get; set; }

        protected IEnumerable<EntityType> AllowedTypes
        {
            get
            {
                yield return EntityType.CompStation;
                yield return EntityType.DistrStation;
                yield return EntityType.ReducingStation;
                yield return EntityType.MeasStation;
            }
        }


        public EnterpriseDTO SelectedEnterprise
        {
            get { return _selectedEnterprise; }
            set
            {
                if (_selectedEnterprise == value) return;
                _selectedEnterprise = value;
                if (_selectedEnterprise == null) return;
                LoadPickedEntities();
                OnPropertyChanged(() => SelectedEnterprise);
                CommandsRaiseCanExecuteChanged();
            }
        }

        public List<EntityTypeDTO> EntityTypeList
        {
            get { return _entityTypeList; }
            set
            {
                _entityTypeList = value;
                OnPropertyChanged(() => EntityTypeList);
            }
        }


        public EntityTypeDTO SelectedEntityType
        {
            get { return _selectedEntityType; }
            set
            {
                if (SetProperty(ref _selectedEntityType, value))
                    LoadEntities();
              }
        }


        public List<EnterpriseDTO> ExchangeEnterprises
        {
            get { return _exchangeEnterprises; }
            set
            {
                _exchangeEnterprises = value;
                OnPropertyChanged(() => ExchangeEnterprises);
            }
        }

        public CommonEntityDTO SelectedEntity
        {
            get { return _selectedEntity; }
            set
            {
                _selectedEntity = value;
                OnPropertyChanged(() => SelectedEntity);
                CommandsRaiseCanExecuteChanged();
            }
        }

        public CommonEntityDTO SelectedPickedEntity
        {
            get { return _selectedPickedEntity; }
            set
            {
                _selectedPickedEntity = value;
                OnPropertyChanged(() => SelectedPickedEntity);
                CommandsRaiseCanExecuteChanged();
            }
        }

        public RangeEnabledObservableCollection<CommonEntityDTO> EntityList { get; set; }

        public DelegateCommand PickItemCommand { get; set; }

        public DelegateCommand UnPickItemCommand { get; set; }

        public DelegateCommand DownloadCommand { get; set; }

         public DelegateCommand ImportCommand { get; set; }

        public DateTime? SelectedTimeStamp
        {
            get { return _selectedTimeStamp; }
            set
            {
                _selectedTimeStamp = value;
                OnPropertyChanged(() => SelectedTimeStamp);
                CommandsRaiseCanExecuteChanged();
            }
        }

        private bool _isSelected;
        private readonly DataExchangeServiceProxy _provider = new DataExchangeServiceProxy();

         public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(() => IsSelected);
            }
        }


         public void DownloadFile()
        {
            HtmlPage.Window.Navigate(UriBuilder.GetTypicalExchangeHandlerUri(SelectedEnterprise.Id, SelectedTimeStamp.Value));
        }

        private async void UnPick()
        {
            await _provider.DeleteTypicalExchangeEntityAsync(
                new DeleteTypicalExchangeEntityParameterSet
                    {
                        EntityId = SelectedPickedEntity.Id,
                        EnterpriseId = SelectedEnterprise.Id
                    });

            if (SelectedPickedEntity.EntityType == SelectedEntityType.EntityType)
            {
                EntityList.Add(SelectedPickedEntity);
            }
            PickedEntityList.Remove(SelectedPickedEntity);
        }

        private void Pick()
        {
            _provider.AddTypicalExchangeEntityAsync(
                new AddTypicalExchangeEntityParameterSet {EntityId = SelectedEntity.Id, EnterpriseId = SelectedEnterprise.Id}
                );

            SelectedEntity.Name = EntityTypes.Single(et => et.Id == (int)SelectedEntity.EntityType).ShortName;
            PickedEntityList.Add(SelectedEntity);
            EntityList.Remove(SelectedEntity);

        }


        private async void LoadEntities()
        {
            var list = await new ObjectModelServiceProxy().GetEntityListAsync(
                new GetEntityListParameterSet
                {
                    EntityType = SelectedEntityType.EntityType,
                    EnterpriseId = Settings.EnterpriseId
                });
            
            EntityList.Clear();
            EntityList.AddRange(list.Where(r => PickedEntityList.All(pe => pe.Id != r.Id)).ToList());
        }


        private async void LoadPickedEntities()
        {
            var result = await _provider.GetTypicalExchangeEntityListAsync(
                new GetTypicalExchangeEntityListParameterSet
                {
                    EnterpriseId = SelectedEnterprise.Id
                });

            foreach (var item in result)
            {
                item.Name = EntityTypes.Single(et => et.Id == (int)item.EntityType).ShortName;
            }
            PickedEntityList.Clear();
            PickedEntityList.AddRange(result);
            EntityList.RemoveRange(EntityList.Where(r => PickedEntityList.Any(pe => pe.Id == r.Id)).ToList());
        }

        private void CommandsRaiseCanExecuteChanged()
        {
            PickItemCommand.RaiseCanExecuteChanged();
            UnPickItemCommand.RaiseCanExecuteChanged();
            DownloadCommand.RaiseCanExecuteChanged();
        }

        public async void Refresh()
        {
            EntityTypeList = ClientCache.DictionaryRepository.EntityTypes.Where(et => AllowedTypes.Contains(et.EntityType)).ToList();
            SelectedEntityType = EntityTypeList.FirstOrDefault();
            var enterpriseDtos = await new DataExchangeServiceProxy().GetEnterpriseExchangeNeighbourListAsync();
            ExchangeEnterprises = enterpriseDtos;
            SelectedEnterprise = ExchangeEnterprises.FirstOrDefault();
        }
    }
}