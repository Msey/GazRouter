using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.EntitySelector;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Data;
using GazRouter.Common;


namespace GazRouter.Controls.Dialogs.EntityPicker
{
    public class EntityPickerDialogViewModel : PickerDialogViewModelBase<CommonEntityWithSiteDTO>
    {
        private List<EntityType> _allowedTypes; 

        // Если выставить этот флаг, то объекты смежных предприятий выбираться не будут
        private bool _onlyCurrentEnterpriseEntities;

        public EntityPickerDialogViewModel(Action closeCallback, List<EntityType> allowedTypes, bool onlyCurrentEnterpriseEntities = false)
            : base(closeCallback)
        {
            _allowedTypes = allowedTypes;
            _onlyCurrentEnterpriseEntities = onlyCurrentEnterpriseEntities;

            ApplyFilterCommand = new DelegateCommand(ApplyFilter);
            SelectCommand = new DelegateCommand(() => DialogResult = true, () => SelectedItem != null);

            Init();
        }

        private bool _isInit;

        protected override async void Init()
        {
            Lock();

            _selectedPipelineType = PipelineTypeList.SingleOrDefault(t => t.PipelineType == PipelineType.Main);
            _selectedEntityType = FastEntityTypeList.FirstOrDefault();

            if (UserProfile.Current.Site.IsEnterprise)
            {
                SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                    _onlyCurrentEnterpriseEntities
                    ? new GetSiteListParameterSet { EnterpriseId = UserProfile.Current.Site.Id }
                    : null);
                var siteId = IsolatedStorageManager.Get<Guid?>("LastSelectedSiteId");
                _selectedSite = SiteList.SingleOrDefault(s => (s.Id == siteId));
            }
            else
            {
                SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                    new GetSiteListParameterSet { SiteId = UserProfile.Current.Site.Id });
                SelectedSite = SiteList.FirstOrDefault();
            }
            OnPropertyChanged(() => SiteList);
            OnPropertyChanged(() => SelectedSite);
            //
            _isInit = true;
            ApplyFilter();
            Unlock();
        }

        #region SITES

        public List<SiteDTO> SiteList { get; set; }

        private SiteDTO _selectedSite;
        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                if (SetProperty(ref _selectedSite, value))
                {
                    IsolatedStorageManager.Set("LastSelectedSiteId", value?.Id);
                    if(_isInit) ApplyFilter();
                }
                
            }
        }

        #endregion


        #region ENTITY_TYPES
        private IEnumerable<EntityType> _fullFastTypeList
        {
            get
            {
                yield return EntityType.CompStation;
                yield return EntityType.CompShop;
                yield return EntityType.DistrStation;
                yield return EntityType.ReducingStation;
                yield return EntityType.MeasStation;
                yield return EntityType.Pipeline;
                yield return EntityType.Valve;
                yield return EntityType.CompUnit;
            }
        }

        public IEnumerable<EntityTypeWrapper> FastEntityTypeList
        {
            get
            {
                var types = ClientCache.DictionaryRepository.EntityTypes.Where(t => _fullFastTypeList.Contains(t.EntityType));
                if (_allowedTypes != null)
                    types = types.Where(t => _allowedTypes.Contains(t.EntityType));

                return types.Select(t => new EntityTypeWrapper(t)).ToList();
            }
        }

        public IEnumerable<EntityTypeWrapper> OtherEntityTypeList
        {
            get
            {
                var types = ClientCache.DictionaryRepository.EntityTypes.Where(t => !_fullFastTypeList.Contains(t.EntityType));
                if (_allowedTypes != null)
                    types = types.Where(t => _allowedTypes.Contains(t.EntityType));
                return types.Select(t => new EntityTypeWrapper(t));
            }
        }

        public bool IsOtherEntityTypeSelectorVisible => OtherEntityTypeList.Any();


        private EntityTypeWrapper _selectedEntityType;
        public EntityTypeWrapper SelectedEntityType
        {
            get { return _selectedEntityType; }
            set
            {
                if (SetProperty(ref _selectedEntityType, value))
                {
                    OnPropertyChanged(() => IsPipelineTypeSelectorVisible);
                    if (_isInit) ApplyFilter();
                }
            }
        }

        #endregion


        
        #region PIPELINE_TYPES

        public IEnumerable<PipelineTypesDTO> PipelineTypeList => ClientCache.DictionaryRepository.PipelineTypes.Values;
        
        private PipelineTypesDTO _selectedPipelineType;

        public PipelineTypesDTO SelectedPipelineType
        {
            get { return _selectedPipelineType; }
            set
            {
                if(SetProperty(ref _selectedPipelineType, value))
                    if (_isInit) ApplyFilter();
            }
        }

        //
        public bool IsPipelineTypeSelectorVisible => _selectedEntityType?.Dto?.EntityType == EntityType.Pipeline;

        #endregion

       
        //
        private readonly GetEntitesPageParameterSet _filter = new GetEntitesPageParameterSet();

        public string HighlightTextPath => NamePart;

        private static Guid _lastToken;

        

        protected override void ApplyFilter()
        {
            _filter.SiteId = SelectedSite?.Id;

            if (_filter.NamePart != NamePart)
            {
                _filter.NamePart = NamePart;
                OnPropertyChanged(() => HighlightTextPath);
            }

            _filter.PipeLineType = null;

            _filter.EntityTypes = new List<EntityType>();
            if (SelectedEntityType?.Dto != null)
            {
                _filter.EntityTypes.Add(SelectedEntityType.Dto.EntityType);
                if (SelectedEntityType.Dto.EntityType == EntityType.Pipeline)
                    _filter.PipeLineType = SelectedPipelineType?.PipelineType;
            }
            else
            {
                if (_allowedTypes != null)
                    _filter.EntityTypes.AddRange(_allowedTypes);
            }

            CreateItems();
        }
        

        private void BindingListItemsLoaded(object sender, VirtualQueryableCollectionViewItemsLoadedEventArgs e)
        {
            Items.ItemsLoaded -= BindingListItemsLoaded;
            Items.Refresh();
        }


        protected override async void OnItemsLoading(object sender, VirtualQueryableCollectionViewItemsLoadingEventArgs args)
        {
            base.OnItemsLoading(sender, args);

            Lock();

            SelectedItem = null;
            
            Items.ItemsLoaded += BindingListItemsLoaded;

            _lastToken = Guid.NewGuid();

            var page = await new ObjectModelServiceProxy().GetEntitiesPageAsync(
                new GetEntitesPageParameterSet
                {
                    PageNumber = args.StartIndex/Items.PageSize,
                    PageSize = Items.PageSize,
                    EntityTypes = _filter.EntityTypes,
                    PipeLineType = _filter.PipeLineType,
                    NamePart = _filter.NamePart,
                    Token = _lastToken,
                    SiteId = _filter.SiteId,
                    OnlyCurrentEnterpriseEntities = _onlyCurrentEnterpriseEntities
                });

            if (page.Token == _lastToken)
            {
                if (page.TotalCount != Items.VirtualItemCount)
                {
                    Items.VirtualItemCount = page.TotalCount;
                }
                Items.Load(args.StartIndex, page.Entities);
            }

            Unlock();

        }

    }


    public class EntityTypeWrapper
    {
        public EntityTypeWrapper(EntityTypeDTO typeDto)
        {
            Dto = typeDto;
        }
        public EntityTypeDTO Dto { get; }

        public virtual string Name => Dto?.ShortName ?? "Все";
    }
}
