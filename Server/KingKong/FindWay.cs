using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.Graph;

namespace KingKong
{
    public class FindWay
    {
        private Point _fromPoint;
        private Graph _graph;
        private readonly long[] _vectFrom;
        private readonly long[] _vectTo;
        private readonly long?[,] _initDecision;

        public FindWay(long?[,] initDecision, Point fromPoint, long[] vectFrom, long[] vectTo)
        {
            _initDecision = initDecision;
            _fromPoint = fromPoint;
            _vectFrom = vectFrom;
            _vectTo = vectTo;

            BuildGraph();
        }

        /// <summary>
        /// Построение цикла в опорном решении
        /// </summary>
        public List<Point> FindCycle()
        {
            var prevVert = new int[_graph.VertexList.Count];
            for (var i = 0; i < prevVert.GetLength(0); i++)
            {
                prevVert[i] = -1;
            }

            var vFirst = _graph.VertexList.Single(v => v.Id == _fromPoint.Row + 1);
            var vEnd = _graph.VertexList.Single(v => v.Id == _vectFrom.GetLength(0) + _fromPoint.Column + 1);

            var queue = new Queue<Vertex>();
            queue.Enqueue(vFirst);
            while (true)
            {
                var vStart = queue.Dequeue();
                vStart.IsChecked = true;

                if (vStart.BalanceSignId == Sign.In)
                {
                    var vNexts =
                        _graph.EdgeList.Where(v => v.V1.Id == vStart.Id && !v.V2.IsChecked).Select(v => v.V2).ToList();
                    vNexts.ForEach(v => prevVert[v.Id - 1] = vStart.Id);
                    vNexts.ForEach(v => v.IsChecked = true);
                    if (vNexts.Any(v => v.Id == vEnd.Id))
                    {
                        break;
                    }
                    vNexts.ForEach(queue.Enqueue);
                }
                else
                {
                    var vNexts =
                        _graph.EdgeList.Where(v => v.V2.Id == vStart.Id && !v.V1.IsChecked)
                            .Select(v => v.V1)
                            .ToList();
                    vNexts.ForEach(v => prevVert[v.Id - 1] = vStart.Id);
                    vNexts.ForEach(v => v.IsChecked = true);
                    vNexts.ForEach(queue.Enqueue);
                }
            }

            var path = new List<Vertex> { vEnd };
            for (var i = vEnd.Id - 1; i != vFirst.Id - 1; i = prevVert[i] - 1)
            {
                path.Add(_graph.VertexList.Single(v => v.Id == prevVert[i]));
            }
            var pathEdges = new List<Edge>();
            for (var i = 0; i < path.Count - 1; i++)
            {
                pathEdges.Add(
                    _graph.EdgeList.Single(
                        e =>
                            (e.V1.Id == path[i].Id && e.V2.Id == path[i + 1].Id) ||
                            (e.V1.Id == path[i + 1].Id && e.V2.Id == path[i].Id)));
            }

            var cycle = new List<Point> { _fromPoint };
            cycle.AddRange(pathEdges.Select(pathEdge => new Point
            {
                Row = pathEdge.V1.Id - 1,
                Column = pathEdge.V2.Id - 1 - _vectFrom.GetLength(0)
            }));

            return cycle;
        }

        private void BuildGraph()
        {
            var listVertex = new List<Vertex>();
            for (var i = 0; i < _vectFrom.GetLength(0); i++)
            {
                listVertex.Add(new Vertex { Id = i + 1, BalanceSignId = Sign.In });
            }
            for (var i = 0; i < _vectTo.GetLength(0); i++)
            {
                listVertex.Add(new Vertex { Id = _vectFrom.GetLength(0) + i + 1, BalanceSignId = Sign.Out });
            }

            var edges = new List<Edge>();
            for (var i = 0; i < _initDecision.GetLength(0); i++)
            {
                for (var j = 0; j < _initDecision.GetLength(1); j++)
                {
                    if (!_initDecision[i, j].HasValue) continue;
                    var vFrom = listVertex.Single(v => v.Id == i + 1);
                    var vTo = listVertex.Single(v => v.Id == _vectFrom.GetLength(0) + j + 1);
                    edges.Add(new Edge(vFrom, vTo));
                }
            }

            _graph = new Graph(1) { EdgeList = edges, VertexList = listVertex };
        }
    }
}