using System;
using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.Controls;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DTO.ManualInput.ValveSwitches;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;

namespace GazRouter.Modes.ValveStatesChangeLog
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class ValveStatesChangeLogMainViewModel : LockableViewModel
    {
        public ValveStatesChangeLogMainViewModel()
        {
            RefreshCommand = new DelegateCommand(Refresh);
            Refresh();
        }

        public async void Refresh()
        {
            ValveStateList = await new ManualInputServiceProxy().GetValveSwitchListAsync(
                new GetValveSwitchListParameterSet
                {
                    BeginDate = SelectedPeriodDates.BeginDate.Value,
                    EndDate = SelectedPeriodDates.EndDate.Value
                });
        }
        
        public DelegateCommand RefreshCommand { get; set; }

        #region Properties
        
        private PeriodDates _selectedPeriodDates = new PeriodDates
        {
            BeginDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now
        };
        public PeriodDates SelectedPeriodDates
        {
            get { return _selectedPeriodDates; }
            set
            {
                _selectedPeriodDates = value;
                OnPropertyChanged(() => SelectedPeriodDates);
                Refresh();
            }
        }

        private ValveSwitchDTO _selectedValveState;
        public ValveSwitchDTO SelectedValveState
        {
            get { return _selectedValveState; }
            set
            {
                _selectedValveState = value;
                OnPropertyChanged(() => SelectedValveState);
            }
        }

        private List<ValveSwitchDTO> _valveStateList = new List<ValveSwitchDTO>();
        public List<ValveSwitchDTO> ValveStateList
        {
            get { return _valveStateList; }
            set
            {
                _valveStateList = value;
                OnPropertyChanged(() => ValveStateList);
            }
        }


        #endregion
    }
}