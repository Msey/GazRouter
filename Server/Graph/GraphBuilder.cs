using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.Core;
using GazRouter.DAL.Dictionaries.ValveTypes;
using GazRouter.DAL.ObjectModel.CompShops;
using GazRouter.DAL.ObjectModel.CompStations;
using GazRouter.DAL.ObjectModel.DistrStations;
using GazRouter.DAL.ObjectModel.MeasLine;
using GazRouter.DAL.ObjectModel.MeasStations;
using GazRouter.DAL.ObjectModel.PipelineConns;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DAL.ObjectModel.ReducingStations;
using GazRouter.DAL.ObjectModel.Sites;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineEndType;
using GazRouter.DTO.ObjectModel.CompStations;
using GazRouter.DTO.ObjectModel.MeasLine;
using GazRouter.DTO.ObjectModel.MeasStations;
using GazRouter.DTO.ObjectModel.Pipelines;
using GazRouter.DTO.ObjectModel.PipelineConns;
using GazRouter.DAL.ObjectModel.Segment.Diameter;
using GazRouter.DAL.ObjectModel.Segment.Pressure;
using GazRouter.DAL.ObjectModel.Segment.Site;
using GazRouter.DAL.ObjectModel.Valves;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.PipelineTypes;
using GazRouter.DTO.Dictionaries.ValveTypes;
using GazRouter.DTO.ObjectModel.CompShops;
using GazRouter.DTO.ObjectModel.DistrStations;
using GazRouter.DTO.ObjectModel.ReducingStations;
using GazRouter.DTO.ObjectModel.Sites;
using GazRouter.DTO.ObjectModel.Valves;

namespace GazRouter.Graph
{
    public class GraphBuilder
    {
        private Graph _graph;
        private int _systemId;
        private Guid _enterpriseId;
        private ExecutionContext _context;
        private int _maxVertexId;

        /// <summary>
        /// Список пайпов в выбранной ГТС
        /// </summary>
        private IEnumerable<PipelineDTO> _pipelines;

        /// <summary>
        /// Список подключений в выбранной ГТС
        /// </summary>
        private IEnumerable<PipelineConnDTO> _pipelineConns;

        /// <summary>
        /// Список ЛПУ в выбранном предприятии и ГТС
        /// </summary>
        private IEnumerable<SiteDTO> _sites;

        /// <summary>
        /// Список ЗЛ в выбранном предприятии и ГТС
        /// </summary>
        private IEnumerable<MeasLineDTO> _measLines;

        /// <summary>
        /// Список КЦ в выбранном предприятии и ГТС
        /// </summary>
        private IEnumerable<CompShopDTO> _compShops;

        ///// <summary>
        ///// Список ГРС в выбранном предприятии и ГТС
        ///// </summary>
        //private IEnumerable<DistrStationDTO> _distrStations;

        /// <summary>
        /// Список кранов в выбранной ГТС
        /// </summary>
        private List<ValveDTO> _valves;

        public GraphBuilder(int systemId, Guid enterpriseId, ExecutionContext context)
        {
            _systemId = systemId;
            _enterpriseId = enterpriseId;
            _context = context;
            _graph = new Graph(_systemId) { VertexList = new List<Vertex>(), EdgeList = new List<Edge>() };

            GetObjects();
            _maxVertexId = 0;
        }

        /// <summary>
        /// Построение графа
        /// </summary>
        /// <returns></returns>
        public Graph BuildGraph()
        {
            // Сначала добавим все пайпы. Вершины ребер будут формироваться как т. подключения другого пайпа или границы сегментов по диаметрам и давлению
            // или км начала и км конца газопровода
            FrameOfGraph();

            // Добавим ЗЛ
            AddMeasLines();

            // Объединим места соединения пайпов
            UnionSameVertices();

            // Добавление типов объектов (КЦ, ГРС) к имеющимся вершинам в графе 
            AddCompShop();
            AddDistrStation();
            AddValve();

            return _graph;
        }

        /// <summary>
        /// Получение всех объектов модели, необходимых для построения графа
        /// </summary>
        private void GetObjects()
        {
            _pipelines = new GetPipelineListQuery(_context).Execute(new GetPipelineListParameterSet { SystemId = _systemId });
            _pipelineConns = new GetPipelineConnListQuery(_context).Execute(new GetPipelineConnListParameterSet
            {
                GasTrasportSystemId
                    =
                    _systemId
            });

            _sites = new GetSiteListQuery(_context).Execute(new GetSiteListParameterSet
            {
                EnterpriseId = _enterpriseId,
                SystemId = _systemId
            });

            var measStations = new List<Guid>();
            foreach (SiteDTO site in _sites)
            {
                measStations.AddRange(
                    new GetMeasStationListQuery(_context).Execute(new GetMeasStationListParameterSet
                    {
                        SiteId = site.Id,
                        SystemId =
                                                                              _systemId
                    })
                                                         .Select(s => s.Id));
            }
            _measLines =
                new GetMeasLineListQuery(_context).Execute(new GetMeasLineListParameterSet
                {
                    MeasStationList =
                                                                       measStations,
                    SystemId = _systemId
                });

            var compStations = new List<CompStationDTO>();
            foreach (var site in _sites)
            {
                compStations.AddRange(new GetCompStationListQuery(_context).Execute(new GetCompStationListParameterSet
                {
                    SiteId = site.Id,
                    SystemId = _systemId
                }));
            }
            _compShops = new GetCompShopListQuery(_context).Execute(
                new GetCompShopListParameterSet
                {
                    StationIdList = compStations.Select(s => s.Id).ToList(),
                    SystemId = _systemId
                });

            _valves = new GetValveListQuery(_context).Execute(new GetValveListParameterSet { SystemId = _systemId });
        }

        /// <summary>
        /// "Скелет" графа
        /// Заведение ребер и вершин для каждой трубы отдельно.
        /// Нет общих точек для труб
        /// </summary>
        private void FrameOfGraph()
        {
            if (!_pipelines.Any())
            {
                return;
            }

            foreach (var pipe in _pipelines)
            {
                var kilometers = Kilometers(pipe);

                var v1 = new Vertex { Id = _maxVertexId };
                _graph.VertexList.Add(v1);
                _maxVertexId += 1;

                for (var i = 0; i < kilometers.Count - 1; i++)
                {
                    var v2 = new Vertex { Id = _maxVertexId };

                    var edge = new Edge(v1, v2)
                    {
                        EntityId = pipe.Id,
                        EntityType = EntityType.Pipeline,
                        KilometerOfStartPoint = kilometers[i],
                        KilometerOfEndPoint = kilometers[i + 1]
                        //, Diameter =
                        //DiameterOfEdge(pipe.Id, kilometers[i], kilometers[i + 1]),
                        //PressureMax =
                        //    PressureMaxOfEdge(pipe.Id, kilometers[i], kilometers[i + 1])
                    };

                    _graph.VertexList.Add(v2);
                    _maxVertexId += 1;
                    _graph.EdgeList.Add(edge);

                    v1 = v2;
                }
            }
        }

        #region Kilometers
        /// <summary>
        /// Функция возвращает отсортированный список километров подключения к трубе,
        /// километров изменения сегментов по диаметру и давлению
        /// </summary>
        /// <param name="pipeline">Газопровод</param>
        /// <returns></returns>
        private List<double> Kilometers(PipelineDTO pipeline)
        {
            var kmList = new List<double>
                             {
                                 pipeline.KilometerOfStartPoint,// Километр начала газопровода
                                 (double)
                                 ((decimal) pipeline.KilometerOfStartPoint + (decimal) pipeline.Length) // Километр конца газопровода
                             };

            // Километры подключения других газопроводов к рассматриваемому
            if (_pipelineConns.Any())
            {
                foreach (var pipelineConn in _pipelineConns)
                {
                    if (pipelineConn.DestPipelineId != pipeline.Id || !pipelineConn.Kilometr.HasValue)
                    {
                        continue;
                    }
                    if (kmList.All(km => km != pipelineConn.Kilometr))
                    {
                        kmList.Add(pipelineConn.Kilometr.Value);
                    }
                }
            }

            //// Километры изменения сегментов по диаметру
            //var diameterKms = KilometersOfStartPointDiameterSegment(pipeline.Id);
            //foreach (var diameterKm in diameterKms.Where(diameterKm => kmList.All(km => km != diameterKm)))
            //    kmList.Add(diameterKm);

            //// Километры изменения сегментов по давлению
            //var pressureKms = KilometersOfStartPointPressureSegment(pipeline.Id);
            //foreach (var pressureKm in pressureKms.Where(pressureKm => kmList.All(km => km != pressureKm)))
            //    kmList.Add(pressureKm);

            kmList.Sort();
            return kmList;
        }

        /// <summary>
        /// Километры начала сегментов по диаметру
        /// </summary>
        /// <param name="pipelineId"></param>
        private List<double> KilometersOfStartPointDiameterSegment(Guid pipelineId)
        {
            var diameterSegments = new GetDiameterSegmentListQuery(_context).Execute(pipelineId);
            return !diameterSegments.Any()
                ? null
                : diameterSegments.Select(diameterSegment => diameterSegment.KilometerOfStartPoint).ToList();
        }

        /// <summary>
        /// Километры начала сегментов по давлению
        /// </summary>
        /// <param name="pipelineId"></param>
        private List<double> KilometersOfStartPointPressureSegment(Guid pipelineId)
        {
            var pressureSegments = new GetPressureSegmentListQuery(_context).Execute(pipelineId);
            return !pressureSegments.Any()
                ? null
                : pressureSegments.Select(pressureSegment => pressureSegment.KilometerOfStartPoint).ToList();
        }

        /// <summary>
        /// Подключение кранов
        /// </summary>
        private List<double> AddValve()
        {
            if (!_valves.Any())
            {
                return null;
            }

            var kmList = new List<double>();
            var valveTypeList = new GetValveTypesListQuery(_context).Execute();
            foreach (var valve in _valves)
            {
                decimal kmValveStart;
                if (valve.Kilometer > 0.0005)
                {
                    kmValveStart = (decimal)valve.Kilometer - (decimal)0.0005;
                }
                else
                {
                    kmValveStart = (decimal)valve.Kilometer;
                }

                var kmValveEnd = (decimal)valve.Kilometer + (decimal)0.0005;

                var edges =
                    _graph.EdgeList.Where(
                        e => e.EntityId == valve.ParentId.Value).ToList();
                var edge = edges.Single(e =>
                    e.KilometerOfStartPoint <= (double)kmValveStart &&
                    e.KilometerOfEndPoint >= (double)kmValveEnd);

                var vFrom = edge.V1;
                var vTo = edge.V2;

                var vFromValve = new Vertex { Id = _maxVertexId };
                _maxVertexId += 1;
                var vToValve = new Vertex { Id = _maxVertexId };
                _maxVertexId += 1;
                _graph.VertexList.AddRange(new List<Vertex> { vFromValve, vToValve });

                _graph.EdgeList.Add(new Edge(vFrom, vFromValve)
                {
                    KilometerOfStartPoint = edge.KilometerOfStartPoint,
                    KilometerOfEndPoint = (double)kmValveStart,
                    Diameter = edge.Diameter,
                    EntityId = edge.EntityId,
                    EntityType = edge.EntityType,
                    PressureMax = edge.PressureMax
                });

                _graph.EdgeList.Add(new Edge(vFromValve, vToValve)
                {
                    KilometerOfStartPoint = (double)kmValveStart,
                    KilometerOfEndPoint = (double)kmValveEnd,
                    Diameter = valveTypeList.Single(type => type.Id == valve.ValveTypeId).DiameterReal,
                    EntityId = valve.Id,
                    EntityType = EntityType.Valve,
                    PressureMax = edge.PressureMax
                });

                _graph.EdgeList.Add(new Edge(vToValve, vTo)
                {
                    KilometerOfStartPoint = (double)kmValveEnd,
                    KilometerOfEndPoint = edge.KilometerOfEndPoint,
                    Diameter = edge.Diameter,
                    EntityId = edge.EntityId,
                    EntityType = edge.EntityType,
                    PressureMax = edge.PressureMax
                });

                _graph.EdgeList.Remove(edge);

                if (valve.Bypass1TypeId.HasValue)
                {
                    _graph.EdgeList.Add(new Edge(vFromValve, vToValve)
                    {
                        KilometerOfStartPoint = (double)kmValveStart,
                        KilometerOfEndPoint = (double)kmValveEnd,
                        Diameter = valveTypeList.Single(type => type.Id == valve.Bypass1TypeId.Value).DiameterReal,
                        EntityId = valve.Id,
                        EntityType = EntityType.Valve,
                        PressureMax = edge.PressureMax
                    });
                }

                if (valve.Bypass2TypeId.HasValue)
                {
                    _graph.EdgeList.Add(new Edge(vFromValve, vToValve)
                    {
                        KilometerOfStartPoint = (double)kmValveStart,
                        KilometerOfEndPoint = (double)kmValveEnd,
                        Diameter = valveTypeList.Single(type => type.Id == valve.Bypass2TypeId.Value).DiameterReal,
                        EntityId = valve.Id,
                        EntityType = EntityType.Valve,
                        PressureMax = edge.PressureMax
                    });
                }

                if (valve.Bypass3TypeId.HasValue)
                {
                    _graph.EdgeList.Add(new Edge(vFromValve, vToValve)
                    {
                        KilometerOfStartPoint = (double)kmValveStart,
                        KilometerOfEndPoint = (double)kmValveEnd,
                        Diameter = valveTypeList.Single(type => type.Id == valve.Bypass3TypeId.Value).DiameterReal,
                        EntityId = valve.Id,
                        EntityType = EntityType.Valve,
                        PressureMax = edge.PressureMax
                    });
                }
            }
            return kmList;
        }

        #region Сегменты по давлению, диаметру
        /// <summary>
        /// Диаметр ребра
        /// </summary>
        /// <param name="pipelineId">Газопровод, на котором находится ребро</param>
        /// <param name="kilometerOfStartPoint">Километр начала ребра</param>
        /// <param name="kilometerOfEndPoint">Километр конца ребра</param>
        /// <returns></returns>
        private double DiameterOfEdge(Guid pipelineId, double kilometerOfStartPoint, double kilometerOfEndPoint)
        {
            var diameterSegments = new GetDiameterSegmentListQuery(_context).Execute(pipelineId);

            var diameter = diameterSegments.Single(
                d =>
                d.KilometerOfStartPoint <= kilometerOfStartPoint &&
                d.KilometerOfEndPoint >= kilometerOfStartPoint &&
                d.KilometerOfStartPoint <= kilometerOfEndPoint &&
                d.KilometerOfEndPoint >= kilometerOfEndPoint)
                                           .DiameterConv;

            return diameter;
        }

        /// <summary>
        /// Максимальное разрешенное давление на ребре
        /// </summary>
        /// <param name="pipelineId">Газопровод, на котором находится ребро</param>
        /// <param name="kilometerOfStartPoint">Километр начала ребра</param>
        /// <param name="kilometerOfEndPoint">Километр конца ребра</param>
        /// <returns></returns>
        private double PressureMaxOfEdge(Guid pipelineId, double kilometerOfStartPoint, double kilometerOfEndPoint)
        {
            var pressureSegments = new GetPressureSegmentListQuery(_context).Execute(pipelineId);

            var pressure = pressureSegments.Single(
                d =>
                d.KilometerOfStartPoint <= kilometerOfStartPoint &&
                d.KilometerOfEndPoint >= kilometerOfStartPoint &&
                d.KilometerOfStartPoint <= kilometerOfEndPoint &&
                d.KilometerOfEndPoint >= kilometerOfEndPoint).Pressure;

            return pressure;
        }
        #endregion
        #endregion

        #region Замерные линии
        /// <summary>
        /// Учет замерных линий
        /// </summary>
        private void AddMeasLines()
        {
            if (!_measLines.Any())
            {
                return;
            }

            foreach (var measLine in _measLines)
            {
                var km = measLine.KmOfConn;
                var pipeId = measLine.PipelineId;
                var edges = _graph.EdgeList.Where(e => e.EntityId == pipeId).ToList();
                Edge edge;

                //Три случая расположения ЗЛ:
                // 1 - ЗЛ совпадает с началом ребра
                if ((edge = edges.SingleOrDefault(e => e.KilometerOfStartPoint == km)) != null)
                {
                    edge.V1.Type = EntityType.MeasLine;
                    edge.V1.EntityId = measLine.Id;
                    continue;
                }

                // 2 - ЗЛ совпадает с концом ребра
                if ((edge = edges.SingleOrDefault(e => e.KilometerOfEndPoint == km)) != null)
                {
                    edge.V2.Type = EntityType.MeasLine;
                    edge.V2.EntityId = measLine.Id;
                    continue;
                }

                // 3 - ЗЛ не совпадает ни с одной вершины ребер
                if ((edge =
                    edges.SingleOrDefault(e => e.KilometerOfStartPoint < km && e.KilometerOfEndPoint > km)) != null)
                {
                    AddMeasLinesHelper(measLine, edge);
                }
            }
        }

        /// <summary>
        /// Добавление ЗЛ в случае, когда ЗЛ находится между вершинами ребра
        /// </summary>
        /// <param name="measLine"></param>
        /// <param name="edge"></param>
        private void AddMeasLinesHelper(MeasLineDTO measLine, Edge edge)
        {
            // Добавление новой вершины - ЗЛ
            var v = new Vertex
            {
                Id = _maxVertexId,
                Type = EntityType.MeasLine,
                EntityId = measLine.Id
            };
            _graph.VertexList.Add(v);
            _maxVertexId += 1;

            // Разделение исходного ребра на два с общей вершиной - ЗЛ
            _graph.EdgeList.Add(new Edge(edge.V1, v)
            {
                EntityId = edge.EntityId,
                EntityType = EntityType.Pipeline,
                Diameter = edge.Diameter,
                PressureMax = edge.PressureMax,
                KilometerOfStartPoint = edge.KilometerOfStartPoint,
                KilometerOfEndPoint = measLine.KmOfConn
            });

            _graph.EdgeList.Add(new Edge(v, edge.V2)
            {
                EntityId = edge.EntityId,
                EntityType = EntityType.Pipeline,
                Diameter = edge.Diameter,
                PressureMax = edge.PressureMax,
                KilometerOfStartPoint = measLine.KmOfConn,
                KilometerOfEndPoint = edge.KilometerOfEndPoint
            });

            _graph.EdgeList.Remove(edge);
        }
        #endregion

        #region Компрессорный цех
        /// <summary>
        /// Подключение КЦ 
        /// </summary>
        private void AddCompShop()
        {
            if (!_compShops.Any())
            {
                return;
            }

            foreach (var compShop in _compShops)
            {
                var conns = _pipelineConns.Where(c => c.CompShopId == compShop.Id).ToList();
                // Не ко всем КЦ проведены газопроводы (напр., на Томске к КС Чанской и КС Ивановская)
                if (!conns.Any())
                {
                    continue;
                }

                //В случае если подведено к КЦ не по одному входному и выходному газопроводу
                var connEndTypeFirst = conns.FirstOrDefault(c => c.EndTypeId == PipelineEndType.EndType);
                Vertex vFrom;
                Edge edge;
                ConnectionPipelineToEntity(out vFrom, out edge, connEndTypeFirst);
                vFrom.EntityId = compShop.ParentId;
                var kmStart = _pipelines.Single(p => p.Id == edge.EntityId).KilometerOfBeginConn.Value;

                var connStartTypeFirst = conns.FirstOrDefault(c => c.EndTypeId == PipelineEndType.StartType);
                Vertex vTo;
                ConnectionPipelineToEntity(out vTo, out edge, connStartTypeFirst);
                var kmEnd = _pipelines.Single(p => p.Id == edge.EntityId).KilometerOfEndConn.Value;

                _graph.EdgeList.Add(new Edge(vFrom, vTo)
                {
                    EntityId = compShop.Id,
                    EntityType = EntityType.CompShop,
                    KilometerOfStartPoint = kmStart,
                    KilometerOfEndPoint = kmEnd
                });

                // Подключение остальных шлейфов
                foreach (var conn in conns)
                {
                    if (conn == connEndTypeFirst)
                    {
                        continue;
                    }
                    if (conn == connStartTypeFirst)
                    {
                        continue;
                    }

                    Vertex v;

                    ConnectionPipelineToEntity(out v, out edge, conn);
                    _graph.VertexList.Remove(v);
                    if (conn.EndTypeId == PipelineEndType.StartType)
                    {
                        edge.V1 = v;
                    }
                    if (conn.EndTypeId == PipelineEndType.EndType)
                    {
                        v.EntityId = compShop.ParentId;
                        edge.V2 = v;
                    }

                }
            }
        }

        /// <summary>
        /// Функция возвращает вершину, ребро подключения газопровода к другому объекту
        /// </summary>
        /// <param name="vertex">Вершина подключения</param>
        /// <param name="edge">Ребро подключения</param>
        /// <param name="conn">Подключение газопровода-КЦ/газопровода-ГРС/газопровод-газопровод</param>
        private void ConnectionPipelineToEntity(out Vertex vertex, out Edge edge, PipelineConnDTO conn)
        {
            vertex = new Vertex();
            edge = new Edge(vertex, vertex);
            double kilometer;

            var pipe = _pipelines.Single(p => p.Id == conn.PipelineId);

            // Тип подключения - StartType 
            if (conn.EndTypeId == PipelineEndType.StartType)
            {
                kilometer = pipe.KilometerOfStartPoint;

                foreach (var e in _graph.EdgeList)
                {
                    if (e.EntityId == conn.PipelineId)
                    {
                        if (e.KilometerOfStartPoint == kilometer)
                        {
                            edge = e;
                            vertex = e.V1;
                        }
                    }
                }
            }

            // Тип подключения - EndType 
            if (conn.EndTypeId == PipelineEndType.EndType)
            {
                kilometer = (double)((decimal)pipe.KilometerOfStartPoint + (decimal)pipe.Length);

                foreach (var e in _graph.EdgeList)
                {
                    if (e.EntityId == conn.PipelineId)
                    {
                        if (e.KilometerOfEndPoint == kilometer)
                        {
                            edge = e;
                            vertex = e.V2;
                        }
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Подключение ГРС
        /// </summary>
        private void AddDistrStation()
        {
            if (!_pipelineConns.Any())
            {
                return;
            }

            foreach (var conn in _pipelineConns)
            {
                if (!conn.DistrStationId.HasValue)
                {
                    continue;
                }

                Vertex vertex;
                Edge edge;
                ConnectionPipelineToEntity(out vertex, out edge, conn);
                vertex.Type = EntityType.DistrStation;
                vertex.EntityId = conn.DistrStationId.Value;
            }
        }

        #region Пересечение газопровод
        /// <summary>
        /// Объединение вершин - соединений газопроводов
        /// </summary>
        private void UnionSameVertices()
        {
            if (!_pipelineConns.Any())
            {
                return;
            }

            var allConnectionPipeline = _pipelineConns.Where(c => c.DestPipelineId.HasValue);

            foreach (var connect in allConnectionPipeline)
            {
                var vertexOnDestPipeline = IntersectionPoint2(connect);
                Vertex v;
                Edge edge;

                ConnectionPipelineToEntity(out v, out edge, connect);

                _graph.VertexList.Remove(v);
                if (edge.V1 == v)
                {
                    edge.V1 = vertexOnDestPipeline;
                }
                else
                {
                    edge.V2 = vertexOnDestPipeline;
                }
            }
        }

        /// <summary>
        /// Точка пересечения - вторая
        /// Точка на газопроводе, к которому подключается
        /// </summary>
        /// <param name="connect"></param>
        private Vertex IntersectionPoint2(PipelineConnDTO connect)
        {
            Vertex vertexOnDestPipeline;
            var edge = _graph.EdgeList.FirstOrDefault(
                e =>
                e.EntityId == connect.DestPipelineId &&
                e.KilometerOfStartPoint == connect.Kilometr);

            if (edge != null)
            {
                vertexOnDestPipeline = edge.V1;
            }

            else
            {
                vertexOnDestPipeline = _graph.EdgeList.First(
                    e =>
                    e.EntityId == connect.DestPipelineId &&
                    e.KilometerOfEndPoint == connect.Kilometr).V2;
            }
            return vertexOnDestPipeline;
        }
        #endregion
    }

    public class GraphBuilderNew
    {
        private Graph _graph;
        private int _systemId;
        private Guid _enterpriseId;
        private ExecutionContext _context;

        /// <summary>
        /// Список пайпов в выбранной ГТС
        /// </summary>
        private IEnumerable<PipelineDTO> _pipelines;

        /// <summary>
        /// Список подключений в выбранной ГТС
        /// </summary>
        private IEnumerable<PipelineConnDTO> _pipelineConns;

        /// <summary>
        /// Список ЛПУ в выбранном предприятии и ГТС
        /// </summary>
        private IEnumerable<SiteDTO> _sites;

        /// <summary>
        /// Список ЗЛ в выбранном предприятии и ГТС
        /// </summary>
        private IEnumerable<MeasLineDTO> _measLines;

        /// <summary>
        /// Список КЦ в выбранном предприятии и ГТС
        /// </summary>
        private IEnumerable<CompShopDTO> _compShops;

        /// <summary>
        /// Список КЦ в выбранном предприятии и ГТС
        /// </summary>
        private List<DistrStationDTO> _distrStations;

        /// <summary>
        /// Список ПРГ в выбранном предприятии и ГТС 
        /// </summary>
        private List<ReducingStationDTO> _reducingStations;

        /// <summary>
        /// Список кранов в выбранной ГТС
        /// </summary>
        private List<ValveDTO> _valves;

        private List<ValveTypeDTO> _valveTypes;

        private int _lastVertexId;

        public GraphBuilderNew(int systemId, Guid enterpriseId, ExecutionContext context)
        {
            _systemId = systemId;
            _enterpriseId = enterpriseId;
            _context = context;
            _graph = new Graph(_systemId) { VertexList = new List<Vertex>(), EdgeList = new List<Edge>() };

            GetObjects();
            _lastVertexId = 1;
        }

        public List<PointOnPipe> Points(PipelineDTO pipeline)
        {
            var points = new List<PointOnPipe>();
            const double valveLength = 0.001;

            // Подключение газопровода к ГРС, КЦ
            points.AddRange(from connect in _pipelineConns
                            where
                                connect.PipelineId == pipeline.Id &&
                                (connect.CompShopId.HasValue || connect.DistrStationId.HasValue)
                            select new PointOnPipe
                            {
                                Kilometer = KmConnection(connect),
                                Name = connect.Description,
                                EntityId = connect.CompShopId ?? connect.DistrStationId.Value,
                                EntityType = connect.CompShopId.HasValue ? EntityType.CompShop : EntityType.DistrStation
                            });

            // Подключение ЗЛ
            points.AddRange(_measLines.Where(l => l.PipelineId == pipeline.Id).Select(measLine => new PointOnPipe
            {
                Kilometer = measLine.KmOfConn,
                EntityId = measLine.Id,
                EntityType = EntityType.MeasLine,
                Name = measLine.Name
            }));

            // Подключение ПРГ
            points.AddRange(
                _reducingStations.Where(r => r.PipelineId == pipeline.Id).SelectMany(r => new List<PointOnPipe>
                {
                    new PointOnPipe
                    {
                        Kilometer = r.Kilometer - valveLength/2.0,
                        EntityId = r.Id,
                        EntityType = EntityType.ReducingStation,
                        Name = r.Name
                    },
                    new PointOnPipe
                    {
                        Kilometer = r.Kilometer + valveLength/2.0,
                        EntityId = r.Id,
                        EntityType = EntityType.ReducingStation,
                        Name = r.Name
                    }
                }));

            // Подключение кранов
            points.AddRange(
                    _valves.Where(v => v.ParentId.Value == pipeline.Id).SelectMany(valve => new List<PointOnPipe>
                    {
                        new PointOnPipe
                        {
                            Kilometer = valve.Kilometer - valveLength/2.0,
                            EntityId = valve.Id,
                            EntityType = EntityType.Valve,
                            Name = valve.Name
                        },
                        new PointOnPipe
                        {
                            Kilometer = valve.Kilometer + valveLength/2.0,
                            EntityId = valve.Id,
                            EntityType = EntityType.Valve,
                            Name = valve.Name
                        }
                    }));

            // Подключение к газопроводу других газопроводов
            foreach (var conn in _pipelineConns.Where(conn => conn.DestPipelineId.HasValue && conn.DestPipelineId.Value == pipeline.Id))
            {
                if (points.All(p => p.Kilometer != conn.Kilometr.Value))
                {
                    points.Add(new PointOnPipe { Kilometer = conn.Kilometr.Value });
                }
            }

            // Начало и конец газопровода
            if (points.All(p => p.Kilometer != pipeline.KilometerOfStartPoint))
            {
                points.Add(new PointOnPipe { Kilometer = pipeline.KilometerOfStartPoint });
            }
            if (points.All(p => p.Kilometer != pipeline.KilometerOfEndPoint))
            {
                points.Add(new PointOnPipe { Kilometer = pipeline.KilometerOfEndPoint });
            }

            // Сегменты газопровода по диаметру
            foreach (
                var dmSegment in
                    new GetDiameterSegmentListQuery(_context).Execute(pipeline.Id)
                        .Where(dmSgm => points.All(p => p.Kilometer != dmSgm.KilometerOfStartPoint)))
            {
                points.Add(new PointOnPipe { Kilometer = dmSegment.KilometerOfStartPoint });
            }

            // Сегменты газопровода по давлению
            foreach (
                var prSegment in
                    new GetPressureSegmentListQuery(_context).Execute(pipeline.Id)
                        .Where(prSgm => points.All(p => p.Kilometer != prSgm.KilometerOfStartPoint)))
            {
                points.Add(new PointOnPipe { Kilometer = prSegment.KilometerOfStartPoint });
            }

            // Сегменты газопровода по ЛПУ
            foreach (
                var siteSegment in
                    new GetSiteSegmentListQuery(_context).Execute(pipeline.Id)
                        .Where(sSgm => points.All(p => p.Kilometer != sSgm.KilometerOfStartPoint)))
            {
                points.Add(new PointOnPipe { Kilometer = siteSegment.KilometerOfStartPoint });
            }

            points.Sort();
            return points;
        }

        /// <summary>
        /// Построение графа
        /// </summary>
        /// <returns></returns>
        public Graph BuildGraph()
        {
            if (!_pipelines.Any())
            {
                return null;
            }

            foreach (var pipeline in _pipelines)
            {
                var points = Points(pipeline);

                // Первая вершина
                var v1 = new Vertex
                {
                    Id = _lastVertexId,
                    EntityId = points[0].EntityId,
                    Type = points[0].EntityType,
                    Name = points[0].Name
                };
                _lastVertexId += 1;
                _graph.VertexList.Add(v1);

                // Вершины, начиная со второй
                for (var i = 1; i < points.Count; i++)
                {
                    Guid eEntityId;
                    EntityType eEntityType;
                    string eName;

                    if ((points[i - 1].EntityType == EntityType.Valve || points[i - 1].EntityType == EntityType.ReducingStation) &&
                        points[i - 1].EntityId == points[i].EntityId)
                    {
                        eEntityId = points[i - 1].EntityId; // ValveId || ReducingStationId
                        eEntityType = points[i - 1].EntityType; // Valve || ReducingStation
                        eName = points[i - 1].Name;
                    }

                    else
                    {
                        eEntityId = pipeline.Id; // PipelineId
                        eEntityType = EntityType.Pipeline; // Pipeline
                        eName = pipeline.Name;
                    }

                    var kmStart = points[i - 1].Kilometer;
                    var kmEnd = points[i].Kilometer;
                    var vEntityId = points[i].EntityId;
                    var vEntityType = points[i].EntityType;
                    var vName = points[i].Name;

                    v1 = AddEdge(pipeline.Id, v1, vEntityId, vEntityType, vName, eEntityId, eEntityType, kmStart, kmEnd, eName);
                }
            }

            foreach (var vertex in _graph.VertexList.Where(v => v.Type == EntityType.Valve).ToList())
            {
                vertex.EntityId = new Guid();
                vertex.Type = new EntityType();
            }

            ConnectPipelines();
            AddCompShop();
            SetBalanceSign();

            _graph.EdgeList.Where(e => e.EntityType == EntityType.Pipeline)
                .ToList()
                .ForEach(e => e.TypeOfPipeline = _pipelines.Single(p => p.Id == e.EntityId).Type);

            return _graph;
        }

        /// <summary>
        /// Получение всех объектов модели, необходимых для построения графа
        /// </summary>
        private void GetObjects()
        {
            _pipelines =
                new GetPipelineListQuery(_context).Execute(new GetPipelineListParameterSet { SystemId = _systemId })
                    .Where(p => p.Type != PipelineType.CompressorShopBridge)
                    .ToList();
            _pipelineConns = new GetPipelineConnListQuery(_context).Execute(new GetPipelineConnListParameterSet
            {
                GasTrasportSystemId
                    =
                    _systemId
            }).Where(c => _pipelines.Any(p => p.Id == c.PipelineId)).ToList();

            _sites = new GetSiteListQuery(_context).Execute(new GetSiteListParameterSet
            {
                EnterpriseId = _enterpriseId,
                SystemId = _systemId
            });

            var measStations = new List<Guid>();
            foreach (SiteDTO site in _sites)
            {
                measStations.AddRange(
                    new GetMeasStationListQuery(_context).Execute(new GetMeasStationListParameterSet
                    {
                        SiteId = site.Id,
                        SystemId = _systemId
                    }).Select(s => s.Id));
            }
            _measLines =
                new GetMeasLineListQuery(_context).Execute(new GetMeasLineListParameterSet
                {
                    MeasStationList = measStations,
                    SystemId = _systemId
                });

            var compStations = new List<CompStationDTO>();
            foreach (var site in _sites)
            {
                compStations.AddRange(new GetCompStationListQuery(_context).Execute(new GetCompStationListParameterSet
                {
                    SiteId = site.Id,
                    SystemId = _systemId
                }));
            }
            _compShops = new GetCompShopListQuery(_context).Execute(
                new GetCompShopListParameterSet
                {
                    StationIdList = compStations.Select(s => s.Id).ToList(),
                    SystemId = _systemId
                });

            _distrStations = new List<DistrStationDTO>();
            foreach (var site in _sites)
            {
                _distrStations.AddRange(new GetDistrStationListQuery(_context).Execute(new GetDistrStationListParameterSet
                {
                    SiteId = site.Id,
                    SystemId = _systemId
                }));
            }

            _reducingStations = new List<ReducingStationDTO>();
            foreach (var site in _sites)
            {
                _reducingStations.AddRange(
                    new GetReducingStationListQuery(_context).Execute(new GetReducingStationListParameterSet
                    {
                        SystemId = _systemId,
                        SiteId = site.Id
                    }));
            }
            _valves = new GetValveListQuery(_context).Execute(new GetValveListParameterSet { SystemId = _systemId });
            _valveTypes = new GetValveTypesListQuery(_context).Execute();

        }

        private double KmConnection(PipelineConnDTO connect)
        {
            var pipe = _pipelines.Single(p => p.Id == connect.PipelineId);
            return connect.EndTypeId == PipelineEndType.StartType ? pipe.KilometerOfStartPoint : pipe.KilometerOfEndPoint;
        }

        private Vertex AddEdge(Guid pipelineId, Vertex v1, Guid vEntityId, EntityType vEntityType, string vName, Guid eEntityId, EntityType eEntityType,
            double kmStart, double kmEnd, string eName)
        {
            var diameter = GetDiameter(pipelineId, kmStart, kmEnd);
            var pressureMax = GetMaxPressure(pipelineId, kmStart, kmEnd);

            var v2 = new Vertex
            {
                Id = _lastVertexId,
                EntityId = vEntityId,
                Type = vEntityType,
                Name = vName
            };

            _lastVertexId += 1;
            _graph.VertexList.Add(v2);

            _graph.EdgeList.Add(new Edge(v1, v2)
            {
                EntityId = eEntityId,
                EntityType = eEntityType,
                KilometerOfStartPoint = kmStart,
                KilometerOfEndPoint = kmEnd,
                Diameter = diameter,
                PressureMax = pressureMax,
                Name = eName
            });

            return v2;
        }

        private double GetDiameter(Guid pipelineId, double kmStart, double kmEnd)
        {
            var segment = new GetDiameterSegmentListQuery(_context).Execute(pipelineId).SingleOrDefault(d =>
                d.KilometerOfStartPoint <= kmStart &&
                d.KilometerOfEndPoint >= kmStart &&
                d.KilometerOfStartPoint <= kmEnd &&
                d.KilometerOfEndPoint >= kmEnd);

            var dm = -1.0;
            if (segment == null)
            {
                var valve =
                    new GetValveListQuery(_context).Execute(new GetValveListParameterSet { PipelineId = pipelineId })
                        .FirstOrDefault();
                if (valve != null)
                {
                    dm = _valveTypes.Single(t => t.Id == valve.ValveTypeId).DiameterConv;
                }
            }

            return segment != null ? segment.DiameterConv : dm;
        }

        private double GetMaxPressure(Guid pipelineId, double kmStart, double kmEnd)
        {
            var segment = new GetPressureSegmentListQuery(_context).Execute(pipelineId).SingleOrDefault(d =>
                d.KilometerOfStartPoint <= kmStart &&
                d.KilometerOfEndPoint >= kmStart &&
                d.KilometerOfStartPoint <= kmEnd &&
                d.KilometerOfEndPoint >= kmEnd);
            return segment != null ? segment.Pressure : 75;
        }

        private void ConnectPipelines()
        {
            foreach (var conn in _pipelineConns.Where(p => p.DestPipelineId.HasValue))
            {
                var vOnPipe = VertexOnPipe(conn);
                var vOnDestPipe = VertexOnDestPipe(conn);

                if (vOnPipe.Type != new EntityType())
                {
                    foreach (var edge in _graph.EdgeList)
                    {
                        if (edge.V1 == vOnDestPipe)
                        {
                            edge.V1 = vOnPipe;
                        }
                        if (edge.V2 == vOnDestPipe)
                        {
                            edge.V2 = vOnPipe;
                        }
                    }
                    _graph.VertexList.Remove(vOnDestPipe);
                }
                else
                {
                    foreach (var edge in _graph.EdgeList)
                    {
                        if (edge.V1 == vOnPipe)
                        {
                            edge.V1 = vOnDestPipe;
                        }
                        if (edge.V2 == vOnPipe)
                        {
                            edge.V2 = vOnDestPipe;
                        }
                    }
                    _graph.VertexList.Remove(vOnPipe);
                }
            }

            foreach (var stat in _distrStations)
            {
                var conns = _pipelineConns.Where(c => c.DistrStationId.HasValue && c.DistrStationId == stat.Id).ToList();

                if (conns.Count > 1)
                {
                    var edge = _graph.EdgeList.First(e => e.V2.EntityId == stat.Id);

                    foreach (var conn in conns)
                    {
                        if (conn.PipelineId == edge.EntityId)
                        {
                            continue;
                        }

                        var v2 = _graph.EdgeList.Single(e => e.EntityId == conn.PipelineId && e.V2.EntityId == stat.Id).V2;
                        _graph.VertexList.Remove(v2);

                        _graph.EdgeList.Single(e => e.EntityId == conn.PipelineId && e.V2.EntityId == stat.Id).V2 =
                            edge.V2;
                    }
                }
            }
        }

        private Vertex VertexOnPipe(PipelineConnDTO conn)
        {
            var pipeline = _pipelines.Single(p => p.Id == conn.PipelineId);

            return conn.EndTypeId == PipelineEndType.StartType
                ? _graph.EdgeList.Single(
                    e => e.EntityId == conn.PipelineId && e.KilometerOfStartPoint == pipeline.KilometerOfStartPoint).V1
                : _graph.EdgeList.Single(
                    e => e.EntityId == conn.PipelineId && e.KilometerOfEndPoint == pipeline.KilometerOfEndPoint).V2;
        }

        private Vertex VertexOnDestPipe(PipelineConnDTO conn)
        {
            var pipeline = _pipelines.Single(p => p.Id == conn.DestPipelineId);
            var edge =
                _graph.EdgeList.SingleOrDefault(e => e.EntityId == pipeline.Id && e.KilometerOfStartPoint == conn.Kilometr.Value);
            //.SingleOrDefault(e => e.KilometerOfStartPoint == conn.Kilometr.Value)
            //.V1;

            if (edge == null)
            {
                return
                    _graph.EdgeList.SingleOrDefault(
                        e => e.EntityId == pipeline.Id && e.KilometerOfEndPoint == conn.Kilometr.Value).V2;
            }
            //.SingleOrDefault(e => e.KilometerOfEndPoint == conn.Kilometr.Value).V2;

            return edge.V1;
        }

        private void AddCompShop()
        {
            foreach (var compShop in _compShops)
            {
                // КС Строжевка / КЦ-3
                if (compShop.Id == new Guid("de45ee43-7e6b-6b48-8137-9df8b3894ab0"))
                {
                    continue;
                }
                // КС Александров-Гай / КЦ-5
                if (compShop.Id == new Guid("395445d6-e060-5042-ac05-1462f344183b"))
                {
                    continue;
                }
                // КС Александров-Гай / КЦ-5
                if (compShop.Id == new Guid("a068709f-086c-f04e-b57e-fca2c5b2eee4"))
                {
                    continue;
                }

                var edgesIn =
                    _graph.EdgeList.Where(e => e.V2.EntityId.HasValue && e.V2.EntityId.Value == compShop.Id).ToList();

                var vIn = edgesIn.First().V2;

                foreach (var edge in edgesIn)
                {
                    if (edge == edgesIn.First())
                    {
                        continue;
                    }

                    _graph.VertexList.Remove(edge.V2);
                    edge.V2 = vIn;
                }

                var edgesOut =
                    _graph.EdgeList.Where(e => e.V1.EntityId.HasValue && e.V1.EntityId.Value == compShop.Id).ToList();

                var vOut = edgesOut.First().V1;

                foreach (var edge in edgesOut)
                {
                    if (edge == edgesOut.First())
                    {
                        continue;
                    }

                    _graph.VertexList.Remove(edge.V1);
                    edge.V1 = vOut;
                }

                double kmStart;
                double kmEnd;

                if (_pipelines.Single(p => p.Id == edgesIn.First().EntityId).BeginEntityId ==
                    _pipelines.Single(p => p.Id == edgesOut.First().EntityId).EndEntityId)
                {
                    kmStart = _pipelines.Single(p => p.Id == edgesIn.First().EntityId).KilometerOfBeginConn.Value;
                    kmEnd = _pipelines.Single(p => p.Id == edgesOut.First().EntityId).KilometerOfEndConn.Value;
                }

                else
                {
                    kmStart = _pipelines.Single(p => p.Id == edgesIn.First().EntityId).KilometerOfBeginConn.Value;
                    kmEnd = kmStart +
                            (_pipelines.Single(p => p.Id == edgesOut.First().EntityId).KilometerOfEndConn.Value -
                             _pipelines.Single(p => p.Id == edgesOut.First().EntityId).KilometerOfStartPoint);
                }

                _graph.VertexList.Single(v => v == vIn).EntityId = compShop.ParentId;
                _graph.VertexList.Single(v => v == vOut).EntityId = new Guid?();

                _graph.EdgeList.Add(new Edge(_graph.VertexList.Single(v => v == vIn),
                    _graph.VertexList.Single(v => v == vOut))
                {
                    EntityId = compShop.Id,
                    EntityType = EntityType.CompShop,
                    KilometerOfStartPoint = kmStart,
                    KilometerOfEndPoint = kmEnd,
                    Diameter = edgesIn.First().Diameter,
                    PressureMax = edgesIn.First().PressureMax
                });
            }
        }

        private void SetBalanceSign()
        {
            _graph.VertexList.Where(v => v.Type == EntityType.DistrStation)
                .ToList()
                .ForEach(v => v.BalanceSignId = Sign.Out);

            var measStations = new List<MeasStationDTO>();
            foreach (SiteDTO site in _sites)
            {
                measStations.AddRange(
                    new GetMeasStationListQuery(_context).Execute(new GetMeasStationListParameterSet
                    {
                        SiteId = site.Id,
                        SystemId = _systemId
                    }));
            }
            foreach (var ml in _graph.VertexList.Where(v => v.Type == EntityType.MeasLine))
            {
                var sign =
                    measStations.Single(ms => ms.Id == _measLines.Single(m => m.Id == ml.EntityId).ParentId)
                        .BalanceSignId;

                ml.BalanceSignId = sign;
            }
        }
    }

    public class PointOnPipe : IComparable
    {
        public double Kilometer { get; set; }

        public EntityType EntityType { get; set; }

        public string Name { get; set; }

        public Guid EntityId { get; set; }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return -1;
            }

            var p = obj as PointOnPipe;
            if (p.Kilometer > Kilometer)
            {
                return -1;
            }

            return 1;
        }
    }

    public class Corridor
    {
        public string Name { get; set; }

        public List<Line> ParallelLines { get; set; }
    }

    public class Line
    {
        public string Name { get; set; }
        public Guid PipeId { get; set; }
        public double KmStart { get; set; }
        public double KmEnd { get; set; }
    }
}
