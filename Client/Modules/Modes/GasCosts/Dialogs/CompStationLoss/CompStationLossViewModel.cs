using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;
namespace GazRouter.Modes.GasCosts.Dialogs.CompStationLoss
{
    public class CompStationLossViewModel : CalcViewModelBase<CompStationLossModel>
    {
#region constructor
        public CompStationLossViewModel(GasCostDTO gasCost, 
                                        Action<GasCostDTO> callback, 
                                        List<DefaultParamValues> defaultParamValues, bool ShowDayly)
        : base(gasCost, callback, defaultParamValues)
        {
            this.ShowDayly = ShowDayly;
            if (!IsEdit) IsItValve = true;

            //Model.GasConsumption = IsItValve
            //        ? GasConsumptionDictionary[Entity.EntityType].Item1
            //        : GasConsumptionDictionary[Entity.EntityType].Item2;

            SetValidationRules();
            PerformCalculate();
        }
#endregion

#region property  

        public Dictionary<EntityType, Tuple<double, double, string>> GasConsumptionDictionary
            =>
                new Dictionary<EntityType, Tuple<double, double, string>>
                {
                    {EntityType.CompShop, new Tuple<double, double, string>(0.003, 0.06, "КС")},
                    {EntityType.DistrStation, new Tuple<double, double, string>(0.002, 0.04, "ГРС")},
                    {EntityType.Pipeline, new Tuple<double, double, string>(0.004, 0.09, "ЛЧ")},
                    {EntityType.MeasStation, new Tuple<double, double, string>(0.002, 0.04, "ГИС")},
                    {EntityType.ReducingStation, new Tuple<double, double, string>(0.004, 0.09, "ПРГ")},
                };

        /// <summary>
        /// Количество ЗРА/свечей с утечками, -
        /// </summary>
        public double LeakageCount
        {
            get { return Model.LeakageCount; }
            set
            {
                Model.LeakageCount = value;
                OnPropertyChanged(() => LeakageCount);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Время существования утечки через ЗРА/свечи, ч
        /// </summary>
        public double LeaksDuration
        {
            get { return Model.LeaksDuration; }
            set
            {
                Model.LeaksDuration = value;
                OnPropertyChanged(() => LeaksDuration);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Технологические потери на КС от ЗРА (в положении свечных кранов "закрыто")
        /// </summary>
        public double Loss
        {
            get { return Model.Loss; }
            set
            {
                Model.Loss = value;
                OnPropertyChanged(() => Loss);
                PerformCalculate();
            }
        }
        
        public double TotalCount
        {
            get { return Model.TotalCount; }
            set
            {
                Model.TotalCount = value;
                OnPropertyChanged(() => TotalCount);
                PerformCalculate();
            }
        }

        public bool IsItValve
        {
            get { return Model.IsItValve; }
            set
            {
                Model.IsItValve = value;
                Model.GasConsumption = GasConsumptionDictionary[Entity.EntityType].Item1;
                Model.EntityName = GasConsumptionDictionary[Entity.EntityType].Item3;
                OnPropertyChanged(() => IsItValve);
                PerformCalculate();
            }
        }

        public bool IsItCandle
        {
            get { return Model.IsItCandle; }
            set
            {
                Model.IsItCandle = value;
                Model.GasConsumption = GasConsumptionDictionary[Entity.EntityType].Item2;
                Model.EntityName = GasConsumptionDictionary[Entity.EntityType].Item3;
                OnPropertyChanged(() => IsItCandle);
                PerformCalculate();
            }
        }
        #endregion

        protected override void SetValidationRules()
        {
            AddValidationFor(() => LeakageCount)
                .When(() => LeakageCount <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");
            
            AddValidationFor(() => TotalCount)
                .When(() => TotalCount <= 0 || TotalCount < LeakageCount)
                .Show("Недопустимое значение. Общее число ЗРА/свечей не может быть меньше числа с утечками");
            
            AddValidationFor(() => LeaksDuration)
                .When(() => LeaksDuration <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");
            
            AddValidationFor(() => Loss)
                .When(() => Loss < 0)
                .Show("Недопустимое значение. Должно быть больше 0.");
        }
    } 
}
#region trash
//        private void ValveShiftingViewModelOnPropertyChanged(object sender, 
//                                                             PropertyChangedEventArgs e)
//        {
//            if (e.PropertyName != "Count") return;
//            var vm = (ValveLeaksViewModel) sender;
//            if (Model.ValveShiftings.Contains(vm.Model)) Model.ValveShiftings.Remove(vm.Model);
//            if (vm.Count != 0) Model.ValveShiftings.Add(vm.Model);
//            OnPropertyChanged(() => ValveConsumption);
//            PerformCalculate();
//        }
//        public double ValveConsumption
//        {
//            get
//            {
//                return ValveShiftings.Sum(vt => vt.Qsum);
//            }
//        }


//         .Select(vs => new ValveLeaksViewModel(vs)).ToList();
//            Model.ValveShiftings.ForEach(e => e.PropertyChanged += (sender, args) => { PerformCalculate(); });
//
//         Model.ValveShiftings.ForEach(v =>
//         {
//             var vm = Model.ValveShiftings.SingleOrDefault(c => Equals(c, v.Model));
//             if (vm != null) v.Model.Count = vm.Count;
//             v.PropertyChanged += ValveShiftingViewModelOnPropertyChanged;
//         });


//public double Loss
//{
//    get { return Model.Q; }
//    set
//    {
//        Model.Q = value;
//        OnPropertyChanged(() => Loss);
//        PerformCalculate();
//    }
//}
//private double SetQsum()
//{
//    return 0;
//}

//public ValveShiftingLeaks()
//{
//    //            Qyt = 1;
//}

//        public double Qsum => SetQsum();
//        public double AverageLeaksTime { get; set; }

//        public double Qsum
//        {
//            get { return Model.Qsum; }
////            set
////            {
////                Model.Qsum = value;
////                OnPropertyChanged(() => Qsum);
////            }
//        }

//        public double AverageLeaksTime
//        {
//            get { return Model.AverageLeaksTime; }
//            set
//            {
//                Model.AverageLeaksTime = value;
//                OnPropertyChanged(() => AverageLeaksTime);
//                OnPropertyChanged(() => Qsum);
//            }
//        }

//        public uint CountYt
//        {
//            get { return Model.CountYt; }
//            set
//            {
//                Model.CountYt = value;
//                OnPropertyChanged(() => CountYt);
//                OnPropertyChanged(() => Qsum);
//            }
//        }



//        public List<ValveLeaksViewModel> ValveShiftings { get; set; }
//        public List<RegulatorRuntimeViewModel> RegulatorRuntimes { get; set; }

//        private async void LoadCoolingUnitInfo()
//        {
//            try
//            {
//                Behavior.TryLock();
//
//                var unit = await new ObjectModelServiceProxy().GetCoolingUnitByIdAsync(Entity.Id);
//
//                Model.UnitType = new CoolingUnitType(
//                       ClientCache.DictionaryRepository.CoolingUnitTypes.First(u => u.Id == unit.CoolingUnitTypeId));
//                OnPropertyChanged(() => UnitType);
//                PerformCalculate();
//            }
//            finally
//            {
//                Behavior.TryUnlock();
//            }
//        }
#endregion