using System;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.DTO.Dictionaries.PipelineTypes;

namespace GazRouter.Graph
{
    public class Edge
    {
        public Edge(Vertex v1, Vertex v2)
        {
            V1 = v1;
            V2 = v2;
        }

        public Vertex V1 { get; set; }
        public Vertex V2 { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Диаметр, мм
        /// </summary>
        public double Diameter { get; set; }

        /// <summary>
        /// Максимальное разрешенное давление, кг/см²
        /// </summary>
        public double PressureMax { get; set; }

        /// <summary>
        /// Километр начала
        /// </summary>
        public double KilometerOfStartPoint { get; set; }

        /// <summary>
        /// Километр конца
        /// </summary>
        public double KilometerOfEndPoint { get; set; }

        /// <summary>
        /// ID объекта
        /// </summary>
        public Guid EntityId { get; set; }

        /// <summary>
        /// Тип объекта
        /// </summary>
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Метка - Входит ли ребро в максимальное дерево
        /// </summary>
        public bool IsItMaxTree { get; set; }

        /// <summary>
        /// Расход, тыс.м³/ч
        /// </summary>
        public double? Consumption { get; set; }

        /// <summary>
        /// Коэффициент гидравлической эффективности
        /// (учитывает фактическое состояние лин участка)
        /// </summary>
        public double? CoefficientOfEfficiency { get; set; }

        /// <summary>
        /// Коэффициент расхода (зависит от диаметра)
        /// </summary>
        public double CoefficientOfFlow { get; set; }

        public PipelineType TypeOfPipeline { get; set; }

        public double? GasVolume { get; set; }
    }
}