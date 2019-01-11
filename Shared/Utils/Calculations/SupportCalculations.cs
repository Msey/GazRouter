using System;
using System.Collections.Generic;
using Utils.Units;
namespace Utils.Calculations
{
    public static class SupportCalculations
    {
        /// <summary>
        ///     Расчет геометрического объема цилидрического тела (участка газопровода), м³
        /// </summary>
        /// <param name="d">Внутренний диаметр трубопровода, м</param>
        /// <param name="l">Длина участка трубопровода, м</param>
        /// <returns></returns>
        public static double GeometricVolume(double d, double l)
        {
            // Геометрический объем участка газопровода, м³
            return Math.PI*Math.Pow(d, 2)/4*l;
        }
        /// <summary>
        ///     Расчет коэффициента сжимаемости газа по приближенной формуле.
        ///     Допускается в диапазоне давлений до 8,45 МПа и температур от 273 до 350 К
        ///     ВАЖНО! Плотность передается абсолютная, к относительной по воздуху приводится уже в функции расчета
        /// </summary>
        /// <param name="p">Абсолютное давление газа, МПа</param>
        /// <param name="t">Температура газа, К</param>
        /// <param name="ro">Абсолютная плотность газа, кг/м³</param>
        /// <returns>Значение коэффициента сжимаемости газа</returns>
        public static double GasCompressibilityFactorApproximate(Pressure p, Temperature t, Density ro, ref string listing)
        {
            //if (p > 8.45 || t < 273 || t > 350) return -1;
            var tK = t.Kelvins;
            var pMpa = p.Mpa;
            if (pMpa > 25 || tK < 260 || tK > 500)
            {
                listing = string.Join("\n", new List<string>
                {
                    $"tK = {t.ToString(TemperatureUnit.Kelvin)}",
                    $"pMpa = {p.ToString(PressureUnit.Mpa)}",
                    $"т.к. {pMpa} > 8,45 || {tK} < 268 || {tK} > 350",
                    $"{listing} = -1",
                });
                return -1;
            }
          
            // Так как формула приближённая, то при расчете коэффициента сжимаемости газа при стандартных условиях полагают Z = 1
            if (Math.Abs(pMpa - StandardConditions.P.Mpa) < 0.001 &&
                Math.Abs(tK - StandardConditions.T.Kelvins) < 0.01){
                listing = string.Join("\n", new List<string>
                {
                    $"tK = {t.ToString(TemperatureUnit.Kelvin)}",
                    $"pMpa = {p.ToString(PressureUnit.Mpa)}",
                    $"т.к. {Math.Abs(pMpa - StandardConditions.P.Mpa)} < 0.001 && {Math.Abs(tK - StandardConditions.T.Kelvins)} < 0.01",
                    $"{listing} = 1",
                });
                return 1;
            }

            if (pMpa < 8.45 && tK >= 273 && tK <= 350)
            {
                var result = 1 -
                             ((10.2 * pMpa - 6) *
                              (0.00345 * ro.KilogramsPerCubicMeter / StandardConditions.DensityOfAir.KilogramsPerCubicMeter -
                               0.000446) +
                              0.015) * (1.3 - 0.0144 * (tK - 283.2));

                listing = string.Join("\n", new List<string>
                {
                    $"{listing} = 1 - ((10,2 * {pMpa} - 6) * (0,00345 * {ro.KilogramsPerCubicMeter} / {StandardConditions.DensityOfAir.KilogramsPerCubicMeter} - 0,000446) + 0,015) * (1,3 - 0,0144 * ({tK} - 283,2)) = {result}",
                    $"tK = {t.ToString(TemperatureUnit.Kelvin)}",
                    $"pMpa = {p.ToString(PressureUnit.Mpa)}",
                    $"ro = {ro.KilogramsPerCubicMeter}",
                });

                return result;
            }

            var ro_otn = ro.KilogramsPerCubicMeter / StandardConditions.DensityOfAir.KilogramsPerCubicMeter;

            var a = -34.14 * Math.Pow(ro_otn, 2.0) + 45.744 * ro_otn - 13.898;
            var b = -5.174 * Math.Pow(ro_otn, 2.0) + 6.9314 * ro_otn - 2.114;
            var c = -62.583 * Math.Pow(ro_otn, 2.0) + 83.133 * ro_otn - 23.678;
            var d = -107.8 * Math.Pow(ro_otn, 2.0) + 143.13 * ro_otn - 42.305;
            var calc = ((-1.0) * a * tK / 1000.0 * (1.74 * Math.Pow(tK / 1000.0, 2.0) - 2.3 * tK / 1000.0 + 1) + b) * pMpa +
                       c * tK / 100.0 * (1.55 * Math.Pow(tK / 1000.0, 2.0) - 2.15 * tK / 1000.0 + 1) - d;
            //
            listing = string.Join("\n", new List<string>
            {
                $"{listing} = ((-1) * a * {tK} / 1000 * (1,74 * ({tK} / 1000)^2 - 2,3 * {tK} / 1000 + 1) + b) * {pMpa} + c * {tK} / 100 * (1,55 * ({tK} / 1000)^ 2 - 2,15 * {tK} / 1000 + 1) - d = {calc}",
                $"ro_otn = {ro.KilogramsPerCubicMeter} / {StandardConditions.DensityOfAir.KilogramsPerCubicMeter} = {ro_otn}",
                $"ro = {ro.KilogramsPerCubicMeter}",
                $"a = -34,14 * {ro_otn}^2 + 45,744 * {ro_otn} - 13,898 = {a}",
                $"b = -5,174 * {ro_otn}^2 + 6,9314 * {ro_otn} - 2,114 = {b}",
                $"c = -62,583 * {ro_otn}^2 + 83,133 * {ro_otn} - 23,678 = {c}",
                $"d = -107,8 * {ro_otn}^2 + 143,13 * {ro_otn} - 42,305 = {d}",
                $"tK = {t.ToString(TemperatureUnit.Kelvin)}",
                $"pMpa = {p.ToString(PressureUnit.Mpa)}",
            });
            //
            return calc;
        }
        public static double GasCompressibilityFactorApproximate(Pressure p, Temperature t, Density ro)
        {
            var stringEmpty = string.Empty;
            return GasCompressibilityFactorApproximate(p, t, ro, ref stringEmpty);
        }
        //public static double PolytropicWorkOfCompression(double z1, double t1, double q, double p1, double p2)
        //{
        //    // Расчет политропной работы сжатия КЦ, кВт*ч
        //    //
        //    // z1 - коэффициент сжимаемости газа по условиям на входе в КЦ
        //    // t1 - температура газа на входе КЦ, К
        //    // q - объем газа, перекачиваемый КЦ за планируемый период времени, млн. м³
        //    // p1, p2 - абсолютное давление на входе нагнетателей первой ступени и выходе нагнетателей последней ступени сжатия, МПа

        //    double k = 320.25; // коэффициент для согласования размерностей величин

        //    double e = 0; // степень повышения давления газа КЦ
        //    if (p1 != 0)
        //        e = p2 / p1;

        //    return k * z1 * t1 * q * (Math.Pow(e, 0.3) - 1);
        //}
        public static double HeatCapacity1(double p, double t)
        {
            return 0.797374487658699
                   - 0.0000520000077332135*t
                   + 0.0000292944444845762*Math.Pow(t, 2)
                   - 0.000001604166659057*Math.Pow(t, 3)
                   + 4.6527777602884*Math.Pow(10, -8)*Math.Pow(t, 4)
                   - 7.58333333597569*Math.Pow(10, -10)*Math.Pow(t, 5)
                   + 5.27777779161986*Math.Pow(10, -12)*Math.Pow(t, 6)
                   - 0.0232951954885227*p
                   + 0.000846661705050654*Math.Pow(p, 2)
                   - 0.0000123082482892776*Math.Pow(p, 3)
                   + 6.58659296768673*Math.Pow(10, -8)*Math.Pow(p, 4);
        }
        /// <summary>
        ///     Расчет теплоемкости газа
        /// </summary>
        /// <param name="p">Абсолютное давление газа, МПа</param>
        /// <param name="t">Температура газа, К</param>
        /// <returns>Теплоемкость</returns>
        public static double HeatCapacity2(double p, double t)
        {
            return (1.695
                    + 1.838*Math.Pow(10, -3)*t
                    + 1.96*Math.Pow(10, 6)*(p - 0.1)*Math.Pow(t, -3))/4.2;
        }
        /// <summary>
        ///     Расчет теплоемкости газа
        ///     согласно СТО Газпром 2-3.5-051
        /// </summary>
        /// <param name="p">Абсолютное давление газа, МПа</param>
        /// <param name="t">Температура газа, К</param>
        /// <param name="xa">Содержание азота (молярная доля)</param>
        /// <param name="xco">Содержание диоксида углерода (молярная доля)</param>
        /// <param name="ro">Плотность газа, кг/м³</param>
        /// <returns>Теплоемкость, кДж/(кг·К)</returns>
        public static double HeatCapacity3(double p, double t, double xa, double xco, Density ro)
        {
            var stringEmpty = string.Empty;
            return HeatCapacity3(p, t, xa, xco, ro, ref stringEmpty);
        }
        public static double HeatCapacity3(double p, double t, double xa, double xco, Density ro, ref string listing)
        {
            var criticalTemperatureListing = string.Empty;
            var criticalTemperature = CriticalTemperature(ro, xa, xco, ref criticalTemperatureListing);
            var tpr = t / criticalTemperature;
            var e0 = 4.437 - 1.015 * tpr + 0.591 * Math.Pow(tpr, 2.0);
            var e1 = 3.29 - 11.37 / tpr + 10.9 / Math.Pow(tpr, 2.0);
            var e2 = 3.23 - 16.27 / tpr + 25.48 / Math.Pow(tpr, 2.0) - 11.81 / Math.Pow(tpr, 3.0);
            var e3 = -0.214 + 0.908 / tpr - 0.967 / Math.Pow(tpr, 2.0);
            var criticalPressureListing = string.Empty;
            var criticalPressure = CriticalPressure(ro, xa, xco, ref criticalPressureListing);
            var ppr = p / criticalPressure;
            var molarMassListing = string.Empty;
            var molarMass = MolarMass(xa, xco, ref molarMassListing);
            var r = PhysicalConstants.R / molarMass;
            var calc = r * (e0 + e1 * ppr + e2 * Math.Pow(ppr, 2.0) + e3 * Math.Pow(ppr, 3));
            // 
            listing = string.Join("\n", new List<string> {

                $"{listing} = {r} * ({e0} + {e1} * {ppr} + {e2} * {ppr}^2 + {e3} * {ppr}^3) = {calc} кДж/(кг∙К) - средняя изобарная теплоемкость газа (СТО Газпром 2-3.5-051)",
                $"R = {PhysicalConstants.R} / {molarMass} = {r}",
                $"M = {molarMassListing} = {molarMass} г⁄моль - молярная масса",
                $"xN = {xa} - содержание азота",
                $"xCO= {xco} - содержание диоксида углерода",
                $"P = {p} МПа - давление газа",
                $"T = {t} К - температура газа",
                $"Pкр = {criticalPressureListing} = {criticalPressure} - псевдокритическое давление",
                $"Tкр = {criticalTemperatureListing} = {criticalTemperature} - псевдокритическая температура",
                $"Pпр = {p} / {criticalPressure} = {ppr} - приведенное давление",
                $"Tпр = {t} / {criticalTemperature} = {tpr} - приведенная температура",
                $"E0 = 4,437 - 1,015 * {tpr} + 0,591 * {tpr}^2 = {e0} - коэффициент E0",
                $"E1 = 3,29 - 11,37 / {tpr} + 10,9 / {tpr}^2 = {e1} - коэффициент E1",
                $"E2 = 3,23 - 16,27 / {tpr} + 25,48 / {tpr}^2 - 11,81 / {tpr}^3 = {e2} - коэффициент E2",
                $"E3 = -0,214 + 0,908 / {tpr} - 0,967 / {tpr}^2 = {e3} - коэффициент E3",
            });
            return calc;
        }

        /// <summary>
        ///     Расчет коэффициента Джоуля-Томсона
        ///     согласно СТО Газпром 2-3.5-051
        /// </summary>
        /// <param name="p">Абсолютное давление газа, МПа</param>
        /// <param name="t">Температура газа, К</param>
        /// <param name="xa">Содержание азота (молярная доля)</param>
        /// <param name="xco">Содержание диоксида углерода (молярная доля)</param>
        /// <param name="ro">Плотность газа, кг/м³</param>
        /// <returns>Коэффициента Джоуля-Томсона, К/МПа</returns>
        public static double JouleThomsonCoefficient(double p, double t, double xa, double xco, Density ro)
        {
            var stringEmpty = string.Empty;
            return JouleThomsonCoefficient(p, t, xa, xco, ro, ref stringEmpty);
        }

        public static double JouleThomsonCoefficient(double p, double t, double xa, double xco, Density ro, ref string listing)
        {
            var criticalTemperatureListing = string.Empty;
            var criticalTemperature = CriticalTemperature(ro, xa, xco, ref criticalTemperatureListing);
            var tpr = t / criticalTemperature;
            var h0 = 24.96 - 20.3 * tpr + 4.57 * Math.Pow(tpr, 2.0);
            var h1 = 5.66 - 19.92 / tpr + 16.89 / Math.Pow(tpr, 2.0);
            var h2 = -4.11 + 14.68 / tpr - 13.39 / Math.Pow(tpr, 2.0);
            var h3 = 0.568 - 2.0 / tpr + 1.79 / Math.Pow(tpr, 2.0);
            var criticalPressureListing = string.Empty;
            var criticalPressure = CriticalPressure(ro, xa, xco, ref criticalPressureListing);
            var ppr = p / criticalPressure;
            var coef = h0 + h1 * ppr + h2 * Math.Pow(ppr, 2.0) + h3 * Math.Pow(ppr, 3);

            listing = string.Join("\n", new List<string>
            {
                $"{listing} = {h0} + {h1} * {ppr} + {h2} * {ppr}^2 + {h3} * {ppr}^3 = {coef} - коэффициент Джоуля-Томпсона (СТО Газпром 2-3.5-051)",
                $"P = {p} МПа - давление газа",
                $"xN = {xa} - содержание азота",
                $"xCO = {xco} - содержание диоксида углерода",
                $"Pкр = {criticalPressureListing} = {criticalPressure} - псевдокритическое давление",
                $"ppr = {p} / {criticalPressure} = {ppr} - приведенное давление",
                $"T = {t} К - температура газа",
                $"Tкр = {criticalTemperatureListing} = {criticalTemperature} - псевдокритическая температура",
                $"Tпр = {t} / {criticalTemperature} = {tpr} - приведенная температура",
                $"H0 = 24,96 - 20,3 * {tpr} + 4,57 * {tpr}^2 = {h0} - коэффициент H0",
                $"H1 = 5,66 - 19,92 / {tpr} + 16,89 / {tpr}^2 = {h1} - коэффициент H1",
                $"H2 = -4,11 + 14,68 / {tpr} - 13,39 / {tpr}^2 = {h2}   коэффициент H2",
                $"H3 = 0,568 - 2,0 / {tpr} + 1,79 / {tpr}^2 = {h3} - коэффициент H3",
            });
            return coef;
        }

        /// <summary>
        ///     Расчет показателя адиабаты по усовершенствованной формуле Кобза
        ///     согласно ГОСТ 30319.1
        /// </summary>
        /// <param name="p">Абсолютное давление газа, МПа</param>
        /// <param name="t">Температура газа, К</param>
        /// <param name="ro">Плотность газа</param>
        /// <param name="xa">Содержание азота (молярная доля)</param>
        /// <returns></returns>
        public static double AdiabaticIndex(Pressure p, Temperature t, Density ro, double xa, ref string listing)
        {
            var tK = t.Kelvins;
            if (p == Pressure.Zero || tK == 0) return 0;
            var result = 1.556*(1 + 0.074*xa) - 0.00039*tK*(1 - 0.68*xa) - 0.208*ro.KilogramsPerCubicMeter + Math.Pow(p.Mpa/tK, 1.43)*(384*(1 - xa)*Math.Pow(p.Mpa/tK, 0.8) + 26.4*xa);
            //
            listing = string.Join("\n", new List<string>
            {
                $"{listing} = 1,556*(1 + 0,074*{xa}) - 0,00039*{tK}*(1 - 0,68*{xa}) - 0,208*{ro.KilogramsPerCubicMeter} + (({p.Mpa}/{tK})^1,43)*(384*(1 - {xa})*({p.Mpa}/{tK})^0,8 + 26,4*{xa}) = {result} - показатель адиабаты газа (ГОСТ 30319.1)",
                $"xN = {xa} - содержание азота",
                $"tK = {t.ToString(TemperatureUnit.Kelvin)} - температура газа",
                $"ro = {ro.KilogramsPerCubicMeter} - плотность газа",
                $"P = {p.ToString(PressureUnit.Mpa)} - давление газа",
            });
            //
            return result;
        }
        public static double AdiabaticIndex(Pressure p, Temperature t, Density ro, double xa)
        {
            var listing = string.Empty;
            return AdiabaticIndex(p, t, ro, xa, ref listing);
        }
        /// <summary>
        ///     Расчет числа Рейнольдса
        ///     согласно СТО Газпром 2-3.5-051-2006
        /// </summary>
        /// <param name="m">Динамическая вязкость газа, Па*с</param>
        /// <param name="ro">Абсолютная плотность газа, кг/м³</param>
        /// <param name="d">Диаметр трубы, м³</param>
        /// <param name="q">Расход газа, м³/c</param>
        /// <returns></returns>
        public static double ReynoldsNumber(double m, Density ro, double d, double q)
        {
            return 17750*q*0.0864*ro.KilogramsPerCubicMeter/StandardConditions.DensityOfAir.KilogramsPerCubicMeter/d/m;
            //0.0864 - коэффициент перевода из м3/с в млн м3/сут , так как Число Рейнольдса в соответствии
            //с СТО считается с q в млн м3/сут
        }
        /// <summary>
        ///     Расчет динамической вязкости газа
        ///     при давлениях до 15МПа и температурах 250 - 400К
        ///     согласно СТО Газпром 2-3.5-051-2006
        /// </summary>
        /// <param name="p">Абсолютное давление газа, МПа</param>
        /// <param name="t">Температура газа, К</param>
        /// <param name="ro"></param>
        /// <param name="xa"></param>
        /// <param name="xco"></param>
        /// <returns>Динамическая вязкость газа, Па*с</returns>
        public static double DynamiсViscosity(Pressure p, Temperature t, Density ro, double xa, double xco)
        {
            var tpr = t.Kelvins/CriticalTemperature(ro, xa, xco); // Температура приведенная
            var ppr = p.Mpa/CriticalPressure(ro, xa, xco); // Давление приведенное

            var m0 = (1.81 + 5.95*tpr)*Math.Pow(10, -6);
            var b1 = tpr == 0 ? 0 : -0.67 + 2.36/tpr - 1.93/(tpr*tpr);
            var b2 = tpr == 0 ? 0 : 0.8 - 2.89/tpr + 2.65/(tpr*tpr);
            var b3 = tpr == 0 ? 0 : -0.1 + 0.354/tpr - 0.314/(tpr*tpr);

            return m0*(1 + b1*ppr + b2*ppr*ppr + b3*ppr*ppr*ppr);
        }
        /// <summary>
        ///     Расчет скорости звука в газе
        /// </summary>
        /// <param name="p">Абсолютное давление газа, МПа</param>
        /// <param name="t">Температура газа, К</param>
        /// <param name="ro">Абсолютная плотность газа, кг/м³</param>
        /// <param name="xa">Содержание азота (мол.доля)</param>
        /// <returns></returns>
        public static double VelocityOfSound(Pressure p, Temperature t, Density ro, double xa)
        {
            var k = AdiabaticIndex(p, t, ro, xa);
            var z = GasCompressibilityFactorApproximate(p, t, ro);

            return 18.591*Math.Sqrt(t.Kelvins*k*z/ro.KilogramsPerCubicMeter);
        }
        /// <summary>
        ///     Расчет псевдокритического давления газа
        /// </summary>
        /// <param name="ro">Плотность газа, кг/м³</param>
        /// <param name="xa">Содержание азота (мол.доля)</param>
        /// <param name="xco">Содержание диоксида углерода (мол.доля)</param>
        /// <returns>Псевдокритическое давление, МПа</returns>
        public static double CriticalPressure(Density ro, double xa, double xco, ref string listing)
        {
            //return 4.5988*(1 - xa - xco) + 3.390*xa + 7.386*xco;
            var calc = 2.9585*(1.608 - 0.05994*ro.KilogramsPerCubicMeter + xco - 0.392*xa);
            listing = string.Join("\n", new List<string>
            {
                $"2,9585 * (1,608 - 0,05994 * {ro.KilogramsPerCubicMeter} + {xco} - 0,392*{xa})",
            });
            return calc;
        }
        public static double CriticalPressure(Density ro, double xa, double xco)
        {
            var stringEmpty = string.Empty;
            return CriticalPressure(ro, xa, xco, ref stringEmpty);
        }
        /// <summary>
        ///     Расчет псевдокритической температуры газа
        /// </summary>
        /// <param name="ro">Плотность газа, кг/м³</param>
        /// <param name="xa">Содержание азота (мол.доля)</param>
        /// <param name="xco">Содержание диоксида углерода (мол.доля)</param>
        /// <returns>Псевдокритическоя температура, К</returns>
        public static double CriticalTemperature(Density ro, double xa, double xco, ref string listing)
        {
            //return 190.555*(1 - xa - xco) + 126.2*xa + 304.2*xco;
            var calc = 88.25*(0.9915 + 1.759*ro.KilogramsPerCubicMeter - xco - 1.681*xa);
            listing = string.Join("\n", new List<string>{
                $"88,25 * (0,9915 + 1,759 * {ro.KilogramsPerCubicMeter} - {xco} - 1,681 * {xa})",
            });
            return calc;
        }
        public static double CriticalTemperature(Density ro, double xa, double xco)
        {
            //return 190.555*(1 - xa - xco) + 126.2*xa + 304.2*xco;
            var stringEmpty = string.Empty;
            return CriticalTemperature(ro, xa, xco, ref stringEmpty);
        }
        /// <summary>
        ///     Расчет политропной работы сжатия
        /// </summary>
        /// <param name="pIn">Давление входа КЦ (ГПА), МПа</param>
        /// <param name="pOut">Давление выхода КЦ (ГПА), МПа</param>
        /// <param name="t1">Температура входа КЦ (ГПА), К</param>
        /// <param name="q">Перекачка газа, млн.м3</param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static double PolytropicWorkOfCompression(Pressure pIn, Pressure pOut, Temperature t1, double q, double z, ref string listing)
        {
            var calc = 320.0*z*t1.Kelvins*q*(Math.Pow(pOut/pIn, 0.3) - 1);
            listing = string.Join("\n", new List<string>
            {
                $"{listing} = 320,0 * {z} * {t1.Kelvins} * {q} * (({pOut.Mpa} / {pIn.Mpa})^0,3 - 1) = {calc} кВт*ч - политропная работа сжатия КЦ",
            });
            return calc;
            // 320.25 - коэф. согласования размерностей величин
        }
        /// <summary>
        ///     Расчет молярной массы газовой смеси
        /// </summary>
        /// <param name="xa">Содержание азота (мол.доля)</param>
        /// <param name="xco">Содержание диоксида углерода (мол.доля)</param>
        /// <returns>Молярная масса газовой смеси, кг/моль</returns>
        public static double MolarMass(double xa, double xco, ref string listing)
        {
            var calc = 16.043*(1 - xa - xco) + 28.01*xa + 44.01*xco;
            listing = string.Join("\n", new List<string>
                {
                    $"16,043*(1 - {xa} - {xco}) + 28,01*{xa} + 44,01*{xco}",
                });
            return calc;
        }
        public static double MolarMass(double xa, double xco)
        {
            var stringEmpty = string.Empty;
            return MolarMass(xa, xco, ref stringEmpty);
        }
        /// <summary>
        ///     Расчет удельной газовой постоянной
        /// </summary>
        /// <param name="xa">Содержание азота (мол.доля)</param>
        /// <param name="xco">Содержание диоксида углерода (мол.доля)</param>
        /// <returns>Удельная газовая постоянная, Дж/(кг*К)</returns>
        public static double SpecificGasConstant(double xa, double xco)
        {
            return 1000.0 * PhysicalConstants.R/MolarMass(xa, xco);
        }
        /// <summary>
        ///     Расчет среднего давления газа на участке газопровода
        /// </summary>
        /// <param name="p1">Давление газа в начале участка, МПа</param>
        /// <param name="p2">Давление газа в конце участка, МПа</param>
        /// <returns>Среднее давление газа, МПа</returns>
        public static double PipelineSectionAveragePressure(double p1, double p2)
        {
            var stringEmpty = string.Empty;
            return 2/3.0*(p1 + Math.Pow(p2, 2.0)/(p1 + p2));
        }
        /// <summary>
        ///     Расчет среднего давления газа на участке газопровода
        /// </summary>
        /// <param name="p1">Давление газа в начале участка, МПа</param>
        /// <param name="p2">Давление газа в конце участка, МПа</param>
        /// <param name="listing"/>
        /// <returns>Среднее давление газа, МПа</returns>
        public static double PipelineSectionAveragePressure(double p1, double p2, ref string listing)
        {
            var result = 2/3.0*(p1 + Math.Pow(p2, 2.0)/(p1 + p2));
            var stringEmpty = string.Empty;
            listing = string.Join("\n", new List<string>
                {
                    $"2/3 * ({p1} + {p2}^2 / ({p1} + {p2})) = {result}",
                });
            return result;
        }
    }
}