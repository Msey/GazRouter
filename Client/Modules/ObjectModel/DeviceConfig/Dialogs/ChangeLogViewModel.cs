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
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.ObjectModel.ChangeLogs;

namespace GazRouter.ObjectModel.DeviceConfig.Dialogs
{
    public class ChangeLogViewModel: DialogViewModel
    {
        private Period _selectedPeriod;
        private DeviceType _selectedDeviceType;

        public ChangeLogViewModel() : base(null)
        {
            _selectedPeriod = new Period(DateTime.Now.MonthStart(), DateTime.Now);
            RefreshCommand = new DelegateCommand(Refresh);

            Refresh();
        }

        public ChangeLogViewModel(DeviceType selectedDeviceType, IEnumerable<DeviceType> devicetypes) : base(null)
        {
            _selectedPeriod = new Period(DateTime.Now.MonthStart(), DateTime.Now);
            RefreshCommand = new DelegateCommand(Refresh);
            _selectedDeviceType = selectedDeviceType;
            DeviceTypes = devicetypes;
            Refresh();
        }
        
        public DeviceType SelectedDeviceType
        {
            get { return _selectedDeviceType; }
            set
            {
                if (value != _selectedDeviceType)
                {
                    _selectedDeviceType = value;
                    Refresh();
                }
            }
        }
        
        public bool IsExecChecked { get; set; }

        public Period SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                if (SetProperty(ref _selectedPeriod, value))
                    Refresh();
            }
        }

        public IEnumerable<DeviceType> DeviceTypes { get; set; }
        public List<ChangeDTO> Log { get; set; }

        public DelegateCommand RefreshCommand { get; set; }

        public async void Refresh()
        {
            Lock();
            Log = await new ObjectModelServiceProxy().GetChangeLogAsync(
                new GetChangeLogParameterSet
                {
                    StartDate = SelectedPeriod.Begin.ToLocal(),
                    EndDate = SelectedPeriod.End.ToLocal(),
                    TableName = _selectedDeviceType.Name
                });
            OnPropertyChanged(() => Log);
            OnPropertyChanged(() => SelectedDeviceType);
            Unlock();
        }
    }
}