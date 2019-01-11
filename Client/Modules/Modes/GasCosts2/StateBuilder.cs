using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GazRouter.DataProviders.GasCosts;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.GasCostItemGroups;
using GazRouter.DTO.GasCosts;
using Microsoft.Practices.Prism;
namespace GazRouter.Modes.GasCosts2
{
    public class StateBuilder
    {
        public Action<StateItem> VisibilityChangedAction { get; set; }

        private void VisibilityChanged(StateItem stateItem)
        {
            VisibilityChangedAction?.Invoke(stateItem);
        }
        public async Task<StateItem> BuildStatesTree(List<GasCostTypeDTO> costMap,
                                                     List<GasCostItemGroupDTO> itemGroups,
                                                     bool isEnabled = true,
                                                     Guid? siteId   = null)
        {
            Dictionary<int, int?> dict = null;
            if (siteId != null)
                dict = await GetStatePermissions(siteId.Value).ConfigureAwait(false);
            //
            var root     = new StateGroup("Всего по ЛПУ");
            var kskc     = new StateGroup("КС, КЦ");
            var pipeline = new StateGroup("Линейная часть");
            var grs      = new StateGroup("ГРС");
            var gis      = new StateGroup("ГИС");
            //
            var task1 = BuildGroup(kskc, costMap, GetTabNum(EntityType.CompStation),  itemGroups, dict, isEnabled);
            var task2 = BuildGroup(pipeline, costMap, GetTabNum(EntityType.Pipeline), itemGroups, dict, isEnabled);
            var task3 = BuildGroup(grs, costMap, GetTabNum(EntityType.DistrStation),  itemGroups, dict, isEnabled);
            var task4 = BuildGroup(gis, costMap, GetTabNum(EntityType.MeasStation),   itemGroups, dict, isEnabled);
            await TaskEx.WhenAll(task1, task2, task3, task4).ConfigureAwait(false);
            // 
            root.Items.Add(kskc);
            root.Items.Add(pipeline);
            root.Items.Add(grs);
            root.Items.Add(gis);
            return root;
        }
        private static async Task<Dictionary<int, int?>> GetStatePermissions(Guid siteId)
        {
            var visibility = await new GasCostsServiceProxy().GetGasCostsVisibilityAsync(siteId);
            var dict       = visibility.ToDictionary(k=>k.CostType, v=>v.Visibility);
            return dict;
        }
        private Task BuildGroup(StateItem summaryItem,
                                IEnumerable<GasCostTypeDTO> stateMap,
                                int tabNum,
                                IEnumerable<GasCostItemGroupDTO> itemGroups,
                                IDictionary<int, int?> dict, bool isEnabled)
        {
            return 
            Task.Factory.StartNew(() =>
            {
                Predicate<int> filterByVisible;
                if (dict == null)
                    filterByVisible = i => true;
                else
                    filterByVisible = costType => !dict.ContainsKey(costType);

                var mapSelectTabNum = stateMap.Where(e => e.TubNum == tabNum).ToArray();
                foreach (var group in itemGroups)
                {
                    if (mapSelectTabNum.Length == 0) continue;
                    //
                    var mapSelectGroup = mapSelectTabNum.Where(e => e.GroupId == group.Id && filterByVisible((int)e.CostType))
                                                        .Select(e => new StateItem(e.CostTypeName, VisibilityChanged)
                                                        {
                                                            TabNum              = tabNum,
                                                            GroupId             = group.Id,
                                                            CostType            = e.CostType,
                                                            CostTypeDescription = e.CostType.ToString(),
                                                            Regular             = e.IsRegular,
                                                            IsEnabled           = isEnabled
                                                        }).ToArray();
                    if (mapSelectGroup.Length == 0) continue;
                    //
                    var newGroup = new StateGroup(group.Name);
                    newGroup.Items.AddRange(mapSelectGroup);
                    summaryItem.Items.Add(newGroup);
                }
            });
        }
        private static int GetTabNum(EntityType entityType)
        {
            switch (entityType)
            {
                case EntityType.CompStation: return 1;  // КС(КЦ)
                case EntityType.Pipeline: return 2;     // Линейная часть
                case EntityType.DistrStation: return 3; // ГРС
                case EntityType.MeasStation: return 4;  // ГИС
            }
            throw new Exception("Недопустимый тип объекта!");
        }
    }
}
