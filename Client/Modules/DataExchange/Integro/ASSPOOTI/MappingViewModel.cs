using DataExchange.ASDU;
using DataExchange.Integro.Summary;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataExchange.CustomSource;
using GazRouter.DataExchange.ExchangeLog;
using GazRouter.DataExchange.Integro.ASSPOOTI;
using GazRouter.DataProviders.DataExchange;
using GazRouter.DataProviders.Integro;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.DataExchange.Asdu;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.Integro;
using GazRouter.DTO.DataExchange.Integro.Enum;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.Aggregators;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Sites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Utils.Extensions;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using UriBuilder = GazRouter.DataProviders.UriBuilder;
using ItemBase = DataExchange.ASDU.ItemBase;
using GazRouter.DTO.Dictionaries.Integro;

namespace DataExchange.Integro.ASSPOOTI
{
    //public class MappingViewModel : LockableViewModel
    //{
    //    private List<AsduPropertyDTO> _propertyBindings;
    //    private SiteDTO _selectedSite;
    //    private ItemBase _selectedItem;
    //    private const String FILE_FILTER = "Text files (*.txt)|*.txt";

    //    public MappingViewModel()
    //    {
    //        InitCommands();
    //        Init();
    //    }

    //    #region Commands

    //    private void InitCommands()
    //    {
    //        RefreshSummariesCommand = new DelegateCommand(RefreshSummaries);
    //        EditSummaryCommand = new DelegateCommand(EditSummary, () => SelectedSummary != null /*& editPermission*/);
    //        AddSummaryCommand = new DelegateCommand(AddSummary, () => true/* editPermission*/);
    //        RemoveSummaryCommand = new DelegateCommand(RemoveSummary, () => SelectedSummary != null /*& editPermission*/);
    //        SaveExporSummaryCommand = new DelegateCommand(SaveExporSummary, () => SelectedSummary != null /*& editPermission*/);
    //        ExportSummaryCommand = new DelegateCommand(ExportSummary, () => SelectedSummary != null /*& editPermission*/);
    //        LoadDescriptorCommand = new DelegateCommand(LoadDescriptor, () => SelectedSummary != null && (SelectedSystem.Id == (int)MappingSourceType.ASSPOOTI || (SelectedSystem.Id == (int)MappingSourceType.ASDU_ESG && !MappingDescriptorList.Any())));
    //        LoadSummaryCommand = new DelegateCommand(LoadFromFileSummary, () => SelectedSummary != null);
    //        LinkParamCommand = new DelegateCommand(LinkParam, () => IsLinkEnabled);
    //        UnLinkParamCommand = new DelegateCommand(UnLinkParam, () => IsUnLinkEnabled);
    //        AddParamCommand = new DelegateCommand(AddLinkParam, () => IsAddLinkEnabled);
    //        LogListCommand = new DelegateCommand(OpenLogList, () => IsAddLinkEnabled);
    //    }

    //    public DelegateCommand RefreshSummariesCommand { get; private set; }
    //    public DelegateCommand AddSummaryCommand { get; private set; }
    //    public DelegateCommand EditSummaryCommand { get; private set; }
    //    public DelegateCommand RemoveSummaryCommand { get; private set; }
    //    public DelegateCommand ExportSummaryCommand { get; private set; }
    //    public DelegateCommand SaveExporSummaryCommand { get; private set; }

    //    //
    //    public DelegateCommand LoadSummaryCommand { get; private set; }
    //    public DelegateCommand LoadDescriptorCommand { get; private set; }
    //    public DelegateCommand LinkParamCommand { get; private set; }
    //    public DelegateCommand UnLinkParamCommand { get; private set; }
    //    public DelegateCommand AddParamCommand { get; private set; }
    //    public DelegateCommand LogListCommand { get; private set; }

    //    #endregion

    //    #region Prop
    //    public List<SummaryItem> SummariesList { get; set; }

    //    //public List<ExchangeTaskDTO> TaskList { get; set; }

    //    private SummaryItem _selectedSummary;
    //    public SummaryItem SelectedSummary
    //    {
    //        get { return _selectedSummary; }
    //        set
    //        {
    //            if (SetProperty(ref _selectedSummary, value))
    //            {
    //                LoadSummeryParam(value);
    //                RefreshCommands();
    //            }
    //        }
    //    }
    ////    TaskList = await new DataExchangeServiceProxy().GetExchangeTaskListAsync(
    ////            new GetExchangeTaskListParameterSet
    ////            {
    ////                SourceId = AstraDataSourceId,
    ////                ExchangeTypeId = (ExchangeType?) SelectedExchangeType.Id
    ////}
    ////        );
    //    private void LoadSummeryParam(SummaryItem value)
    //    {
    //        try
    //        {
    //            Behavior.TryLock();
    //            if (MappingDescriptorList != null)
    //                MappingDescriptorList.Clear();
    //            if (MappingParamList != null)
    //                MappingParamList.Clear();                
    //            if (value != null)
    //            {
    //                if (!string.IsNullOrEmpty(value.Dto.Descriptor))
    //                {
    //                    var index = value.Dto.Descriptor.IndexOf(";");
    //                    if (index > 0)
    //                    {
    //                        //var fileName = value.Dto.Descriptor.Substring(index+1, value.Dto.Descriptor.Length - index-1);
    //                        var fileName = value.Dto.Descriptor.Substring(0, index);
    //                        LoadDescriptor(fileName);
    //                        //GetLinkParam();
    //                    }
    //                }
    //                GetLinkParam();
    //            }
    //        }
    //        finally
    //        {
    //            Behavior.TryUnlock();
    //        }

    //    }

    //    private DateTime selDate = SeriesHelper.GetCurrentSession(); //new DateTime(DateTime.Now.Year,DateTime.Now.Month, DateTime.Now.Day);
    //    public DateTime SelDate
    //    {
    //        get { return selDate; }
    //        set
    //        {
    //            SetProperty(ref selDate, value);
    //        }
    //    }
    //    /// <summary>
    //    ///     Список ЛПУ
    //    /// </summary>
    //    public List<SiteDTO> SiteList { get; set; }

    //    /// <summary>
    //    ///     Выбранное ЛПУ
    //    /// </summary>
    //    public SiteDTO SelectedSite
    //    {
    //        get { return _selectedSite; }
    //        set
    //        {
    //            if (SetProperty(ref _selectedSite, value))
    //            {
    //                Refresh();
    //            }
    //        }
    //    }

    //    public IEnumerable<string> TypeList
    //    {
    //        get
    //        {
    //            yield return "КС";
    //            yield return "ГРС";
    //            yield return "ГИС";
    //            yield return "ПРГ";
    //            yield return "ЗРА";
    //            yield return "Расчётный объект";
    //        }
    //    }

    //    private string selectedType = "КС";
    //    public string SelectedType
    //    {
    //        get { return selectedType; }
    //        set
    //        {
    //            if (SetProperty(ref selectedType, value))
    //            {
    //                Refresh();
    //            }
    //        }
    //    }

    //    public List<ItemBase> Items { get; set; }

    //    public ItemBase SelectedItem
    //    {
    //        get { return _selectedItem; }
    //        set
    //        {
    //            if (SetProperty(ref _selectedItem, value))
    //            {
    //                LoadPropertyList();
    //            }
    //        }
    //    }

    //    public List<MappingPropertyItem> PropertyList { get; set; }

    //    private MappingPropertyItem selectedProperty;
    //    public MappingPropertyItem SelectedProperty
    //    {
    //        get { return selectedProperty; }
    //        set
    //        {
    //            if (SetProperty(ref selectedProperty, value))
    //            {

    //                RefreshLinkCommands();
    //                if (selectedProperty != null)
    //                {
    //                    //SelectedMappingParam = MappingParamList.FirstOrDefault(f => f.EntityId == selectedProperty.EntityId);
    //                }
    //            }
    //        }
    //    }

    //    public List<SystemItem> Systems
    //    {
    //        get
    //        {
    //            return new List<SystemItem>
    //            {                    
    //                new SystemItem { Id = (int)MappingSourceType.ASDU_ESG, Name = "АСДУ ЕСГ", SourceType = MappingSourceType.ASDU_ESG},
    //                //new SystemItem { Id = (int)MappingSystemType.ASSPOOTI, Name = "АССПООТИ" },
    //            };
    //        }
    //    }

    //    private SystemItem _selectedSystem;
    //    public SystemItem SelectedSystem
    //    {
    //        get { return _selectedSystem; }
    //        set
    //        {
    //            if (SetProperty(ref _selectedSystem, value))
    //            {
    //                RefreshSummaries();
    //                IsReadOnlyDesc = (value.Id == (int)MappingSourceType.ASSPOOTI);
    //                //IsDescriptorVisibil = (value.Id == (int)MappingSystemType.ASSPOOTI);
    //            }
    //        }
    //    }

    //    private bool isDescriptorVisibil = true;
    //    public bool IsDescriptorVisibil
    //    {
    //        get { return isDescriptorVisibil; }
    //        set
    //        {
    //            SetProperty(ref isDescriptorVisibil, value);
    //        }
    //    }

    //    private bool isReadOnlyDesc = true;
    //    public bool IsReadOnlyDesc
    //    {
    //        get { return isReadOnlyDesc; }
    //        set
    //        {
    //            SetProperty(ref isReadOnlyDesc, value);
    //        }
    //    }

    //    private bool CanLoadSummary()
    //    {
    //        return true;
    //    }

    //    public bool IsLinkEnabled
    //    {
    //        get { return SelectedDescriptor != null && SelectedProperty != null; }
    //    }

    //    public bool IsAddLinkEnabled
    //    {
    //        get { return !MappingParamList.Any(a => string.IsNullOrEmpty(a.ParameterDescriptorName) || a.PropertyTypeId == null); }
    //    }

    //    public bool IsUnLinkEnabled
    //    {
    //        get { return SelectedMappingParam != null; }
    //    }

    //    private ObservableCollection<MappingDescriptorDto> mappingDescriptorList = new ObservableCollection<MappingDescriptorDto>();
    //    public ObservableCollection<MappingDescriptorDto> MappingDescriptorList
    //    {
    //        get { return mappingDescriptorList; }
    //        set
    //        {
    //            if (SetProperty(ref mappingDescriptorList, value))
    //            {
    //            }
    //        }
    //    }


    //    private MappingDescriptorDto selectedDescriptor;
    //    public MappingDescriptorDto SelectedDescriptor
    //    {
    //        get { return selectedDescriptor; }
    //        set
    //        {
    //            if (SetProperty(ref selectedDescriptor, value))
    //            {
    //                RefreshLinkCommands();
    //            }
    //        }
    //    }

    //    private bool isFilterByEntity = false;
    //    public bool IsFilterByEntity
    //    {
    //        get { return isFilterByEntity; }
    //        set
    //        {
    //            if (SetProperty(ref isFilterByEntity, value))
    //            {
    //                RefreshLinkCommands();
    //            }
    //        }
    //    }
        

    //    private ObservableCollection<MappingParamDto> mappingParamList = new ObservableCollection<MappingParamDto>();
    //    public ObservableCollection<MappingParamDto> MappingParamList
    //    {
    //        get { return mappingParamList;  }
    //        set
    //        {
    //            if (SetProperty(ref mappingParamList, value))
    //            {
    //            }
    //        }
    //    }

    //    private MappingParamDto selectedMappingParam;
    //    public MappingParamDto SelectedMappingParam
    //    {
    //        get { return selectedMappingParam; }
    //        set
    //        {
    //            if (SetProperty(ref selectedMappingParam, value))
    //            {
    //                RefreshLinkCommands();
    //            }
    //        }
    //    }

    //    #endregion

    //    public async void Init()
    //    {
    //        // получить список ЛПУ
    //        if (UserProfile.Current.Site.IsEnterprise)
    //        {
    //            SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
    //                new GetSiteListParameterSet
    //                {
    //                    EnterpriseId = UserProfile.Current.Site.Id
    //                });
    //        }
    //        OnPropertyChanged(() => SiteList);

    //        _selectedSite = SiteList.FirstOrDefault();
    //        OnPropertyChanged(() => SelectedSite);

    //        RefreshSummaries();
    //        Refresh();
    //    }
    //    private async void SetSummaryExchTasks()
    //    {
    //        var paramSummary = new GetSummaryParameterSet() { SystemId = SelectedSystem.Id };
    //        var summaries = await new IntegroServiceProxy().GetSummariesListByParamsAsync(paramSummary); // IntegroServiceProxy
    //        var paramTask = new GetExchangeTaskListParameterSet() { SourceId = (int)DataExchangeSources.Asduesg };
    //        var taskList = await new DataExchangeServiceProxy().GetExchangeTaskListAsync(paramTask);

    //        var summaryTasks =
    //            from s in summaries
    //            join t in taskList on s.ExchangeTaskId equals t.Id into ps
    //            from p in ps.DefaultIfEmpty()
    //            select new SummaryItem(s,p);

    //        SummariesList = summaryTasks.ToList();
    //        OnPropertyChanged(() => SummariesList);
    //    }
    //    private async void RefreshSummaries()
    //    {
    //        if (SelectedSystem == null)
    //            return;
    //        //GetSummariesSystemId(SelectedSystem.Id);
    //        //if (SelectedSite == null)
    //        //{
    //        //    return;
    //        //}
    //        if (MappingDescriptorList != null)
    //            MappingDescriptorList.Clear();
    //        if (MappingParamList != null)
    //            MappingParamList.Clear();
    //        try
    //        {
    //            Behavior.TryLock();
    //            //var parameters = new GetSummaryParameterSet() { SystemId = SelectedSystem.Id };
    //            //var summaries = await new IntegroServiceProxy().GetSummariesListByParamsAsync(parameters); // IntegroServiceProxy
    //            //SummariesList = summaries.Select(s => new SummaryItem(s)).ToList();
    //            SetSummaryExchTasks();
    //        }
    //        finally
    //        {
    //            Behavior.TryUnlock();
    //        }
    //    }

    //    //private async void GetSummariesSystemId(int systemId)
    //    //{
    //    //    try
    //    //    {
    //    //        Behavior.TryLock();
    //    //        var parameters = new GetSummaryParameterSet() { SystemId = systemId };
    //    //        var summaries = await new IntegroServiceProxy().GetSummariesListByParamsAsync(parameters);
    //    //        SummariesList = summaries.Select(s => new SummaryItem(s,null)).ToList();
    //    //        OnPropertyChanged(() => SummariesList);
    //    //    }
    //    //    finally
    //    //    {
    //    //        Behavior.TryUnlock();
    //    //    }
    //    //}

    //    private void AddSummary()
    //    {
    //        var viewModel = new AddEditSummaryViewModel(RefreshSummaries, SelectedSystem);
    //        var view = new AddEditSummaryView { DataContext = viewModel };
    //        view.ShowDialog();
    //    }

    //    private void EditSummary()
    //    {
    //        var summExch = new SummaryExchTaskDTO() { Summary = SelectedSummary.Dto, ExchangeTask = SelectedSummary.TaskDto };
    //        var viewModel = new AddEditSummaryViewModel(RefreshSummaries, summExch, SelectedSystem);
    //        var view = new AddEditSummaryView { DataContext = viewModel };
    //        view.ShowDialog();
    //    }

    //    private void RemoveSummary()
    //    {            
    //        var dp = new DialogParameters
    //        {
    //            Closed = async (s, e) =>
    //            {
    //                if (e.DialogResult.HasValue && e.DialogResult.Value)
    //                {
    //                    await new IntegroServiceProxy().DeleteSummaryAsync(SelectedSummary.Dto.Id);
    //                    RefreshSummaries();
    //                }
    //            },
    //            Content = "Вы уверены, что хотите удалить сводку?",
    //            Header = "Удаление сводки",
    //            OkButtonContent = "Да",
    //            CancelButtonContent = "Нет"
    //        };

    //        RadWindow.Confirm(dp);
    //    }

    //    private void SaveExporSummary()
    //    {
    //        var summaryId = SelectedSummary.Dto.Id;
    //        var dt = ((DateTime)SelDate).ToLocal();
    //        //var seriesId = 
    //        HtmlPage.Window.Navigate(GetExchangeHandlerUri(summaryId, dt, 0, (int)SelectedSummary.Dto.PeriodType,SelectedSystem.Id));
    //    }

    //    public static Uri GetExchangeHandlerUri(Guid summaryId, DateTime dt, int seriesId, int periodTypeId,int systemTypeId, bool xmlOnly = false)
    //    {
    //        var result = new System.UriBuilder(HtmlPage.Document.DocumentUri.OriginalString.ToLower().Replace(@"/default.aspx", string.Empty).TrimEnd('/'))
    //        {
    //            Query = $"summaryId={summaryId}&dt={dt.Ticks}&seriesId={seriesId}&periodTypeId={periodTypeId}&systemTypeId={systemTypeId}"
    //        };
    //        result.Path += $"/{DateTime.Now.Ticks}.axml";
    //        return result.Uri;
    //    }
    //    private async void ExportSummary()
    //    {
    //        try
    //        {
    //            Behavior.TryLock();
    //            // Уточнить.
    //            var offset = (int)TimeZoneInfo.Local.BaseUtcOffset.TotalHours - Settings.ServerTimeUtcOffset;
    //            ExportSummaryParams parameters = new ExportSummaryParams()
    //            {
    //                PeriodDate = offset != 0 ? SelDate.AddHours(-offset) : SelDate, // уточнить
    //                SummaryId = SelectedSummary.Dto.Id,
    //                SystemId = SelectedSystem.Id,
    //                PeriodType = SelectedSummary.Dto.PeriodType,
    //            };
    //            var summaries = await new IntegroServiceProxy().ExportSummaryAsync(parameters);
    //            if (summaries.ResultType == ExportResultType.Error)
    //            {
    //                throw new ServerException(new Exception(summaries.Description));
    //            }
    //            else
    //                MessageBox.Show(String.Format("Файл '{0}' успешно сформирован. ", summaries.Description));
    //        }
    //        finally
    //        {
    //            Behavior.TryUnlock();
    //        }
    //    }

    //    private async void Refresh()
    //    {
    //        if (SelectedSite == null)
    //        {
    //            return;
    //        }

    //        try
    //        {
    //            Behavior.TryLock();


    //            _propertyBindings =
    //                await new DataExchangeServiceProxy().GetAsduPropertyListAsync(new GetAsduEntityListParameterSet());

    //            // Получить список объектов для обмена
    //            var entityBindings = GetEntityBindings();

    //            if (selectedType == "КС")
    //            {
    //                // Получить список ГПА
    //                var stationTree = await new ObjectModelServiceProxy().GetCompStationTreeAsync(SelectedSite.Id);

    //                Items = new List<ItemBase>();

    //                foreach (var station in stationTree.CompStations)
    //                {
    //                    var stationBinding = new AsduEntityDTO
    //                    {
    //                        EntityId = station.Id,
    //                        EntityName = station.Name,
    //                        EntityTypeId = EntityType.CompStation,
    //                        EntityGid =
    //                            _propertyBindings.Where(pb => pb.EntityId == station.Id)
    //                                .Select(pb => pb.EntityGid)
    //                                .FirstOrDefault(),
    //                        IsActive = entityBindings.Any(id => station.Id == id)
    //                    };
    //                    var stationItem = new BindableItem(stationBinding, UpdateEntityBinding);
    //                    Items.Add(stationItem);

    //                    foreach (var shop in stationTree.CompShops.Where(cs => cs.ParentId == station.Id))
    //                    {
    //                        var shopBinding = new AsduEntityDTO
    //                        {
    //                            EntityId = shop.Id,
    //                            EntityName = shop.Name,
    //                            EntityTypeId = EntityType.CompShop,
    //                            EntityGid =
    //                                _propertyBindings.Where(pb => pb.EntityId == shop.Id)
    //                                    .Select(pb => pb.EntityGid)
    //                                    .FirstOrDefault(),
    //                            IsActive = entityBindings.Any(id => shop.Id == id)
    //                        };
    //                        var shopItem = new BindableItem(shopBinding, UpdateEntityBinding);
    //                        shopItem.Parent = stationItem;
    //                        stationItem.Children.Add(shopItem);

    //                        var unitFolder = new GroupItem("ГПА");
    //                        unitFolder.Parent = shopItem;
    //                        shopItem.Children.Add(unitFolder);
    //                        // ГПА
    //                        foreach (var unit in stationTree.CompUnits.Where(u => u.ParentId == shop.Id))
    //                        {
    //                            var unitBinding = new AsduEntityDTO
    //                            {
    //                                EntityId = unit.Id,
    //                                EntityName = unit.Name,
    //                                EntityGid =
    //                                    _propertyBindings.Where(pb => pb.EntityId == unit.Id)
    //                                        .Select(pb => pb.EntityGid)
    //                                        .FirstOrDefault(),
    //                                EntityTypeId = EntityType.CompUnit,
    //                                IsActive = entityBindings.Any(id => unit.Id == id)
    //                            };

    //                            var unitItem = new BindableItem(unitBinding, UpdateEntityBinding);
    //                            unitItem.Parent = unitFolder;
    //                            unitFolder.Children.Add(unitItem);
    //                        }

    //                        // Точки измерения параметров газа (физ-хим. показатели)
    //                        foreach (var mp in stationTree.MeasPoints.Where(mp => mp.ParentId == shop.Id))
    //                        {
    //                            var mpBinding = new AsduEntityDTO
    //                            {
    //                                EntityId = mp.Id,
    //                                EntityName = mp.Name,
    //                                EntityGid =
    //                                    _propertyBindings.Where(pb => pb.EntityId == mp.Id)
    //                                        .Select(pb => pb.EntityGid)
    //                                        .FirstOrDefault(),
    //                                EntityTypeId = EntityType.MeasPoint,
    //                                IsActive = entityBindings.Any(id => mp.Id == id)
    //                            };

    //                            var mpItem = new BindableItem(mpBinding, UpdateEntityBinding);
    //                            mpItem.Parent = shopItem;
    //                            shopItem.Children.Add(mpItem);
    //                        }
    //                    }
    //                }
    //            }

    //            if (selectedType == "ГРС")
    //            {
    //                var distrTree = await new ObjectModelServiceProxy().GetDistrStationTreeAsync(
    //                    new GetDistrStationListParameterSet
    //                    {
    //                        SiteId = SelectedSite.Id
    //                    });

    //                Items = new List<ItemBase>();

    //                foreach (var ds in distrTree.DistrStations)
    //                {
    //                    var dsBinding = new AsduEntityDTO
    //                    {
    //                        EntityId = ds.Id,
    //                        EntityName = ds.Name,
    //                        EntityGid =
    //                            _propertyBindings.Where(pb => pb.EntityId == ds.Id)
    //                                .Select(pb => pb.EntityGid)
    //                                .FirstOrDefault(),
    //                        EntityTypeId = EntityType.DistrStation,
    //                        IsActive = entityBindings.Any(id => ds.Id == id)
    //                    };

    //                    var dsItem = new BindableItem(dsBinding, UpdateEntityBinding);

    //                    Items.Add(dsItem);

    //                    var fldrItem = new GroupItem("Выходы");
    //                    fldrItem.Parent = dsItem;
    //                    dsItem.Children.Add(fldrItem);

    //                    // Выходы
    //                    foreach (var dso in distrTree.DistrStationOutlets.Where(o => o.ParentId == ds.Id))
    //                    {
    //                        var dsoBinding =
    //                            new AsduEntityDTO
    //                            {
    //                                EntityId = dso.Id,
    //                                EntityName = dso.Name,
    //                                EntityGid =
    //                                    _propertyBindings.Where(pb => pb.EntityId == dso.Id)
    //                                        .Select(pb => pb.EntityGid)
    //                                        .FirstOrDefault(),
    //                                EntityTypeId = EntityType.DistrStationOutlet,
    //                                IsActive = entityBindings.Any(id => dso.Id == id)
    //                            };

    //                        var dsoItem = new BindableItem(dsoBinding, UpdateEntityBinding);
    //                        dsoItem.Parent = fldrItem;
    //                        fldrItem.Children.Add(dsoItem);
    //                    }

    //                    // Точки измерения параметров газа (физ-хим. показатели)
    //                    foreach (var mp in distrTree.MeasPoints.Where(mp => mp.ParentId == ds.Id))
    //                    {
    //                        var mpBinding =
    //                            new AsduEntityDTO
    //                            {
    //                                EntityId = mp.Id,
    //                                EntityName = mp.Name,
    //                                EntityTypeId = EntityType.MeasPoint,
    //                                EntityGid =
    //                                    _propertyBindings.Where(pb => pb.EntityId == mp.Id)
    //                                        .Select(pb => pb.EntityGid)
    //                                        .FirstOrDefault(),
    //                                IsActive = entityBindings.Any(id => mp.Id == id)
    //                            };

    //                        var mpItem = new BindableItem(mpBinding, UpdateEntityBinding);
    //                        mpItem.Parent = dsItem;
    //                        dsItem.Children.Add(mpItem);
    //                    }
    //                }
    //            }

    //            if (selectedType == "ГИС")
    //            {
    //                var measTree = await new ObjectModelServiceProxy().GetMeasStationTreeAsync(
    //                    new GetMeasStationListParameterSet { SiteId = SelectedSite.Id });

    //                Items = new List<ItemBase>();

    //                foreach (var ms in measTree.MeasStations)
    //                {
    //                    var msItem = new GroupItem(ms.Name);
    //                    Items.Add(msItem);
    //                    foreach (var ml in measTree.MeasLines.Where(m => m.ParentId == ms.Id))
    //                    {
    //                        var dsBinding =
    //                            new AsduEntityDTO
    //                            {
    //                                EntityId = ml.Id,
    //                                EntityName = ml.Name,
    //                                EntityGid =
    //                                    _propertyBindings.Where(pb => pb.EntityId == ml.Id)
    //                                        .Select(pb => pb.EntityGid)
    //                                        .FirstOrDefault(),
    //                                EntityTypeId = EntityType.MeasLine,
    //                                IsActive = entityBindings.Any(id => ml.Id == id)
    //                            };

    //                        var dlItem = new BindableItem(dsBinding, UpdateEntityBinding);
    //                        dlItem.Parent = msItem;
    //                        msItem.Children.Add(dlItem);
    //                    }
    //                }
    //            }

    //            if (selectedType == "ПРГ")
    //            {
    //                var rsList = await new ObjectModelServiceProxy().GetReducingStationListAsync(
    //                    new GetReducingStationListParameterSet
    //                    {
    //                        SiteId = SelectedSite.Id
    //                    });

    //                Items = new List<ItemBase>();

    //                foreach (var rs in rsList)
    //                {
    //                    var rsBinding =
    //                        new AsduEntityDTO
    //                        {
    //                            EntityId = rs.Id,
    //                            EntityName = rs.Name,
    //                            EntityGid =
    //                                _propertyBindings.Where(pb => pb.EntityId == rs.Id)
    //                                    .Select(pb => pb.EntityGid)
    //                                    .FirstOrDefault(),
    //                            EntityTypeId = EntityType.MeasLine,
    //                            IsActive = entityBindings.Any(id => rs.Id == id)
    //                        };

    //                    Items.Add(new BindableItem(rsBinding, UpdateEntityBinding));
    //                }
    //            }

    //            if (selectedType == "ЗРА")
    //            {
    //                var pipeTree = await new ObjectModelServiceProxy().GetPipelineTreeAsync(SelectedSite.Id);

    //                Items = new List<ItemBase>();

    //                foreach (var pipeType in ClientCache.DictionaryRepository.PipelineTypes.Values)
    //                {
    //                    var typeItem = new GroupItem(pipeType.Name) { IsExpanded = false };
    //                    foreach (var pipe in pipeTree.Pipelines.Where(p => p.Type == pipeType.PipelineType))
    //                    {
    //                        var pipeBinding =
    //                            new AsduEntityDTO
    //                            {
    //                                EntityId = pipe.Id,
    //                                EntityName = pipe.Name,
    //                                EntityGid =
    //                                    _propertyBindings.Where(pb => pb.EntityId == pipe.Id)
    //                                        .Select(pb => pb.EntityGid)
    //                                        .FirstOrDefault(),
    //                                EntityTypeId = EntityType.Pipeline,
    //                                IsActive = entityBindings.Any(id => pipe.Id == id)
    //                            };

    //                        var pipeItem = new BindableItem(pipeBinding, UpdateEntityBinding) { IsExpanded = false };
    //                        pipeItem.Parent = typeItem;
    //                        typeItem.Children.Add(pipeItem);

    //                        foreach (var valve in pipeTree.LinearValves.Where(v => v.ParentId == pipe.Id))
    //                        {
    //                            var valveBinding =
    //                                new AsduEntityDTO
    //                                {
    //                                    EntityId = valve.Id,
    //                                    EntityGid =
    //                                        _propertyBindings.Where(pb => pb.EntityId == valve.Id)
    //                                            .Select(pb => pb.EntityGid)
    //                                            .FirstOrDefault(),
    //                                    EntityName = valve.Name,
    //                                    EntityTypeId = EntityType.Valve,
    //                                    IsActive = entityBindings.Any(id => valve.Id == id)
    //                                };

    //                            var valveItem = new BindableItem(valveBinding, UpdateEntityBinding);
    //                            valveItem.Parent = pipeItem;
    //                            pipeItem.Children.Add(valveItem);
    //                        }
    //                    }
    //                    if (typeItem.Children.Any())
    //                    {
    //                        Items.Add(typeItem);
    //                    }
    //                }
    //            }
    //            if (selectedType == "Расчётный объект")
    //            {

    //                var aggrList = await new ObjectModelServiceProxy().GetAggregatorListAsync(new GetAggregatorListParameterSet
    //                {
    //                    SiteId = SelectedSite.Id
    //                });
    //                Items = new List<ItemBase>();
    //                foreach (var rs in aggrList)
    //                {
    //                    var rsBinding =
    //                        new AsduEntityDTO
    //                        {
    //                            EntityId = rs.Id,
    //                            EntityName = rs.Name,
    //                            EntityGid =
    //                                _propertyBindings.Where(pb => pb.EntityId == rs.Id)
    //                                    .Select(pb => pb.EntityGid)
    //                                    .FirstOrDefault(),
    //                            EntityTypeId = EntityType.Aggregator,
    //                            IsActive = entityBindings.Any(id => rs.Id == id)
    //                        };

    //                    Items.Add(new BindableItem(rsBinding, UpdateEntityBinding));
    //                }
    //            }

    //            OnPropertyChanged(() => Items);
    //        }
    //        finally
    //        {
    //            Behavior.TryUnlock();
    //        }
    //    }

    //    private async void Refresh1()
    //    {
    //        if (SelectedSite == null)
    //        {
    //            return;
    //        }

    //        try
    //        {
    //            Behavior.TryLock();



    //            _propertyBindings = await new DataExchangeServiceProxy().GetAsduPropertyListAsync(new GetAsduEntityListParameterSet());

    //            // Получить список объектов для обмена
    //            var entityBindings = GetEntityBindings();

    //            // Получить список ГПА
    //            var stationTree = await new ObjectModelServiceProxy().GetCompStationTreeAsync(SelectedSite.Id);

    //            Items = new List<ItemBase>();

    //            foreach (var station in stationTree.CompStations)
    //            {
    //                var stationBinding = new AsduEntityDTO
    //                {
    //                    EntityId = station.Id,
    //                    EntityName = station.Name,
    //                    EntityTypeId = EntityType.CompStation,
    //                    EntityGid =
    //                        _propertyBindings.Where(pb => pb.EntityId == station.Id)
    //                            .Select(pb => pb.EntityGid)
    //                            .FirstOrDefault(),
    //                    IsActive = entityBindings.Any(id => station.Id == id)
    //                };
    //                var stationItem = new BindableItem(stationBinding, UpdateEntityBinding);
    //                Items.Add(stationItem);

    //                foreach (var shop in stationTree.CompShops.Where(cs => cs.ParentId == station.Id))
    //                {
    //                    var shopBinding = new AsduEntityDTO
    //                    {
    //                        EntityId = shop.Id,
    //                        EntityName = shop.Name,
    //                        EntityTypeId = EntityType.CompShop,
    //                        EntityGid =
    //                            _propertyBindings.Where(pb => pb.EntityId == shop.Id)
    //                                .Select(pb => pb.EntityGid)
    //                                .FirstOrDefault(),
    //                        IsActive = entityBindings.Any(id => shop.Id == id)
    //                    };
    //                    var shopItem = new BindableItem(shopBinding, UpdateEntityBinding);
    //                    shopItem.Parent = stationItem;
    //                    stationItem.Children.Add(shopItem);

    //                    foreach (var unit in stationTree.CompUnits.Where(u => u.ParentId == shop.Id))
    //                    {
    //                        var unitBinding = new AsduEntityDTO
    //                        {
    //                            EntityId = unit.Id,
    //                            EntityName = unit.Name,
    //                            EntityGid =
    //                                _propertyBindings.Where(pb => pb.EntityId == unit.Id)
    //                                    .Select(pb => pb.EntityGid)
    //                                    .FirstOrDefault(),
    //                            EntityTypeId = EntityType.CompUnit,
    //                            IsActive = entityBindings.Any(id => unit.Id == id)
    //                        };

    //                        var unitItem = new BindableItem(unitBinding, UpdateEntityBinding);
    //                        unitItem.Parent = shopItem;
    //                        shopItem.Children.Add(unitItem);
    //                    }
    //                }
    //            }

    //            OnPropertyChanged(() => Items);
    //        }
    //        finally
    //        {
    //            Behavior.TryUnlock();
    //        }
    //    }

    //    private void LoadDescriptor(string fileName)
    //    {
    //        if (string.IsNullOrEmpty(fileName))
    //            return;

    //        System.IO.FileStream fileStream = null;
    //        if (string.IsNullOrEmpty(fileName))
    //        {
    //            OpenFileDialog openFileDialog = new OpenFileDialog();
    //            if (openFileDialog.ShowDialog() == true)
    //            {
    //                fileStream = openFileDialog.File.OpenRead();
    //            }
    //        }
    //        else if ((File.Exists(fileName)))
    //        {
    //            fileStream = File.OpenRead(fileName);
    //        }
    //        if (fileStream != null)
    //        {
    //            MappingDescriptorList = new ObservableCollection<MappingDescriptorDto>(MappingHelper.ParsingDescriptorFile(fileStream));
    //        }
    //    }
    //    private async void LoadFromFileSummary()
    //    {
    //        OpenFileDialog openFileDialog = new OpenFileDialog();
    //        //openFileDialog.Filter = FILE_FILTER;
    //        if (openFileDialog.ShowDialog() == true)
    //        {
    //            try
    //            {

    //                //SelectedSummary.Dto.Descriptor = openFileDialog.File.Name + ";";
    //                System.IO.FileStream fileStream = openFileDialog.File.OpenRead();
    //                var parameters = MappingHelper.ParsingSummaryFile(fileStream, SelectedSummary);
    //                if (parameters == null || !parameters.Any())
    //                    return;
    //                List<AddEditSummaryPParameterSet> lex = new List<AddEditSummaryPParameterSet>();
    //                foreach (var item in MappingParamList)
    //                {
    //                    var ex = parameters.FirstOrDefault(f =>
    //                                      f.ParametrGid == item.ParameterDescriptorName &&
    //                                      f.ContentList.Any(a1 => a1.EntityId == item.EntityId && a1.PropertyTypeId == item.PropertyTypeId));
    //                    if (ex != null)
    //                    {
    //                        lex.Add(ex);
    //                        parameters.Remove(ex);
    //                    }
    //                }
    //                SummatyLoadResult res = await new IntegroServiceProxy().AddSummaryParamListAsync(parameters);
    //                res.DublicateEntity = lex;
    //            }
    //            catch (Exception e)
    //            {
    //                throw new Exception($"Ошибка чтения файла:{e.Message} {e.StackTrace}");
    //            }
    //            LoadSummeryParam(SelectedSummary);
    //            //UpdateSummary();
    //        }
    //    }

    //    private void LoadDescriptor()
    //    {

    //        if (SelectedSystem.Id == (int)MappingSourceType.ASSPOOTI)
    //        {
    //            OpenFileDialog openFileDialog = new OpenFileDialog();
    //            //openFileDialog.Filter = FILE_FILTER;

    //            if (openFileDialog.ShowDialog() == true)
    //            {
    //                try
    //                {
    //                    //SelectedSummary.Dto.Descriptor = openFileDialog.File.Name.Substring(0, openFileDialog.File.Name.IndexOf(openFileDialog.File.Extension));
    //                    //SelectedSummary.Descriptor = openFileDialog.File.Name.Substring(0, openFileDialog.File.Name.IndexOf(openFileDialog.File.Extension));
    //                    SelectedSummary.Dto.Descriptor = openFileDialog.File.Name + ";";
    //                    System.IO.FileStream fileStream = openFileDialog.File.OpenRead();
    //                    MappingDescriptorList = new ObservableCollection<MappingDescriptorDto>(MappingHelper.ParsingDescriptorFile(fileStream));
    //                }
    //                catch (Exception e)
    //                {
    //                    throw new Exception($"Ошибка чтения файла:{e.Message} {e.StackTrace}");
    //                }
    //                UpdateSummary();
    //            }
    //        }
    //        else
    //        {
    //            if (MappingDescriptorList == null)
    //                MappingDescriptorList = new ObservableCollection<MappingDescriptorDto>();
    //            MappingDescriptorList.Add(new MappingDescriptorDto());
    //        }
    //        RefreshLinkCommands();
    //    }

    //    private async void UpdateSummary()
    //    {
    //        var param = new AddEditSummaryParameterSet
    //        {
    //            Id = SelectedSummary.Dto.Id,
    //            Name = SelectedSummary.Dto.Name,
    //            Descriptor = SelectedSummary.Dto.Descriptor,
    //            TransformFileName = SelectedSummary.Dto.TransformFileName,
    //            PeriodType = SelectedSummary.Dto.PeriodType,
    //            SystemId = SelectedSummary.Dto.SystemId,

    //        };
    //        await new IntegroServiceProxy().AddEditSummaryAsync(param);
    //    }

    //    private void OpenLogList()
    //    {
    //        //var vm = new ExchangeLogViewModel(x => Refresh(), ((DataSourceItem)SelectedItem).Dto.Id);
    //        //var v = new ExchangeLogView { DataContext = vm };
    //        //v.ShowDialog();
    //       // var vm = new ExchangeLogViewModel();
    //        var v = new ExchangeLogCatalog { DataContext = null };
    //        v.ShowDialog();
    //    }
    //    private void AddLinkParam()
    //    {
    //        SelectedMappingParam = new MappingParamDto();
    //        MappingParamList.Add(SelectedMappingParam);
    //        RefreshLinkCommands();
    //    }

    //    private async void GetLinkParam()
    //    {
    //        try
    //        {
    //            var mappParamList = new List<MappingParamDto>();
    //            var res = await new IntegroServiceProxy().GetSummariesParamListAsync(SelectedSummary.Dto.Id);
    //            foreach (var item in res)
    //            {
    //                foreach (var con in item.SummaryParamContentList)
    //                {
    //                    var newItem = new MappingParamDto()
    //                    {
    //                        PropertyTypeName = item.Name,
    //                        PropertyTypeId = con.PropertyTypeId,
    //                        EntityId = con.EntityId,
    //                        ParameterDescriptorName = item.ParameterGid,
    //                        SummaryParameterId = item.Id
    //                    };
    //                    mappParamList.Add(newItem);
    //                }
    //            }
    //            MappingParamList = new ObservableCollection<MappingParamDto>(mappParamList.OrderBy(o => o.PropertyTypeName));
    //        }
    //        finally
    //        {
    //            Behavior.TryUnlock();
    //        }
    //    }

    //    private async void LinkParam()
    //    {
    //        if (SelectedMappingParam == null)
    //            return;
    //        if (!Guid.Empty.Equals(SelectedMappingParam.SummaryParameterId) && SelectedMappingParam.EntityId != null && SelectedMappingParam.EntityId != Guid.Empty)
    //        {
    //            var mes = MessageBox.Show("Перепривязать параметр?", "Привязка.", MessageBoxButton.OKCancel);
    //            if (mes == MessageBoxResult.Cancel)
    //                return;
    //            //MessageBoxProvider.Confirm("Перепривязать параметр?", confirmed =>
    //            //{
    //            //    if (!confirmed)
    //            //    {
    //            //        return;
    //            //    }
    //            //}, "Привязка");
    //        }

    //        SelectedMappingParam.ParameterDescriptorId = SelectedDescriptor.Id;
    //        SelectedMappingParam.ParameterDescriptorName = (SelectedSystem.Id == (int)MappingSourceType.ASSPOOTI) ? SelectedDescriptor.Key : SelectedDescriptor.Description;
    //        //
    //        SelectedMappingParam.EntityId = SelectedProperty.EntityId;
    //        SelectedMappingParam.PropertyTypeId = (int)SelectedProperty.PropertyTypeId;
    //        SelectedMappingParam.PropertyTypeName = String.Format("{0}.{1}.{2}.{3}", 
    //            SelectedSite.Name, 
    //            selectedType,
    //            //SelectedItem.Name, 
    //            SelectedItem.GetParentName(),
    //            SelectedProperty.Name );
    //        if (Guid.Empty.Equals(SelectedMappingParam.SummaryParameterId))
    //        {
    //            SelectedMappingParam.SummaryParameterId = Guid.NewGuid();
    //        }
    //        var parameters = new AddEditSummaryPParameterSet()
    //        {
    //            SummaryParamId = (Guid)SelectedMappingParam.SummaryParameterId,
    //            SummaryId = SelectedSummary.Dto.Id,
    //            Name = SelectedMappingParam.PropertyTypeName,
    //            ParametrGid = SelectedMappingParam.ParameterDescriptorName,
    //            Aggregate = string.Empty,
    //            ContentList = new List<AddEditSummaryPContentParameterSet>()
    //            { new AddEditSummaryPContentParameterSet()  {
    //                SummaryParamId = (Guid)SelectedMappingParam.SummaryParameterId,
    //                EntityId = SelectedMappingParam.EntityId,
    //                PropertyTypeId = (int)SelectedMappingParam.PropertyTypeId}
    //            }
    //        };
    //        await new IntegroServiceProxy().AddEditSummaryParamAsync(parameters); // IntegroServiceProxy
    //        if (SelectedSystem.Id == (int)MappingSourceType.ASDU_ESG)
    //        {
    //            SelectedDescriptor.Description = string.Empty;
    //        }

    //        RefreshLinkCommands();
    //    }

    //    private async void UnLinkParam()
    //    {
    //        if (SelectedMappingParam == null)
    //            return;
    //        MessageBoxProvider.Confirm("Удалить привязку?", confirmed =>
    //        {
    //            if (confirmed)
    //            {
    //                try
    //                {
    //                    Behavior.TryLock(); //await
    //                    new IntegroServiceProxy().DeleteSummaryParamAsync(SelectedMappingParam.SummaryParameterId); // IntegroServiceProxy
    //                    MappingParamList.Remove(SelectedMappingParam);
    //                }
    //                finally
    //                {
    //                    Behavior.TryUnlock();
    //                }
    //                RefreshLinkCommands(); 
    //            }
    //        }, "Привязка");
    //    }
    //    public bool UpdateEntityBinding(AsduEntityDTO dto)
    //    {
    //        new DataExchangeServiceProxy().SetAsduEntityAsync(
    //            new SetAsduPropertyParameterSet
    //            {
    //                EntityId = dto.EntityId,
    //                ParameterGid = dto.EntityGid,
    //            });

    //        //Items
    //        //    .Where(bi => bi is BindableItem)
    //        //    .Cast<BindableItem>()
    //        //    .FirstOrDefault(bi => bi.EntityId == dto.EntityId);

    //        var bindableItem = SelectedItem as BindableItem;
    //        bindableItem.IsActive = GetEntityBindings().Any(id => bindableItem.EntityId == id);
    //        return true;
    //    }

    //    private List<Guid> GetEntityBindings()
    //    {
    //        return _propertyBindings.GroupBy(dto => dto.EntityId).Select(g => g.Key).ToList();
    //    }

    //    public bool UpdatePropertyBinding(AsduPropertyDTO dto)
    //    {
    //        new DataExchangeServiceProxy().SetAsduPropertyAsync(
    //            new SetAsduPropertyParameterSet
    //            {
    //                EntityId = dto.EntityId,
    //                ParameterGid = dto.ParameterGid,
    //                PropertyTypeId = dto.PropertyTypeId
    //            });

    //        var oldBndng =
    //            _propertyBindings.FirstOrDefault(
    //                b => b.EntityId == dto.EntityId && b.PropertyTypeId == dto.PropertyTypeId);
    //        if (oldBndng != null)
    //        {
    //            oldBndng.ParameterGid = dto.ParameterGid;
    //        }
    //        else
    //        {
    //            _propertyBindings.Add(dto);
    //        }

    //        var bindableItem = SelectedItem as BindableItem;
    //        bindableItem.IsActive = GetEntityBindings().Any(id => bindableItem.EntityId == id);

    //        return true;
    //    }

    //    // Загрузка списка свойств по выбранному объекту
    //    private void LoadPropertyList()
    //    {
    //        PropertyList = new List<MappingPropertyItem>();

    //        var item = SelectedItem as BindableItem;
    //        if (item != null)
    //        {
    //            var propList =
    //                ClientCache.DictionaryRepository.EntityTypes.Single(et => et.EntityType == item.EntityType)
    //                    .EntityProperties;

    //            foreach (var prop in propList)
    //            {
    //                var bnd =
    //                    new AsduPropertyDTO
    //                    {
    //                        EntityId = item.EntityId,
    //                        PropertyTypeId = prop.PropertyType,
    //                        ParameterGid =
    //                            _propertyBindings.FirstOrDefault(
    //                                b => b.EntityId == item.EntityId && b.PropertyTypeId == prop.PropertyType)?
    //                                .ParameterGid
    //                    };
    //                PropertyList.Add(new MappingPropertyItem(prop.Name, bnd));
    //            }
    //        }
    //        OnPropertyChanged(() => PropertyList);
    //    }

    //    private void RefreshLinkCommands()
    //    {
    //        AddParamCommand.RaiseCanExecuteChanged();
    //        LinkParamCommand.RaiseCanExecuteChanged();
    //        UnLinkParamCommand.RaiseCanExecuteChanged();
    //        LoadDescriptorCommand.RaiseCanExecuteChanged();
    //        LoadSummaryCommand.RaiseCanExecuteChanged();
    //    }

    //    private void RefreshCommands()
    //    {
    //        EditSummaryCommand.RaiseCanExecuteChanged();
    //        RemoveSummaryCommand.RaiseCanExecuteChanged();
    //        ExportSummaryCommand.RaiseCanExecuteChanged();
    //        LoadDescriptorCommand.RaiseCanExecuteChanged();
    //        LoadSummaryCommand.RaiseCanExecuteChanged();
    //    }

    //    //public void cboxSystems_Loaded()
    //    //{
    //    //    var x = 1;
    //    //}
    //}
}
