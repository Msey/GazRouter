using System;
using System.Linq;
using DataProviders;
using DataProviders.ObjectModel;
using DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;

namespace GR_ExcelFunc.Model
{
    public class ExcelData : BaseSync
    {
        private ObjectModelServiceProxy _OmServiceClient;
        private SeriesDataServiceProxy _SeriesDataClient;
        //var entity = ExecuteSync(new ObjectModelServiceProxy().GetEntityByIdAsync, id.Convert());

        public ExcelData()
        {
            _OmServiceClient = new ObjectModelServiceProxy();
            _SeriesDataClient = new SeriesDataServiceProxy();
        }

        public object[,] SiteList()
        {
            var siteList = ExecuteSync(_OmServiceClient.GetSiteListAsync, new GazRouter.DTO.ObjectModel.Sites.GetSiteListParameterSet { });
           
            object[,] res2 = new object[siteList.Count() + 1, 3];
            res2[0, 0] = "Идентификатор";
            res2[0, 1] = "Наименование";
            res2[0, 2] = "Полное наименование";
            var i = 1;
            foreach (var ss in siteList)
            {
                res2[i, 0] = GetOracleGuid(ss.Id);
                res2[i, 1] = ss.Name;
                res2[i, 2] = ss.Path;
                i++;
            }
            return res2;
        }

        public object[,] CompShopList()
        {
            var csList = ExecuteSync(_OmServiceClient.GetCompShopListAsync, new GetCompShopListParameterSet());
           
            object[,] res2 = new object[csList.Count() + 1, 4];
            res2[0, 0] = "Идентификатор";
            res2[0, 1] = "Наименование";
            res2[0, 2] = "Краткое наименование";
            res2[0, 3] = "Полное наименование";
            var i = 1;
            foreach (var ss in csList)
            {
                res2[i, 0] = GetOracleGuid(ss.Id);
                res2[i, 1] = ss.Name;
                res2[i, 2] = ss.ShortPath;
                res2[i, 3] = ss.Path;
                i++;
            }
            return res2;
        }

        public object[,] CompStationList()
        {
            var csList = ExecuteSync(_OmServiceClient.GetCompStationListAsync, new GetCompStationListParameterSet());

            object[,] res2 = new object[csList.Count() + 1, 3];
            res2[0, 0] = "Идентификатор";
            res2[0, 1] = "Наименование";
            res2[0, 2] = "Полное наименование";
            var i = 1;
            foreach (var ss in csList)
            {
                res2[i, 0] = GetOracleGuid(ss.Id);
                res2[i, 1] = ss.Name;
                res2[i, 2] = ss.Path;
                i++;
            }
            return res2;
        }

        public object[,] CompUnitList()
        {
            try
            {
                var csList = ExecuteSync(_OmServiceClient.GetCompUnitListAsync, new GetCompUnitListParameterSet());
                object[,] res2 = new object[csList.Count() + 1, 4];

                res2[0, 0] = "Идентификатор";
                res2[0, 1] = "Наименование";
                res2[0, 2] = "Краткое наименование";
                res2[0, 3] = "Полное наименование";
                var i = 1;
                foreach (var ss in csList)
                {
                    res2[i, 0] = GetOracleGuid(ss.Id);
                    res2[i, 1] = ss.Name;
                    res2[i, 2] = ss.ShortPath;
                    res2[i, 3] = ss.Path;
                    i++;
                }
                return res2;
            }
            catch (Exception err)
            {
                return new object[,] {{err.Data.ToString()}, {err.Message}};
            }
        }

        public object[,] DistrStationList()
        {
            try
            {
                var csList = ExecuteSync(_OmServiceClient.GetDistrStationTreeAsync,  new GetDistrStationListParameterSet());
                var maxCountOutlets = csList.DistrStationOutlets.GroupBy(o => o.ParentId).Max(e => e.Count());
                var columnCount =  (maxCountOutlets*2) + 2;
                object[,] res2 = new object[csList.DistrStations.Count() + 1, columnCount];
                res2[0, 0] = "Ид ГРС ";
                res2[0, 1] = "Наименование ГРС";
                var i = 1;
                
                foreach (var ss in csList.DistrStations)
                {
                    res2[i, 0] = GetOracleGuid(ss.Id);
                    res2[i, 1] = ss.Name;
                    var j = 2;
                    var outs = csList.DistrStationOutlets.Where(o => o.ParentId == ss.Id).OrderBy(o => o.Name);
                    foreach (var ou in outs)
                    {
                        res2[i, j] = GetOracleGuid(ou.Id);
                        res2[i, j + 1] = ou.Name;
                        j = j + 2;
                    }
                    i++;
                }
                var k = 1;
                for (int j = 2; j < columnCount; j++)
                {
                    if( (j % 2) == 0)
                        res2[0, j] = string.Format("Ид выхода № {0}", k);
                    else
                    {
                        res2[0, j] = string.Format("Наименование выхода № {0}", k);
                        k++;
                    }
                }
                return res2;
            }
            catch (Exception err)
            {
                return new object[,] {{err.Data.ToString()}, {err.Message}};
            }
        }

        private string GetOracleGuid(Guid netGuid)
        {
            return netGuid.Convert().ToString().Replace("-", "").ToUpper();
        }


        public object[,] PropertyData(DateTime starTime, DateTime endTime, Guid entId, PropertyType propertyType, PeriodType period)
        {
            try
            {
                var param = new GetPropertyValueListParameterSet
                {
                    StartDate = starTime,
                    EndDate = endTime,
                    EntityId = entId,
                    PeriodTypeId = period,
                    PropertyTypeId = propertyType
                };
                var dataList = ExecuteSync(_SeriesDataClient.GetPropertyValueListAsync, param);
                object[,] res2 = new object[dataList.Count() + 1, 2];

                res2[0, 0] = "Метка времени";
                res2[0, 1] = propertyType.ToString();
                var i = 1;
                foreach (var ss in dataList)
                {
                    res2[i, 0] = ss.Date;
                    res2[i, 1] = Common.GetValue(ss);
                    i++;
                }
                return res2;
            }
            catch (Exception err)
            {
                return new object[,] { { err.Data.ToString() }, { err.Message } };
            }
        }
    }
}