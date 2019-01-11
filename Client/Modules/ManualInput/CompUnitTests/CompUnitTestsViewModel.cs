using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Application;
using GazRouter.Common;
using GazRouter.Common.Cache;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Attachment;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DTO.Attachments;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ManualInput.CompUnitTests;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Telerik.Windows.Controls;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.ManualInput.CompUnitTests
{
    public class CompUnitTestsViewModel : LockableViewModel
    {
        public DelegateCommand AddTestCommand { get; private set; }
        public DelegateCommand RemoveTestCommand { get; private set; }
        public DelegateCommand EditTestCommand { get; private set; }
        public DelegateCommand AddAttachmentCommand { get; private set; }
        public DelegateCommand<AttachmentBaseDTO> DeleteAttachmentCommand { get; private set; }
        public DelegateCommand RefreshCommand { get; private set; }


        //                            §§§s
        //                            @@§³   §§§§§§
        //                           @@@    @@@
        // §§§§ss                  @@@@@aa @@@
        // §§§§§§§§§s     ▄▄▄    @@@@§§§§§§§@@
        //  §§§$$$$$§§§s  ██▀▀▀▀▄▄@@§§§§§§§§@@
        //   §§§$$$$$§§§§§██O°°°OOO██▄§§§§§§§§@@
        //    ³§§§$$$$$§§§§█OoooOOOO█████▄████▄▄_s§§§§§ss
        //      ³³§§§§§§§§§§█OOOOOOO█▀§§§§█O   OO▀██▄$$$§§§§
        //               §§§█▄OOOO°█▀§@@▐█OooOOOOO█▌$$§§§
        //                 §§§█████▀§@@@@█OOOOOOO█$$$§³³
        //                 OO@@@@OOO@@@a█OOOOO█▀
        //                O@@OOOO@@▄OO§s@a█████▀
        //                @@O@@OO@@█OO³³@O§
        //                @@O@OOBB@@@§§@█@
        //              @@OOO@█▄BB@§§§§@§§§s
        //             OOOO@@@██BBBB@³@§§§§
        //            O@@OOOO@@█BBBBB@@§§
        //          @O@O@OOOOOO██▄BBBBBB


        // ##################################################                                                     ¶¶¶¶¶¶¶¶             
        // ##################################################                                                   ¶¶        ¶¶¶
        // ########        #######        ###################                                      ¶¶¶¶¶¶¶    ¶¶            ¶¶
        // #####             ###             ################                                   ¶¶¶       ¶¶  ¶              ¶¶      
        // ####               #                ##############                                 ¶¶            ¶¶¶               ¶¶
        // ###                                  #############                                ¶          ¶¶¶  ¶¶  ¶¶¶¶¶        ¶¶
        // ##                                    ############                                ¶          ¶¶¶  ¶¶  ¶¶¶¶¶        ¶¶
        // ##                                    ############                                ¶         ¶¶¶¶¶  ¶¶  ¶¶¶         ¶¶
        // ###                                  #############                                ¶          ¶¶¶   ¶¶¶            ¶¶¶¶
        // #####                              ###############                                ¶¶              ¶¶ ¶¶¶        ¶¶¶  ¶¶
        // #######                          #################                                 ¶¶            ¶¶    ¶¶¶¶¶¶¶¶¶¶    ¶¶
        // ##########                    ####################                                  ¶¶         ¶¶¶                     ¶
        // ##############            ########################                                  ¶¶¶¶¶¶¶¶¶¶¶¶                       ¶
        // ##################   #############################                                  ¶                                  ¶
        // ##################################################                                 ¶¶                                 ¶¶
        //                                                                                    ¶¶                        ¶¶      ¶¶
        //                                                                                     ¶¶        ¶¶            ¶¶     ¶¶¶
        //                                                                                      ¶¶          ¶¶¶¶¶     ¶¶   ¶¶¶¶¶
        //                                                                                        ¶¶          ¶¶¶¶¶¶¶¶¶¶¶¶¶¶¶
        //                                                                                         ¶¶¶¶¶       ¶¶¶¶¶¶¶¶¶


        

       
        public CompUnitTestsViewModel()
        {
            var editPermission = Authorization2.Inst.IsEditable(LinkType.CompUnitTests);
            LoadSiteList();

            RefreshCommand = new DelegateCommand(Refresh);
            AddTestCommand = new DelegateCommand(AddTest,
                () => SelectedItem is EntityItem && ((EntityItem) SelectedItem).Dto.EntityType == EntityType.CompUnit && editPermission);
            RemoveTestCommand = new DelegateCommand(RemoveTest, () => SelectedItem is TestItem && editPermission);
            EditTestCommand = new DelegateCommand(EditTest, () => SelectedItem is TestItem && editPermission);
            AddAttachmentCommand = new DelegateCommand(AddAttachment, () => SelectedItem is TestItem && editPermission);
            DeleteAttachmentCommand = new DelegateCommand<AttachmentBaseDTO>(DeleteAttachment);
        }


        private void RefreshCommands()
        {
            AddTestCommand.RaiseCanExecuteChanged();
            RemoveTestCommand.RaiseCanExecuteChanged();
            EditTestCommand.RaiseCanExecuteChanged();
            AddAttachmentCommand.RaiseCanExecuteChanged();
        }


        public List<SiteDTO> SiteList { get; set; }

        private SiteDTO _selectedSite;
        public SiteDTO SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                if(SetProperty(ref _selectedSite, value))
                    Refresh();
            }
        }


        private async void LoadSiteList()
        {

            if (UserProfile.Current.Site.IsEnterprise)
                SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                    new GetSiteListParameterSet { EnterpriseId = UserProfile.Current.Site.Id });
            else
                SiteList = await new ObjectModelServiceProxy().GetSiteListAsync(
                    new GetSiteListParameterSet { SiteId = UserProfile.Current.Site.Id });

            OnPropertyChanged(() => SiteList);
            if (SiteList.Any()) SelectedSite = SiteList.First();

        }

        public List<GridItem> Items { get; set; }

        private GridItem _selectedItem;
        public GridItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                    RefreshCommands();
            }
        }

        public async void Refresh()
        {
            if (SelectedSite == null) return;

            try
            {
                Behavior.TryLock();

                // Получить список ГПА
                var stationTree = await new ObjectModelServiceProxy().GetCompStationTreeAsync(SelectedSite.Id);

                var testList = await new ManualInputServiceProxy().GetCompUnitTestListAsync(
                    new GetCompUnitTestListParameterSet
                    {
                        SiteId = SelectedSite.Id
                    });
                
                // Сформировать дерево
                Items = new List<GridItem>();
                
                foreach (var station in stationTree.CompStations)
                {
                    var stationItem = new EntityItem(station);
                    foreach (var shop in stationTree.CompShops.Where(cs => cs.ParentId == station.Id))
                    {
                        var shopItem = new EntityItem(shop);
                        foreach (var unit in stationTree.CompUnits.Where(u => u.ParentId == shop.Id))
                        {
                            var unitItem = new EntityItem(unit);
                            foreach (var test in testList.Where(t => t.CompUnitId == unit.Id))
                            {
                                var testItem = new TestItem(test);
                                unitItem.Children.Add(testItem);
                            }
                            shopItem.Children.Add(unitItem);
                        }
                        stationItem.Children.Add(shopItem);
                    }
                    Items.Add(stationItem);
                }

                OnPropertyChanged(() => Items);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }


        private void AddTest()
        {
            var viewModel = new AddEditCompUnitTestViewModel(id => Refresh(), ((EntityItem)SelectedItem).Dto.Id);
            var view = new AddEditCompUnitTestView { DataContext = viewModel };
            view.ShowDialog();
        }

        private void EditTest()
        {
            var viewModel = new AddEditCompUnitTestViewModel(id => Refresh(), ((TestItem)SelectedItem).Dto);
            var view = new AddEditCompUnitTestView { DataContext = viewModel };
            view.ShowDialog();
        }

        private void RemoveTest()
        {
            RadWindow.Confirm(new DialogParameters
            {
                Header = "Подтверждение",
                Content = new TextBlock
                {
                    Text = "Внимание! Удаляем запись об испытании. Необходимо Ваше подтверждение.",
                    TextWrapping = TextWrapping.Wrap,
                    Width = 250
                },
                OkButtonContent = "Удалить",
                CancelButtonContent = "Отмена",
                Closed = async (obj, args) =>
                {
                    if (args.DialogResult.HasValue && args.DialogResult.Value)
                    {
                        await new ManualInputServiceProxy().DeleteCompUnitTestAsync(((TestItem)(SelectedItem)).Dto.Id);
                        Refresh();
                    }
                }
            });
        }
        

        private void DeleteAttachment(AttachmentBaseDTO dto)
        {
            var d = dto as AttachmentDTO<int, int>;
            if (d != null)
            {
                RadWindow.Confirm(new DialogParameters
                {
                    Header = "Подтверждение",
                    Content = new TextBlock
                    {
                        Text = "Внимание! Удаляем прикрепленный документ. Необходимо Ваше подтверждение.",
                        TextWrapping = TextWrapping.Wrap,
                        Width = 250
                    },
                    OkButtonContent = "Удалить",
                    CancelButtonContent = "Отмена",
                    Closed = async (obj, args) =>
                    {
                        if (args.DialogResult.HasValue && args.DialogResult.Value)
                        {
                            await new ManualInputServiceProxy().RemoveCompUnitTestAttachmentAsync(d.Id);
                            Refresh();
                        }
                    }
                });
                
            }
            
        }


        private void AddAttachment()
        {
            var vm = new AddEditAttachmentViewModel(async obj =>
            {
                var x = (AddEditAttachmentViewModel)obj;
                if (x.DialogResult.HasValue && x.DialogResult.Value)
                {
                    await new ManualInputServiceProxy().AddCompUnitTestAttachmentAsync(
                        new AddAttachmentParameterSet<int>
                        {
                            Description = x.Description,
                            Data = x.FileData,
                            FileName = x.FileName,
                            ExternalId = ((TestItem)SelectedItem).Dto.Id
                        });
                    Refresh();
                }
            });
            var v = new AddEditAttachmentView { DataContext = vm };
            v.ShowDialog();
        }


    }


    public class TestItem : GridItem
    {
        private readonly CompUnitTestDTO _dto;

        public TestItem(CompUnitTestDTO dto)
        {
            _dto = dto;
        }

        public CompUnitTestDTO Dto
        {
            get { return _dto; }
        }
    }

    public class EntityItem : GridItem
    {
        private readonly CommonEntityDTO _entityDto;

        public CommonEntityDTO Dto
        {
            get { return _entityDto; }
        }

        public EntityItem(CommonEntityDTO entityDto)
        {
            _entityDto = entityDto;
        }

        /// <summary>
        /// Тип ГПА
        /// </summary>
        public string CompUnitTypeName
        {
            get
            {
                if (_entityDto.EntityType != EntityType.CompUnit) return "";
                var unit = (CompUnitDTO)_entityDto;
                return ClientCache.DictionaryRepository.CompUnitTypes.Single(t => t.Id == unit.CompUnitTypeId).Name;
            }
        }

        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

    }


    public class GridItem
    {
        public GridItem()
        {
            Children = new List<GridItem>();
        }

        [Display(AutoGenerateField = false)]
        public List<GridItem> Children { get; set; }

    }


}