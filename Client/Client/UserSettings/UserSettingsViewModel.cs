using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.Authorization;
using GazRouter.DTO.Authorization.User;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using Utils.Units;
using DelegateCommand = Microsoft.Practices.Prism.Commands.DelegateCommand;

namespace GazRouter.Client.UserSettings
{
    public class UserSettingsViewModel : DialogViewModel
    {
        public IEnumerable<Tuple<TemperatureUnit, string>> TemperatureUnitList
        {
            get
            {
                yield return
                    new Tuple<TemperatureUnit, string>(TemperatureUnit.Celsius,
                        Temperature.GetAbbreviation(TemperatureUnit.Celsius));

                yield return
                    new Tuple<TemperatureUnit, string>(TemperatureUnit.Kelvin,
                        Temperature.GetAbbreviation(TemperatureUnit.Kelvin));
            }
        }

        public IEnumerable<Tuple<PressureUnit, string>> PressureUnitList
        {
            get
            {
                yield return
                    new Tuple<PressureUnit, string>(PressureUnit.Kgh, Pressure.GetAbbreviation(PressureUnit.Kgh));

                yield return
                    new Tuple<PressureUnit, string>(PressureUnit.Mpa, Pressure.GetAbbreviation(PressureUnit.Mpa));
            }
        }

        public IEnumerable<Tuple<CombustionHeatUnit, string>> CombHeatUnitList
        {
            get
            {
                yield return
                    new Tuple<CombustionHeatUnit, string>(CombustionHeatUnit.MJ, "МДж/м3");

                yield return
                    new Tuple<CombustionHeatUnit, string>(CombustionHeatUnit.kcal, "ккал/м3");
            }
        }


        private Tuple<TemperatureUnit, string> _selectedTemperatureUnit;
        public Tuple<TemperatureUnit, string> SelectedTemperatureUnit
        {
            get { return _selectedTemperatureUnit; }
            set { SetProperty(ref _selectedTemperatureUnit, value); }
        }


        private Tuple<PressureUnit, string> _selectedPressureUnit;
        public Tuple<PressureUnit, string> SelectedPressureUnit
        {
            get { return _selectedPressureUnit; }
            set { SetProperty(ref _selectedPressureUnit, value); }
        }


        private Tuple<CombustionHeatUnit, string> _selectedCombHeatUnit;
        public Tuple<CombustionHeatUnit, string> SelectedCombHeatUnit
        {
            get { return _selectedCombHeatUnit; }
            set { SetProperty(ref _selectedCombHeatUnit, value); }
        }


        public List<DeltaThresholdWrap> Thresholds { get; set; }
        
        

        
        private bool SaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors; 
        }

        private int _eventLogArchivingDelay;

        public int EventLogArchivingDelay
        {
            get { return _eventLogArchivingDelay; }
            set { SetProperty(ref _eventLogArchivingDelay, value); SaveCommand.RaiseCanExecuteChanged(); }
        }


        public UserSettingsViewModel(IEventAggregator eventAggregator = null)
            : base(null)
        {
            SaveCommand = new DelegateCommand(UpdateCurrent, SaveCommandCanExecute);

            if (UserProfile.Current.UserSettings == null) return;

            SelectedPressureUnit = PressureUnitList.FirstOrDefault(p => p.Item1 == UserProfile.Current.UserSettings.PressureUnit);
            SelectedTemperatureUnit = TemperatureUnitList.FirstOrDefault(p => p.Item1 == UserProfile.Current.UserSettings.TemperatureUnit);
            SelectedCombHeatUnit = CombHeatUnitList.FirstOrDefault(p => p.Item1 == UserProfile.Current.UserSettings.CombHeatUnit);


            var physicalTypes = new Dictionary<PhysicalType, bool>
            {
                { PhysicalType.Pressure, false },
                { PhysicalType.Temperature, false },
                { PhysicalType.Volume, true },
                { PhysicalType.Rpm, false },
                { PhysicalType.Percentage, false }
            };

            Thresholds =
                physicalTypes.Select(
                    t =>
                        new DeltaThresholdWrap(UserProfile.Current.UserSettings.DeltaThresholds.GetThreshold(t.Key) ??
                                               new DeltaThreshold(t.Key, 0, 0, t.Value))).ToList();


            EventLogArchivingDelay = UserProfile.Current.UserSettings.EventLogArchivingDelay;
            
            SetValidationRules();
            ValidateAll();
        }

        public DelegateCommand SaveCommand { get; }

        private async void UpdateCurrent()
        {
            UserProfile.Current.UserSettings.PressureUnit = SelectedPressureUnit.Item1;
            UserProfile.Current.UserSettings.TemperatureUnit = SelectedTemperatureUnit.Item1;
            UserProfile.Current.UserSettings.CombHeatUnit = SelectedCombHeatUnit.Item1;
            UserProfile.Current.UserSettings.EventLogArchivingDelay = EventLogArchivingDelay;

            UserProfile.Current.UserSettings.DeltaThresholds = new DeltaThresholdList(Thresholds.Select(t => t.Delta).ToList());
            
            try
            {
                Behavior.TryLock();

                await new UserManagementServiceProxy().EditUserSettingsAsync(
                new EditUserSettingsParameterSet
                {
                    Id = UserProfile.Current.Id,
                    SettingsUser = UserProfile.Current.UserSettings
                });

                UserProfile.Current.UserSettings = (await new UserManagementServiceProxy().GetProfileInfoAsync()).UserSettings;
                
                DialogResult = true;
            }
            finally
            {
                Behavior.TryUnlock();
            }
        }

        private void SetValidationRules()
        {
            AddValidationFor(() => EventLogArchivingDelay)
                .When(() => EventLogArchivingDelay <= 0 || EventLogArchivingDelay > 30)
                .Show("Допустимый диапазон значений от 1 до 30 дней");
        }


        
        
    }

    public class DeltaThresholdWrap : PropertyChangedBase
    {
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();
        public DeltaThreshold Delta { get; set; }

        public DeltaThresholdWrap(DeltaThreshold delta)
        {
            Delta = delta;
        }

        public string TypeName
            => ClientCache.DictionaryRepository.PhisicalTypes.Single(t => t.PhysicalType == Delta.PhysicalType).Name;

        public string UnitName
            =>
                Delta.IsPercentage
                    ? "%"
                    : ClientCache.DictionaryRepository.PhisicalTypes.Single(t => t.PhysicalType == Delta.PhysicalType)
                        .UnitName;

        public double ShowThreshold
        {
            get { return Delta.ShowThreshold; }
            set
            {
                Delta.ShowThreshold = value < 0 ? 0 : value;
                OnPropertyChanged(() => ShowThreshold);

                if (Delta.ShowThreshold > Delta.WarnThreshold)
                {
                    Delta.WarnThreshold = Delta.ShowThreshold;
                    OnPropertyChanged(() => WarnThreshold);
                }
            }
        }

        public double WarnThreshold
        {
            get { return Delta.WarnThreshold; }
            set
            {
                Delta.WarnThreshold = value < 0 ? 0 : value;
                OnPropertyChanged(() => WarnThreshold);

                if (Delta.ShowThreshold > Delta.WarnThreshold)
                {
                    Delta.ShowThreshold = Delta.WarnThreshold;
                    OnPropertyChanged(() => ShowThreshold);
                }
            }
        }

    }

}