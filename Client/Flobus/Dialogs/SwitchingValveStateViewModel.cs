using System;
using System.Collections.Generic;
using GazRouter.Common.ViewModel;
using GazRouter.Controls;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DTO.ManualInput.ValveSwitches;

namespace GazRouter.Flobus.Dialogs
{
    public class SwitchingValveStateViewModel : DialogViewModel
    {
        public SwitchingValveStateViewModel(Guid? valveId, string valveName)
            : base(null)
        {
            Title = $"Журнал переключений крана {valveName}";
            _valveId = valveId;
            Refresh();
        }

        public async void Refresh()
        {
            try
            {
                Behavior.TryLock();
                ValveStateList = await new ManualInputServiceProxy().GetValveSwitchListAsync(
                    new GetValveSwitchListParameterSet
                    {
                        BeginDate = SelectedPeriodDates.BeginDate.Value,
                        EndDate = SelectedPeriodDates.EndDate.Value,
                        ValveId = _valveId
                    });
            }
            finally
            {
                Behavior.TryUnlock();
            }
            
        }
        

        #region Properties

        private readonly Guid? _valveId;
        public string Title { get; set; }

        private PeriodDates _selectedPeriodDates = new PeriodDates
        {
            BeginDate = DateTime.Now.AddYears(-1),
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
