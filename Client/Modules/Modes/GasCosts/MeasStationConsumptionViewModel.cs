using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.ObjectModel;
using JetBrains.Annotations;
namespace GazRouter.Modes.GasCosts
{
    public class MeasStationConsumptionViewModel : ConsumptionViewModelBase
    {
        public MeasStationConsumptionViewModel([NotNull] GasCostsMainViewModel mainViewModel)
            : base(mainViewModel)
        {
        }

        public override string Header
        {
            get { return "ГИС"; }
        }
        public static IEnumerable<ColumnDescription> GetStaticColumnCollection()
        {
            return new List<ColumnDescription>
            {
                new ColumnDescription(CostType.CT47, "На ревизию и замену средств измерений расхода газа, сужающих устройств"),
                new ColumnDescription(CostType.CT50, "На ремонт и реконструкцию ГИС"),
                new ColumnDescription(CostType.CT52, "Для обогрева зданий ГИС"),
                new ColumnDescription(CostType.CT26, "На проведение химического анализа газа"),
                new ColumnDescription(CostType.CT49, "Стравливание газа при эксплуатации силовых пневмоприводов кранов"),
                new ColumnDescription(CostType.CT48, "Стравливание газа при работе пневморегуляторов, пневмоустройств на газе"),
                new ColumnDescription(CostType.CT56, "Стравливание газа при проверке работоспособности предохранительных клапанов"),
                new ColumnDescription(CostType.CT54, "Стравливание газа при продувке дренажей УСБ и продувку импульсных линий отбора газа на датчики давления и перепада давления"),
                new ColumnDescription(CostType.CT63, "Утечки газа на технологических объектах ГИС"),
                new ColumnDescription(CostType.CT91, "Стравливание газа при эксплуатации приборов КИП, автоматики и телемеханики"),
                new ColumnDescription(CostType.CT104, "Потери при авариях на ГИС"),
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
            foreach (var measStationDTO in TreeData.MeasStations.Where(e=>!e.IsVirtual))
            {                
                var stationsSummaryRow = new EntitySummaryRow(measStationDTO, this);

                var boilersSummaryRow = new EntitySummaryRow("Котлы", this);

                foreach (var boilerDTO in TreeData.Boilers.Where(c => c.ParentId == measStationDTO.Id))
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
        public TreeDataDTO TreeData { get; set; }

        public void LoadTree(TreeDataDTO data)
        {
            Entities = data.MeasStations.Cast<EntityDTO>().ToList();
            TreeData = data;
            UpdateColumnsVisibility(data);
        }
        
    }
}