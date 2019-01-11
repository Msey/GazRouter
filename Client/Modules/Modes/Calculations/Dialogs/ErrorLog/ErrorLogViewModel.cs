using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Calculations;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Log;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using Microsoft.Practices.Prism.Commands;
using Utils.Extensions;

namespace GazRouter.Modes.Calculations.Dialogs.ErrorLog
{
    public class ErrorLogViewModel : DialogViewModel
    {
        private int? _calculationId;
        private Period _selectedPeriod;

        public ErrorLogViewModel() : base(null)
        {
            _selectedPeriod = new Period(DateTime.Now.AddDays(-1), DateTime.Now);
            RefreshCommand = new DelegateCommand(Refresh);

            Refresh();
        }

        public ErrorLogViewModel(int calculationId) : base(null)
        {
            _selectedPeriod = new Period(DateTime.Now.AddDays(-1), DateTime.Now);
            RefreshCommand = new DelegateCommand(Refresh);
            _calculationId = calculationId;

            Refresh();
        }

        public Period SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                if (SetProperty(ref _selectedPeriod, value))
                    Refresh();
            }
        }

        public List<LogCalculationDTO> Log { get; set; }

        
        public DelegateCommand RefreshCommand { get; set; }

        public async void Refresh()
        {
            Lock();
            Log = await new CalculationServiceProxy().GetLogsAsync(
                new GetLogListParameterSet
                {
                    CalculationId = _calculationId,
                    StartDate = SelectedPeriod.Begin.ToLocal(),
                    EndDate = SelectedPeriod.End.ToLocal()
                });
            OnPropertyChanged(() => Log);
            Unlock();
        }
    }
}