using System;

namespace GazRouter.DTO.ObjectModel.Entities
{
    [Flags]
    public enum EntityFilter
    {
        Sites = 1,
        CompStations = 2,
        CompShops = 4,
        CompUnits = 8,
        DistrStations = 16,
        MeasStations = 32,
        ReducingStations = 64,
        MeasLines = 128,
        DistrStationOutlets = 256,
        Consumers = 512,
        MeasPoints = 1024,
        CoolingStations = 2048,
        CoolingUnits = 4096,
        PowerPlants = 8192,
        PowerUnits = 16384,
        Boilers = 32768,
        BoilerPlants = 65536,
        Pipelines = 131072,
        LinearValves = 262144,
        OperConsumers = 524288
    }
}