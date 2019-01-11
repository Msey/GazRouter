using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.Boilers;
using JetBrains.Annotations;
using System.Diagnostics;

namespace GazRouter.Modes.GasCosts
{
    public class PipelineConsumptionViewModel : ConsumptionViewModelBase
    {
        private List<PipelineTypeForFilter> _pipeLinesTreeFilters = null;
        private List<PipelineTypeForFilter> _pipeLinesMainFilters = null;
        public PipelineConsumptionViewModel([NotNull] GasCostsMainViewModel mainViewModel)
            : base(mainViewModel)
        {
            _pipeLinesTreeFilters = mainViewModel.PipeLinesTreeFilters;
            _pipeLinesMainFilters = mainViewModel.pipelineMainTypes;
        }
        public override string Header
        {
            get
            {
                return "Линейная часть";
            }
        }

        public static IEnumerable<ColumnDescription> GetStaticColumnCollection()
        {
                        return new List<ColumnDescription> {
                             new ColumnDescription(CostType.CT29,"На ремонт, реконструкцию и техническое перевооружение участков МГ, врезку отводов и перемычек"),
                             new ColumnDescription(CostType.CT31,"На очистку внутренней полости участков МГ очистными устройствами"),
                             new ColumnDescription(CostType.CT32,"На проведение внутритрубной технической диагностики ЛЧ МГ"),
                             new ColumnDescription(CostType.CT33,"На заправку метанольных установок"),
                             new ColumnDescription(CostType.CT34,"На ликвидацию гидратных пробок"),
                             new ColumnDescription(CostType.CT55,"На выработку электроэнергии ЭСН"),
                             new ColumnDescription(CostType.CT96,"На поддержание в работоспособном состоянии резервных источников электроэнергии"),
                             new ColumnDescription(CostType.CT36,"На выработку тепловой энергии для обогрева помещений радиорелейных пунктов, домов линейных обходчиков и производственных зданий"),
                             new ColumnDescription(CostType.CT60,"На проведение химического анализа газа"),
                             new ColumnDescription(CostType.CT93,"На собственные нужды автоматических редуцирующих пунктов"),
                             new ColumnDescription(CostType.CT28,"Стравливание газа при эксплуатации силовых пневмоприводов кранов на ЛЧ"),
                             new ColumnDescription(CostType.CT88,"Стравливание газа при эксплуатации кранов–регуляторов в пунктах регулирования давления газа на межсистемных перемычках ЛЧ МГ"),
                             new ColumnDescription(CostType.CT30,"Стравливание газа при продувке аппаратов, импульсных линий"),
                             new ColumnDescription(CostType.CT89,"Стравливание газа при проверке работоспособности предохранительных клапанов на газопроводах-отводах"),
                             new ColumnDescription(CostType.CT64,"Утечки газа на технологических объектах ЛЧ МГ"),
                             new ColumnDescription(CostType.CT90,"Стравливание газа при эксплуатации приборов КИП, автоматики и телемеханики"),
                             new ColumnDescription(CostType.CT35,"На продувку импульсных линий ПРГ"),
                             new ColumnDescription(CostType.CT37,"При эксплуатации кранов ПРГ"),
                             new ColumnDescription(CostType.CT44,"При эксплуатации кранов–регуляторов в пунктах регулирования давления газа"),
                             new ColumnDescription(CostType.CT53,"Стравливание газа при проверке работоспособности предохранительных клапанов на узлах редуцирования"),
                             new ColumnDescription(CostType.CT51,"При утечке газа на технологических объектах ЛЧ МГ (ПРГ)"),
                             new ColumnDescription(CostType.CT94,"Стравливание газа при эксплуатации приборов КИП, автоматики и телемеханики ПРГ"),
                             new ColumnDescription(CostType.CT100,"Потери при ликвидации аварий на трубопроводах (ЛЧ)"),
                         };   
        }

        public override List<CostType> GetCostTypeCollection()
        {
            return GetStaticColumnCollection().Select(x => x.CostType).ToList();
        }
        protected override List<ColumnDescription> GetColumnCollection()
        {
            var columnCollection = GetStaticColumnCollection().ToList();
            BindStnToColumnsHeader(columnCollection);
            return columnCollection;
        }
        protected override void FillGasCostsSummaries(EntitySummaryRow totalSummaryRow)
        {
            bool containsReducingStations = false;
            bool containsBoilers = false;
            bool containsPowerUnits = false;

            var SelectedPipelineMainTypes = MainViewModel.pipelineMainTypes.Where(pltf => pltf.IsSelected).ToList();
            var SelectedPipelineTypes = MainViewModel.PipeLinesTreeFilters.Where(pltf => pltf.IsSelected).ToList();
            foreach (var PipelineMainType in SelectedPipelineMainTypes)
            {
                var PipelineMainTypeDTO = new PipelineTypeEntityDTO(PipelineMainType.DisplayName, SiteId);
                var PipelineMainTypeSummaryRow = new EntitySummaryRow(PipelineMainTypeDTO.Name, this);

                foreach (var PipelineMainDTO in TreeData.Pipelines.Where(pl => pl.Type == PipelineMainType.Value).Select(p => new PipelineEntityDTO(p, PipelineMainTypeDTO.Id)))
                {
                    var PipelineMainSummaryRow = new EntitySummaryRow(PipelineMainDTO, this) { IsExpanded=false, };
                    FillChildGasCostsSummaries(PipelineMainSummaryRow, SelectedPipelineTypes, PipelineMainDTO, ref containsReducingStations, ref containsBoilers, ref containsPowerUnits);
                    PipelineMainTypeSummaryRow.AddSubItem(PipelineMainSummaryRow);
                }

                if (PipelineMainTypeSummaryRow.SubItemsCount > 0)
                    totalSummaryRow.AddSubItem(PipelineMainTypeSummaryRow);
            }

            if (!containsReducingStations) HideColumn(EntityType.ReducingStation);
            if (!containsBoilers) HideColumn(EntityType.Boiler);
            if (!containsPowerUnits) HideColumn(EntityType.PowerUnit);
        }

        private void FillChildGasCostsSummaries(EntitySummaryRow ParentSummaryRow, List<PipelineTypeForFilter> PipelineChildTypes, PipelineEntityDTO PipelineParentDTO, ref bool containsReducingStations, ref bool containsBoilers, ref bool containsPowerUnits)
        {

            var ReducingStationsSummaryRow = new EntitySummaryRow("ПРГ", this) { IsExpanded = false };
            foreach (var ReducingStationDTO in TreeData.ReducingStations.Where(rs => rs.PipelineId == PipelineParentDTO.Id))
                ReducingStationsSummaryRow.AddSubItem(new EntitySummaryRow(ReducingStationDTO, this) { IsExpanded = false });

            var BoilersSummaryRow = new EntitySummaryRow("Котлы", this) { IsExpanded = false };
            foreach (var BoilerDTO in TreeData.Boilers.Where(b => b.ParentId == PipelineParentDTO.Id && b.SiteId == SiteId))
                BoilersSummaryRow.AddSubItem(new EntitySummaryRow(BoilerDTO, this) { IsExpanded = false });
            // сортировка котлов
            BoilersSummaryRow.Items.Sort((ex, ey) =>
            {
                if ((ex.Entity as BoilerDTO).Kilometr != (ey.Entity as BoilerDTO).Kilometr)
                {
                    return (ex.Entity as BoilerDTO).Kilometr
                           .CompareTo((ey.Entity as BoilerDTO).Kilometr);
                }
                return ex.Entity.Name
                           .CompareTo(ey.Entity.Name);
            });
            //

            var PowerUnitsSummaryRow = new EntitySummaryRow("Электроагрегаты", this) { IsExpanded = false };
            foreach (var PowerUnitDTO in TreeData.PowerUnits.Where(pu => pu.ParentId == PipelineParentDTO.Id && pu.SiteId == SiteId))
                PowerUnitsSummaryRow.AddSubItem(new EntitySummaryRow(PowerUnitDTO, this) { IsExpanded = false });

            if (ReducingStationsSummaryRow.SubItemsCount > 0)
            {
                ParentSummaryRow.AddSubItem(ReducingStationsSummaryRow);
                containsReducingStations = true;
            }
            if (BoilersSummaryRow.SubItemsCount > 0)
            {
                ParentSummaryRow.AddSubItem(BoilersSummaryRow);
                containsBoilers = true;
            }
            if (PowerUnitsSummaryRow.SubItemsCount > 0)
            {
                ParentSummaryRow.AddSubItem(PowerUnitsSummaryRow);
                containsPowerUnits = true;
            }

            foreach (var PipelineChildType in PipelineChildTypes)
            {
                var PipelineChildTypeDTO = new PipelineTypeEntityDTO(PipelineChildType.DisplayName, SiteId);
                var PipelineChildTypeSummaryRow = new EntitySummaryRow(PipelineChildTypeDTO.Name, this) { IsExpanded = false };
                
                foreach (var PipelineChildDTO in TreeData.Pipelines.Where(pl => pl.Type == PipelineChildType.Value &&
                                ((pl.Type == PipelineType.CompressorShopOutlet && pl.EndEntityId == PipelineParentDTO.Id) || pl.BeginEntityId == PipelineParentDTO.Id)).
                                    Select(p => new PipelineEntityDTO(p, PipelineChildTypeDTO.Id)))
                {
                    var PipelineChildSummaryRow = new EntitySummaryRow(PipelineChildDTO, this) { IsExpanded = false, }; ;
                    FillChildGasCostsSummaries(PipelineChildSummaryRow, PipelineChildTypes, PipelineChildDTO, ref containsReducingStations, ref containsBoilers, ref containsPowerUnits);
                    PipelineChildTypeSummaryRow.AddSubItem(PipelineChildSummaryRow);
                }              

                if (PipelineChildTypeSummaryRow.SubItemsCount > 0)
                    ParentSummaryRow.AddSubItem(PipelineChildTypeSummaryRow);
            }
        }
        
        public TreeDataDTO TreeData { get; set; }

        class PipelineTypeEntityDTO : EntityDTO
        {
            public PipelineTypeEntityDTO(string pipeLineTypeName, Guid siteId)
            {
                Id = Guid.NewGuid();
                ParentId = siteId;
                Name = pipeLineTypeName;
            }

            public PipelineTypeEntityDTO(PipelineTypesDTO dto, Guid siteId):this(dto.Name, siteId)
            {
            }
        }
        class PipelineEntityDTO : EntityDTO
        {
            private readonly PipelineDTO _pipelineDTO;
            public PipelineType PipelineType => _pipelineDTO.Type;
            public string PipelineTypeName => _pipelineDTO.TypeName;            
            public PipelineEntityDTO(PipelineDTO pipelineDTO, Guid parentId)
            {
                _pipelineDTO = pipelineDTO;
                ParentId = parentId;
                Id = pipelineDTO.Id;
                Name = pipelineDTO.Name;
                ShortPath = pipelineDTO.Name;
                Path = pipelineDTO.Name;
            }

            public override EntityType EntityType { get
            {
                return _pipelineDTO.EntityType;
            } }
        }

        public void LoadTree(TreeDataDTO data, object pipetypes = null)
        {
            var dicTypes = ClientCache.DictionaryRepository.PipelineTypes.Values.OrderBy(c => c.SortOrder).ToDictionary(
              c => c.PipelineType,
              c => new PipelineTypeEntityDTO(c, SiteId));

            // сортировка котлов
            int i = 0; 
            var sortboilers = data.Boilers.OrderBy(b => b.ParentId).ThenBy(b => b.Kilometr).ThenBy(b => b.Name);
            foreach (BoilerDTO bs in sortboilers){data.Boilers[i] = bs; i++;}

            TreeData = data;

            Entities = data.Pipelines.Select(p => new PipelineEntityDTO(p, dicTypes[p.Type].Id)).Cast<EntityDTO>().ToList();
            //Entities.AddRange(dicTypes.Values);
            UpdateColumnsVisibility(data);
        }
    }
}