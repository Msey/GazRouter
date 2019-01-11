using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Application.Helpers;
using GazRouter.Common.ViewModel;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ManualInput.ValveSwitches;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.Dialogs.ValveControlCosts;
using Microsoft.Practices.Prism.Commands;
using Utils.Extensions;
namespace GazRouter.Modes.GasCosts.Imports
{
    public class ImportValveSwitchesViewModel : DialogViewModel
	{
        public ImportValveSwitchesViewModel(Action closeCallback, DateTime callerSelectedDate, Guid selectedSiteId)
            : base(closeCallback)
        {
            _selectedDate = callerSelectedDate.ToLocal();
            _selectedSiteId = selectedSiteId;
            SaveCommand = new DelegateCommand(Save);
            LoadData();
        }



        public DelegateCommand SaveCommand { get; set; }

        private DateTime _selectedDate;

        private Guid _selectedSiteId;

        public string InputDate => _selectedDate.ToLongDateString();

        public List<GroupItem> ValveSwitches { get; set; }



        private async void LoadData()
        {
            try
            {
                Behavior.TryLock();

                var switches = (await new ManualInputServiceProxy().GetValveSwitchListAsync(
                    new GetValveSwitchListParameterSet
                    {
                        BeginDate = SeriesHelper.GetDispDayStart(_selectedDate.Date).ToLocal(),
                        EndDate = SeriesHelper.GetDispDayEnd(_selectedDate.Date).ToLocal(),
                        SiteId = _selectedSiteId
                    })).Select(s => new ValveSwitch(s)).ToList();

                
                ValveSwitches = new List<GroupItem>();


                
                foreach (var byGroup in switches.GroupBy(s => s.GroupId))
                {
                    var g = switches.First(s => s.GroupId == byGroup.Key);
                    var group = new GroupItem
                    {
                        EntityId = g.GroupId,
                        EntityType = g.Dto.CompShopId.HasValue ? EntityType.CompShop : EntityType.Pipeline,
                        SiteId = g.Dto.SiteId,
                        Name = g.GroupName,
                        TypeName = g.GroupTypeName
                    };
                    ValveSwitches.Add(group);
                    

                    foreach (var byValveType in byGroup.GroupBy(s => s.ValveTypeId))
                    {
                        var valveType = ClientCache.DictionaryRepository.ValveTypes.Single(t => t.Id == byValveType.Key);
                        var v = new SwitchItem
                        {
                            ValveTypeId = valveType.Id,
                            Name = valveType.Name,
                            Count = byValveType.Count(),
                            Norm = valveType.RatedConsumption,
                            Valves = string.Join(", ", byValveType.Select(s => s.ValveName).Distinct())
                        };
                        group.Children.Add(v);
                    }
                }

                OnPropertyChanged(() => ValveSwitches);
            }
            finally
            {
               Behavior.TryUnlock();
            }
        }

        private void Save()
        {
            try
            {
                Behavior.TryLock();
                foreach (var group in ValveSwitches)
                {
                    var model = new ValveControlCostsModel();
                    model.ValveShiftings.AddRange(
                        group.Children.Select(
                            s => new ValveShifting
                            {
                                Id = s.ValveTypeId,
                                Name = s.Name,
                                Count = (uint)s.Count,
                                RatedConsumption = s.Norm
                            }));
                    model.Calculate();

                    GasCostImportHelper.SaveCost(
                        model,
                        _selectedDate.Date.ToLocal(),
                        group.EntityId,
                        group.EntityType == EntityType.CompShop ? CostType.CT21 : CostType.CT28,
                        group.SiteId,
                        null);
                }
            }
            finally
            {
                Behavior.TryUnlock();
                DialogResult = true;
            }
        }


        public class ValveSwitch
        {

            public ValveSwitch(ValveSwitchDTO dto)
            {
                Dto = dto;
            }

            public ValveSwitchDTO Dto { get; }

            public Guid GroupId => Dto.CompShopId ?? Dto.PipelineId;

            public string GroupName => Dto.CompShopId.HasValue
                ? $"{Dto.CompStationName}. {Dto.CompShopName}"
                : Dto.PipelineName;

            public string GroupTypeName => Dto.CompShopId.HasValue ? "КЦ" : "Газопровод";

            public int ValveTypeId 
            {
                get
                {
                    switch (Dto.SwitchType)
                    {
                        case ValveSwitchType.Valve:
                            return Dto.ValveTypeId;
                        case ValveSwitchType.Bypass1:
                            return Dto.Bypass1TypeId;
                        case ValveSwitchType.Bypass2:
                            return Dto.Bypass2TypeId;
                        case ValveSwitchType.Bypass3:
                            return Dto.Bypass3TypeId;
                        default:
                            return -1;
                    }
                }
            }

            public string ValveName
            {
                get
                {
                    switch (Dto.SwitchType)
                    {
                        case ValveSwitchType.Valve:
                            return Dto.ValveName;
                        case ValveSwitchType.Bypass1:
                            return Dto.ValveName + ".1";
                        case ValveSwitchType.Bypass2:
                            return Dto.ValveName + ".2";
                        case ValveSwitchType.Bypass3:
                            return Dto.ValveName + ".4";
                        default:
                            return "";
                    }
                }
            }
        }


        public class ItemBase
        {
            public string Name { get; set; }

            public int ValveTypeId { get; set; }

            public string TypeName { get; set; }

            public string Valves { get; set; }

            public int Count { get; set; }

            public double Norm { get; set; }
            
            public virtual double Q { get; set; }
        }

        public class GroupItem : ItemBase
        {
            public GroupItem()
            {
                Children = new List<SwitchItem>();
            }

            public List<SwitchItem> Children { get; set; }

            public Guid EntityId { get; set; }

            public EntityType EntityType { get; set; }

            public Guid SiteId { get; set; }
        

            public override double Q => Children.Sum(c => c.Q);
    }


        public class SwitchItem : ItemBase
        {
            public override double Q => Count*Norm;
        }
	}
}
