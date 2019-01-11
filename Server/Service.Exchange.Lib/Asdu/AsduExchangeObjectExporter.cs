using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GazRouter.DataServices.Infrastructure;
using GazRouter.DAL.Core;
using GazRouter.DAL.DataExchange.Asdu;
using GazRouter.DAL.Dictionaries.Enterprises;
using GazRouter.DTO.DataExchange.Asdu;
using GazRouter.DTO.Dictionaries.Enterprises;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.SeriesData.PropertyValues;
using Utils.Extensions;

namespace GazRouter.Service.Exchange.Lib.Asdu
{
    public class AsduExchangeObjectExporter 
    {
        private readonly ExecutionContext _context;
        protected List<AsduExchangePropertyValueDTO> _data;
        private EnterpriseDTO _currentEnterprise;
        readonly IDictionary<string, string> _enterpriseDict = new Dictionary<string, string> {{ "T_SRT", "ГП ТГ Саратов" } };
        private readonly GetEntityPropertyValueListParameterSet _parametersSet;

        public AsduExchangeObjectExporter(ExecutionContext context, int seriesId, PeriodType periodType = PeriodType.Twohours)
        {
            _context = context;
            _currentEnterprise = new GetEnterpriseListQuery(_context).Execute(AppSettingsManager.CurrentEnterpriseId).Single(e => e.Id == AppSettingsManager.CurrentEnterpriseId);
            _parametersSet = new GetEntityPropertyValueListParameterSet
            {
                PeriodType = periodType,
                SeriesId = seriesId,
            };

        }
        public AsduExchangeObjectExporter(ExecutionContext context,  DateTime dt, PeriodType periodType = PeriodType.Twohours)
        {
            _context = context;
            _currentEnterprise = new GetEnterpriseListQuery(_context).Execute(AppSettingsManager.CurrentEnterpriseId).Single(e => e.Id == AppSettingsManager.CurrentEnterpriseId);
            _parametersSet = new GetEntityPropertyValueListParameterSet
            {
                PeriodType = periodType,
                EndDate = dt,
                StartDate = dt.Add(-periodType.ToTimeSpan())
            };
        }


        public AsduExchangeObject Build()
        {
            _data = new GetEntityPropertyValueListForAsduQuery(_context).Execute(_parametersSet);
            if (!_data.Any()) return null;

            var keyDate = _data.First().PropertyValue.Date;
            Debug.Assert(keyDate != null, "keyDate != null");
            var result = new AsduExchangeObject()
            {
                HeaderSection =
                           new AsduExchangeHeader
                           {
                               Generated = new AsduAtDateTime {Value = DateTime.Now},
                               ReferenceTime = new AsduInDateTime {Value = (DateTime) keyDate},
                               Template = new AsduElementId { Id = $@"{_currentEnterprise.Code}.PT2H.RT.V1" },
                               Sender = new AsduElementId { Id = $@"{_enterpriseDict[_currentEnterprise.Code]}" }
                            },

                DataSections = GetDataSections().ToList()
            };

            return result;
        }


        private IEnumerable<AsduDataSection> GetDataSections()
        {
            foreach (var item in _data)
            {
                var value = ExchangeHelper.GetValue(item.PropertyValue)?.ToString();
                if (string.IsNullOrEmpty(value)) continue;;

                yield return new AsduDataSection
                {
                    Value = new AsduExchangeData
                    {
                        Value = value,
                    },
                    Identifier = new AsduExchangeIdentifier { Id = ConvertGuid(item.ParameterGid) },
                };
            }
        }

        private static string ConvertGuid(Guid guid)
        {
            return guid.Convert().ToString().Replace("-", "").ToUpper();
        }
    }
}