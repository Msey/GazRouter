using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using Utils.Calculations;

namespace GazRouter.Modes.GasCosts.GasCompessibility
{
    /// <summary>
    /// 
    /// </summary>
    public class GasCompressibilityCalculator
    { 
        static GasCompressibilityCalculator()
        {
            _table1 = Tables.TableA1();
            _table2 = Tables.TableA2();
            _table3 = Tables.TableA3();
            
        }
        //  private List<ComponentContent> _x; // молярные доли компонент газа
        //  private static double _kx;
        private static readonly List<PureComponentsParameters> _table1;
        private static readonly List<ComponentsBinaryInteraction> _table2;
        private static readonly List<DimensionlessCoefficients> _table3;
        private static double Lt = 1; // К

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">Молярные доли компонент газа</param>
        /// <param name="P">Давление, МПа</param>
        /// <param name="T">Температура, К</param>
        public static double GasCompressibilityFactor(List<ComponentContent> x, double P, double T)
        {
            var kx = Kx(x, _table1, _table2);

            var pi = P / (0.001 * Math.Pow(kx, -3.0) * PhysicalConstants.R * Lt);  // приведенное давление
            var tau = T / Lt; // приведенная температура
            var sigma_start = SigmaStartValue(P, T, kx);
            var sigma = Result(x, _table1, _table2, _table3, pi, tau, sigma_start);
            var z = 1 + A0(x, _table1, _table2, _table3, tau, sigma);

            return z;
        }

        public static double Result(List<ComponentContent> x, List<PureComponentsParameters> t1,
            List<ComponentsBinaryInteraction> t2, List<DimensionlessCoefficients> t3, double pi, double tau, double sigma)
        {
            while (Math.Abs((PiRaschet(x, t1, t2, t3, tau, sigma) - pi)/pi) > Math.Pow(10.0, -6.0))
            {
                var deltaSigma = (pi/tau - (1 + A0(x, t1, t2, t3, tau, sigma))*sigma)/
                                 (1 + A1(x, t1, t2, t3, tau, sigma));
                sigma = sigma + deltaSigma;
            }
            return sigma;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static double SigmaStartValue(double P, double T, double kx)
        {
            return 1000.0*P*Math.Pow(kx, 3.0)/(PhysicalConstants.R * T);
        }

        private static double PiRaschet(List<ComponentContent> x, List<PureComponentsParameters> t1, List<ComponentsBinaryInteraction> t2, List<DimensionlessCoefficients> t3, double tau, double sigma)
        {
            return sigma * tau * (1 + A0(x, t1, t2, t3, tau, sigma));
        }

        private static double A0(List<ComponentContent> x, List<PureComponentsParameters> t1, List<ComponentsBinaryInteraction> t2, List<DimensionlessCoefficients> t3, double tau, double sigma)
        {
            return t3.Sum(
                t =>
                    t.an*Math.Pow(sigma, t.bn)*Math.Pow(tau, -t.un)*
                    (t.bn*Dn(x, t1, t2, t3, t.n) +
                     (t.bn - t.cn*t.kn*Math.Pow(sigma, t.kn))*Un(x, t1, t2, t3, t.n)*Math.Exp(-t.cn*Math.Pow(sigma, t.kn))));
        }

        private static double A1(List<ComponentContent> x, List<PureComponentsParameters> t1, List<ComponentsBinaryInteraction> t2, List<DimensionlessCoefficients> t3, double tau,  double sigma)
        {
            return t3.Sum(
                t =>
                    t.an*Math.Pow(sigma, t.bn)*Math.Pow(tau, -t.un)*
                    (
                        (t.bn + 1)*t.bn*Dn(x, t1, t2, t3, t.n) +
                        ((t.bn - t.cn*t.kn*Math.Pow(sigma, t.kn))*
                         (t.bn - t.cn*t.kn*Math.Pow(sigma, t.kn) + 1) -
                         t.cn*Math.Pow(t.kn, 2.0)*Math.Pow(sigma, t.kn))*
                        Un(x, t1, t2, t3, t.n)*Math.Exp(-t.cn*Math.Pow(sigma, t.kn))
                        )
                );
        }

        private static double Dn(List<ComponentContent> x, List<PureComponentsParameters> t1, List<ComponentsBinaryInteraction> t2, List<DimensionlessCoefficients> t3, int n)
        {
            if (n >= 1 && n <= 12)
                return Bn(x, t1, t2, t3, n)*Math.Pow(Kx(x, t1, t2), -3);
            if (n >= 13 && n <= 18)
                return Bn(x, t1, t2, t3, n)*Math.Pow(Kx(x, t1, t2), -3) - Cn(x, t1, t2, t3, n);
            return 0;
        }

        private static double Un(List<ComponentContent> x, List<PureComponentsParameters> t1, List<ComponentsBinaryInteraction> t2, List<DimensionlessCoefficients> t3, int n)
        {
            return n <= 12 ? 0 : Cn(x, t1, t2, t3, n);
        }

        private static double Cn(List<ComponentContent> x, List<PureComponentsParameters> t1, List<ComponentsBinaryInteraction> t2, List<DimensionlessCoefficients> t3, int n)
        {
            var t3_row = t3.Single(t => t.n == n);
            var Q = x.Sum(item => item.Concentration*t1.Single(t => t.Component == item.Component).Qi);
            var F = x.Sum(item => Math.Pow(item.Concentration, 2)*t1.Single(t => t.Component == item.Component).Fi);
            var Cn = Math.Pow(G(x, t1, t2) + 1 - t3_row.gn, t3_row.gn)*Math.Pow(Math.Pow(Q, 2.0) + 1 - t3_row.qn, t3_row.qn)*
                     Math.Pow(F + 1 - t3_row.fn, t3_row.fn)*Math.Pow(V(x, t1, t2), t3_row.un);

            return Cn;
        }

        private static double G(List<ComponentContent> x, List<PureComponentsParameters> t1, List<ComponentsBinaryInteraction> t2)
        {
            var sum1 = x.Sum(item => item.Concentration*t1.Single(t => t.Component == item.Component).Qi);

            var sum2 = 0.0;
            for (var i = 0; i < x.Count - 1; i++)
            {
                for (var j = i + 1; j < x.Count; j++)
                {
                    var row =
                        t2.SingleOrDefault(t => t.Component1 == x[i].Component && t.Component2 == x[j].Component) ??
                        t2.SingleOrDefault(t => t.Component2 == x[i].Component && t.Component1 == x[j].Component);
                    var gij = row == null ? 1.0 : row.Gij;
                    sum2 += x[i].Concentration * x[j].Concentration * (gij - 1) *
                            (t1.Single(t => t.Component == x[i].Component).Gi +
                             t1.Single(t => t.Component == x[j].Component).Gi);
                }
            }

            return sum1 + sum2;
        }

        private static double V(List<ComponentContent> x, List<PureComponentsParameters> t1, List<ComponentsBinaryInteraction> t2)
        {
            var sum1 = x.Sum(item => item.Concentration * Math.Pow(t1.Single(t => t.Component == item.Component).Ei, 5/2.0));

            var sum2 = 0.0;
            for (var i = 0; i < x.Count - 1; i++)
            {
                for (var j = i + 1; j < x.Count; j++)
                {
                    var row =
                        t2.SingleOrDefault(t => t.Component1 == x[i].Component && t.Component2 == x[j].Component) ??
                        t2.SingleOrDefault(t => t.Component2 == x[i].Component && t.Component1 == x[j].Component);
                    var vij = row == null ? 1.0 : row.Vij;
                    sum2 += x[i].Concentration*x[j].Concentration*(Math.Pow(vij, 5.0) - 1)*
                            Math.Pow(
                                t1.Single(t => t.Component == x[i].Component).Ei*
                                t1.Single(t => t.Component == x[j].Component).Ei, 5/2.0);
                }
            }

            return Math.Pow(Math.Pow(sum1, 2.0) + 2.0*sum2, 1/5.0);
        }

        private static double Bn(List<ComponentContent> x, List<PureComponentsParameters> t1, List<ComponentsBinaryInteraction> t2, List<DimensionlessCoefficients> t3, int n)
        {
            // var sum = 0.0;

            return x.Sum(itemX =>
                x.Sum(
                    itemX2 =>
                        itemX.Concentration*itemX2.Concentration*Bnij(t1, t2, t3, n, itemX.Component, itemX2.Component)*
                        Math.Pow(Eij(t1, t2, itemX.Component, itemX2.Component), t3.Single(t => t.n == n).un)*
                        Math.Pow(
                            t1.Single(t => t.Component == itemX.Component).Ki*
                            t1.Single(t => t.Component == itemX2.Component).Ki, 3/2.0))
                );
        }

        private static double Eij(List<PureComponentsParameters> t1, List<ComponentsBinaryInteraction> t2, PropertyType c1, PropertyType c2)
        {
            var row =
                t2.SingleOrDefault(t => t.Component1 == c1 && t.Component2 == c2) ??
                t2.SingleOrDefault(t => t.Component2 == c1 && t.Component1 == c2);

            return (row != null ? row.Eij : 1.0) *
                   Math.Sqrt(t1.Single(t => t.Component == c1).Ei * t1.Single(t => t.Component == c2).Ei);
        }

        private static double Bnij(List<PureComponentsParameters> t1, List<ComponentsBinaryInteraction> t2, List<DimensionlessCoefficients> t3, int n, PropertyType c1, PropertyType c2)
        {
            var row =
                t2.SingleOrDefault(t => t.Component1 == c1 && t.Component2 == c2) ??
                t2.SingleOrDefault(t => t.Component2 == c1 && t.Component1 == c2);
            var gij = (row != null ? row.Gij : 1.0)*
                      (t1.Single(t => t.Component == c1).Gi + t1.Single(t => t.Component == c2).Gi)/2.0;
            var t3_row = t3.Single(t => t.n == n);

            var res = Math.Pow(gij + 1 - t3_row.gn, t3_row.gn)*
                      Math.Pow(t1.Single(t => t.Component == c1).Qi*t1.Single(t => t.Component == c2).Qi + 1 - t3_row.qn,
                          t3_row.qn)*
                      Math.Pow(
                          Math.Sqrt(t1.Single(t => t.Component == c1).Fi*t1.Single(t => t.Component == c2).Fi) + 1 -
                          t3_row.fn, t3_row.fn)*
                      Math.Pow(t1.Single(t => t.Component == c1).Si*t1.Single(t => t.Component == c2).Si + 1 - t3_row.sn,
                          t3_row.sn)*
                      Math.Pow(t1.Single(t => t.Component == c1).Wi*t1.Single(t => t.Component == c2).Wi + 1 - t3_row.wn,
                          t3_row.wn);

            return res;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static double Kx(List<ComponentContent> _x, List<PureComponentsParameters> t1, List<ComponentsBinaryInteraction> t2)
        {
            var sum1 = _x.Sum(x => x.Concentration*Math.Pow(t1.Single(t => t.Component == x.Component).Ki, 5/2.0));
            
            var sum2 = 0.0;
            for (var i = 0; i < _x.Count - 1; i++)
            {
                for (var j = i + 1; j < _x.Count; j++)
                {
                    var row =
                        t2.SingleOrDefault(t => t.Component1 == _x[i].Component && t.Component2 == _x[j].Component) ??
                        t2.SingleOrDefault(t => t.Component2 == _x[i].Component && t.Component1 == _x[j].Component);
                    var kij = row == null ? 1.0 : row.Kij;
                    sum2 += _x[i].Concentration * _x[j].Concentration * (Math.Pow(kij, 5.0) - 1) *
                            Math.Pow(
                                t1.Single(t => t.Component == _x[i].Component).Ki*
                                t1.Single(t => t.Component == _x[j].Component).Ki, 5/2.0);
                }
            }

            return Math.Pow(Math.Pow(sum1, 2.0) + 2 * sum2, 1 / 5.0);
        }
    }
}