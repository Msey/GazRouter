using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.ViewModel;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.GasCosts;
using GazRouter.Modes.GasCosts.DefaultDataDialog;
using GazRouter.Modes.GasCosts.Dialogs.BoilerConsumptions;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Prism.Commands;
using Utils.Units;

namespace GazRouter.Modes.GasCosts2
{
    /// <summary>
    /// 
    /// передать список котлов
    /// 
    /// </summary>
    public class BoilerConsumptionsViewModel2 : DialogViewModel
    {
#region constructor
        public BoilerConsumptionsViewModel2(Action closeCallback, 
                                            List<GasCostDTO> previewDayCosts, 
                                            DateTime selectedDate, 
                                            Guid selectedSiteId, 
                                            List<DefaultParamValues> defaultParamValues, 
                                            int coef,
                                            ILookup<CostType, BoilerExtension> boilers) 
            : base(closeCallback)
        {
            this.defaultParamValues = defaultParamValues;
            this.Coef               = coef;
            this.selectedSiteId     = selectedSiteId;
            this._previewDayCosts   = previewDayCosts;
            this.selectedDate       = selectedDate;
            SaveCommand             = new DelegateCommand(Save);
            // 
            LoadItems(boilers);
        }
#endregion
#region variables
        public DelegateCommand SaveCommand { get; set; }
        private readonly List<DefaultParamValues> defaultParamValues;
        private readonly DateTime selectedDate;
        private readonly Guid selectedSiteId;
        public readonly int Coef;
        public string InputDate => selectedDate.ToLongDateString();
        private List<BoilerConsumptionsInputModel> _items;
        public List<BoilerConsumptionsInputModel> Items
        {
            get { return _items; }
            private set { SetProperty(ref _items, value); }
        }
        public string VolumeType => ((Coef > 1) ? "м3" : "тыс.м3");
        public string FormatType => ((Coef > 1) ? "F0" : "n3");
        private List<GasCostDTO> _previewDayCosts;
#endregion
        #region methods
        private async void Save()
        {
            Behavior.TryLock();
            if (Items.Any(x => x.HasErrors))
            {
                MessageBoxProvider.Alert("Было допущено несколько ошибок валидации.\n" +
                                         "Сохранение отменено.", "Ошибка", "Ок");
                Behavior.TryUnlock();
                return;
            }
            try
            {
                for (var i = 0; i < Items.Count; i++)
                    await Items[i].Save();
            }
            finally
            {
                Behavior.TryUnlock();
                DialogResult = true;
            }
        }
        public void LoadItems(ILookup<CostType, BoilerExtension> costBoilers)
        {
            var list = new List<BoilerConsumptionsInputModel>();
            costBoilers.ForEach(costBoiler =>
            {
                costBoiler.ToList().ForEach(boiler =>
                {
                    var gasCost = new GasCostDTO
                    {
                        Date           = selectedDate,
                        CostType       = costBoiler.Key, 
                        Entity         = boiler.BoilerDTO,
                        SiteId         = selectedSiteId,
                        ChangeDate     = selectedDate,
                        ChangeUserName = UserProfile.Current.UserName,
                        Target         = Target.Fact
                    };
                    list.Add(new BoilerConsumptionsInputModel(gasCost, defaultParamValues, ClientCache, Coef)
                    {
                        LocationName   = boiler.BoilerDTO.Name /*station.Name*/,
                        ItemName       = /*((boilerPlantDTO != null) ? boilerPlantDTO.Name + " / " : string.Empty) +*/ boiler.BoilerDTO.Name,
                        CombHeat       = /*(boilerCostPreviousDayPrototype.Entity != null) ? boilerCostPreviousDayPrototype.CombHeat      : */ CombustionHeat.FromKCal(7000),
                        LightingCount  = /*(boilerCostPreviousDayPrototype.Entity != null) ? boilerCostPreviousDayPrototype.LightingCount : */ 5,
                        Period         =                                                                                                       24,
                        ShutdownPeriod = /*(boilerCostPreviousDayPrototype.Entity != null) ? boilerCostPreviousDayPrototype.ShutdownPeriod :*/ 5
                    });
                });
            });
            Items = list;
        }
#endregion
    }
}
