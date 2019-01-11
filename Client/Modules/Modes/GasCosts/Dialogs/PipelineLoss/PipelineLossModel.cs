using System;
using System.ComponentModel.DataAnnotations;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using System.Collections.Generic;
using GazRouter.DTO.ObjectModel.CompShops;
using Utils.Calculations;
using Utils.Units;
using Pressure = Utils.Units.Pressure;

namespace GazRouter.Modes.GasCosts.Dialogs.PipelineLoss
{
    public class PipelineLossModel : Listing, ICostCalcModel
    {
        public double Calculate()
        {
            // А.3.2 Этап I. Расчет массы выброшенного газа из аварийного участка МГ от момента начала аварии до отсечения аварийной секции t1 (для первого аварийного участка)
            var l1 = LengthToCompStation1*1000.0; // расстояние от места разрыва до КС1, м
            var l2 = LengthToCompStation2 * 1000.0; // расстояние от места разрыва до КС2, м
            var l = l1+l2; // длина МГ между КЦ
            var x1 = LengthToLinearValve1 * 1000.0; // расстояние от места разрыва до ближайшего линейного крана до разрыва, м
            var x2 = LengthToLinearValve2*1000.0; // расстояние от места разрыва до ближайшего линейного крана за разрывом, м
        //    var d0 = 1; // внутренний диаметр трубы, м

            var pAir = PressureAir;
            var pInAbs = PressureIn + pAir;
            var pOutAbs = PressureOut + pAir;

            // давление (абс.), температура газа в трубе на момент аварии в месте разрыва МГ 
            var listingPo = string.Empty;
            var listingTo = string.Empty;
            var po = Pressure.FromMpa(ParameterInPoint(pInAbs.Mpa, pOutAbs.Mpa, l, l1, ref listingPo)); //Pressure.FromMpa(Math.Sqrt(Math.Pow(pInAbs.Mpa, 2) - (Math.Pow(pInAbs.Mpa, 2) - Math.Pow(pOutAbs.Mpa, 2))*L1/L));
            var to = Temperature.FromKelvins(298.1); // Temperature.FromKelvins(ParameterInPoint(TemperatureIn.Kelvins, TemperatureOut.Kelvins, L, L1, ref listingTo));

            // давление, температура газа в месте ближайшего линейного крана (абс.) 1го аварийного участка
            var listingP1 = string.Empty;
            var listingT1 = string.Empty;
            var p1 = Pressure.FromMpa(ParameterInPoint(pInAbs.Mpa, pOutAbs.Mpa, l, l1 - x1, ref listingP1)); //(Math.Sqrt(Math.Pow(pInAbs.Mpa, 2) - (Math.Pow(pInAbs.Mpa, 2) - Math.Pow(pOutAbs.Mpa, 2))*(L1 - x1)/L));
            var t1 = Temperature.FromKelvins(ParameterInPoint(TemperatureIn.Kelvins, TemperatureOut.Kelvins, l, l1 - x1, ref listingT1));

            // давление газа в месте ближайшего линейного крана (абс.) 2го аварийного участка
            var listingP2 = string.Empty;
            var listingT2 = string.Empty;
            var p2 = Pressure.FromMpa(ParameterInPoint(pInAbs.Mpa, pOutAbs.Mpa, l, l1 + x2, ref listingP2)); //Math.Sqrt(Math.Pow(pInAbs.Mpa, 2) - (Math.Pow(pInAbs.Mpa, 2) - Math.Pow(pOutAbs.Mpa, 2)) * (L1 + x2) / L));
            var t2 = Temperature.FromKelvins(ParameterInPoint(TemperatureIn.Kelvins, TemperatureOut.Kelvins, l, l1 + x2, ref listingT2));

            // средние давления, температура газа на 1м аварийном участке от разрыва до КЦ, МПа
            var listingP1Avg = string.Empty;
            var listingT1Avg = string.Empty;
            var p1Avg = Pressure.FromMpa(SupportCalculations.PipelineSectionAveragePressure(pInAbs.Mpa, po.Mpa, ref listingP1Avg)); //Pressure.FromMpa(2.0 / 3.0 * (pInAbs.Mpa + Math.Pow(po.Mpa, 2.0) / (pInAbs.Mpa + po.Mpa)));
            var t1Avg = Temperature.FromKelvins(304.8); // Temperature.FromKelvins(PipelineSectionAverageTemperature(TemperatureIn.Kelvins, to.Kelvins, ref listingT1Avg));

            // среднее значение давления газа в аварийной секции 1го участка от разрыва до ближайшего линейного крана, МПа
            var listingP1xAvg = string.Empty;
            var listingT1xAvg = string.Empty;
            var p1xAvg = Pressure.FromMpa(SupportCalculations.PipelineSectionAveragePressure(p1.Mpa, po.Mpa, ref listingP1xAvg)); // Pressure.FromMpa(2.0 / 3.0 * (p1.Mpa + Math.Pow(po.Mpa, 2.0) / (p1.Mpa + po.Mpa)));
            var t1xAvg = Temperature.FromKelvins(298.3); Temperature.FromKelvins(PipelineSectionAverageTemperature(t1.Kelvins, to.Kelvins, ref listingT1xAvg));

            // среднее значение давления газа на 2м аварийном участке от разрыва до КЦ, МПа
            var listingP2Avg = string.Empty;
            var listingT2Avg = string.Empty;
            var p2Avg = Pressure.FromMpa(SupportCalculations.PipelineSectionAveragePressure(po.Mpa, pOutAbs.Mpa, ref listingP2Avg)); // Pressure.FromMpa(2.0 / 3.0 * (po.Mpa + Math.Pow(pOutAbs.Mpa, 2.0) / (pOutAbs.Mpa + po.Mpa)));
            var t2Avg = Temperature.FromKelvins(PipelineSectionAverageTemperature(to.Kelvins, TemperatureOut.Kelvins, ref listingT2Avg));

            // среднее значение давления газа в аварийной секции 2го участка от разрыва до ближайшего линейного крана, МПа
            var listingP2xAvg = string.Empty;
            var listingT2xAvg = string.Empty;
            var p2xAvg = Pressure.FromMpa(SupportCalculations.PipelineSectionAveragePressure(po.Mpa, p2.Mpa, ref listingP2xAvg)); // Pressure.FromMpa(2.0 / 3.0 * (po.Mpa + Math.Pow(p2.Mpa, 2.0) / (p2.Mpa + po.Mpa)));
            var t2xAvg = Temperature.FromKelvins(PipelineSectionAverageTemperature(to.Kelvins, t2.Kelvins, ref listingT2xAvg));
            
            var listingFirstPart = string.Empty;
            var listingSecondPart = string.Empty;
            var v1 = N > 1 && !DoesBridgesClose
                ? CalculateMultilinePipeline(po, to, p1Avg, t1Avg, p1xAvg, t1xAvg, T21, T1, x1, l1, 1,
                    ref listingFirstPart)
                : CalculateSinglePipeline(po, to, p1Avg, t1Avg, p1xAvg, t1xAvg, T21, T1, x1, l1, 1, ref listingFirstPart); // тыс.м3
            var v2 = N > 1 && !DoesBridgesClose
                ? CalculateMultilinePipeline(po, to, p2Avg, t2Avg, p2xAvg, t2xAvg, T19, T2, x2, l2, 2,
                    ref listingSecondPart)
                : CalculateSinglePipeline(po, to, p2Avg, t2Avg, p2xAvg, t2xAvg, T19, T2, x2, l2, 2, ref listingSecondPart); // тыс.м3
            var v = v1 + v2; // тыс.м3

            #region Listing

            ListingCalculation = string.Join("\n", new List<string>
            {
                "Расчет выполнен согласно СТО Газпром 2-1.19-530-2011",
                "Исходные данные:",
                $"Pн = {PressureIn.ToString(PressureUnit.Mpa)} - давление газа на выходе из КС1 выше по потоку газа",
                $"Pк = {PressureOut.ToString(PressureUnit.Mpa)} - давление газа на входе в КС2 ниже по потоку газа",
                $"Tн = {TemperatureIn.ToString(TemperatureUnit.Kelvin)} - температура газа на выходе из КС1 выше по потоку газа",
                $"Tк = {TemperatureOut.ToString(TemperatureUnit.Kelvin)} - температура газа на входе в КС2 ниже по потоку газа",
                $"ρ = {Density.KilogramsPerCubicMeter} кг/м³ - плотность газа",
                $"Q = {Q} млн м³/сут - производительность аварийной нитки газопровода до аварии в норм. режиме его эксплуатации",
                $"L = {l} м - длина участка МГ между КЦ",
                $"L1 = {l1} м - расстояние вдоль аварийной нитки от места разрыва до КС1",
                $"L2 = {l2} м - расстояние вдоль аварийной нитки от места разрыва до КС2",
                $"x1 = {x1} м - расстояние от места разрыва до ближайшего линейного крана для аварийного участка до разрыва",
                $"x2 = {x2} м - расстояние от места разрыва до ближайшего линейного крана для аварийного участка за разрывом",
                $"d1 = {Diameter1} мм - внутр. диаметр трубы до разрыва",
                $"d2 = {Diameter2} мм - внутр. диаметр трубы за разрывом",
                $"t21 = {T21} с - время, прошедшее от момента аварии до момента полного закрытия охранного крана № 21",
                $"t19 = {T19} с - время, прошедшее от момента аварии до момента полного закрытия охранного крана № 19",
                $"t1 = {T1} с - время, прошедшее от момента аварии до момента полного закрытия линейного крана до разрыва",
                $"t2 = {T2} с - время, прошедшее от момента аварии до момента полного закрытия линейного крана за разрывом",
                $"Pатм = {PressureAir.ToString(PressureUnit.MmHg)} - атмосферное давление",
                $"pнАбс = {PressureIn.Mpa} + {pAir.Mpa} = {pInAbs.ToString(PressureUnit.Mpa)} - абс. давление на выходе из КС1 выше по потоку газа",
                $"pкАбс = {PressureOut.Mpa} + {pAir.Mpa} = {pOutAbs.ToString(PressureUnit.Mpa)} - абс. давление на входе в КС2 ниже по потоку газа",
                $"po = {listingPo} = {po.ToString(PressureUnit.Mpa)} - давление газа в трубе на момент аварии в месте разрыва МГ",
                $"to = {listingTo} = {to.ToString(TemperatureUnit.Kelvin)} - температура газа в трубе на момент аварии в месте разрыва МГ",
                $"p1 = {listingP1} = {p1.ToString(PressureUnit.Mpa)} - давление газа в месте ближайшего линейного крана аварийного участка до разрыва",
                $"t1 = {listingT1} = {t1.ToString(TemperatureUnit.Kelvin)} - температура газа в месте ближайшего линейного крана аварийного участка до разрыва",
                $"p2 = {listingP2} = {p2.ToString(PressureUnit.Mpa)} - давление газа в месте ближайшего линейного крана аварийного участка за разрывом",
                $"t2 = {listingT2} = {t2.ToString(TemperatureUnit.Kelvin)} - температура газа в месте ближайшего линейного крана аварийного участка за разрывом",
                $"p1ср = {listingP1Avg} = {p1Avg.ToString(PressureUnit.Mpa)} - ср. давление газа на аварийном участке до разрыва",
                $"t1ср = {listingT1Avg} = {t1Avg.ToString(TemperatureUnit.Kelvin)} - ср. температура газа на аварийном участке до разрыва",
                $"p2ср = {listingP2Avg} = {p2Avg.ToString(PressureUnit.Mpa)} - ср. давление газа на аварийном участке за разрывом",
                $"t2ср = {listingT2Avg} = {t2Avg.ToString(TemperatureUnit.Kelvin)} - ср. температура газа на аварийном участке за разрывом",
                $"p1xср = {listingP1xAvg} = {p1xAvg.ToString(PressureUnit.Mpa)} - ср. давление газа в аварийной секции аварийного участка до разрыва",
                $"t1xср = {listingT1xAvg} = {t1xAvg.ToString(TemperatureUnit.Kelvin)} - ср. температура газа в аварийной секции аварийного участка до разрыва",
                $"p2xср = {listingP2xAvg} = {p2xAvg.ToString(PressureUnit.Mpa)} - ср. давление газа в аварийной секции аварийного участка за разрывом",
                $"t2xср = {listingT2xAvg} = {t2xAvg.ToString(TemperatureUnit.Kelvin)} - ср. температура газа в аварийной секции аварийного участка за разрывом",
                "",
                "Расчет.",
                $"{listingFirstPart}",
                "",
                $"{listingSecondPart}",
                "",
                $"V = {v1} + {v2} = {v} тыс.м³ - суммарный объем газа, выброшенный из первого и второго аварийных участков",
            });

            #endregion

            //TODO: а где расчет?
            return v;
        }

        /// <summary>
        ///     Расчет однониточного газопровода
        /// </summary>
        /// <param name="po">Абс. давление газа в трубе на момент аварии в месте разрыва МГ</param>
        /// <param name="to">Температура газа в трубе на момент аварии в месте разрыва МГ</param>
        /// <param name="pAvg">Среднее давление газа на авар. участке</param>
        /// <param name="tAvg">Средняя температура на авар. участке</param>
        /// <param name="pxAvg">Среднее давление газа в авар. секции</param>
        /// <param name="txAvg">Средняя температура в авар. секции</param>
        /// <param name="tki">Время, прошедшее от момента аварии до момента полного закрытия охр. крана</param>
        /// <param name="ti">Время, прошедшее от момента аварии до момента полного закрытия лин. крана</param>
        /// <param name="xi">Расстояние от места разрыва до ближайшего линейного крана, м</param>
        /// <param name="li">Расстояние от места разрыва до КС, м</param>
        /// <param name="i">Номер рассчитываемого аварийного участка (1 - участок выше по потоку, 2 - участок ниже по потоку)</param>
        /// <param name="listing">Листинг</param>
        /// <returns> Полный объем газа, выброшенного при аварии, тыс. м3 </returns>
        private double CalculateSinglePipeline(Pressure po, Temperature to, Pressure pAvg, Temperature tAvg, Pressure pxAvg,
            Temperature txAvg, double tki, double ti, double xi, double li, int i, ref string listing)
        {
            // показатель адиабаты
            var k = SupportCalculations.AdiabaticIndex(pAvg, tAvg, Density, NitrogenContent / 100);

            // коэффициент сжимаемости до разрыва при параметрах Po и To
            var zo = SupportCalculations.GasCompressibilityFactorApproximate(po, to, Density);

            // коэффициент сжимаемости до разрыва при параметрах P1sr и T1sr
            var z1Avg = SupportCalculations.GasCompressibilityFactorApproximate(pAvg, tAvg, Density);

            // коэффициент сжимаемости до разрыва при параметрах P1xsr и T1xsr
            var z1xAvg = SupportCalculations.GasCompressibilityFactorApproximate(pxAvg, txAvg, Density);

            // газовая постоянная
            var r = SupportCalculations.SpecificGasConstant(NitrogenContent / 100, CarbonDioxideContent / 100);

            var d0 = i == 1 ? 0.001 * Diameter1 : 0.001 * Diameter2;
            // коэффициент гидравлического сопротивления при квадратичном режиме, когда k=0.03мм
            var lambda = 0.03817 / Math.Pow(d0 * 1000.0, 0.2);

            // производительность газпровода, кг/с (в нормальном режиме эксплуатации)
            var g = Density.KilogramsPerCubicMeter * Q * Math.Pow(10, 6) / 86400.0;

            // скорость звука в газе до разрыва, м/с
            var ao = Math.Sqrt(k * r * z1Avg * tAvg.Kelvins);

            // постоянная времени, с
            var eL = 2.0 / 3.0 * li / ao * Math.Sqrt(k * lambda * li / d0);

            // начальный критический массовый расход газа, кг/с (А.9)
            var g0 = po.Mpa * Math.Pow(10.0, 6) * Math.PI * Math.Pow(d0, 2) * Math.Sqrt(k) / (4 * Math.Sqrt(r * zo * to.Kelvins)) *
                     Math.Pow(2 / (k + 1), (k + 1) / (2 * (k - 1)));
            // масса газа, которая нагнетается в аварийный участок до момента отсечения аварийного участка, кг
            var time = ti < tki && ti > 0 ? ti : tki;
            var mks1 = g * time;

            // масса газа в первом аварийном участке газопровода до аварии, кг 
            var sign = i == 1 ? "+" : "-";
            var mg1 = i == 1
                ? li * Math.PI * Math.Pow(d0, 2) * pAvg.Mpa * Math.Pow(10, 6.0) / (4.0 * r * z1Avg * tAvg.Kelvins) + mks1
                : li * Math.PI * Math.Pow(d0, 2) * pAvg.Mpa * Math.Pow(10, 6.0) / (4.0 * r * z1Avg * tAvg.Kelvins) - mks1;

            // коэфициент (А.8)
            var hL = 2 * mg1 / (eL * g0);

            // Масса газа, выброшенного на этапе адиабатического расширения в начальной стадии истечения при аварийном разрыве из первого аварийного участка газопровода, кг (А.5)
            var mn = 2 * mg1 * d0 / (lambda * li * Math.Sqrt(k)) *
                     (Math.Sqrt(1 / k * Math.Pow((k + 1) / 2.0, (k + 1) / (k - 1)) + lambda * li / d0) -
                      Math.Sqrt(1 / k * Math.Pow((k + 1) / 2.0, (k + 1) / (k - 1))));

            // масса газа, выброшенного из первого аварийного участка на первом этапе истечения аварийной секции t1 (А.1)
            var m11 = mn * (1 - Math.Exp(-tki / (Math.Pow(hL, 2) * eL))) + (mg1 - mn) * (1 - Math.Exp(-tki / eL));

            // А.3.2 Этап II. Расчет массы выброшенного газа из аварийной секции первого аварийного участка МГ после ее отсечения через время t1
            // Масса газа, находящегося в первой аварийной секции газопровода до аварии, кг (А.12)

            var hasLinearValve = i == 1 ? !NoHasLinearValve1 : !NoHasLinearValve2;
            var mx = hasLinearValve
                ? xi * Math.PI * Math.Pow(d0, 2.0) * pxAvg.Mpa * Math.Pow(10.0, 6.0) / (4.0 * r * z1xAvg * txAvg.Kelvins)
                : 0;
            // Масса газа из первой аварийной секции M12 на втором этапе истечения, кг (А.11)
            var m12 = hasLinearValve
                ? mx * (mn * Math.Exp(-ti / (Math.Pow(hL, 2.0) * eL)) + (mg1 - mn) * Math.Pow(hL, 2.0) * Math.Exp(-ti / eL)) /
                  (mn + (mg1 - mn) * Math.Pow(hL, 2.0))
                : 0;

            var listing2 = hasLinearValve ? string.Join("\n", new List<string>
            {
                $"M{i}2 = {mx}*({mn}*EXP(-{ti}/(({hL}^2)*{eL}))+({mg1}-{mn})*({hL}^2)*EXP(-{ti}/{eL}))/({mn}+({mg1}-{mn})*({hL}^2)) = {m12} кг",
                $"Mx = {xi}*ПИ()*({d0}^2)*{pxAvg.Mpa*Math.Pow(10.0, 6)}/(4*{r}*{z1xAvg}*{txAvg.Kelvins}) = {mx} кг - масса газа, находящегося в первой аварийной секции газопровода до аварии",
            }) : string.Join("\n", new List<string> { $"Расчет выброса ведется только по формулам первого этапа, т.к. лин. кран на аварийном участке отсутствует или не может быть закрыт" });

            // Полная масса газа, выброшенного из первого аварийного участка (А.13)
            var m1 = m11 + m12;
            var v1 = m1 / (1000.0 * Density.KilogramsPerCubicMeter);

            #region listing

            listing = string.Join("\n", new List<string>
            {
                $"{i}. Расчет {i}го аварийного участка:",
                $"M{i} = {m11}+{m12} = {m1} кг - полная выброшенная масса из аварийного участка (A.13)",
                $"V{i} = {m1}/(1000*{Density.KilogramsPerCubicMeter}) = {v1} тыс.м³",
                "a.",
                $"Этап I. Расчет массы выброшенного газа из аварийного участка МГ от момента начала аварии до отсечения аварийной секции t{i}:",
                "Масса газа, выброшенного из первого аварийного участка на первом этапе истечения (А.1):",
                $"M{i}1 = {mn}*(1-EXP(-{tki}/(({hL}^2)*{eL})))+({mg1}-{mn})*(1-EXP(-{tki}/{eL})) = {m11} кг",
                "Масса газа, выброшенного на этапе адиабатич. расширения в начальн. стадии истечения при аварийном разрыве из аварийного участка (А.5):",
                $"Mn = 2*{mg1}*{d0}/({lambda}*{li}*КОРЕНЬ({k}))",
                $"             *(КОРЕНЬ(1/{k}*СТЕПЕНЬ(({k}+1)/2;({k}+1)/({k}-1))+{lambda}*{li}/{d0})-КОРЕНЬ(1/{k}*СТЕПЕНЬ(({k}+1)/2;({k}+1)/({k}-1)))) = {mn} кг",
                $"hL = 2*{mg1}/({eL}*{g0}) = {hL} - коэфициент (А.8)",
                "Масса газа в первом аварийном участке газопровода до аварии (А.2):",
                $"Mg{i} = {li}*ПИ()*({d0}^2)*{pAvg.Mpa*Math.Pow(10.0, 6)}/(4*{r}*{z1Avg}*{tAvg.Kelvins}) {sign} {mks1} = {mg1} кг",
                $"Mks{i} = {g}*{time} = {mks1} кг - масса газа, нагнетаемого в аварийный участок до момента отсечения аварийного участка (А.3)",
                "Начальный критический массовый расход газа (А.9):",
                $"G0 = {po.Mpa*Math.Pow(10.0, 6)}*ПИ()*({d0}^2)*КОРЕНЬ({k})/(4*КОРЕНЬ({r}*{zo}*{to.Kelvins}))*СТЕПЕНЬ(2/({k}+1);({k}+1)/(2*({k}-1))) = {g0} кг/с",
                $"eL = 2/3*{li}/{ao}*КОРЕНЬ({k}*{lambda}*{li}/{d0}) = {eL} с - постоянная времени (А.6)",
                $"ao = КОРЕНЬ({k}*{r}*{z1Avg}*{tAvg.Kelvins}) = {ao} м/с - скорость звука в газе до разрыва (А.7)",
                $"G = {Density.KilogramsPerCubicMeter}*{Q}*(10^6)/86400 = {g} кг/с - производительность газопровода в нормальном режиме эксплуатации (А.4)",
                "b.",
                $"Этап II.Расчет массы выброшенного газа из аварийной секции {i}го аварийного участка МГ после ее отсечения через время t{i}",
                "Масса газа из аварийной секции на втором этапе истечения (A.11):",
                $"{listing2}",
            });

            #endregion listing

            return v1;
        }

        /// <summary>
        ///     Расчет многониточного газопровода при открытых перемычках между никтами
        /// </summary>
        /// <param name="po">Абс. давление газа в трубе на момент аварии в месте разрыва МГ</param>
        /// <param name="to">Температура газа в трубе на момент аварии в месте разрыва МГ</param>
        /// <param name="pAvg">Среднее давление газа на авар. участке</param>
        /// <param name="tAvg">Средняя температура на авар. участке</param>
        /// <param name="pxAvg">Среднее давление газа в авар. секции</param>
        /// <param name="txAvg">Средняя температура в авар. секции</param>
        /// <param name="tki">Время, прошедшее от момента аварии до момента полного закрытия охр. крана</param>
        /// <param name="ti">Время, прошедшее от момента аварии до момента полного закрытия лин. крана</param>
        /// <param name="xi">Расстояние от места разрыва до ближайшего линейного крана, м</param>
        /// <param name="li">Расстояние от места разрыва до КС, м</param>
        /// <param name="i">Номер рассчитываемого аварийного участка (1 - участок выше по потоку, 2 - участок ниже по потоку)</param>
        /// <param name="listing"></param>
        /// <returns> Полный объем газа, выброшенного при аварии, тыс. м3 </returns>
        private double CalculateMultilinePipeline(Pressure po, Temperature to, Pressure pAvg, Temperature tAvg,
            Pressure pxAvg, Temperature txAvg, double tki, double ti, double xi, double li, int i,
            ref string listing)
        {
            // показатель адиабаты
            var k = SupportCalculations.AdiabaticIndex(pAvg, tAvg, Density, NitrogenContent / 100);

            // коэффициент сжимаемости до разрыва при параметрах Po и To
            var zo = SupportCalculations.GasCompressibilityFactorApproximate(po, to, Density);

            var d0 = i == 1 ? 0.001 * Diameter1 : 0.001 * Diameter2;

            // коэффициент гидравлического сопротивления при квадратичном режиме, когда k=0.03мм
            var lambda = 0.03817 / Math.Pow(d0 * 1000.0, 0.2);

            // коэффициент сжимаемости до разрыва при параметрах Pisr и Tisr
            var zAvg = SupportCalculations.GasCompressibilityFactorApproximate(pAvg, tAvg, Density);

            // коэффициент сжимаемости до разрыва при параметрах P1xsr и T1xsr
            var z1xAvg = SupportCalculations.GasCompressibilityFactorApproximate(pxAvg, txAvg, Density);

            // газовая постоянная
            var r = SupportCalculations.SpecificGasConstant(NitrogenContent / 100, CarbonDioxideContent / 100);

            // производительность газпровода, кг/с (в нормальном режиме эксплуатации) (Б.5)
            var g = Density.KilogramsPerCubicMeter * Q * Math.Pow(10, 6) / 86400.0;

            // скорость звука в газе до разрыва, м/с (Б.8)
            var ao = Math.Sqrt(k * r * zAvg * tAvg.Kelvins);

            // постоянная времени, с (Б.7)
            var eL = 2.0 / 3.0 * li / ao * Math.Sqrt(k * lambda * li / d0);

            // Масса газа, которая нагнетается в аварийный участок газопровода КС1 до момента отсечения аварийного участка, кг (Б.4)
            var time = ti < tki && ti > 0 ? ti : tki;
            var mks1 = g * time;

            // Масса газа, находящаяся в аварийной нитке iго аварийного участка газопровода до аварии, кг (Б.3)
            var sign = i == 1 ? "+" : "-";
            var mg1 = i == 1
                ? li * Math.PI * Math.Pow(d0, 2.0) * pAvg.Mpa * Math.Pow(10, 6.0) / (4.0 * r * zAvg * tAvg.Kelvins) + mks1
                : li * Math.PI * Math.Pow(d0, 2.0) * pAvg.Mpa * Math.Pow(10, 6.0) / (4.0 * r * zAvg * tAvg.Kelvins) - mks1;

            // начальный критический массовый расход газа, кг/с (Б.10)
            var g0 = po.Mpa * Math.Pow(10.0, 6) * Math.PI * Math.Pow(d0, 2) * Math.Sqrt(k) / (4 * Math.Sqrt(r * zo * to.Kelvins)) *
                     Math.Pow(2 / (k + 1), (k + 1) / (2 * (k - 1)));

            // коэфициент (Б.9)
            var hL = 2 * mg1 / (eL * g0);

            // Величина Mn, кг (Б.6)
            var mn = 2 * mg1 * d0 / (lambda * li * Math.Sqrt(k)) *
                     (Math.Sqrt(1 / k * Math.Pow((k + 1) / 2.0, (k + 1) / (k - 1)) + lambda * li / d0) -
                      Math.Sqrt(1 / k * Math.Pow((k + 1) / 2.0, (k + 1) / (k - 1))));

            // Скорость газового потока в районе перемычек между нитками, необходимая для существования стационарного режима истечения (Б.13)
            var m = MachNumberCalculator(k, lambda, xi, d0, 0.01, 1);
            var vp = m * ao;

            // (Б.12)
            var gStac = po.Mpa * Math.Pow(10.0, 6) * Math.PI * Math.Pow(d0, 2) / (4 * r * zAvg * to.Kelvins) * vp;

            // Расход газа G1(t), кг/с, для первого аварийного участка протяженностью L1 до времени выхода на режим квазистационарного истечения (Б.2)
            var tStac = Time(gStac, mn, hL, eL, mg1, 1, ti);
            var g1 = G1(mn, hL, eL, mg1, tStac);
            // var G1 = Mn/(Math.Pow(hL, 2)*eL)*Math.Exp(-t/(Math.Pow(hL, 2)*eL)) + (Mg1 - Mn)/eL*Math.Exp(-t/eL);

            // Масса газа, выброшенного из первого аварийного участка на первом этапе истечения за время переходного процесса (Б.15)
            var m11 = mn * (1 - Math.Exp(-tStac / (Math.Pow(hL, 2.0) * eL))) + (mg1 - mn) * (1 - Math.Exp(-tStac / eL));

            var tOkonch = Time(gStac, mn, hL, eL, mg1, N, ti);
            var nG1 = N * (G1(mn, hL, eL, mg1, tOkonch));

            // Полная масса газа, выброшенного на этапе стационарного истечения (Б.16)
            var m12 = gStac * (tOkonch - tStac);

            // Расчет истечения газа после окончания режима стационарного истечения до времени закрытия линейного крана (Б.17)
            var m13 = N *
                      (mn * (Math.Exp(-tOkonch / (Math.Pow(hL, 2.0) * eL)) - Math.Exp(-ti / (Math.Pow(hL, 2.0) * eL))) +
                       (mg1 - mn) * (Math.Exp(-tOkonch / eL) - Math.Exp(-ti / eL)));

            var hasLinearValve = i == 1 ? !NoHasLinearValve1 : !NoHasLinearValve2;
            // Масса газа, находящегося в iой аварийной секции газопровода до аварии, кг (Б.19)
            var mx = hasLinearValve ? xi * Math.PI * Math.Pow(d0, 2.0) * pxAvg.Mpa * Math.Pow(10, 6.0) / (4.0 * r * z1xAvg * txAvg.Kelvins) : 0;

            // Расчет массового расхода газа из аварийного газопровода после локализации аварии и закрытия линейных кранов (Б.18)
            var m14 = hasLinearValve ? mx * (mn * Math.Exp(-ti / (Math.Pow(hL, 2.0) * eL)) + (mg1 - mn) * Math.Pow(hL, 2.0) * Math.Exp(-ti / eL)) /
                      (mn + (mg1 - mn) * Math.Pow(hL, 2.0)) : 0;

            var mi = m11 + m12 + m13 + m14;
            var vi = mi / (1000.0 * Density.KilogramsPerCubicMeter);
            var listing2 = hasLinearValve ? string.Join("\n", new List<string>
            {
                $"M{i}4 = {mx}*({mn}*EXP(-{ti}/(СТЕПЕНЬ({hL};2)*{eL}))+({mg1}-{mn})*СТЕПЕНЬ({hL};2)*EXP(-{ti}/{eL}))/",
                $"              ({mn}+({mg1}-{mn})*СТЕПЕНЬ({hL};2)) = {m14} кг - масса газа из {i}ой аварийной секции на втором этапе истечения (Б.18)",
                $"Mx = {xi}*ПИ()*СТЕПЕНЬ({d0};2)*{pxAvg.Mpa*Math.Pow(10, 6.0)}/(4*{r}*{z1xAvg}*{txAvg.Kelvins}) = {mx} кг - масса газа, находящегося в {i}ой аварийной секции газопровода до аварии (Б.19)",
            }) : string.Join("\n", new List<string> { $"Расчет выброса ведется только по формулам первого этапа, т.к. лин. кран на аварийном участке отсутствует или не может быть закрыт" });

            #region listing

            listing = string.Join("\n", new List<string>
            {
                $"{i}. Расчет {i}го аварийного участка:",
                $"M{i} = {m11}+{m12}+{m13}+{m14} = {mi} кг - полная выброшенная масса из аварийного участка (Б.20)",
                $"V{i} = {mi}/(1000*{Density.KilogramsPerCubicMeter}) = {vi} тыс.м³",
                "Этап I. Расчет массового расхода газа из аварийного газопровода от момента аварии до отсечения аварийной секции",
                "a. Расчет параметров истечения для переходного процесса",
                $"Mг{i} = {li}*ПИ()*СТЕПЕНЬ({d0};2)*{pAvg.Mpa*Math.Pow(10, 6.0)}/(4*{r}*{zAvg}*{tAvg.Kelvins}){sign}{mks1} = {mg1} кг -",
                $"                             масса газа, находящаяся в аварийной нитке {i}го аварийного участка газопровода до аварии (Б.13)",
                $"Mкс{i} = {g}*{time} = {mks1} кг - масса газа, которая нагнетается в аварийный участок газопровода КС1 до момента отсечения аварийного участка (Б.4)",
                $"Gкс = {Density.KilogramsPerCubicMeter}*{Q}*СТЕПЕНЬ(10;6)/86400 = {g} кг/с - производительность газопровода (в нормальном режиме эксплуатации) (Б.5)",
                $"Mн = 2*{mg1}*{d0}/({lambda}*{li}*КОРЕНЬ({k}))*",
                $"                           (КОРЕНЬ(1/{k}*СТЕПЕНЬ(({k}+1)/2;({k}+1)/({k}-1))+{lambda}*{li}/{d0})-КОРЕНЬ(1/{k}*СТЕПЕНЬ(({k}+1)/2;({k}+1)/({k}-1)))) = {mn} кг -",
                $"                            масса газа, выброшенного на этапе адиаб. расширения в начальн. стадии истечения при аварийном разрыве из {i}го авар. участка (Б.6)",
                $"εL = 2/3*{li}/{ao}*КОРЕНЬ({k}*{lambda}*{li}/{d0}) = {eL} - постоянная времени (Б.7)",
                $"aₒ = КОРЕНЬ({k}*{r}*{zAvg}*{tAvg.Kelvins} = {ao} м/с - скорость звука в газе до разрыва (Б.8)",
                $"ηL = 2*{mg1}/({eL}*{g0}) = {hL} (Б.9)",
                $"Gₒ = {po.Mpa*Math.Pow(10.0,6)}*ПИ()*СТЕПЕНЬ({d0};2)*КОРЕНЬ({k})/(4*КОРЕНЬ({r}*{zo}*{to.Kelvins}))*СТЕПЕНЬ(2/({k}+1);({k}+1)/(2*({k}-1))) = {g0} кг/с - ",
                "                                нач. критический массовый расход газа, кг/с (Б.10)",
                $"Gстац = {po.Mpa*Math.Pow(10.0,6)}*ПИ()*СТЕПЕНЬ({d0};2)/(4*{r}*{zAvg}*{to.Kelvins})*{vp} = {gStac} кг/с - расход при достижении стационарного режима истечения (Б.12)",
                $"νп = {m}*{ao} = {vp} м/с - скорость газового потока в районе перемычек между нитками, необходимая для существования стационарного режима истечения (Б.13)",
                $"М = {m} - число Маха в рассматриваемой точке газового потока (Б.14)",
                $"tстац = {tStac} с - значение момента времени, при котором расход становится равным стационарному",
                $"G{i}(tстац) = {mn}/(СТЕПЕНЬ({hL};2)*{eL})*EXP(-{tStac}/(СТЕПЕНЬ({hL};2)*{eL}))+({mg1}-{mn})/{eL}*EXP(-{tStac}/{eL}) = {g1} кг/с - аварийно массовый расход (Б.2)",
                $"M{i}1 = {mn}*(1-EXP(-{tStac}/(СТЕПЕНЬ({hL};2)*{eL})))+({mg1}-{mn})*(1-EXP(-{tStac}/{eL})) = {m11} кг - ",
                $"                                  масса газа, выброшенного из {i}го авар. участка на 1м этапе истечения за время переходного процесса",
                "b. Расчет истечения газа после достижения режима стационарного истечения",
                $"M{i}2 = {gStac}*({tOkonch}-{tStac}) = {m12} кг - масса газа, выброшенного на этапе стационарного истечения (Б.16)",
                $"tоконч = {tOkonch} c - время окончания стационарного режима истечения",
                $"Gmax = N·G{i}(tоконч) = {N}*{mn}/(СТЕПЕНЬ({hL};2)*{eL})*EXP(-{tOkonch}/(СТЕПЕНЬ({hL};2)*{eL}))+({mg1}-{mn})/{eL}*EXP(-{tOkonch}/{eL}) = {nG1} кг/с - ",
                "                            максимально возможный суммарный для всех ниток расход (Б.2)",
                "c. Расчет истечения газа после окончания режима стационарного истечения до времени закрытия линейного крана",
                $"M{i}3 = {N}*({mn}*(EXP(-{tOkonch}/(СТЕПЕНЬ({hL};2)*{eL}))-EXP(-{ti}/(СТЕПЕНЬ({hL};2)*{eL})))+({mg1}-{mn})*(EXP(-{tOkonch}/{eL})-EXP(-{ti}/{eL}))) = {m13} кг - ",
                "                            масса газа, выброшенная на этом этапе (Б.17)",
                "Этап II. Расчет массового расхода газа из аварийного газопровода полсе локализации аварии и закрытия линейных кранов",
                $"{listing2}",
            });

            #endregion

            return vi;
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
        /// Расход газа G1(t), кг/с, для первого аварийного участка протяженностью L1 до времени выхода на режимквазистационарного истечения (Б.2)
        /// </summary>
        /// <param name="mn"></param>
        /// <param name="hL"></param>
        /// <param name="eL"></param>
        /// <param name="mg1"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        private double G1(double mn, double hL, double eL, double mg1, double t)
        {
            return mn / (Math.Pow(hL, 2) * eL) * Math.Exp(-t / (Math.Pow(hL, 2) * eL)) + (mg1 - mn) / eL * Math.Exp(-t / eL);
        }

        /// <summary>
        /// Значение момента времени, при котором расход сравнивается со стационарным (G1(t) = Gстац), с
        /// </summary>
        /// <param name="gStac"></param>
        /// <param name="mn"></param>
        /// <param name="hL"></param>
        /// <param name="eL"></param>
        /// <param name="mg1"></param>
        /// <param name="count"></param>
        /// <param name="ti"></param>
        /// <returns></returns>
        private double Time(double gStac, double mn, double hL, double eL, double mg1, int count, double ti)
        {
            var t = 0.0;

            while (count * G1(mn, hL, eL, mg1, t) > gStac && t < ti)
            {
                if (t < 1)
                    t += 0.1;
                else t += 1;
            }
            
            return t < ti ? t : ti;
        }

        

        [Display(Name = "КЦ в начале участка")]
        public Guid? CompShopFromId { get; set; }

        [Display(Name = "КЦ в конце участка")]
        public Guid? CompShopToId { get; set; }

        [Display(Name = "Километр разрыва газопровода")]
        public double KmBreaking { get; set; }

        [Display(Name = "Километр установки ближайшего лин. крана 21")]
        public double KmOfLinearValve1 { get; set; }

        [Display(Name = "Километр установки ближайшего лин. крана 19")]
        public double KmOfLinearValve2 { get; set; }

        [Display(Name = "Километр подключения КС1")]
        public double KmOfCompStation1 { get; set; }

        [Display(Name = "Километр подключения КС2")]
        public double KmOfCompStation2 { get; set; }

        [Display(Name = "Расстояние до ближайшего лин. крана 21, км")]
        public double LengthToLinearValve1 { get; set; }

        [Display(Name = "Расстояние до ближайшего лин. крана 19, км")]
        public double LengthToLinearValve2 { get; set; }

        [Display(Name = "Расстояние до КС1, км")]
        public double LengthToCompStation1 { get; set; } 

        [Display(Name = "Расстояние до КС2, км")]
        public double LengthToCompStation2 { get; set; } 

        [Display(Name = "Давление газа в начале участка до разрыва, кг/см²")]
        public Pressure PressureIn { get; set; }

        [Display(Name = "Давление газа в конце участка до разрыва, кг/см²")]
        public Pressure PressureOut { get; set; } 

        [Display(Name = "Температура газа в начале участка до разрыва, Гр.С")]
        public Temperature TemperatureIn { get; set; } = Temperature.FromCelsius(0);

        [Display(Name = "Температура газа в конце участка до разрыва, Гр.С")]
        public Temperature TemperatureOut { get; set; } = Temperature.FromCelsius(0);

        [Display(Name = "Плотность газа, кг/м³")]
        public Density Density { get; set; } 

        [Display(Name = "Содержание азота в газе, мол.доля %")]
        public double NitrogenContent { get; set; }

        [Display(Name = "Содержание двуокиси углерода СО2 в газе, мол.доля %")]
        public double CarbonDioxideContent { get; set; }

        [Display(Name = "Давление атмосферное, мм рт.ст.")]
        public Pressure PressureAir { get; set; }

        [Display(Name = "Производительность газопровода в нормальном режиме, млн.м3/сут.")]
        public double Q { get; set; }

        [Display(Name = "Время прошедшее с момента аварии до момента полного закрытия линейного крана 1, с")]
        public int T1 { get; set; } 

        [Display(Name = "Время прошедшее с момента аварии до момента полного закрытия линейного крана 2, с")]
        public int T2 { get; set; } 

        [Display(Name = "Время прошедшее с момента аварии до момента полного закрытия охранного крана №21, с")]
        public int T21 { get; set; } 

        [Display(Name = "Время прошедшее с момента аварии до момента полного закрытия охранного крана №19, с")]
        public int T19 { get; set; } 

        [Display(Name = "Количество параллельно включенных ниток МГ")]
        public int N { get; set; }

        [Display(Name = "Признак закрытия перемычек между нитками")]
        public bool DoesBridgesClose { get; set; }

        [Display(Name = "Признак отсутствия лин. кран до разрыва")]
        public bool NoHasLinearValve1 { get; set; }

        [Display(Name = "Признак отсутствия лин. кран за разрывом")]
        public bool NoHasLinearValve2 { get; set; }

        [Display(Name = "Внутренний диаметр трубы до разрыва, мм")]
        public double Diameter1 { get; set; }

        [Display(Name = "Внутренний диаметр трубы за разрывом, мм")]
        public double Diameter2 { get; set; } 

    }
    
}