using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.GasCosts;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.MeasuringLoader;
using JetBrains.Annotations;
using Microsoft.Practices.Prism.Commands;
using Utils.Extensions;
using System.Linq;
namespace GazRouter.Modes.GasCosts.Dialogs.ViewModel
{
    public abstract class CalcViewModelBase<TModel> : DialogViewModel<Action<GasCostDTO>>
        where TModel : ICostCalcModel, new()
    {
        protected CalcViewModelBase([NotNull] GasCostDTO gasCost, Action<GasCostDTO> closeCallback,
            List<DefaultParamValues> defaultParamValues, bool useMeasuringLoader = false)
            : base(closeCallback)
        {
            DefaultParamValues = defaultParamValues;
            _useMeasuringLoader = useMeasuringLoader;
            if (closeCallback == null) throw new ArgumentNullException(nameof(closeCallback));
            if (gasCost == null) throw new ArgumentNullException(nameof(gasCost));
            if (gasCost.CostType == CostType.None) throw new ArgumentException("CostType");
            CostType = gasCost.CostType;
            TargetId = gasCost.Target;
            SiteId = gasCost.SiteId;
            Entity = gasCost.Entity;
            Id = gasCost.Id != 0 ? gasCost.Id : (int?)null;
            _eventDate = gasCost.Date;
            EventDateRangeStart = _eventDate.MonthStart();
            EventDateRangeEnd = _eventDate.MonthEnd();
            if (IsEdit)
            {
                _result = gasCost.CalculatedVolume;
                _measured = gasCost.MeasuredVolume;
            }
            SaveCommand = new DelegateCommand(Save, CanSave);
            Model = IsEdit ? ParseModel(gasCost.InputString) : new TModel();
            SetValidationRules();
            ValidateAll();
#region MeasuringLoader

                MeasuringLoader = new MeasuringLoaderViewModel(gasCost.Entity.Id, 
                                                               gasCost.Date,
                                                               _useMeasuringLoader,
                                                               MeasuringLoaded, 
                                                               LoadLastInteredModel);
#endregion
#region listing
            ShowListingCommand = new DelegateCommand(() =>
            {
                var window = new CalcResultView();
                var viewModel = new CalcResultViewModel(Model.ToString(), () => { window.Close(); });
                window.DataContext = viewModel;
                window.ShowDialog();
            }, CanCalculate);
#endregion
            SaveCommand.CanExecuteChanged += (sender, args) => { ShowListingCommand.RaiseCanExecuteChanged(); };
        }
#region variables
        private DateTime _eventDate;
        private double? _measured;
        private double? _result;
        private readonly bool _useMeasuringLoader;

        private static int _coef = 1;
        public static int Coef
        {
            get { return _coef; }
            set
            {
                _coef = value;
            }
        }
        public List<DefaultParamValues> DefaultParamValues { get; set; }
        public MeasuringLoaderViewModel MeasuringLoader { get; set; }
        public bool ShowDayly { get; set; }
        public TModel Model { get; private set; }
        /// <summary>
        ///     Дата события
        /// </summary>
        public virtual DateTime EventDate
        {
            get { return _eventDate; }
            set
            {
                SetProperty(ref _eventDate, value);
            }
        }
        public virtual string VolumeType
        {
            get
            {
                return ((Coef > 1) ? "м³" : "тыс. м³");
            }
        }
        public virtual string FormatType
        {
            get
            {
                return ((Coef > 1) ? "n0" : "n3");
            }
        }
        public CommonEntityDTO Entity { get; }
        /// <summary>
        ///     Начало диапазона выбора даты события
        /// </summary>
        public DateTime EventDateRangeStart { get; }
        /// <summary>
        ///     Конец диапазона выбора даты события
        /// </summary>
        public DateTime EventDateRangeEnd { get; }
        /// <summary>
        ///     Количество часов в выбранном месяце
        /// </summary>
        public uint RangeInHours => (uint)(EventDateRangeEnd.AddDays(1) - EventDateRangeStart).TotalHours;
        public bool IsCurrentMonth => false; //EventDate.Month == DateTime.Now.Month && EventDate.Year == DateTime.Now.Year; // 
        /// <summary> Расход газа </summary>
        public double? Result
        {
            get
            {
                return _result * Coef;
            }
            set
            {
                if (SetProperty(ref _result, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public double? Measured
        {
            get
            {               
                return _measured * Coef;
            }
            set
            {
                if (SetProperty(ref _measured, value)) SaveCommand.RaiseCanExecuteChanged();
                PerformCalculate();
            }
        }
        public double? MeasuredInputField
        {
            get
            {
               return _measured; 
            }
            set
            {
                AddValidationFor(() => MeasuredInputField)
                    .When(() => MeasuredInputField <= 0)
                    .Show("Недопустимое значение.Должно быть больше 0.");
                if (SetProperty(ref _measured, value)) SaveCommand.RaiseCanExecuteChanged();
                PerformCalculate();
            }
        }
        public Target TargetId { get; }
        public Guid SiteId { get; }
        public int? Id { get; set; }
        public bool IsEdit
        {
            get { return Id.HasValue; }
        }
        public bool IsFact
        {
            get { return TargetId == Target.Fact; }
        }
        private CostType CostType { get; }
#endregion
#region commands
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand ShowListingCommand { get; set; }
#endregion
#region methods
        protected virtual bool CanSave()
        {
            if (!Result.HasValue && !Measured.HasValue) return false;
            return !Measured.HasValue || !(Measured <= 0);
        }
        protected virtual bool CanCalculate()
        {
            return Result.HasValue;
        }
        protected string GetInputDataString()
        {
            using (TextWriter tw = new StringWriter())
            {
                new XmlSerializer(typeof(TModel)).Serialize(tw, Model);
                return tw.ToString();
            }
        }
        protected virtual void PerformCalculate()
        {
            Result = null;
            ValidateAll();
            if (HasErrors) return;
            Result = Math.Round(Model.Calculate(), 3);
        }
        protected async void Save()
        {
            Behavior.TryLock();
            var inputDataString = GetInputDataString();
            if (!Id.HasValue)
            {
                var addGasCostParameterSet = new AddGasCostParameterSet
                {
                    CalculatedVolume = Result / (Coef),
                    MeasuredVolume = _measured / (Coef),
                    Date = EventDate,
                    EntityId = Entity.Id,
                    CostType = CostType,
                    Target = TargetId,
                    InputData = inputDataString,
                    SiteId = SiteId
                };
                Id = await new GasCostsServiceProxy().AddGasCostAsync(addGasCostParameterSet);
            }
            else
            {
                var parameterSet = new EditGasCostParameterSet
                {
                    CostId = Id.Value,
                    CalculatedVolume = Result / (Coef),
                    MeasuredVolume = _measured / (Coef),
                    Date = EventDate,
                    EntityId = Entity.Id,
                    CostType = CostType,
                    Target = TargetId,
                    InputData = inputDataString,
                    SiteId = SiteId
                };
                await new GasCostsServiceProxy().EditGasCostAsync(parameterSet);
            }
            Behavior.TryUnlock();
            DialogResult = true;
        }
        protected abstract void SetValidationRules();
        protected override void InvokeCallback(Action<GasCostDTO> closeCallback)
        {

            closeCallback(new GasCostDTO
            {
                CalculatedVolume = Result / (Coef),
                Date = EventDate,
                CostType = CostType,
                Target = TargetId,
                Id = Id.Value,
                MeasuredVolume = _measured / (Coef),
                Entity = Entity,
                InputString = GetInputDataString(),
                SiteId = SiteId
            });
        }
        private TModel ParseModel(string inputString)
        {
            if (string.IsNullOrEmpty(inputString)) return new TModel();
            var xmlSerializer = new XmlSerializer(typeof(TModel));
            TextReader tr = new StringReader(inputString);
            return (TModel)xmlSerializer.Deserialize(tr);
        }
        protected virtual void MeasuringLoaded()
        {
        }
        protected async void LoadLastInteredModel()
        {
            var parameter = new GetGasCostListParameterSet
            {
                StartDate = _eventDate.AddDays(-30),
                EndDate   = DateTime.Now,
                SiteId    = SiteId,
                Target    = Target.Fact,
            };
            var gasCosts = await new GasCostsServiceProxy().GetGasCostListAsync(parameter);
            if (gasCosts.Count <= 0) return;
            //
            var gasCosts2 = gasCosts.Where(e=>e.CostType == CostType && !string.IsNullOrEmpty(e.InputString))
                                    .ToList();
            if (gasCosts2.Count == 0) return;
            var gasCost = gasCosts2.OrderBy(e => e.ChangeDate).Last();
            Model = ParseModel(gasCost.InputString);
            UpdateProperty();
        }
        protected virtual void UpdateProperty()
        {
            var property = GetType().GetProperties().ToList();
            property.ForEach(propertyInfo => OnPropertyChanged(propertyInfo.Name));
        }
#endregion
    }
}