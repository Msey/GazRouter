using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Controls.Converters;
using GazRouter.Controls.Dialogs.ObjectDetails;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ManualInput.ValveSwitches;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Utils.Extensions;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.DataProviders.ObjectModel;

namespace GazRouter.Modes.ProcessMonitoring.Reports.Forms.Valves
{

    [RegionMemberLifetime(KeepAlive = false)]
    public class ValvesViewModel : FormViewModelBase
    {
        
        public override ReportSettings GetReportSettings()
        {
            return new ReportSettings
            {
                PeriodSelector = true
            };
        }

        public override async void Refresh()
        {
            try
            {
                Behavior.TryLock();


                var switches = await new ManualInputServiceProxy().GetValveSwitchListAsync(
                    new GetValveSwitchListParameterSet
                    {
                        BeginDate = Period.Begin.ToLocal(),
                        EndDate = Period.End.ToLocal()
                    });


                var valves = await new ObjectModelServiceProxy().GetValveListAsync(new GetValveListParameterSet());

                Items = switches.OrderByDescending((s) => s.SwitchingDate).ThenBy((s) => s.PipelineName).ThenBy((s) => s.ValveName).ThenBy((s) => s.Kilometr).
                    Select(s => new SwitchItem(s,valves.FirstOrDefault(v => v.Id == s.Id))).ToList();

                OnPropertyChanged(() => Items);
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
        

        public List<SwitchItem> Items { get; set; }

        private SwitchItem _selectedItem;
        public SwitchItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                SetProperty(ref _selectedItem, value);
            }
        }
    }

    public class SwitchItem
    {
        public SwitchItem(ValveSwitchDTO dto, ValveDTO valve)
        {
            Dto = dto;
            Valve = valve;
        }

        public int ValveTypeId
        {
            get
            {
                switch (Dto.SwitchType)
                {
                    case ValveSwitchType.Valve:
                        return Dto.ValveTypeId;

                    case ValveSwitchType.Bypass1:
                        return Dto.Bypass1TypeId;

                    case ValveSwitchType.Bypass2:
                        return Dto.Bypass2TypeId;

                    case ValveSwitchType.Bypass3:
                        return Dto.Bypass3TypeId;

                    default:
                        return 0;
                }
            }
        }

        public ValveSwitchDTO Dto { get; }
        public ValveDTO Valve { get; }
    }
}