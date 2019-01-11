using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Balances.Routes;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.Graph
{
    public class MinRoute
    {
        #region Поля
        private const double Inf = 1000 * 1000 * 1000;
        private readonly Graph _graph;

        /// <summary>
        /// Начальная вершина
        /// </summary>
        private readonly Vertex _vertexFrom;

        /// <summary>
        /// Список начальных вершин
        /// </summary>
        private readonly List<Vertex> _verticesFrom;

        /// <summary>
        /// Массив расстояний
        /// (расстояние от д. вершины до любой другой)
        /// </summary>
        private double[] _distance;

        /// <summary>
        /// Массив "предков"
        /// (храниться индекс предыдущей вершины в кратч. пути)
        /// </summary>
        //private int[] _p;

        private bool _any;

        #endregion

        #region Конструкторы

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph">Граф</param>
        /// <param name="vertexFrom">Начальная вершина</param>
        public MinRoute(Graph graph, Vertex vertexFrom)
        {
            _graph = graph;
            _vertexFrom = vertexFrom;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph">Граф</param>
        /// <param name="entityIdFrom">Id сущности начальной вершины</param>
        public MinRoute(Graph graph, Guid entityIdFrom)
        {
            _graph = graph; // DeleteCompShopEdges(graph);
            _verticesFrom = _graph.VertexList.Where(v => v.EntityId == entityIdFrom).ToList();
        }
        #endregion

        #region Funcs MinRoute
        /// <summary>
        /// Метод применяется, если точка начала пути задана вершиной графа
        /// </summary>
        /// <param name="vertexTo"></param>
        /// <returns></returns>
        public List<RouteSectionDTO> GetMinRoute(Vertex vertexTo)
        {
            var route = new List<RouteSectionDTO>();

            // Начало пути задано вершиной графа
            if (_vertexFrom == null)
            {
                return route;
            }
            var previousVertices = AlgorithmDijkstra(_vertexFrom);
            route = RouteRestoration(_vertexFrom, vertexTo, previousVertices);
            return route;
        }

        /// <summary>
        /// Метод применяется, если точка начала пути задана вершиной графа
        /// </summary>
        /// <param name="verticesTo"></param>
        /// <returns></returns>
        public List<List<RouteSectionDTO>> GetMinRoute(List<Vertex> verticesTo)
        {
            var routes = new List<List<RouteSectionDTO>>();

            if (_vertexFrom == null)
            {
                return routes;
            }
            var previousVertices = AlgorithmDijkstra(_vertexFrom);
            routes.AddRange(verticesTo.Select(vTo => RouteRestoration(_vertexFrom, vTo, previousVertices)));
            return routes;
        }

        /// <summary>
        /// Метод применяется, если точка начала пути задана entityId
        /// </summary>
        /// <param name="entityIdTo"></param>
        /// <returns></returns>
        public List<RouteSectionDTO> GetMinRoute(Guid entityIdTo)
        {
            var route = new List<RouteSectionDTO>();
            var verticesTo = _graph.VertexList.Where(v => v.EntityId == entityIdTo).ToList();

            if (_verticesFrom.Any() && verticesTo.Any())
            {
                foreach (var vFrom in _verticesFrom)
                {
                    var sections = new List<RouteSectionDTO>();
                    var previousVertices = AlgorithmDijkstra(vFrom);
                    foreach (var vTo in verticesTo)
                    {
                        var sectionsHelper = RouteRestoration(vFrom, vTo, previousVertices);
                        sections = sections.Any()
                                                   ? CompareTwoRoutes(sections, sectionsHelper)
                                                   : sectionsHelper;
                    }
                    route = route.Any() ? CompareTwoRoutes(route, sections) : sections;
                }
            }

            return route;
        }

        /// <summary>
        /// Метод применяется, если точка начала пути задана entityId
        /// </summary>
        /// <param name="entitiesIdTo"></param>
        /// <returns></returns>
        public List<List<RouteSectionDTO>> GetMinRoute(List<Guid> entitiesIdTo)
        {
            var routes = new List<List<RouteSectionDTO>>();

            if (_verticesFrom.Any() && entitiesIdTo.Any())
            {
                foreach (var vFrom in _verticesFrom)
                {
                    var routesHelper = new List<List<RouteSectionDTO>>();
                    var previousVertices = AlgorithmDijkstra(vFrom);

                    foreach (var entityIdTo in entitiesIdTo)
                    {
                        // Вершин(ы) с CompUnitId
                        // Если вершин несколько, то выбираем ту, до которой путь наименьший
                        var verticesTo = _graph.VertexList.Where(v => v.EntityId == entityIdTo).ToList();

                        if (verticesTo.Any())
                        {
                            var sections = new List<RouteSectionDTO>();
                            foreach (var vTo in verticesTo)
                            {
                                var sectionsHelper = RouteRestoration(vFrom, vTo, previousVertices);
                                sections = sections.Any() ? CompareTwoRoutes(sections, sectionsHelper) : sectionsHelper;
                            }
                            routesHelper.Add(sections);
                        }
                    }
                    routes = routes.Any() ? CompareSomeRoutes(routes, routesHelper) : routesHelper;
                }
            }

            return routes;
        }

        #region Восстановление пути
        /// <summary>
        /// Список вершин, из которых состоит минимальный путь
        /// </summary>
        /// <param name="vertexFrom">Начальная вершина</param>
        /// <param name="vertexTo">Конечная вершина</param>
        /// <param name="previousVertices"></param>
        /// <returns></returns>
        public List<Vertex> VerticesInMinRoute(Vertex vertexFrom, Vertex vertexTo, int[] previousVertices)
        {
            var path = new List<Vertex>();

            for (var curr = _graph.VertexList.IndexOf(vertexTo); curr != -1; curr = previousVertices[curr])
            {
                path.Add(_graph.VertexList[curr]);
            }
            path.Reverse();

            return path;
        }

        public List<List<Vertex>> VerticesInMinRoute(Vertex vertexFrom, List<Vertex> verticesTo, int[] previousVertices)
        {
            var paths = new List<List<Vertex>>();
            foreach (var vTo in verticesTo)
            {
                var path = new List<Vertex>();
                for (var curr = _graph.VertexList.IndexOf(vTo); curr != -1; curr = previousVertices[curr])
                {
                    path.Add(_graph.VertexList[curr]);
                }
                path.Reverse();
                paths.Add(path);
            }
            return paths;
        }

        /// <summary>
        /// Восстановление кратчайшего пути на основе машрута из последовательности вершин
        /// </summary>
        /// <param name="vertexFrom">Начальная вершина</param>
        /// <param name="vertexTo">Конечная вершина</param>
        /// <returns>Список сегментов в пути</returns>
        private List<RouteSectionDTO> RouteRestoration(Vertex vertexFrom, Vertex vertexTo, int[] previousVertices)
        {
            var vertices = VerticesInMinRoute(vertexFrom, vertexTo, previousVertices);
            var route = new List<RouteSectionDTO>();
            Edge edgeLast = null;

            for (var i = 0; i < vertices.Count - 1; i++)
            {
                var edges =
                    _graph.EdgeList.Where(
                        e =>
                        (e.V1 == vertices[i] && e.V2 == vertices[i + 1]) ||
                        (e.V1 == vertices[i + 1] && e.V2 == vertices[i])).ToList();

                var edge = edgeLast != null && edges.Count > 1
                               ? (edges.Where(e => e.EntityId == edgeLast.EntityId)).First()
                               : edges.First();

                double kmStart;
                double kmEnd;
                if (route.Count > 0)
                {
                    KilometersOfSectionsRoute(edgeLast, edge, out kmStart, out kmEnd);

                    if (route[route.Count - 1].PipelineId == edge.EntityId)
                    {
                        route[route.Count - 1].KilometerEnd = kmEnd;
                        edgeLast = edge;
                        continue;
                    }
                }
                else
                {
                    kmStart = edge.V1 == vertices[i] ? edge.KilometerOfStartPoint : edge.KilometerOfEndPoint;
                    kmEnd = edge.V1 == vertices[i] ? edge.KilometerOfEndPoint : edge.KilometerOfStartPoint;
                }
                route.Add(new RouteSectionDTO
                {
                    PipelineId = edge.EntityId,
                    KilometerStart = kmStart,
                    KilometerEnd = kmEnd,
                    PipelineName = edge.Name,
                    SortOrder = i
                });
                edgeLast = edge;
            }

            var sameSections = route.Where(r => route.Count(rr => rr.PipelineId == r.PipelineId) > 1).Distinct().ToList();
            if (sameSections.Any())
            {
                route = DeleteWasteSections(route);
            }
            return route;
        }

        /// <summary>
        /// Функция удаляет ненужные секции 
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        private List<RouteSectionDTO> DeleteWasteSections(List<RouteSectionDTO> route)
        {
            var newRoute = new List<RouteSectionDTO>();
            var k = 0;
            for (var i = 0; i < route.Count; i = k)
            {
                var sameSections =
                    route.Where(s => s.PipelineId == route[i].PipelineId && s.SortOrder > route[i].SortOrder).ToList();

                if (sameSections.Any())
                {
                    var lastSameSection = sameSections.Single(ss => ss.SortOrder == sameSections.Max(s => s.SortOrder));
                    k = route.IndexOf(lastSameSection) + 1;
                    newRoute.Add(new RouteSectionDTO
                    {
                        PipelineId = route[i].PipelineId,
                        KilometerStart = route[i].KilometerStart,
                        KilometerEnd = lastSameSection.KilometerEnd,
                        SortOrder = route[i].SortOrder
                    });
                }
                else
                {
                    newRoute.Add(route[i]);
                    k++;
                }
            }
            return newRoute;
        }

        /// <summary>
        /// Возвращает километры начала и конца участка пути
        /// </summary>
        /// <param name="edgeLast">Последнее ребро маршруте</param>
        /// <param name="edgeCurrent">Добавляемое ребро</param>
        /// <param name="kmStart">Км начала добавляемого участка маршрута</param>
        /// <param name="kmEnd">Км конца добавляемого участка маршрута</param>
        private void KilometersOfSectionsRoute(Edge edgeLast, Edge edgeCurrent, out double kmStart, out double kmEnd)
        {
            kmStart = -1;
            kmEnd = -1;

            if (edgeLast.EntityId != edgeCurrent.EntityId)
            {
                if (edgeCurrent.V1 == edgeLast.V2 || edgeCurrent.V1 == edgeLast.V1)
                {
                    kmStart = edgeCurrent.KilometerOfStartPoint;
                    kmEnd = edgeCurrent.KilometerOfEndPoint;
                }
                if (edgeCurrent.V2 == edgeLast.V1 || edgeCurrent.V2 == edgeLast.V2)
                {
                    kmStart = edgeCurrent.KilometerOfEndPoint;
                    kmEnd = edgeCurrent.KilometerOfStartPoint;
                }
            }
            else
            {
                if (edgeCurrent.KilometerOfStartPoint == edgeLast.KilometerOfEndPoint)
                {
                    kmStart = edgeCurrent.KilometerOfStartPoint;
                    kmEnd = edgeCurrent.KilometerOfEndPoint;
                }
                if (edgeCurrent.KilometerOfEndPoint == edgeLast.KilometerOfStartPoint)
                {
                    kmStart = edgeCurrent.KilometerOfEndPoint;
                    kmEnd = edgeCurrent.KilometerOfStartPoint;
                }
            }
        }
        #endregion

        /// <summary>
        /// Сравнение двух маршрутов
        /// </summary>
        /// <param name="routeFirst">Первый маршрут</param>
        /// <param name="routeSecond">Второй марщрут</param>
        /// <returns>Минимальный маршрут</returns>
        private List<RouteSectionDTO> CompareTwoRoutes(List<RouteSectionDTO> routeFirst,
                                                       List<RouteSectionDTO> routeSecond)
        {
            var length = routeFirst.Sum(s => Math.Abs(s.KilometerEnd.Value - s.KilometerStart.Value));
            if (routeSecond.Sum(s => Math.Abs(s.KilometerEnd.Value - s.KilometerStart.Value)) < length)
            {
                return routeSecond;
            }
            return routeFirst;
        }

        private List<List<RouteSectionDTO>> CompareSomeRoutes(List<List<RouteSectionDTO>> routesFirst,
                                                              List<List<RouteSectionDTO>> routesSecond)
        {
            for (var i = 0; i < routesFirst.Count; i++)
            {
                routesFirst[i] = CompareTwoRoutes(routesFirst[i], routesSecond[i]);
            }
            return routesFirst;
        }
        #endregion

        #region Алгоритм расчета кратчайшего пути (Дейкстра)
        /// <summary>
        /// Запуск алгоритма расчета (алгоритм Дейкстра)
        /// </summary>
        private int[] AlgorithmDijkstra(Vertex vertexFrom)
        {
            //Массив с вершинами-предками
            var prevVertex = new int[_graph.VertexList.Count];
            _distance = new double[_graph.VertexList.Count];
            _graph.VertexList.ForEach(v => v.IsChecked = false);

            for (var i = 0; i < _distance.GetLength(0); i++)
            {
                _distance[i] = Inf;
            }
            _distance[_graph.VertexList.IndexOf(vertexFrom)] = 0;

            for (var i = 0; i < prevVertex.GetLength(0); i++)
            {
                prevVertex[i] = -1;
            }

            OneStep(vertexFrom, prevVertex);
            foreach (var point in _graph.VertexList)
            {
                var anotherP = GetAnotherUncheckedPoint();
                if (anotherP != null)
                {
                    OneStep(anotherP, prevVertex);
                }
                else
                {
                    break;
                }
            }

            return prevVertex;
        }

        /// <summary>
        /// Метод, делающий один шаг алгоритма. Принимает на вход вершину.
        /// </summary>
        /// <param name="beginVertex"></param>
        /// <param name="prevVertex">Массив с вершинами-предками</param>
        private void OneStep(Vertex beginVertex, int[] prevVertex)
        {
            foreach (Vertex nextP in GetNeighbor(beginVertex))
            {
                if (nextP.IsChecked)
                {
                    continue;
                }
                var myEdge = GetMyEdge(nextP, beginVertex);
                var newMetka = _distance[_graph.VertexList.IndexOf(beginVertex)] +
                               (double)((decimal)myEdge.KilometerOfEndPoint - (decimal)myEdge.KilometerOfStartPoint);

                if (_distance[_graph.VertexList.IndexOf(nextP)] > newMetka)
                {
                    _distance[_graph.VertexList.IndexOf(nextP)] = newMetka;
                    prevVertex[_graph.VertexList.IndexOf(nextP)] = _graph.VertexList.IndexOf(beginVertex);
                }
            }
            beginVertex.IsChecked = true; // вычеркиваем
        }

        /// <summary>
        /// Поиск соседей для вершины. Для неориентированного графа ищутся все соседи.
        /// </summary>
        /// <param name="currVertex"></param>
        /// <returns></returns>
        private IEnumerable<Vertex> GetNeighbor(Vertex currVertex)
        {
            //var firstPoints = from ff in _graph.EdgeList where ff.V1 == currVertex select ff.V2;
            //var secondPoints = from sp in _graph.EdgeList where sp.V2 == currVertex select sp.V1;
            var totalPoints = new List<Vertex>();

            if (!currVertex.Pressure.HasValue)
            {
                return totalPoints;
            }
            if (currVertex.Type == EntityType.CompShop)
            {
                var edge =
                    _graph.EdgeList.Single(
                        e =>
                            e.EntityType == EntityType.CompShop &&
                            (e.V1.Id == currVertex.Id || e.V2.Id == currVertex.Id));

                if (edge.V1.Id == currVertex.Id)
                {
                    totalPoints.Add(edge.V2);
                }
                else
                {
                    var firstPoints = from ff in _graph.EdgeList
                                      where (ff.V1 == currVertex && ff.V2.Type != EntityType.CompShop)
                                      select ff.V2;
                    var secondPoints = from sp in _graph.EdgeList
                                       where (sp.V2 == currVertex && sp.V1.Type != EntityType.CompShop)
                                       select sp.V1;
                    totalPoints = firstPoints.Concat(secondPoints).ToList();
                }
            }
            else
            {
                var firstPoints = from ff in _graph.EdgeList
                                  where
                                      (ff.V1 == currVertex && ff.V1.Pressure.HasValue &&
                                       ff.V1.Pressure.Value <= currVertex.Pressure.Value)
                                  select ff.V2;

                var secondPoints = from sp in _graph.EdgeList
                                   where
                                       (sp.V2 == currVertex && sp.V2.Pressure.HasValue &&
                                        sp.V2.Pressure.Value <= currVertex.Pressure.Value)
                                   select sp.V1;
                totalPoints = firstPoints.Concat(secondPoints).ToList();
            }

            return totalPoints;
        }

        /// <summary>
        /// Получаем ребро, соединяющее две входные точки
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private Edge GetMyEdge(Vertex a, Vertex b)
        {
            var edges = _graph.EdgeList.Where(e => (e.V1 == a & e.V2 == b) ||
                                                        (e.V2 == a & e.V1 == b)).ToList();
            var edge = edges.First();
            if (edges.Count > 1)
            {
                foreach (var edge1 in edges)
                {
                    if (edge == edge1)
                    {
                        continue;
                    }
                    if ((edge1.KilometerOfEndPoint - edge1.KilometerOfStartPoint) < (edge.KilometerOfEndPoint - edge.KilometerOfStartPoint))
                    {
                        edge = edge1;
                    }
                }
            }
            return edge;
        }

        /// <summary>
        /// Получаем очередную неотмеченную вершину, ближайшую к заданной.
        /// </summary>
        /// <returns></returns>
        private Vertex GetAnotherUncheckedPoint()
        {
            var pointsUncheck = _graph.VertexList.Where(p => p.IsChecked == false).ToList();
            if (!pointsUncheck.Any())
            {
                return null;
            }

            var minValueMetka = _distance[_graph.VertexList.IndexOf(pointsUncheck.First())]; //pointsUncheck.First().ValueMetka;
            var minPoint = pointsUncheck.First();
            foreach (var p in pointsUncheck)
            {
                if (_distance[_graph.VertexList.IndexOf(p)] < minValueMetka)
                {
                    minValueMetka = _distance[_graph.VertexList.IndexOf(p)];
                    minPoint = p;
                }
            }
            return minPoint;
        }
        #endregion
    }
}