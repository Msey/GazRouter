using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Application;
using GazRouter.Application.Wrappers.ChemicalTests;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ManualInput.ChemicalTests;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.DTO.ObjectModel.Entities;
using System;

namespace GazRouter.ManualInput.ChemicalTests
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class ChemicalTestsViewModel : LockableViewModel
    {
        public ChemicalTestsViewModel()
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.ChemicalTests);
            RefreshCommand = new DelegateCommand(LoadData);
            AddCommand = new DelegateCommand(() =>
            {
                var vm = new AddEditChemicalTestViewModel(x => LoadData(), SelectedTest.Dto, false);
                var v = new AddEditChemicalTestView {DataContext = vm};
                v.ShowDialog();
            }, () => SelectedTest != null && editPermission);

            EditCommand = new DelegateCommand(() =>
            {
                var vm = new AddEditChemicalTestViewModel(x => LoadData(), SelectedTest.Dto, true);
                var v = new AddEditChemicalTestView { DataContext = vm };
                v.ShowDialog();
            }, () => SelectedTest != null && SelectedTest.Dto.ChemicalTestId.HasValue && editPermission);

            DeleteCommand = new DelegateCommand(DeleteTest, () => SelectedTest != null && SelectedTest.Dto.ChemicalTestId.HasValue && editPermission);

            LoadSiteList();

            
        }


        public DelegateCommand RefreshCommand { get; set; }
        public DelegateCommand AddCommand { get; set; }
        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand DeleteCommand { get; set; }



        public List<SiteDTO> SiteList { get; set; }

        private SiteDTO _selectedSite;
        private ChemicalTest _selectedTest;

        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                _selectedSite = value;
                OnPropertyChanged(() => SelectedSite);
                LoadData();
            }
        }


        public List<ChemicalTest> TestList { get; set; }

        public ChemicalTest SelectedTest
        {
            get { return _selectedTest; }
            set
            {
                _selectedTest = value;
                OnPropertyChanged(() => SelectedTest);
                UpdateCommands();
            }
        }

        private void UpdateCommands()
        {
            AddCommand.RaiseCanExecuteChanged();
            EditCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Загрузка списка ЛПУ
        /// </summary>
        private async void LoadSiteList()
        {
            Lock();
            SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                new GetSiteListParameterSet
                {
                    EnterpriseId = UserProfile.Current.Site.IsEnterprise ? UserProfile.Current.Site.Id : (Guid?)null
                });

            if (!UserProfile.Current.Site.IsEnterprise)
            {
                var site = SiteList.Single(s => s.Id == UserProfile.Current.Site.Id);
                SiteList = SiteList.Where(s => s.Id == site.Id || site.DependantSiteIdList.Contains(s.Id)).ToList();
            }

            OnPropertyChanged(() => SiteList);
            if (SiteList.Count > 0) SelectedSite = SiteList.First();
            Unlock();
        }

        private async void LoadData()
        {
            if (_selectedSite != null)
            {
                try
                {
                    Behavior.TryLock();
                    var list = (await new ManualInputServiceProxy().GetChemicalTestListAsync(
                        new GetChemicalTestListParameterSet
                        {
                            SiteId = _selectedSite.Id
                        }))
                        .Select(t => new ChemicalTest(t)).ToList();
                    var v = await new ObjectModelServiceProxy().GetFullTreeAsync(new EntityTreeGetParameterSet
                    {
                        Filter = /*EntityFilter.Sites |*/
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
                            EntityFilter.CoolingUnits |
                            EntityFilter.PowerPlants |
                            EntityFilter.PowerUnits |
                            EntityFilter.BoilerPlants |
                            EntityFilter.Boilers,
                        SiteId = _selectedSite.Id
                    });
                    TestList = new List<ChemicalTest>();
                    foreach(var cs in v.CompStations.Where(x=>x.ParentId == _selectedSite.Id).OrderBy(x=>x.SortOrder))
                    {
                        AddToTestList(TestList, list, cs);
                        foreach(var compShop in v.CompShops.Where(x=>x.ParentId == cs.Id).OrderBy(x => x.SortOrder))
                        {
                            AddToTestList(TestList, list, compShop);
                            AddToTestList(TestList, list, v.CompUnits, compShop.Id);
                            AddToTestList(TestList, list, v.MeasPoints, compShop.Id);
                        }
                        AddToTestList(TestList, list, v.BoilerPlants, cs.Id, x=> AddToTestList(TestList, list, v.Boilers, x.Id));
                        AddToTestList(TestList, list, v.PowerPlants, cs.Id, x => AddToTestList(TestList, list, v.PowerUnits, x.Id));
                        AddToTestList(TestList, list, v.CoolingStations, cs.Id, x => AddToTestList(TestList, list, v.CoolingUnits, x.Id));
                       
                    }

                    foreach (var ds in v.DistrStations.Where(x => x.ParentId == _selectedSite.Id).OrderBy(x => x.SortOrder))
                    {
                        AddToTestList(TestList, list, ds);
                        AddToTestList(TestList, list, v.Boilers, ds.Id);
                        AddToTestList(TestList, list, v.DistrStationOutlets, ds.Id);
                        AddToTestList(TestList, list, v.MeasPoints, ds.Id);
                    }

                    foreach (var ms in v.MeasStations.Where(x => x.ParentId == _selectedSite.Id).OrderBy(x => x.SortOrder))
                    {
                        AddToTestList(TestList, list, ms);
                        AddToTestList(TestList, list, v.Boilers, ms.Id);
                        AddToTestList(TestList, list, v.MeasLines, ms.Id, x => AddToTestList(TestList, list, v.MeasPoints, x.Id));
                    }

                    foreach (var rs in v.ReducingStations.Where(x => x.ParentId == _selectedSite.Id).OrderBy(x => x.SortOrder))
                    {
                        AddToTestList(TestList, list, rs);
                    }

                    if (list.Count > 0) TestList.AddRange(list);
                    OnPropertyChanged(() => TestList);

                }
                finally 
                {
                    Behavior.TryUnlock();
                }
                
            }
        }

        private void AddToTestList<T>(List<ChemicalTest> testList, List<ChemicalTest> sourceList, T item, Action<T> action = null) where T : DTO.ObjectModel.EntityDTO
        {
            var v = sourceList.FirstOrDefault(x => x.ParentEntityDto.Id == item.Id);
            if (v != null) { testList.Add(v); sourceList.Remove(v); }
            action?.Invoke(item);
        }
        private void AddToTestList<T>(List<ChemicalTest> testList, List<ChemicalTest> sourceList, List<T> childList, Guid parentId, Action<T> action = null) where T : DTO.ObjectModel.EntityDTO
        {
            foreach (var v in childList.Where(x => x.ParentId == parentId).OrderBy(x => x.SortOrder)) AddToTestList(testList, sourceList, v, action);
        }

        

        private void DeleteTest()
        {

            if (SelectedTest == null || !SelectedTest.Dto.ChemicalTestId.HasValue) return;
            
            RadWindow.Confirm(
                new DialogParameters
                {
                    OkButtonContent = "Да",
                    CancelButtonContent = "Нет",
                    Header = "Подтверждение удаления",
                    Content = new TextBlock
                    {
                        Text = "Вы действительно хотите удалить результаты химического анализа?",
                        TextWrapping = TextWrapping.Wrap,
                        Width = 200
                    },
                    Closed = async (o, args) =>
                    {
                        if (args.DialogResult.HasValue && args.DialogResult.Value)
                        {
                            try
                            {
                                Behavior.TryLock();
                                await new ManualInputServiceProxy().DeleteChemicalTestAsync(SelectedTest.Dto.ChemicalTestId.Value);
                            }
                            finally 
                            {
                                Behavior.TryUnlock();
                                LoadData();
                            }
                        }
                    }
                });
            
        }
        
    }
    
}
