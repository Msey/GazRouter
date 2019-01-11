using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application.Helpers;
using GazRouter.Application.Wrappers.Entity;
using GazRouter.Common.ViewModel;
using GazRouter.Controls.Dialogs.ObjectDetails.Events;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ManualInput.ValveSwitches;
using GazRouter.DTO.ObjectModel.Valves;

namespace GazRouter.Controls.Dialogs.ObjectDetails.Measurings.Valve
{
    public class ValveStatesViewModel : ValidationViewModel
    {
        private Guid _valveId;

        public ValveStatesViewModel(Guid valveId)
        {
            _valveId = valveId;
            _selectedPeriod = new Period(new DateTime(DateTime.Today.Year, 1, 1), DateTime.Now);
            LoadSwitches();
        }


        public List<ValveSwitchDTO> SwitchList { get; set; }

        private Period _selectedPeriod;
        /// <summary>
        /// Переод, за который будут отображаться переключения кранов
        /// </summary>
        public Period SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                if (SetProperty(ref _selectedPeriod, value))
                {
                    LoadSwitches();
                }
            }
        }

        private async void LoadSwitches()
        {
            try
            {
                Behavior.TryLock();

                // Получение истории переключений
                SwitchList = await new ManualInputServiceProxy().GetValveSwitchListAsync(
                    new GetValveSwitchListParameterSet
                    {
                        ValveId = _valveId,
                        BeginDate = _selectedPeriod.Begin,
                        EndDate = _selectedPeriod.End
                    });
                OnPropertyChanged(() => SwitchList);
            }
            finally
            {
                Behavior.TryUnlock();
            }

        }

    }
}