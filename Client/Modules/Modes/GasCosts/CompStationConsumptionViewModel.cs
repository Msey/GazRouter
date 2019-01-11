using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel;
namespace GazRouter.Modes.GasCosts
{
    public class CompStationConsumptionViewModel : ConsumptionViewModelBase
    {
#region constructor
        /// <summary> empty! </summary>
        /// <param name="gasCostsMainViewModel"></param>
        public CompStationConsumptionViewModel(GasCostsMainViewModel gasCostsMainViewModel)
            : base(gasCostsMainViewModel)
        {
        }
#endregion

#region property
        public override string Header => "КC,КЦ";
        public TreeDataDTO TreeData { get; set; }
#endregion

#region methods
        public static IEnumerable<ColumnDescription> GetStaticColumnCollection()
        {
            return new List<ColumnDescription>
            {
                new ColumnDescription(CostType.CT12, "Расход топливного газа ГПА для компримирования газа КЦ"),
                new ColumnDescription(CostType.CT20, "На подогрев топливного газа в КЦ"),
                new ColumnDescription(CostType.CT23, "На ремонт, реконструкцию и техническое перевооружение КЦ"),
                new ColumnDescription(CostType.CT22, "При плановой остановке КЦ"),
                new ColumnDescription(CostType.CT58, "На проведение химического анализа газа"),
                new ColumnDescription(CostType.CT66, "На обогрев укрытий ГПА"),
                new ColumnDescription(CostType.CT83, "Для опробования ГПА на работоспособность"),
                new ColumnDescription(CostType.CT65, "На установку термического обезвреживания"),
                new ColumnDescription(CostType.CT13, "На выработку холода СОГ"),
                new ColumnDescription(CostType.CT14,
                    "На выработку тепловой энергии котельными для горячего водоснабжения производственных и административных зданий КЦ, КС"),
                new ColumnDescription(CostType.CT15,"На выработку электроэнергии ЭСН"),
                new ColumnDescription(CostType.CT95,"На поддержание в работоспособном состоянии резервных источников электроэнергии (КЦ)"),
                new ColumnDescription(CostType.CT84,
                    "На ревизию и замену средств измерений количества газа (на ревизию и замену сужающего устройства на узле измерения расхода газа"),
                new ColumnDescription(CostType.CT16, "На подогрев газа в ЦООГ"),
                new ColumnDescription(CostType.CT92, "На собственные технологические нужды КПТГ"),
                new ColumnDescription(CostType.CT85, "На освидетельствование сосудов, работающих под давлением"),
                new ColumnDescription(CostType.CT18, "Стравливание газа при плановых пусках ГПА"),
                new ColumnDescription(CostType.CT19, "Стравливание газа при плановых остановках ГПА"),
                new ColumnDescription(CostType.CT21,
                    "Стравливание газа при эксплуатации силовых пневмоприводов кранов, кранов–регуляторов в КЦ"),
                new ColumnDescription(CostType.CT24,
                    "Стравливание газа при продувке аппаратов (пылеуловители, фильтры-сепараторы, адсорберы, сепараторы, ресиверы и др.) КЦ"),
                new ColumnDescription(CostType.CT57,
                    "Стравливание газа при проверке работоспособности предохранительных клапанов в блоке подготовки топливного газа КЦ, котельной"),
                new ColumnDescription(CostType.CT17, "Стравливание газа из системы уплотнения ЦБК"),
                new ColumnDescription(CostType.CT61, "Утечки газа на технологических объектах КС"),
                new ColumnDescription(CostType.CT27,
                    "Стравливание газа при эксплуатации приборов КИП, автоматики и телемеханики"),
                new ColumnDescription(CostType.CT87,
                    "Стравливание газа при продувке дренажей УСБ и продувке импульсных линий отбора газа на датчики давления и перепада давления"),
                new ColumnDescription(CostType.CT98,
                    "Потери при авариях на КС"),
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
        public void LoadTree(TreeDataDTO data)
        {
            TreeData = data;
            Entities = data.CompShops.Cast<EntityDTO>().ToList();
            UpdateColumnsVisibility(data);
        } 

        protected override void FillGasCostsSummaries(EntitySummaryRow totalSummaryRow)
        {
            foreach (var compStationDTO in TreeData.CompStations)
            {
                var compStationsSummaryRow = new EntitySummaryRow(compStationDTO, this); // КС
                var boilersPlantsSummaryRow = new EntitySummaryRow("Котельные", this) { IsExpanded = false }; ;   // Котельные
                foreach (var boilerPlantDTO in TreeData.BoilerPlants.Where(c => c.ParentId == compStationDTO.Id))// Котельные
                {
                    var boilerPlantSummaruRow = new EntitySummaryRow(boilerPlantDTO, this) { IsExpanded = false};
                    foreach (var boiler in TreeData.Boilers.Where(c => c.ParentId == boilerPlantDTO.Id)) // Котлы
                        boilerPlantSummaruRow.AddSubItem(new EntitySummaryRow(boiler, this) { IsExpanded = false });
                    boilersPlantsSummaryRow.AddSubItem(boilerPlantSummaruRow);
                }
                if (boilersPlantsSummaryRow.SubItemsCount != 0)
                    compStationsSummaryRow.AddSubItem(boilersPlantsSummaryRow);
                var powerPlantSummaryRow = new EntitySummaryRow("ЭСН", this) { IsExpanded = false }; ;
                foreach (var powerPlantDTO in TreeData.PowerPlants.Where(c => c.ParentId == compStationDTO.Id))// ЭСН - электростанция собственных нужд
                {
                    var powerPlantRow = new EntitySummaryRow(powerPlantDTO, this){ IsExpanded = false };
                    foreach (var powerUnitDTO in TreeData.PowerUnits.Where(c => c.ParentId == powerPlantDTO.Id))// Электроагрегат
                        powerPlantRow.AddSubItem(new EntitySummaryRow(powerUnitDTO, this) { IsExpanded = false });
                    if (powerPlantRow.SubItemsCount > 0)
                        powerPlantSummaryRow.AddSubItem(powerPlantRow);
                }
                if (powerPlantSummaryRow.SubItemsCount > 0)
                    compStationsSummaryRow.AddSubItem(powerPlantSummaryRow);
                var coolingStationsSummaryRow = new EntitySummaryRow("СОГ-и", this) { IsExpanded = false }; ;
                foreach (var coolingStationDTO in TreeData.CoolingStations.Where(c => c.ParentId == compStationDTO.Id))// СОГ - станция охлаждения газа
                {
                    var coolingStationRow = new EntitySummaryRow(coolingStationDTO, this) { IsExpanded = false };
                    foreach (var coolingUnitDTO in TreeData.CoolingUnits.Where(c => c.ParentId == coolingStationDTO.Id))// УОГ - установка охлаждения газа
                        coolingStationRow.AddSubItem(new EntitySummaryRow(coolingUnitDTO, this) { IsExpanded = false });
                    if (coolingStationRow.SubItemsCount > 0)
                        coolingStationsSummaryRow.AddSubItem(coolingStationRow);
                }
                if (coolingStationsSummaryRow.SubItemsCount > 0)
                compStationsSummaryRow.AddSubItem(coolingStationsSummaryRow);
                foreach (var compShopDTO in TreeData.CompShops.Where(c => c.ParentId == compStationDTO.Id))// КЦ
                {
                    var compShopSummaryRow = new EntitySummaryRow(compShopDTO, this) { IsExpanded = false };// var sr = new EntitySummaryRow("ГПА", this);
                    foreach (var compUnitDTO in TreeData.CompUnits.Where(c => c.ParentId == compShopDTO.Id))// ГПА
                        compShopSummaryRow.AddSubItem(new EntitySummaryRow(compUnitDTO, this) { IsExpanded = false });/* if (sr.SubItemsCount > 0) compShopSummaryRow.AddSubItem(sr);*/
                    compStationsSummaryRow.AddSubItem(compShopSummaryRow);
                }
                totalSummaryRow.AddSubItem(compStationsSummaryRow);
            }
        }
    }
#endregion
}