using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.Modes.ProcessMonitoring.Reports.Forms;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utils.Extensions;

namespace GazRouter.Modes.ProcessMonitoring.Reports
{
    public class ReportsViewModel : LockableViewModel, INavigationAware
    {
        public ReportsViewModel()
        {
            Setup = new ReportSettings();
            //_selectedDate = DateTime.Now.ToLocal();
            
            RefreshCommand = new DelegateCommand(RefreshReport);
        }

        private async void Init(string reportType)
        {
            // Получить отчет по идентификатору отчета
            var modelViewType = GetType(reportType);
            var newInstance = Activator.CreateInstance(modelViewType);
            FormViewModel = (FormViewModelBase)newInstance;
            ExportExcelCommand = new DelegateCommand(FormViewModel.ExportToExcel);
            HasExcelExport = FormViewModel.HasExcelExport;
            HasUnitCondition = FormViewModel.HasUnitCondition;
            //
            Setup = FormViewModel.GetReportSettings();
            OnPropertyChanged(() => FormViewModel);
            OnPropertyChanged(() => IsReportSelected);
            OnPropertyChanged(() => HasExcelExport);
            OnPropertyChanged(() => HasUnitCondition);
            OnPropertyChanged(() => CompUnitStateList);
            OnPropertyChanged(() => ExportExcelCommand);
            OnPropertyChanged(() => Setup);

            if (Setup.SerieSelector)
            {
                // Установить текущую дату
                _selectedDate = _selectedDate == default(DateTime) ?  SeriesHelper.GetLastCompletedSession() : _selectedDate;
                OnPropertyChanged(() => SelectedDate);
            }
            if (Setup.PeriodSelector)
            {
                // Установить период по умолчанию
                _selectedPeriod = new Period
                {
                    Begin = DateTime.Today.AddDays(1 - DateTime.Today.Day),
                    End = DateTime.Today.AddDays(1)
                };
                OnPropertyChanged(() => SelectedPeriod);
            }
            if (Setup.SiteSelector)
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
                else
                {
                    SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                    new GetSiteListParameterSet
                    {
                        SiteId = UserProfile.Current.Site.Id
                    });
                }
                OnPropertyChanged(() => SiteList);

                if (Setup.EmptySiteAllowed)
                {
                    _selectedSite = SiteList.First();
                    OnPropertyChanged(() => SelectedSite);
                }
                else
                {
                    _selectedSite = SiteList.First();
                    OnPropertyChanged(() => SelectedSite);
                }
            }
            if (Setup.SystemSelector)
            {
                _selectedSystem = SystemList.First();
                OnPropertyChanged(() => SelectedSystem);
            }
            if (HasUnitCondition) OnPropertyChanged(() => SelectedSite);

            RefreshReport();
        }
        public ReportSettings Setup { get; set; }
        private DateTime _selectedDate;
        /// <summary>
        /// Выбранная дата. 
        /// Должна быть задана всегда, NULL недопустим. 
        /// Поэтому первым делом инициализируется в конструкторе.
        /// </summary>
        public DateTime SelectedDate
        {
            get { return _selectedDate; }
            set
            {
                if (SetProperty(ref _selectedDate, value))
                {
                    RefreshReport();
                }
            }
        }
        private Period _selectedPeriod;
        /// <summary>
        /// Выбранный период. 
        /// Должна быть задана всегда, NULL недопустим. 
        /// Поэтому первым делом инициализируется в конструкторе.
        /// </summary>
        public Period SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                if (SetProperty(ref _selectedPeriod, value))
                {
                    RefreshReport();
                }
            }
        }
        /// <summary>
        /// Список ЛПУ
        /// </summary>
        public List<SiteDTO> SiteList { get; set; }
        private SiteDTO _selectedSite;
        /// <summary>
        /// Выбранное ЛПУ
        /// </summary>
        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                if (SetProperty(ref _selectedSite, value))
                {
                    RefreshReport();
                }
            }
        }
        /// <summary>
        /// Список газотранспортных систем
        /// </summary>
        public List<GasTransportSystemDTO> SystemList
        {
            get { return ClientCache.DictionaryRepository.GasTransportSystems; }
        }
        private GasTransportSystemDTO _selectedSystem;
        /// <summary>
        /// Выбранная ГТС
        /// </summary>
        public GasTransportSystemDTO SelectedSystem
        {
            get { return _selectedSystem; }
            set
            {
                if (SetProperty(ref _selectedSystem, value))
                {
                    RefreshReport();
                }
            }
        }
        private bool _showDetails;
        public bool ShowDetails
        {
            get { return _showDetails;}
            set
            {
                if(SetProperty(ref _showDetails, value))
                    if (IsReportSelected)
                    {
                        FormViewModel.ShowDetails = value;
                        FormViewModel.RefreshDetails();
                    }
            }
        }
        public DelegateCommand SetDeltaThresholdsCommand { get; private set; }
        public DelegateCommand RefreshCommand { get; private set; }
        public DelegateCommand ExportExcelCommand { get; set; }

        public FormViewModelBase FormViewModel { get; private set; }
        /// <summary>
        /// Выбран ли отчет?
        /// </summary>
        public bool IsReportSelected => FormViewModel != null;
        public bool HasExcelExport { get; set; }
        /// <summary>
        /// Список состояний ГПА
        /// </summary>
        public bool HasUnitCondition { get; set; }

        private StateBaseDTO _selectedState;
        public StateBaseDTO SelectedState
        {
            get { return _selectedState; }
            set
            {
                if (SetProperty(ref _selectedState, value))
                {
                    RefreshReport();
                }
            }
        }
        public List<StateBaseDTO> CompUnitStateList
        {
            get { return ClientCache.DictionaryRepository.CompUnitStates.Where(s => s.Id != (int)CompUnitState.Undefined).ToList();}
        }


        private void RefreshReport()
        {
            if (IsReportSelected)
            {
                FormViewModel.Site = SelectedSite;
                FormViewModel.Timestamp = SelectedDate;
                FormViewModel.Period = SelectedPeriod;
                FormViewModel.System = SelectedSystem;
                FormViewModel.ShowDetails = ShowDetails;
                FormViewModel.SelectedState = SelectedState;
                FormViewModel.Refresh();
            }
        }

        
        public void OnNavigatedTo(NavigationContext navigationContext)
        {            
            Init(navigationContext.Parameters["reportId"]);
        }
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
        private static Type GetType(string typeName)
        {
            var type = Type.GetType(typeName);
            if (type != null) return type;
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if (type != null) return type;
            }
            return null;
        }
    }
    public enum ReportType
    {
        CompShops = 1,
        CompUnits = 2,
        DistrStations = 3,
        ChemicalTests = 4,
        Balance = 5,
        Valves = 6,
        CompUnitFailures = 7,
        GasInPipes = 8,
        MeasStations = 9

    }
}
#region trash
//        private static FormViewModelBase GetReport(ReportType reportType)
//        {
//            switch (reportType)
//            {
//                case ReportType.CompShops:
//                    return new CompShopsViewModel();
//
//                case ReportType.CompUnits:
//                    return new CompUnitsViewModel();
//
//                case ReportType.DistrStations:
//                    return new DistrStationsViewModel();
//                    
//                case ReportType.ChemicalTests:
//                    return new ChemicalTestsViewModel();
//                    
//                case ReportType.Valves:
//                    return new ValvesViewModel();
//                    
//                case ReportType.CompUnitFailures:
//                    return new CompUnitFailuresViewModel();
//                    
//                case ReportType.GasInPipes:
//                    return new GasInPipesViewModel();
//
//                case ReportType.Balance:
//                    return null;
//
//                case ReportType.MeasStations:
//                    return new MeasStationsViewModel();
//
//                default:
//                    return null;
//            }
//        }
#endregion