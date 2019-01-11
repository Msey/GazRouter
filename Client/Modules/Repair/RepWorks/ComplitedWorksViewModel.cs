using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GazRouter.Application;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Repairs;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.Repairs.Complexes;
using GazRouter.DTO.Repairs.Plan;
using GazRouter.Repair.Dialogs;
using GazRouter.Repair.Gantt;
using GazRouter.Repair.Plan.Gantt;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GanttView;
using Telerik.Windows.Controls.Scheduling;
using Telerik.Windows.Core;
using Telerik.Windows.Data;
using Telerik.Windows.Diagrams.Core;
using AddEditComplexView = GazRouter.Repair.Plan.Dialogs.AddEditComplexView;
using AddEditComplexViewModel = GazRouter.Repair.Plan.Dialogs.AddEditComplexViewModel;
using AddEditRepairView = GazRouter.Repair.ReqWorks.Dialogs.AddEditRepairView;
using AddEditRepairViewModel = GazRouter.Repair.ReqWorks.Dialogs.AddEditRepairViewModel;
using AddEditRepairReportView = GazRouter.Repair.RepWorks.Dialogs.AddEditRepairReportView;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.DTO.Repairs.RepairReport;
using GazRouter.Repair.ReqWorks.Dialogs;
using GazRouter.Controls.Attachment;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Repairs.Workflow;

namespace GazRouter.Repair.RepWorks
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class ComplitedWorksViewModel : CurrentWorksViewModel
    {


        public ComplitedWorksViewModel() : base()
        {
            IsAttachReportVisible = false;
            //Stage = WorkStateDTO.Stages.Complited;
            //MonthColorList.ColorChanged += MonthColorListOnColorChanged;
            base.IsEditPermission = Authorization2.Inst.IsEditable(LinkType.RepairComplited);
                        
            EditRepairCommand = new DelegateCommand(EditRepair, () => SelectedRepair != null );

           // Stage = WorkStateDTO.Stages.Complited;

            base.SelectedYear = IsolatedStorageManager.Get<int?>("RepairPlanLastSelectedYear") ?? DateTime.Today.Year;
            base.SelectedSystem = SystemList.First();

            LoadData();
        }



        public override async void LoadData(int? repairId = null, int? complexId = null)
        {
            Stage = WorkStateDTO.Stages.Complited;
            base.LoadData();
        }
            //Lock();

            //    //var param = new GetRepairPlanParameterSet
            //    //{
            //    //    Year = _selectedYear.Value,
            //    //    SystemId = _selectedSystem.Id
            //    //};
            //    //if (_selectedSite != null)
            //    //    param.SiteId = _selectedSite.Id;

            //    //var plan = await new RepairsServiceProxy().GetRepairPlanAsync(param);

            //    //RepairList = plan.RepairList.Select(Plan.Repair.Create).OrderBy(r => r.SortOrder).ToList();
            //    //OnPropertyChanged(() => RepairList);




            //    //// Выставляем этап планирования
            //    //_planningStage = plan.PlanningStage.Stage;
            //    //OnPropertyChanged(() => PlanningStage);


            //    //RefreshCommands();

            //    //Unlock();

            //    //if (repairId.HasValue)
            //    //{
            //    //    _selectedRepair = RepairList.FirstOrDefault(r => r.Id == repairId.Value);
            //    //    OnPropertyChanged(() => SelectedRepair);
            //    //}


            //    //RefreshCommands();
            //}




        private void RefreshCommands()
        {
            AddRepairCommand.RaiseCanExecuteChanged();
            EditRepairCommand.RaiseCanExecuteChanged();
            RemoveRepairCommand.RaiseCanExecuteChanged();

            AddReportCommand.RaiseCanExecuteChanged();
            EditReportCommand.RaiseCanExecuteChanged();
            DeleteReportCommand.RaiseCanExecuteChanged();

            AddAttachmentCommand.RaiseCanExecuteChanged();
            DeleteAttachmentCommand.RaiseCanExecuteChanged();
        }
    }
}