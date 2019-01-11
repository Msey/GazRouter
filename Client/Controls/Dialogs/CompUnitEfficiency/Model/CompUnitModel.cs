using System;
using System.Collections.Generic;
using System.Linq;
using GazRouter.DTO;
using Utils.Calculations;
using Utils.Units;

namespace GazRouter.Controls.Dialogs.CompUnitEfficiency.Model
{
    public class CompUnitModel
    {
        public CompUnitModelMeasuredValues Measurings { get; set; }
        public CompUnitModelCalculatedValues Results { get; set; }
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Объемная производительность, м3/мин
        /// </summary>
        private double? _pumping;
        private double? _fuelGasConsumption;
        private bool _itsOk;

        public CompUnitModel()
        {
            Measurings = new CompUnitModelMeasuredValues();
            Results = new CompUnitModelCalculatedValues();
        }

        #region Паспортные параметры

        /// <summary>
        /// Номинальные обороты ротора ЦБН, об/мин
        /// V_SUPERCHARGER_TYPES
        /// </summary>
        public double RpmRated { get; set; }

        /// <summary>
        /// Номинальная мощность ЦБН, кВт
        /// V_COMP_UNIT_TYPES
        /// </summary>
        public double PowerRated { get; set; }

        /// <summary>
        /// Минимально допустимая объемная подача, м3/мин
        /// V_SUPERCHARGER_TYPES
        /// </summary>
        public double Qmin { get; set; }

        /// <summary>
        /// Максимально допустимая объемная подача, м3/мин
        /// V_SUPERCHARGER_TYPES
        /// </summary>
        public double Qmax { get; set; }

        /// <summary>
        /// Коэффициент адиабаты, -
        /// V_SUPERCHARGER_TYPES
        /// </summary>
        public double KaRated { get; set; }

        #region Массив точек

        /// <summary>
        /// Массив точек производительность - степень сжатия
        /// V_SUPERCHARGER_TYPES
        /// </summary>
        public List<Point> CompRatioPoints { get; set; }

        /// <summary>
        /// Массив точек производительность - политр. КПД
        /// V_SUPERCHARGER_TYPES
        /// </summary>
        public List<Point> PolytrEfficiencyPoints { get; set; }

        /// <summary>
        /// Массив точек производительность - мощность
        /// </summary>
        public List<Point> PowerInPoints { get; set; }

        #endregion

        /// <summary>
        /// Номинальный коэффициент техн состояние по мощности, -
        /// V_COMP_UNIT_TYPES
        /// </summary>
        public double? CoefTechStateByPowerRated { get; set; }

        /// <summary>
        /// Номинальный коэффициент техн состояние по ТГ, -
        /// V_COMP_UNIT_TYPES
        /// </summary>
        public double? CoefTechStateFuelRated { get; set; }

        /// <summary>
        /// Номинальный политропный К.П.Д ЦБН, -
        /// V_SUPERCHARGER_TYPES (N_CBN_RATED)
        /// </summary>
        public double PolytropicEfficiencyRated { get; set; }

        /// <summary>
        /// Номинальный КПД ГТУ, -
        /// V_COMP_UNIT_TYPES (RATED_EFFICIENCY)
        /// </summary>
        public double NGtuRated { get; set; }

        /// <summary>
        /// Коэффициент, учитывающий влияние температуры атмосферного воздуха
        /// V_COMP_UNIT_K_T_AIRS (K_T_VALUE)
        /// </summary>
        public double? CoefTAir { get; set; }

        /// <summary>
        /// КПД электродвигателя
        /// </summary>
        public double? MotorisierteEfficiencyFactor { get; set; }

        /// <summary>
        /// КПД редуктора
        /// </summary>
        public double? ReducerEfficiencyFactor { get; set; }

        #endregion

        #region Рабочие функции

        #region PumpingCalculation

        /// <summary>
        /// Объемная производительность, м3/мин
        /// согласно графику "Газодинамические характеристики компрессора"
        /// </summary>
        /// <param name="rpmRelative">Относительные обороты</param>
        /// <param name="e">Степень сжатия</param>
        /// <returns></returns>
        private double? GetPumping(double rpmRelative, double e)
        {
            // Интерполяция сплайном кривой зависимости Пол КПД - Объемн производительность
            // для различных отн оборотов, в т.ч. и равных 1

            var splinePolEff = new CubicSpline();
            splinePolEff.BuildSpline(PolytrEfficiencyPoints.Select(p => p.X * rpmRelative).ToArray(),
                                     PolytrEfficiencyPoints.Select(eff => eff.Y).ToArray(),
                                     PolytrEfficiencyPoints.Count);

            var compRatioPointsNew = CompRatioPoints.Select(p => new Point { X = p.Y, Y = p.X }).ToList();
            compRatioPointsNew.Sort();

            // Точки на кривой Степень сжатия - Объемн производительность
            // для различных отн оборотов, в т.ч. и равных 1
            var compRatio = new double[compRatioPointsNew.Count];
            var pumping = new double[compRatioPointsNew.Count];

            for (var i = 0; i < compRatioPointsNew.Count; i++)
            {
                pumping[i] = Qi(rpmRelative, compRatioPointsNew[i].Y);
                compRatio[i] = Ei(rpmRelative, compRatioPointsNew[i].X, splinePolEff.Interpolate(pumping[i]));
            }

            // Интерполяция сплайном кривой зависимости Пол КПД - Объемн производительность
            // для различных отн оборотов, в т.ч. и равных 1
            var splineCompRatio = new CubicSpline();
            splineCompRatio.BuildSpline(compRatio, pumping, compRatioPointsNew.Count);

            var pump = splineCompRatio.Interpolate(e);

            return pump; //return (pump > Qmin*rpmRelative && pump < Qmax*rpmRelative) ? pump : (double?) null;
        }


        #region Calculations


        /// <summary>
        /// Qi
        /// </summary>
        /// <param name="nNom"> Относительные обороты ротора</param>
        /// <param name="qRated">Значение производительности в точке на кривой nNom=1</param>
        /// <returns></returns>
        public double Qi(double nNom, double qRated)
        {
            return nNom * qRated;
        }


        /// <summary>
        /// Степень сжатия ERated
        /// </summary>
        /// <param name="qRated"></param>
        /// <returns></returns>
        public double ERated(double qRated)
        {
            var spline = new CubicSpline();
            spline.BuildSpline(CompRatioPoints.Select(p => p.X).ToArray(), CompRatioPoints.Select(p => p.Y).ToArray(),
                               CompRatioPoints.Count);
            return spline.Interpolate(qRated);
        }

        /// <summary>
        /// Ei
        /// </summary>
        /// <param name="nNom">Относительные обороты</param>
        /// <param name="eRated">Значение степени сжатия точки на кривой nNom=1</param>
        /// <param name="nRated">Значение политр КПД точки на кривой nNom=1</param>
        /// <returns></returns>
        public double Ei(double nNom, double eRated, double nRated)
        {
            var ei = eRated;

            if (nNom != 1)
            {
                var m = 100 * nRated * KaRated /
                    (nRated * KaRated - KaRated + 1);

                var t = nNom > 1 ?
                    m / (m - 1) : (m - 1) / m;
                // var t = m/(m - 1);

                ei = Math.Pow(Math.Pow(nNom, 2) * (Math.Pow(eRated, t) - 1) + 1, t);
            }

            return ei;
        }

        /// <summary>
        /// Политропный КПД NRated
        /// </summary>
        /// <param name="qRated"></param>
        /// <returns></returns>
        public double NRated(double qRated)
        {
            var spline = new CubicSpline();
            spline.BuildSpline(PolytrEfficiencyPoints.Select(p => p.X).ToArray(),
                               PolytrEfficiencyPoints.Select(p => p.Y).ToArray(),
                               PolytrEfficiencyPoints.Count);
            return spline.Interpolate(qRated);
        }

        /// <summary>
        /// Внутренняя мощность NRorated
        /// </summary>
        /// <param name="qRated"></param>
        /// <returns></returns>
        public double? NRorated(double qRated)
        {
            if (PowerInPoints.Any())
            {
                var spline = new CubicSpline();
                spline.BuildSpline(PowerInPoints.Select(p => p.X).ToArray(), PowerInPoints.Select(p => p.Y).ToArray(),
                               PowerInPoints.Count);
                return spline.Interpolate(qRated);
            }

            return null;
        }

        /// <summary>
        /// Внутренняя мощность для кривых nNom!=1
        /// </summary>
        /// <param name="nNom">Относительные обороты</param>
        /// <param name="NRoRated">Внутренняя мощность (nNom=1)</param>
        /// <returns></returns>
        public double NRoi(double nNom, double NRoRated)
        {
            return Math.Pow(nNom, 3) * NRoRated;
        }

        #endregion

        #endregion

        /// <summary>
        /// Вычисление расхода ТГ, тыс м3/ч
        /// согласно СТО Газпром 2-3.5-051-2006 п.18.7.3.1
        /// </summary>
        /// <param name="powerIn">Внутренняя мощность, кВт</param>
        /// <param name="tAir">Температура наружного воздуха, К</param>
        /// <param name="pAir">Атмосферное давление, МПа</param>
        /// <returns></returns>
        public double GetFuelGasConsumption(double powerIn, double tAir, double pAir)
        {
            var fuelConsumptionRated = 3.6 * PowerRated / (NGtuRated * PhysicalQuantityConversions.Kcal2Kilojoule(8000));
            // Номинальный расход топливного газа (согласно СТО Газпром 2-3.5-051-2006 п.18.7.3.1)

            var coefTechStateFuelRated = CoefTechStateFuelRated.HasValue && CoefTechStateFuelRated.Value > 0 ? CoefTechStateFuelRated.Value : 1.05;
            var coefPressureAir = CoeffPressureAir(pAir);

            var fuelConsumption = fuelConsumptionRated *
                                  (0.75 * powerIn / PowerRated + 0.25 * coefPressureAir * Math.Sqrt(tAir / 288)) *
                                  coefTechStateFuelRated;

            return fuelConsumption;
        }

        /// <summary>
        /// Коэффициент, учитывающий влияние высоты над уровнем моря
        /// </summary>
        /// <param name="pressureAir">Атмосферное давление, МПа</param>
        /// <returns></returns>
        private double CoeffPressureAir(double pressureAir)
        {
            var coefPressureAir = 9.8456 * pressureAir + 0.0024;

            return coefPressureAir;
        }




        #region Внутренняя мощность

        /// <summary>
        /// Коэффициент псевдоизоэнтропы
        /// Согласно СТО Газпром 2-3.5-113-2007 п.5.2.1
        /// </summary>
        /// <param name="tIn">Температура на входе нагнетателя, К</param>
        /// <param name="tOut">Температура на выходе нагнетателя, К</param>
        /// <param name="delta">Относительная плотность газа по воздуху</param> 
        /// <param name="pInAbs">Давление на входе нагнетателя, кг/см2</param>
        /// <param name="pOutAbs">Давление на вsходе нагнетателя, кг/см2</param>
        /// <returns></returns>
        private double K1K(double tIn, double tOut, double delta, double pInAbs, double pOutAbs)
        {
            var tsrC = PhysicalQuantityConversions.K2C((tIn + tOut) / 2);
            var mt = TemperatureCoefficientOfPolytrope(tIn, tOut, pInAbs, pOutAbs);

            var k1K = 4.16 + 0.0041 * (tsrC - 10) + 3.93 * (delta - 0.55) + 5 * (mt - 0.3);

            return k1K;
        }

        /// <summary>
        /// Температурный показатель политропы
        /// </summary>
        /// <param name="tIn">Температура на входе нагнетателя, К</param>
        /// <param name="tOut">Температура на выходе нагнетателя, К</param>
        /// <param name="pInAbs">Давление на входе нагнетателя, кг/см2</param>
        /// <param name="pOutAbs">Давление на вsходе нагнетателя, кг/см2</param>
        /// <returns></returns>
        private double TemperatureCoefficientOfPolytrope(double tIn, double tOut, double pInAbs, double pOutAbs)
        {
            var mt = Math.Log10(tOut / tIn) /
                    Math.Log10(PhysicalQuantityConversions.Kgh2Mpa(pOutAbs) / PhysicalQuantityConversions.Kgh2Mpa(pInAbs));

            return mt;
        }

        /// <summary>
        /// Внутренняя мощность, кВт
        /// Согласно СТО Газпром 2-3.5-113-2007 п.5.2.1
        /// </summary>
        /// <param name="zsr">Средний коэффициент сжимаемости</param>
        /// <param name="gasConstant">Газовая постоянная, кДж/кг*К</param>
        /// <param name="tIn">Температура на входе нагнетателя, К</param>
        /// <param name="tOut">Температура на выходе нагнетателя, К</param>
        /// <param name="massFlowRate">Массовый расход газа, кг/с</param>
        /// <param name="delta">Относительная плотность газа по воздуху</param>
        /// <param name="pInAbs">Давление на входе нагнетателя, кг/см2</param>
        /// <param name="pOutAbs">Давление на вsходе нагнетателя, кг/см2</param>
        /// <returns></returns>
        private double PowerSuperchargerMethod1(double zsr, double gasConstant, double tIn, double tOut, double massFlowRate, double delta, double pInAbs, double pOutAbs)
        {
            var k1K = K1K(tIn, tOut, delta, pInAbs, pOutAbs);
            var power = k1K * zsr * gasConstant * (tOut - tIn) * massFlowRate;

            return power;
        }

        /// <summary>
        /// Внутренняя мощность, кВт
        /// согласно графику "Газодинамические характеристики компрессора"
        /// </summary>
        /// <param name="pumping">Объемная производительность, м3/мин</param>
        /// <param name="rpmRelative">Относительные обороты, -</param>
        /// <returns></returns>
        private double? PowerSuperchargerMethod2(double pumping, double rpmRelative)
        {
            // Интерполяция сплайном кривой зависимости Пол КПД - Объемн производительность
            // для различных отн оборотов, в т.ч. и равных 1

            if (PowerInPoints.Any())
            {
                var pumpingPowerIn = new double[PowerInPoints.Count];
                for (var i = 0; i < pumpingPowerIn.GetLength(0); i++)
                    pumpingPowerIn[i] = Qi(rpmRelative, PowerInPoints[i].X);

                var powerInPoints = new double[PowerInPoints.Count];
                for (var i = 0; i < powerInPoints.GetLength(0); i++)
                    powerInPoints[i] = NRoi(rpmRelative, PowerInPoints[i].Y);

                var splinePolEff = new CubicSpline();
                splinePolEff.BuildSpline(pumpingPowerIn, PowerInPoints.Select(p => p.Y).ToArray(), pumpingPowerIn.GetLength(0));

                var pMPa = Measurings.PIn;
                var power = splinePolEff.Interpolate(pumping) * (100 * pMPa.Mpa / 9.80665);
                return power;
            }
            
            

            return null;
        }

        /// <summary>
        /// Вычисление внутренней мощности, кВт
        /// </summary> 
        /// <param name="zsr">Средний коэффициент сжимаемости</param>
        /// <param name="gasConstant">Газовая постоянная, кДж/кг*К</param>
        /// <param name="tIn">Температура на входе нагнетателя, К</param>
        /// <param name="tOut">Температура на выходе нагнетателя, К</param>
        /// <param name="massFlowRate">Массовый расход газа, кг/с</param>
        /// <param name="delta">Относительная плотность газа по воздуху</param>
        /// <param name="pInAbs">Давление на входе нагнетателя, кг/см2</param>
        /// <param name="pOutAbs">Давление на вsходе нагнетателя, кг/см2</param>
        /// <param name="pumping">Объемная производительность, м³/мин</param>
        /// <param name="rpmRelative">Относительные обороты, -</param>
        /// <returns></returns>
        private double GetPowerIn(double zsr, double gasConstant, double tIn, double tOut, double massFlowRate,
                                  double delta, double pInAbs, double pOutAbs, double pumping, double rpmRelative)
        {
            return PowerSuperchargerMethod2(pumping, rpmRelative).HasValue
                       ? PowerSuperchargerMethod2(pumping, rpmRelative).Value
                       : PowerSuperchargerMethod1(zsr, gasConstant, tIn, tOut, massFlowRate, delta, pInAbs, pOutAbs);
        }

        #endregion

        #region Располагаемая мощность

        /// <summary>
        /// Вспомогательная функция для нахождения располагаемой мощности
        /// </summary>
        /// <param name="coefPressureAir">Коэффициент, учитывающий высоту над уровнем моря</param>
        /// <param name="tAir">Температура наружного воздуха, К</param>
        /// <returns></returns>
        private double PowerMaxHelper(double coefPressureAir, double tAir)
        {
            var ratedPower = PowerRated; // номинальная мощность ЦБН, кВт
            var kN = CoefTechStateByPowerRated.HasValue && CoefTechStateByPowerRated.Value > 0 ? CoefTechStateByPowerRated.Value : 0.95; // Коэффициент технического состояния ГТУ по мощности (паспортное значение)
            var kt = CoefTAir.HasValue ? 1 - CoefTAir.Value * (tAir - 288) / tAir : 1 - 3 * (tAir - 288) / tAir; //Коэффициент, учитывающий влияние температуры атмосферного воздуха
            const double ky = 0.985; // коэффициент, учитывающий влияние системы утилизации тепла выхлопных газов
            const double k_n = 1; //коэффициет влияния относительной скорости вращения ротора силовой турбины. Учитывается в составе коэффициента kN.

            return ratedPower * kN * kt * ky * k_n * coefPressureAir;
        }

        /// <summary>
        /// Располагаемая мощность, кВт
        /// согласно СТО Газпром 2-3.5-051-2006 п.18.7.1.4
        /// </summary>
        /// <param name="pAir">Атмосферное давление, МПа</param>
        /// <param name="tAir">Температура наружного воздуха, К</param>
        /// <returns></returns>
        private double GetPowerMax(double pAir, double tAir)
        {
            var powerRated = PowerRated;
            var coefPressureAir = CoeffPressureAir(pAir);
            var powerMaxHelper = PowerMaxHelper(coefPressureAir, tAir);
            double powerMax;

            if (powerMaxHelper < 1.1 * powerRated)
                powerMax = powerMaxHelper;
            else
                powerMax = 1.1 * powerRated;

            return powerMax;
        }

        #endregion

        /// <summary>
        /// Вычисление эффективной мощности, кВт
        /// согласно СТО Газпром 2-3.5-113-2007 п.5.2.1
        /// </summary>
        /// <param name="powerIn">Внутренняя мощность, кВт</param>
        /// <param name="eceMechanic">Механический КПД</param>
        /// <returns></returns>
        public double GetPowerEffective(double powerIn, double eceMechanic)
        {
            return powerIn / eceMechanic;
        }

        /// <summary>
        /// Вычисление КПД ГПА, -
        /// СТО Газпром 2-3.5-113-2007 п.5.2.1
        /// </summary>
        /// <param name="efficiency">Эффективный КПД ГТУ</param>
        /// <param name="polytrEfficiency">Политропный КПД</param>
        /// <returns></returns>
        private double GetNGgpa(double efficiency, double polytrEfficiency)
        {
            return efficiency * polytrEfficiency;
        }

        /// <summary>
        /// Эффективный КПД ГТУ
        /// согласно ПР 51-31323949-43-99 п.5.7
        /// </summary>
        /// <param name="massFuelGasComsumption"> Массовый расход ТГ, кг/с</param>
        /// <param name="powerEffective">Эффективная мощность, кВт</param>
        /// <param name="massCombustionHeatLow">Фактическая массовая низшая теплота сгорания газа, кДж/кг</param>
        /// <returns></returns>
        private double GetEfficiency(double massFuelGasComsumption, double powerEffective, double massCombustionHeatLow)
        {
            return powerEffective / (massFuelGasComsumption * massCombustionHeatLow);
        }

        #region Политропный КПД ЦБН

        /// <summary>
        /// Политропный КПД ЦБН
        /// </summary>
        /// <param name="pumping">Объемная производительность, м3/мин</param>
        /// <param name="rpmRelative">Относительные приведенные обороты</param>
        /// <param name="tIn">Температура на входе нагнетателя, К</param>
        /// <param name="tOut">Температура на выходе нагнетателя, К</param>
        /// <param name="delta">Относительная плотность газа по воздуху</param> 
        /// <param name="pInAbs">Давление на входе нагнетателя, кг/см2</param>
        /// <param name="pOutAbs">Давление на вsходе нагнетателя, кг/см2</param>
        /// <returns></returns>
        private double GetPolytropicEfficiency(double pumping, double rpmRelative, double tIn, double tOut, double delta, double pInAbs, double pOutAbs)
        {
            var polytropicEfficiency = PolytropicEfficiencyMethod1(pumping, rpmRelative).HasValue
                                       ? PolytropicEfficiencyMethod1(pumping, rpmRelative).Value
                                       : PolytropicEfficiencyMethod2(tIn, tOut, delta, pInAbs, pOutAbs);

            return polytropicEfficiency;
        }

        /// <summary>
        /// Политропный КПД ЦБН
        /// согласно графику "Газодинамические характеристики компрессора"
        /// </summary>
        /// <param name="pumping">Объемная производительность, м3/мин</param>
        /// <param name="rpmRelative">Относительные приведенные обороты</param>
        /// <returns></returns>
        private double? PolytropicEfficiencyMethod1(double pumping, double rpmRelative)
        {
            var pumpingPolEffic = new double[PolytrEfficiencyPoints.Count];
            for (var i = 0; i < pumpingPolEffic.GetLength(0); i++)
                pumpingPolEffic[i] = Qi(rpmRelative, PolytrEfficiencyPoints[i].X);

            var splinePolEff = new CubicSpline();
            splinePolEff.BuildSpline(pumpingPolEffic, PolytrEfficiencyPoints.Select(p => p.Y).ToArray(),
                                     pumpingPolEffic.GetLength(0));

            return splinePolEff.Interpolate(pumping);
        }

        /// <summary>
        /// Политропный КПД ЦБН
        /// ПР 51-31323949-43-99 п.6.1.7.2. Метод Шульца
        /// </summary>
        /// <param name="tIn">Температура на входе нагнетателя, К</param>
        /// <param name="tOut">Температура на выходе нагнетателя, К</param>
        /// <param name="delta">Относительная плотность газа по воздуху</param> 
        /// <param name="pInAbs">Давление на входе нагнетателя, кг/см2</param>
        /// <param name="pOutAbs">Давление на вsходе нагнетателя, кг/см2</param>
        /// <returns></returns>
        private double PolytropicEfficiencyMethod2(double tIn, double tOut, double delta, double pInAbs, double pOutAbs)
        {
            var k1K = K1K(tIn, tOut, delta, pInAbs, pOutAbs);
            var mt = TemperatureCoefficientOfPolytrope(tIn, tOut, pInAbs, pOutAbs);

            var r = 1 / (k1K * mt);

            return r;
        }

        #endregion

        /// <summary>
        /// Коэффициент технического состояния ЦБН
        /// согласно СТО Газпром 2-3.5-113-2007 п.5.2.3.3
        /// </summary>
        /// <param name="polytrEfficiency">Политропный КПД</param>
        /// <returns></returns>
        private double GetCoefTechStateSupercharger(double polytrEfficiency)
        {
            return polytrEfficiency / PolytropicEfficiencyRated;
        }

        #region Коэффициент тех состояния по топливному газу

        /// <summary>
        /// Коэффициент технического состояния ГТУ по топливному газу
        /// согласно СТО Газпром 2-3.5-113-2007 п.5.2.3.2
        /// </summary>
        /// <param name="fuelGasConsumption">Расход ТГ, тыс. м3/ч</param>
        /// <param name="pressureAir">Давление атмосферное, МПа</param>
        /// <param name="tIn">Температура на входе в нагнетатель, К</param>
        /// <param name="combustionHeatLow">Низшая теплота сгорания газа, ккал/м3</param>
        /// <returns></returns>
        private double GetCoefTechStateByFuelGas(double fuelGasConsumption, Utils.Units.Pressure pressureAir, Temperature tIn, double combustionHeatLow)
        {
            var fuelGasConsumptionRated = 3.6 * PowerRated / (NGtuRated * PhysicalQuantityConversions.Kcal2Kilojoule(8000));
            var fuelGasConsumptionRelative = FuelGasConsumptionRelative(fuelGasConsumption, pressureAir, tIn,
                                                                        combustionHeatLow);

            return fuelGasConsumptionRelative / fuelGasConsumptionRated;
        }

        /// <summary>
        /// Фактический приведенный расход топливного газа
        /// согласно СТО Газпром 2-3.5-113-2007 п.5.2.3.2
        /// </summary>
        /// <param name="fuelGasConsumption">Расход ТГ, тыс. м3</param>
        /// <param name="pressureAir">Давление атмосферное, МПа</param>
        /// <param name="tInK">Температура на входе в нагнетатель, К</param>
        /// <param name="combustionHeatLow">Низшая теплота сгорания газа, ккал/м3</param>
        /// <returns></returns>
        private double FuelGasConsumptionRelative(double fuelGasConsumption, Pressure pressureAir, Temperature tInK, double combustionHeatLow)
        {
            var pStandart = StandardConditions.P;

            return fuelGasConsumption * pStandart.Mpa / pressureAir.Mpa *
                         Math.Sqrt(288 / tInK.Kelvins) *
                         combustionHeatLow /
                         8000;
        }
        #endregion

        #region Коэффициент тех состояния по мощности

        /// <summary>
        /// Коэффициент технического состояния ГТУ по мощности
        /// согласно СТО Газпром 2-3.5-113-2007 п.5.2.3.1 и ПР 51-31323949-43-99 п.5.1.2
        /// </summary>
        /// <param name="powerEffective">Эффективная мощность, кВт</param>
        /// <param name="pressureAir">Атмосферное давление, МПа</param>
        /// <param name="tIn">Температура входа в нагнетатель, К</param>
        /// <returns></returns>
        private double GetCoefTechStateByPower(double powerEffective, Pressure pressureAir, Temperature tIn)
        {
            var nepr = PowerEfficientRelative(powerEffective, pressureAir, tIn);
            var nRated = PowerRated;

            return nepr / nRated;
        }

        /// <summary>
        /// Приведенная эффективная мощность ГТУ 
        /// </summary>
        /// <param name="powerEffective">Эффективная мощность, кВт</param>
        /// <param name="pressureAir">Атмосферное давление, МПа</param>
        /// <param name="tIn">Температура входа в нагнетатель, К</param>
        /// <returns></returns>
        private double PowerEfficientRelative(double powerEffective, Pressure pressureAir, Temperature tIn)
        {
            return powerEffective * (StandardConditions.P / pressureAir) *
                         Math.Sqrt(288 / tIn.Kelvins);
        }

        #endregion

        /// <summary>
        /// Коэффициент загрузки
        /// Должен быть меньше 0.95
        /// </summary>
        /// <param name="powerEffective">Эффективная мощность, кВт</param>
        /// <param name="maxPower">Располагаемая мощность, кВт</param>
        /// <returns></returns>
        private double GetCoefficientCharge(double powerEffective, double maxPower)
        {
            return powerEffective / maxPower;
        }

        /// <summary>
        /// Удаленность от границ помпажа
        /// Коэффициент должен быть не менее 1,1
        /// </summary>
        /// <param name="rpmRelative">Относительные приведенные обороты, -</param>
        /// <param name="pumping">Объемная производительность, м3/мин</param>
        /// <returns></returns>
        private double GetSurging(double rpmRelative, double pumping)
        {
            return (pumping - 1.1 * Qmin * rpmRelative) / pumping * 100;
        }

        #endregion


        public bool CalculateTurbine()
        {
            var pInAbs = Measurings.PIn + StandardConditions.P; //Давление на входе нагнетателя, кг/см2
            var pOutAbs = Measurings.POut + StandardConditions.P; //Давление на выходе нагнетателя, кг/см2

            var tIn = Measurings.TIn; //Температура на входе нагнетателя, К
            var tOutK = Measurings.TOut; //Температура на выходе нагнетателя, К

            var e = pOutAbs / pInAbs; //Степень сжатия, -

            var gasCompressibilityFactorInlet =
                SupportCalculations.GasCompressibilityFactorApproximate(pInAbs,tIn, Measurings.Density);
            //Коэф. сжимаемости на входе, - (согласно РД 153-39.0-112-2001 п.7.3)

            var gasCompressibilityFactorOutlet =
                SupportCalculations.GasCompressibilityFactorApproximate(pOutAbs,tOutK, Measurings.Density);
            //Коэф. сжимаемости на выходе, - (РД 153-39.0-112-2001 п.7.3)

            var gasCompressibilityFactorAverage = (gasCompressibilityFactorInlet + gasCompressibilityFactorOutlet) / 2;//ср. коэф. сжимаемости


            var delta = Measurings.Density.KilogramsPerCubicMeter / StandardConditions.DensityOfAir.KilogramsPerCubicMeter; //Относительная плотность газа по воздуху (СТО Газпром 2-3.5-113-2007 п.5.2.1)

            var gasConstant = 0.287 / delta; //Газовая постоянная, кДж/кг-К (ОНТП 51-1-85 п.12.37)

            var densitySuperchargerInlet = pInAbs.Mpa * 1000 /
                                           (gasCompressibilityFactorInlet * gasConstant * tIn.Kelvins); //Плотность газа на входе в ЦБН, кг/м3 (ОНТП 51-1-85 п.12.38)

            var rpmRelative = Measurings.Rpm / RpmRated; //Относительные обороты ротора, -
            const double eceMechanic = 0.985; //Механический КПД ЦБН 0.985 или оценивается при проведении спец испытаний
            // var coefPressureAir = 9.8456 * PhysicalQuantityConversions.mmHg2Mpa(Measurings.PressureAir) + 0.0024; //Коэффициент, учитывающий влияние высоты над уровнем моря

            var massCombustionHeatLow = PhysicalQuantityConversions.Kcal2Kilojoule(Measurings.CombustionHeatLow.KCal) /
                                        Measurings.Density.KilogramsPerCubicMeter; // Фактическая массовая низшая теплота сгорания газа, кДж/кг

            _pumping = Measurings.PumpingMeasured > 0 ? Measurings.PumpingMeasured : GetPumping(rpmRelative, e);

            if (!_pumping.HasValue )//|| rpmRelative > 1.1 || rpmRelative < 0.7)
            {
                ErrorMessage = "Объемная производительность не найдена";
                Results = new CompUnitModelCalculatedValues();
                _itsOk = false;
            }

            else
            {
                //var massFlowRate = pInAbs.Mpa * 98066.5 * _pumping.Value / (gasCompressibilityFactorInlet * gasConstant * 1000 * tIn.Kelvins * 60);
                var massFlowRate = pInAbs.Mpa * 1000.0 * _pumping.Value / (gasCompressibilityFactorInlet * gasConstant * tIn.Kelvins * 60);
                //Массовый расход газа через ЦБН, кг/с (СТО Газпром 2-3.5-051-2006 п.18.7.2.2)

                var powerIn = GetPowerIn(gasCompressibilityFactorAverage, gasConstant, tIn.Kelvins, tOutK.Kelvins, massFlowRate,
                                           delta, pInAbs.Mpa, pOutAbs.Mpa, _pumping.Value, rpmRelative);

                if (Measurings.FuelGasConsumptionMeasured > 0)
                    _fuelGasConsumption = Measurings.FuelGasConsumptionMeasured;
                else
                    _fuelGasConsumption = GetFuelGasConsumption(powerIn,
                                                                Measurings.TemperatureAir.Kelvins,
                                                                Measurings.PressureAir.Mpa);

                var massFuelGasComsumption = _fuelGasConsumption * Measurings.Density.KilogramsPerCubicMeter / 3.6; // Массовый расход ТГ, кг/с

                var polytrEfficiency = GetPolytropicEfficiency(_pumping.Value, rpmRelative, tIn.Kelvins, tOutK.Kelvins, delta, pInAbs.Mpa,
                                                                 pOutAbs.Mpa);



                var powerMax = GetPowerMax(Measurings.PressureAir.Mpa, Measurings.TemperatureAir.Kelvins); //Располагаемая мощность, кВт

                var powerEffective = GetPowerEffective(powerIn, eceMechanic);
                //Вычисление эффективной мощности, кВт (СТО Газпром 2-3.5-113-2007 п.5.2.1)

                var efficiency = GetEfficiency(massFuelGasComsumption.Value, powerEffective,
                                     massCombustionHeatLow); //Эффективный КПД ГТУ

                Results.PumpingCommercial = massFlowRate * gasConstant / 4; //Вычисление коммерческой производительности, млн м3/сут (СТО Газпром 2-3.5-051-2006 п.18.7.2.2)
                Results.NGgpa = GetNGgpa(efficiency, polytrEfficiency); //КПД ГПА
                Results.PowerMax = powerMax;
                Results.PowerEffective = powerEffective;
                Results.PowerIn = powerIn;
                Results.Efficiency = efficiency;
                Results.PolytropicEfficiency = polytrEfficiency;
                Results.CoefTechState = GetCoefTechStateSupercharger(polytrEfficiency); // Коэффициент технического состояния ЦБН
                Results.CoefTechStateByFuelGas = GetCoefTechStateByFuelGas(_fuelGasConsumption.Value,
                                                                             Measurings.PressureAir, tIn,
                                                                             Measurings.CombustionHeatLow.KCal); // Коэффициент технического состояния по ТГ

                Results.CoefTechStateByPower = GetCoefTechStateByPower(powerEffective,
                                                                         Measurings.PressureAir, tIn); //Коэффициент технического состояния по мощности
                Results.CoefficientCharge = GetCoefficientCharge(powerEffective, powerMax); //Коэффициент загрузки, -
                Results.Surging = GetSurging(rpmRelative, _pumping.Value); //Удаленность от границ помпажа, %
                Results.RpmRelative = rpmRelative; // Относительные обороты
                Results.CompressionRatio = e; // Степень сжатия
                Results.DensitySuperchargerInlet = densitySuperchargerInlet;
                Results.PumpingCalculated = _pumping;
                Results.FuelGasConsumptionCalculated = _fuelGasConsumption;

                _itsOk = true;
            }

            return _itsOk;
        }

        public bool CalculateMotorieserte()
        {
            var pInAbs = Measurings.PIn + StandardConditions.P; //Давление на входе нагнетателя, кг/см2
            var pOutAbs = Measurings.POut + StandardConditions.P; //Давление на выходе нагнетателя, кг/см2

            var tInK = Measurings.TIn; //Температура на входе нагнетателя, К
            var tOutK = Measurings.TOut; //Температура на выходе нагнетателя, К

            var e = pOutAbs / pInAbs; //Степень сжатия, -

            var gasCompressibilityFactorInlet =
                SupportCalculations.GasCompressibilityFactorApproximate(pInAbs,
                                                                        tInK, Measurings.Density);
            //Коэф. сжимаемости на входе, - (согласно РД 153-39.0-112-2001 п.7.3)

            var gasCompressibilityFactorOutlet =
                SupportCalculations.GasCompressibilityFactorApproximate(pOutAbs,
                                                                        tOutK, Measurings.Density);
            //Коэф. сжимаемости на выходе, - (РД 153-39.0-112-2001 п.7.3)

            var gasCompressibilityFactorAverage = (gasCompressibilityFactorInlet + gasCompressibilityFactorOutlet) / 2;//ср. коэф. сжимаемости


            var delta = Measurings.Density.KilogramsPerCubicMeter / StandardConditions.DensityOfAir.KilogramsPerCubicMeter; //Относительная плотность газа по воздуху (СТО Газпром 2-3.5-113-2007 п.5.2.1)

            var gasConstant = 0.287 / delta; //Газовая постоянная, кДж/кг-К (ОНТП 51-1-85 п.12.37)

            var densitySuperchargerInlet = (pInAbs).Mpa * 1000 /
                                           (gasCompressibilityFactorInlet * gasConstant * tInK.Kelvins); //Плотность газа на входе в ЦБН, кг/м3 (ОНТП 51-1-85 п.12.38)

            var rpmRelative = Measurings.Rpm / RpmRated; //Относительные обороты ротора, -
            const double eceMechanic = 0.985; //Механический КПД ЦБН 0.985 или оценивается при проведении спец испытаний
            // var coefPressureAir = 9.8456 * PhysicalQuantityConversions.mmHg2Mpa(Measurings.PressureAir) + 0.0024; //Коэффициент, учитывающий влияние высоты над уровнем моря



            _pumping = Measurings.PumpingMeasured > 0 ? Measurings.PumpingMeasured : GetPumping(rpmRelative, e);

            if (!_pumping.HasValue)
            {
                ErrorMessage = "Объемная производительность не найдена";
                Results = new CompUnitModelCalculatedValues();
                _itsOk = false;
            }

            else
            {
                //var massFlowRate = pInAbs.Mpa * 98066.5 * _pumping.Value / (gasCompressibilityFactorInlet * gasConstant * 1000 * tInK.Kelvins * 60);
                var massFlowRate = pInAbs.Mpa * 1000.0 * _pumping.Value / (gasCompressibilityFactorInlet * gasConstant * tInK.Kelvins * 60);
                //Массовый расход газа через ЦБН, кг/с (СТО Газпром 2-3.5-051-2006 п.18.7.2.2)

                var powerIn = GetPowerIn(gasCompressibilityFactorAverage, gasConstant, tInK.Kelvins, tOutK.Kelvins, massFlowRate,
                                           delta, pInAbs.Mpa, pOutAbs.Mpa, _pumping.Value, rpmRelative);


                var polytrEfficiency = GetPolytropicEfficiency(_pumping.Value, rpmRelative, tInK.Kelvins, tOutK.Kelvins, delta, pInAbs.Mpa,
                                                                 pOutAbs.Mpa);


                var powerEffective = GetPowerEffective(powerIn, eceMechanic);
                //Вычисление эффективной мощности, кВт (СТО Газпром 2-3.5-113-2007 п.5.2.1)


                Results.PumpingCommercial = massFlowRate * gasConstant / 4; //Вычисление коммерческой производительности, млн м3/сут (СТО Газпром 2-3.5-051-2006 п.18.7.2.2)

                Results.PowerEffective = powerEffective;
                Results.PowerIn = powerIn;
                Results.PolytropicEfficiency = polytrEfficiency;
                Results.CoefTechState = GetCoefTechStateSupercharger(polytrEfficiency); // Коэффициент технического состояния ЦБН

                Results.Surging = GetSurging(rpmRelative, _pumping.Value); //Удаленность от границ помпажа, %
                Results.RpmRelative = rpmRelative; // Относительные обороты
                Results.CompressionRatio = e; // Степень сжатия
                Results.DensitySuperchargerInlet = densitySuperchargerInlet;
                Results.PumpingCalculated = _pumping;
                Results.NGgpa = polytrEfficiency * MotorisierteEfficiencyFactor * ReducerEfficiencyFactor;

                _itsOk = true;
            }




            return _itsOk;
        }

    }

    public class CompUnitModelMeasuredValues
    {
        /// <summary>
        /// Избыточное давление на входе нагнетателя, кг/см2
        /// </summary>
        public Pressure PIn { get; set; }

        /// <summary>
        /// Избыточное давление на выходе нагнетателя, кг/см2
        /// </summary>
        public Pressure POut { get; set; }

        /// <summary>
        /// Температура на входе нагнетателя, C
        /// </summary>
        public Temperature TIn { get; set; } = Temperature.FromCelsius(0);

        /// <summary>
        /// Температура на выходе нагнетателя, С
        /// </summary>
        public Temperature TOut { get; set; } = Temperature.FromCelsius(0);

        /// <summary>
        /// Рабочие обороты ротора ЦБН, об/мин
        /// </summary>
        public double Rpm { get; set; }

        /// <summary>
        /// Объемная производительность, м3/мин
        /// </summary>
        public double PumpingMeasured { get; set; }

        /// <summary>
        /// Расход ТГ, тыс. м³/ч
        /// </summary>
        public double FuelGasConsumptionMeasured { get; set; }

        /// <summary>
        /// Низшая теплота сгорания газа, ккал/м³
        /// </summary>
        public CombustionHeat CombustionHeatLow { get; set; }

        /// <summary>
        /// Плотность газа, кг/м³
        /// </summary>
        public Density Density { get; set; }

        /// <summary>
        /// Атмосферное давление, мм рт ст
        /// </summary>
        public Pressure PressureAir { get; set; }

        /// <summary>
        /// Температура наружного воздуха, C
        /// </summary>
        public Temperature TemperatureAir { get; set; }

    }
    
    public class CompUnitModelCalculatedValues
    {
        /// <summary>
        /// Степень сжатия, -
        /// </summary>
        public double CompressionRatio { get; set; }

        /// <summary>
        /// Относительные обороты, -
        /// </summary>
        public double RpmRelative { get; set; }

        /// <summary>
        /// Расход ТГ, тыс. м³/час
        /// </summary>
        public double? FuelGasConsumptionCalculated { get; set; }

        /// <summary>
        /// Объем перекачиваемого газа, м³/мин
        /// </summary>
        public double? PumpingCalculated { get; set; }

        /// <summary>
        /// Коммерч. производительность, млн.м³/сут
        /// согласно СТО Газпром 2-3.5-051-2006 п.18.7.2.2
        /// </summary>
        public double? PumpingCommercial { get; set; }

        /// <summary>
        /// Удаленность от границ помпажа
        /// Коэффициент должен быть не менее 1,1
        /// </summary>
        public double? Surging { get; set; }

        /// <summary>
        /// КПД ГПА, -
        /// СТО Газпром 2-3.5-113-2007 п.5.2.1
        /// </summary>
        public double? NGgpa { get; set; }

        /// <summary>
        /// Коэф. техн. состояния ЦБН, -
        /// </summary>
        public double? CoefTechState { get; set; }

        /// <summary>
        /// Коэф. техн. состояния ГТУ по топл. газу, -
        /// согласно СТО Газпром 2-3.5-113-2007 п.5.2.3.2
        /// </summary>
        public double? CoefTechStateByFuelGas { get; set; }

        /// <summary>
        /// Коэф. техн. состояния ГТУ по мощности, -
        /// </summary>
        public double? CoefTechStateByPower { get; set; }

        /// <summary>
        /// Политропный КПД ЦБН, -
        /// </summary>
        public double? PolytropicEfficiency { get; set; }

        /// <summary>
        /// Коэффициент загрузки, -
        /// Должен быть меньше 0.95
        /// </summary>
        public double? CoefficientCharge { get; set; }

        /// <summary>
        /// Внутренняя мощность ЦБН, кВт
        /// </summary>
        public double? PowerIn { get; set; }

        /// <summary>
        /// Располагаемая мощность ГПА, кВт
        /// </summary>
        public double? PowerMax { get; set; }

        /// <summary>
        /// Эффективная мощность ЦБН, кВт
        /// </summary>
        public double? PowerEffective { get; set; }

        /// <summary>
        /// Эффективный КПД ГТУ, -
        /// </summary>
        public double? Efficiency { get; set; }

        /// <summary>
        /// Плотность на входе в нагнетатель, кг/м³
        /// </summary>
        public double DensitySuperchargerInlet { get; set; }


    }

    public class CubicSpline
    {
        private SplineTuple[] _splines; // Сплайн

        /// <summary>
        /// Структура, описывающая сплайн на каждом сегменте сетки
        /// </summary>
        private struct SplineTuple
        {
            public double A, B, C, D, X;
        }

        /// <summary>
        /// Построение сплайна
        ///  </summary>
        /// <param name="x">Узлы сетки, должны быть упорядочены по взрастанию, кратные узлы запрещены</param>
        /// <param name="y">Значения функции в узлах сетки</param>
        /// <param name="n">Количество узлов сетки</param>
        public void BuildSpline(double[] x, double[] y, int n)
        {
            // Инициализация массива сплайнов
            _splines = new SplineTuple[n];
            for (int i = 0; i < n; ++i)
            {
                _splines[i].X = x[i];
                _splines[i].A = y[i];
            }
            _splines[0].C = _splines[n - 1].C = 0.0;

            // Решение СЛАУ относительно коэффициентов сплайнов c[i] методом прогонки для трехдиагональных матриц
            // Вычисление прогоночных коэффициентов - прямой ход метода прогонки
            var alpha = new double[n - 1];
            var beta = new double[n - 1];
            alpha[0] = beta[0] = 0.0;
            for (int i = 1; i < n - 1; ++i)
            {
                var hi = x[i] - x[i - 1];
                var hi1 = x[i + 1] - x[i];
                var a = hi;
                var c = 2.0 * (hi + hi1);
                var b = hi1;
                var f = 6.0 * ((y[i + 1] - y[i]) / hi1 - (y[i] - y[i - 1]) / hi);
                var z = (a * alpha[i - 1] + c);
                alpha[i] = -b / z;
                beta[i] = (f - a * beta[i - 1]) / z;
            }

            // Нахождение решения - обратный ход метода прогонки
            for (int i = n - 2; i > 0; --i)
            {
                _splines[i].C = alpha[i] * _splines[i + 1].C + beta[i];
            }

            // По известным коэффициентам c[i] находим значения b[i] и d[i]
            for (int i = n - 1; i > 0; --i)
            {
                double hi = x[i] - x[i - 1];
                _splines[i].D = (_splines[i].C - _splines[i - 1].C) / hi;
                _splines[i].B = hi * (2.0 * _splines[i].C + _splines[i - 1].C) / 6.0 + (y[i] - y[i - 1]) / hi;
            }


        }

        /// <summary>
        /// Вычисление значения интерполированной функции в произвольной точке
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public double Interpolate(double x)
        {
            int n = _splines.Length;
            SplineTuple s;

            if (x <= _splines[0].X) // Если x меньше точки сетки x[0] - пользуемся первым эл-тов массива
            {
                s = _splines[0];
            }
            else if (x >= _splines[n - 1].X) // Если x больше точки сетки x[n - 1] - пользуемся последним эл-том массива
            {
                s = _splines[n - 1];
            }
            else // Иначе x лежит между граничными точками сетки - производим бинарный поиск нужного эл-та массива
            {
                int i = 0;
                int j = n - 1;
                while (i + 1 < j)
                {
                    int k = i + (j - i) / 2;
                    if (x <= _splines[k].X)
                    {
                        j = k;
                    }
                    else
                    {
                        i = k;
                    }
                }
                s = _splines[j];
            }

            double dx = x - s.X;
            // Вычисляем значение сплайна в заданной точке по схеме Горнера (в принципе, "умный" компилятор применил бы схему Горнера сам, но ведь не все так умны, как кажутся)
            return s.A + (s.B + (s.C / 2.0 + s.D * dx / 6.0) * dx) * dx;
        }

    }

    public class Point : IComparable
    {
        /// <summary>
        /// 
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Y { get; set; }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            var othetPoint = obj as Point;
            if (othetPoint != null)
                return X.CompareTo(othetPoint.X);
            throw new Exception("Object is not a Point");
        }
    }
    
}
