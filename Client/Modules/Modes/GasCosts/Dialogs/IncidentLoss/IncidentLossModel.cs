using System;
using System.ComponentModel.DataAnnotations;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using System.Collections.Generic;
using System.Linq;
using Utils.Calculations;
using Utils.Units;
using Pressure = Utils.Units.Pressure;
namespace GazRouter.Modes.GasCosts.Dialogs.IncidentLoss
{
    public class BranchParameter
    {
        public uint Number;
        public double Length;
        public double Diameter;
        public double Lambda;
        public double Result;
    }

    public class IncidentLossModel : Listing, ICostCalcModel
    {
        public double Calculate()
        {
            BranchParameterList.Where(i => i.Length > 0 && i.Diameter > 0).ToList()
                .ForEach(i => i.Result = Math.Round(i.Lambda*i.Length*Math.Pow(i.Diameter/1000.0, -5.0), 3));
            var pAir = PressureAir;
            var pn = pAir + PressureIn;
            var pMg = pAir + PressureOut;
            var pMgPa = pMg.Mpa*Math.Pow(10, 6.0); // Па
            var pnPa = pn.Mpa*Math.Pow(10, 6.0); // Па

            var branchDm = BranchParameterList.First(p => p.Result == BranchParameterList.Max(i => i.Result)).Diameter / 1000.0; // м
            var pipeDm = PipeDiameter / 1000.0; // м

            var branchLength = BranchParameterList.First(p => p.Result == BranchParameterList.Max(i => i.Result)).Length;

            var pAvg = SupportCalculations.PipelineSectionAveragePressure(pnPa, pMgPa); // Па
            var listingT1 = string.Empty;
            var tAvg = PipelineSectionAverageTemperature(TemperatureIn.Kelvins, TemperatureOut.Kelvins, ref listingT1);

            // коэффициент сжимаемости газа до аварии при параметрах Pмг, Tмг
            var zMg = SupportCalculations.GasCompressibilityFactorApproximate(pMg, TemperatureOut, Density);
            // коэффициент сжимаемости газа до аварии при параметрах Pср, Tср
            var zAvg = SupportCalculations.GasCompressibilityFactorApproximate(Pressure.FromMpa(pAvg/1000000.0), TemperatureOut, Density);

            // газовая постоянная
            var r = SupportCalculations.SpecificGasConstant(NitrogenContent / 100, CarbonDioxideContent / 100);

            // показатель адиабаты
            var k = SupportCalculations.AdiabaticIndex(pMg, TemperatureOut, Density, NitrogenContent / 100);

            // скорость звука в газе до разрыва, м/с (В.5)
            var ao = Math.Sqrt(k * r * zMg * TemperatureOut.Kelvins);
            var aoAvg = Math.Sqrt(k * r * zAvg * tAvg);

            // коэффициент гидравлического сопротивления при квадратичном режиме, когда k=0.03мм
            var branchLambda = 0.013;// 0.03817 / Math.Pow(BranchDiameter, 0.2);
            var lambda = 0.01; //0.03817 / Math.Pow(PipeDiameter, 0.2);

            // постоянная времени, с (В.4)
            var ep = 2.0 / 3.0 * branchLength / ao * Math.Sqrt(k * branchLambda * branchLength / branchDm);

            // масса газа, находящегося в патрубке до аварии Mn (В.3)
            var mp = branchLength * Math.PI*Math.Pow(branchDm, 2.0)*pMgPa/(4.0*r*zMg*TemperatureOut.Kelvins);

            // начальный критический массовый расход газа, кг/с (В.7)
            var gop = pMgPa * Math.PI * Math.Pow(branchDm, 2) * Math.Sqrt(k) / (4 * Math.Sqrt(r * zMg * TemperatureOut.Kelvins)) *
                     Math.Pow(2 / (k + 1), (k + 1) / (2 * (k - 1)));

            // коэффициент (В.6)
            var hp = 2.0 * mp / (ep * gop);

            // (В.2) кг
            var mnp = 2.0 * mp * branchDm / (branchLambda * branchLength * Math.Sqrt(k)) *
                     (Math.Sqrt(1 / k * Math.Pow((k + 1) / 2.0, (k + 1) / (k - 1)) + branchLambda * branchLength / branchDm) -
                      Math.Sqrt(1 / k * Math.Pow((k + 1) / 2.0, (k + 1) / (k - 1))));

            // Скорость газового потока в районе перемычек между нитками, необходимая для существования стационарного режима истечения (В.9)
            var m = MachNumberCalculator(k, branchLambda, branchLength, branchDm, 0.01, 1);
            var vp = m * ao;

            // Величина стационарного расхода (В.8)
            var gStac = pMgPa * Math.PI * Math.Pow(branchDm, 2.0) / (4.0 * r * zMg * TemperatureOut.Kelvins) * vp;

            // Расход газа Gn(t), кг/с, для патрубка до времени выхода на режим квазистационарного истечения (В.1)
            var tStac = Time(Gp, gStac, mp, hp, ep, 0);
            
            // Масса газа, выброшенного из патрубка на первом этапе истечения за время переходного процесса (Б.15)
            var m1 = mnp * (1 - Math.Exp(-tStac / (Math.Pow(hp, 2.0) * ep)));

            // постоянная времени, с (В.4)
            var eL = 2.0 / 3.0 * Length / aoAvg * Math.Sqrt(k * lambda * Length);

            // масса газа, находящаяся в нитке МГ, которая соединяется с аварийным патрубком на КС до аварии, кг
            var mmg = Length*Math.PI*Math.Pow(pipeDm, 2.0)*pAvg/(4.0*r*zAvg*tAvg);
            var go = pAvg * Math.PI * Math.Pow(pipeDm, 2) * Math.Sqrt(k) / (4 * Math.Sqrt(r * zAvg * tAvg)) *
                     Math.Pow(2 / (k + 1), (k + 1) / (2 * (k - 1)));
            // коэффициент (В.17)
            var hL = 2.0 * mmg / (eL * go);

            // (В.14)
            var mn = 2.0 * mmg * pipeDm / (lambda * Length * Math.Sqrt(k)) *
                     (Math.Sqrt(1 / k * Math.Pow((k + 1) / 2.0, (k + 1) / (k - 1)) + lambda * Length / pipeDm) -
                      Math.Sqrt(1 / k * Math.Pow((k + 1) / 2.0, (k + 1) / (k - 1))));


            var tOkonch = Time(Gmg, gStac, mn, hL, eL, mmg);
       
            // Аварийный массовый расход из МГ в отсутствие патрубка (как если бы разрыв произошел на входе/выходе в КС) (В.12)
            var gmg = mn / (Math.Pow(hL, 2.0) * eL) * Math.Exp(-tOkonch / (Math.Pow(hL, 2.0) * eL)) + (mmg - mn) / eL * Math.Exp(-tOkonch / eL);

            var m2 = gmg * (tOkonch - tStac);

            // Расчет истечения газа после окончания режима стационарного истечения до времени закрытия линейного крана КР7 (КР8) (В.20)
            var m3 = mn * (Math.Exp(-tOkonch / (Math.Pow(hL, 2.0) * eL)) - Math.Exp(-T7 / (Math.Pow(hL, 2.0) * eL))) +
                       (mmg - mn) * (Math.Exp(-tOkonch / eL) - Math.Exp(-T7 / eL));

            // Масса газа, находящегося в iой аварийной секции газопровода до аварии, кг (Б.19)
            var mx = BranchParameterList.Sum(i => i.Length*Math.Pow(i.Diameter/1000, 2.0))*Math.PI*pAvg/(4.0*r*zAvg*tAvg);
            var t = BranchParameterList.Where(i => i.Length > 0 && i.Diameter > 0).Aggregate(string.Empty, (current, item) => current + $"{item.Length}*СТЕПЕНЬ({item.Diameter/1000};2)+");
            t = t.Remove(t.Length-1);

            // Расчет массового расхода газа из аварийного газопровода после локализации аварии и закрытия линейных кранов (Б.18)
            var m4 = mx * (mn * Math.Exp(-T7 / (Math.Pow(hL, 2.0) * eL)) + (mmg - mn) * Math.Pow(hL, 2.0) * Math.Exp(-T7 / eL)) /
                      (mn + (mmg - mn) * Math.Pow(hL, 2.0));

           var mo = m1 + m2 + m3 + m4;
           var v = mo/(1000*Density.KilogramsPerCubicMeter);


            ListingCalculation = string.Join("\n", new List<string>
            {
                "Расчет выполнен согласно СТО Газпром 2-1.19-530-2011.",
                "Этап I. Расчет истечения газа от момента аварии до отсечения аварийного участка от МГ",
                "a. Расчет массы газа, выброшенного при переходном процессе установления стационарного профиля давления в патрубке",
                $"Мнп = 2*{mp}*{branchDm}/({branchLambda}*{branchLength}*КОРЕНЬ({k}))*(КОРЕНЬ(1/{k}*СТЕПЕНЬ(({k}+1)/2;({k}+1)/({k}-1))+{branchLambda}*{branchLength}/{branchDm})-КОРЕНЬ(1/{k}*СТЕПЕНЬ(({k}+1)/2;({k}+1)/({k}-1)))) = {mnp} кг (В.2)",
                $"Mп = {branchLength}*ПИ()*СТЕПЕНЬ({branchDm};2)*{pMgPa}/(4*{r}*{zMg}*{TemperatureOut.Kelvins}) = {mp} кг - масса газа, находящегося в патрубке до аварии (В.3)",
                $"εп = 2/3*{branchLength}/{ao}*КОРЕНЬ({k}*{branchLambda}*{branchLength}/{branchDm}) = {ep} - постоянная времени (В.4)",
                $"aₒ = КОРЕНЬ({k}*{r}*{zMg}*{TemperatureOut.Kelvins}) = {ao} м/с - скорость звука в газе до разрыва (В.5)",
                $"ηп = 2*{mp}/({ep}*{gop}) = {hp} (В.6)",
                $"Gₒ = {pMgPa}*ПИ()*СТЕПЕНЬ({branchDm};2)*КОРЕНЬ({k})/(4*КОРЕНЬ({r}*{zMg}*{TemperatureOut.Kelvins}))*СТЕПЕНЬ(2/({k}+1);({k}+1)/(2*({k}-1))) = {gop} кг/с - нач. критический массовый расход газа, кг/с (В.7)",
                $"Gстац = {pMgPa}*ПИ()*СТЕПЕНЬ({branchDm};2)/(4*{r}*{zMg}*{TemperatureOut.Kelvins})*{vp} = {gStac} кг/с - расход при достижении стационарного режима истечения (В.8)",
                $"νп = {m}*{ao} = {vp} м/с - скорость газового потока в районе перемычек между нитками, необходимая для существования стационарного режима истечения (В.9)",
                $"М = {m} - число Маха в рассматриваемой точке газового потока (В.10)",
                $"tстац = {tStac} с - значение момента времени, при котором расход становится равным стационарному",
                $"M1 = {mnp}*(1-EXP(-{tStac}/(СТЕПЕНЬ({hp};2)*{ep}))) = {m1} кг - масса газа, выброшенного из патрубка на 1м этапе истечения за время переходного процесса (В.11)",
                $"",
                "b. Расчет истечения газа после достижения режима стационарного истечения",
                $"Ммг = {Length}*ПИ()*СТЕПЕНЬ({pipeDm};2)*{pAvg}/(4*{r}*{zMg}*{tAvg}) = {mmg} кг - масса газа, находящаяся в нитке МГ, которая соединяется с аварийным патрубком на КС до аварии (В.13)",
                $"Мн = 2*{mmg}*{pipeDm}/({lambda}*{Length}*КОРЕНЬ({k}))*(КОРЕНЬ(1/{k}*СТЕПЕНЬ(({k}+1)/2;({k}+1)/({k}-1))+{lambda}*{Length}/{pipeDm})-КОРЕНЬ(1/{k}*СТЕПЕНЬ(({k}+1)/2;({k}+1)/({k}-1)))) = {mn} кг (В.14)",
                $"εL = 2/3*{Length}/{aoAvg}*КОРЕНЬ({k}*{lambda}*{Length}) = {eL} с - постоянная времени (В.15)",
                $"aₒ = КОРЕНЬ({k}*{r}*{zAvg}*{tAvg}) = {aoAvg} м/с - скорость звука в газе до разрыва (В.16)",
                $"ηL = 2*{mmg}/({eL}*{go}) = {hL} (В.17)",
                $"Gₒ = {pAvg}*ПИ()*СТЕПЕНЬ({pipeDm};2)*КОРЕНЬ({k})/(4*КОРЕНЬ({r}*{zAvg}*{tAvg}))*СТЕПЕНЬ(2/({k}+1);({k}+1)/(2*({k}-1))) = {go} кг/с - нач. критический массовый расход газа, кг/с (В.18)",
                $"tоконч = {tOkonch} c - время окончания стационарного режима истечения",
                $"Gмг(tоконч) = {mn}/(СТЕПЕНЬ({hL};2)*{eL})*EXP(-{tOkonch}/(СТЕПЕНЬ({hL};2)*{eL}))+({mmg}-{mn})/{eL}*EXP(-{tOkonch}/{eL}) = {gmg} кг/с - аварийный массовый расход из МГ в отсутствие патрубка (В.12)",
                $"M2 = {gStac}*({tOkonch}-{tStac}) = {m2} кг - масса газа, выброшенного на этапе стационарного истечения (В.19)",
                $"",
                $"c. Расчет истечения газа после окончания режима стационарного истечения до времени закрытия крана Кр7 (Кр8)",
                $"M3 = {mn}*(EXP(-{tOkonch}/(СТЕПЕНЬ({hL};2)*{eL}))-EXP(-{T7}/(СТЕПЕНЬ({hL};2)*{eL})))+({mmg}-{mn})*(EXP(-{tOkonch}/{eL})-EXP(-{T7}/{eL})) = {m3} кг (В.20)",
                $"",
                $"Этап II. Расчет массового расхода газа после локализации аварии и закрытия линейных кранов",
                $"mx = ({t})*ПИ()*{pAvg}/(4*{r}*{zAvg}*{tAvg}) = {mx} кг - суммарная масса газа, находящегося до аварии во всех однородных участках трубы в пределах площадочного объекта от крана, отсекающего площадочный объект от ЛЧ МГ до места аварии (В.22)",
                $"M4 = {mx}*({mn}*EXP(-{T7}/(СТЕПЕНЬ({hL};2)*{eL}))+({mmg}-{mn})*СТЕПЕНЬ({hL};2)*EXP(-{T7}/{eL}))/({mn}+({mmg}-{mn})*СТЕПЕНЬ({hL};2)) = {m4} кг - масса газа, выброшенного на втором этапе истечения (В.21)",
                $"",
                $"M = {m1} + {m2} + {m3} + {m4} = {mo} кг - полная масса газа, выброшенного из первого аварийного  участка (В.23)",
                $"V = {mo}/(1000*{Density.KilogramsPerCubicMeter}) = {v} тыс.м³ - объем газа, выброшенного из первого аварийного участка (В.24)",
            });

            return v;
        }
        


        /// <summary>
        /// Нахождение неизвестного параметра на отрезке при известных значениях входных и выходных данных
        /// </summary>
        /// <param name="pIn">Входной параметр</param>
        /// <param name="pOut">Выходной параметр</param>
        /// <param name="l">Длина отрезка</param>
        /// <param name="x">Расстояние до неизвестной точки с начала отрезка</param>
        /// <param name="listing"></param>
        /// <returns></returns>
        private double ParameterInPoint(double pIn, double pOut, double l, double x, ref string listing)
        {
            var result = Math.Sqrt(Math.Pow(pIn, 2) - (Math.Pow(pIn, 2) - Math.Pow(pOut, 2))*x/l);
            listing = string.Join("\n", new List<string>
            {
                $"КОРЕНЬ(СТЕПЕНЬ({pIn};2)-(СТЕПЕНЬ({pIn};2)-СТЕПЕНЬ({pOut};2))*{x}/{l})",
            });
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tIn"></param>
        /// <param name="tOut"></param>
        /// <param name="listing"></param>
        /// <returns></returns>
        private double PipelineSectionAverageTemperature(double tIn, double tOut, ref string listing)
        {
            var result = (tIn + tOut)/2.0;
            listing = string.Join("\n", new List<string>
            {
                $"({tIn} + {tOut})/2",
            });
            return result;
        }

        /// <summary>
        /// Уравнения для определения числа Маха 
        /// </summary>
        /// <param name="m">число Маха</param>
        /// <param name="k">коэффициент адиабаты</param>
        /// <param name="lambda">Коэффициент гидравлического сопротивления</param>
        /// <param name="x">расстояния от места разрыва до ближайшего линейного крана, м</param>
        /// <param name="d">внутренний диаметр трубы, м</param>
        /// <returns></returns>
        private double MachNumber(double m, double k, double lambda, double x, double d)
        {
            return 1.0/(2*Math.Pow(m, 2.0)) - 1/2.0 -
                   (k + 1)/4.0*Math.Log((1 + (k - 1)/2*Math.Pow(m, 2.0))/((k + 1)/2*Math.Pow(m, 2.0))) -
                   k*lambda*x/(2*d);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="k"></param>
        /// <param name="lambda"></param>
        /// <param name="x"></param>
        /// <param name="d"></param>
        /// <param name="mMin"></param>
        /// <param name="mMax"></param>
        /// <returns></returns>
        private double MachNumberCalculator(double k, double lambda, double x, double d, double mMin, double mMax)
        {
            const double delta = 0.001;
            while (Math.Abs(MachNumber((mMin + mMax) / 2.0, k, lambda, x, d)) > delta)
            {
                var min = MachNumber(mMin, k, lambda, x, d);
                var max = MachNumber(mMax, k, lambda, x, d);
                var aver = MachNumber((mMin + mMax)/2.0, k, lambda, x, d);
                if (min*aver < 0)
                {
                    mMax = (mMin + mMax)/2.0;
                }
                else
                {
                    mMin = (mMin + mMax) / 2.0;
                }
            }

            return (mMin + mMax) / 2.0;
        }


        /// <summary>
        /// Значение момента времени, при котором расход сравнивается со стационарным (G1(t) = Gстац), с
        /// </summary>
        /// <param name="gStac"></param>
        /// <param name="gFunc"></param>
        /// <param name="m"></param>
        /// <param name="hL"></param>
        /// <param name="eL"></param>
        /// <param name="mmg"></param>
        /// <returns></returns>
        private double Time(Func<double, double, double, double,double, double> gFunc, double gStac, double m, double hL, double eL, double mmg)
        {
            var t = 0.0;

            var f = gFunc(m, hL, eL, t, mmg);

            while (gFunc(m, hL, eL, t, mmg) > gStac && t < T7)
            {
                if (t < 1)
                    t += 0.1;
                else t += 1;
            }

            return t < T7 ? t : T7;
        }

        /// <summary>
        /// Расход газа для патрубка до времени выхода на режим квазистационарного истечения Gp(t), кг/с (В.1)
        /// </summary>
        /// <param name="mnp"></param>
        /// <param name="hp"></param>
        /// <param name="ep"></param>
        /// <param name="t"></param>
        /// <param name="mmg"></param>
        /// <returns></returns>
        private double Gp(double mnp, double hp, double ep, double t, double mmg = 0)
        {
            return mnp / (Math.Pow(hp, 2) * ep) * Math.Exp(-t / (Math.Pow(hp, 2) * ep));
        }

        /// <summary>
        /// Расход газа Gmg(t), кг/с, для первого аварийного участка протяженностью L1 до времени выхода на режимквазистационарного истечения (Б.2)
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="hL"></param>
        /// <param name="eL"></param>
        /// <param name="t"></param>
        /// <param name="mmg"></param>
        /// <returns></returns>
        private double Gmg(double mn, double hL, double eL, double t, double mmg)
        {
            return mn / (Math.Pow(hL, 2) * eL) * Math.Exp(-t / (Math.Pow(hL, 2) * eL)) + (mmg - mn) / eL * Math.Exp(-t / eL);
        }

        
        [Display(Name = "Параметры однородных участков трубы")]
        public List<BranchParameter> BranchParameterList { get; set; }
        
        [Display(Name = "Внутренний диаметр основной трубы МГ, мм")]
        public double PipeDiameter { get; set; }

        [Display(Name = "Длина участка МГ, примыкающего к узлу подключения площадного объекта (вход/выход)")]
        public double Length { get; set; } 

        [Display(Name = "Давление газа на выходе из предыдущей КС выше по потоку газа, кг/см²")]
        public Pressure PressureIn { get; set; } 

        [Display(Name = "Давление газа на входе в рассматриваемую КС, кг/см²")]
        public Pressure PressureOut { get; set; }

        [Display(Name = "Температура газа в начале участка до разрыва, Гр.С")]
        public Temperature TemperatureIn { get; set; } = Temperature.FromCelsius(0);

        [Display(Name = "Температура газа на входе в КС, Гр.С")]
        public Temperature TemperatureOut { get; set; } = Temperature.FromCelsius(0);

        [Display(Name = "Плотность газа, кг/м³")]
        public Density Density { get; set; } 

        [Display(Name = "Содержание азота в газе, мол.доля %")]
        public double NitrogenContent { get; set; }

        [Display(Name = "Содержание двуокиси углерода СО2 в газе, мол.доля %")]
        public double CarbonDioxideContent { get; set; }

        [Display(Name = "Давление атмосферное, мм рт.ст.")]
        public Pressure PressureAir { get; set; } 
        
        [Display(Name = "Время прошедшее с момента аварии до момента полного закрытия запорного крана на КС, с")]
        public int T7 { get; set; }
        

    }
    
}