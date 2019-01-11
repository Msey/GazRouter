using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Controls.Tree;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Appearance.Versions;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.BoilerPlants;
using GazRouter.DTO.ObjectModel.Boilers;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.CoolingStations;
using GazRouter.DTO.ObjectModel.CoolingUnit;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.Entities;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.DTO.ObjectModel.PowerPlants;
using GazRouter.DTO.ObjectModel.PowerUnits;
using GazRouter.Flobus;
using GazRouter.Flobus.UiEntities.FloModel;
using GazRouter.Flobus.VM;
using GazRouter.Flobus.VM.FloModel;
using GazRouter.Flobus.VM.Model;
using GazRouter.Modes.ProcessMonitoring.Schema;
using GazRouter.Modes.ProcessMonitoring.Views;
using GazRouter.Modes.ValveStatesChangeLog;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.Flobus.Interfaces;
using GazRouter.Repair.Converters;
using GazRouter.DataProviders.Repairs;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.Flobus.UiEntities.FloModel.Measurings;
using GazRouter.Repair;
using GazRouter.DTO.Dictionaries.RepairTypes;
using System.Windows.Controls;
using Telerik.Windows.Documents.Fixed.Model;
using Telerik.Windows.Documents.Fixed.Model.Editing;
using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Telerik.Windows.Media.Imaging;
using System.Windows.Printing;
using System.IO;
using GazRouter.DTO.Dictionaries.PeriodTypes;

namespace GazRouter.Modes.ProcessMonitoring
{
    public class FloViewControlViewModel : FloControlViewModelBase
    {
        private int? _systemId;
        private bool _isWindowButtonEnabled;
        private FindObjectDialogContent _findObjectDialogContent;
        private ModelTreeDialogContent _modelTreeDialogContent;
        private SchemeVersionItemDTO _selectedSchemeVersion;
        private List<SchemeVersionItemDTO> _schemeVersionList = new List<SchemeVersionItemDTO>();
        private ISearchable _selectedItem;
        private DateTime? _timestamp;
        private ValveStatesDialogContent _valveStatesDialogContent;
        private Visibility _periodSelectorVisibility = Visibility.Collapsed;
        private DelegateCommand<GoToTrendCommandParameter> _gotoTrendCommand;
        private PrintDocument pd;

        public FloViewControlViewModel()
        {
            IsEditPermission = Authorization2.Inst.IsEditable(LinkType.RepairDva);
            ModelTreeCommand = new DelegateCommand(() => ShowModelTreeDialogWindow());
            ValveStatesCommand = new DelegateCommand(() => ShowValveStateDialogWindow());
            FindObjectCommand = new DelegateCommand(() => ShowFindObjectDialogWindow());

            PrintSchemeCommand = new DelegateCommand<FrameworkElement>((schema) => 
            {
                schema.Dispatcher.BeginInvoke(()=> ImageLoader(schema));                
                pd.Print("Scheme");
            });
            PrintToPngSchemeCommand = new DelegateCommand<FrameworkElement>(ExportSchemeToPNG);
            PrintToPdfSchemeCommand = new DelegateCommand<FrameworkElement>(ExportSchemeToPDF);

            pd = new PrintDocument();
            pd.PrintPage += new EventHandler<PrintPageEventArgs>(PrintDocumentPage);

            _timestamp = SeriesHelper.GetLastCompletedSession();
            _selectedPeriod = PeriodTypeList.Single(p => p.PeriodType == PeriodType.Twohours);
            PropertyChanged += FloViewControlViewModel_PropertyChanged;

            LoadSchemeVersionList();
        }
                
        private void FloViewControlViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsModelLoaded")
            {
                OnPropertyChanged(() => ShowDayly);
                OnPropertyChanged(() => IsDayMode);
                OnPropertyChanged(() => IsTwoHoursMode);
                OnPropertyChanged(() => Timestamp);
            }
        }

        private bool _isEditPermission;
        public bool IsEditPermission
        {
            get { return _isEditPermission; }
            set
            {
                _isEditPermission = value;
                OnPropertyChanged(() => IsEditPermission);
            }
        }

        public ISearchable SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged(() => SelectedItem);
            }
        }

        public DelegateCommand ModelTreeCommand { get; set; }
        public DelegateCommand ValveStatesCommand { get; set; }
        public DelegateCommand FindObjectCommand { get; set; }

        public DelegateCommand<FrameworkElement> PrintSchemeCommand { get; set; }
        public DelegateCommand<FrameworkElement> PrintToPngSchemeCommand { get; set; }
        public DelegateCommand<FrameworkElement> PrintToPdfSchemeCommand { get; set; }


        /// <summary>
        ///     Список версий схем, доступный для выбора
        /// </summary>
        public List<SchemeVersionItemDTO> SchemeVersionList
        {
            get { return _schemeVersionList; }
            set { SetProperty(ref _schemeVersionList, value); }
        }

        /// <summary>
        ///     Выбранная версия схемы
        /// </summary>
        public SchemeVersionItemDTO SelectedSchemeVersion
        {
            get { return _selectedSchemeVersion; }
            set
            {
                if (SetProperty(ref _selectedSchemeVersion, value))
                {
                    if (_selectedSchemeVersion != null)
                    {
                        LoadModel(_selectedSchemeVersion.Id);
                    }
                }
            }
        }
      
        public bool IsTwoHoursMode
        {
            get { return IsModelLoaded && !ShowDayly; }
        }
        public bool IsDayMode
        {
            get
            {
                return IsModelLoaded && ShowDayly;
            }
        }

        private bool _showDayly = false;
        /// <summary>
        /// Признак показа в суточном режиме
        /// </summary>
        public bool ShowDayly
        {
            get { return _showDayly; }
            set
            {
                bool old = _showDayly;

                _showDayly = value;
                OnPropertyChanged(() => ShowDayly);
                OnPropertyChanged(() => IsDayMode);
                OnPropertyChanged(() => IsTwoHoursMode);
                if (Timestamp.HasValue && _selectedSchemeVersion != null)
                {
                    LoadMeasurings();
                }
            }
        }


        private PeriodTypeDTO _selectedPeriod;
        /// <summary>
        /// Выбранный тип периода
        /// </summary>
        public PeriodTypeDTO SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                if (SetProperty(ref _selectedPeriod, value))
                {
                    switch (value.PeriodType)
                    {
                        case PeriodType.Twohours:
                            ShowDayly = false;
                            break;

                        case PeriodType.Day:
                            ShowDayly = true;
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// Список периодов
        /// </summary>

        public List<PeriodTypeDTO> PeriodTypeList =>
            ClientCache.DictionaryRepository.PeriodTypes.Where( p => p.PeriodType == PeriodType.Twohours || p.PeriodType == PeriodType.Day).ToList();

        //ShowRepair

        private bool _showRepair = false;
        /// <summary>
        /// Признак показа в суточном режиме
        /// </summary>
        public bool ShowRepair
        {
            get { return _showRepair; }
            set
            {
                _showRepair = value;
                OnPropertyChanged(() => ShowRepair);
                if (Timestamp.HasValue && _selectedSchemeVersion != null)
                {
                    LoadRepairList();
                }
            }
        }

        public System.Globalization.CultureInfo CultureWithFormattedPeriod
        {
            get
            {
                var tempCultureInfo = new System.Globalization.CultureInfo("ru-RU") { DateTimeFormat = { ShortDatePattern = "dd MMMM yyyy"} };
                return tempCultureInfo;
            }
        }

        /// <summary>
        ///     Загрузка доступных пользователю версий схем, по одной версии для каждой ГТС.
        ///     Список отображается в виде отпадающего списка ГТС в тулбаре. Пользователь
        ///     выбирает ГТС и для выбранной ГТС загружается последняя опубликованная схема.
        /// </summary>
        /// <summary>
        ///     Выбранный сеанс данных
        /// </summary>
        public DateTime? Timestamp
        {
            get { return _timestamp; }
            set
            {
                if (SetProperty(ref _timestamp, value))
                {
                    if (_selectedSchemeVersion != null)
                    {
                        LoadMeasurings();
                        LoadRepairList();
                    }
                }
            }
        }
                
        public Visibility PeriodSelectorVisibility
        {
            get { return _periodSelectorVisibility; }
            set
            {
                _periodSelectorVisibility = value;
                OnPropertyChanged(() => PeriodSelectorVisibility);
            }
        }

        public ValveStatesDialogContent ValveStatesDialogContent
        {
            get
            {
                if (_valveStatesDialogContent == null)
                {
                    ValveStatesChangeLog = new ValveStatesChangeLogMainViewModel();
                    ValveStatesChangeLog.PropertyChanged += ValveStatesChangeLogOnPropertyChanged;

                    var conf = IsolatedStorageManager.SchemaValveStatesDialogConfig;
                    _valveStatesDialogContent = new ValveStatesDialogContent
                    {
                        Height = conf.Height,
                        Width = conf.Width,
                        Left = conf.Left,
                        Top = conf.Top,
                        DataContext = new ValveStatesChangeLogMainView {DataContext = ValveStatesChangeLog}
                    };
                }
                return _valveStatesDialogContent;
            }
        }

        public FindObjectDialogContent FindObjectDialogContent
        {
            get
            {
                if (_findObjectDialogContent == null)
                {
                    FindObjectViewModel.PropertyChanged += OnFindObjectViewModelPropertyChanged;

                    var conf = IsolatedStorageManager.SchemaFindObjectDialogContent;
                    _findObjectDialogContent = new FindObjectDialogContent
                    {
                        Height = conf.Height,
                        Width = conf.Width,
                        Left = conf.Left,
                        Top = conf.Top,
                        DataContext = FindObjectViewModel
                    };
                }
                return _findObjectDialogContent;
            }
        }

        public ModelTreeDialogContent ModelTreeDialogContent
        {
            get
            {
                if (_modelTreeDialogContent == null)
                {
                    PipeLineTree = new TreeViewModelPipeline();
                    ModelTree = new TreeViewModelPointObjects();
                    ModelTree.PropertyChanged += modelTree_PropertyChanged;
                    PipeLineTree.PropertyChanged += modelTree_PropertyChanged;

                    var conf = IsolatedStorageManager.SchemaModelTreeDialogContent;
                    _modelTreeDialogContent = new ModelTreeDialogContent
                    {
                        Height = conf.Height,
                        Width = conf.Width,
                        Left = conf.Left,
                        Top = conf.Top,
                        ModelTree = ModelTree,
                        PipeLineTree = PipeLineTree
                    };

                    if (_systemId.HasValue)
                    {
                        ModelTreeDialogContent.Behavior.TryLock();
                        FillTree();
                    }
                }
                return _modelTreeDialogContent;
            }
        }

        public ValveStatesChangeLogMainViewModel ValveStatesChangeLog { get; private set; }

        public TreeViewModelPipeline PipeLineTree { get; set; }
        public TreeViewModelPointObjects ModelTree { get; set; }

        public bool IsWindowButtonEnabled
        {
            get { return _isWindowButtonEnabled; }
            set { SetProperty(ref _isWindowButtonEnabled, value); }
        }

        protected override void AfterModelLoaded(SchemeViewModel viewModel)
        {
            LoadMeasurings();
        }

        protected override void CloseLoadSchemeFormCallback(SchemeVersionItemDTO schemeDTO)
        {
            if (SelectedSchemeVersion != null)
            {
                //_systemId = SelectedSchemeVersion.SystemId;
                //LoadModel(schemeDTO.Id);
            }
        }

        private void CreateModelTreeDialogWindow()
        {
            var dialogWindow = new ModelTreeDialogWindow {DataContext = ModelTreeDialogContent};

            dialogWindow.SizeChanged += (sender, args) =>
            {
                IsolatedStorageManager.SchemaModelTreeDialogContent = new SchemaDialogConfig
                {
                    Height = ((RadWindow) sender).ActualHeight,
                    Width = ((RadWindow) sender).ActualWidth,
                    Left = ((RadWindow) sender).Left,
                    Top = ((RadWindow) sender).Top,
                    IsOpen = ModelTreeDialogContent.IsOpen
                };
            };

            dialogWindow.LocationChanged += (sender, args) =>
            {
                IsolatedStorageManager.SchemaModelTreeDialogContent = new SchemaDialogConfig
                {
                    Height = ((RadWindow) sender).ActualHeight,
                    Width = ((RadWindow) sender).ActualWidth,
                    Left = ((RadWindow) sender).Left,
                    Top = ((RadWindow) sender).Top,
                    IsOpen = ModelTreeDialogContent.IsOpen
                };
            };

            dialogWindow.Closed += (sender, args) =>
            {
                ModelTreeDialogContent.Left = ((RadWindow) sender).Left;
                ModelTreeDialogContent.Top = ((RadWindow) sender).Top;
                ModelTreeDialogContent.Height = ((RadWindow) sender).ActualHeight;
                ModelTreeDialogContent.Width = ((RadWindow) sender).ActualWidth;

                ModelTreeDialogContent.IsOpen = false;
                ModelTreeDialogContent.DialogResult = true;

                IsolatedStorageManager.SchemaModelTreeDialogContent = new SchemaDialogConfig
                {
                    Height = ModelTreeDialogContent.Height,
                    Width = ModelTreeDialogContent.Width,
                    Left = ModelTreeDialogContent.Left,
                    Top = ModelTreeDialogContent.Top,
                    IsOpen = ModelTreeDialogContent.IsOpen
                };
            };
            dialogWindow.Show();
        }

        private void ShowValveStateDialogWindow(bool firstLoading = false)
        {
            if (firstLoading)
            {
                ValveStatesDialogContent.IsOpen = true;
                ValveStatesDialogContent.DialogResult = false;
                CreateValveStateDialogWindow();
            }
            else
            {
                if (ValveStatesDialogContent.IsOpen)
                {
                    ValveStatesDialogContent.IsOpen = false;
                    ValveStatesDialogContent.DialogResult = true;
                }
                else
                {
                    ValveStatesDialogContent.IsOpen = true;
                    ValveStatesDialogContent.DialogResult = false;
                    CreateValveStateDialogWindow();
                }
            }
        }

        private void CreateValveStateDialogWindow()
        {
            //var dialogWindow = new ValveStateDialogWindow { DataContext = ValveStatesDialogContent };

            //dialogWindow.SizeChanged += (sender, args) =>
            //{
            //    IsolatedStorageManager.SchemaValveStatesDialogConfig = new SchemaDialogConfig
            //    {
            //        Height = ((RadWindow)sender).ActualHeight,
            //        Width = ((RadWindow)sender).ActualWidth,
            //        Left = ((RadWindow)sender).Left,
            //        Top = ((RadWindow)sender).Top,
            //        IsOpen = ValveStatesDialogContent.IsOpen
            //    };
            //};

            //dialogWindow.LocationChanged += (sender, args) =>
            //{
            //    IsolatedStorageManager.SchemaValveStatesDialogConfig = new SchemaDialogConfig
            //    {
            //        Height = ((RadWindow)sender).ActualHeight,
            //        Width = ((RadWindow)sender).ActualWidth,
            //        Left = ((RadWindow)sender).Left,
            //        Top = ((RadWindow)sender).Top,
            //        IsOpen = ValveStatesDialogContent.IsOpen
            //    };
            //};

            //dialogWindow.Closed += (sender, args) =>
            //{
            //    ValveStatesDialogContent.Left = ((RadWindow)sender).Left;
            //    ValveStatesDialogContent.Top = ((RadWindow)sender).Top;
            //    ValveStatesDialogContent.Height = ((RadWindow)sender).ActualHeight;
            //    ValveStatesDialogContent.Width = ((RadWindow)sender).ActualWidth;

            //    ValveStatesDialogContent.IsOpen = false;
            //    ValveStatesDialogContent.DialogResult = true;

            //    IsolatedStorageManager.SchemaValveStatesDialogConfig = new SchemaDialogConfig
            //    {
            //        Height = ValveStatesDialogContent.Height,
            //        Width = ValveStatesDialogContent.Width,
            //        Left = ValveStatesDialogContent.Left,
            //        Top = ValveStatesDialogContent.Top,
            //        IsOpen = ValveStatesDialogContent.IsOpen
            //    };
            //};
            //dialogWindow.Show();
        }

        private void ShowFindObjectDialogWindow(bool firstLoading = false)
        {
            if (firstLoading)
            {
                FindObjectDialogContent.IsOpen = true;
                FindObjectDialogContent.DialogResult = false;
                CreateFindObjectDialogWindow();
            }
            else
            {
                if (FindObjectDialogContent.IsOpen)
                {
                    FindObjectDialogContent.IsOpen = false;
                    FindObjectDialogContent.DialogResult = true;
                }
                else
                {
                    FindObjectDialogContent.IsOpen = true;
                    FindObjectDialogContent.DialogResult = false;
                    CreateFindObjectDialogWindow();
                }
            }
        }

        private void CreateFindObjectDialogWindow()
        {
            var dialogWindow = new FindObjectDialogWindow {DataContext = FindObjectDialogContent};

            dialogWindow.SizeChanged += (sender, args) =>
            {
                IsolatedStorageManager.SchemaFindObjectDialogContent = new SchemaDialogConfig
                {
                    Height = ((RadWindow) sender).ActualHeight,
                    Width = ((RadWindow) sender).ActualWidth,
                    Left = ((RadWindow) sender).Left,
                    Top = ((RadWindow) sender).Top,
                    IsOpen = FindObjectDialogContent.IsOpen
                };
            };

            dialogWindow.LocationChanged += (sender, args) =>
            {
                IsolatedStorageManager.SchemaFindObjectDialogContent = new SchemaDialogConfig
                {
                    Height = ((RadWindow) sender).ActualHeight,
                    Width = ((RadWindow) sender).ActualWidth,
                    Left = ((RadWindow) sender).Left,
                    Top = ((RadWindow) sender).Top,
                    IsOpen = FindObjectDialogContent.IsOpen
                };
            };

            dialogWindow.Closed += (sender, args) =>
            {
                FindObjectDialogContent.Left = ((RadWindow) sender).Left;
                FindObjectDialogContent.Top = ((RadWindow) sender).Top;
                FindObjectDialogContent.Height = ((RadWindow) sender).ActualHeight;
                FindObjectDialogContent.Width = ((RadWindow) sender).ActualWidth;

                FindObjectDialogContent.IsOpen = false;
                FindObjectDialogContent.DialogResult = true;

                IsolatedStorageManager.SchemaFindObjectDialogContent = new SchemaDialogConfig
                {
                    Height = FindObjectDialogContent.Height,
                    Width = FindObjectDialogContent.Width,
                    Left = FindObjectDialogContent.Left,
                    Top = FindObjectDialogContent.Top,
                    IsOpen = FindObjectDialogContent.IsOpen
                };
            };
            dialogWindow.Show();
        }

        private void modelTree_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var send = sender as TreeViewModelBase;
            if (send == null || e.PropertyName != nameof(TreeViewModelBase.SelectedNode) || send.SelectedNode == null ||
                send.SelectedNode is FolderNode)
            {
                return;
            }
            var node = (EntityNode) send.SelectedNode;
            switch (node.Entity.EntityType)
            {
                case EntityType.CompShop:
                    SelectedItem = Model.CompressorShops.FirstOrDefault(p => p.Id == node.Entity.Id);
                    break;
                case EntityType.DistrStation:
                    SelectedItem = Model.DistributingStations.FirstOrDefault(p => p.Id == node.Entity.Id);
                    break;
                case EntityType.MeasLine:
                    SelectedItem = Model.MeasuringLines.FirstOrDefault(p => p.Id == node.Entity.Id);
                    break;
                case EntityType.ReducingStation:
                    SelectedItem = Model.ReducingStations.FirstOrDefault(p => p.Id == node.Entity.Id);
                    break;
                case EntityType.Valve:
                    SelectedItem = Model.Valves.FirstOrDefault(p => p.Id == node.Entity.Id);
                    break;
            }
        }

        private void OnFindObjectViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedItem")
            {
                return;
            }

            SelectedItem = FindObjectViewModel.SelectedItem;
        }

        private void ValveStatesChangeLogOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "SelectedValveState")
            {
                return;
            }

            var valseState = ValveStatesChangeLog.SelectedValveState;
            if (valseState == null)
            {
                return;
            }

            var valve = Model.Valves.FirstOrDefault(v => v.Id == valseState.Id);
            if (valve == null)
            {
                return;
            }

            // if (valve.PipelinePosition == null) return;
            SelectedItem = valve;
        }

        private async void LoadSchemeVersionList()
        {
            try
            {
                Behavior.TryLock("Загрузка версий схем");
                SchemeVersionList = await new SchemeServiceProxy().GetPublishedSchemeVersionListAsync();
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private async void FillTree()
        {
            var treeData = await new ObjectModelServiceProxy().GetFullTreeAsync(new EntityTreeGetParameterSet
            {
                Filter = EntityFilter.Sites |
                         EntityFilter.CompStations |
                         EntityFilter.CompShops |
                         EntityFilter.CompUnits |
                         EntityFilter.DistrStations |
                         EntityFilter.MeasStations |
                         EntityFilter.ReducingStations |
                         EntityFilter.MeasLines |
                         EntityFilter.DistrStationOutlets |
                         EntityFilter.Consumers |
                         EntityFilter.MeasPoints |
                         EntityFilter.CoolingStations |
                         EntityFilter.PowerUnits |
                         EntityFilter.Boilers |
                         EntityFilter.CoolingUnits |
                         EntityFilter.PowerPlants |
                         EntityFilter.BoilerPlants |
                         EntityFilter.Pipelines |
                         EntityFilter.LinearValves,
                SystemId = _systemId
            });
            //Линейная часть
            PipeLineTree.FillTree(treeData, null);

            //Точечные объекты
            var schemeSource = Model.CompressorShops.
                Union<ISearchable>(Model.DistributingStations).
                Union(Model.MeasuringLines).
                Union(Model.Valves).
                Union(Model.ReducingStations).Select(p => p.Id).ToList();

            var entities = new TreeData
            {
                Enterprises = ClientCache.DictionaryRepository.Enterprises,
                Sites = treeData.Sites,
                CompStations = treeData.CompStations,
                CompShops = treeData.CompShops.Where(entity => schemeSource.Any(id => id == entity.Id)).ToList(),
                CompUnits = new List<CompUnitDTO>(),
                DistrStations = treeData.DistrStations.Where(entity => schemeSource.Any(id => id == entity.Id)).ToList(),
                MeasStations = treeData.MeasStations,
                ReducingStations =
                    treeData.ReducingStations.Where(entity => schemeSource.Any(id => id == entity.Id)).ToList(),
                MeasLines = treeData.MeasLines.Where(entity => schemeSource.Any(id => id == entity.Id)).ToList(),
                DistrStationOutlets = new List<DistrStationOutletDTO>(),
                Consumers = new List<ConsumerDTO>(),
                MeasPoints = new List<MeasPointDTO>(),
                CoolingStations = new List<CoolingStationDTO>(),
                PowerUnits = new List<PowerUnitDTO>(),
                Boilers = new List<BoilerDTO>(),
                CoolingUnits = new List<CoolingUnitDTO>(),
                PowerPlants = new List<PowerPlantDTO>(),
                BoilerPlants = new List<BoilerPlantDTO>(),
                Pipelines = treeData.Pipelines,
                LinearValves = treeData.LinearValves.Where(entity => schemeSource.Any(id => id == entity.Id)).ToList()
            };
            ModelTree.FillTree(entities);

            ObservableCollection<NodeBase> source;
            if (UserProfile.Current.Site.IsEnterprise)
            {
                source = ModelTree.Nodes.Single(p => p.Entity.Id == UserProfile.Current.Site.Id).Children;
            }
            else
            {
                var userSite = treeData.Sites.FirstOrDefault(p => p.Id == UserProfile.Current.Site.Id);
                source = ModelTree.Nodes.Single(p => userSite != null && p.Entity.Id == userSite.ParentId).Children;
            }
            ModelTree.Nodes = source;

            ModelTreeDialogContent.Behavior.TryUnlock();
        }

        /// <summary>
        ///     Загрузка измеренных значений
        /// </summary>
        private async void LoadMeasurings()
        {
            try
            {
                Behavior.TryLock("Загрузка серии данных");
                await TaskEx.Delay(TimeSpan.FromSeconds(3));

                await FloModelHelper.LoadMeasuringsAsync(_timestamp, Model, ShowDayly ? DTO.Dictionaries.PeriodTypes.PeriodType.Day : DTO.Dictionaries.PeriodTypes.PeriodType.Twohours);

                RefreshCommands();
                IsWindowButtonEnabled = true;
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private void ShowModelTreeDialogWindow(bool firstLoading = false)
        {
            if (firstLoading)
            {
                ModelTreeDialogContent.IsOpen = true;
                ModelTreeDialogContent.DialogResult = false;
                CreateModelTreeDialogWindow();
            }
            else
            {
                if (ModelTreeDialogContent.IsOpen)
                {
                    ModelTreeDialogContent.IsOpen = false;
                    ModelTreeDialogContent.DialogResult = true;
                }
                else
                {
                    ModelTreeDialogContent.IsOpen = true;
                    ModelTreeDialogContent.DialogResult = false;
                    CreateModelTreeDialogWindow();
                }
            }
        }

        private void AddRepairMarkup(SchemeViewModel viewModel, Repair.Plan.Repair repair)
        {
            if (viewModel == null)
            {
                return;
            }                       
            switch (repair.Dto.EntityType)
            {

                case EntityType.Pipeline:
                    foreach (var work in repair.Dto.Works)
                    {
                        var tooltip =
                            $"{repair.Dto.StartDate:d} - {repair.Dto.EndDate:d}\r\n{repair.Dto.Description}";
                        var pipeline = viewModel.Pipelines.FirstOrDefault(p => p.Id == repair.Dto.EntityId);
                        pipeline?.Markups.Add(new PipelineMarkup
                        {
                            StartKm = work.KilometerStart ?? 0,
                            EndKm = work.KilometerEnd ?? 0,
                            Data = work,
                            Color = repair.Dto.StartDate <= DateTime.Now && repair.Dto.EndDate >= DateTime.Now ? Colors.Red: _startDateToColorConverter.MonthToColor(repair.Dto.StartDate.Month),
                            Tooltip = tooltip
                        });
                    }
                    break;
                case EntityType.CompShop:
                    var cs = viewModel.CompressorShops.FirstOrDefault(c => c.Id == repair.Dto.EntityId);
                    if (cs != null)
                    {
                        cs.Data = new CompressorShopMeasuring((CompressorShopMeasuring)cs.Data, repair.Dto);
                    }
                    break;
                case EntityType.DistrStation:
                    var ds = viewModel.DistributingStations.FirstOrDefault(d => d.Id == repair.Dto.EntityId);
                    if (ds != null)
                    {
                        ds.Data = new DistributingStationMeasuring(((DistributingStationMeasuring)ds.Data), repair.Dto);
                    }
                    break;


            }
        }
                
        private readonly StartDateToColorConverter _startDateToColorConverter = new StartDateToColorConverter();
        private RepairTypeDTO _selectedRepairType;
        private IEnumerable<RepairTypeDTO> _repairTypeList;
        private int? _selectedObjectTypeIndex;
        private int? _selectedTypeIndex;

        public MonthToColorList MonthColorList { get; set; } = new MonthToColorList();

        private static void ClearRepairs(SchemeViewModel schemasource)
        {
            foreach (var pipeline in schemasource.Pipelines)
            {
                pipeline.Markups.Clear();
            }

            foreach (var compressorShop in schemasource.CompressorShops)
            {
                compressorShop.Data = new CompressorShopMeasuring((CompressorShopMeasuring)compressorShop.Data);
            }
            foreach (var distrStation in schemasource.DistributingStations)
            {
                distrStation.Data = new DistributingStationMeasuring((DistributingStationMeasuring)distrStation.Data);
            }

        }
         
        private async void LoadRepairList()
        {
            try
            {
                Behavior.TryLock("Загрузка данных по ремонтам");

                if (Model == null) return;

                if (!ShowRepair)
                {
                    ClearRepairs(Model);
                    SelectedRepairType = null;
                    return;
                }
                int year = Timestamp.Value.Year;
                int year_max = DateTime.Now.Year;
                if (year < year_max)
                    year_max = year + 1;

                var SystemList = ClientCache.DictionaryRepository.GasTransportSystems;
                var param = new GetRepairPlanParameterSet
                {
                    Year = Timestamp.Value.Year,
                    SystemId = SystemList.FirstOrDefault().Id
                };

                if (!UserProfile.Current.Site.IsEnterprise)
                {
                    param.SiteId = UserProfile.Current.Site.Id;
                }

                var rep_list = new List<RepairPlanBaseDTO>();
                while (year <= year_max)
                {
                    param.Year = year;
                    var plan = await new RepairsServiceProxy().GetRepairPlanAsync(param);
                    rep_list.AddRange(plan.RepairList);
                    year++;
                }

                RepairList.Clear();
                RepairList.AddRange(rep_list.Select(Repair.Plan.Repair.Create)
                    .Where(r => r.Dto.StartDate >= Timestamp && r.Dto.StartDate <= Timestamp.Value.AddYears(1)
                             || r.Dto.EndDate   >= Timestamp && r.Dto.EndDate   <= Timestamp.Value.AddYears(1))
                    .OrderByDescending(r => r.Dto.EndDate).ToList());

                RedrawRepairItems();

            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        public void RedrawRepairItems()
        {
            if (Model == null) return;
            ClearRepairs(Model);
            var list = RepairList.ToList();

            if (SelectedTypeIndex != null)
            {
                switch (SelectedTypeIndex)
                {
                    case 0:
                        list = list.Where(r => r.Dto.StartDate <= DateTime.Now && r.Dto.EndDate >= DateTime.Now).ToList();
                        break;
                    case 1:
                        list = list.Where(r => r.Dto.StartDate >= DateTime.Now && r.Dto.EndDate >= DateTime.Now).ToList();
                        break;
                }
            }

            if (SelectedObjectTypeIndex != null)
            {
                switch (SelectedObjectTypeIndex)
                {
                    case 0:
                        list = list.Where(r => r.Dto.EntityType == EntityType.Pipeline).ToList();
                        break;
                    case 1:
                        list = list.Where(r => r.Dto.EntityType == EntityType.CompShop).ToList();
                        break;
                    case 2:
                        list = list.Where(r => r.Dto.EntityType == EntityType.DistrStation).ToList();
                        break;
                }
            }

            RepairTypeList = ClientCache.DictionaryRepository.RepairTypes.Where(p => list.Any(r => r.Dto.RepairTypeId == p.Id));

            if (SelectedRepairType != null)
                list = list.Where(r => r.Dto.RepairTypeId == SelectedRepairType.Id).ToList();

            foreach (var newItem in list)
                AddRepairMarkup(Model, newItem);
        }
                
        public  ObservableCollection<Repair.Plan.Repair> RepairList { get; } = new ObservableCollection<Repair.Plan.Repair>();

        public IEnumerable<RepairTypeDTO> RepairTypeList
        {
            get { return _repairTypeList; }
            set
            {
                if (SetProperty(ref _repairTypeList, value) && SelectedRepairType != null)
                    SelectedRepairType = RepairTypeList.FirstOrDefault(r => r.Id == SelectedRepairType.Id);
            }
        }
        public int? SelectedTypeIndex
        {
            get { return _selectedTypeIndex; }
            set
            {
                if (SetProperty(ref _selectedTypeIndex, value) && ShowRepair)
                {
                    SelectedRepairType = null;
                    RedrawRepairItems();
                }
            }
        }

        public RepairTypeDTO SelectedRepairType
        {
            get { return _selectedRepairType; }
            set
            {
                if(SetProperty(ref _selectedRepairType, value) && ShowRepair)
                    RedrawRepairItems(); 
            }
        }

        public int? SelectedObjectTypeIndex
        {
            get { return _selectedObjectTypeIndex; }
            set
            {
                if (SetProperty(ref _selectedObjectTypeIndex, value) && ShowRepair)
                {
                    SelectedRepairType = null;
                    RedrawRepairItems();
                }
            }
        }

        
        private Image SchemeImage { get; set; }

        private void ImageLoader(FrameworkElement content)
        {
            Image image = new Image();
            using (MemoryStream outStream = new MemoryStream())
            {
                var bitmap = new WriteableBitmap(content, null);
                bitmap.Invalidate();
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(outStream);
                BitmapImage img = new BitmapImage();
                img.SetSource(outStream);
                image.Source = img;
            }
            SchemeImage = image;
        }
        void ExportSchemeToPNG(FrameworkElement content)
        {
            var dialog = new SaveFileDialog { DefaultExt = "png", Filter = "Файл PNG (*.png) | *.png" };
            if (!(dialog.ShowDialog() ?? false))
            {
                return;
            }
            using (var fileStream = dialog.OpenFile())
            {
                var bitmap = new WriteableBitmap(content, null);
                bitmap.Invalidate();
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(fileStream);
            }
        }
        void ExportSchemeToPDF(FrameworkElement content)
        {
            if (IsBusyLoading || !IsModelLoaded)
            {
                MessageBoxProvider.Alert("Схема ещё не загружена!", "Внимание");
                return;
            }
            var schema = FindChild(content, x => x is Flobus.Schema) as Flobus.Schema;
            var dlg = new SaveFileDialog() { Filter = "Pdf files (*.pdf)|*.pdf|All files (*.*)|*.*", FilterIndex = 1, /*DefaultFileName = Header*/ };
            if (dlg.ShowDialog() == true)
            {
                var stream = dlg.OpenFile();
                Behavior.TryLock("Экспорт в PDF...");
                OnPropertyChanged(() => IsBusyLoading);
                var bw = new System.ComponentModel.BackgroundWorker();
                bw.DoWork += (s, e) =>
                {
                    System.Threading.Thread.Sleep(1000);
                    content.Dispatcher.BeginInvoke(() => {
                        try
                        {
                            using (stream)
                            {
                                var document = new RadFixedDocument();
                                var pageSize = new Size(1056, 1488.58);//new Size(796.8, 1123.2)

                                var rect = schema.CalculateEnclosingBounds();
                                var magicK = 1.0;

                                for (var y = Math.Max(rect.Top - 100, 0); y <= rect.Bottom; y += pageSize.Height * magicK)
                                {
                                    for (var x = Math.Max(rect.Left - 100, 0); x <= rect.Right; x += pageSize.Width * magicK)
                                    {

                                        var page = new RadFixedPage();
                                        var vvv = schema.CreateDiagramImage(new Rect(x, y, pageSize.Width * magicK, pageSize.Height * magicK), pageSize);
                                        var source = new Telerik.Windows.Documents.Fixed.Model.Resources.ImageSource(vvv);
                                        page.Size = new Size(source.Width, source.Height);
                                        new FixedContentEditor(page).DrawImage(source);
                                        document.Pages.Add(page);
                                    }
                                }
                                new PdfFormatProvider().Export(document, stream);//ExportSettings.ImageQuality = ImageQuality.High;
                                stream.Flush();
                            }
                        }
                        finally
                        {
                            Behavior.TryUnlock();
                            OnPropertyChanged(() => IsBusyLoading);
                        }
                    });
                };
                bw.RunWorkerAsync();
            }
        }
        private static DependencyObject FindChild(DependencyObject dObject, Func<DependencyObject, bool> predicate)
        {
            var fElement = dObject as FrameworkElement;
            if (fElement != null)
            {
                int cCount = VisualTreeHelper.GetChildrenCount(fElement);
                for (int i = 0; i < cCount; i++)
                {
                    var child = VisualTreeHelper.GetChild(fElement, i);
                    if (predicate(child)) return child;
                    var v = FindChild(child, predicate);
                    if (v != null) return v;
                }
            }
            return null;
        }
        void PrintDocumentPage(object sender, PrintPageEventArgs e)
        {            
            e.PageVisual = SchemeImage;
        }

    }
}