using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel;
using JetBrains.Annotations;

namespace GazRouter.Modes.GasCosts
{
    public class DistrStationConsumptionViewModel : ConsumptionViewModelBase
    {
        public DistrStationConsumptionViewModel([NotNull] GasCostsMainViewModel mainViewModel)
            : base(mainViewModel)
        {
        }

        public override string Header => "ГРС";

        public TreeDataDTO TreeData { get; set; }

        public void LoadTree(TreeDataDTO data)
        {
            TreeData = data;
            Entities = data.DistrStations.Cast<EntityDTO>().ToList();
            UpdateColumnsVisibility(data);
        }

        protected override List<ColumnDescription> GetColumnCollection()
        {
            var columnCollection = GetStaticColumnCollection().ToList();
            BindStnToColumnsHeader(columnCollection);
            return columnCollection;
        }
        public static IEnumerable<ColumnDescription> GetStaticColumnCollection()
        {
            return new List<ColumnDescription>
            {
                new ColumnDescription(CostType.CT38, "На работу подогревателей редуцируемого газа перед регуляторами давления"),
                new ColumnDescription(CostType.CT40, "На заправку одоризационных и метанольных установок"),
                new ColumnDescription(CostType.CT41, "На ревизию и замену средств измерений расхода газа (ревизию и замену сужающего устройства на узле измерения расхода газа)"),
                new ColumnDescription(CostType.CT43, "На ремонт и реконструкцию ГРС"),
                new ColumnDescription(CostType.CT45, "На выработку тепловой энергии для обогрева зданий ГРС и домов операторов"),
                new ColumnDescription(CostType.CT59, "На проведение химического анализа газа"),
                new ColumnDescription(CostType.CT86, "На освидетельствование сосудов, работающих под давлением"),
                new ColumnDescription(CostType.CT42, "Стравливание газа при эксплуатации силовых пневмоприводов кранов"),
                new ColumnDescription(CostType.CT25, "Стравливание газа при при проверке работоспособности предохранительных клапанов"),
                new ColumnDescription(CostType.CT74, "Стравливание газа при работе пневморегуляторов, пневмоустройств на газе"),
                new ColumnDescription(CostType.CT39, "Стравливание газа при продувке аппаратов (сепараторов, пылеуловителей, конденсатосборников)"),
                new ColumnDescription(CostType.CT75, "Стравливание газа при продувке дренажей УСБ и продувку импульсных линий отбора газа на датчики давления и перепада давления"),
                new ColumnDescription(CostType.CT62, "Утечки газа на технологических объектах ГРС"),
                new ColumnDescription(CostType.CT46, "Стравливание газа при эксплуатации приборов КИП, автоматики и телемеханики"),
                new ColumnDescription(CostType.CT102, "Потери при авариях на ГРС"),
            };
        }

        public override List<CostType> GetCostTypeCollection()
        {
            return GetStaticColumnCollection().Select(x => x.CostType).ToList();
        }
        protected override void FillGasCostsSummaries(EntitySummaryRow totalSummaryRow)
        {
            foreach (var distrStationDTO in TreeData.DistrStations)
            {
                var stationsSummaryRow = new EntitySummaryRow(distrStationDTO, this);

                var boilersSummaryRow = new EntitySummaryRow("Котлы", this);

                foreach (var boilerDTO in TreeData.Boilers.Where(c => c.ParentId == distrStationDTO.Id))
                {
                    boilersSummaryRow.AddSubItem(new EntitySummaryRow(boilerDTO, this));
                }
                if (boilersSummaryRow.SubItemsCount != 0)
                {
                    stationsSummaryRow.AddSubItem(boilersSummaryRow);
                }


                totalSummaryRow.AddSubItem(stationsSummaryRow);
            }
        }
        
    }
}