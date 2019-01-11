using System;
using System.Collections.Generic;
using System.Linq;

namespace GazRouter.Prediction
{
    public class ConsumptionPoint
    {
        public ConsumptionPoint(ConsumptionValueList consvalueList)
        {
            ConsvalueList = consvalueList;
        }
        public ConsumptionValueList ConsvalueList { get; private set; }

        /// <summary>
        ///     Функция, рассчитывающая прогноз на один день (используется МНК)
        /// </summary>
        /// <param name="cvLst"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public double ForecastOneDay(ConsumptionValueList cvLst, int number)
        {
            var paramLinearRegr = new List<ParametersLinearRegr>();

            foreach (var cv in cvLst)
            {
                var rgr = new ParametersLinearRegr(cvLst, cv.Date);
                paramLinearRegr.Add(rgr);
            }

            var arr = new double[paramLinearRegr.Count - 6 + number*1, 4];
            for (var i = 0; i < arr.GetLength(0) - 1; i++)
            {
                arr[i, 0] = paramLinearRegr[i].One;
                arr[i, 1] = paramLinearRegr[i].T0;
                arr[i, 2] = paramLinearRegr[i].Q1;
                arr[i, 3] = paramLinearRegr[i].Q0;
            }

            arr[arr.GetLength(0) - 1, 0] = paramLinearRegr[paramLinearRegr.Count - 7 + number].One;
            arr[arr.GetLength(0) - 1, 1] = paramLinearRegr[paramLinearRegr.Count - 7 + number].T0;
            arr[arr.GetLength(0) - 1, 2] = paramLinearRegr[paramLinearRegr.Count - 7 + number].Q1;
            arr[arr.GetLength(0) - 1, 3] = paramLinearRegr[paramLinearRegr.Count - 7 + number].Q0;

            // Заполнение матрицы F в уравнении Q = F A (дни прогноза не входят в массив)
            var masF = new double[arr.GetLength(0) - 1, arr.GetLength(1) - 1];
            for (var i = 0; i < masF.GetLength(0); i++)
            {
                for (var j = 0; j < masF.GetLength(1); j++)
                {
                    masF[i, j] = arr[i, j];
                }
            }

            // Заполнение матрицы Q в уравнении Q = F A (дни прогноза не входят в массив)
            var masQ = new double[arr.GetLength(0) - 1, 1];
            for (var i = 0; i < masQ.GetLength(0); i++)
            {
                masQ[i, 0] = arr[i, arr.GetLength(1) - 1];
            }

            // Нахождение коэффициентов регрессии из уравнения A = (F_tr * F) ^ (-1) * (F_tr * Q)
            var regrСoef =
                MyMath.MultiplicationMatrix(
                    MyMath.MultiplicationMatrix(
                        MyMath.InverseMatrix(MyMath.MultiplicationMatrix(MyMath.TransposeMatrix(masF), masF)),
                        MyMath.TransposeMatrix(masF)), masQ);

            var fprogn = new double[1, masF.GetLength(1)];
                // Заполнение матрицы F в уравнении Q = F A (содержит данные только в прогнозные дни)   
            double[,] qq; // Матрица qq содержит прогноз газопотребления на день

            for (var j = 0; j < fprogn.GetLength(1); j++)
            {
                fprogn[0, j] = arr[masF.GetLength(0), j];
            }

            qq = MyMath.MultiplicationMatrix(fprogn, regrСoef);

            return qq[0, 0];
        }

        /// <summary>
        ///     Функция, содержащая прогноз на неделю
        /// </summary>
        /// <returns></returns>
        public ConsumptionValueList ForecastOneWeek()
        {
            ConsvalueList = ConsvalueList.NoZeroValues(ConsvalueList[ConsvalueList.Count - 7].Date);
                // Удаление из рассмотрения дней, в которые газопотребление равно 0

            // Фильтрация случайных выбросов
            var srednHelper = ConsvalueList.Sum(variable => variable.Volume);
            var average = srednHelper/ConsvalueList.Count;
            for (var i = 1; i < ConsvalueList.Count - 7; i++)
            {
                var a = ConsvalueList[i].Volume/ConsvalueList[i - 1].Volume;
                if (a > 2 || a < 0.5)
                {
                    var a1 = Math.Abs(average - ConsvalueList[i].Volume);
                    var a2 = Math.Abs(average - ConsvalueList[i - 1].Volume);
                    if (a1 - a2 > 0)
                    {
                        ConsvalueList.RemoveAt(i);
                    }
                    else
                    {
                        ConsvalueList.RemoveAt(i - 1);
                    }
                    i--;
                }
            }

            var result = new double[7, 1];
            for (var i = 0; i < 7; i++)
            {
                var q = ForecastOneDay(ConsvalueList, i);
                result[i, 0] = q;
                ConsvalueList[ConsvalueList.Count - 7 + i].Volume = q;
            }

            // Создание и заполнение списка, содержащего прогноз газопотребления на неделю
            var cvList = new ConsumptionValueList();
            for (var i = 0; i < 7; i++)
            {
                var cv = new ConsumptionValue
                {
                    Date = ConsvalueList[ConsvalueList.Count - 7 + i].Date,
                    Volume = result[i, 0]
                };
                cvList.Add(cv);
            }

            return cvList;
        }
    }
}