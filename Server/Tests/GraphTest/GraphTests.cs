using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.Balances.Routes;
using GazRouter.DAL.Balances.Values;
using GazRouter.DAL.Dictionaries.Enterprises;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DAL.ObjectModel.ReducingStations;
using GazRouter.DAL.ObjectModel.Segment.Site;
using GazRouter.DAL.ObjectModel.Sites;
using GazRouter.DAL.ObjectModel.Valves;
using GazRouter.DTO.Balances.Routes;
using GazRouter.DTO.Balances.Values;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.ObjectModel.Valves;
using GazRouter.Graph;
using KingKong;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestBase.Infra;

namespace GraphTest
{
    [TestClass]
    public class GraphTests : TransactionTestsBase
    {
        [TestMethod, TestCategory("UnStable")]
        public void GetRoutesTest()
        {
            const int contractId = 6224; //1688;
            TransportTaskTestNew(contractId);
            
            GasInPipeCalculationTest();
        }
        
        public void TransportTaskTestNew(int contractId)
        {
            // todo:  - отключен, т.к. падает билд!
//            var balanceValues =
//                    new GetBalanceValueListQuery(Context).Execute(contractId);
//            var measStations =
//                new GetMeasStationListQuery(Context).Execute(new GetMeasStationListParameterSet());
//            var gasOwnersId = balanceValues.Select(bv => bv.GasOwnerId).Distinct().ToList();
//            var routes = new GetRouteListQuery(Context).Execute(new GetRouteListParameterSet());
//            const double d = 1000.0;
//
//            foreach (var gasOwnerId in gasOwnersId)
//            {
//                //Заполнение объемов поставок
//                var shippers =
//                    balanceValues.Where(
//                        v =>
//                            v.GasOwnerId == gasOwnerId && v.MeasStationId.HasValue &&
//                            measStations.Single(ms => ms.Id == v.MeasStationId.Value).BalanceSignId == Sign.In).ToList();
//                var vectFrom = new int[shippers.Count];
//                shippers.ForEach(sh => vectFrom[shippers.IndexOf(sh)] = (int) (sh.BaseValue*d));
//                //Заполнение объемов потребителей
//                var consumers = balanceValues.Where(
//                        v =>
//                            v.GasOwnerId == gasOwnerId && v.MeasStationId.HasValue &&
//                            measStations.Single(ms => ms.Id == v.MeasStationId.Value).BalanceSignId == Sign.Out).ToList();
//                var outletDistr =
//                    from v in balanceValues.Where(v => v.GasOwnerId == gasOwnerId && v.DistrStationId.HasValue)
//                    group v by v.DistrStationId.Value
//                    into gr
//                    select new {Id = gr.Key, Volume = gr.Sum(g => g.BaseValue)};
//                outletDistr.ToList()
//                    .ForEach(
//                        o =>
//                            consumers.Add(new BalanceValueDTO
//                            {
//                                BaseValue = o.Volume,
//                                ContractId = contractId,
//                                DistrStationId = o.Id,
//                                GasOwnerId = gasOwnerId
//                            }));
//                consumers.AddRange(balanceValues.Where(v => v.GasOwnerId == gasOwnerId && v.CompStationId.HasValue));
//                var vectTo = new int[consumers.Count];
//                consumers.ForEach(con => vectTo[consumers.IndexOf(con)] = (int) (con.BaseValue*d));
//
//                if (vectFrom.Sum() == vectTo.Sum())
//                {
//                    //Заполнение матрицы matrCost(матрица расстояний) 
//                    var matrCost = new int[vectFrom.GetLength(0), vectTo.GetLength(0)];
//                    for (var i = 0; i < matrCost.GetLength(0); i++)
//                    {
//                        for (var j = 0; j < matrCost.GetLength(1); j++)
//                        {
//                            var outletId = consumers[j].MeasStationId.HasValue
//                                ? consumers[j].MeasStationId.Value
//                                : (consumers[j].DistrStationId.HasValue
//                                    ? consumers[j].DistrStationId.Value
//                                    : consumers[j].CompStationId.Value);
//                            var route = routes.SingleOrDefault(
//                                r => r.InletId == shippers[i].MeasStationId.Value && r.OutletId == outletId);
//                            matrCost[i, j] = route != null && route.Length > 0
//                                ? (int) (route.Length*d) : int.MaxValue;
//                        }
//                    }
//                    
//                    //Запуск расчета и запись результатов расчет в БД
//                    var result = new TransportTask(vectFrom, vectTo, matrCost).RunOptimization();
//                    var resultDouble = new double?[result.GetLength(0), result.GetLength(1)];
//                    for (var i = 0; i < result.GetLength(0); i++)
//                    {
//                        for (var j = 0; j < result.GetLength(1); j++)
//                        {
//                            if (!result[i, j].HasValue)
//                                continue;
//                            resultDouble[i, j] = result[i, j] / d;
//                        }
//                    }
//                    for (var i = 0; i < resultDouble.GetLength(0); i++)
//                    {
//                        for (var j = 0; j < resultDouble.GetLength(1); j++)
//                        {
//                            if (!resultDouble[i, j].HasValue || resultDouble[i,j].Value == 0.0)
//                                continue;
//                            var outletId = consumers[j].MeasStationId.HasValue
//                                ? consumers[j].MeasStationId.Value
//                                : (consumers[j].DistrStationId.HasValue
//                                    ? consumers[j].DistrStationId.Value
//                                    : consumers[j].CompStationId.Value);
//                            var route = routes.Single(
//                                r => r.InletId == shippers[i].MeasStationId.Value && r.OutletId == outletId);
//                            
//                            //new AddTransportCommand(Context).Execute(new AddTransportParameterSet
//                            //{
//                            //    ContractId = contractId,
//                            //    OwnerId = gasOwnerId,
//                            //    RouteId = route.RouteId.Value,
//                            //    Volume = result[i, j].Value
//                            //});
//                        }
//                    }
//                }
//                var t = 0;
//            }

        }

        public void GasInPipeCalculationTest()
        {
            const int systemId = 1;
            var enterprises = new GetEnterpriseListQuery(Context).Execute(null);
            var tomsk = enterprises.Single(ent => ent.Code == "T_TMS");
            //var yugorsk = enterprises.Single(ent => ent.Code == "T_YGR");
            // var saratov = enterprises.Single(ent => ent.Code == "T_SRT");

            var newBuilder = new GraphBuilderNew(systemId, tomsk.Id, Context);
            var p = newBuilder.BuildGraph();
            var groups = p.VertexList.GroupBy(e => e.Type);

            var bridge = p.EdgeList.Where(e => e.Name == "НГПЗ-Парабель I, р/н., р.Ларь-Еган, 131,6км").ToList();
            bridge.Where(b => b.KilometerOfEndPoint >= 133.2).ToList().ForEach(b => b.KilometerOfEndPoint = 133.2);

            var connectivity = CheckConnectivity(p);
            Assert.IsTrue(connectivity);

            // Замена параллельных газопроводов газопроводом dm=(sum di^5/2)^2/5
            p.EdgeList.RemoveAll(
                e =>
                    e.EntityId == new Guid("434346c6-3f96-0729-e040-f00a3905260c") ||
                    e.EntityId == new Guid("434346c6-d895-0729-e040-f00a3905260c"));
            p.VertexList.RemoveAll(v => p.EdgeList.Count(e => e.V1.Id == v.Id || e.V2.Id == v.Id) < 1);
            p.EdgeList.Where(
                e =>
                    (e.EntityId == new Guid("434346c6-4096-0729-e040-f00a3905260c") && e.KilometerOfStartPoint >= 305.06) ||
                    (e.EntityId == new Guid("434346c6-3e96-0729-e040-f00a3905260c") && e.KilometerOfEndPoint <= 404.5))
                .ToList()
                .ForEach(e => e.Diameter = Math.Pow(2.0 * Math.Pow(e.Diameter, 5 / 2.0), 2 / 5.0));

            // Загрузка данных, задание недостающих данных, подготовка графа к расчету
            p.LoadMeasurings(new DateTime(2013, 03, 23), Context); // Томск
                                                                   // p.LoadMeasurings(new DateTime(2014, 08, 21), Context); // Саратов

            p.EdgeList.RemoveAll(
                e =>
                    p.EdgeList.Count(
                        ee =>
                            (e.V1.Id == ee.V1.Id || e.V1.Id == ee.V2.Id)) == 1 && p.EdgeList.Count(
                                ee =>
                                    (e.V2.Id == ee.V1.Id || e.V2.Id == ee.V2.Id)) == 1);
            p.VertexList.RemoveAll(v => p.EdgeList.Count(e => e.V1.Id == v.Id || e.V2.Id == v.Id) < 1);

            connectivity = CheckConnectivity(p);
            Assert.IsTrue(connectivity);
            SetMissingData(p);
            PrepareGraphToCalculate(p);
            EquivalentEdges(p);

            connectivity = CheckConnectivity(p);
            Assert.IsTrue(connectivity);
            var rootId =
                p.VertexList.First(
                    v =>
                        v.Consumption ==
                        p.VertexList.Where(vvv => vvv.Consumption.HasValue).Max(vv => vv.Consumption)).Id;

            var listBefore = new List<Kilometers>();
            p.VertexList.Where(v => v.Pressure.HasValue)
                .ToList()
                .ForEach(v => listBefore.Add(new Kilometers { KmStart = v.Id, Cons = v.Pressure.Value }));

            foreach (var root in p.VertexList.Where(v => v.Id == rootId))
            {
                var pNew = DeepCopy(p);
                //new GasInPipeCalculationNew(pNew, pNew.VertexList.Single(v => v.Id == root.Id)).Calculate();

                new GasInPipeCalculation(pNew, pNew.VertexList.Single(v => v.Id == root.Id)).Calculate();
                var sites = AllSites(tomsk.Id, systemId);

                var bigSupply = 0.0;
                foreach (var site in sites.OrderBy(s => s.Value))
                {
                    var graphOneSite = Site(pNew, site.Key);
                    connectivity = CheckConnectivity(graphOneSite);
                    Assert.IsTrue(connectivity);

                    var arr = new object[graphOneSite.VertexList.Count(v => v.Pressure.HasValue) - 1, 4];
                    foreach (var vertex in graphOneSite.VertexList.Where(v => Root(graphOneSite, site.Key.Id).Id == v.Id))
                    {
                        var graphOneSiteCopy = DeepCopy(graphOneSite);
                        var vFirst1 = graphOneSiteCopy.VertexList.Single(v => v.Id == vertex.Id);
                        var calc = new GasInPipeCalculation(graphOneSiteCopy, vFirst1).CalculatePressure();
                        graphOneSiteCopy.VertexList.ForEach(
                            v => p.VertexList.Single(vv => vv.Id == v.Id).Pressure = v.Pressure);

                        var index = 0;
                        foreach (var d in calc.OrderByDescending(c => Math.Abs(c.Value)))
                        {
                            var pReal = listBefore.Single(l => (int)l.KmStart == d.Key.Id).Cons;
                            var pCalc = Math.Round(d.Key.Pressure.Value, 4);
                            arr[index, 0] = d.Key.Name;
                            arr[index, 1] = pReal;
                            arr[index, 2] = pCalc;
                            arr[index, 3] = Math.Round(pReal - pCalc, 4);
                            index++;
                        }
                        foreach (var edge in graphOneSiteCopy.EdgeList)
                        {
                            var gasSupply = new GasVolume().GasSupplyOnOneEdge(edge);
                            edge.GasVolume = gasSupply;
                        }

                        bigSupply += graphOneSiteCopy.EdgeList.Where(e => e.GasVolume.HasValue).Sum(e => e.GasVolume.Value);
                    }
                }
                var sup = bigSupply;
            }

            ////////////////////////////// Кратчайший путь ///////////////////////////////
            foreach (var vertex in p.VertexList.Where(v => !v.Pressure.HasValue).ToList())
            {
                var edge = p.EdgeList.First(e => e.V1.Id == vertex.Id || e.V2.Id == vertex.Id);
                vertex.Pressure = edge.V1.Pressure.HasValue ? edge.V1.Pressure : edge.V2.Pressure;
            }
            var verticesFrom =
                p.VertexList.Where(v => v.Type == EntityType.MeasLine && v.BalanceSignId == Sign.In)
                    .ToList();
            var verticesTo =
                p.VertexList.Where(
                    v =>
                        v.Type == EntityType.DistrStation || v.Type == EntityType.CompShop ||
                        (v.Type == EntityType.MeasLine && v.BalanceSignId == Sign.Out)).ToList();
            foreach (var vFrom in verticesFrom)
            {
                var minRoute = new MinRoute(p, vFrom);
                var routes = minRoute.GetMinRoute(verticesTo);
                Assert.IsTrue(routes.Any());
            }
        }

        /// <summary>
        /// Подготовка графа к расчету (удал нераб КЦ, удал кр. ребер с неизв. параметрами)
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        private void PrepareGraphToCalculate(Graph graph)
        {
            // Удалить неработающие КЦ
            var compShops = graph.EdgeList.Where(e => e.EntityType == EntityType.CompShop)
                .Where(compShop => compShop.V1.Pressure == compShop.V2.Pressure).ToList();
            graph.EdgeList.RemoveAll(e => compShops.Contains(e));

            foreach (var compShop in compShops)
            {
                compShop.V1.Pressure = null;
                compShop.V2.Pressure = null;
                compShop.V1.Type = new EntityType();
                compShop.V2.Type = new EntityType();
            }

            var vert = graph.VertexList.Where(
                v =>
                    (!v.Pressure.HasValue || v.Pressure.Value == 0) &&
                    (!v.Consumption.HasValue || v.Consumption.Value == 0) &&
                    ((graph.EdgeList.Count(e => e.V1 == v) == 1 && graph.EdgeList.All(e => e.V2 != v)) ||
                     (graph.EdgeList.Count(e => e.V2 == v) == 1 && graph.EdgeList.All(e => e.V1 != v)))).ToList();
            vert.ForEach(v => v.Consumption = 0);
            vert.ForEach(v => v.Pressure = null);

            // Удалить концы с неизв P и Q
            var queue = new Queue<Vertex>();
            var vFirst =
                graph.VertexList.FirstOrDefault(
                    v =>
                        graph.EdgeList.Count(e => e.V1.Id == v.Id || e.V2.Id == v.Id) == 1 &&
                        (!v.Pressure.HasValue || v.Pressure == 0) && (!v.Consumption.HasValue || v.Consumption == 0));
            if (vFirst != null)
            {
                queue.Enqueue(vFirst);
            }
            while (queue.Any())
            {
                var vertex = queue.Dequeue();
                var edge = graph.EdgeList.Single(e => e.V1.Id == vertex.Id || e.V2.Id == vertex.Id);
                graph.EdgeList.Remove(edge);
                graph.VertexList.Remove(vertex);

                vertex =
                    graph.VertexList.FirstOrDefault(
                        v =>
                            graph.EdgeList.Count(e => e.V1.Id == v.Id || e.V2.Id == v.Id) == 1 &&
                            (!v.Pressure.HasValue || v.Pressure == 0) && (!v.Consumption.HasValue || v.Consumption == 0));
                if (vertex != null)
                {
                    queue.Enqueue(vertex);
                }
            }
        }

        private void EquivalentEdges(Graph graph)
        {
            // Замена последовательных трубопроводов эквивалентным трубопроводом
            var verticesDelete = new List<Vertex>();
            foreach (var vertex in graph.VertexList)
            {
                var edges = graph.EdgeList.Where(e => e.V1.Id == vertex.Id || e.V2.Id == vertex.Id).ToList();
                if (edges.Count == 2)
                {
                    var newEdge = NewEdge(edges);
                    if (newEdge == null)
                    {
                        continue;
                    }
                    verticesDelete.Add(vertex);
                    graph.EdgeList.Remove(edges[0]);
                    graph.EdgeList.Remove(edges[1]);
                    graph.EdgeList.Add(newEdge);
                }
            }
            graph.VertexList.RemoveAll(v => verticesDelete.Contains(v));

            // Замена параллельных трубопроводов эквивалентным трубопроводом
            var edgess = new List<Edge>();
            var allSameEdges = new List<List<Edge>>();
            foreach (var edge in graph.EdgeList)
            {
                if (edgess.Contains(edge))
                {
                    continue;
                }

                var sameEdge = graph.EdgeList.Where(e => e.V1 == edge.V1 && e.V2 == edge.V2 && e != edge).ToList();
                if (!sameEdge.Any())
                {
                    continue;
                }

                //var dm = Math.Pow(edge.Diameter, 2.5);
                //dm += sameEdge.Sum(e => Math.Pow(e.Diameter, 2.5));
                //edgess.AddRange(sameEdge);
                //edge.Diameter = Math.Pow(dm, 2 / 5.0);
                //edge.CoefficientOfFlow += sameEdge.Sum(e => e.CoefficientOfFlow);

                var list = new List<Edge> { edge };
                list.AddRange(sameEdge);
                allSameEdges.Add(list);
            }
            foreach (var allSameEdge in allSameEdges)
            {
                var edgeMain = allSameEdge.Any(e => e.TypeOfPipeline == PipelineType.Booster)
                    ? allSameEdge.First(e => e.TypeOfPipeline != PipelineType.Booster)
                    : allSameEdge.First();
                //if (allSameEdge.Any(e => e.TypeOfPipeline == PipelineType.Main))
                //{
                //    edgeMain = allSameEdge.First(e => e.TypeOfPipeline == PipelineType.Main);
                //}
                //else
                //{
                //    edgeMain = allSameEdge.Any(e => e.TypeOfPipeline == PipelineType.Distribution)
                //        ? allSameEdge.First(e => e.TypeOfPipeline == PipelineType.Distribution)
                //        : allSameEdge.First();
                //}
                edgeMain.Diameter = Math.Pow(allSameEdge.Sum(s => Math.Pow(s.Diameter, 2.5)), 2 / 5.0);
                edgeMain.CoefficientOfFlow += allSameEdge.Sum(e => e.CoefficientOfFlow);
                allSameEdge.Remove(edgeMain);
                graph.EdgeList.RemoveAll(e => allSameEdge.Contains(e));
            }

            //graph.EdgeList.RemoveAll(e => edgess.Contains(e));
            graph.EdgeList.RemoveAll(
                e =>
                    e.EntityType == EntityType.CompShop &&
                    (!e.V1.Pressure.HasValue || !e.V2.Pressure.HasValue ||
                     e.V1.Pressure.Value == e.V2.Pressure.Value));
        }

        private Edge NewEdge(List<Edge> edges)
        {
            var edgeFirst = new Edge(new Vertex(), new Vertex());
            var edgeLast = new Edge(new Vertex(), new Vertex());
            var pipe = new Edge(new Vertex(), new Vertex());

            if (edges.Any(e => e.EntityType == EntityType.ReducingStation))
            {
                return null;
            }
            if (edges.Any(e => e.EntityType == EntityType.CompShop))
            {
                return null;
            }
            if (edges.Any(e => e.EntityType == EntityType.Valve))
            {
                edgeFirst = edges[0].KilometerOfStartPoint < edges[1].KilometerOfStartPoint ? edges[0] : edges[1];
                edgeLast = edges[0].KilometerOfStartPoint < edges[1].KilometerOfStartPoint ? edges[1] : edges[0];
                pipe = edges.Single(e => e.EntityType != EntityType.Valve);
            }
            if (edges.All(e => e.EntityType == EntityType.Pipeline))
            {
                if (edges[0].EntityId != edges[1].EntityId)
                {
                    return null;
                }
                edgeFirst = edges[0].KilometerOfStartPoint < edges[1].KilometerOfStartPoint ? edges[0] : edges[1];
                edgeLast = edges[0].KilometerOfStartPoint < edges[1].KilometerOfStartPoint ? edges[1] : edges[0];
                pipe = edgeFirst;
            }

            var edge = new Edge(edgeFirst.V1, edgeLast.V2)
            {
                KilometerOfStartPoint = edgeFirst.KilometerOfStartPoint,
                KilometerOfEndPoint = edgeLast.KilometerOfEndPoint,
                Diameter = edgeFirst.Diameter,
                PressureMax = edgeFirst.PressureMax,
                EntityId = pipe.EntityId,
                EntityType = pipe.EntityType,
                Name = pipe.Name,
                CoefficientOfFlow =
                    Math.Sqrt(edges.Sum(e => Math.Abs(e.KilometerOfEndPoint - e.KilometerOfStartPoint)) /
                              edges.Sum(
                                  e =>
                                      Math.Abs(e.KilometerOfEndPoint - e.KilometerOfStartPoint) /
                                      Math.Pow(e.CoefficientOfFlow, 2.0))),
                TypeOfPipeline = pipe.TypeOfPipeline
            };
            return edge;
        }

        /// <summary>
        /// Задание недостающих данных
        /// (диам, Pпрг, Qгис)
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        private void SetMissingData(Graph graph)
        {
            // 1
            graph.EdgeList.Where(e => e.Diameter == -1.0).ToList().ForEach(e => e.Diameter = 800);

            // 2
            foreach (var edge in graph.EdgeList.Where(e => e.EntityType == EntityType.ReducingStation).ToList())
            {
                ///////// ПРГ Новокузнецк ГРС-1
                if (edge.EntityId == new Guid("9f0766d7-a4f2-fa4b-8747-89fc1d7ef9cb"))
                {
                    edge.V1.Pressure = 29.95;
                    edge.V2.Pressure = 17.29;
                    continue;
                }
                edge.V1.Pressure = 61.0;
                edge.V2.Pressure = 33.5;
            }

            //3
            SetForMeasLineQ(graph);
        }

        private bool CheckConnectivity(Graph graph)
        {
            var validateGraph = new ValidateGraph(graph);
            var connectivity = validateGraph.GraphConnectivity();
            var pipess = new GetPipelineListQuery(Context).Execute(new GetPipelineListParameterSet());
            if (connectivity)
            {
                return true;
            }

            var disjoinedEdges = validateGraph.DisjointedPipelinesId();
            var disjoined = new Dictionary<PipelineDTO, List<Kilometers>>();
            foreach (var disjoinedEdge in disjoinedEdges)
            {
                if (pipess.SingleOrDefault(pp => pp.Id == disjoinedEdge.Key) != null)
                {
                    disjoined.Add(pipess.Single(pp => pp.Id == disjoinedEdge.Key), disjoinedEdge.Value);
                }
            }
            graph.EdgeList.RemoveAll(
                e =>
                    disjoinedEdges.Any(
                        d =>
                            d.Key == e.EntityId &&
                            d.Value.Any(
                                v => e.KilometerOfStartPoint == v.KmStart && e.KilometerOfEndPoint == v.kmEnd)));
            graph.VertexList.RemoveAll(v => graph.EdgeList.All(e => e.V1 != v && e.V2 != v));

            validateGraph = new ValidateGraph(graph);
            connectivity = validateGraph.GraphConnectivity();

            return connectivity;
        }

        private Graph DeepCopy(Graph graph)
        {
            var verticesNew = new List<Vertex>();
            graph.VertexList.ForEach(
                v =>
                    verticesNew.Add(new Vertex
                    {
                        Consumption = v.Consumption,
                        Density = v.Density,
                        EntityId = v.EntityId,
                        Id = v.Id,
                        IsChecked = v.IsChecked,
                        Name = v.Name,
                        Pressure = v.Pressure,
                        Type = v.Type,
                        Temperature = v.Temperature,
                        TSoil = v.TSoil
                    }));

            var edgesNew = new List<Edge>();
            graph.EdgeList.ForEach(
                e =>
                    edgesNew.Add(new Edge(verticesNew.Single(v => v.Id == e.V1.Id),
                        verticesNew.Single(v => v.Id == e.V2.Id))
                    {
                        Consumption = e.Consumption,
                        Diameter = e.Diameter,
                        EntityId = e.EntityId,
                        EntityType = e.EntityType,
                        IsItMaxTree = e.IsItMaxTree,
                        KilometerOfStartPoint = e.KilometerOfStartPoint,
                        KilometerOfEndPoint = e.KilometerOfEndPoint,
                        Name = e.Name,
                        PressureMax = e.PressureMax,
                        CoefficientOfEfficiency = e.CoefficientOfEfficiency
                    }));
            var graphNew = new Graph(graph.SystemId)
            {
                EdgeList = edgesNew,
                VertexList = verticesNew
            };

            return graphNew;
        }

        private Graph Site(Graph graph, SiteDTO site)
        {
            var gr = new Graph(graph.SystemId)
            {
                EdgeList = new List<Edge>(),
                VertexList = new List<Vertex>(),
                Name = site.Name
            };

            var segments = new GetSiteSegmentListQuery(Context).Execute(new Guid?());
            segments.RemoveAll(s => s.SiteId != site.Id);
            var edgesTrue =
                graph.EdgeList.Where(
                    e =>
                        segments.Any(
                            s =>
                                s.PipelineId == e.EntityId && e.KilometerOfStartPoint >= s.KilometerOfStartPoint &&
                                e.KilometerOfEndPoint <= s.KilometerOfEndPoint)).ToList();

            var shops =
                new GetCompShopListQuery(Context).Execute(new GetCompShopListParameterSet
                {
                    StationIdList =
                        new GetCompStationListQuery(Context).Execute(new GetCompStationListParameterSet
                        {
                            SystemId = graph.SystemId,
                            SiteId = site.Id
                        }).Select(s => s.Id).ToList()
                }).ToList();

            var valves =
                new GetValveListQuery(Context).Execute(new GetValveListParameterSet
                {
                    SystemId = graph.SystemId,
                    SiteId = site.Id
                }).Select(v => v.Id).ToList();

            var redStat = new GetReducingStationListQuery(Context).Execute(new GetReducingStationListParameterSet
            {
                SystemId = graph.SystemId,
                SiteId = site.Id
            }).ToList();

            edgesTrue.AddRange(
                graph.EdgeList.Where(
                    e =>
                        shops.Any(s => s.Id == e.EntityId) || redStat.Any(r => r.Id == e.EntityId) ||
                        valves.Any(v => v == e.EntityId)).ToList());

            gr.EdgeList.AddRange(edgesTrue);
            foreach (var edge in edgesTrue)
            {
                if (!gr.VertexList.Contains(edge.V1))
                {
                    gr.VertexList.Add(edge.V1);
                }
                if (!gr.VertexList.Contains(edge.V2))
                {
                    gr.VertexList.Add(edge.V2);
                }
            }
            return gr;
        }

        private Vertex Root(Graph graph, Guid siteId)
        {
            var root = new Vertex();
            // Aleksandrovskoe LPU
            if (siteId == new Guid("2b8244b6-787f-634e-979d-70b6efbe60ac"))
            {
                root = graph.VertexList.Single(v => v.Id == 109);
            }
            // Tomskoe
            if (siteId == new Guid("fc13969a-b0cc-f24a-9bf3-dc6ec4fbb84e"))
            {
                root = graph.VertexList.Single(v => v.Id == 1624);
            }
            // Yurginskoe
            if (siteId == new Guid("540386f2-bda6-7647-9884-fb83c2604330"))
            {
                root = graph.VertexList.Single(v => v.Id == 252);
            }
            // Kemerovskoe
            if (siteId == new Guid("567fd95e-cc26-fb4f-8567-b2d0fa685cca"))
            {
                root = graph.VertexList.Single(v => v.Id == 1148);
            }
            // Novokuznetskoe
            if (siteId == new Guid("735b0c1d-e097-ce41-95d9-d3eb1d0a7d1e"))
            {
                root = graph.VertexList.Single(v => v.Id == 35);
            }
            // Omskoe
            if (siteId == new Guid("ee3f4b05-dbc3-314f-864a-a355a79aa4cf"))
            {
                root = graph.VertexList.Single(v => v.Id == 857);
            }
            // Barabinskoe
            if (siteId == new Guid("96f8d697-9c45-5a48-a3df-fc769fd9d569"))
            {
                root = graph.VertexList.Single(v => v.Id == 1084);
            }
            //Novosibirskoe
            if (siteId == new Guid("dcd970ac-abfd-dc46-8bf7-c8247942a501"))
            {
                root = graph.VertexList.Single(v => v.Id == 1668);
            }
            //Altaiskoe
            if (siteId == new Guid("037e415a-ebe7-3248-8578-31882f10654b"))
            {
                root = graph.VertexList.Single(v => v.Id == 1090);
            }

            return root;
        }

        private Dictionary<Vertex, Vertex> CoupleOfMeasLine(Graph graph)
        {
            const double deltaKm = 5.0;
            var couple = new Dictionary<Vertex, Vertex>();

            var signIn = new Dictionary<Vertex, PointOnPipe>();
            graph.VertexList.Where(v => v.Type == EntityType.MeasLine && v.BalanceSignId == Sign.In)
                .ToList()
                .ForEach(v => signIn.Add(v, PipelineId(graph, v)));

            var signOut = new Dictionary<Vertex, PointOnPipe>();
            graph.VertexList.Where(v => v.Type == EntityType.MeasLine && v.BalanceSignId == Sign.Out)
                .ToList()
                .ForEach(v => signOut.Add(v, PipelineId(graph, v)));

            foreach (var sIn in signIn)
            {
                var sOut =
                    signOut.SingleOrDefault(
                        o =>
                            o.Value.EntityId == sIn.Value.EntityId &&
                            Math.Abs(o.Value.Kilometer - sIn.Value.Kilometer) < deltaKm);
                if (sOut.Value == null)
                {
                    continue;
                }
                couple.Add(sIn.Key, sOut.Key);
            }

            return couple;
        }

        private PointOnPipe PipelineId(Graph graph, Vertex vertex)
        {
            var point = new PointOnPipe();
            var edge = graph.EdgeList.SingleOrDefault(e => e.V1.Id == vertex.Id);
            if (edge != null)
            {
                point.EntityId = edge.EntityId;
                point.Kilometer = edge.KilometerOfStartPoint;
            }
            else
            {
                edge = graph.EdgeList.SingleOrDefault(e => e.V2.Id == vertex.Id);
                point.EntityId = edge.EntityId;
                point.Kilometer = edge.KilometerOfEndPoint;
            }
            return point;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <returns></returns>
        private void SetForMeasLineQ(Graph graph)
        {
            var couple = CoupleOfMeasLine(graph);
            var sumQ =
                graph.VertexList.Where(
                    v => (v.BalanceSignId == Sign.In || v.BalanceSignId == Sign.Out) && v.Consumption.HasValue)
                    .Sum(v => v.Consumption);

            var unknownQIn = new List<Vertex>();
            var unknownQOut = new List<Vertex>();

            foreach (var c in couple)
            {
                if (!c.Key.Consumption.HasValue && !c.Value.Consumption.HasValue)
                {
                    unknownQIn.Add(c.Key);
                    unknownQOut.Add(c.Value);
                }
            }

            unknownQIn.AddRange(
                graph.VertexList.Where(
                    v =>
                        v.Type == EntityType.MeasLine && !v.Consumption.HasValue && v.BalanceSignId == Sign.In &&
                        couple.All(c => c.Key != v)));
            unknownQOut.AddRange(
                graph.VertexList.Where(
                    v =>
                        v.Type == EntityType.MeasLine && !v.Consumption.HasValue && v.BalanceSignId == Sign.Out &&
                        couple.All(c => c.Value != v)));

            if (sumQ > 0)
            {
                unknownQOut.ForEach(l => l.Consumption = (-1.0) * sumQ / unknownQOut.Count);
            }
            else
            {
                unknownQIn.ForEach(l => l.Consumption = (-1.0) * sumQ / unknownQIn.Count);
            }
        }

        private Dictionary<SiteDTO, int> AllSites(Guid enterpriseId, int systemId)
        {
            var order = new Dictionary<SiteDTO, int>();

            var sites =
                new GetSiteListQuery(Context).Execute(new GetSiteListParameterSet
                {
                    EnterpriseId = enterpriseId,
                    SystemId = systemId
                });

            var aleksandrovskoe = new Guid("2b8244b6-787f-634e-979d-70b6efbe60ac");
            var tomskoe = new Guid("fc13969a-b0cc-f24a-9bf3-dc6ec4fbb84e");
            var omskoe = new Guid("ee3f4b05-dbc3-314f-864a-a355a79aa4cf");
            var barabinskoe = new Guid("96f8d697-9c45-5a48-a3df-fc769fd9d569");
            var novosibirskoe = new Guid("dcd970ac-abfd-dc46-8bf7-c8247942a501");
            var altaiskoe = new Guid("037e415a-ebe7-3248-8578-31882f10654b");
            var yurginskoe = new Guid("540386f2-bda6-7647-9884-fb83c2604330");
            var kemerovskoe = new Guid("567fd95e-cc26-fb4f-8567-b2d0fa685cca");
            var novokuznetskoe = new Guid("735b0c1d-e097-ce41-95d9-d3eb1d0a7d1e");

            order.Add(sites.Single(s => s.Id == aleksandrovskoe), 1);
            order.Add(sites.Single(s => s.Id == novokuznetskoe), 1);
            order.Add(sites.Single(s => s.Id == omskoe), 1);
            order.Add(sites.Single(s => s.Id == altaiskoe), 1);
            order.Add(sites.Single(s => s.Id == tomskoe), 2);
            order.Add(sites.Single(s => s.Id == kemerovskoe), 2);
            order.Add(sites.Single(s => s.Id == barabinskoe), 2);
            order.Add(sites.Single(s => s.Id == yurginskoe), 4);
            order.Add(sites.Single(s => s.Id == novosibirskoe), 3);

            return order;
        }
    }
}
