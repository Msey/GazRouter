using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using GazRouter.Application;
using GazRouter.Common.Cache;
using GazRouter.DataProviders.ManualInput;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ManualInput.CompUnitStates;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.Sites;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using GazRouter.Common;
using System.Windows.Controls;
using GazRouter.Controls.Converters;

namespace GazRouter.Modes.ProcessMonitoring.Reports.Forms.CompUnitFailures
{

    [RegionMemberLifetime(KeepAlive = false)]
    public class CompUnitFailuresViewModel : FormViewModelBase
    {
        public override bool HasExcelExport { get; } = true;
        public override void ExportToExcel()
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = "xlsx",
                Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*",
                FilterIndex = 1,
                //DefaultFileName = Header
            };
            if (dialog.ShowDialog() == true)
            {
                var date = DateTime.Now;
                var excelReport = new ExcelReport("Остановы ГПА");
                excelReport.Write("Дата:").Write(date.Date).NewRow();
                excelReport.Write("Время:").Write(date.ToString("HH:mm")).NewRow();
                excelReport.Write("ФИО:").Write(UserProfile.Current.UserName).NewRow();
                excelReport.Write($"События за период с {Period.Begin:d} по {Period.End:d}").NewRow();
                excelReport.NewRow();
                excelReport.WriteHeader("КС", 160);
                excelReport.WriteHeader("КЦ", 100); // 60
                excelReport.WriteHeader("ГПА", 100); //60
                excelReport.WriteHeader("Дата", 110);
                excelReport.WriteHeader("Влияет на транспорт газа", 80);        
                excelReport.WriteHeader("Вид останова", 120);
                excelReport.WriteHeader("Результат устранения отказа", 120);
                excelReport.WriteHeader("Время вынужденного простоя", 120);
                excelReport.WriteHeader("Признак останова", 200); // 250
                excelReport.WriteHeader("Причина останова", 250); //120
                excelReport.WriteHeader("Внешнее проявление", 120);
                excelReport.WriteHeader("Описание причины", 120);
                excelReport.WriteHeader("Выполненные работы", 120);
                excelReport.WriteHeader("Связанные пуски", 120);
                excelReport.WriteHeader("Прикрепленные документы", 120);

                excelReport.NewRow();
                var stname = "";
                var shname = "";
                foreach (var station in Items)
                {
                    var stationItem = station as EntityItem;
                    //excelReport.WriteCell(stationItem.Dto.Name);
                    stname = stationItem.Dto.Name;
                    var newLine = false;
                    foreach (var shop in stationItem.Children)
                    {
                        //int level = 1;
                        var shopItem = shop as EntityItem;
                        //if (newLine) excelReport.NewRow(1, level);
                        //excelReport.WriteCell(shopItem.Dto.Name);
                        shname = shopItem.Dto.Name;
                        //newLine = false;
                        foreach (var unit in shopItem.Children)
                        {
                            //level = 2;
                            var unitItem = unit as EntityItem;
                            //if (newLine) excelReport.NewRow(1, level);
                            if (newLine) excelReport.NewRow();
                            excelReport.WriteCell(stname);
                            excelReport.WriteCell(shname);
                            excelReport.WriteCell(unitItem.Dto.Name + "\n" + unitItem.CompUnitTypeName);
                            newLine = false;
                            foreach (var failure in unitItem.Children)
                            {
                                //level = 3;
                                var failureItem = failure as FailureItem;
                                //if (newLine) excelReport.NewRow(1, level);
                                excelReport.WriteCell(failureItem.Dto.StateChangeDate.ToString("dd.MM.yyyy HH:mm"));
                                excelReport.WriteCell(failureItem.Dto.FailureDetails.IsCritical ? "Да" : "Нет");
                                excelReport.WriteCell(new CompUnitStopTypeToNameConverter().Convert(failureItem.Dto.StopType, typeof(string), null, null));

                                var cell = "";
                                if (failureItem.ToWork) cell = "Запущен в работу \n" + failureItem.Dto.FailureDetails.ToWorkDate?.ToString("dd.MM.yyyy HH:mm");
                                else if (failureItem.ToReserve) cell = "Переведен в резерв \n" + failureItem.Dto.FailureDetails.ToReserveDate?.ToString("dd.MM.yyyy HH:mm");
                                else if (failureItem.NotFinished) cell = "Ведутся востановительные работы";
                                excelReport.WriteCell(cell);

                                excelReport.WriteCell(new TimeSpanConverter().Convert(failureItem.DownTime, typeof(string), null, null));
                                excelReport.WriteCell(new CompUnitFailureFeatureToNameConverter().Convert(failureItem.Dto.FailureDetails.FailureFeature, typeof(string), null, null));
                                excelReport.WriteCell(new CompUnitFailureCauseToNameConverter().Convert(failureItem.Dto.FailureDetails.FailureCause, typeof(string), null, null));

                                excelReport.WriteCell(failureItem.Dto.FailureDetails.FailureExternalView);
                                excelReport.WriteCell(failureItem.Dto.FailureDetails.FailureCauseDescription);
                                excelReport.WriteCell(failureItem.Dto.FailureDetails.FailureWorkPerformed);

                                var startList = "";
                                foreach (var v in failureItem.Dto.FailureDetails.UnitStartList)
                                {
                                    startList += v.StateChangeDate.ToString("dd.MM.yyyy HH:mm") + "\n" + v.CompUnitName + "\n" + new CompUnitTypeToNameConverter().Convert(v.CompUnitTypeId, typeof(string), null, null) + "\n";
                                }
                                excelReport.WriteCell(startList);

                                var attachmentList = "";
                                foreach (var v in failureItem.Dto.FailureDetails.AttachmentList)
                                {
                                    //attachmentList += v.Description + "\n" + v.FileName + "\n" + v.DataLength + "\n";
                                    attachmentList += v.Description + "\n" + v.FileName + "\n" + DataLengthConverter.Convert(v.DataLength) + "\n";
                                }
                                excelReport.WriteCell(attachmentList);

                                newLine = true;
                            }
                            //newLine = true;
                        }
                        //newLine = true;
                    }

                    excelReport.NewRow();
                }

                excelReport.Move(0, 0, "Статистика отказов");
                excelReport.Write("Дата:").Write(date.Date).NewRow();
                excelReport.Write("Время:").Write(date.ToString("HH:mm")).NewRow();
                excelReport.Write("ФИО:").Write(UserProfile.Current.UserName).NewRow();
                excelReport.Write($"События за период с {Period.Begin:d} по {Period.End:d}").NewRow();
                excelReport.NewRow();
                excelReport.WriteHeader("Статистика отказов", 160);
                excelReport.WriteHeader("Наименование", 160);
                excelReport.WriteHeader("Кол-во, шт.", 60);
                excelReport.WriteHeader("Доля, %.", 60);
                //excelReport.NewRow();

                var siname = "";
                foreach (var si in Statistic)
                {
                    siname = si.Name;
                    //excelReport.WriteCell(si.Name);
                    //excelReport.WriteCell(string.Empty);
                    //excelReport.WriteCell(string.Empty);
                    //excelReport.WriteCell(string.Empty);
                    foreach (var v in si.Children)
                    {
                        //excelReport.NewRow(1, 1);
                        excelReport.NewRow();
                        excelReport.WriteCell(siname);
                        excelReport.WriteCell(v.Name);
                        excelReport.WriteCell(v.Count);
                        excelReport.WriteCell($"{v.Percent}%");
                    }
                    //excelReport.NewRow();
                }

                using (var stream = dialog.OpenFile())
                {
                    excelReport.Save(stream);
                }
            }
        }

        public override ReportSettings GetReportSettings()
        {
            return new ReportSettings
            {
                PeriodSelector = true,
                EmptySiteAllowed = true,
                SiteSelector = true,
            };
        }

        public override async void Refresh()
        {
            //if (Site == null) return;

            try
            {
                Behavior.TryLock();

                /*
                // Получить список ГПА 
                var stationTree = await new ObjectModelServiceProxy().GetCompStationTreeAsync(UserProfile.Current.Site.IsEnterprise ? null : (Guid?)UserProfile.Current.Site.Id);
                */
                // Получить дерево станций->цехов->ГПА для выбранного ЛПУ
                var stationTree = await new ObjectModelServiceProxy().GetCompStationTreeAsync(Site?.Id);
                // Получить список ЛПУ
                if (UserProfile.Current.Site.IsEnterprise)
                    stationTree.Sites = await new ObjectModelServiceProxy().GetSiteListAsync(
                        new GetSiteListParameterSet { EnterpriseId = UserProfile.Current.Site.Id });
                else
                {
                    stationTree.Sites = await new ObjectModelServiceProxy().GetSiteListAsync(
                        new GetSiteListParameterSet { SiteId = UserProfile.Current.Site.Id });
                }

                // Получить текущие состояния ГПА
                var failureList = await new ManualInputServiceProxy().GetCompUnitFailureListAsync(
                    new GetFailureListParameterSet
                    {
                        Begin = Period.Begin,
                        End = Period.End,
                        SiteId = UserProfile.Current.Site.IsEnterprise ? null : (Guid?)UserProfile.Current.Site.Id
                    });
                // Сформировать дерево
                Items = new List<GridItem>();

                foreach (var site in stationTree.Sites)
                {
                    var siteItem = new EntityItem(site);
                    foreach (var station in stationTree.CompStations.Where(s => s.ParentId == site.Id))
                    {
                        var stationItem = new EntityItem(station);
                        foreach (var shop in stationTree.CompShops.Where(cs => cs.ParentId == station.Id))
                        {
                            var shopItem = new EntityItem(shop);
                            foreach (var unit in stationTree.CompUnits.Where(u => u.ParentId == shop.Id))
                            {
                                var unitItem = new EntityItem(unit);
                                foreach (var failure in failureList.Where(f => f.CompUnitId == unit.Id))
                                {
                                    var failureItem = new FailureItem(failure);
                                    unitItem.Children.Add(failureItem);
                                }
                                if (unitItem.Children.Any())
                                    shopItem.Children.Add(unitItem);
                            }
                            if (shopItem.Children.Any())
                                stationItem.Children.Add(shopItem);
                        }
                        if (stationItem.Children.Any())
                            Items.Add(stationItem);
                    }
                    if (siteItem.Children.Any())
                        Items.Add(siteItem);
                }

                

                OnPropertyChanged(() => Items);


                // Формировать статистику
                Statistic = new List<StatisticItem>();

                var total = failureList.Count;


                // По виду останова
                var stopTypeGroup = new StatisticItem { Name = "По виду останова" };
                Statistic.Add(stopTypeGroup);
                foreach (var st in ClientCache.DictionaryRepository.CompUnitStopTypes)
                {
                    var count = failureList.Count(f => f.StopType == st.CompUnitStopType);
                    if (count > 0)
                    {
                        stopTypeGroup.Children.Add(new StatisticItem
                        {
                            Name = st.Name,
                            Count = count,
                            Total = total
                        });
                    }
                }


                // По типу ГПА
                var unitTypeGroup = new StatisticItem {Name = "По типу ГПА"};
                Statistic.Add(unitTypeGroup);
                var typeList =
                    stationTree.CompUnits.Where(u => failureList.Any(f => f.CompUnitId == u.Id))
                        .Select(u => u.CompUnitTypeId)
                        .Distinct();
                foreach (var utid in typeList)
                {
                    unitTypeGroup.Children.Add(new StatisticItem
                    {
                        Name = ClientCache.DictionaryRepository.CompUnitTypes.Single(t => t.Id == utid).Name,
                        Count =
                            failureList.Count(
                                f =>
                                    stationTree.CompUnits.Where(u => u.CompUnitTypeId == utid)
                                        .Any(u => u.Id == f.CompUnitId)),
                        Total = total
                    });
                }

                // По ЛПУ
                var siteGroup = new StatisticItem { Name = "По ЛПУ" };
                Statistic.Add(siteGroup);
                foreach (var site in stationTree.Sites)
                {
                    var stations = stationTree.CompStations.Where(s => s.ParentId == site.Id);
                    var shops = stationTree.CompShops.Where(s => stations.Any(x => x.Id == s.ParentId));
                    var units = stationTree.CompUnits.Where(u => shops.Any(x => x.Id == u.ParentId));
                    var count = failureList.Count(f => units.Any(u => u.Id == f.CompUnitId));
                    if (count > 0)
                    {
                        siteGroup.Children.Add(new StatisticItem
                        {
                            Name = site.Name,
                            Count = count,
                            Total = total
                        });
                    }
                }


                // По признаку отказа
                var featureGroup = new StatisticItem {Name = "По признаку отказа"};
                Statistic.Add(featureGroup);
                foreach (var ff in ClientCache.DictionaryRepository.CompUnitFailureFeatures)
                {
                    var count = failureList.Count(f => f.FailureDetails.FailureFeature == ff.CompUnitFailureFeature);
                    if (count > 0)
                    {
                        featureGroup.Children.Add(new StatisticItem
                        {
                            Name = ff.Description,
                            Count = count,
                            Total = total
                        });
                    }
                }

                // По причине отказа
                var causeGroup = new StatisticItem { Name = "По причине отказа" };
                Statistic.Add(causeGroup);
                foreach (var fc in ClientCache.DictionaryRepository.CompUnitFailureCauses)
                {
                    var count = failureList.Count(f => f.FailureDetails.FailureCause == fc.CompUnitFailureCause);
                    if (count > 0)
                    {
                        causeGroup.Children.Add(new StatisticItem
                        {
                            Name = fc.Name,
                            Count = count,
                            Total = total
                        });
                    }
                }
                
                
                OnPropertyChanged(() => Statistic);

            }
            finally
            {
                Behavior.TryUnlock();
            }
        }
        

        public List<GridItem> Items { get; set; }
        
        public List<StatisticItem> Statistic { get; set; }


    }


    public class FailureItem : GridItem
    {
        private readonly CompUnitStateDTO _stateDto;

        public FailureItem(CompUnitStateDTO stateDto)
        {
            _stateDto = stateDto;
        }

        public CompUnitStateDTO Dto
        {
            get { return _stateDto; }
        }

        /// <summary>
        /// Агрегат запущен после восствновительных работ
        /// </summary>
        public bool ToWork
        {
            get
            {
                return (Dto.FailureDetails.ToWorkDate.HasValue && !Dto.FailureDetails.ToReserveDate.HasValue) ||
                       (Dto.FailureDetails.ToWorkDate.HasValue && Dto.FailureDetails.ToReserveDate.HasValue &&
                        Dto.FailureDetails.ToWorkDate.Value < Dto.FailureDetails.ToReserveDate.Value);
            }
        }

        /// <summary>
        /// Агрегат переведен в резерв после восствновительных работ
        /// </summary>
        public bool ToReserve
        {
            get
            {
                return (Dto.FailureDetails.ToReserveDate.HasValue && !Dto.FailureDetails.ToWorkDate.HasValue) ||
                       (Dto.FailureDetails.ToReserveDate.HasValue && Dto.FailureDetails.ToWorkDate.HasValue &&
                        Dto.FailureDetails.ToReserveDate.Value < Dto.FailureDetails.ToWorkDate.Value);
            }
        }

        /// <summary>
        /// Идут восствновительные работы
        /// </summary>
        public bool NotFinished
        {
            get
            {
                return !Dto.FailureDetails.ToWorkDate.HasValue && !Dto.FailureDetails.ToReserveDate.HasValue;
            }
        }

        /// <summary>
        /// Время вынужденного простоя
        /// </summary>
        public TimeSpan? DownTime
        {
            get
            {
                if (ToWork) 
                    return Dto.FailureDetails.ToWorkDate - Dto.StateChangeDate;
                if (ToReserve)
                    return Dto.FailureDetails.ToReserveDate - Dto.StateChangeDate;

                return null;
            }
        }

        public override bool IsFailure
        {
            get { return true; }
        }
    }

    public class EntityItem : GridItem
    {
        private readonly CommonEntityDTO _entityDto;

        public CommonEntityDTO Dto
        {
            get { return _entityDto; }
        }

        public EntityItem(CommonEntityDTO entityDto)
        {
            _entityDto = entityDto;
        }

        /// <summary>
        /// Тип ГПА
        /// </summary>
        public string CompUnitTypeName
        {
            get
            {
                if (_entityDto.EntityType != EntityType.CompUnit) return "";
                var unit = (CompUnitDTO)_entityDto;
                return ClientCache.DictionaryRepository.CompUnitTypes.Single(t => t.Id == unit.CompUnitTypeId).Name;
            }
        }

        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();
    }
    

    public class GridItem
    {
        public GridItem()
        {
            Children = new List<GridItem>();
        }
        
        [Display(AutoGenerateField = false)]
        public List<GridItem> Children { get; set; }

        public virtual bool IsFailure 
        {
            get { return false; }
        }
        
    }


    public class StatisticItem
    {
        public StatisticItem()
        {
            Children = new List<StatisticItem>();
        }

        public string Name { get; set; }

        public int? Count { get; set; }

        public int? Total { get; set; }

        public double? Percent
        {
            get
            {
                return Count.HasValue && Total.HasValue
                    ? (double?) Math.Round(((double) Count.Value)/Total.Value*100, 1)
                    : null;
            }
        }

        [Display(AutoGenerateField = false)]
        public List<StatisticItem> Children { get; set; }
    }
}