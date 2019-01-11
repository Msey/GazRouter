using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.EntityTypes;
using GazRouter.Graph;
using GazRouter.Prediction;
using Utils.Calculations;
using Utils.Units;

namespace KingKong
{
    public class GasInPipeCalculation
    {
        private readonly Graph _graph;
        private readonly Vertex _root;

        public GasInPipeCalculation(Graph graph, Vertex root)
        {
            _graph = graph;
            _root = root;
            _graph.VertexList.ForEach(v => v.IsChecked = false);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Calculate()
        {
            var vLevels = new MaxTree(_graph, _root).BuildMaxTree();

            // Матрицы построения
            var helpfulMatrices = new HelpfulMatrices(_graph, _root);
            var eIndeces = helpfulMatrices.EdgeIndeces();
            var vIndeces = helpfulMatrices.VertexIndeces();
            var extendedCyclomMatr = helpfulMatrices.ExtendedCyclomaticMatr(eIndeces, vLevels);
            var cyclomMatr = helpfulMatrices.CyclomaticMatr(extendedCyclomMatr);
            var rd = helpfulMatrices.Rd(eIndeces, vLevels, vIndeces);

            var xTree = new double[_graph.EdgeList.Count(e => e.IsItMaxTree)];
            var xChord = StartingValuesForXChord(eIndeces);
            var matrLambda = MatrixLambda(eIndeces);
            var vectQ = VectQ(vIndeces);
            var rdTrQ = MyMath.MultiplicationMatrVect(MyMath.TransposeMatrix(rd), vectQ);
            var cyclomMatrTr = MyMath.TransposeMatrix(cyclomMatr);
            var extCyclMatrTr = MyMath.TransposeMatrix(extendedCyclomMatr);

            var eps = double.MaxValue;
            var bl = MyMath.MultiplicationDiagonalMatrix(extendedCyclomMatr, matrLambda);

            while (Math.Abs(eps) > 0.01)
            {
                // xд = Rд_tr*Q + Bд_tr*xк(N)
                var bdTrxk = MyMath.MultiplicationMatrVect(cyclomMatrTr, xChord);
                xTree = MyMath.SumVectors(rdTrQ, bdTrxk);
                var matrX = MatrixX(xChord, xTree);

                // Δxк(N) = (BɅX(N)B_tr)^(-1) * (-1/2BɅX(N)x(N))
                var blx = MyMath.MultiplicationDiagonalMatrix(bl, matrX);
                var blxBtr = MyMath.MultiplicationMatrix(blx, extCyclMatrTr);
                var minustwoBlXx = MyMath.MultiplicationMatrVect(MyMath.MultiplicationMatrScalar(blx, -1.0 / 2.0),
                    VectorX(xChord, xTree));
                var deltaXchord = MyMath.MultiplicationMatrVect(MyMath.InverseMatrix(blxBtr), minustwoBlXx);

                // xк(N+1) = xк(N) + Δxк(N)
                xChord = MyMath.SumVectors(xChord, deltaXchord);

                if (deltaXchord.GetLength(0) == 0)
                {
                    break;
                }
                eps = Math.Abs(MyMath.MaxNorm(deltaXchord)); // eps = ||Δxк|| 
            }

            // Присваивание ребрам графа полученные результаты расходов
            for (var i = 0; i < xTree.GetLength(0); i++)
            {
                var ind = eIndeces.Single(index => index.Value == (i + xChord.GetLength(0)));
                _graph.EdgeList.Single(e => e == ind.Key).Consumption = xTree[i];
            }
            for (var i = 0; i < xChord.GetLength(0); i++)
            {
                var ind = eIndeces.Single(index => index.Value == i);
                _graph.EdgeList.Single(e => e == ind.Key).Consumption = xChord[i];
            }
        }

        public Dictionary<Vertex, double> CalculatePressure()
        {
            var epsP = new Dictionary<Vertex, double>();
            var vertexLevel = new MaxTree(_graph, _root).BuildMaxTree();
            var helpfulMatrices = new HelpfulMatrices(_graph, _root);
            var indeces = helpfulMatrices.EdgeIndeces();
            var vertexIndex = helpfulMatrices.VertexIndeces();

            var tree = new Graph(_graph.SystemId)
            {
                EdgeList = _graph.EdgeList.Where(e => e.IsItMaxTree).ToList(),
                VertexList = _graph.VertexList
            };

            var coefEffic = new CoefficientOfEfficiency(tree, _root, vertexLevel);
            double[,] atrInvNew;
            Dictionary<Vertex, Vertex> roots;
            coefEffic.CalculateCoefficientOfEfficiency(out atrInvNew, out roots);

            var pressureVector = new double[vertexIndex.Count];
            var lXx = LXx(indeces);

            var pKvadrat = MyMath.MultiplicationMatrVect(atrInvNew, lXx);
            for (var i = 0; i < pKvadrat.GetLength(0); i++)
            {
                var vertexI = vertexIndex.Single(vi => vi.Value == i).Key;
                if (roots.ContainsKey(vertexI))
                {
                    pKvadrat[i] += Math.Pow(roots[vertexI].Pressure.Value, 2);
                }
                else
                {
                    pKvadrat[i] += Math.Pow(_root.Pressure.Value, 2);
                }
                pressureVector[i] = Math.Sqrt(pKvadrat[i]);
            }

            for (var i = 0; i < pressureVector.GetLength(0); i++)
            {
                var vertex = vertexIndex.Single(vi => vi.Value == i).Key;
                var t = pressureVector[i];
                if (vertex.Pressure.HasValue)
                {
                    epsP.Add(vertex, vertex.Pressure.Value - t);
                }
                vertex.Pressure = t;
            }

            return epsP;
        }

        #region Вспомогательные методы для Calculate

        /// <summary>
        /// Средняя температура газа нитки магистрального газопровода, К
        /// (Согласно Методике определения запаса газа газотранспортных предприятий)
        /// </summary>
        /// <param name="tIn">Температура газа в начале участка газопровода, К</param>
        /// <param name="tOut">Температура газа в конце участка газопровода, К</param>
        /// <param name="tSoil">Температура грунта на глубине заложения оси газопровода, К</param>
        /// <returns></returns>
        private static double TemperatureAverage(double tIn, double tOut, double tSoil)
        {
            if (tIn == tOut)
            {
                return tIn;
            }
            return tSoil + (tIn - tOut) / Math.Log(Math.Abs((tIn - tSoil) / (tOut - tSoil)));
        }

        /// <summary>
        /// Коэффициент гидравлического сопротивления
        /// (частный случай при к = 0,03мм)
        /// согласно Трубопроводный транспорт нефти и газа, 1987, Белоусов, пар.6.4
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double SimplifiedLambda(double d)
        {
            return 0.03817 / Math.Pow(d, 0.2);
        }

        /// <summary>
        /// Диагональная матрица с элементами |xi| - расход по дуге i 
        /// </summary>
        /// <param name="chordEdge"></param>
        /// <param name="treeEdge"></param>
        private double[,] MatrixX(double[] chordEdge, double[] treeEdge)
        {
            var matrX = new double[_graph.EdgeList.Count, _graph.EdgeList.Count];
            var notTreeCount = _graph.EdgeList.Count(e => !e.IsItMaxTree);

            for (var i = 0; i < notTreeCount; i++)
            {
                matrX[i, i] = Math.Abs(chordEdge[i]);
            }

            for (var i = 0; i < _graph.EdgeList.Count(e => e.IsItMaxTree); i++)
            {
                matrX[i + notTreeCount, i + notTreeCount] = Math.Abs(treeEdge[i]);
            }

            return matrX;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="chordEdge"></param>
        /// <param name="treeEdge"></param>
        /// <returns></returns>
        private double[] VectorX(double[] chordEdge, double[] treeEdge)
        {
            var vectX = new double[_graph.EdgeList.Count];
            var notTreeCount = _graph.EdgeList.Count(e => !e.IsItMaxTree);
            for (var i = 0; i < notTreeCount; i++)
            {
                vectX[i] = chordEdge[i];
            }

            for (var i = 0; i < _graph.EdgeList.Count(e => e.IsItMaxTree); i++)
            {
                vectX[i + notTreeCount] = treeEdge[i];
            }

            return vectX;
        }

        /// <summary>
        /// Задание начальных значений вектора ХChord
        /// (матрица значений расхода по хордам)
        /// </summary>
        /// <param name="eIndeces"></param>
        /// <returns></returns>
        private double[] StartingValuesForXChord(Dictionary<Edge, int> eIndeces)
        {
            var vectChordX = new double[_graph.EdgeList.Count(e => !e.IsItMaxTree)];
            var minCons = _graph.VertexList.Min(v => v.Consumption);
            _graph.EdgeList.Where(e => !e.IsItMaxTree).ToList().ForEach(ee => vectChordX[eIndeces[ee]] = minCons.Value);

            return vectChordX;
        }

        /// <summary> 
        /// Вектор внешних притоков Q, тыс. м3/ч.
        /// Компонента вектора отрицательная, если имеет место отбор, и положительная в случае притока 
        /// </summary>
        /// <param name="vIndeces"></param>
        /// <returns></returns>
        private double[] VectQ(Dictionary<Vertex, int> vIndeces)
        {
            var vectQ = new double[_graph.VertexList.Count - 1];
            foreach (var v in _graph.VertexList)
            {
                if (v.Id == _root.Id)
                {
                    continue;
                }

                var rowIndex = vIndeces[v];

                if (v.Consumption.HasValue)
                {
                    vectQ[rowIndex] = v.Consumption.Value;
                }

                else
                {
                    vectQ[rowIndex] = 0;
                }
            }

            return vectQ;
        }

        /// <summary>
        /// Задание начальных значений матрицы Lambda
        /// (диаг. матрица значений обобщ. коэф-та гидравл сопротивления).
        /// СТО Газпром 2-3.5-051-2006, п.18.5.2 
        /// </summary>
        /// <param name="eIndeces"></param>
        /// <returns></returns>
        private double[,] MatrixLambda(Dictionary<Edge, int> eIndeces)
        {
            var matrLambda = new double[_graph.EdgeList.Count, _graph.EdgeList.Count];

            foreach (var edge in _graph.EdgeList)
            {
                var column = eIndeces[edge];
                const double densityRandom = 0.7;
                const double tRandom = 273.15;
                const int pRandom = 36;
                matrLambda[column, column] = Lambda(edge, densityRandom, tRandom, pRandom);
            }

            return matrLambda;
        }

        private double Lambda(Edge edge, double ro, double temperature, double pressure)
        {
            var density = edge.V1.Density.HasValue
                ? Density.FromKilogramsPerCubicMeter(edge.V1.Density.Value)
                : edge.V2.Density.HasValue
                    ? Density.FromKilogramsPerCubicMeter(edge.V2.Density.Value)
                    : Density.FromKilogramsPerCubicMeter(ro);

            var tIn = edge.V1.Temperature;
            var tOut = edge.V2.Temperature;
            var tSoil = edge.V1.TSoil ?? edge.V2.TSoil;
            var tAverage = tIn.HasValue && tOut.HasValue && tSoil.HasValue
                ? TemperatureAverage(tIn.Value, tOut.Value, tSoil.Value)
                : temperature;
            var z =
                SupportCalculations.GasCompressibilityFactorApproximate(Pressure.From(pressure, PressureUnit.Kgh),
                    Temperature.From(tAverage, TemperatureUnit.Kelvin), density);
            var delta = density.KilogramsPerCubicMeter /
                        StandardConditions.DensityOfAir.KilogramsPerCubicMeter;
            var length = Math.Abs(edge.KilometerOfEndPoint - edge.KilometerOfStartPoint);
            var perevodEdinic = Math.Pow(10.2, 2) * Math.Pow(24.0 / 1000, 2);
            var lambda = perevodEdinic * Math.Pow(3.32, -2) * Math.Pow(10, 12) * delta * tAverage * z *
                         Math.Pow(edge.Diameter, -5) * length * SimplifiedLambda(edge.Diameter);

            return lambda;
        }
        #endregion

        #region Вспомогательные методы для CalculatePressure
        private double[] LXx(Dictionary<Edge, int> indeces)
        {
            var n = _graph.VertexList.Count - 1;
            var matr = new double[n];

            var l = _graph.EdgeList.Count(e => !e.IsItMaxTree);
            foreach (var edge in _graph.EdgeList.Where(e => e.IsItMaxTree))
            {
                var index = indeces[edge] - l;

                if (edge.V2.Pressure.HasValue && edge.V1.Pressure.HasValue)
                {
                    matr[index] = Math.Pow(edge.V1.Pressure.Value, 2.0) - Math.Pow(edge.V2.Pressure.Value, 2.0);
                }
                else
                {
                    if (edge.Consumption.HasValue)
                    {
                        var roTz = 180;
                        //var roTz = edge.V2.Density.Value*edge.V2.Temperature.Value*0.9;
                        var length = edge.KilometerOfEndPoint - edge.KilometerOfStartPoint;
                        var lambda = Math.Pow(10.2, 2) * Math.Pow(24.0 / 1000.0, 2.0) * Math.Pow(3.32, -2) * Math.Pow(10, 12) *
                                     roTz * Math.Pow(edge.Diameter, -5.0) * length *
                                     SimplifiedLambda(edge.Diameter) /
                                     StandardConditions.DensityOfAir.KilogramsPerCubicMeter;
                        var e = edge.CoefficientOfEfficiency.HasValue
                            ? Math.Pow(edge.CoefficientOfEfficiency.Value, 2.0)
                            : 1.0;
                        var a = Math.Abs(edge.Consumption.Value) * edge.Consumption.Value * lambda / e;
                        matr[index] = a;
                    }
                }
            }
            return matr;
        }
        #endregion
    }

    public class CoefficientOfEfficiency
    {
        private readonly Graph _graph;
        private readonly Vertex _root;
        private readonly Dictionary<Vertex, int> _levels;

        public CoefficientOfEfficiency(Graph graph, Vertex root, Dictionary<Vertex, int> levels)
        {
            _graph = graph;
            _root = root;
            _graph.EdgeList.ForEach(e => e.IsItMaxTree = true);
            _levels = levels;
        }

        public void CalculateCoefficientOfEfficiency(out double[,] aTrInvNew, out Dictionary<Vertex, Vertex> rootsNew)
        {
            var helpfulMatr = new HelpfulMatrices(_graph, _root);
            var edgesOrder = helpfulMatr.EdgeIndeces();
            var verticesOrder = new Dictionary<Vertex, int>();
            _graph.VertexList.Where(v => v.Id != _root.Id)
                .ToList()
                .ForEach(v => verticesOrder.Add(v, verticesOrder.Count));
            var incidenceMatr = helpfulMatr.IncidenceMatrix(edgesOrder, verticesOrder);

            var aTr = MyMath.TransposeMatrix(incidenceMatr);
            var aTrInv = MyMath.InverseMatrix(aTr);
            var lxx = LXx(edgesOrder);

            aTrInvNew = new double[aTrInv.GetLength(0), aTrInv.GetLength(1)];
            rootsNew = new Dictionary<Vertex, Vertex>();

            foreach (var level in _levels.OrderByDescending(l => l.Value))
            {
                if (!level.Key.Pressure.HasValue || level.Key.Id == _root.Id ||
                    level.Key.Type == new EntityType())
                {
                    continue;
                }

                var row = verticesOrder[level.Key];
                var edges = Way(aTrInv, row, edgesOrder);
                var verticesSorted = SortList(edges, _root, level.Key);

                for (var i = 1; i < verticesSorted.Count; i++)
                {
                    if (!rootsNew.ContainsKey(verticesSorted[i]))
                    {
                        rootsNew.Add(verticesSorted[i], verticesSorted.First());
                    }
                }

                double x = 0;
                var pInSqr = Math.Pow(verticesSorted.First().Pressure.Value, 2.0);
                var pOutSqr = Math.Pow(verticesSorted.Last().Pressure.Value, 2.0);
                var pressureSqrVector = new double[verticesSorted.Count];
                pressureSqrVector[0] = pInSqr;
                pressureSqrVector[pressureSqrVector.GetLength(0) - 1] = pOutSqr;
                var newEdges = new List<Edge>();
                for (var i = 0; i < verticesSorted.Count - 1; i++)
                {
                    var edge =
                        edges.Single(
                            e =>
                                (e.V1.Id == verticesSorted[i].Id && e.V2.Id == verticesSorted[i + 1].Id) ||
                                (e.V1.Id == verticesSorted[i + 1].Id && e.V2.Id == verticesSorted[i].Id));
                    newEdges.Add(edge);
                    var column = edgesOrder[edge];
                    aTrInvNew[row, column] = aTrInv[row, column];
                    for (var j = i + 1; j < verticesSorted.Count - 1; j++)
                    {
                        var rowi = verticesOrder[verticesSorted[j]];
                        aTrInvNew[rowi, column] = aTrInv[rowi, column];
                    }
                }

                var length = newEdges.Sum(e => Math.Abs(e.KilometerOfEndPoint - e.KilometerOfStartPoint));
                for (var i = 0; i < newEdges.Count; i++)
                {
                    if (newEdges[i].V1.Pressure.HasValue && newEdges[i].V2.Pressure.HasValue)
                    {
                        continue;
                    }

                    x += Math.Abs(newEdges[i].KilometerOfEndPoint - newEdges[i].KilometerOfStartPoint);
                    var pressureSqr = pInSqr - (pInSqr - pOutSqr) * x / length;
                    pressureSqrVector[i + 1] = pressureSqr;

                    if (newEdges[i].CoefficientOfEfficiency.HasValue)
                    {
                        continue;
                    }

                    var coef = Math.Sqrt(Math.Abs(lxx[edgesOrder[newEdges[i]]] / (pressureSqrVector[i] - pressureSqr)));
                    if (double.IsNaN(coef) || coef == 0.0)
                    {
                        coef = 1.0;
                    }
                    newEdges[i].CoefficientOfEfficiency = coef;
                }
            }

            for (var i = 0; i < aTrInvNew.GetLength(0); i++)
            {
                var sumAbs = 0.0;
                for (var j = 0; j < aTrInvNew.GetLength(1); j++)
                {
                    sumAbs += Math.Abs(aTrInvNew[i, j]);
                }
                if (sumAbs < 1)
                {
                    for (var j = 0; j < aTrInv.GetLength(1); j++)
                    {
                        aTrInvNew[i, j] = aTrInv[i, j];
                    }
                }
            }
        }

        /// <summary>
        /// Коэффициент гидравлического сопротивления
        /// (частный случай при к = 0,03мм)
        /// согласно Трубопроводный транспорт нефти и газа, 1987, Белоусов, пар.6.4
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        private static double SimplifiedLambda(double d)
        {
            return 0.03817 / Math.Pow(d, 0.2);
        }
        private static List<Edge> Way(double[,] aTrInv, int row, Dictionary<Edge, int> edgesOrder)
        {
            var edges = new List<Edge>();
            for (var i = 0; i < aTrInv.GetLength(0); i++)
            {
                if (aTrInv[row, i] == 1.0 || aTrInv[row, i] == -1.0)
                {
                    edges.Add(edgesOrder.Single(e => e.Value == i).Key);
                }
            }
            return edges;
        }

        private double[] LXx(Dictionary<Edge, int> indeces)
        {
            var n = _graph.VertexList.Count - 1;
            var matr = new double[n];

            var l = _graph.EdgeList.Count(e => !e.IsItMaxTree);
            foreach (var edge in _graph.EdgeList.Where(e => e.IsItMaxTree))
            {
                var index = indeces[edge] - l;
                if (edge.V2.Pressure.HasValue && edge.V1.Pressure.HasValue)
                {
                    matr[index] = Math.Pow(edge.V1.Pressure.Value, 2.0) - Math.Pow(edge.V2.Pressure.Value, 2.0);
                }
                else
                {
                    if (edge.Consumption.HasValue)
                    {
                        var roTz = 180;
                        var length = edge.KilometerOfEndPoint - edge.KilometerOfStartPoint;
                        var lambda = Math.Pow(10.2, 2) * Math.Pow(24.0 / 1000.0, 2.0) * Math.Pow(3.32, -2) * Math.Pow(10, 12) *
                                     roTz * Math.Pow(edge.Diameter, -5) * length *
                                     SimplifiedLambda(edge.Diameter) /
                                     StandardConditions.DensityOfAir.KilogramsPerCubicMeter;

                        matr[index] = Math.Abs(edge.Consumption.Value) * edge.Consumption.Value * lambda;
                    }
                }
            }
            return matr;
        }

        private List<Vertex> SortList(List<Edge> edges, Vertex vFrom, Vertex vTo)
        {
            var sortedList = new List<Vertex> { vFrom };
            var edgesNew = new List<Edge>();

            while (vFrom.Id != vTo.Id)
            {
                var edge =
                    edges.Single(e => (e.V1.Id == vFrom.Id || e.V2.Id == vFrom.Id) && edgesNew.All(es => es != e));
                edgesNew.Add(edge);
                vFrom = edge.V1.Id == vFrom.Id ? edge.V2 : edge.V1;
                sortedList.Add(vFrom);
            }

            for (var i = sortedList.Count - 2; i > -1; i--)
            {
                if (sortedList[i].Pressure.HasValue && i > 0)
                {
                    sortedList.RemoveRange(0, i);
                    break;
                }
            }
            return sortedList;
        }
    }

    public class HelpfulMatrices
    {
        private readonly Graph _graph;
        private readonly Vertex _root;

        public HelpfulMatrices(Graph graph, Vertex root)
        {
            _graph = graph;
            _graph.VertexList.Sort();
            _root = root;
        }

        /// <summary>
        /// Расширенная цикломатическая матрица (строка есть независ цикл)
        /// Имеет вид [E|B_д]. Слева - хорды, образующие циклы, сверху - хорды и дуги дерева.
        /// </summary>
        public double[,] ExtendedCyclomaticMatr(Dictionary<Edge, int> indeces, Dictionary<Vertex, int> levels)
        {
            // Число хорд в макс дереве (исп-ся для размерности матрицы)
            var chordCount = _graph.EdgeList.Count - _graph.VertexList.Count + 1;
            // Сама матрица
            var cyclomMatr = new double[chordCount, _graph.EdgeList.Count];

            foreach (var edge in _graph.EdgeList.Where(e => !e.IsItMaxTree).ToList())
            {
                var route = new List<Vertex>();
                var routeMin = new List<Vertex>();
                var routeMax = new List<Vertex>();

                var rowIndex = indeces[edge];
                cyclomMatr[rowIndex, rowIndex] = 1;

                var level1 = levels[edge.V1];
                var level2 = levels[edge.V2];
                var minLevelVertex = level1 < level2 ? edge.V1 : edge.V2;
                var maxLevelVertex = level1 > level2 ? edge.V1 : edge.V2;

                routeMax.Add(maxLevelVertex);
                routeMin.Add(minLevelVertex);
                while (levels[minLevelVertex] != levels[maxLevelVertex])
                {
                    var nextEdges =
                        _graph.EdgeList.Where(e => e.IsItMaxTree && (e.V1 == maxLevelVertex || e.V2 == maxLevelVertex))
                            .ToList();
                    var nextEdge =
                        nextEdges.Single(
                            e => levels[e.V1] < levels[maxLevelVertex] || levels[e.V2] < levels[maxLevelVertex]);
                    var nextVertex = nextEdge.V1 == maxLevelVertex ? nextEdge.V2 : nextEdge.V1;

                    routeMax.Add(nextVertex);
                    maxLevelVertex = nextVertex;
                }

                if (maxLevelVertex.Id != minLevelVertex.Id)
                {
                    while (maxLevelVertex.Id != minLevelVertex.Id)
                    {
                        var nextEdgeMin = _graph.EdgeList.Where(e => e.IsItMaxTree && (e.V1.Id == minLevelVertex.Id || e.V2.Id == minLevelVertex.Id))
                            .ToList()
                            .Single(e => levels[e.V1] < levels[minLevelVertex] || levels[e.V2] < levels[minLevelVertex]);
                        var nextVertexMin = nextEdgeMin.V1.Id == minLevelVertex.Id ? nextEdgeMin.V2 : nextEdgeMin.V1;

                        routeMin.Add(nextVertexMin);
                        minLevelVertex = nextVertexMin;
                        var nextEdgeMax = _graph.EdgeList.Where(e => e.IsItMaxTree && (e.V1.Id == maxLevelVertex.Id || e.V2.Id == maxLevelVertex.Id))
                            .ToList()
                            .Single(e => levels[e.V1] < levels[maxLevelVertex] || levels[e.V2] < levels[maxLevelVertex]);
                        var nextVertexMax = nextEdgeMax.V1.Id == maxLevelVertex.Id ? nextEdgeMax.V2 : nextEdgeMax.V1;

                        routeMax.Add(nextVertexMax);
                        maxLevelVertex = nextVertexMax;
                    }
                }

                if (edge.V2.Id == routeMax.First().Id)
                {
                    route.AddRange(routeMax);
                    routeMin.Remove(routeMin.Last());
                    routeMin.Reverse();
                    route.AddRange(routeMin);
                }
                else
                {
                    route.AddRange(routeMin);
                    routeMax.Remove(routeMax.Last());
                    routeMax.Reverse();
                    route.AddRange(routeMax);
                }

                for (var i = 1; i < route.Count; i++)
                {
                    var edge1 =
                        _graph.EdgeList.SingleOrDefault(
                            e => e.IsItMaxTree && e.V1 == route[i - 1] && e.V2 == route[i]);
                    var edge2 =
                        _graph.EdgeList.SingleOrDefault(
                            e => e.IsItMaxTree && e.V1 == route[i] && e.V2 == route[i - 1]);

                    if (edge1 != null)
                    {
                        cyclomMatr[rowIndex, indeces[edge1]] = 1.0;
                    }
                    else
                    {
                        cyclomMatr[rowIndex, indeces[edge2]] = -1.0;
                    }
                }
            }

            return cyclomMatr;
        }

        /// <summary>
        /// Цикломатическая матрица B_д.
        /// Слева - хорды, сверху - дуги дерева.
        /// </summary>
        /// <returns></returns>
        public double[,] CyclomaticMatr(double[,] extCyclomaticMatr)
        {
            // Число хорд в макс дереве (исп-ся для размерности матрицы)
            var chordCount = _graph.EdgeList.Count(e => !e.IsItMaxTree);
            var cyclMatr = new double[extCyclomaticMatr.GetLength(0), _graph.VertexList.Count - 1];

            for (var i = 0; i < cyclMatr.GetLength(0); i++)
            {
                for (var j = 0; j < cyclMatr.GetLength(1); j++)
                {
                    cyclMatr[i, j] = extCyclomaticMatr[i, j + chordCount];
                }
            }
            return cyclMatr;
        }

        /// <summary>
        /// Матрица Rd.
        /// (строка есть путь по дереву от каждой вершины к корню)
        /// </summary>
        /// <returns></returns>
        public double[,] Rd(Dictionary<Edge, int> indeces, Dictionary<Vertex, int> levels, Dictionary<Vertex, int> vertIndeces)
        {
            var matr = new double[_graph.VertexList.Count - 1, _graph.VertexList.Count - 1];
            // Число хорд в макс дереве (исп-ся для размерности матрицы)
            var chordCount = _graph.EdgeList.Count - _graph.VertexList.Count + 1;

            foreach (var level in levels.OrderBy(l => l.Value))
            {
                var vertex = level.Key;
                if (vertex.Id == _root.Id)
                {
                    continue;
                }

                var prevEdges =
                    _graph.EdgeList.Where(e => e.IsItMaxTree && (e.V1.Id == vertex.Id || e.V2.Id == vertex.Id))
                        .ToList();
                var prevEdge =
                    prevEdges.Single(
                        e => levels[e.V1] < levels[vertex] || levels[e.V2] < levels[vertex]);

                var prevVertex = prevEdge.V1 == vertex ? prevEdge.V2 : prevEdge.V1;

                var rowIndex = vertIndeces[vertex];
                double value;
                if (prevEdge.V2.Id == vertex.Id)
                {
                    value = -1.0;
                }
                else
                {
                    value = 1.0;
                }

                if (prevVertex.Id == _root.Id)
                {
                    matr[rowIndex, indeces[prevEdge] - chordCount] = value;
                    continue;
                }

                var prevRow = vertIndeces[prevVertex];
                for (var i = 0; i < matr.GetLength(0); i++)
                {
                    matr[rowIndex, i] = matr[prevRow, i];
                }
                matr[rowIndex, indeces[prevEdge] - chordCount] = value;
            }
            return matr;
        }

        /// <summary>
        /// Матрица инцидентности для графа-дерева (графа без циклов)
        /// </summary>
        /// <param name="indeces"></param>
        /// <param name="vertIndeces"></param>
        /// <returns></returns>
        public double[,] IncidenceMatrix(Dictionary<Edge, int> indeces, Dictionary<Vertex, int> vertIndeces)
        {
            var matr = new double[_graph.VertexList.Count - 1, _graph.EdgeList.Count(e => e.IsItMaxTree)];
            foreach (var edge in _graph.EdgeList.Where(e => e.IsItMaxTree).ToList())
            {
                var columnIndex = indeces[edge] - _graph.EdgeList.Count(e => !e.IsItMaxTree);

                if (edge.V1 != _root)
                {
                    var rowIndexV1 = vertIndeces[edge.V1];
                    matr[rowIndexV1, columnIndex] = 1.0;
                }

                if (edge.V2 != _root)
                {
                    var rowIndexV2 = vertIndeces[edge.V2];
                    matr[rowIndexV2, columnIndex] = -1.0;
                }
            }
            return matr;
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<Edge, int> EdgeIndeces()
        {
            var indeces = new Dictionary<Edge, int>();
            var indexMaxTree = _graph.EdgeList.Count(e => !e.IsItMaxTree);
            var indexNotMaxTree = 0;

            foreach (var edge in _graph.EdgeList)
            {
                if (edge.IsItMaxTree)
                {
                    indeces.Add(edge, indexMaxTree);
                    indexMaxTree++;
                }
                else
                {
                    indeces.Add(edge, indexNotMaxTree);
                    indexNotMaxTree++;
                }
            }
            return indeces;
        }

        public Dictionary<Vertex, int> VertexIndeces()
        {
            var d = new Dictionary<Vertex, int>();

            var index = 0;
            foreach (var vertex in _graph.VertexList.Where(vertex => vertex != _root))
            {
                d.Add(vertex, index);
                index++;
            }
            return d;
        }
    }

    public class MaxTree
    {
        private readonly Graph _graph;
        private readonly Vertex _root;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph">Граф</param>
        /// <param name="root">Корень дерева (вершина с неизв расходом)</param>
        public MaxTree(Graph graph, Vertex root)
        {
            _graph = graph;
            _root = root;
            _graph.VertexList.ForEach(v => v.IsChecked = false);
            _graph.EdgeList.ForEach(e => e.IsItMaxTree = false);
        }

        /// <summary>
        /// Построение максимального дерева (графа без циклов).
        /// Ребра, составляющие макс дерево, отмечены IsItMaxTree = true
        /// </summary>
        /// <returns></returns>
        public Dictionary<Vertex, int> BuildMaxTree()
        {
            // Суть алгоритма. Нахожу для первой вершины все ребра, подключенные к ней. Отмечаю вершину пройденной. 
            // Далее для соседних вершин (которые тоже отмечены как "пройденные") снова ищу подключенные к ним ребра 
            // (При этом слежу за тем, чтобы другой конец этих ребер не был пройденным. Иначе пропускаю ребро.)
            // Далее алгоритм повторяется... (для всех соседних верщин ищется подключенные к ним ребра и т.д.)

            var dict = new Dictionary<Vertex, int>();
            _root.IsChecked = true;

            var queue = new Queue<Vertex>();
            queue.Enqueue(_root);
            dict.Add(_root, 0);

            while (queue.Any())
            {
                var vertex = queue.Dequeue();

                var edges1 = _graph.EdgeList.Where(e => e.V1 == vertex && e.V2.IsChecked == false).ToList();
                edges1.ForEach(e => e.IsItMaxTree = true);
                edges1.ForEach(e => e.V2.IsChecked = true);
                edges1.ForEach(e => queue.Enqueue(e.V2));

                var edges2 = _graph.EdgeList.Where(e => e.V2 == vertex && e.V1.IsChecked == false).ToList();
                edges2.ForEach(e => e.IsItMaxTree = true);
                edges2.ForEach(e => e.V1.IsChecked = true);
                edges2.ForEach(e => queue.Enqueue(e.V1));

                edges1.ForEach(e => dict.Add(e.V2, dict[vertex] + 1));
                edges2.ForEach(e => dict.Add(e.V1, dict[vertex] + 1));
            }
            return dict;
        }
    }

    public class GasVolume
    {
        /// <summary>
        /// Запас газа в ребре, тыс. м3
        /// </summary>
        /// <param name="edge"></param>
        /// <returns></returns>
        public double GasSupplyOnOneEdge(Edge edge)
        {
            var geomVolume = Math.PI * Math.Pow(edge.Diameter / 1000, 2.0) *
                             Math.Abs(edge.KilometerOfEndPoint - edge.KilometerOfStartPoint) / 4.0 * 1000.0;
            var pAverage = edge.V1.Pressure.Value > edge.V2.Pressure.Value
                ? 2 / 3.0 * (edge.V1.Pressure.Value + Math.Pow(edge.V2.Pressure.Value, 2.0) / (edge.V1.Pressure.Value + edge.V2.Pressure.Value))
                : 2 / 3.0 * (edge.V2.Pressure.Value + Math.Pow(edge.V1.Pressure.Value, 2.0) / (edge.V1.Pressure.Value + edge.V2.Pressure.Value));
            var tAverage = 2 + 273.15;
            var zAverage = 0.88;

            var supply = geomVolume * pAverage * 293.15 / (1.033 * Math.Pow(10.0, 3.0) * zAverage * tAverage);

            return supply;
        }
    }

    #region NewVersion

    public class GasInPipeCalculationNew
    {
        private readonly Graph _graph;
        private readonly Vertex _root;

        public GasInPipeCalculationNew(Graph graph, Vertex root)
        {
            _graph = graph;
            _root = root;
            _graph.VertexList.ForEach(v => v.IsChecked = false);
        }

        public void Calculate()
        {
            var vertLevels = new MaxTree(_graph, _root).BuildMaxTree();
            new ContourMethodOfConsumption(_graph, _root, vertLevels).Run();
            double[,] atrInv;
            Dictionary<Vertex, Vertex> roots;
            new CoefficientOfEfficiencyCalculator(_graph, _root, vertLevels).Run(out atrInv, out roots);
            var pressure = new PressureCalculator(_graph, _root, atrInv, roots).Run();

            foreach (var edge in _graph.EdgeList)
            {
                if (!edge.V1.Pressure.HasValue || !edge.V2.Pressure.HasValue)
                {
                    continue;
                }
                //
                var pIn = edge.V1.Pressure > edge.V2.Pressure ? edge.V1.Pressure.Value : edge.V2.Pressure.Value;
                var pOut = edge.V1.Pressure > edge.V2.Pressure ? edge.V2.Pressure.Value : edge.V1.Pressure.Value;
                var length = Math.Abs(edge.KilometerOfEndPoint - edge.KilometerOfStartPoint);
                edge.GasVolume = MathFormula.GasSupplyOnOneEdge(pIn, pOut, edge.Diameter, length);
            }

            var gasSupplyTotal = _graph.EdgeList.Where(e => e.GasVolume.HasValue).Sum(e => e.GasVolume);
        }

        private void CalculateConsumption()
        {
        }
    }

    /// <summary>
    /// Контурный метод расходов
    /// </summary>
    public class ContourMethodOfConsumption
    {
        private readonly Graph _graph;
        private readonly Vertex _root;
        private readonly Dictionary<Vertex, int> _vertLevels;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="root"></param>
        /// <param name="vertLevels"></param>
        public ContourMethodOfConsumption(Graph graph, Vertex root, Dictionary<Vertex, int> vertLevels)
        {
            _graph = graph;
            _root = root;
            _vertLevels = vertLevels;
        }

        public void Run()
        {
            var vertLevels = new MaxTree(_graph, _root).BuildMaxTree();

            // Матрицы построения
            var eIndeces = Helper.EdgeIndeces(_graph);
            var vIndeces = Helper.VertexIndeces(_graph, _root);
            var extendedCyclomMatr = ExtendedCyclomaticMatr(eIndeces, vertLevels);
            var cyclomMatr = CyclomaticMatr(extendedCyclomMatr);
            var rd = Rd(eIndeces, vIndeces);

            var xTree = new double[_graph.EdgeList.Count(e => e.IsItMaxTree)];
            var xChord = StartingValuesForXChord(eIndeces);
            var matrLambda = MatrixLambda(eIndeces);
            var vectQ = VectQ(vIndeces);
            var rdTrQ = MyMath.MultiplicationMatrVect(MyMath.TransposeMatrix(rd), vectQ);
            var cyclomMatrTr = MyMath.TransposeMatrix(cyclomMatr);
            var extCyclMatrTr = MyMath.TransposeMatrix(extendedCyclomMatr);
            var bl = MyMath.MultiplicationDiagonalMatrix(extendedCyclomMatr, matrLambda);

            var eps = double.MaxValue;
            while (Math.Abs(eps) > 0.01)
            {
                // xд = Rд_tr*Q + Bд_tr*xк(N)
                var bdTrxk = MyMath.MultiplicationMatrVect(cyclomMatrTr, xChord);
                xTree = MyMath.SumVectors(rdTrQ, bdTrxk);
                var matrX = MatrixX(xChord, xTree);

                // Δxк(N) = (BɅX(N)B_tr)^(-1) * (-1/2BɅX(N)x(N))
                var blx = MyMath.MultiplicationDiagonalMatrix(bl, matrX);
                var blxBtr = MyMath.MultiplicationMatrix(blx, extCyclMatrTr);
                var minustwoBlXx = MyMath.MultiplicationMatrVect(MyMath.MultiplicationMatrScalar(blx, -1.0 / 2.0),
                    VectorX(xChord, xTree));
                var deltaXchord = MyMath.MultiplicationMatrVect(MyMath.InverseMatrix(blxBtr), minustwoBlXx);

                // xк(N+1) = xк(N) + Δxк(N)
                xChord = MyMath.SumVectors(xChord, deltaXchord);

                if (deltaXchord.GetLength(0) == 0)
                {
                    break;
                }
                eps = Math.Abs(MyMath.MaxNorm(deltaXchord)); // eps = ||Δxк|| 
            }

            // Присваивание ребрам графа полученные результаты расходов
            for (var i = 0; i < xTree.GetLength(0); i++)
            {
                var ind = eIndeces.Single(index => index.Value == (i + xChord.GetLength(0)));
                _graph.EdgeList.Single(e => e == ind.Key).Consumption = xTree[i];
            }
            for (var i = 0; i < xChord.GetLength(0); i++)
            {
                var ind = eIndeces.Single(index => index.Value == i);
                _graph.EdgeList.Single(e => e == ind.Key).Consumption = xChord[i];
            }
        }

        #region BuildMatrices
        /// <summary>
        /// Расширенная цикломатическая матрица (строка есть независ цикл)
        /// Имеет вид [E|B_д]. Слева - хорды, образующие циклы, сверху - хорды и дуги дерева.
        /// </summary>
        public double[,] ExtendedCyclomaticMatr(Dictionary<Edge, int> indeces, Dictionary<Vertex, int> levels)
        {
            // Число хорд в макс дереве (исп-ся для размерности матрицы)
            var chordCount = _graph.EdgeList.Count - _graph.VertexList.Count + 1;
            var cyclomMatr = new double[chordCount, _graph.EdgeList.Count];

            foreach (var edge in _graph.EdgeList.Where(e => !e.IsItMaxTree).ToList())
            {
                var route = new List<Vertex>();
                var rowIndex = indeces[edge];
                cyclomMatr[rowIndex, rowIndex] = 1;

                Vertex lowerVertex, higherVertex;
                if (levels[edge.V1] < levels[edge.V2])
                {
                    higherVertex = edge.V1;
                    lowerVertex = edge.V2;
                }
                else
                {
                    higherVertex = edge.V2;
                    lowerVertex = edge.V1;
                }

                var routeFromLowerVertex = new List<Vertex> { lowerVertex };
                var routeFromHigherVertex = new List<Vertex> { higherVertex };

                while (levels[higherVertex] != levels[lowerVertex])
                {
                    var nextVertex = StepUp(lowerVertex);
                    routeFromLowerVertex.Add(nextVertex);
                    lowerVertex = nextVertex;
                }
                if (lowerVertex.Id != higherVertex.Id)
                {
                    while (lowerVertex.Id != higherVertex.Id)
                    {
                        var nextVertexMin = StepUp(higherVertex);
                        routeFromHigherVertex.Add(nextVertexMin);
                        higherVertex = nextVertexMin;

                        var nextVertexMax = StepUp(lowerVertex);
                        routeFromLowerVertex.Add(nextVertexMax);
                        lowerVertex = nextVertexMax;
                    }
                }

                routeFromLowerVertex.Remove(routeFromLowerVertex.Last());
                if (edge.V2.Id == routeFromLowerVertex.First().Id)
                {
                    route.AddRange(routeFromLowerVertex);
                    routeFromHigherVertex.Reverse();
                    route.AddRange(routeFromHigherVertex);
                }
                else
                {
                    route.AddRange(routeFromHigherVertex);
                    routeFromLowerVertex.Reverse();
                    route.AddRange(routeFromLowerVertex);
                }

                for (var i = 1; i < route.Count; i++)
                {
                    var edge1 =
                        _graph.EdgeList.SingleOrDefault(
                            e => e.IsItMaxTree && e.V1 == route[i - 1] && e.V2 == route[i]);
                    var edge2 =
                        _graph.EdgeList.SingleOrDefault(
                            e => e.IsItMaxTree && e.V1 == route[i] && e.V2 == route[i - 1]);

                    double a;
                    int columnIndex;
                    if (edge1 != null)
                    {
                        a = 1.0;
                        columnIndex = indeces[edge1];
                    }
                    else
                    {
                        a = -1.0;
                        columnIndex = indeces[edge2];
                    }
                    cyclomMatr[rowIndex, columnIndex] = a;
                }
            }

            return cyclomMatr;
        }

        /// <summary>
        /// matrXii = [|xChord|/|xTree|]
        /// </summary>
        /// <param name="chordEdge"></param>
        /// <param name="treeEdge"></param>
        private double[,] MatrixX(double[] chordEdge, double[] treeEdge)
        {
            var matrX = new double[_graph.EdgeList.Count, _graph.EdgeList.Count];
            var notTreeCount = _graph.EdgeList.Count(e => !e.IsItMaxTree);

            for (var i = 0; i < notTreeCount; i++)
            {
                matrX[i, i] = Math.Abs(chordEdge[i]);
            }

            for (var i = 0; i < _graph.EdgeList.Count(e => e.IsItMaxTree); i++)
            {
                matrX[i + notTreeCount, i + notTreeCount] = Math.Abs(treeEdge[i]);
            }

            return matrX;
        }

        /// <summary>
        /// VectorX = [vChord/vTree]
        /// </summary>
        /// <param name="chordEdge"></param>
        /// <param name="treeEdge"></param>
        /// <returns></returns>
        private double[] VectorX(double[] chordEdge, double[] treeEdge)
        {
            var vectX = new double[_graph.EdgeList.Count];
            var notTreeCount = _graph.EdgeList.Count(e => !e.IsItMaxTree);
            for (var i = 0; i < notTreeCount; i++)
            {
                vectX[i] = chordEdge[i];
            }

            for (var i = 0; i < _graph.EdgeList.Count(e => e.IsItMaxTree); i++)
            {
                vectX[i + notTreeCount] = treeEdge[i];
            }

            return vectX;
        }

        /// <summary>
        /// Возвращает вершину, находящуюся на один уровень выше текущей вершины
        /// </summary>
        /// <param name="currentVertex">Текущая вершина</param>
        /// <returns></returns>
        private Vertex StepUp(Vertex currentVertex)
        {
            var nextCandidateV1 = _graph.EdgeList.SingleOrDefault(
                        e =>
                            e.IsItMaxTree && e.V2.Id == currentVertex.Id && _vertLevels[e.V1] == _vertLevels[currentVertex] - 1);
            var nextCandidateV2 = _graph.EdgeList.SingleOrDefault(
                e =>
                    e.IsItMaxTree && e.V1.Id == currentVertex.Id && _vertLevels[e.V2] == _vertLevels[currentVertex] - 1);
            return nextCandidateV1 == null ? nextCandidateV2.V2 : nextCandidateV1.V1;
        }

        /// <summary>
        /// Цикломатическая матрица B_д.
        /// Слева - хорды, сверху - дуги дерева.
        /// </summary>
        /// <param name="extCyclomaticMatr">Расширенная цикломатическая матрица</param>
        /// <returns></returns>
        private double[,] CyclomaticMatr(double[,] extCyclomaticMatr)
        {
            // Число хорд в макс дереве (исп-ся для размерности матрицы)
            var chordCount = _graph.EdgeList.Count(e => !e.IsItMaxTree);
            var cyclMatr = new double[extCyclomaticMatr.GetLength(0), _graph.VertexList.Count - 1];

            for (var i = 0; i < cyclMatr.GetLength(0); i++)
            {
                for (var j = 0; j < cyclMatr.GetLength(1); j++)
                {
                    cyclMatr[i, j] = extCyclomaticMatr[i, j + chordCount];
                }
            }
            return cyclMatr;
        }

        /// <summary>
        /// Матрица Rd.
        /// (строка есть путь по дереву от каждой вершины к корню)
        /// </summary>
        /// <param name="edgeIndeces">Порядок ребер в матрице</param>
        /// <param name="vertIndeces">Порядок вершин в матрице</param>
        /// <returns></returns>
        private double[,] Rd(Dictionary<Edge, int> edgeIndeces, Dictionary<Vertex, int> vertIndeces)
        {
            var matr = new double[_graph.VertexList.Count - 1, _graph.VertexList.Count - 1];
            var chordCount = _graph.EdgeList.Count - _graph.VertexList.Count + 1;

            foreach (var level in _vertLevels.OrderBy(l => l.Value))
            {
                if (level.Key.Id == _root.Id)
                {
                    continue;
                }

                var vertex = level.Key;
                var rowIndex = vertIndeces[vertex];
                var higherVertex = StepUp(vertex);
                var edge1 = _graph.EdgeList.SingleOrDefault(e => e.V1.Id == vertex.Id && e.V2.Id == higherVertex.Id);
                var edge2 = _graph.EdgeList.SingleOrDefault(e => e.V1.Id == higherVertex.Id && e.V2.Id == vertex.Id);

                int columnIndex;
                double a;
                if (edge1 == null)
                {
                    a = -1.0;
                    columnIndex = edgeIndeces[edge2] - chordCount;
                }
                else
                {
                    a = 1.0;
                    columnIndex = edgeIndeces[edge1] - chordCount;
                }

                matr[rowIndex, columnIndex] = a;
                if (higherVertex.Id == _root.Id)
                {
                    continue;
                }

                var higherRow = vertIndeces[higherVertex];
                for (var i = 0; i < matr.GetLength(0); i++)
                {
                    if (i == columnIndex)
                    {
                        continue;
                    }
                    matr[rowIndex, i] = matr[higherRow, i];
                }
            }
            return matr;
        }

        #endregion

        #region SetValues

        /// <summary> 
        /// Вектор внешних притоков Q, тыс. м3/ч.
        /// Компонента вектора отрицательная, если имеет место отбор, и положительная в случае притока 
        /// </summary>
        /// <param name="vIndeces">Порядок вершин в векторе</param>
        /// <returns></returns>
        private double[] VectQ(Dictionary<Vertex, int> vIndeces)
        {
            var vectQ = new double[_graph.VertexList.Count - 1];
            foreach (var v in _graph.VertexList)
            {
                if (v.Id == _root.Id)
                {
                    continue;
                }
                var rowIndex = vIndeces[v];
                if (v.Consumption.HasValue)
                {
                    vectQ[rowIndex] = v.Consumption.Value;
                }
                else
                {
                    vectQ[rowIndex] = 0;
                }
            }

            return vectQ;
        }

        /// <summary>
        /// Задание начальных значений матрицы Lambda
        /// (диаг. матрица значений обобщ. коэф-та гидравл сопротивления).
        /// СТО Газпром 2-3.5-051-2006, п.18.5.2 
        /// </summary>
        /// <param name="eIndeces">Порядок ребер в матрице</param>
        /// <returns></returns>
        private double[,] MatrixLambda(Dictionary<Edge, int> eIndeces)
        {
            var matrLambda = new double[_graph.EdgeList.Count, _graph.EdgeList.Count];
            foreach (var edge in _graph.EdgeList)
            {
                var column = eIndeces[edge];
                var ro = edge.V1.Density.HasValue
                    ? Density.FromKilogramsPerCubicMeter(edge.V1.Density.Value)
                    : Density.FromKilogramsPerCubicMeter(0.75);
                var pAverage = edge.V1.Pressure.HasValue && edge.V2.Pressure.HasValue
                    ? MathFormula.PressureAverage(edge.V1.Pressure.Value, edge.V2.Pressure.Value)
                    : 36.7;
                var tAverage = edge.V1.Temperature.HasValue && edge.V2.Temperature.HasValue && edge.V1.TSoil.HasValue
                    ? MathFormula.TemperatureAverage(edge.V1.Temperature.Value, edge.V2.Temperature.Value,
                        edge.V1.TSoil.Value)
                    : 273.15 + 5;
                var pKgh = Pressure.FromKgh(pAverage);
                var tK = Temperature.FromKelvins(tAverage);
                var z = SupportCalculations.GasCompressibilityFactorApproximate(pKgh, tK, ro);
                var length = Math.Abs(edge.KilometerOfEndPoint - edge.KilometerOfStartPoint);

                matrLambda[column, column] = MathFormula.BigLambda(edge.Diameter, tAverage, ro.KilogramsPerCubicMeter, z,
                    length, edge.CoefficientOfEfficiency);
            }
            return matrLambda;
        }

        /// <summary>
        /// Задание начальных значений вектора ХChord
        /// (матрица значений расхода по хордам)
        /// </summary>
        /// <param name="eIndeces"></param>
        /// <returns></returns>
        private double[] StartingValuesForXChord(Dictionary<Edge, int> eIndeces)
        {
            var vectChordX = new double[_graph.EdgeList.Count(e => !e.IsItMaxTree)];
            var minCons = _graph.VertexList.Where(v => v.Consumption.HasValue).Min(v => v.Consumption);
            _graph.EdgeList.Where(e => !e.IsItMaxTree).ToList().ForEach(ee => vectChordX[eIndeces[ee]] = minCons.Value);

            return vectChordX;
        }

        #endregion
    }

    /// <summary>
    /// Вычисление коэффициентов эффективностей
    /// </summary>
    public class CoefficientOfEfficiencyCalculator
    {
        private readonly Graph _graph;
        private readonly Vertex _root;
        private readonly Dictionary<Vertex, int> _levels;

        public CoefficientOfEfficiencyCalculator(Graph graph, Vertex root, Dictionary<Vertex, int> levels)
        {
            _graph = graph;
            _root = root;
            _levels = levels;
        }

        public void Run(out double[,] aTrInvNew, out Dictionary<Vertex, Vertex> rootsNew)
        {
            var edgesOrder = Helper.EdgeIndeces(_graph);
            var verticesOrder = Helper.VertexIndeces(_graph, _root);
            var incidenceMatr = IncidenceMatrix(edgesOrder, verticesOrder);

            var aTr = MyMath.TransposeMatrix(incidenceMatr);
            var aTrInv = MyMath.InverseMatrix(aTr);
            // var lxx = LXx(edgesOrder);
            aTrInvNew = new double[aTrInv.GetLength(0), aTrInv.GetLength(1)];
            rootsNew = new Dictionary<Vertex, Vertex>();

            foreach (var level in _levels.OrderByDescending(l => l.Value))
            {
                if (!level.Key.Pressure.HasValue || level.Key.Id == _root.Id ||
                    level.Key.Type == new EntityType())
                {
                    continue;
                }

                var row = verticesOrder[level.Key];
                var edges = Way(aTrInv, row, edgesOrder);
                var verticesSorted = SortList(edges, _root, level.Key);

                for (var i = 1; i < verticesSorted.Count; i++)
                {
                    if (!rootsNew.ContainsKey(verticesSorted[i]))
                    {
                        rootsNew.Add(verticesSorted[i], verticesSorted.First());
                    }
                }

                double x = 0;
                var pInSqr = Math.Pow(verticesSorted.First().Pressure.Value, 2.0);
                var pOutSqr = Math.Pow(verticesSorted.Last().Pressure.Value, 2.0);
                var pressureSqrVector = new double[verticesSorted.Count];
                pressureSqrVector[0] = pInSqr;
                pressureSqrVector[pressureSqrVector.GetLength(0) - 1] = pOutSqr;
                var newEdges = new List<Edge>();
                for (var i = 0; i < verticesSorted.Count - 1; i++)
                {
                    var edge =
                        edges.Single(
                            e =>
                                (e.V1.Id == verticesSorted[i].Id && e.V2.Id == verticesSorted[i + 1].Id) ||
                                (e.V1.Id == verticesSorted[i + 1].Id && e.V2.Id == verticesSorted[i].Id));
                    newEdges.Add(edge);
                    var column = edgesOrder[edge];
                    aTrInvNew[row, column] = aTrInv[row, column];
                    for (var j = i + 1; j < verticesSorted.Count - 1; j++)
                    {
                        var rowi = verticesOrder[verticesSorted[j]];
                        aTrInvNew[rowi, column] = aTrInv[rowi, column];
                    }
                }

                var bigLength = newEdges.Sum(e => Math.Abs(e.KilometerOfEndPoint - e.KilometerOfStartPoint));
                for (var i = 0; i < newEdges.Count; i++)
                {
                    if (newEdges[i].V1.Pressure.HasValue && newEdges[i].V2.Pressure.HasValue)
                    {
                        continue;
                    }

                    var currentLength = Math.Abs(newEdges[i].KilometerOfEndPoint - newEdges[i].KilometerOfStartPoint);
                    x += currentLength;
                    var pressureSqr = pInSqr - (pInSqr - pOutSqr) * x / bigLength;
                    pressureSqrVector[i + 1] = pressureSqr;

                    if (newEdges[i].CoefficientOfEfficiency.HasValue)
                    {
                        continue;
                    }
                    var ro = newEdges[i].V1.Density.HasValue
                    ? Density.FromKilogramsPerCubicMeter(newEdges[i].V1.Density.Value)
                    : Density.FromKilogramsPerCubicMeter(0.75);
                    var pAverage = newEdges[i].V1.Pressure.HasValue && newEdges[i].V2.Pressure.HasValue
                        ? MathFormula.PressureAverage(newEdges[i].V1.Pressure.Value, newEdges[i].V2.Pressure.Value)
                        : 36.7;
                    var tAverage = newEdges[i].V1.Temperature.HasValue && newEdges[i].V2.Temperature.HasValue &&
                                   newEdges[i].V1.TSoil.HasValue
                        ? MathFormula.TemperatureAverage(newEdges[i].V1.Temperature.Value,
                            newEdges[i].V2.Temperature.Value, newEdges[i].V1.TSoil.Value)
                        : 273.15 + 5;
                    var pKgh = Pressure.FromKgh(pAverage);
                    var tK = Temperature.FromKelvins(tAverage);
                    var z = SupportCalculations.GasCompressibilityFactorApproximate(pKgh, tK, ro);
                    var lxx = MathFormula.BigLambda(newEdges[i].Diameter, tAverage, ro.KilogramsPerCubicMeter, z,
                        currentLength, newEdges[i].CoefficientOfEfficiency) * Math.Pow(newEdges[i].Consumption.Value, 2.0);
                    var coef = Math.Sqrt(Math.Abs(lxx / (pressureSqrVector[i] - pressureSqr)));
                    if (double.IsNaN(coef) || coef == 0.0)
                    {
                        coef = 1.0;
                    }
                    newEdges[i].CoefficientOfEfficiency = coef;
                }
            }

            for (var i = 0; i < aTrInvNew.GetLength(0); i++)
            {
                var sumAbs = 0.0;
                for (var j = 0; j < aTrInvNew.GetLength(1); j++)
                {
                    sumAbs += Math.Abs(aTrInvNew[i, j]);
                }
                if (sumAbs < 1)
                {
                    for (var j = 0; j < aTrInv.GetLength(1); j++)
                    {
                        aTrInvNew[i, j] = aTrInv[i, j];
                    }
                }
            }
        }

        private static List<Edge> Way(double[,] aTrInv, int row, Dictionary<Edge, int> edgesOrder)
        {
            var edges = new List<Edge>();
            for (var i = 0; i < aTrInv.GetLength(0); i++)
            {
                if (aTrInv[row, i] == 1.0 || aTrInv[row, i] == -1.0)
                {
                    edges.Add(edgesOrder.Single(e => e.Value == i).Key);
                }
            }
            return edges;
        }

        /// <summary>
        /// Матрица инцидентности для графа-дерева (графа без циклов)
        /// </summary>
        /// <param name="indeces"></param>
        /// <param name="vertIndeces"></param>
        /// <returns></returns>
        private double[,] IncidenceMatrix(Dictionary<Edge, int> indeces, Dictionary<Vertex, int> vertIndeces)
        {
            var matr = new double[_graph.VertexList.Count - 1, _graph.EdgeList.Count(e => e.IsItMaxTree)];
            foreach (var edge in _graph.EdgeList.Where(e => e.IsItMaxTree).ToList())
            {
                var columnIndex = indeces[edge] - _graph.EdgeList.Count(e => !e.IsItMaxTree);

                if (edge.V1.Id != _root.Id)
                {
                    var rowIndexV1 = vertIndeces[edge.V1];
                    matr[rowIndexV1, columnIndex] = 1.0;
                }

                if (edge.V2.Id != _root.Id)
                {
                    var rowIndexV2 = vertIndeces[edge.V2];
                    matr[rowIndexV2, columnIndex] = -1.0;
                }
            }
            return matr;
        }

        private List<Vertex> SortList(List<Edge> edges, Vertex vFrom, Vertex vTo)
        {
            var sortedList = new List<Vertex> { vFrom };
            var edgesNew = new List<Edge>();

            while (vFrom.Id != vTo.Id)
            {
                var edge =
                    edges.Single(e => (e.V1.Id == vFrom.Id || e.V2.Id == vFrom.Id) && edgesNew.All(es => es != e));
                edgesNew.Add(edge);
                vFrom = edge.V1.Id == vFrom.Id ? edge.V2 : edge.V1;
                sortedList.Add(vFrom);
            }

            for (var i = sortedList.Count - 2; i > -1; i--)
            {
                if (sortedList[i].Pressure.HasValue && i > 0)
                {
                    sortedList.RemoveRange(0, i);
                    break;
                }
            }
            return sortedList;
        }
    }

    public class PressureCalculator
    {
        private readonly Graph _graph;
        private readonly Vertex _root;
        private double[,] _aTrInv;
        private Dictionary<Vertex, Vertex> _roots;

        public PressureCalculator(Graph graph, Vertex root, double[,] aTrInv, Dictionary<Vertex, Vertex> roots)
        {
            _graph = graph;
            _root = root;
            _aTrInv = aTrInv;
            _roots = roots;
        }

        public Dictionary<Vertex, double> Run()
        {
            var epsP = new Dictionary<Vertex, double>();
            var indeces = Helper.EdgeIndeces(_graph);
            var vertexIndex = Helper.VertexIndeces(_graph, _root);

            var pressureVector = new double[vertexIndex.Count];
            var lXx = LXx(indeces);

            var pKvadrat = MyMath.MultiplicationMatrVect(_aTrInv, lXx);
            for (var i = 0; i < pKvadrat.GetLength(0); i++)
            {
                var vertexI = vertexIndex.Single(vi => vi.Value == i).Key;
                if (_roots.ContainsKey(vertexI))
                {
                    pKvadrat[i] += Math.Pow(_roots[vertexI].Pressure.Value, 2);
                }
                else
                {
                    pKvadrat[i] += Math.Pow(_root.Pressure.Value, 2);
                }
                pressureVector[i] = Math.Sqrt(pKvadrat[i]);
            }

            for (var i = 0; i < pressureVector.GetLength(0); i++)
            {
                var vertex = vertexIndex.Single(vi => vi.Value == i).Key;
                var t = pressureVector[i];
                if (vertex.Pressure.HasValue)
                {
                    epsP.Add(vertex, vertex.Pressure.Value - t);
                }
                vertex.Pressure = t;
            }

            return epsP;
        }

        private double[] LXx(Dictionary<Edge, int> indeces)
        {
            var n = _graph.VertexList.Count - 1;
            var vector = new double[n];

            var l = _graph.EdgeList.Count(e => !e.IsItMaxTree);
            foreach (var edge in _graph.EdgeList.Where(e => e.IsItMaxTree))
            {
                var index = indeces[edge] - l;

                if (edge.V2.Pressure.HasValue && edge.V1.Pressure.HasValue)
                {
                    vector[index] = Math.Pow(edge.V1.Pressure.Value, 2.0) - Math.Pow(edge.V2.Pressure.Value, 2.0);
                }
                else
                {
                    if (edge.Consumption.HasValue)
                    {
                        var ro = edge.V1.Density.HasValue
                            ? Density.FromKilogramsPerCubicMeter(edge.V1.Density.Value)
                            : Density.FromKilogramsPerCubicMeter(0.75);
                        var pAverage = edge.V1.Pressure.HasValue && edge.V2.Pressure.HasValue
                            ? MathFormula.PressureAverage(edge.V1.Pressure.Value, edge.V2.Pressure.Value)
                            : 36.7;
                        var tAverage = edge.V1.Temperature.HasValue && edge.V2.Temperature.HasValue && edge.V1.TSoil.HasValue
                            ? MathFormula.TemperatureAverage(edge.V1.Temperature.Value, edge.V2.Temperature.Value,
                                edge.V1.TSoil.Value)
                            : 273.15 + 5;
                        var pKgh = Pressure.FromKgh(pAverage);
                        var tK = Temperature.FromKelvins(tAverage);
                        var z = SupportCalculations.GasCompressibilityFactorApproximate(pKgh, tK, ro);
                        var length = Math.Abs(edge.KilometerOfEndPoint - edge.KilometerOfStartPoint);

                        vector[index] = MathFormula.BigLambda(edge.Diameter, tAverage, ro.KilogramsPerCubicMeter, z,
                            length, edge.CoefficientOfEfficiency) * Math.Abs(edge.Consumption.Value) *
                            edge.Consumption.Value;
                    }
                }
            }
            return vector;
        }
    }

    public class Helper
    {
        /// <summary>
        /// Индексы ребер в матрицах
        /// </summary>
        public static Dictionary<Edge, int> EdgeIndeces(Graph graph)
        {
            var indeces = new Dictionary<Edge, int>();
            var indexMaxTree = graph.EdgeList.Count(e => !e.IsItMaxTree);
            var indexNotMaxTree = 0;

            foreach (var edge in graph.EdgeList)
            {
                if (edge.IsItMaxTree)
                {
                    indeces.Add(edge, indexMaxTree);
                    indexMaxTree++;
                }
                else
                {
                    indeces.Add(edge, indexNotMaxTree);
                    indexNotMaxTree++;
                }
            }
            return indeces;
        }

        /// <summary>
        /// Индексы вершин в матрицах
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Vertex, int> VertexIndeces(Graph graph, Vertex root)
        {
            var d = new Dictionary<Vertex, int>();

            var index = 0;
            foreach (var vertex in graph.VertexList.Where(vertex => vertex.Id != root.Id))
            {
                d.Add(vertex, index);
                index++;
            }
            return d;
        }
    }

    public class MathFormula
    {
        /// <summary>
        /// Коэффициент гидравлического сопротивления
        /// (частный случай при к = 0,03мм)
        /// согласно Трубопроводный транспорт нефти и газа, 1987, Белоусов, пар.6.4
        /// </summary>
        /// <param name="d">Диаметр трубы, мм</param>
        /// <returns></returns>
        public static double SimplifiedHydraulicCoefficient(double d)
        {
            return 0.03817 / Math.Pow(d, 0.2);
        }

        /// <summary>
        /// Обобщенный коэффициент сопротивления Lambda.
        /// P1^2 - P2^2 = Lambda * q^2
        /// </summary>
        /// <param name="d">Диаметр, мм</param>
        /// <param name="t">Средняя по длине трубы температура газа, К</param>
        /// <param name="ro">Плотность газа, кг/м3</param>
        /// <param name="z">Коэффициент сжимаемости газа</param>
        /// <param name="l">Длина трубы, км</param>
        /// <param name="e">Коэффициент гидравлической эффективности</param>
        /// <returns></returns>
        public static double BigLambda(double d, double t, double ro, double z, double l, double? e)
        {
            var fromKghToMpa = 10.2;
            var fromThouCubMPerHourToMlnCubMPerDay = 24.0 / 1000;
            var delta = ro / StandardConditions.DensityOfAir.KilogramsPerCubicMeter;
            var lambda = SimplifiedHydraulicCoefficient(d);
            var eValue = e ?? 1.0;

            var bigLambda = Math.Pow(fromKghToMpa, 2.0) * Math.Pow(fromThouCubMPerHourToMlnCubMPerDay, 2.0) * delta * t * z * l *
                            lambda / (Math.Pow(3.32, 2) * Math.Pow(10.0, -12.0) * Math.Pow(d, 5.0) * Math.Pow(eValue, 2.0));
            return bigLambda;
        }

        /// <summary>
        /// Средняя температура газа нитки магистрального газопровода, К
        /// (Согласно Методике определения запаса газа газотранспортных предприятий)
        /// </summary>
        /// <param name="tIn">Температура газа в начале участка газопровода, К</param>
        /// <param name="tOut">Температура газа в конце участка газопровода, К</param>
        /// <param name="tSoil">Температура грунта на глубине заложения оси газопровода, К</param>
        /// <returns></returns>
        public static double TemperatureAverage(double tIn, double tOut, double tSoil)
        {
            if (tIn == tOut)
            {
                return tIn;
            }
            return tSoil + (tIn - tOut) / Math.Log(Math.Abs((tIn - tSoil) / (tOut - tSoil)));
        }

        /// <summary>
        /// Среднее давление нитки магистрального газопровода, кг/см2
        /// (Согласно Методике определения запаса газа газотранспортных предприятий)
        /// </summary>
        /// <param name="pIn">Давление газа в начале участка газопровода, кг/см2</param>
        /// <param name="pOut">Давление газа в конце участка газопровода, кг/см2</param>
        /// <returns></returns>
        public static double PressureAverage(double pIn, double pOut)
        {
            return 2.0 / 3.0 * (pIn + Math.Pow(pOut, 2.0) / (pIn + pOut));
        }

        /// <summary>
        /// Запас газа в ребре, тыс. м3
        /// </summary>
        /// <param name="pIn"></param>
        /// <param name="pOut"></param>
        /// <param name="diameter">Диаметр, мм</param>
        /// <param name="length">Длина, км</param>
        /// <returns></returns>
        public static double GasSupplyOnOneEdge(double pIn, double pOut, double diameter, double length)
        {
            var geomVolume = Math.PI * Math.Pow(diameter / 1000, 2.0) * length / 4.0 * 1000.0;
            var pAverage = PressureAverage(pIn, pOut);
            var tAverage = 2 + 273.15;
            var zAverage = 0.88;

            var supply = geomVolume * pAverage * 293.15 / (1.033 * Math.Pow(10.0, 3.0) * zAverage * tAverage);

            return supply;
        }
    }

    #endregion
}
