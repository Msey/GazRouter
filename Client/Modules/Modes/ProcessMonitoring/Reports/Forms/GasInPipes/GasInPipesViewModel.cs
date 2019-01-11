using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using GazRouter.Application;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.SeriesData.GasInPipes;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Documents.Spreadsheet.Expressions.Functions;


namespace GazRouter.Modes.ProcessMonitoring.Reports.Forms.GasInPipes
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class GasInPipesViewModel : FormViewModelBase
    {
        private ItemBase _selectedItem;

        public override ReportSettings GetReportSettings()
        {
            return new ReportSettings
            {
                SystemSelector = true,
                SerieSelector = true
            };
        }

        public List<ItemBase> Items { get; set; }

        public override async void Refresh()
        {
            if (System == null) return;
            
            try
            {
                Behavior.TryLock();

                // Список газопроводов
                var pipelines = await new ObjectModelServiceProxy().GetPipelineListAsync(
                    new GetPipelineListParameterSet { SystemId = System.Id });

                // Список КЦ
                var shops =
                    (await
                        new ObjectModelServiceProxy().GetCompShopListAsync(new GetCompShopListParameterSet
                        {
                            SystemId = System.Id
                        })).Where(s => s.KmOfConn.HasValue).ToList();
                
                // Загрузка запаса газа
                var volumes = await new SeriesDataServiceProxy().GetGasInPipeListAsync(
                    new GetGasInPipeListParameterSet
                    {
                        SystemId = System.Id,
                        BeginDate = Timestamp,
                        EndDate = Timestamp
                    });
                

                Items = new List<ItemBase>();


                // Общая сумма по ГТС
                var total = new GroupItem
                {
                    Name = "ВСЕГО по ГТС"
                };
                Items.Add(total);

                // Группы по типам газопроводов
                PipelineType[] typeList = {PipelineType.Main, PipelineType.Looping, PipelineType.Distribution};

                // todo: Добавить тип прочее

                foreach (var type in typeList)
                {
                    var pipeTypeItem = new PipeTypeItem
                    {
                        Name = ClientCache.DictionaryRepository.PipelineTypes[type].Name
                    };

                    // Группы по газопроводам
                    foreach (var pipe in pipelines.Where(p => p.Type == type))
                    {
                        var pipeItem = new PipeItem(pipe);
                        pipeTypeItem.Children.Add(pipeItem);
                            
                        // Добавляем участки с запасом
                        //pipeGroup.Children.AddRange(
                        //    volumes.Where(v => v.PipelineId == pipe.Id).Select(v => new GasInPipeItem(v)));


                        var secList = volumes.Where(v => v.PipelineId == pipe.Id).ToList();

                        // Отбираем только те КЦ, которые относятся к данному газопроводу, 
                        // и принадлежат предприятию (по границам газопровода и км. подключения цеха)
                        var shopList =
                            shops.Where(
                                s =>
                                    s.PipelineId == pipe.Id && s.KmOfConn >= pipe.KilometerOfStartPoint &&
                                    s.KmOfConn <= pipe.KilometerOfStartPoint + pipe.Length)
                                .OrderBy(s => s.KmOfConn)
                                .ToList();
                        
                        // формируем группы от станции до станции
                        if (shopList.Any())
                        {
                            var stationSegmentList = new List<StationToStationItem>();

                            if (shopList.First().KmOfConn - pipe.KilometerOfStartPoint > 1)
                                stationSegmentList.Add(new StationToStationItem(pipe.KilometerOfStartPoint, shopList.First()));

                            for (var i = 1; i < shopList.Count; i++)
                            {
                                if (shopList[i-1].KmOfConn == shopList[i].KmOfConn) continue;
                                stationSegmentList.Add(new StationToStationItem(shopList[i - 1], shopList[i]));
                            }

                            if (pipe.KilometerOfStartPoint + pipe.Length - shopList.Last().KmOfConn > 1)
                                stationSegmentList.Add(new StationToStationItem(shopList.Last(),
                                    pipe.KilometerOfStartPoint + pipe.Length));
                            

                            // Теперь нужно запихнуть туда конкретные участки
                            if (stationSegmentList.Any())
                            {
                                foreach (var sec in secList)
                                {
                                    var tmpLst =
                                        stationSegmentList.Where(
                                            s =>
                                                (s.KmBegin <= sec.KmBegin && sec.KmBegin <= s.KmEnd) ||
                                                (s.KmBegin <= sec.KmEnd && sec.KmEnd <= s.KmEnd)).OrderBy(s => s.KmBegin);


                                    if (tmpLst.Count() == 1)
                                        tmpLst.First().Children.Add(new SectionItem(sec));

                                    if (tmpLst.Count() == 2)
                                    {
                                        if (tmpLst.First().KmEnd - sec.KmEnd > sec.KmBegin - tmpLst.Last().KmBegin)
                                            tmpLst.First().Children.Add(new SectionItem(sec));
                                        else
                                            tmpLst.Last().Children.Add(new SectionItem(sec));


                                    }

                                }
                            }

                            pipeItem.Children.AddRange(stationSegmentList);
                        }
                        else
                            pipeItem.Children.AddRange(secList.Select(s => new SectionItem(s)));
                    }

                    

                    if (pipeTypeItem.Children.Any())
                        total.Children.Add(pipeTypeItem);

                }
                    
                OnPropertyChanged(() => Items);

            }
            finally 
            {
                Behavior.TryUnlock();
            }
        }


        public ItemBase SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    LoadTrend(value);
                }
            }
        }

        
        public List<Tuple<DateTime, double?>> TrendData { get; set; }

        private async void LoadTrend(ItemBase item)
        {
            TrendData = new List<Tuple<DateTime, double?>>();
            if (item != null)
                TrendData = await item.GetTrend(Timestamp.AddDays(-1), Timestamp);
            OnPropertyChanged(() => TrendData);
            OnPropertyChanged(() => TrendMax);
        }

        public double? TrendMax
        {
            get
            {
                return 1.5 * TrendData?.Max(v => v.Item2);
            }
        }
        
    }



   


    public abstract class ItemBase
    {
        protected ItemBase()
        {
            Children = new List<ItemBase>();
        }
        
        [Display(AutoGenerateField = false)]
        public List<ItemBase> Children { get; set; }

        /// <summary>
        /// Наименование объекта
        /// </summary>
        public virtual string Name { get; set; }



        public virtual string ImageSource { get; set; }


        public bool HasImage => !string.IsNullOrEmpty(ImageSource);


        /// <summary>
        /// Километр начала (участка или газопровода)
        /// </summary>
        public virtual double? KmBegin { get; set; }


        /// <summary>
        /// Километр конца (участка или газопровода)
        /// </summary>
        public virtual double? KmEnd { get; set; }


        /// <summary>
        /// протяженность участка, км.
        /// </summary>
        public virtual double? Length => KmEnd - KmBegin;


        /// <summary>
        /// Диаметр участка
        /// </summary>
        public string Diameter { get; set; }


        /// <summary>
        /// Запас газа, тыс.м3
        /// </summary>
        public virtual double? Volume { get; set; }


        /// <summary>
        /// Изменение запаса, тыс.м3
        /// </summary>
        public virtual double? Delta { get; set; }


        public virtual bool IsExpanded => true;

        /// <summary>
        /// Увеличение запаса
        /// </summary>
        public bool IsGrowth => Delta.HasValue && Delta.Value > 0;


        /// <summary>
        /// Уменьшение запаса
        /// </summary>
        public bool IsReduction => Delta.HasValue && Delta.Value < 0;


        /// <summary>
        /// Данные тренда
        /// </summary>
        protected List<Tuple<DateTime, double?>> Trend; 
        public async Task<List<Tuple<DateTime, double?>>> GetTrend(DateTime begin, DateTime end)
        {
            if (Trend == null)
                Trend = await LoadTrend(begin, end);
            return await TaskEx.FromResult(Trend);
        }


        protected virtual Task<List<Tuple<DateTime, double?>>> LoadTrend(DateTime begin, DateTime end)
        {
            return TaskEx.FromResult(new List<Tuple<DateTime, double?>>());
        }

    }
    

    public class GroupItem : ItemBase
    {

        public override double? Volume
        {
            get { return Children.Any(v => v.Volume.HasValue) ? Children.Sum(v => v.Volume) : null; }
        }

        public override double? Delta
        {
            get { return Children.Any(v => v.Delta.HasValue) ? Children.Sum(v => v.Delta) : null; }
        }
    }



    /// <summary>
    /// Тип газопровода
    /// </summary>
    public class PipeTypeItem : GroupItem
    {
        
        public override string ImageSource => "/Common;component/Images/16x16/folder.png";

        public override bool IsExpanded => false;
        
    }

    /// <summary>
    /// Газопровод
    /// </summary>
    public class PipeItem : GroupItem
    {
        private PipelineDTO _dto;

        public PipeItem(PipelineDTO dto)
        {
            _dto = dto;
        }

        public override string Name => _dto.Name;

        public override string ImageSource => "/Common;component/Images/16x16/EntityTypes/pipeline.png";

        public override bool IsExpanded => false;

        protected override async Task<List<Tuple<DateTime, double?>>> LoadTrend(DateTime begin, DateTime end)
        {
            var data = await new SeriesDataServiceProxy().GetGasInPipeListAsync(
                new GetGasInPipeListParameterSet
                {
                    BeginDate = begin,
                    EndDate = end,
                    PipelineId = _dto.Id,
                    KmBegin = _dto.KilometerOfStartPoint,
                    KmEnd = _dto.KilometerOfEndPoint
                });

            return await TaskEx.FromResult(
                data.GroupBy(v => v.Timestamp)
                    .Select(t => new Tuple<DateTime, double?>(t.Key, t.Sum(i => i.Volume)))
                    .OrderBy(t => t.Item1)
                    .ToList());

        }
    }




    /// <summary>
    /// Для описания участков от КЦ до КЦ
    /// </summary>
    public class StationToStationItem : GroupItem
    {
        private readonly Guid _pipelineId;

        public StationToStationItem(double kmBegin, CompShopDTO endShop)
        {
            _pipelineId = endShop.PipelineId;
            KmBegin = kmBegin;
            KmEnd = endShop.KmOfConn;
            Name = $"{KmBegin:0.#} км. - {KmEnd:0.#} км., {endShop.StationName}";
        }
        public StationToStationItem(CompShopDTO beginShop, CompShopDTO endShop)
        {
            _pipelineId = beginShop.PipelineId;
            KmBegin = beginShop.KmOfConn;
            KmEnd = endShop.KmOfConn;
            Name = $"{KmBegin:0.#} км., {beginShop.StationName} - {KmEnd:0.#} км., {endShop.StationName}";
        }

        public StationToStationItem(CompShopDTO beginShop, double kmEnd)
        {
            _pipelineId = beginShop.PipelineId;
            KmBegin = beginShop.KmOfConn;
            KmEnd = kmEnd;
            Name = $"{KmBegin:0.#} км., {beginShop.StationName} - {KmEnd:0.#} км.";
        }

        protected override async Task<List<Tuple<DateTime, double?>>> LoadTrend(DateTime begin, DateTime end)
        {
            var data = await new SeriesDataServiceProxy().GetGasInPipeListAsync(
                new GetGasInPipeListParameterSet
                {
                    BeginDate = begin,
                    EndDate = end,
                    PipelineId = _pipelineId,
                    KmBegin = Children.Min(s => s.KmBegin),
                    KmEnd = Children.Max(s => s.KmEnd)
                });

            return await TaskEx.FromResult(
                data.GroupBy(v => v.Timestamp)
                    .Select(t => new Tuple<DateTime, double?>(t.Key, t.Sum(i => i.Volume)))
                    .OrderBy(t => t.Item1)
                    .ToList());

        }


    }






    public class SectionItem : ItemBase
    {
        private readonly GasInPipeDTO _dto;


        public SectionItem(GasInPipeDTO dto)
        {
            _dto = dto;
        }

        public override string Name => $"{_dto.KmBegin:n0} - {_dto.KmEnd:n0}";

        public override double? KmBegin => _dto.KmBegin;

        public override double? KmEnd => _dto.KmEnd;


        public override double? Volume => _dto.Volume;

        public override double? Delta => _dto.Delta;

        public override bool IsExpanded => false;


        protected override async Task<List<Tuple<DateTime, double?>>> LoadTrend(DateTime begin, DateTime end)
        {
            var data = await new SeriesDataServiceProxy().GetGasInPipeListAsync(
                new GetGasInPipeListParameterSet
                {
                    BeginDate = begin,
                    EndDate = end,
                    PipelineId = _dto.PipelineId,
                    KmBegin = _dto.KmBegin,
                    KmEnd = _dto.KmEnd
                });

            return await TaskEx.FromResult(
                data.GroupBy(v => v.Timestamp)
                    .Select(t => new Tuple<DateTime, double?>(t.Key, t.Sum(i => i.Volume)))
                    .OrderBy(t => t.Item1)
                    .ToList());

        }
    }
    
    
}
