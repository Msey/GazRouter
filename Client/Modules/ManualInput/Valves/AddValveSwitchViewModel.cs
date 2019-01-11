using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.Application;
using GazRouter.Application.Helpers;
using GazRouter.Common;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.StatesModel;
using GazRouter.DTO.ManualInput.ValveSwitches;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.Modes.GasCosts.Imports;
using Utils.Extensions;


namespace GazRouter.ManualInput.Valves
{
    public class AddValveSwithViewModel : AddEditViewModelBase<ValveSwitchDTO, int>
    {
        private Guid _siteId;
        private DateTime _sessionMinTime;
        private DateTime _sessionMaxTime;

        public AddValveSwithViewModel(Action<int> actionBeforeClosing, DateTime date, Guid siteId)
            : base(actionBeforeClosing)
        {
            _siteId = siteId;

            _sessionMinTime = date.AddHours(-2).AddSeconds(1);
            _sessionMaxTime = date;

            MinDate = _sessionMinTime;
            MaxDate = _sessionMaxTime;

            MinTime = TimeSpan.FromHours(_sessionMinTime.Hour);
            MaxTime = TimeSpan.FromHours(_sessionMaxTime.Hour);

            if (MinDate > DateTime.Now) MinDate = DateTime.Now;
            if (MaxDate > DateTime.Now) MaxDate = DateTime.Now;

            SwitchDate = DateTime.Now;
            if (SwitchDate < MinDate) SwitchDate = MinDate;
            if (SwitchDate > MaxDate) SwitchDate = MaxDate;

            
            SetValidationRules();

            SaveCommand.RaiseCanExecuteChanged();

            LoadPrevValues();
        }

        public DateTime MinDate { get; set; }
        public DateTime MaxDate { get; set; }

        public TimeSpan MinTime { get; set; }

        public TimeSpan MaxTime { get; set; }

       
        /// <summary>
        /// Восстанавливает предыдущее состояние полей формы - выбранный газопровод и крановый узел
        /// </summary>
        private async void LoadPrevValues()
        {
            var pipelineId = IsolatedStorageManager.Get<Guid?>("ValveSwitchLastSelectedPipe");
            if (!pipelineId.HasValue) return;

            var pipeline = await new ObjectModelServiceProxy().GetPipelineByIdAsync(pipelineId.Value);
            if (pipeline == null) return;

            _selectedPipeline = pipeline;
            OnPropertyChanged(() => SelectedPipeline);
            OnPropertyChanged(() => IsPipelineSelected);

            var valveId = IsolatedStorageManager.Get<Guid?>("ValveSwitchLastSelectedValve");

            LoadValveList(valveId);

            

        }


        private async void LoadValveSwitches()
        {
            if (SelectedValve == null) return;
            var switches = await new ManualInputServiceProxy().GetValveSwitchListAsync(
                new GetValveSwitchListParameterSet
                {
                    ValveId = SelectedValve.Id,
                    BeginDate = _sessionMinTime,
                    EndDate = _sessionMaxTime
                });

            if (switches.Count > 0)
            {
                var swDate = switches.Max(s => s.SwitchingDate);
                SwitchDate = swDate.AddMinutes(1);
            }
        }


        private DateTime _switchDate;
        /// <summary>
        /// Дата переключения
        /// </summary>
        public DateTime SwitchDate
        {
            get { return _switchDate; }
            set
            {
                if(SetProperty(ref _switchDate,value))
                    SaveCommand.RaiseCanExecuteChanged();
            }
        }
        
        private CommonEntityDTO _selectedPipeline;
        /// <summary>
        /// Выбранный газопровод
        /// </summary>
        public CommonEntityDTO SelectedPipeline
        {
            get { return _selectedPipeline; }
            set
            {
                if (_selectedPipeline != value)
                {
                    _selectedPipeline = value;
                    OnPropertyChanged(() => SelectedPipeline);
                    OnPropertyChanged(() => IsPipelineSelected);

                    LoadValveList();

                    SaveCommand.RaiseCanExecuteChanged();

                    IsolatedStorageManager.Set("ValveSwitchLastSelectedPipe", value.Id);
                }

            }
        }

        public bool IsPipelineSelected => _selectedPipeline != null;

        public List<EntityType> AllowedTypes => new List<EntityType>
        {
            EntityType.Pipeline
        };

        /// <summary>
        /// Список кранов на газопроводе
        /// </summary>
        public List<ValveDTO> ValveList { get; set; }

        
        public ValveDTO _selectedValve;
        /// <summary>
        /// Выбранный кран
        /// </summary>
        public ValveDTO SelectedValve
        {
            get { return _selectedValve; }
            set
            {
                _selectedValve = value;
                OnPropertyChanged(() => SelectedValve);
                OnPropertyChanged(() => IsValveSelected);

                OnPropertyChanged(() => ValveSwitchTypeList);

                SelectedValveSwitchType = ValveSwitchTypeList.Count == 1 ? (ValveSwitchType?)ValveSwitchType.Valve : null;

                ValveState = null;
                
                SaveCommand.RaiseCanExecuteChanged();

                if (value != null)
                    IsolatedStorageManager.Set("ValveSwitchLastSelectedValve", value.Id);

                LoadValveSwitches();
            }
        }

        /// <summary>
        /// Список возможных состояний крана
        /// </summary>
        public List<StateBaseDTO> ValveStateList
        {
            get { return ClientCache.DictionaryRepository.ValveStates; }
        }


        /// <summary>
        /// Выбран ли кран
        /// </summary>
        public bool IsValveSelected
        {
            get { return SelectedValve != null; }
        }


        public List<ValveSwitchType> ValveSwitchTypeList
        {
            get
            {
                var list = new List<ValveSwitchType> { ValveSwitchType.Valve };
                if (!IsValveSelected) return list;

                if (SelectedValve.Bypass1TypeId.HasValue) list.Add(ValveSwitchType.Bypass1);
                else return list;

                if (SelectedValve.Bypass2TypeId.HasValue) list.Add(ValveSwitchType.Bypass2);
                else return list;

                if (SelectedValve.Bypass3TypeId.HasValue) list.Add(ValveSwitchType.Bypass3);
                else return list;

                return list;
            }
        }

        private ValveSwitchType? _selectedValveSwitchType;
        public ValveSwitchType? SelectedValveSwitchType
        {
            get { return _selectedValveSwitchType; }
            set
            {
                _selectedValveSwitchType = value;
                OnPropertyChanged(() => SelectedValveSwitchType);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }

        

        private StateBaseDTO _valveState;
        /// <summary>
        /// Состояние крана
        /// </summary>
        public StateBaseDTO ValveState
        {
            get { return _valveState; }
            set
            {
                _valveState = value;
                OnPropertyChanged(() => ValveState);
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        


        private async void LoadValveList(Guid? valveId = null)
        {
            if (SelectedPipeline != null)
            {
                Lock();
                ValveList =
                    await
                        new ObjectModelServiceProxy().GetValveListAsync(
                            new GetValveListParameterSet
                            {
                                PipelineId = SelectedPipeline.Id,
                                SiteId = _siteId
                            });
                OnPropertyChanged(() => ValveList);

                SelectedValve = valveId.HasValue ? ValveList.SingleOrDefault(v => v.Id == valveId.Value) : null;

                Unlock();
            }
        }


        protected override bool OnSaveCommandCanExecute()
        {
            ValidateAll();
            return !HasErrors;
        }
        

        private void SetValidationRules()
        {
            AddValidationFor(() => SwitchDate)
                .When(() => SwitchDate < MinDate || SwitchDate > MaxDate)
                .Show(
                    $"Недопустимое время переключения. Должно быть в интервале между {MinDate:dd.MM.yyyy HH:mm} и {MaxDate:dd.MM.yyyy HH:mm}");

            AddValidationFor(() => SelectedPipeline)
                .When(() => !IsPipelineSelected)
                .Show("Выберите газопровод, на котором установлен крановый узел");

            AddValidationFor(() => SelectedValve)
                .When(() => IsPipelineSelected && !IsValveSelected)
                .Show("Выберите крановый узел");

            AddValidationFor(() => SelectedValveSwitchType)
                .When(() => SelectedValveSwitchType == null)
                .Show("Выберите тип переключения");

            AddValidationFor(() => ValveState)
                .When(() => IsValveSelected && ValveState == null)
                .Show("Выберите новое состояние крана");

        }
        

        protected override string CaptionEntityTypeName => " переключения";
        


        
        protected override async void CreateNew()
        {
            await new ManualInputServiceProxy().AddValveSwitchAsync(
                new AddValveSwitchParameterSet
                {
                    SwitchingDate = SwitchDate.ToLocal(),
                    ValveId = SelectedValve.Id,
                    ValveSwitchType = SelectedValveSwitchType.Value,
                    State = (ValveState)ValveState.Id,
                });

            //GasCostImportHelper.ImportValveCosts(SwitchDate, SelectedValve, SelectedValveSwitchType.Value, 0);
            
            DialogResult = true;
            
        }
    }
    
}
