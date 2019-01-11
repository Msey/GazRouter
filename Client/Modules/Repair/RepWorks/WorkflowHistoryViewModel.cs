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
using AddEditRepairView = GazRouter.Repair.Plan.Dialogs.AddEditRepairView;
using AddEditRepairViewModel = GazRouter.Repair.Plan.Dialogs.AddEditRepairViewModel;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;
using GazRouter.DTO.Repairs.Workflow;

namespace GazRouter.Repair.RepWorks
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class WorkflowHistoryViewModel : MainViewModelBase
    {
        private int? _repId;
        public WorkflowHistoryViewModel(int? repId)
        {
            _repId = repId;
            
            LoadData();
        }

        private List<WorkflowHistoryDTO> _historyList;
        public List<WorkflowHistoryDTO> HistoryList
        {
            get { return _historyList; }
            set
            {
                if (SetProperty(ref _historyList, value))
                    OnPropertyChanged(() => HistoryList);
            }
        }
        private async void LoadData()
        {
            if (_repId.HasValue)
            {
                Lock();

                var res = await new RepairsServiceProxy().GetWorkflowHistoryAsync(_repId.Value);
                HistoryList = res;

                Unlock();
            }
        }
    }
}