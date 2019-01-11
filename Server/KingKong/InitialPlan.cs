using System;
using System.Collections.Generic;
using System.Linq;

namespace KingKong
{
    public class InitialPlan
    {
        private readonly long[] _vectFrom;
        private readonly long[] _vectTo;
        private readonly long[,] _matrCost;
        private int _basicCellCount;
        private long?[,] _initialPlan;

        public InitialPlan(long[] vectFrom, long[] vectTo, long[,] matrCost)
        {
            _vectFrom = vectFrom;
            _vectTo = vectTo;
            _matrCost = matrCost;
            _basicCellCount = 0;
        }

        public void InitialPlanBuilder(out long?[,] initialPlan, out bool isInitialPlanSingular)
        {
            isInitialPlanSingular = false;
            MethodDualPreferences();
            if (_basicCellCount < _vectFrom.GetLength(0) + _vectTo.GetLength(0) - 1)
            {
                isInitialPlanSingular = true;
                _basicCellCount = 0;
                IfMatrixIsSingular();
                MethodDualPreferences();
            }

            initialPlan = _initialPlan;
        }

        /// <summary>
        /// Построение опорного плана методом двойного предпочтения
        /// </summary>
        public void MethodDualPreferences()
        {
            _initialPlan = new long?[_vectFrom.GetLength(0), _vectTo.GetLength(0)];
            var vectFrom = CopyVect(_vectFrom);
            var vectTo = CopyVect(_vectTo);

            var bestPoints = BestPoints();
            var mainPriority =
                bestPoints.Where(a => bestPoints.Count(aa => aa.Row == a.Row && aa.Column == a.Column) > 1)
                    .Distinct(new PointComparer()).ToList();

            mainPriority.OrderBy(mp => mp.Value)
                .ThenByDescending(mp => Math.Min(vectFrom[mp.Row], vectTo[mp.Column]))
                .ToList()
                .ForEach(p => Step(p, vectFrom, vectTo));

            var secondPriority = bestPoints.Except(mainPriority).Distinct(new PointComparer()).ToList();
            secondPriority.OrderBy(sp => _matrCost[sp.Row, sp.Column]).ToList().ForEach(p => Step(p, vectFrom, vectTo));

            // Остальные ячейки
            var otherPoints = NonAsterixes(vectFrom, vectTo).OrderBy(na => na.Value).ToList();
            while (vectTo.Any(v => v > 0.0) || vectFrom.Any(v => v > 0.0))
            {
                var minNonAsterix = otherPoints.First();
                Step(minNonAsterix, vectFrom, vectTo);

                //if (Equals(vectFrom[minNonAsterix.Row], 0))
                //{
                //    otherPoints.RemoveAll(na => na.Row == minNonAsterix.Row);
                //}
                //if (Equals(vectTo[minNonAsterix.Column], 0))
                //{
                //    otherPoints.RemoveAll(na => na.Column == minNonAsterix.Column);
                //}

                if (vectFrom[minNonAsterix.Row] == 0)
                {
                    otherPoints.RemoveAll(na => na.Row == minNonAsterix.Row);
                }
                if (vectTo[minNonAsterix.Column] == 0)
                {
                    otherPoints.RemoveAll(na => na.Column == minNonAsterix.Column);
                }
            }
        }

        private static long[] CopyVect(long[] vect)
        {
            var newVect = new long[vect.GetLength(0)];
            for (var i = 0; i < vect.GetLength(0); i++)
            {
                newVect[i] = vect[i];
            }

            return newVect;
        }

        private void Step(Point point, long[] vectFrom, long[] vectTo)
        {
            var min = Math.Min(vectFrom[point.Row], vectTo[point.Column]);
            //if (Equals(min, 0))
            //{
            //    return;
            //}
            if (min == 0)
            {
                return;
            }

            vectFrom[point.Row] -= min;
            vectTo[point.Column] -= min;

            _initialPlan[point.Row, point.Column] = min;
            _basicCellCount++;
        }

        /// <summary>
        /// Если план перевозок оказался вырожденным, то обном объемы поставок и потребления, прибавив ко всем VFrom некое маленькое число eps 
        /// и к одному из элементов вектора VTo - count(VFrom)*eps
        /// </summary>
        private void IfMatrixIsSingular()
        {
            //Пусть eps = 0.005. Тк тип данных - int, 
            //то умножим все элементы на 1000 и будем работать с int b = 0.005*1000 
            const long a = 1000;
            const long b = 5;
            for (var i = 0; i < _vectFrom.GetLength(0); i++)
            {
                _vectFrom[i] = _vectFrom[i] * a + b;
            }
            for (var i = 0; i < _vectTo.GetLength(0); i++)
            {
                _vectTo[i] *= a;
            }
            _vectTo[_vectTo.GetLength(0) - 1] += _vectFrom.GetLength(0) * b;
        }

        /// <summary>
        /// Поиск минимальной стоимости в _matrCost среди неотмеченных
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Point> NonAsterixes(long[] vectFrom, long[] vectTo)
        {
            var asterixes = new List<Point>();
            for (var i = 0; i < _matrCost.GetLength(0); i++)
            {
                //if (Equals(vectFrom[i], 0))
                //{
                //    continue;
                //}
                if (Math.Abs(vectFrom[i]) > 0)
                    for (var j = 0; j < _matrCost.GetLength(1); j++)
                    {
                        //if (Equals(vectTo[j], 0))
                        //{
                        //    continue;
                        //}
                        if (Math.Abs(vectTo[j]) > 0)
                            asterixes.Add(new Point { Row = i, Column = j, Value = _matrCost[i, j] });
                    }
            }

            return asterixes;
        }

        private List<Point> BestPoints()
        {
            var asterixes = new List<Point>();

            // Минимальный элемент в строке
            for (var i = 0; i < _matrCost.GetLength(0); i++)
            {
                long min = _matrCost[i, 0];
                var asterix = new Point { Row = i, Column = 0 };

                for (var j = 1; j < _matrCost.GetLength(1); j++)
                {
                    if (_matrCost[i, j] < min)
                    {
                        min = _matrCost[i, j];
                        asterix.Column = j;
                    }
                }
                asterixes.Add(asterix);
            }

            // Минимальный элемент в столбце
            for (var j = 0; j < _matrCost.GetLength(1); j++)
            {
                long min = _matrCost[0, j];
                var asterix = new Point { Row = 0, Column = j };

                for (var i = 1; i < _matrCost.GetLength(0); i++)
                {
                    if (_matrCost[i, j] < min)
                    {
                        min = _matrCost[i, j];
                        asterix.Row = i;
                    }
                }
                asterixes.Add(asterix);
            }

            return asterixes;
        }
    }
}