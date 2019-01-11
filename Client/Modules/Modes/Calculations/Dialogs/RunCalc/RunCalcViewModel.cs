using System;
using System.Collections.Generic;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Calculations;
using GazRouter.DTO.Calculations;
using GazRouter.DTO.Calculations.Calculation;
using Microsoft.Practices.Prism.Commands;
using Utils.Extensions;

namespace GazRouter.Modes.Calculations.Dialogs.RunCalc
{
    public class RunCalcViewModel : DialogViewModel
    {
        private DateTime _startTimeStamp;
        private DateTime _endTimeStamp;
        private CalculationDTO _calculation;

        public RunCalcViewModel(CalculationDTO dto, Action action) : base(action)
        {
            _calculation = dto;
            _startTimeStamp = SeriesHelper.GetCurrentSession();
            _endTimeStamp = _startTimeStamp;
            
            RunCommand = new DelegateCommand(RunCalc, () => EndTimeStamp >= StartTimeStamp);
        }

        
        public DateTime StartTimeStamp
        {
            get { return _startTimeStamp; }
            set
            {
                if (SetProperty(ref _startTimeStamp, value))
                    RaiseCanExecuteCommands();
            }
        }

        public DateTime EndTimeStamp
        {
            get { return _endTimeStamp; }
            set
            {
                if (SetProperty(ref _endTimeStamp, value))
                    RaiseCanExecuteCommands();
            }
        }

        public DelegateCommand RunCommand { get; }


        public List<SerializableTuple<DateTime, string>> CalcStatuses { get; set; }
        
        private async void RunCalc()
        {
            var parameterSet = new RunCalcParameterSet
            {
                PeriodType = _calculation.PerionTypeId,
                StartTimeStamp = StartTimeStamp.ToLocal(),
                EndTimeStamp = EndTimeStamp.ToLocal()
            };

            Lock();
            CalcStatuses = await new CalculationServiceProxy().RunCalcAsync(parameterSet);
            OnPropertyChanged(() => CalcStatuses);
            Unlock();
        }

        private void RaiseCanExecuteCommands()
        {
            ValidateAll();
            RunCommand.RaiseCanExecuteChanged();
        }
    }
}