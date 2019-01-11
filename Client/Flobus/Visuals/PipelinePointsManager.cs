using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using GazRouter.Flobus.Interfaces;
using GazRouter.Flobus.Model;
using GazRouter.Flobus.UiEntities;
using GazRouter.Flobus.UiEntities.FloModel;
using GazRouter.Flobus.Utilites;
using JetBrains.Annotations;
using Telerik.Windows.Diagrams.Core;
using static System.Double;
using PositionChangedEventArgs = GazRouter.Flobus.EventArgs.PositionChangedEventArgs;

namespace GazRouter.Flobus.Visuals
{
    /// <summary>
    ///     Управляет списком точек газопровода
    /// </summary>
    public class PipelinePointsManager : IPipelinePointManager
    {
        /// <summary>
        ///     список точек
        /// </summary>
        private readonly LinkedList<IPipelinePoint> _listPoints = new LinkedList<IPipelinePoint>();

        /// <summary>
        ///     список разрывов
        /// </summary>
        private List<Segment> _gaps;

        /// <summary>
        ///     Флаг указывающий, что смещаются все точки сразу
        /// </summary>
        private bool _allPointsMove;

        /// <summary>
        ///     Нужно ли пересчитать инфраструктурные точки
        /// </summary>
        private bool _isRecalcInfraPoint;
        
        /// <summary>
        ///     Создает экземпляр PipelinePointsManager
        /// </summary>
        /// <param name="startPoint">координаты начала</param>
        /// <param name="endPoint">координаты конца</param>
        /// <param name="startKm">км начала</param>
        /// <param name="endKm">км конца</param>
        /// <param name="intermediatePoints">список промежуточных точек</param>
        public PipelinePointsManager(Point startPoint, Point endPoint, double startKm, double endKm,
            IEnumerable<IGeometryPoint> intermediatePoints = null, List<Segment> gaps = null )
        {
            AddStartEndPoints(startPoint, endPoint, startKm, endKm);
            if (intermediatePoints != null)
            {
                AddIntermediatePoints(intermediatePoints);
            }
            _gaps = gaps ?? new List<Segment>();
            
            EnsureRectangularLine();
            CheckMinLenght();
        }

        /// <summary>
        ///     Возвращает прямоугольник на котором располагается  газопровод
        /// </summary>
        public Rect? PipelineRect
        {
            get
            {
                if (Count == 0)
                {
                    return null;
                }

                double maxX = 0;
                double maxY = 0;
                var minX = MaxValue;
                var minY = MaxValue;
                foreach (var pt in _listPoints)
                {
                    var pos = pt.Position;
                    if (pos.X > maxX)
                    {
                        maxX = pos.X;
                    }
                    if (pos.Y > maxY)
                    {
                        maxY = pos.Y;
                    }
                    if (pos.X < minX)
                    {
                        minX = pos.X;
                    }
                    if (pos.Y < minY)
                    {
                        minY = pos.Y;
                    }
                }
                return new Rect(minX, minY, maxX - minX, maxY - minY);
            }
        }

        /// <summary>
        ///     Начальная точка газопровода
        /// </summary>
        public IPipelinePoint Start => _listPoints.First.Value;

        /// <summary>
        ///     Конечная точка газопровода
        /// </summary>
        public IPipelinePoint End => _listPoints.Last.Value;

        /// <summary>
        ///     Список разрывов газопровода
        /// </summary>
        public List<PointSegment> OverlaySegments => _gaps
            .Select(gap => new PointSegment(Km2Point(gap.KmBeginning), Km2Point(gap.KmEnd)))
            .ToList();

        /// <summary>
        ///     Список сегментов газопровода
        /// </summary>
        public List<GeometrySegment> GeometrySegments { get; } = new List<GeometrySegment>();

        /// <summary>
        ///     Кол-во точек газопровода
        /// </summary>
        public int Count => _listPoints.Count;

        /// <summary>
        ///     Указывает что все отрезки являются горизонтальными или вертикальными
        /// </summary>
        public bool IsOrthogonal
        {
            get
            {
                var node = _listPoints.First;
                while (node != _listPoints.Last)
                {
                    if (node.Value.Position.X != node.Next.Value.Position.X &&
                        node.Value.Position.Y != node.Next.Value.Position.Y)
                    {
                        return false;
                    }

                    node = node.Next;
                }
                return true;
            }
        }

        /// <summary>
        ///     Минимальная длина газпоровода в пикселях
        /// </summary>
        public int MinLength { get; } = 7;

        /// <summary>
        ///     ReadOnly список точек
        /// </summary>
        public IList<IPipelinePoint> Points => _listPoints.ToList().AsReadOnly();

        public IEnumerable<Point> GeometryPoints => GeometryNodes.Select(node => node.Value.Position);

        /// <summary>
        ///     Список координат изгиба газопровода
        /// </summary>
        private IEnumerable<LinkedListNode<IPipelinePoint>> GeometryNodes
        {
            get
            {
                if (_listPoints.First.Next == _listPoints.Last)
                {
                    yield break;
                }
                var turnNode = _listPoints.First.Next;
                var orientation = IsHorizontalLine(turnNode.Value.Position, _listPoints.First.Value.Position)
                    ? Orientation.Vertical
                    : Orientation.Horizontal;
                var nextNode = turnNode.Next;
                while (nextNode != null)
                {
                    if (orientation == Orientation.Horizontal)
                    {
                        if (nextNode.Value.Position.Y != turnNode.Value.Position.Y)
                        {
                            yield return turnNode;
                            orientation = Orientation.Vertical;
                        }
                    }
                    else
                    {
                        if (nextNode.Value.Position.X != turnNode.Value.Position.X)
                        {
                            yield return turnNode;

                            orientation = Orientation.Horizontal;
                        }
                    }
                    turnNode = nextNode;
                    nextNode = nextNode.Next;
                }
            }
        }

        /// <summary>
        ///     Добавляет новую точку газопровода
        /// </summary>
        /// <param name="newItem">новая точка</param>
        public void Add(IPipelinePoint newItem)
        {
            var curNode = _listPoints.First;
            if (newItem.Km < curNode.Value.Km)
            {
                throw new ArgumentException("Км не может быть меньше километра начала");
            }
            if (newItem.Type == PointType.Intermediate)
            {
                newItem.PositionChanged += OnPointPositionChanged;
            }
            while (curNode != null)
            {
                var point = curNode.Value;
                if (Math.Abs(newItem.Km - point.Km) < 0.001)
                {
                    if (point.Type == PointType.First || point.Type == PointType.Last)
                    {
                        ((PipelinePoint) point).AttachedElement = newItem;
                    }
                    else
                    {
                        throw new ArgumentException("Элемент с таким КМ уже есть");
                    }
                    return;
                }
                if (newItem.Km < point.Km)
                {
                    var node = _listPoints.AddBefore(curNode, newItem);
                    ArrangeInternal(node);
                    if (newItem.Type != PointType.Infra)
                    {
                    }
                    return;
                }
                curNode = curNode.Next;
            }
            throw new ArgumentException("Км не может быть больше километра конца");
        }

        /// <summary>
        ///     Доавбляет промежуточную точку
        /// </summary>
        /// <param name="newPoint">координаты точки на газопроводе</param>
        /// <param name="km">км</param>
        /// <returns></returns>
        public IPipelinePoint AddIntermediatePoint(Point newPoint, double? km = null)
        {
            var pp = _listPoints.First;
            while (pp != _listPoints.Last)
            {
                var point = pp.Value;
                if (point.Position == newPoint || (km.HasValue && km.Value == point.Km))
                {
                    if (point.Type == PointType.Turn)
                    {
                        var pipelinePoint = (PipelinePoint) point;
                        pipelinePoint.Type = PointType.Intermediate;
                        pipelinePoint.PositionChanged += OnPointPositionChanged;
                        if (point.Position != newPoint)
                        {
                            point.Position = newPoint;
                        }
                    }
                    return point;
                }
                var next = pp.Next.Value;
                if (point.Position.Y == newPoint.Y && next.Position.Y == newPoint.Y)
                {
                    if ((point.Position.X < newPoint.X && newPoint.X < next.Position.X)
                        || (point.Position.X > newPoint.X && newPoint.X > next.Position.X))
                    {
                        return AddPipelinePointInternal(newPoint, point, next);
                    }
                }
                else if (point.Position.X == newPoint.X && next.Position.X == newPoint.X)
                {
                    if ((point.Position.Y < newPoint.Y && newPoint.Y < next.Position.Y)
                        || (point.Position.Y > newPoint.Y && newPoint.Y > next.Position.Y))
                    {
                        return AddPipelinePointInternal(newPoint, point, next);
                    }
                }
                pp = pp.Next;
            }
            if (km.HasValue)
            {
                var point = ((IPipelinePointManager) this).CreatePoint(PointType.Intermediate, km.Value, newPoint);
                Add(point);
                return point;
            }

            throw new ArgumentException("Эта точка не на линии");
        }

        public void Arrange()
        {
            //идем по всем геометрическим сегментам газопровода 
            //если длина сегмента меньше minSegmentLenght 
            //удаляем этот сегмент, а соседние ортогональные сегменты сливаем в один 
            //беря за приоритетный более длинный сегмент  
            const double minGeoremtrySegmentLength = 10;
            bool smallSegmentFounded;
            do
            {
                smallSegmentFounded = false;
                var startSegment = _listPoints.First;

                foreach (var endSegment in GeometryNodes)
                {
                    //                    if (endSegment.Value.Type == PointType.Intermediate || endSegment.Value.Type == PointType.Turn)
                    {
                        var segmentLength = Support2D.RectangularPathLength(startSegment.Value.Position,
                            endSegment.Value.Position);
                        if (segmentLength < minGeoremtrySegmentLength &&
                            (startSegment.Value.Type == PointType.Turn || endSegment.Value.Type == PointType.Turn))
                        {
                            smallSegmentFounded = true;

                            if (startSegment.Value.Type == PointType.Turn)
                            {
                                endSegment.Value.Position = startSegment.Value.Position;
                                // _listPoints.Remove(startSegment);
                            }
                            else
                            {
                                startSegment.Value.Position = endSegment.Value.Position;
                                // _listPoints.Remove(endSegment);
                            }
                            break;
                            /*
                                                        var IsHorizontal = IsHorizontalLine(startSegment.Value.Position, endSegment.Value.Position);
                                                        //  var prevGeomerty
                            //                    var prevNode = null;
                                                        LinkedListNode<IPipelinePoint> prevNode = null;
                                                        if (startSegment != _listPoints.First)
                                                        {
                                                            prevNode = PrevGeometryNode(startSegment, true);
                                                        }
                                                        LinkedListNode<IPipelinePoint> nextNode = null;
                                                        if (endSegment != _listPoints.Last)
                                                        {
                                                            nextNode = NextGeometryNode(endSegment, true);
                                                        }
                                                        double? prevLength = 0;
                                                        if (prevNode != null)
                                                        {
                                                            prevLength = Support2D.RectangularPathLength(prevNode.Value.Position,
                                                                startSegment.Value.Position);
                                                        }
                                                        double nextLength = 0;
                                                        if (nextNode != null)
                                                        {
                                                            nextLength = Support2D.RectangularPathLength(endSegment.Value.Position,
                                                                nextNode.Value.Position);
                                                        }
                                                        if (prevNode != null && prevLength < nextLength)
                                                        {
                                                            while (prevNode != startSegment)
                                                            {
                                                                if (IsHorizontal)
                                                                {
                                                                    prevNode.Value.Position = new Point(prevNode.Value.Position.X,
                                                                        nextNode.Value.Position.Y);
                                                                }
                                                                else
                                                                {
                                                                    prevNode.Value.Position = new Point(nextNode.Value.Position.X,
                                                                        prevNode.Value.Position.Y);
                                                                }
                                                                prevNode = prevNode.Next;
                                                            }
                                                        }
                                                        else if (nextNode != null && nextLength < prevLength)
                                                        {
                                                            while (nextNode != endSegment)
                                                            {
                                                                if (IsHorizontal)
                                                                {
                                                                    nextNode.Value.Position = new Point(nextNode.Value.Position.X,
                                                                        prevNode.Value.Position.Y);
                                                                }
                                                                else
                                                                {
                                                                    nextNode.Value.Position = new Point(prevNode.Value.Position.X,
                                                                        nextNode.Value.Position.Y);
                                                                }
                                                                nextNode = nextNode.Previous;
                                                            }
                                                        }
                            */
                        }
                    }
                    startSegment = endSegment;
                }
            }
            while (smallSegmentFounded);
        }

        public IPipelinePoint CreatePoint(IGeometryPoint geometryPoint, PointType infra)
        {
            return new PipelinePoint(this, geometryPoint, infra);
        }

        public PipelineSegment FindGeometrySegment(IPipelinePoint point)
        {
            var node = _listPoints.Find(point);
            var nodeStart = node;

            var nodeEnd = node;
            while (nodeStart.Value.Type != PointType.Intermediate && nodeStart.Value.Type != PointType.First)
            {
                nodeStart = nodeStart.Previous;
            }
            while (nodeEnd.Value.Type != PointType.Intermediate && nodeEnd.Value.Type != PointType.Last)
            {
                nodeEnd = nodeEnd.Next;
            }

            return new PipelineSegment(nodeStart.Value, nodeEnd.Value);
        }

        public List<Point> GetGeometryPoints(double startKm, double endKm)
        {
            var result = new List<Point>();
            var startPoint = Km2Point(startKm);
            var endPoint = Km2Point(endKm);
            var cursor = _listPoints.First;
            while (cursor != _listPoints.Last)
            {
                if (cursor.Value.Km > endKm)
                {
                    break;
                }
                if (cursor.Value.Km >= startKm && cursor.Value.Km <= endKm)
                {
                    result.Add(cursor.Value.Position);
                }

                cursor = NextGeometryNode(cursor);
            }
            if (result.Count == 0 || result[0] != startPoint)
            {
                result.Insert(0, startPoint);
            }
            if (result[result.Count - 1] != endPoint)
            {
                result.Add(endPoint);
            }
            return result;
        }

        public IPipelinePoint FindOrCreateInfraPoint(double km, bool makeInfra)
        {
            var point = FindPoint(km);
            if (point == null)
            {
                point = Add(PointType.Infra, km, Km2Point(km));
            }
            else
            {
                if (point.Type == PointType.Intermediate && makeInfra)
                {
                    ((PipelinePoint) point).Type = PointType.Infra;
                }
            }
            return point;
        }
        public IPipelinePoint FindPoint(double km)
        {
            return _listPoints.FirstOrDefault(p => Math.Abs(p.Km - km) < 0.001);
        }
        /// <summary>
        ///     Возвращает объект PipelineSegment для указанной точки
        /// </summary>
        /// <param name="p">координаты точки на газопроводе</param>
        /// <returns>объект типа PipelineSegment</returns>
        PipelineSegment IPipelinePointManager.FindSegment(Point p)
        {
            //        IPipelinePoint startPoint = new NewPipelinePoint(this,StartPoint, KmBegining, PointType.First);
            //        IPipelinePoint endPoint = new NewPipelinePoint(this, EndPoint, KmEnd, PointType.Last);
            //        if (Points.Count == 0)
            //        {
            //            return new PipelineSegment(startPoint, endPoint);
            //        }

            var startNode = _listPoints.First;
            var spot = new Rect(new Point(p.X - 4, p.Y - 4), new Size(8, 8));
            var intersectionPoint = new Point();
            while (startNode.Next != null)
            {
                if (spot.IntersectsLineSegment(startNode.Value.Position, startNode.Next.Value.Position,
                    ref intersectionPoint))
                {
                    return new PipelineSegment(startNode.Value, startNode.Next.Value);
                }
                startNode = startNode.Next;
            }

            return null;
        }
        /// <summary> Возвращает объект PipelineSegment для указанного километра </summary>
        /// <param name="km">пикетный километр на газопроводе</param>
        /// <param name="skipInfra"></param>
        /// <returns>объект типа PipelineSegment</returns>
        public PipelineSegment FindSegment(double km, bool skipInfra = false)
        {
            var cursor = _listPoints.First;
            if (km < cursor.Value.Km)
            {
                return null;
            }
            while (cursor.Next != null)
            {
                if (km <= cursor.Next.Value.Km)
                {
                    var segBegin = cursor;
                    var segEnd = cursor.Next;
                    if (!skipInfra)
                    {
                        return new PipelineSegment(segBegin.Value, segEnd.Value);
                    }
                    while (segBegin.Value.Type == PointType.Infra)
                    {
                        segBegin = segBegin.Previous;
                    }
                    while (segEnd.Value.Type == PointType.Infra)
                    {
                        segEnd = segEnd.Next;
                    }
                    return new PipelineSegment(segBegin.Value, segEnd.Value);
                }
                cursor = cursor.Next;
            }
            return null;
        }
        public Point Km2Point(double km)
        {
            var segment = FindSegment(km);
            return segment == null ? new Point(km, km) : FindSegment(km).Km2Point(km);// todo: временное решение 
        }

        public void MakeInfra(IPipelinePoint point)
        {
            if (point.Type == PointType.Intermediate)
            {
                ((PipelinePoint) point).Type = PointType.Infra;
            }
        }

        public void Move(Vector offset)
        {
            _allPointsMove = true;
            foreach (var pipelinePoint in _listPoints)
            {
                pipelinePoint.Position = pipelinePoint.Position.Add(offset);
            }
            _allPointsMove = false;
        }

        public void MoveAlong(IPipelinePoint pipelinePoint, Vector offset)
        {
            if (pipelinePoint.Type != PointType.Infra)
            {
                return;
            }

            var node = _listPoints.Find(pipelinePoint);

            var prevPoint = node.Previous.Value;
            var nextPoint = node.Next.Value;

            var startPoint = prevPoint.Position;
            var endPoint = nextPoint.Position;
            var point = Telerik.Windows.Diagrams.Core.Utils.ProjectPointOnLine(pipelinePoint.Position.Add(offset),
                startPoint, endPoint);

            if ((point != startPoint || point != endPoint) && !Support2D.PointAtLine(startPoint, endPoint, point))
            {
                if ((point.X < startPoint.X && point.X < endPoint.X) ||
                    (point.Y < startPoint.Y && point.Y < endPoint.Y))
                {
                    point = Support2D.PointOnLine(startPoint, endPoint, (Support2D.Length(startPoint, endPoint) - 1)*-1);
                }
                else
                {
                    point = Support2D.PointOnLine(startPoint, endPoint, -1);
                }
            }
            pipelinePoint.Position =
                point;
        }

        public void MovePoint(IPipelinePoint pipelinePoint, IPipelinePoint prev_point)
        {
            if (pipelinePoint.Type != PointType.Infra || pipelinePoint == prev_point)
            {
                return;
            }
            _listPoints.Remove(pipelinePoint);
            _listPoints.AddAfter(_listPoints.Find(prev_point), pipelinePoint);
        }

        public void RecalculateIntermediateKm(IPipelinePoint point)
        {
            //foreach (var int_point in _listPoints.Where(p => p.Type == PointType.Intermediate))
            //{
            //    var seg = ((IPipelinePointManager)this).FindSegment(int_point.Position);
            //    int_point.Km = GetKmOnIntermediatePoint(int_point);
            //}
            CheckIntermediatePoint(point);
        }

        public double GetKmOnIntermediatePoint(IPipelinePoint int_point)
        {
            LinkedListNode<IPipelinePoint> prev_point = _listPoints.Find(int_point);
            LinkedListNode<IPipelinePoint> next_point = _listPoints.Find(int_point);

            while (prev_point.Value.Type != PointType.Infra && prev_point.Value.Type != PointType.First)
                prev_point = prev_point.Previous;
            while (next_point.Value.Type != PointType.Infra && next_point.Value.Type != PointType.Last)
                next_point = next_point.Next;

            double mid_infra_lenght = Math.Abs(prev_point.Value.Position.X - next_point.Value.Position.X) + Math.Abs(prev_point.Value.Position.Y - next_point.Value.Position.Y);

            return prev_point.Value.Km + Math.Round(Math.Abs(prev_point.Value.Km -next_point.Value.Km)
                              / mid_infra_lenght
                              * Support2D.RectangularPathLength(prev_point.Value.Position, int_point.Position), 3);
        }

        public double MaxAlloweKm(IPipelinePoint pipelinePoint)
        {
            return _listPoints.Find(pipelinePoint).Next.Value.Km - 0.001;
        }

        public double MinAllowedKm(IPipelinePoint pipelinePoint)
        {
            return _listPoints.Find(pipelinePoint).Previous.Value.Km + 0.001;
        }

        public IPipelinePoint NextPoint(IPipelinePoint point)
        {
            return _listPoints.Find(point)?.Next?.Value;
            //            C5.KeyValuePair<double, IPipelinePoint> s;
            //            return _dictionary.TrySuccessor(point.Km, out s) ? s.Value : null;
            /*  int index = IndexOf(point);
              if (index >= 0 && index < Count - 1) return this[index + 1];



              return WeakSuccessor();*/
        }

        /// <summary>
        ///     Возвращает предшествующую точку газопровода для указанной
        /// </summary>
        /// <param name="point">Точка газопровода, для которой нужно получить предшествующую точку</param>
        /// <returns>Предшествующая точка</returns>
        public IPipelinePoint PreviousPoint(IPipelinePoint point)
        {
            return _listPoints.Find(point)?.Previous.Value;
            // C5.KeyValuePair<double, IPipelinePoint> predecessor ;

            //return _dictionary.F(point.Km, out predecessor) ? predecessor.Value : null;
            //     if (index > 0) return this[index - 1];
            //
            //   return null;
        }

        public void RecalcInfraPoints(IPipelinePoint point, Point oldPosition)
        {
            if (point.Type == PointType.Infra)
            {
                return;
            }
            _isRecalcInfraPoint = true;
            var changedPositionNode = _listPoints.Find(point);
            var position = point.Position;
            if (point.Type == PointType.First || point.Type == PointType.Intermediate)
            {
                var nextIntermediateNode = NextGeometryNode(changedPositionNode);

                var endPoint = nextIntermediateNode.Value.Position;
                var oldLenght = Support2D.RectangularPathLength(oldPosition, endPoint);
                var newLenght = Support2D.RectangularPathLength(position, endPoint);

                PipelinePoint turnPoint = null;
                if (endPoint.X != position.X && endPoint.Y != position.Y)
                {
                    turnPoint = CreateTurnPoint(changedPositionNode, nextIntermediateNode);
                }

                var node = changedPositionNode.Next;
                while (node != nextIntermediateNode)
                {
                    if (turnPoint != null && (node.Value.Km > turnPoint.Km))
                    {
                        _listPoints.AddBefore(node, turnPoint);
                        turnPoint = null;
                    }

                    var next = node.Next;
                    //Удаляем предыдущие точки поворота
                    if (node.Value.Type == PointType.Turn)
                    {
                        _listPoints.Remove(node);
                    }
                    else
                    {
                        node.Value.Position = Support2D.PointOnRectangularLine(position, endPoint,
                            Support2D.RectangularPathLength(oldPosition, node.Value.Position) * newLenght / oldLenght);
                    }

                    node = next;
                }
            }

            if (point.Type == PointType.Last || point.Type == PointType.Intermediate)

            {
                var prevIntermediateNode = PrevGeometryNode(changedPositionNode);
                var startPoint = prevIntermediateNode.Value.Position;
                var oldLenght = Support2D.RectangularPathLength(startPoint, oldPosition);
                var newLenght = Support2D.RectangularPathLength(startPoint, position);

                PipelinePoint turnPoint = null;
                if (startPoint.X != position.X && startPoint.Y != position.Y)
                {
                    turnPoint = CreateTurnPoint(prevIntermediateNode, changedPositionNode);
                }
                var node = changedPositionNode.Previous;
                while (node != prevIntermediateNode)
                {
                    if (turnPoint != null && node.Value.Km < turnPoint.Km)
                    {
                        _listPoints.AddAfter(node, turnPoint);
                        turnPoint = null;
                    }

                    var prev = node.Previous;
                    if (node.Value.Type == PointType.Turn)
                    {
                        _listPoints.Remove(node);
                    }
                    else
                    {
                        node.Value.Position = Support2D.PointOnRectangularLine(startPoint, position,
                            Support2D.RectangularPathLength(startPoint, node.Value.Position) * newLenght / oldLenght);
                    }

                    node = prev;
                }
                /*
                                _pipeline.SegmentChanged(prevIntermediateNode.Value.Km, point.Km,
                                    Support2D.Radians2Degrees(new PipelineSegment(prevIntermediateNode.Value, point).Angle));
                */
            }
            _isRecalcInfraPoint = false;
        }

        public List<IPipelinePoint> GetPointsOnKm(double km_begin, double km_end)
        {
            var _geometryList = GeometryNodes.Select(n => n.Value).ToList();
            List<IPipelinePoint> res_list = new List<IPipelinePoint>();
            foreach(var point in _geometryList)
                if (point.Km > km_begin && point.Km <= km_end)
                    res_list.Add(point);
            return res_list;
        }

        public List<List<IPipelinePoint>> GetPointsSeparateGaps()
        {
            int count = _gaps.Count;
            List<List<IPipelinePoint>> points = new List<List<IPipelinePoint>>();
            var start_km = _listPoints.First.Value.Km;
            var end_km = _listPoints.Last.Value.Km;
            foreach(var gap in _gaps)
            {
                points.Add(GetPointsOnKm(start_km, gap.KmBeginning));
                start_km = gap.KmEnd;
            }
            points.Add(GetPointsOnKm(start_km, end_km));
            return points;
        }

        /// <summary>
        ///     Удаление точки газопровода
        /// </summary>
        /// <param name="point">Удаляемая точка</param>
        /// <returns>True - если точка удалена успешно, false - если удалить невозможно</returns>
        public bool RemovePoint(IPipelinePoint point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            if (point.Type != PointType.Intermediate && point.Type != PointType.Infra)
            {
                throw new ArgumentException("Можно удалять только инфраструктурные точки или промежуточные точки");
            }

            var node = _listPoints.Find(point);

            var ptBegin = node.Previous.Value;

            var ptEnd = node.Next.Value;

            var refresh_point = PrevGeometryNode(node);
            
            var bResult = _listPoints.Remove(point);

            RecalcInfraPoints(refresh_point.Value, refresh_point.Value.Position);

            return bResult;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var pipelinePoint in _listPoints)
            {
                sb.AppendLine($"{pipelinePoint.Km} - {pipelinePoint.Position} - {pipelinePoint.Type}");
            }
            return sb.ToString();
        }

        public IList<GeometrySegment> CreateGeometrySegments(IList<Point> list)
        {
            var result = new List<GeometrySegment>();
            GeometrySegment segment;
            if (list[0].X != list[1].X && list[0].Y != list[1].Y)
            {
                var point = new Point(list[1].X, list[0].Y);
                result.Add(new GeometrySegment(list[0], point));
                segment = new GeometrySegment(point, list[1]);
            }
            else
            {
                segment = new GeometrySegment(list[0], list[1]);
            }

            for (var i = 2; i < list.Count; i++)
            {
                var point = list[i];
                if ((segment.Orientation == Orientation.Horizontal && point.Y == segment.Start.Y) ||
                    (segment.Orientation == Orientation.Vertical && point.X == segment.Start.X))
                {
                    segment.End = point;
                }
                else
                {
                    result.Add(segment);
                    if (segment.End.X != point.X && segment.End.Y != point.Y)
                    {
                        var endPoint = new Point(point.X, segment.End.Y);
                        result.Add(new GeometrySegment(segment.End, endPoint));
                        segment = new GeometrySegment(endPoint, point);
                    }
                    else
                    {
                        segment = new GeometrySegment(segment.End, point);
                    }
                }
            }
            result.Add(segment);
            return result;
        }

        IPipelinePoint IPipelinePointManager.CreatePoint(PointType intermediate, double km, Point p)
        {
            return new PipelinePoint(this, intermediate, km, p);
        }

        internal IPipelinePoint Add(PointType infra, double km, Point point)
        {
            var pipelinePoint = new PipelinePoint(this, infra, km, point);
            Add(pipelinePoint);
            return pipelinePoint;
        }

        private static double CalcNewKm(Point beginPoint, Point endPoint, double beginKm, double endKm, Point newPoint)
        {
            return beginKm + (endKm - beginKm)/
                   Support2D.RectangularPathLength(beginPoint, endPoint)*
                   Support2D.RectangularPathLength(beginPoint, newPoint);
        }

        private static double CalcNewKm(LinkedListNode<IPipelinePoint> prev, LinkedListNode<IPipelinePoint> next, Point newPoint)
        {
            var prev_segment = new PointSegment(prev.Value.Position, newPoint);
            var next_segment = new PointSegment(next.Value.Position, newPoint);

            List<IPipelinePoint> prev_infra_list = new List<IPipelinePoint>();
            List<IPipelinePoint> next_infra_list = new List<IPipelinePoint>();
            prev_infra_list.Add(prev.Value);
            var point = prev;
            while (point != next)
            {
                if (point.Value.Type == PointType.Infra)
                {
                    if (prev_segment.IsContains(point.Value.Position)) prev_infra_list.Add(point.Value);
                    if (next_segment.IsContains(point.Value.Position)) next_infra_list.Add(point.Value);
                }
                point = point.Next;
            }
            next_infra_list.Add(next.Value);
            var start = prev_infra_list.Last();
            var end   = next_infra_list.First();
            if (start == end) return start.Km;
            //if (start.Position == end.Position) return start.Km + end.Km / 2;
            var result =  CalcNewKm(start.Position, end.Position, start.Km, end.Km, newPoint);
            if(result.IsNanOrInfinity())
                result = CalcNewKm(prev.Value.Position, next.Value.Position, prev.Value.Km, next.Value.Km, newPoint);

            return result;
        }

        private static LinkedListNode<IPipelinePoint> NextGeometryNode(LinkedListNode<IPipelinePoint> node,
            bool includeTurn = false)
        {
            var next = node.Next;
            while (next.Value.Type != PointType.Last &&
                   next.Value.Type != PointType.Intermediate && !(includeTurn && next.Value.Type == PointType.Turn))

            {
                next = next.Next;
            }
            return next;
        }

        private static LinkedListNode<IPipelinePoint> PrevGeometryNode(LinkedListNode<IPipelinePoint> node,
            bool includeTurn = false)
        {
            var prev = node.Previous;
            while (prev.Value.Type != PointType.First &&
                   prev.Value.Type != PointType.Intermediate && !(includeTurn && prev.Value.Type == PointType.Turn))
            {
                prev = prev.Previous;
            }
            return prev;
        }
  
        private void ArrangeInternal(LinkedListNode<IPipelinePoint> node)
        {
            EnsurePrevPointRectangular(node);
            EnsureNextPointRectangular(node);
        }

        /// <summary>
        ///     Добавляет набор промежуточных точек
        /// </summary>
        /// <param name="intermediatePoints"></param>
        private void AddIntermediatePoints(IEnumerable<IGeometryPoint> intermediatePoints)
        {
            var prevKm = NaN;
            foreach (var item in intermediatePoints.OrderBy(p => p.Km))
            {
                if (item.Km < Start.Km)
                {
                    continue;
                }

                if (item.Km > End.Km)
                {
                    continue;
                }
                if (prevKm == item.Km)
                {
                    continue;
                }
                var pp = CreatePoint(item, PointType.Intermediate);
                if (item.Km == Start.Km)
                {
                    ((PipelinePoint) Start).AttachedElement = pp;
                }
                if (item.Km == End.Km)
                {
                    ((PipelinePoint) End).AttachedElement = pp;
                }

                pp.PositionChanged += OnPointPositionChanged;
                _listPoints.AddBefore(_listPoints.Last, pp);
            }
        }

        private void OnPointPositionChanged(object sender, PositionChangedEventArgs e)
        {
            if (!_allPointsMove && !_isRecalcInfraPoint)
            {
                RecalcInfraPoints((IPipelinePoint) sender, e.OldPosition);
                EnsureRectangularLine();
            }
        }

        private void AddStartEndPoints(Point startPoint, Point endPoint, double startKm, double endKm)
        {
            if (_listPoints.Count > 0)
            {
                throw new InvalidOperationException("Список не пустой");
            }

            if (startKm >= endKm)
            {
                throw new ArgumentException("Км начала больше км конца");
            }
            var start = new PipelinePoint(this, PointType.First, startKm, startPoint);
            _listPoints.AddFirst(start);
            var end = new PipelinePoint(this, PointType.Last, endKm, endPoint);
            _listPoints.AddLast(end);
            Start.PositionChanged += OnPointPositionChanged;
            End.PositionChanged += OnPointPositionChanged;
        }
        
        private void CheckIntermediatePoint(IPipelinePoint point)
        {
            var node = _listPoints.Find(point);
            var prev_in = GetPrevIntermediatePoint(node);
            var next_in = GetNextIntermediatePoint(node);
            if (prev_in != null)
                prev_in.Value.Km = CalcNewKm(_listPoints.Find(PrevGeometryPoint(prev_in)), _listPoints.Find(NextGeometryPoint(prev_in)), prev_in.Value.Position);
            if (next_in != null)
                next_in.Value.Km = CalcNewKm(_listPoints.Find(PrevGeometryPoint(next_in)), _listPoints.Find(NextGeometryPoint(next_in)), next_in.Value.Position);

        }
                

        public LinkedListNode<IPipelinePoint> GetNextIntermediatePoint(LinkedListNode<IPipelinePoint> pipelinePoint)
        {
            while (pipelinePoint != _listPoints.Last && pipelinePoint.Value.Type != PointType.Intermediate)
                pipelinePoint = pipelinePoint.Next;
            return pipelinePoint.Value.Type == PointType.Intermediate ? pipelinePoint : null;
        }

        public LinkedListNode<IPipelinePoint> GetPrevIntermediatePoint(LinkedListNode<IPipelinePoint> pipelinePoint)
        {
            while (pipelinePoint != _listPoints.First && pipelinePoint.Value.Type != PointType.Intermediate)
                pipelinePoint = pipelinePoint.Previous;
            return pipelinePoint.Value.Type == PointType.Intermediate ? pipelinePoint : null;
        }

        public LinkedListNode<IPipelinePoint> GetNextTurnPoint(LinkedListNode<IPipelinePoint> pipelinePoint)
        {
            var prevPipelinePoint = pipelinePoint.Value.Type == PointType.Last ? pipelinePoint : pipelinePoint.Next;
            while (prevPipelinePoint.Value.Type != PointType.Last)
            {
                if (prevPipelinePoint.Value.Type == PointType.Turn)
                    break;
                prevPipelinePoint = prevPipelinePoint.Next;
            }
            return prevPipelinePoint.Value.Type == PointType.Turn ? prevPipelinePoint : null;
        }        

        private LinkedListNode<IPipelinePoint> EnsureNextPointRectangular(LinkedListNode<IPipelinePoint> pipelinePoint)
        {
            var nextPipelinePoint = pipelinePoint.Next;
            while (nextPipelinePoint.Value.Type != PointType.Intermediate && nextPipelinePoint.Value.Type != PointType.Last)
            {
                nextPipelinePoint = nextPipelinePoint.Next;
            }

            var point = pipelinePoint.Value.Position;
            var nextPoint = nextPipelinePoint.Value.Position;
            if (point.X != nextPoint.X && point.Y != nextPoint.Y)
            {
                var turnPoint = CreateTurnPoint(pipelinePoint, nextPipelinePoint);
                var old_turnPoint = GetNextTurnPoint(pipelinePoint); 
                if (old_turnPoint == null || (turnPoint.Km != old_turnPoint.Value.Km || turnPoint.Position != old_turnPoint.Value.Position))
                {
                    if (old_turnPoint != null) _listPoints.Remove(GetNextTurnPoint(pipelinePoint).Value);
                    var addAfter = pipelinePoint;
                    while (!(addAfter.Value.Km < turnPoint.Km && turnPoint.Km <= addAfter.Next.Value.Km))
                    {
                        addAfter = addAfter.Next;
                    }
                    _listPoints.AddAfter(addAfter,
                        turnPoint);
                }
            }
            return nextPipelinePoint;
        }

        private void EnsurePrevPointRectangular(LinkedListNode<IPipelinePoint> pipelinePoint)
        {
            var prevPipelinePoint = pipelinePoint.Previous;
            while (prevPipelinePoint.Value.Type == PointType.Infra)
            {
                prevPipelinePoint = prevPipelinePoint.Previous;
            }
            var point = pipelinePoint.Value.Position;
            var prevPoint = prevPipelinePoint.Value.Position;
            if (point.X != prevPoint.X && point.Y != prevPoint.Y)
            {
                var turnPoint = CreateTurnPoint(prevPipelinePoint, pipelinePoint);
                var addAfter = prevPipelinePoint;
                while (!(addAfter.Value.Km < turnPoint.Km && turnPoint.Km <= addAfter.Next.Value.Km))
                {
                    addAfter = addAfter.Next;
                }
                _listPoints.AddAfter(addAfter, turnPoint);
            }
        }

        /// <summary>
        ///     Следит чтобы все отрезки были горизонтальными или вертикальными
        /// </summary>
        public void EnsureRectangularLine()
        {
            var pipelinePoint = _listPoints.First;
            _listPoints.OrderBy(o => o.Km);
            while (pipelinePoint != _listPoints.Last)
            {
                EnsureSegmentRectangular(pipelinePoint);
                pipelinePoint = pipelinePoint.Next;
            }
        }

        private void EnsureSegmentRectangular(LinkedListNode<IPipelinePoint> pipelinePoint)
        {
            var first = pipelinePoint.Value;
            var second = pipelinePoint.Next.Value;
            if ((first.Position.X == second.Position.X) || (first.Position.Y == second.Position.Y))
                return;

            var newPoint = new Point(second.Position.X, first.Position.Y);
            var newKm = CalcNewKm(first.Position,second.Position,first.Km,second.Km,newPoint);
            var turn = new PipelinePoint(this, PointType.Turn, newKm, newPoint);

            _listPoints.AddAfter(pipelinePoint, turn);
        }


        /// <summary>
        ///     Проверяет лежат ли обе точки на одной горизонтали
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns></returns>
        private bool IsHorizontalLine(Point point1, Point point2)
        {
            return point2.X.Equals(point1.X);
        }

        private PipelinePoint CreateTurnPoint(LinkedListNode<IPipelinePoint> pipelinePoint,
            LinkedListNode<IPipelinePoint> nextPipelinePoint)
        {
            var point = pipelinePoint.Value.Position;
            var nextPoint = nextPipelinePoint.Value.Position;
            var newPoint = new Point(nextPoint.X, point.Y);

            var newKm = CalcNewKm(pipelinePoint, nextPipelinePoint, newPoint);
            //var newKm = CalcNewKm(point, nextPoint, pipelinePoint.Value.Km, nextPipelinePoint.Value.Km, newPoint);

            var turnPoint = new PipelinePoint(this, PointType.Turn, newKm, newPoint);
            return turnPoint;
        }

        private IPipelinePoint AddPipelinePointInternal(Point position, IPipelinePoint beginPoint,
            IPipelinePoint endPoint)
        {
            var newKm =
                Math.Round(CalcNewKm(_listPoints.Find(beginPoint), _listPoints.Find(endPoint), position), 3);
            var pipelinePoint = new PipelinePoint(this, PointType.Intermediate, newKm, position);
            Add(pipelinePoint);
            return pipelinePoint;
        }

        private void CheckMinLenght()
        {
            var length = Support2D.RectangularPathLength(Start.Position, End.Position);
            if (length < MinLength)
            {
                var prevPoint = _listPoints.Last.Previous.Value;
                End.Position = length != 0
                    ? Support2D.PointOnLine(prevPoint.Position, End.Position, MinLength - length)
                    : End.Position.Add(new Point(MinLength, 0));
            }
        }

        private Point AlignPoint(PipelinePoint pipelinePoint, Point transformedPoint)
        {
            const int delta = 5;
            var newX = transformedPoint.X;
            var newY = transformedPoint.Y;
            var node = _listPoints.Find(pipelinePoint);

            if (node != _listPoints.First)
            {
                var prevPointPosition = PrevGeometryPoint(node).Position;
                if (Math.Abs(prevPointPosition.X - newX) < delta)
                {
                    newX = prevPointPosition.X;
                }
                else if (Math.Abs(prevPointPosition.Y - newY) < delta)
                {
                    newY = prevPointPosition.Y;
                }
            }

            if (node != _listPoints.Last)
            {
                var nextPointPosition = NextGeometryPoint(node).Position;
                if (Math.Abs(nextPointPosition.X - newX) < delta)
                {
                    newX = nextPointPosition.X;
                }
                else if (Math.Abs(nextPointPosition.Y - newY) < delta)
                {
                    newY = nextPointPosition.Y;
                }
            }

            return new Point(newX, newY);
        }

        private IPipelinePoint PrevGeometryPoint(LinkedListNode<IPipelinePoint> node)
        {
            return PrevGeometryNode(node).Value;
        }

        private IPipelinePoint NextGeometryPoint(LinkedListNode<IPipelinePoint> node)
        {
            return NextGeometryNode(node).Value;
        }

        private class PipelinePoint : IPipelinePoint
        {
            private readonly PipelinePointsManager _parent;
            private readonly IGeometryPoint _geometryPoint;
            private double _km;
            private Point _position;

            public PipelinePoint([NotNull] PipelinePointsManager pipelinePointsManager, PointType type, double km,
                Point point) : this(pipelinePointsManager, type)
            {
                _km = km;
                _position = point.Round();
            }

            public PipelinePoint(PipelinePointsManager pipelinePointsManager, IGeometryPoint geometryPoint,
                PointType type = PointType.Intermediate)
                : this(pipelinePointsManager, type, geometryPoint.Km, geometryPoint.Position)
            {
                _geometryPoint = geometryPoint;
            }

            private PipelinePoint([NotNull] PipelinePointsManager pipelinePointsManager, PointType type)
            {
                if (pipelinePointsManager == null)
                {
                    throw new ArgumentNullException(nameof(pipelinePointsManager));
                }
                _parent = pipelinePointsManager;

                Type = type;
            }

            public event EventHandler<PositionChangedEventArgs> PositionChanged;
            public event PropertyChangedEventHandler PropertyChanged;

            public double MinAllowedKm
            {
                get { return Math.Min(_parent.PreviousPoint(this)?.Km ?? 10000000 + 0.01, Km); }
            }

            public double MaxAllowedKm
            {
                get { return Math.Max(_parent.NextPoint(this)?.Km ?? 0 - 0.01, Km); }
            }

            public IPipelinePoint AttachedElement { get; set; }

            public PointType Type { get; internal set; }

            public Point Position
            {
                get { return _position; }
                set
                {
                    var newPosition = value.Round();
//                    LogManager.GetCurrentClassLogger().Debug($"newPositon={newPosition}");

                    if (newPosition != _position)
                    {
                        var oldValue = _position;
                        _position = newPosition;
                        if (AttachedElement != null)
                        {
                            AttachedElement.Position = newPosition;
                        }
                        if (_geometryPoint != null)
                        {
                            _geometryPoint.Position = newPosition;
                        }
                        OnPositionChanged(new PositionChangedEventArgs(oldValue, newPosition));
                    }
                }
            }

            public double Km
            {
                get { return _km; }
                set
                {
                    _km = value;
                    if (_geometryPoint != null)
                    {
                        _geometryPoint.Km = value;
                    }
                }
            }
            
            public Point Align(Point transformedPoint)
            {
                return _parent.AlignPoint(this, transformedPoint);
            }
            
            protected virtual void OnPositionChanged(PositionChangedEventArgs e)
            {
                PositionChanged?.Invoke(this, e);
            }
        }
    }
}