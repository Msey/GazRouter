using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Browser;
using GazRouter.Application;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.DataExchange.Asdu;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.Prism.Commands;
using Utils.Extensions;
using UriBuilder = GazRouter.DataProviders.UriBuilder;

namespace DataExchange.ASDU
{
    public class AsduSourceViewModel : LockableViewModel
    {
        private List<AsduPropertyDTO> _propertyBindings;

        private ItemBase _selectedItem;


        private SiteDTO _selectedSite;
        private DateTime? _selectedTimestamp;

        private string _selectedType;

        private bool _isReadOnly;
        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set
            {
                _isReadOnly = value;
                OnPropertyChanged(() => IsReadOnly);
            }
        }

        public AsduSourceViewModel()
        {
            ///IsReadOnly = Authorization.Instance.IsAuthorized(GetType()) != Permissions.Write;
            
            RefreshCommand = new DelegateCommand(Refresh);
            DownloadCommand = new DelegateCommand(Download, () => SelectedTimestamp.HasValue);

            Init();
        }

        public DelegateCommand RefreshCommand { get; private set; }
        public DelegateCommand DownloadCommand { get; }


        /// <summary>
        ///     Список ЛПУ
        /// </summary>
        public List<SiteDTO> SiteList { get; set; }

        /// <summary>
        ///     Выбранное ЛПУ
        /// </summary>
        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                if (SetProperty(ref _selectedSite, value))
                {
                    Refresh();
                }
            }
        }

        public IEnumerable<string> TypeList
        {
            get
            {
                yield return "КС";
                yield return "ГРС";
                yield return "ГИС";
                yield return "ПРГ";
                yield return "ЗРА";
            }
        }


        public string SelectedType
        {
            get { return _selectedType; }
            set
            {
                if (SetProperty(ref _selectedType, value))
                {
                    Refresh();
                }
            }
        }

        public List<ItemBase> Items { get; set; }

        public ItemBase SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    LoadPropertyList();
                }
            }
        }

        public List<PropertyItem> PropertyList { get; set; }

        public DateTime? SelectedTimestamp
        {
            get { return _selectedTimestamp; }
            set
            {
                if (SetProperty(ref _selectedTimestamp, value))
                {
                    DownloadCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private void Download()
        {
            HtmlPage.Window.Navigate(UriBuilder.GetAsduHandlerUri(PeriodType.Twohours, SelectedTimestamp.Value.ToLocal()));
        }


        public async void Init()
        {
            // получить список ЛПУ
            if (UserProfile.Current.Site.IsEnterprise)
            {
                SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                    new GetSiteListParameterSet
                    {
                        EnterpriseId = UserProfile.Current.Site.Id
                    });
            }
            OnPropertyChanged(() => SiteList);

            _selectedSite = SiteList.First();
            OnPropertyChanged(() => SelectedSite);

            _selectedType = "КС";
            OnPropertyChanged(() => SelectedType);

            Refresh();
        }


        public bool UpdatePropertyBinding(AsduPropertyDTO dto)
        {
            new DataExchangeServiceProxy().SetAsduPropertyAsync(
                new SetAsduPropertyParameterSet
                {
                    EntityId = dto.EntityId,
                    ParameterGid = dto.ParameterGid,
                    PropertyTypeId = dto.PropertyTypeId
                });

            var oldBndng =
                _propertyBindings.FirstOrDefault(
                    b => b.EntityId == dto.EntityId && b.PropertyTypeId == dto.PropertyTypeId);
            if (oldBndng != null)
            {
                oldBndng.ParameterGid = dto.ParameterGid;
            }
            else
            {
                _propertyBindings.Add(dto);
            }

            var bindableItem = SelectedItem as BindableItem;
            bindableItem.IsActive = GetEntityBindings().Any(id => bindableItem.EntityId == id);

            return true;
        }
        public bool UpdateEntityBinding(AsduEntityDTO dto)
        {
            new DataExchangeServiceProxy().SetAsduEntityAsync(
                new SetAsduPropertyParameterSet
                {
                    EntityId = dto.EntityId,
                    ParameterGid = dto.EntityGid,
                });

            //Items
            //    .Where(bi => bi is BindableItem)
            //    .Cast<BindableItem>()
            //    .FirstOrDefault(bi => bi.EntityId == dto.EntityId);

            var bindableItem = SelectedItem as BindableItem;
            bindableItem.IsActive = GetEntityBindings().Any(id => bindableItem.EntityId == id);
            return true;
        }

        private async void Refresh()
        {
           
        }

        private List<Guid> GetEntityBindings()
        {
            return _propertyBindings.GroupBy(dto => dto.EntityId).Select(g => g.Key).ToList();
        }

        // Загрузка списка свойств по выбранному объекту
        private void LoadPropertyList()
        {
            PropertyList = new List<PropertyItem>();

            var item = SelectedItem as BindableItem;
            if (item != null)
            {
                var propList =
                    ClientCache.DictionaryRepository.EntityTypes.Single(et => et.EntityType == item.EntityType)
                        .EntityProperties;

                foreach (var prop in propList)
                {
                    var bnd =
                        new AsduPropertyDTO
                        {
                            EntityId = item.EntityId,
                            PropertyTypeId = prop.PropertyType,
                            ParameterGid =
                                _propertyBindings.FirstOrDefault(
                                    b => b.EntityId == item.EntityId && b.PropertyTypeId == prop.PropertyType)?
                                    .ParameterGid
                        };
                    PropertyList.Add(new PropertyItem(prop.Name, bnd, UpdatePropertyBinding));
                }
            }
            OnPropertyChanged(() => PropertyList);
        }
    }
}