using System;

namespace GazRouter.Prediction
{
    public class MyMath
    {
        /// <summary>
        /// Функция, возвращающая транспонированную матрицу
        /// </summary>
        /// <param name="matr"></param>
        /// <returns></returns>
        public static double[,] TransposeMatrix(double[,] matr)
        {
            var matr1 = new double[matr.GetLength(1), matr.GetLength(0)];

            for (var i = 0; i < matr.GetLength(0); i++)
            {
                for (var j = 0; j < matr.GetLength(1); j++)
                {
                    matr1[j, i] = matr[i, j];
                }
            }

            return matr1;
        }

        /// <summary>
        /// Функция для нахождения обратной матрицы
        /// </summary>
        /// <param name="matr"></param>
        /// <returns></returns>
        public static double[,] InverseMatrix(double[,] matr)
        {
            var matri = new double[matr.GetLength(0), matr.GetLength(1)];

            for (var i = 0; i < matr.GetLength(0); i++)
            {
                for (var j = 0; j < matr.GetLength(1); j++)
                {
                    matri[i, j] = matr[i, j];
                }
            }

            var n = matri.GetLength(0);
            var row = new int[n];
            var col = new int[n];
            var temp = new double[n];
            int hold, I_pivot, J_pivot;
            double pivot, abs_pivot;

            // установиим row и column как вектор изменений.
            for (var k = 0; k < n; k++)
            {
                row[k] = k;
                col[k] = k;
            }
            // начало главного цикла
            for (var k = 0; k < n; k++)
            {
                // найдем наибольший элемент для основы
                pivot = matri[row[k], col[k]];
                I_pivot = k;
                J_pivot = k;

                for (var i = k; i < n; i++)
                {
                    for (var j = k; j < n; j++)
                    {
                        abs_pivot = Math.Abs(pivot);
                        if (Math.Abs(matri[row[i], col[j]]) > abs_pivot)
                        {
                            I_pivot = i;
                            J_pivot = j;
                            pivot = matri[row[i], col[j]];
                        }
                    }
                }

                if (pivot == 0.0)
                {
                    pivot = Math.Pow(10, -20.0);
                    //    pivot = Double.Epsilon;
                }

                //перестановка к-ой строки и к-ого столбца с стобцом и строкой, содержащий основной элемент(pivot основу)
                hold = row[k];
                row[k] = row[I_pivot];
                row[I_pivot] = hold;
                hold = col[k];
                col[k] = col[J_pivot];
                col[J_pivot] = hold;
                // k-ую строку с учетом перестановок делим на основной элемент

                var a = 1.0 / pivot;
                matri[row[k], col[k]] = a; //1.0 / pivot;
             
                for (var j = 0; j < n; j++)
                {
                    if (j != k)
                    {
                        var aa = matri[row[k], col[j]] * matri[row[k], col[k]];
                        matri[row[k], col[j]] = aa; // matri[row[k], col[j]] * matri[row[k], col[k]];
                    }
                }
                // внутренний цикл
                for (var i = 0; i < n; i++)
                {
                    if (k != i)
                    {
                        for (var j = 0; j < n; j++)
                        {
                            if (k != j)
                            {
                                var aaa = matri[row[i], col[j]] - matri[row[i], col[k]] *
                                        matri[row[k], col[j]];
                                matri[row[i], col[j]] = aaa;
                                //     matri[row[i], col[j]] - matri[row[i], col[k]] * matri[row[k], col[j]];
                            }
                        }

                        var aaaa = -matri[row[i], col[k]] * matri[row[k], col[k]];
                        matri[row[i], col[k]] = aaaa; // -matri[row[i], col[k]] * matri[row[k], col[k]];
                    }
                }
            }
            // конец главного цикла
            // переставляем назад rows
            for (var j = 0; j < n; j++)
            {
                for (var i = 0; i < n; i++)
                {
                    var a = matri[row[i], j];
                    temp[col[i]] = a; //matri[row[i], j];
                }
                for (var i = 0; i < n; i++)
                {
                    matri[i, j] = temp[i];
                }
            }
            // переставляем назад columns
            for (var i = 0; i < n; i++)
            {
                for (var j = 0; j < n; j++)
                {
                    temp[row[j]] = matri[i, col[j]];
                }
                for (var j = 0; j < n; j++)
                {
                    matri[i, j] = temp[j];
                }
            }
            return matri;
        }

        /// <summary>
        /// Функция для произведения двух матриц
        /// </summary>
        /// <param name="matr1"></param>
        /// <param name="matr2"></param>
        /// <returns></returns>
        public static double[,] MultiplicationMatrix(double[,] matr1, double[,] matr2)
        {
            double[,] matr3 = new double[matr1.GetLength(0), matr2.GetLength(1)];

            for (var i = 0; i < matr1.GetLength(0); i++)
            {
                for (var j = 0; j < matr2.GetLength(1); j++)
                {
                    for (var k = 0; k < matr2.GetLength(0); k++)
                    {
                        matr3[i, j] += matr1[i, k] * matr2[k, j];
                    }
                }
            }

            return matr3;
        }

        public static double[,] MultiplicationDiagonalMatrix(double[,] matr, double[,] diagMatr)
        {
            double[,] result = new double[matr.GetLength(0), diagMatr.GetLength(1)];

            for (var i = 0; i < matr.GetLength(0); i++)
            {
                for (var j = 0; j < matr.GetLength(1); j++)
                {
                    result[i, j] = matr[i, j] * diagMatr[j, j];
                }
            }

            return result;
        }

        /// <summary>
        /// Функция, умножающая матрицу на вектор
        /// </summary>
        /// <param name="matr"></param>
        /// <param name="vect"></param>
        /// <returns></returns>
        public static double[] MultiplicationMatrVect(double[,] matr, double[] vect)
        {
            var result = new double[matr.GetLength(0)];

            if (matr.GetLength(1) == vect.GetLength(0))
            {
                for (var i = 0; i < matr.GetLength(0); i++)
                {
                    for (var j = 0; j < matr.GetLength(1); j++)
                    {
                        result[i] += matr[i, j]*vect[j];
                    }
                }
            }
            else
            {
                return null;
            }

            return result;
        }

        /// <summary>
        /// Умножение матрицы на число
        /// </summary>
        /// <param name="matr"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static double[,] MultiplicationMatrScalar(double[,] matr, double scalar)
        {
            var result = new double[matr.GetLength(0), matr.GetLength(1)];

            for (var i = 0; i < matr.GetLength(0); i++)
            {
                for (var j = 0; j < matr.GetLength(1); j++)
                {
                    result[i, j] = matr[i, j] * scalar;
                }
            }

            return result;
        }

        /// <summary>
        /// Функция, суммирующая два вектора
        /// </summary>
        /// <param name="vect1"></param>
        /// <param name="vect2"></param>
        /// <returns></returns>
        public static double[] SumVectors(double[] vect1, double[] vect2)
        {
            var result = new double[vect1.GetLength(0)];

            if (vect1.GetLength(0) == vect2.GetLength(0))
            {
                for (var i = 0; i < result.GetLength(0); i++)
                {
                    result[i] = vect1[i] + vect2[i];
                }
            }

            return result;
        }

        /// <summary>
        /// Функция возвращает max_норму вектора
        /// </summary>
        /// <param name="vect"></param>
        /// <returns></returns>
        public static double MaxNorm(double[] vect)
        {
            var max = vect[0];

            for (var i = 1; i < vect.GetLength(0); i++)
            {
                if (Math.Abs(vect[i]) > Math.Abs(max))
                {
                    max = vect[i];
                }
            }

            return max;
        }
    }
}
