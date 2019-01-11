using System;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.GasTransportSystems;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.Prism.Regions;
using GazRouter.DTO.Dictionaries.StatesModel;

namespace GazRouter.Modes.ProcessMonitoring.Reports.Forms
{

    [RegionMemberLifetime(KeepAlive = false)]
    public class FormViewModelBase : LockableViewModel
    {
        public DateTime Timestamp { get; set; }
        public SiteDTO Site { get; set; }
        public Period Period { get; set; }
        public GasTransportSystemDTO System { get; set; }
        public StateBaseDTO SelectedState { get; set; }

        public bool ShowDetails { get; set; }

        public virtual void Refresh() { }


        public virtual ReportSettings GetReportSettings()
        {
            return new ReportSettings();
        }


        public virtual void RefreshDetails() { }

        public virtual bool HasExcelExport { get; } = false;
        public virtual bool HasUnitCondition { get; } = false;
        public virtual void ExportToExcel() { }
    }

}