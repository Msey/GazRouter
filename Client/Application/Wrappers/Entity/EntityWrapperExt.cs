using System;
using System.Collections.Generic;
using GazRouter.DTO.ObjectModel;
using GazRouter.DTO.ObjectModel.Boilers;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.CompUnits;
using GazRouter.DTO.ObjectModel.Consumers;
using GazRouter.DTO.ObjectModel.CoolingUnit;
using GazRouter.DTO.ObjectModel.DistrStationOutlets;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.OperConsumers;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.PowerUnits;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Valves;

namespace GazRouter.Application.Wrappers.Entity
{
    public static class EntityWrapperExt
    {
        private static readonly Dictionary<Type, Func<CommonEntityDTO, bool, IEntityWrapper>> Types =
            new Dictionary<Type, Func<CommonEntityDTO, bool, IEntityWrapper>>();

        static EntityWrapperExt()
        {
            RegisterWrapper<CompStationDTO>((t, x) => new CompStationWrapper(t, x));
            RegisterWrapper<CompShopDTO>((t, x) => new CompShopWrapper(t, x));
            RegisterWrapper<CompUnitDTO>((t, x) => new CompUnitWrapper(t, x));
            RegisterWrapper<DistrStationOutletDTO>((t, x) => new DistrStationOutletWrapper(t, x));
            RegisterWrapper<DistrStationDTO>((t, x) => new DistrStationWrapper(t, x));
            RegisterWrapper<MeasLineDTO>((t, x) => new MeasLineWrapper(t, x));
            RegisterWrapper<MeasPointDTO>((t, x) => new MeasPointWrapper(t, x));
            RegisterWrapper<MeasStationDTO>((t, x) => new MeasStationWrapper(t, x));
            RegisterWrapper<PipelineDTO>((t, x) => new PipelineWrapper(t, true));
            RegisterWrapper<ReducingStationDTO>((t, x) => new ReducingStationWrapper(t, x));
            RegisterWrapper<ValveDTO>((t, x) => new ValveWrapper(t, x));
            RegisterWrapper<BoilerDTO>((t, x) => new BoilerWrapper(t, x));
            RegisterWrapper<PowerUnitDTO>((t, x) => new PowerUnitWrapper(t, x));
            RegisterWrapper<CoolingUnitDTO>((t, x) => new CoolingUnitWrapper(t, x));
            RegisterWrapper<ConsumerDTO>((t, x) => new ConsumerWrapper(t, x));
            RegisterWrapper<OperConsumerDTO>((t, x) => new OperConsumerWrapper(t, x));
        }

        private static void RegisterWrapper<T>(Func<T, bool, IEntityWrapper> func) where T : CommonEntityDTO
        {
            Types.Add(typeof (T), (t, x) => func((T) t, x));
        }

        public static IEntityWrapper GetWrapper(this CommonEntityDTO dto, bool displaySystem)
        {
            var type = dto.GetType();
            return Types.ContainsKey(type) ? Types[type](dto, displaySystem) : new EntityWrapperBase<CommonEntityDTO>(dto, displaySystem);
        }
    }
}