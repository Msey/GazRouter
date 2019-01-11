using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.Common.Cache;
using GazRouter.DTO.Dictionaries.PhisicalTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using Microsoft.Practices.ServiceLocation;

namespace GazRouter.Modes.ProcessMonitoring.Dashboards.ContentManagement
{
    
    public class BadDataHound
    {
        public Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> Data { get; set; }
        private static IClientCache ClientCache => ServiceLocator.Current.GetInstance<IClientCache>();

        public BadDataHound(Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> data)
        {
            Data = data;

            foreach (var entity in data)
            {
                foreach (var property in entity.Value)
                {
                    var pt =
                        ClientCache.DictionaryRepository.PropertyTypes.SingleOrDefault(
                            p => p.PropertyType == property.Key);
                    if (pt != null && pt.PhysicalType.PhysicalType == PhysicalType.Pressure)
                    {
                        // По методу 3-х сигм
                        // Расчет сигмы и среднего
                        property.Value.Sort();
                        var vl = property.Value.OfType<PropertyValueDoubleDTO>().Select(value => value.Value).ToList();
                        var avg = vl.Average();
                        var sigma = Math.Sqrt(vl.Sum(v => Math.Pow(v - avg, 2))/vl.Count);

                        //Проверка данных
                        foreach (var v in property.Value.OfType<PropertyValueDoubleDTO>().Where(v => Math.Abs(v.Value - avg) > 3 * sigma))
                        {
                            v.QualityCode = QualityCode.Bad;
                        }
                    }
                }
            }
        }
    }
}