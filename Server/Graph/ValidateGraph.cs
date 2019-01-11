using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DAL.Core;
using GazRouter.DAL.ObjectModel.Pipelines;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.ObjectModel.Pipelines;

namespace GazRouter.Graph
{
    public class ValidateGraph
    {
        private readonly Graph _graph;

        public ValidateGraph(Graph graph)
        {
            graph.VertexList.ForEach(v => v.IsChecked = false);
            _graph = graph;
        }
        
        /// <summary>
        /// Проверка на связность графа
        /// </summary>
        /// <returns></returns>
        public bool GraphConnectivity()
        {
            var connectivity = true;

            var vFirst = _graph.VertexList.First(v =>_graph.EdgeList.Count(e => e.V1 == v) == 1);
            // _graph.VertexList.First(v => v.Type == EntityType.MeasLine && _graph.EdgeList.Count(e => e.V1 == v) == 1);
            vFirst.IsChecked = true;

            var queue = new Queue<Vertex>();
            queue.Enqueue(vFirst);

            while (queue.Count > 0)
            {
                var neighbors = AdjacentVertices(queue.Dequeue());

                if (!neighbors.Any()) continue;
                neighbors.ForEach(v => v.IsChecked = true);
                neighbors.ForEach(v => queue.Enqueue(v));
            }
            
            if (_graph.VertexList.Any(vertex => vertex.IsChecked == false))
            {
                connectivity = false;
            }

            return connectivity;
        }

        /// <summary>
        /// Несвязные газопроводы
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, List<Kilometers>> DisjointedPipelinesId()
        {
            var dict = new Dictionary<Guid, List<Kilometers>>();
            
            var edges = _graph.EdgeList.Where(e => e.V1.IsChecked == false || e.V2.IsChecked == false).ToList();
            
            foreach (var edge in edges)
            {
                //if (edge.EntityType != EntityType.Pipeline)
                //    continue;

                if (dict.ContainsKey(edge.EntityId))
                    dict[edge.EntityId].Add(new Kilometers
                    {
                        KmStart = edge.KilometerOfStartPoint,
                        kmEnd = edge.KilometerOfEndPoint
                    });
                else
                {
                    dict.Add(edge.EntityId, new List<Kilometers>
                    {
                        new Kilometers
                        {
                            KmStart = edge.KilometerOfStartPoint,
                            kmEnd = edge.KilometerOfEndPoint
                        }
                    });
                }

                //if (pipesId.All(p => p != edge.EntityId))
                //    pipesId.Add(edge.EntityId);
            }

            return dict;
        }

        /// <summary>
        /// Смежные вершины
        /// </summary>
        /// <param name="vrt"></param>
        /// <returns></returns>
        public List<Vertex> AdjacentVertices(Vertex vrt)
        {
            var vertices = new List<Vertex>();
            _graph.EdgeList.Where(e => e.V1 == vrt && e.V2.IsChecked == false).ToList().ForEach(e=> vertices.Add(e.V2));
            _graph.EdgeList.Where(e => e.V2 == vrt && e.V1.IsChecked == false).ToList().ForEach(e => vertices.Add(e.V1));
            
            return vertices;
        } 

    }

    public class Kilometers
    {
        public double KmStart { get; set; }

        public double kmEnd { get; set; }

        public double Cons { get; set; }
    }
}