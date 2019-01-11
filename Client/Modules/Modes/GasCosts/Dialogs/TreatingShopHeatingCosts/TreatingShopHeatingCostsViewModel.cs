using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.Dialogs.ViewModel;

namespace GazRouter.Modes.GasCosts.Dialogs.TreatingShopHeatingCosts
{
    public class TreatingShopHeatingCostsViewModel : CalcViewModelBase<TreatingShopHeatingCostsModel>
	{
        public TreatingShopHeatingCostsViewModel(GasCostDTO gasCost, Action<GasCostDTO> closeCallback, List<DefaultParamValues> defaultParamValues, bool ShowDayly)
            :base(gasCost,  closeCallback, defaultParamValues)
        {

            this.ShowDayly = ShowDayly;
            if (!IsEdit)
            {
                // Если вводится фактическое значение и выбран текущий месяц, 
                // то устанавливать дату в текущий день
                if (TargetId == Target.Fact && IsCurrentMonth)
                    EventDate = EventDate.AddDays(DateTime.Now.Day - EventDate.Day);
                
            }
            PerformCalculate();
        }


        // #region HeaterTypeList

        //   public List<HeaterType> HeaterTypeList
        //   {
        //       get
        //       {
        //           return ClientCache.DictionaryRepository.HeaterTypes
        //.Select(t =>
        //	new HeaterType
        //	{
        //		EffeciencyFactorRated = t.EffeciencyFactorRated,
        //		GasConsumptionRate = t.GasConsumptionRate,
        //		Id = t.Id,
        //		Name = t.Name
        //	})
        //               .Where(h => h.GasConsumptionRate != 0)
        //               .ToList();
        //       }

        //   }

        //public HeaterType SelectedHeaterType
        //{
        //    get { return Model.HeaterType; }
        //    set
        //    {
        //        Model.HeaterType = value;
        //        OnPropertyChanged(() => SelectedHeaterType);
        //        OnPropertyChanged(() => IsHeaterTypeSelected);

        //        HeaterConsumption = value != null ? value.GasConsumptionRate : 0;
        //    }
        //}

        //public bool IsHeaterTypeSelected
        //{
        //    get { return SelectedHeaterType != null; }
        //}

        /// <summary>
        /// 
        /// </summary>
        
        // #endregion

        //#region Values
        public double HeaterConsumption
        {
            get { return Model.HeaterConsumption; }
            set
            {
                Model.HeaterConsumption = value;
                OnPropertyChanged(() => HeaterConsumption);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Время работы подогревателя в расчетном периоде, ч
        /// </summary>
        public int Period
        {
            get { return Model.Period; }
            set
            {
                Model.Period = value;
                OnPropertyChanged(() => Period);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Фактический расход газа через ЦООГ за аналогичный период прошлого года, тыс.м³
        /// </summary>
        public double HeaterConsumptionFact
        {
            get { return Model.HeaterConsumptionFact; }
            set
            {
                Model.HeaterConsumptionFact = value;
                OnPropertyChanged(() => HeaterConsumptionFact);
                PerformCalculate();
            }
        }

        /// <summary>
        /// Фактический среднее значение расхода в подогревателе газа ЦООГ за аналогичный период прошлого года, м³/ч
        /// </summary>
        public double HeaterConsumptionAverage
        {
            get { return Model.HeaterConsumptionAverage; }
            set
            {
                Model.HeaterConsumptionAverage = value;
                OnPropertyChanged(() => HeaterConsumptionAverage);
                PerformCalculate();
            }
        }
        //#endregion
        
        protected override void SetValidationRules()
        {
            
            AddValidationFor(() => HeaterConsumption)
                .When(() => HeaterConsumption <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");


            AddValidationFor(() => Period)
                .When(() => Period < 0 || Period > RangeInHours)
                .Show(string.Format("Недопустимое значение (диапазон допустимых значений от 1 до {0})", RangeInHours));

            AddValidationFor(() => HeaterConsumptionFact)
                .When(() => HeaterConsumptionFact <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

            AddValidationFor(() => HeaterConsumptionAverage)
                .When(() => HeaterConsumptionAverage <= 0)
                .Show("Недопустимое значение. Должно быть больше 0.");

        }
    }
}
