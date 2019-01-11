using System;
using System.Collections.Generic;
using System.Linq;

namespace KingKong
{
    public class TransportTask
    {
        private readonly long[] _vectFrom;
        private readonly long[] _vectTo;
        private readonly long[,] _matrCost;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vectFrom">Вектор объемов поставок от поставщиков</param>
        /// <param name="vectTo">Вектор объемов потребления потребителей</param>
        /// <param name="matrCost">Матрица тарифов</param>
        public TransportTask(long[] vectFrom, long[] vectTo, long[,] matrCost)
        {
            _vectFrom = vectFrom;
            _vectTo = vectTo;
            _matrCost = matrCost;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public long?[,] RunOptimization()
        {
            long?[,] initDecision;
            bool isItSingularPlan;
            new InitialPlan(_vectFrom, _vectTo, _matrCost).InitialPlanBuilder(out initDecision, out isItSingularPlan);
            var sum = ObjectiveFunction(initDecision);
            long?[,] optimalPlan;
            while (true)
            {
                var matrCost = PotentialMethod(initDecision);
                var isItOptimal = IsItOptimalSolution(matrCost);
                if (isItOptimal)
                {
                    optimalPlan = initDecision;
                    break;
                }
                initDecision = TrafficRedistribution(initDecision, NewBasicCell(matrCost));
            }

            if (isItSingularPlan)
            {
                for (var i = 0; i < optimalPlan.GetLength(0); i++)
                {
                    for (var j = 0; j < optimalPlan.GetLength(1); j++)
                    {
                        if (!optimalPlan[i, j].HasValue) continue;
                        var p = optimalPlan[i, j].Value / 1000.0;
                        optimalPlan[i, j] = (int) Math.Round(p);
                    }
                }
            }
            sum = ObjectiveFunction(initDecision);
            
            return optimalPlan;
        }

        /// <summary>
        /// Целевая функция
        /// </summary>
        /// <param name="initDecision"></param>
        /// <returns></returns>
        public long ObjectiveFunction(long?[,] initDecision)
        {
            long sum = 0;

            for (var i = 0; i < initDecision.GetLength(0); i++)
            {
                for (var j = 0; j < initDecision.GetLength(1); j++)
                {
                    if (!initDecision[i, j].HasValue)
                    {
                        continue;
                    }
                    sum += initDecision[i, j].Value * _matrCost[i, j];
                }
            }

            return sum;
        }

        /// <summary>
        /// Метод потенциалов. Возвращает матрицу косвенных тарифов (новые тарифы для опорной матрицы)
        /// </summary>
        /// <param name="initialDecision">Метод потенциалов</param>
        /// <returns>Опорное решение</returns>
        private long[,] PotentialMethod(long?[,] initialDecision)
        {
            var matrCostNew = new long[_matrCost.GetLength(0), _matrCost.GetLength(1)];
            long?[] vectU, vectV;
            FindUV(initialDecision, out vectU, out vectV);

            for (var i = 0; i < matrCostNew.GetLength(0); i++)
            {
                for (var j = 0; j < matrCostNew.GetLength(1); j++)
                {
                    matrCostNew[i, j] = vectU[i].Value + vectV[j].Value;
                }
            }

            return matrCostNew;
        }

        /// <summary>
        /// Метод нахождения потенциалов ui, vj
        /// </summary>
        /// <param name="initialDecision"></param>
        /// <param name="vectU"></param>
        /// <param name="vectV"></param>
        private void FindUV(long?[,] initialDecision, out long?[] vectU, out long?[] vectV)
        {
            vectU = new long?[_vectFrom.GetLength(0)];
            vectV = new long?[_vectTo.GetLength(0)];
            vectU[0] = 1;
            var queueRow = new Queue<int>();
            var queueColumn = new Queue<int>();
            queueRow.Enqueue(0);

            while (queueRow.Any() || queueColumn.Any())
            {
                if (queueRow.Any())
                {
                    var row = queueRow.Dequeue();
                    for (var j = 0; j < initialDecision.GetLength(1); j++)
                    {
                        if (initialDecision[row, j].HasValue && !vectV[j].HasValue)
                        {
                            queueColumn.Enqueue(j);
                            vectV[j] = _matrCost[row, j] - vectU[row];
                        }
                    }
                }
                if (!queueColumn.Any()) continue;
                var column = queueColumn.Dequeue();
                for (var i = 0; i < initialDecision.GetLength(0); i++)
                {
                    if (initialDecision[i, column].HasValue && !vectU[i].HasValue)
                    {
                        queueRow.Enqueue(i);
                        vectU[i] = _matrCost[i, column] - vectV[column];
                    }
                }
            }
        }

        /// <summary>
        /// Проверка на оптимальность решения
        /// </summary>
        /// <param name="matrCostNew"></param>
        /// <returns></returns>
        private bool IsItOptimalSolution(long[,] matrCostNew)
        {
            var isTheBest = true;

            for (var i = 0; i < matrCostNew.GetLength(0); i++)
            {
                for (var j = 0; j < matrCostNew.GetLength(1); j++)
                {
                    if (matrCostNew[i, j] > _matrCost[i, j])
                    {
                        isTheBest = false;
                        break;
                    }
                }
            }

            return isTheBest;
        }

        /// <summary>
        /// Новая базисная клетка
        /// </summary>
        /// <param name="matrCostNew"></param>
        /// <returns></returns>
        private Point NewBasicCell(long[,] matrCostNew)
        {
            var asterix = new Point();
            long maxValue = 0;

            for (var i = 0; i < matrCostNew.GetLength(0); i++)
            {
                for (var j = 0; j < matrCostNew.GetLength(1); j++)
                {
                    if (matrCostNew[i, j] <= _matrCost[i, j]) continue;
                    if (maxValue > matrCostNew[i, j] - _matrCost[i, j]) continue;
                    maxValue = matrCostNew[i, j] - _matrCost[i, j];
                    asterix = new Point { Row = i, Column = j };
                }
            }
            return asterix;
        }

        /// <summary>
        /// Перераспределение поставок
        /// </summary>
        /// <param name="initDecision"></param>
        /// <param name="newcell"></param>
        private long?[,] TrafficRedistribution(long?[,] initDecision, Point newcell)
        {
            var cycle = new FindWay(initDecision, newcell, _vectFrom, _vectTo).FindCycle();

            // расставить +- и найти мин перевозку
            var min = initDecision[cycle[1].Row, cycle[1].Column].Value;
            var oldAsterix = cycle[1];
            for (var i = 3; i < cycle.Count; i = i + 2)
            {
                if (initDecision[cycle[i].Row, cycle[i].Column] > min) continue;
                min = initDecision[cycle[i].Row, cycle[i].Column].Value;
                oldAsterix = cycle[i];
            }

            // вычесть значение мин перевозки из минус элементов и прибавить значение мин перевозки к плюс элементам
            for (var i = 0; i < cycle.Count; i++)
            {
                if (i % 2 == 0)
                {
                    initDecision[cycle[i].Row, cycle[i].Column] += min;
                }
                else
                {
                    initDecision[cycle[i].Row, cycle[i].Column] -= min;
                }
            }
            initDecision[oldAsterix.Row, oldAsterix.Column] = null;
            initDecision[newcell.Row, newcell.Column] = min;

            return initDecision;
        }
    }

    public struct Point
    {
        /// <summary>
        /// номер строки
        /// </summary>
        public int Row;

        /// <summary>
        /// номер столбца
        /// </summary>
        public int Column;

        /// <summary>
        /// значение элемента (стоимости)
        /// </summary>
        public long Value;
    }

    public class PointComparer : IEqualityComparer<Point>
    {
        public bool Equals(Point x, Point y)
        {
            //Check whether the products' properties are equal.
            return x.Row == y.Row && x.Column == y.Column;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.
        public int GetHashCode(Point point)
        {
            //Get hash code for the Name field if it is not null.
            var hashAsterixRow = point.Row > -1 ? 0 : point.Row.GetHashCode();

            //Get hash code for the Code field.
            var hashAsterixColumn = point.Column.GetHashCode();

            //Calculate the hash code for the product.
            return hashAsterixRow ^ hashAsterixColumn;
        }
    }
}
