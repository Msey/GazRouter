using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using ExchangeTest.Properties;
using GazRouter.DAL.Core;
using GazRouter.DAL.DataExchange.ExchangeLog;
using GazRouter.DAL.DataExchange.ExchangeTask;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.CompUnits;
using GazRouter.DAL.ObjectModel.Entities;
using GazRouter.DAL.SeriesData.Series;
using GazRouter.DAL.SysEvents;
using GazRouter.DTO.DataExchange.ExchangeLog;
using GazRouter.DTO.DataExchange.ExchangeTask;
using GazRouter.DTO.DataExchange.Transformation;
using GazRouter.DTO.Dictionaries.CompUnitSealingTypes;
using GazRouter.DTO.Dictionaries.ExchangeTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.SeriesData.Series;
using GazRouter.DTO.SysEvents;
using GazRouter.Service.Exchange.Lib;
using GazRouter.Service.Exchange.Lib.Cryptography;
using GazRouter.Service.Exchange.Lib.Export;
using GazRouter.Service.Exchange.Lib.Import;
using GazRouter.Service.Exchange.Lib.Import.Astra;
using GazRouter.Service.Exchange.Lib.Run;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;
using ExchangeStatus = GazRouter.DTO.DataExchange.ExchangeTask.ExchangeStatus;

namespace ExchangeTest
{
    [TestClass]
    public class ExchangeObjectTests : TransactionTestsBase
    {
        //[TestMethod ,TestCategory(Stable)]
        //public void SerializationTest()
        //{
        //    var enterprises = new GetEnterpriseListQuery(Context).Execute(null);
        //    var ent = enterprises.FirstOrDefault(en => en.IsGr.HasValue && (bool)en.IsGr);
        //    if (ent != null)
        //    {
        //        var eo = new TypicalExchangeObjectExporter(Context, ent).Build(DateTime.Now);
        //        XmlHelper.Save(eo, "eo.xml");
        //        XmlHelper.Get<ExchangeObject<TypicalExchangeData>>("eo.xml");
        //    }

        //}


        [TestMethod]
        [TestCategory(UnStable)]
        public void ExchangeGuidTest()
        {
            var cfgs = new GetExchangeTaskListQuery(Context).Execute(new GetExchangeTaskListParameterSet());
            var cfg = cfgs.FirstOrDefault();
            if (cfg == null) return;
            new SpecificExchangeObjectExporter(Context, cfg).Export(DateTime.Now);
            var sdto = new GetSeriesQuery(Context).Execute(new GetSeriesParameterSet
            {
                PeriodType = PeriodType.Twohours
            });
            if (sdto != null)
            {
                var eo = new SpecificExchangeObjectExporter(Context, cfg).Export(sdto.KeyDate);
                var overs = new XmlAttributeOverrides();
                var attr = new XmlElementAttribute("Id", typeof(string));
                var attrs = new XmlAttributes {XmlElements = {attr}};
                overs.Add(typeof(SiteDTO), "Item", attrs);
                XmlHelper.Save(eo, "eo.xml");
                //XmlHelper.Get<ExchangeObject<SpecificExchangeData>>("eo.xml");
            }
            cfg.FileNameMask = "gdn_gty_snc_<yyyyMMddHH#1d>.txt";
        }

        [TestMethod]
        [TestCategory(Stable)]
        public void GetRawDateTimeFromFileNameTest()
        {
            var fileName = "gdn_gty_snc_2016112506.txt";
            var mask = "gdn_gty_snc_<yyyyMMddHH>.txt";
            var result = ExchangeHelper.GetRawDateTimeFromFileName(mask, fileName);
            Assert.AreEqual(result, "2016112506");
            fileName = "GTS.16";
            mask = "GTS.<HH>";
            result = ExchangeHelper.GetRawDateTimeFromFileName(mask, fileName);
            Assert.AreEqual(result, "16");
            fileName = "gdn_gty_snc_2016112506.txt";
            mask = "gdn_gty_snc_<yyyyMMddHH#1d>.txt";
            result = ExchangeHelper.GetRawDateTimeFromFileName(mask, fileName);
            Assert.AreEqual(result, "2016112506");
            fileName = "gdn_gty_snc_2016112506.txt";
            mask = "gdn_gty_snc_yyyyMMddHH#1d.txt";
            string offset, dateFormat, end, start;
            Assert.IsFalse(ExchangeHelper.TryParseFileNameMask(mask, out start, out end, out dateFormat, out offset));
        }


        [TestMethod]
        [TestCategory(Stable)]
        public void GetTimestampTest()
        {
            var fileName = "gdn_gty_snc_2016112506.txt";
            var mask = "gdn_gty_snc_<yyyyMMddHH#0>.txt";
            Exception e;
            var result1 =
                new SpecificExchangeImporter().GetTimeStamp(new ExchangeTaskDTO {FileNameMask = mask}, fileName, out e);
            Assert.AreEqual(result1, new DateTime(2016, 11, 25, 6, 0, 0));
            mask = "gdn_gty_snc_<yyyyMMddHH#-1>.txt";
            result1 = new SpecificExchangeImporter().GetTimeStamp(new ExchangeTaskDTO {FileNameMask = mask}, fileName,
                out e);
            Assert.AreEqual(result1, new DateTime(2016, 11, 24, 6, 0, 0));
            mask = "gdn_gty_snc_<yyyyMMddHH#-1.12:0>.txt";
            result1 = new SpecificExchangeImporter().GetTimeStamp(new ExchangeTaskDTO {FileNameMask = mask}, fileName,
                out e);
            Assert.AreEqual(result1, new DateTime(2016, 11, 23, 18, 0, 0));
        }

        [TestMethod]
        [TestCategory(Stable)]
        public void ImporterIsValidTest()
        {
            var fileName = "gdn_gty_snc_2016112506.txt";
            var mask = "gdn_gty_snc_<yyyyMMddHH#0>.txt";
            var isValid = new SpecificExchangeImporter().IsValid(new ExchangeTaskDTO {FileNameMask = mask}, fileName);
            Assert.IsTrue(isValid);
            mask = "gdn_gty_snc_<yyyyMMddHH#1.11:0>.txt";
            isValid = new SpecificExchangeImporter().IsValid(new ExchangeTaskDTO {FileNameMask = mask}, fileName);
            Assert.IsTrue(isValid);
            mask = "gdn_gty_snc_<yyyyMMddHH#1.11:0>.txt";
            isValid = new SpecificExchangeImporter().IsValid(new ExchangeTaskDTO {FileNameMask = mask},
                "gdn_gty_snc_2016112506.tx");
            Assert.IsFalse(isValid);
        }

        [TestMethod]
        [TestCategory(Stable)]
        public void MiscTest()
        {
            int count;
            count = Enumerable.Range(0, 500).GroupBy(i => i / 1000).Count();
            Assert.IsTrue(count == 1);
            count = Enumerable.Range(0, 999).GroupBy(i => i / 1000).Count();
            Assert.IsTrue(count == 1);
            count = Enumerable.Range(0, 1001).GroupBy(i => i / 1000).Count();
            Assert.IsTrue(count == 2);
            var ar = new[] {"1", "2", "3", "4", "5"};
            var result = ar.Zip(Enumerable.Range(0, ar.Count()), (id, i) => new Tuple<string, int>(id, i))
                .GroupBy(t => t.Item2 / 2).Select(g =>
                {
                    if (g.Key == 0) return "12";
                    if (g.Key == 1) return "34";
                    return "5";
                }).Aggregate((s1, s2) => s1 + s2);
            Assert.IsTrue(result == "12345");
        }


        [TestMethod]
        [TestCategory(UnStable)]
        public void ImportExchangeTest()
        {
            var check1 = new CheckEntityQuery(Context).Execute(Guid.NewGuid());
            Assert.IsFalse(check1);
            var cs = new GetCompStationListQuery(Context).Execute(null).First();
            var check2 = new CheckEntityQuery(Context).Execute(cs.Id);
            Assert.IsTrue(check2);
            var newId = Guid.NewGuid();
            var id = new AddCompStationCommand(Context).Execute(new AddCompStationParameterSet
            {
                Name = cs.Name,
                ParentId = cs.ParentId,
                RegionId = cs.RegionId,
                SortOrder = cs.SortOrder,
                Id = newId
            });
            Assert.AreEqual(id, newId);
            var csh = new GetCompShopListQuery(Context).Execute(null).First();
            newId = Guid.NewGuid();
            id = new AddCompShopCommand(Context).Execute(new AddCompShopParameterSet
            {
                Name = csh.Name,
                ParentId = csh.ParentId,
                SortOrder = csh.SortOrder,
                KmOfConn = csh.KmOfConn ?? 0,
                PipelineId = csh.PipelineId,
                PipingVolume = csh.PipingVolume ?? 0,
                Id = newId,
                EngineClassId = csh.EngineClass
            });
            Assert.AreEqual(id, newId);

            var cu = new GetCompUnitListQuery(Context).Execute(null).First();
            newId = Guid.NewGuid();
            id = new AddCompUnitCommand(Context).Execute(new AddCompUnitParameterSet
            {
                Name = cu.Name,
                ParentId = cu.ParentId,
                SortOrder = cu.SortOrder,
                CompUnitTypeId = cu.CompUnitTypeId,
                DryMotoringConsumption = cu.DryMotoringConsumption,
                HasRecoveryBoiler = cu.HasRecoveryBoiler,
                InjectionProfileVolume = cu.InjectionProfileVolume,
                SuperchargerTypeId = cu.SuperchargerTypeId,
                TurbineStarterConsumption = cu.TurbineStarterConsumption,
                Id = newId,
                SealingType = cu.SealingType.HasValue ? cu.SealingType.Value : CompUnitSealingType.Dry
            });
            Assert.AreEqual(id, newId);
        }

        [TestMethod]
        [TestCategory(UnStable)]
        public void CrudExchangeLog()
        {
            var sdto = new GetSeriesQuery(Context).Execute(
                new GetSeriesParameterSet
                {
                    PeriodType = PeriodType.Twohours
                });

            var taskId = new GetExchangeTaskListQuery(Context).Execute(
                    new GetExchangeTaskListParameterSet
                    {
                        ExchangeTypeId = ExchangeType.Import
                    })
                .Select(t => t.Id).FirstOrDefault();

            var dt = new AddExchangeLogCommand(Context).Execute(
                new AddEditExchangeLogParameterSet
                {
                    SeriesId = sdto.Id,
                    ExchangeTaskId = taskId,
                    Content = "ok"
                });
            Assert.IsNotNull(dt);
            var result =
                new GetExchangeLogQuery(Context).Execute(
                    new GetExchangeLogParameterSet {ExchangeTaskId = taskId});
            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        [TestCategory(Stable)]
        public void SpecificImportTest()
        {
            const string text = @"4001;1100;2x2
4001;2;51.4
4001;4;66.3
4001;22;24.0
4001;23;50.0
4001;24;39.0
4001;27;21.0
4001;25;14.0
4003;1100;4x1
4003;2;50.9
4003;4;66.0
4003;22;22.0
4003;23;47.0
4003;24;36.0
4005;1100;0x0
4005;2;53.0
4005;4;64.6
4005;22;24.0
4005;23;0.0
4005;24;36.0
4002;4;65.4
4002;59;3574.000
4004;4;64.6
4004;59;2751.000
4006;4;64.6
4006;59;0.000
4007;4;33.4
4007;23;15.0
4007;25;15.0
4007;26;-11.7";

            var task = new ExchangeTaskDTO
            {
                Transformation = Resources.SAR_Import_GTSamara_v1,
                Id = 178,
                FileNameMask = "SM<ddMMyyHH>.UG",
                PeriodTypeId = PeriodType.Twohours
            };

            var importParams = new ImportParams {Text = text, Task = task, TimeStamp = DateTime.Today};
            var eo = new SpecificExchangeImporter().Import<ExtData>(importParams);
            int seriesId;
            eo.Sync(Context, task, out seriesId);
        }

        [TestMethod]
        [TestCategory(Stable)]
        public void AstraCsImportTest()
        {
            const string text = @"
   
    Ямбургс 7 Ур,2221,543,89.000,2017072500,0
    Ныда    1 Е1,4303,882,90.000,2017072500,0
    Ныда    2 Е2,4343,882,91.000,2017072500,0
    Ныда    3 Пр,4383,873,92.000,2017072500,0
    Ныда    4 Т1,2103,447,93.000,2017072500,0
    Ныда    5 Т2,2143,1360,94.000,2017072500,0
    Ныда    6 Пв,2183,737,95.000,2017072500,0
    Ныда    7 Ур,2223,740,96.000,2017072500,0
    П.Хетти 1 Уж,4705,0,97.000,2017072500,0
    П.Хетти 2 Ц1,4745,1371,98.000,2017072500,0
    П.Хетти 3 Ц2,4785,1373,99.000,2017072500,0
    П.Хетти 4 Е1,4825,1346,100.000,2017072500,0
    П.Хетти 5 Е2,4405,0,101.000,2017072500,0
    П.Хетти 6 Пр,4445,1336,102.000,2017072500,0
    П.Хетти 7 Т1,2105,1373,103.000,2017072500,0
    П.Хетти 8 Т2,2145,958,104.000,2017072500,0
    П.Хетти 9 Пв,2185,922,105.000,2017072500,0
    П.Хетт 10 Ур,2225,492,106.000,2017072500,0
    Ягельна 1 Уж,4707,0,107.000,2017072500,0
    Ягельна 2 Ц1,4747,0,108.000,2017072500,0
    Ягельна 3 Ц2,4787,0,109.000,2017072500,0
    Ягельна 4 Е1,4827,449,110.000,2017072500,0
    Ягельна 5 Е2,4407,870,111.000,2017072500,0
    Ягельна 6 Пр,4447,1305,112.000,2017072500,0
    Ягельна 7 Т1,2107,1302,113.000,2017072500,0
    Ягельна 8 Т2,2147,1293,114.000,2017072500,0
    Ягельна 9 Пв,2187,570,115.000,2017072500,0
    Ягельн 10 Ур,2227,899,116.000,2017072500,0
    Приозер 1 Уж,4709,678,117.000,2017072500,0
    Приозер 2 Ц1,4749,956,118.000,2017072500,0
    Приозер 3 Ц2,4789,0,119.000,2017072500,0
    Приозер 4 Е1,4829,914,120.000,2017072500,0
    Приозер 5 Е2,4409,479,121.000,2017072500,0
    Приозер 6 Пр,4449,1412,122.000,2017072500,0
    Приозер 7 Т1,2109,1423,123.000,2017072500,0
    Приозер 8 Т2,2149,947,124.000,2017072500,0
    Приозер 9 Пв,2189,940,125.000,2017072500,0
    Приозе 10 Ур,2229,482,126.000,2017072500,0
    Сосновс 1 Уж,4711,0,127.000,2017072500,0
    Сосновс 2 Ц1,4751,0,128.000,2017072500,0
    Сосновс 3 Ц2,4791,0,129.000,2017072500,0
    Сосновс 4 Е1,4831,1280,130.000,2017072500,0
    Сосновс 5 Е2,4411,891,131.000,2017072500,0
    Сосновс 6 Пр,4451,1309,132.000,2017072500,0
    Сосновс 7 Т1,2111,885,133.000,2017072500,0
    Сосновс 8 Т2,2151,883,134.000,2017072500,0
    Сосновс 9 Пв,2191,894,135.000,2017072500,0
    Соснов 10 Ур,2231,1307,136.000,2017072500,0
    В.Казым 1 Уж,4713,686,137.000,2017072500,0
    В.Казым 2 Ц1,4753,1336,138.000,2017072500,0
    В.Казым 3 Ц2,4793,0,139.000,2017072500,0
    В.Казым 4 Е1,4833,1376,140.000,2017072500,0
    В.Казым 5 Е2,4413,1380,141.000,2017072500,0
    В.Казым 6 Пр,4453,458,142.000,2017072500,0
    В.Казым 7 Т1,2113,0,143.000,2017072500,0
    В.Казым 8 Т2,2153,476,144.000,2017072500,0
    В.Казым 9 Пв,2193,0,145.000,2017072500,0
    В.Казы 10 Ур,2233,0,146.000,2017072500,0
    Бобровс 1 Уж,4715,0,147.000,2017072500,0
    Бобровс 2 Ц1,4755,1288,148.000,2017072500,0
    Бобровс 3 Ц2,4795,1297,149.000,2017072500,0
    Бобровс 4 Е1,4835,0,150.000,2017072500,0
    Бобровс 5 Е2,4415,1369,151.000,2017072500,0
    Бобровс 6 Пр,4455,1369,152.000,2017072500,0
    Бобровс 7 Т1,2115,0,153.000,2017072500,0
    Бобровс 8 Т2,2155,1361,154.000,2017072500,0
    Бобровс 9 Пв,2195,1299,155.000,2017072500,0
    Бобров 10 Ур,2235,1291,156.000,2017072500,0
    Октябрь 1 Уж,4717,0,157.000,2017072500,0
    Октябрь 2 Ц1,4757,0,158.000,2017072500,0
    Октябрь 3 Ц2,4797,1483,159.000,2017072500,0
";

            var task = new ExchangeTaskDTO
            {
                Transformation = Resources.ASTRA_Import_CSHOP,
                Id = 10,
                FileNameMask = "ASTRA_IMPORT_CS<ddmmyyyyHHmi>.csv",
                PeriodTypeId = PeriodType.Twohours
            };

            var importParams = new ImportParams {Text = text, Task = task};
            var eo = new AstraExchangeImporter().Import<ExtData>(importParams);
            int seriesId;
            eo.Sync(Context, task, out seriesId);
        }

        [TestMethod]
        [TestCategory(Stable)]
        public void AstraPipeImportTest()
        {
            const string text = @"
   
    Надым-29.6км П1,1543.6,1.00,4,0.00,17.00,2017072500,0
МСП в Т1 и Пв,53.1,2.00,0,0.00,0.00,2017072500,0
МСП Лупинг,27.6,3.00,0,0.00,0.00,2017072500,0
30км-44км     П1,740.0,4.00,4,29.60,45.00,2017072500,0
44км-45км 1   П1,35.8,5.00,4,45.00,46.00,2017072500,0
44км-45км 2   П1,43.6,6.00,4,45.00,46.00,2017072500,0
45км-75км     П1,1514.6,7.00,4,46.00,75.00,2017072500,0
75км-109км    П1,1675.1,8.00,4,75.00,109.00,2017072500,0
109-112км 1   П1,108.8,9.00,4,109.00,112.20,2017072500,0
109-112км 2   П1,108.8,10.00,4,109.00,112.20,2017072500,0
112км-Л.Юган  П1,907.9,11.00,4,112.20,129.00,2017072500,0
Надым-42км    П2,2189.2,12.00,5,0.00,44.00,2017072500,0
42км-45км     П2,104.0,13.00,5,44.00,46.00,2017072500,0
45км-75км     П2,1517.1,14.00,5,46.00,75.00,2017072500,0
";

            var task = new ExchangeTaskDTO
            {
                Transformation = Resources.ASTRA_Import_PIPE,
                Id = 10,
                FileNameMask = "ASTRA_IMPORT_PIPE<ddmmyyyyHHmi>.csv",
                PeriodTypeId = PeriodType.Twohours
            };

            var importParams = new ImportParams {Text = text, Task = task, TimeStamp = DateTime.Today};
            var eo = new AstraExchangeImporter().Import<AstraPipeData>(importParams);
            int seriesId;
            eo.Sync(Context, task, out seriesId);
        }


        [TestMethod]
        [TestCategory(Stable)]
        public void CryptDecryptTest()
        {
            var eo = new ExchangeObject<TypicalExchangeData>();

            var original = XmlHelper.GetBytes(eo);
            var encrypted = Cryptoghraphy.Encrypt(original);
            var decrypted = Cryptoghraphy.Decrypt(encrypted);
            var eo1 = XmlHelper.Get<ExchangeObject<TypicalExchangeData>>(decrypted);
        }


        [TestMethod]
        [TestCategory(Stable)]
        public void ExchangeScheduleTest()
        {
            Action<ExecutionContext, SeriesDTO, ExchangeTaskDTO> action = (context, dto, arg3) => { };
            var tasks = new GetExchangeTaskListQuery(Context)
                .Execute(new GetExchangeTaskListParameterSet())
                .Where(t => t.ExchangeTypeId == ExchangeType.Export)
                .ToList();

            var typical = tasks.Where(t => t.EnterpriseId.HasValue).Where(t => t.ExchangeTypeId == ExchangeType.Export)
                .ToList();
            Assert.IsTrue(typical.Count() == 1);
            tasks = tasks.Except(typical).Union(typical.SelectMany(
                t =>
                {
                    t.Lag = t.Lag == 0 ? 10 : t.Lag;
                    var t1 = t.Clone();
                    t.PeriodTypeId = PeriodType.Twohours;
                    t1.PeriodTypeId = PeriodType.Day;
                    return new List<ExchangeTaskDTO> {t, t1};
                })).Where(t => t.PeriodTypeId == PeriodType.Twohours || t.PeriodTypeId == PeriodType.Day).ToList();
            typical = tasks.Where(t => t.EnterpriseId.HasValue).Where(t => t.ExchangeTypeId == ExchangeType.Export)
                .ToList();
            Assert.IsTrue(typical.Count() == 2);
            Assert.IsTrue(typical.Any(t => t.PeriodTypeId == PeriodType.Twohours));
            Assert.IsTrue(typical.Any(t => t.PeriodTypeId == PeriodType.Day));


            var daylyTasks = tasks.Where(t => t.PeriodTypeId == PeriodType.Day);
            Assert.IsTrue(daylyTasks.Any());
            var hoursTasks = tasks.Where(t => t.PeriodTypeId == PeriodType.Twohours);
            Assert.IsTrue(hoursTasks.Any());
            var events = new List<SysEventDTO>
            {
                new SysEventDTO {PeriodTypeId = PeriodType.Twohours, SeriesId = 1},
                new SysEventDTO {PeriodTypeId = PeriodType.Twohours, SeriesId = 1},
                new SysEventDTO {PeriodTypeId = PeriodType.Twohours, SeriesId = 1},
                new SysEventDTO {PeriodTypeId = PeriodType.Twohours, SeriesId = 1},
                new SysEventDTO {PeriodTypeId = PeriodType.Twohours, SeriesId = 1},
                new SysEventDTO {PeriodTypeId = PeriodType.Twohours, SeriesId = 1},
                new SysEventDTO {PeriodTypeId = PeriodType.Day, SeriesId = 1},
                new SysEventDTO {PeriodTypeId = PeriodType.Day, SeriesId = 1},
                new SysEventDTO {PeriodTypeId = PeriodType.Day, SeriesId = 1},
                new SysEventDTO {PeriodTypeId = PeriodType.Day, SeriesId = 1}
            };
            var now = new DateTime(2011, 11, 1, 12, 0, 0);
            tasks.ForEach(t => t.ExchangeStatus = ExchangeStatus.Off);
            var result = ProcessTasks(tasks, action, now, events);
            Assert.IsTrue(!result.Any());
            tasks.ForEach(t => t.ExchangeStatus = ExchangeStatus.Scheduled);
            tasks.ForEach(t => t.Lag = 10);
            tasks.Take(2).ToList().ForEach(t => t.Lag = 5);
            now = new DateTime(2011, 11, 1, 12, 0, 0);
            result = ProcessTasks(tasks, action, now, events);
            Assert.IsTrue(!result.Any());
            now = new DateTime(2011, 11, 1, 12, 10, 0);
            result = ProcessTasks(tasks, action, now, events);
            now = new DateTime(2011, 11, 1, 0, 0, 0);
            result = ProcessTasks(tasks, action, now, events);
            Assert.IsTrue(!result.Any());
            now = new DateTime(2011, 11, 1, 0, 10, 0);
            result = ProcessTasks(tasks, action, now, events);
            var count1 = result.Count;

            now = new DateTime(2011, 11, 1, 23, 55, 0);
            result = ProcessTasks(tasks, action, now, events);
            Assert.IsFalse(result.Any());


            now = new DateTime(2011, 11, 1, 0, 5, 0);
            result = ProcessTasks(tasks, action, now, events);
            var count2 = result.Count;
            Assert.AreEqual(2, count2);
            //Assert.AreEqual(tasks.Count, count1 + count2);
            tasks.ForEach(t => t.ExchangeStatus = ExchangeStatus.Event);
            result = ProcessTasks(tasks, action, now, events);
            Assert.IsTrue(result.Any());
            Assert.AreEqual(result.Count, hoursTasks.Count() * 6 + daylyTasks.Count() * 4);

            var createDate = DateTime.Today.AddDays(-1);
            events = new GetSysEventListQuery(Context).Execute(new GetSysEventListParameters
            {
                CreateDate = createDate,
                EventTypeId = SysEventType.END_CALCULATION_AFTER_LOAD_DATA,
                EventStatusId = SysEventStatus.Waiting
            });
            if (events.Any())
            {
                now = new DateTime(2011, 11, 1, 1, 5, 0);
                new ExchangeTaskScheduler(tasks, action, Context).Tick(now);
                events = new GetSysEventListQuery(Context).Execute(new GetSysEventListParameters
                {
                    CreateDate = createDate,
                    EventTypeId = SysEventType.END_CALCULATION_AFTER_LOAD_DATA,
                    EventStatusId = SysEventStatus.Waiting
                });
                Assert.IsTrue(!events.Any());
            }
            var tsk = tasks.First();
            tsk.Lag = 60;
            tsk.PeriodTypeId = PeriodType.Twohours;
            tsk.ExchangeStatus = ExchangeStatus.Scheduled;
            now = new DateTime(2017, 11, 1, 0, 0, 0);
            var result1 = Enumerable.Range(0, 50).Select(m => now + TimeSpan.FromMinutes(2*m))
                .SelectMany(now1 => ProcessTasks(new List<ExchangeTaskDTO> {tsk}, action, now1, events, timerInterval: 2).Select(a => new {now1, a}))
                .ToList();
            Assert.AreEqual(2, result1.Count);
        }

        private List<Action<ExecutionContext>> ProcessTasks(List<ExchangeTaskDTO> tasks,
            Action<ExecutionContext, SeriesDTO, ExchangeTaskDTO> action, DateTime now, List<SysEventDTO> events, int timerInterval = 10)
        {
            var gr = tasks.GroupBy(t => t.ExchangeStatus);
            var eventTasks = gr.Where(g => g.Key == ExchangeStatus.Event).SelectMany(g => g).ToList();
            IEnumerable<Action<ExecutionContext>> result1 = new List<Action<ExecutionContext>>();
            if (eventTasks.Any())
                result1 =
                    from ev in events
                    from t in tasks
                    where t.PeriodTypeId == ev.PeriodTypeId
                    let seriesDTO = ExchangeHelper.GetSerie(Context, ev.SeriesId)
                    select new Action<ExecutionContext>(ctx => action(ctx, seriesDTO, t));
            var result2 = gr.Where(g => g.Key == ExchangeStatus.Scheduled)
                .SelectMany(g => g)
                .GroupBy(t => new Tuple<PeriodType, int>(t.PeriodTypeId, t.Lag))
                .Where(g =>
                {
                    var period = g.Key.Item1.ToTimeSpan();
                    var lag = TimeSpan.FromMinutes(g.Key.Item2);
                    var rest = (now - lag).Ticks % period.Ticks;
                    var restMinites = TimeSpan.FromTicks(rest);
                    var delta = TimeSpan.FromMinutes(timerInterval).Add(-TimeSpan.FromSeconds(1));
                    return (now - lag).Ticks % period.Ticks <= delta.Ticks;

                })
                .SelectMany(g =>
                {
                    var endDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
                    var seriesDTO = ExchangeHelper.GetSerie(Context, dt: endDate, periodTypeId: g.Key.Item1);
                    return g.Select(t => new {Task = t, SeriesDTO = seriesDTO});
                })
                .Select(t => { return new Action<ExecutionContext>(ctx => action(ctx, t.SeriesDTO, t.Task)); });
            return result1.Union(result2).ToList();
        }

        [TestMethod]
        [TestCategory(Stable)]
        public void SpecificExchangeTest()
        {
            var cfgs = new GetExchangeTaskListQuery(Context).Execute(new GetExchangeTaskListParameterSet());
            var cfg = cfgs.FirstOrDefault();
            if (cfg == null) return;
            new SpecificExchangeObjectExporter(Context, cfg).Export(DateTime.Now);
            var sdto = new GetSeriesQuery(Context).Execute(new GetSeriesParameterSet
            {
                PeriodType = PeriodType.Twohours
            });

            if (sdto != null)
                new SpecificExchangeObjectExporter(Context, cfg).Export(sdto.KeyDate);

            cfg.IsSql = true;
            cfg.SqlProcedureName = "rd.P_EXCHANGE_TASK_DATA.GET_EXCHANGE_TASK_DATA_F";

            if (sdto != null)
            {
                var eo = new SpecificExchangeObjectExporter(Context, cfg).Export(sdto.KeyDate);
                var xml = eo.ToXml();
                Assert.IsFalse(xml.Contains("&lt;"));
            }
        }

        [TestMethod]
        [TestCategory(Stable)]
        public void PeriodConditionTest()
        {
            var delta = TimeSpan.FromMinutes(30).Ticks + TimeSpan.FromSeconds(1).Ticks;
            const PeriodType periodType = PeriodType.Twohours;
            for (var i = 0; i < 100; i++)
                Assert.IsTrue(i % 4 != 0 || TimeSpan.FromTicks(i * delta % periodType.ToTimeSpan().Ticks) <=
                              TimeSpan.FromMinutes(3));
        }
    }
}