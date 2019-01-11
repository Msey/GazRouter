using System;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using GazRouter.Application.Helpers;
using GazRouter.Common.Cache;
using GazRouter.DataProviders.GasCosts;
using GazRouter.DataProviders.ObjectModel;
using GazRouter.DataProviders.SeriesData;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.Dictionaries.Targets;
using GazRouter.DTO.Dictionaries.ValvePurposes;
using GazRouter.DTO.GasCosts;
using GazRouter.DTO.GasCosts.Import;
using GazRouter.DTO.ManualInput.ValveSwitches;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.DTO.SeriesData.CompUnitPropertyValues;
using GazRouter.Modes.GasCosts.Dialogs.ChemicalAnalysisCosts;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using GazRouter.Modes.GasCosts.Dialogs.UnitStartCosts;
using GazRouter.Modes.GasCosts.Dialogs.UnitStopCosts;
using GazRouter.Modes.GasCosts.Dialogs.ValveControlCosts;
using Microsoft.Practices.ServiceLocation;
using Utils.Extensions;
using Utils.Units;

namespace GazRouter.Modes.GasCosts.Imports
{
    public class GasCostImportHelper
    {
        // Переключение крана
        public static void ImportValveCosts(DateTime eventDate, ValveDTO valve, ValveSwitchType switchType,
            int shiftId)
        {
            var model = new ValveControlCostsModel();
            
            var valveTypeId = 0;
            switch (switchType)
            {
                case ValveSwitchType.Valve:
                    valveTypeId = valve.ValveTypeId;
                    break;

                case ValveSwitchType.Bypass1:
                    valveTypeId = valve.Bypass1TypeId ?? valve.ValveTypeId;
                    break;

                case ValveSwitchType.Bypass2:
                    valveTypeId = valve.Bypass2TypeId ?? valve.ValveTypeId;
                    break;

                case ValveSwitchType.Bypass3:
                    valveTypeId = valve.Bypass3TypeId ?? valve.ValveTypeId;
                    break;
            }

            var valveType =
                ServiceLocator.Current.GetInstance<IClientCache>()
                    .DictionaryRepository.ValveTypes.Single(vt => vt.Id == valveTypeId);
            var shifting = new ValveShifting
            {
                Id = valveType.Id,
                Name = valveType.Name,
                Count = 1,
                RatedConsumption = valveType.RatedConsumption
            };


            model.ValveShiftings.Add(shifting);
            model.Calculate();

            SaveCost(model, eventDate,
                valve.ValvePurposeId != ValvePurpose.Linear ? valve.CompShopId.Value : valve.ParentId.Value,
                valve.ValvePurposeId != ValvePurpose.Linear ? CostType.CT21 : CostType.CT28,
                valve.SiteId, shiftId);
        }

        // Пуск ГПА
        public static void SaveCompUnitStartCosts(DateTime eventDate, CompUnitDTO unit, Guid siteId, int startId)
        {
            var model = new UnitStartCostsModel
            {
                Unit = new CompUnit(unit),
                // Нужно уточнить у дочки при пуске агрегата нужно ли продувать контур?
                // Пока устанавливаю что нужно
                ProfileIsNotEmpty = true,
                StartCount = 1,
                NormalShifting = true
            };

            SaveCost(model, eventDate, unit.Id, CostType.CT18, siteId, startId);
        }

        // Останов ГПА
        public static async void SaveCompUnitStopCosts(DateTime eventDate, CompUnitDTO unit, Guid siteId, int stopId)
        {
            var ts = SeriesHelper.GetLastCompletedSession();
            var values = await new SeriesDataServiceProxy().GetCompUnitPropertyValueListAsync(
                new GetCompUnitPropertyValuesParameterSet
                {
                    CompUnitId = unit.Id,
                    Timestamp = ts
                });

            var model = new UnitStopCostsModel
            {
                Unit = new CompUnit(unit),
                PressureInlet =
                    Pressure.FromKgh(values.GetOrDefault(PropertyType.PressureSuperchargerInlet)?.Value ?? 0),
                PressureOutlet =
                    Pressure.FromKgh(values.GetOrDefault(PropertyType.PressureSuperchargerOutlet)?.Value ?? 0),
                TemperatureInlet =
                    Temperature.FromCelsius(values.GetOrDefault(PropertyType.TemperatureSuperchargerInlet)?.Value ?? 0),
                TemperatureOutlet =
                    Temperature.FromCelsius(values.GetOrDefault(PropertyType.TemperatureSuperchargerOutlet)?.Value ?? 0),
                Density = Density.FromKilogramsPerCubicMeter(values.GetOrDefault(PropertyType.Density)?.Value ?? 0),
                PressureAir = Pressure.FromMmHg(values.GetOrDefault(PropertyType.Density)?.Value ?? 0),
                NormalShifting = true,
                StopCount = 1
            };

            SaveCost(model, eventDate, unit.Id, CostType.CT19, siteId, stopId);
        }

        // Хим.анализ
        public static async void SaveChemicalAnalysisCosts(DateTime eventDate, Guid pointId, int analysisId)
        {
            var point = await new ObjectModelServiceProxy().GetEntityByIdAsync(pointId) as MeasPointDTO;
            if (point == null) return;

            var model = new ChemicalAnalysisCostsModel
            {
                MeasCount = 1,
                Q = point.ChromatographConsumptionRate,
                Time = point.ChromatographTestTime
            };
            model.Calculate();

            if (point.CompShopId.HasValue)
            {
                SaveCost(model, eventDate, point.CompShopId.Value, CostType.CT58, point.SiteId, analysisId);
            }

            if (point.DistrStationId.HasValue)
            {
                SaveCost(model, eventDate, point.DistrStationId.Value, CostType.CT59, point.SiteId, analysisId);
            }

            if (point.MeasLineId.HasValue)
            {
                var measLine =
                    await new ObjectModelServiceProxy().GetEntityByIdAsync(point.MeasLineId.Value) as MeasLineDTO;
                if (measLine?.ParentId != null)
                {
                    SaveCost(model, eventDate, measLine.ParentId.Value, CostType.CT26, point.SiteId, analysisId);
                }
            }
        }

        public static async void SaveCost<TModel>(TModel model, DateTime eventDate, Guid entityId, CostType costType,
            Guid siteId, int? extId)
            where TModel : ICostCalcModel
        {

            var importId = extId.HasValue ? 
                await new GasCostsServiceProxy().AddGasCostImportInfoAsync(
                    new AddGasCostImportInfoParameterSet
                    {
                        ImportDate = DateTime.Now,
                        ExternalId = extId.Value
                    })
                    : (int?)null;

            string inputDataString;
            using (TextWriter tw = new StringWriter())
            {
                var xmlSerializer = new XmlSerializer(typeof(TModel));
                xmlSerializer.Serialize(tw, model);
                inputDataString = tw.ToString();
                tw.Close();
            }

            var addGasCostParameterSet = new AddGasCostParameterSet
            {
                CalculatedVolume = model.Calculate(),
                MeasuredVolume = null,
                Date = eventDate,
                EntityId = entityId,
                CostType = costType,
                Target = Target.Fact,
                InputData = inputDataString,
                SiteId = siteId,
                ImportId = importId
            };

            await new GasCostsServiceProxy().AddGasCostAsync(addGasCostParameterSet);
        }
    }
}