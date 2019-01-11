using System;
using GazRouter.DTO.Dictionaries.BalanceSigns;
using GazRouter.DTO.Dictionaries.EntityTypes;

namespace GazRouter.Graph
{
    public class Vertex : IComparable
    {
        /// <summary>
        /// ID вершины
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// ID объекта модели
        /// </summary>
        public Guid? EntityId { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EntityType Type { get; set; }

        /// <summary>
        /// Давление, кг/см²
        /// </summary>
        public double? Pressure { get; set; }

        /// <summary>
        /// Расход, тыс. м³/с
        /// </summary>
        public double? Consumption { get; set; }
        
        /// <summary>
        /// Температура, К
        /// </summary>
        public double? Temperature { get; set; }

        /// <summary>
        /// Плотность газа, кг/м³
        /// </summary>
        public double? Density { get; set; }

        /// <summary>
        /// Температура грунта, К
        /// </summary>
        public double? TSoil { get; set; }

        /// <summary>
        /// Метка вершины - принадлежность вершины графу (для проверки связности графа)
        /// </summary>
        public bool IsChecked { get; set; }

        // Знак в балансе(In - '+', Out - '-')
        public Sign BalanceSignId { get; set; }

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return -1;
            }

            var v = obj as Vertex;
            if (v.Id > Id)
            {
                return -1;
            }

            return 1;

        }

    }

}