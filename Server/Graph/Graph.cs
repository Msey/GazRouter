using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.Core;
using GazRouter.DAL.Dictionaries.ValveTypes;
using GazRouter.DAL.ObjectModel.MeasLine;
using GazRouter.DAL.ObjectModel.MeasPoint;
using GazRouter.DAL.ObjectModel.Valves;
using GazRouter.DAL.SeriesData.PropertyValues;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PeriodTypes;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasPoint;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.DTO.SeriesData.PropertyValues;

namespace GazRouter.Graph
{
    public class Graph
    {
        private int _systemId;

        public Graph(int gasTransportSystemId)
        {
            _systemId = gasTransportSystemId;
            VertexList = new List<Vertex>();
            EdgeList = new List<Edge>();
        }

        public List<Vertex> VertexList { get; set; }
        public List<Edge> EdgeList { get; set; }
        public string Name { get; set; }

        public int SystemId
        {
            get
            {
                return _systemId;
            }
        }

        /// <summary>
        /// Соседние вершины
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public List<Vertex> NeighbourVertices(Vertex v)
        {
            var neighbours = new List<Vertex>();
            foreach (var edge in this.EdgeList)
            {
                if (edge.V1 == v)
                {
                    neighbours.Add(edge.V2);
                }

                if (edge.V2 == v)
                {
                    neighbours.Add(edge.V1);
                }
            }

            return neighbours;
        }

        public void LoadMeasurings(DateTime date, ExecutionContext context)
        {
            var data = new GetEntityPropertyValueListQuery(context).Execute(new GetEntityPropertyValueListParameterSet
            {
                PeriodType = PeriodType.Twohours,
                StartDate = date,
                EndDate = date.AddDays(1),
                CreateEmpty = false,
                LoadMessages = true
            });

            ParseMeasurings(data, context);

        }

        private static double? GetLastValueDouble(Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> measurings,
         Guid entityId, PropertyType propertyType)
        {
            var value = GetLastValueDto(measurings, entityId, propertyType) as PropertyValueDoubleDTO;
            return value?.Value;
        }

        private static BasePropertyValueDTO GetLastValueDto(
            Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> measurings, Guid entityId, PropertyType propertyType)
        {
            if (!measurings.ContainsKey(entityId))
            {
                return null;
            }
            if (!measurings[entityId].ContainsKey(propertyType))
            {
                return null;
            }

            return measurings[entityId][propertyType].OrderByDescending(v => v.Date).FirstOrDefault();
        }

        private void ParseMeasurings(Dictionary<Guid, Dictionary<PropertyType, List<BasePropertyValueDTO>>> measurings, ExecutionContext context)
        {
            var perevodQ = 1;//24.0/1000;
            var perevodT = 273.15;
            var perevodP = 1;//1/10.2;

            // Параметры по ГРС, линиям замера газа
            foreach (var vertex in VertexList.Where(v => v.Type == EntityType.DistrStation || v.Type == EntityType.MeasLine))
            {
                var sign = 0.0;
                if (vertex.BalanceSignId == Sign.In)
                {
                    sign = 1.0;
                }
                if (vertex.BalanceSignId == Sign.Out)
                {
                    sign = -1.0;
                }
                vertex.Consumption = sign * perevodQ *
                                     GetLastValueDouble(measurings, vertex.EntityId.Value, PropertyType.Flow);
                if (vertex.Type == EntityType.DistrStation)
                {
                    vertex.Pressure = perevodP *
                                      GetLastValueDouble(measurings, vertex.EntityId.Value, PropertyType.PressureInlet);
                    vertex.Temperature = perevodT +
                                         GetLastValueDouble(measurings, vertex.EntityId.Value,
                                             PropertyType.TemperatureInlet);
                }
                else
                {
                    vertex.Pressure = perevodP *
                                  GetLastValueDouble(measurings, vertex.EntityId.Value, PropertyType.PressureOutlet);
                    vertex.Temperature = perevodT + GetLastValueDouble(measurings, vertex.EntityId.Value, PropertyType.TemperatureOutlet);
                }
            }

            // Параметры по КЦ
            foreach (var edge in EdgeList.Where(e => e.EntityType == EntityType.CompShop))
            {
                edge.V1.Pressure = perevodP * GetLastValueDouble(measurings, edge.EntityId, PropertyType.PressureInlet);
                edge.V1.Temperature = perevodT + GetLastValueDouble(measurings, edge.EntityId,
                    PropertyType.TemperatureInlet);

                edge.V2.Pressure = perevodP * GetLastValueDouble(measurings, edge.EntityId, PropertyType.PressureOutlet);
                edge.V2.Temperature = perevodT + GetLastValueDouble(measurings, edge.EntityId,
                    PropertyType.TemperatureOutlet);

                edge.Consumption = perevodQ * GetLastValueDouble(measurings, edge.EntityId, PropertyType.Pumping);
            }

            var valves = new GetValveListQuery(context).Execute(new GetValveListParameterSet());
            var valveTypes = new GetValveTypesListQuery(context).Execute();

            var removedEdges = new List<Edge>();
            // Параметры по кранам
            foreach (var edge in EdgeList.Where(e => e.EntityType == EntityType.Valve && e.Name != "372" && e.Name != "374"))
            {
                if (
                    new List<Guid>
                    {
                        new Guid("f8e5352c-fea5-aa4b-bdc5-51c9651c9e66"),
                        new Guid("a49dd222-1ba2-984d-ad5f-44bb49a8869f"),
                        new Guid("4274e911-5083-f749-8873-e3846c43f746"),
                        new Guid("1efe036c-83c9-8646-ac1c-85c1ecf4993e")
                    }.Any(l => l == edge.EntityId))
                {
                    continue;
                }
                //edge.V1.Pressure = perevodP*GetLastValueDouble(measurings, edge.EntityId, PropertyType.PressureInlet);
                //edge.V1.Temperature = perevodT +
                //                      GetLastValueDouble(measurings, edge.EntityId, PropertyType.TemperatureInlet);
                //edge.V2.Pressure = perevodP*GetLastValueDouble(measurings, edge.EntityId, PropertyType.PressureOutlet);
                //edge.V2.Temperature = perevodT +
                //                      GetLastValueDouble(measurings, edge.EntityId, PropertyType.TemperatureOutlet);

                var valve = valves.Single(v => v.Id == edge.EntityId);

                var stateValve = GetLastValueDouble(measurings, valve.Id, PropertyType.StateValve);
                var stateByPass1 = valve.Bypass1TypeId.HasValue ? GetLastValueDouble(measurings, valve.Id, PropertyType.StateBypass1) : null;
                var stateByPass2 = valve.Bypass2TypeId.HasValue ? GetLastValueDouble(measurings, valve.Id, PropertyType.StateBypass2) : null;
                var stateByPass3 = valve.Bypass3TypeId.HasValue ? GetLastValueDouble(measurings, valve.Id, PropertyType.StateBypass3) : null;

                // кран закрыт
                if (stateValve.HasValue && stateValve == 2)
                {
                    removedEdges.Add(edge);
                    continue;
                }

                var diameter = valveTypes.Single(t => t.Id == valve.ValveTypeId).DiameterConv;
                if (stateByPass1.HasValue && stateByPass1 == 1)
                {
                    diameter += valveTypes.Single(t => t.Id == valve.Bypass1TypeId).DiameterConv;
                }
                if (stateByPass2.HasValue && stateByPass2.Value == 1)
                {
                    diameter += valveTypes.Single(t => t.Id == valve.Bypass2TypeId).DiameterConv;
                }
                if (stateByPass3.HasValue && stateByPass3.Value == 1)
                {
                    diameter += valveTypes.Single(t => t.Id == valve.Bypass3TypeId).DiameterConv;
                }
                edge.Diameter = diameter;
            }

            var measPoints = new GetMeasPointListQuery(context).Execute(new GetMeasPointListParameterSet());

            // Температура грунта плотность газа
            foreach (var vertex in VertexList.Where(v => v.EntityId.HasValue).ToList())
            {
                var measPoint =
                    measPoints.SingleOrDefault(
                        mp =>
                            (mp.CompShopId.HasValue && mp.CompShopId == vertex.EntityId) ||
                            (mp.DistrStationId.HasValue && mp.DistrStationId == vertex.EntityId) ||
                            (mp.MeasLineId.HasValue && mp.MeasLineId == vertex.EntityId));
                if (measPoint == null)
                {
                    continue;
                }
                vertex.TSoil = perevodT + GetLastValueDouble(measurings, measPoint.Id, PropertyType.TemperatureEarth);
                vertex.Density = GetLastValueDouble(measurings, measPoint.Id, PropertyType.Density);
            }

            EdgeList.RemoveAll(e => removedEdges.Any(ee => ee == e));
            VertexList.RemoveAll(v => EdgeList.All(e => e.V1 != v && e.V2 != v));

        }
    }
}