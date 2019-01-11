using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GazRouter.DTO.Dictionaries.PropertyTypes;
using GazRouter.Modes.GasCosts.GasCompessibility;
using System.Linq;
using GazRouter.Controls.Dialogs.PipingVolumeCalculator;
using GazRouter.Modes.GasCosts.Dialogs.Model;
using Utils.Calculations;
using Utils.Units;
using Pressure = Utils.Units.Pressure;
namespace GazRouter.Modes.GasCosts.Dialogs.RepairCosts
{
    public class RepairCostsModel : Listing, ICostCalcModel
    {
        public RepairCostsModel()
        {
            PressureFinalIn = Pressure.Zero;
            PressureFinalOut = Pressure.Zero;
            PurgeCount = 1;
            Piping = new List<PipeSection>();
        }
        public double Calculate()
        {
            var listingPi = string.Empty;
            //
            var pInitialIn = PressureInitialIn;// + pAir;
            var pInitialOut = PressureInitialOut;// + pAir;
            var pFinalIn = PressureFinalIn + PressureAir;
            var pFinalOut = PressureFinalOut + PressureAir;
            var tInitialIn = TemperatureInitialIn;
            var tInitialOut = TemperatureInitialOut;
            //
            if (HasRecovery)
            {
                pInitialIn = PressureRecoveryIn;// + pAir;
                pInitialOut = PressureRecoveryOut;// + pAir;
                tInitialIn = TemperatureRecoveryIn;
                tInitialOut = TemperatureRecoveryOut;
                //var pRecoveryIn = PressureRecoveryIn + pAir;
                //var pRecoveryOut = PressureRecoveryOut + pAir;
                // Расчет расхода газа на выработку традиционными способами
                //pI = Pressure.FromMpa(SupportCalculations.PipelineSectionAveragePressure(pRecoveryIn.Mpa, pRecoveryOut.Mpa, ref listingPi)); 
                //tI = (TemperatureRecoveryIn + TemperatureRecoveryOut) / 2;
            }
            if (HasMobilePumpRecovery)
            {
                pInitialIn = PressureMobilePumpRecoveryIn;// + pAir;
                pInitialOut = PressureMobilePumpRecoveryOut;// + pAir;
                tInitialIn = TemperatureMobilePumpRecoveryIn;
                tInitialOut = TemperatureMobilePumpRecoveryOut;
                //var pMobilePumpRecoveryIn = PressureMobilePumpRecoveryIn + pAir;
                //var pMobilePumpRecoveryOut = PressureMobilePumpRecoveryOut + pAir;
                //// Расчет расхода газа на выработку мобильной КС
                //pI = Pressure.FromMpa(SupportCalculations.PipelineSectionAveragePressure(pMobilePumpRecoveryIn.Mpa, pMobilePumpRecoveryOut.Mpa, ref listingPi)); 
                //tI = (TemperatureMobilePumpRecoveryIn + TemperatureMobilePumpRecoveryOut) / 2;
            }
            
            var pInitialInAbs = pInitialIn + PressureAir;
            var pInitialOutAbs = pInitialOut + PressureAir;
            var pI = Pressure.FromMpa(SupportCalculations.PipelineSectionAveragePressure(pInitialInAbs.Mpa, pInitialOutAbs.Mpa, ref listingPi));
            var tI = (tInitialIn + tInitialOut) / 2;
            var gasContent = new List<ComponentContent>
            {
                new ComponentContent
                {
                    Component = PropertyType.ContentMethane,
                    Concentration = 1 - (NitrogenContent/100.0 + CarbonDioxideContent/100.0)
                },
                new ComponentContent
                {
                    Component = PropertyType.ContentNitrogen,
                    Concentration = NitrogenContent/100.0
                },
                new ComponentContent
                {
                    Component = PropertyType.ContentCarbonDioxid,
                    Concentration = CarbonDioxideContent/100.0
                },
            };
            var zI = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, pI.Mpa, tI.Kelvins);

            // Расчет стравливаемого газа
            var listingPf = string.Empty;
            var pF =
                Pressure.FromMpa(SupportCalculations.PipelineSectionAveragePressure(pFinalIn.Mpa, pFinalOut.Mpa, ref listingPf)); 
            var tF = (TemperatureFinalIn + TemperatureFinalOut) / 2;
            var zF = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, pF.Mpa, tF.Kelvins);
            // Расчет расхода газа при полном стравливании            
            double result;
            if ((PressureFinalIn == Pressure.Zero) && (PressureFinalOut == Pressure.Zero)) // полное стравливание
            {
                var qPr = 0.001 * 3 * Volume * PurgeCount;
                var qOp = 0.001*Volume*pI.Mpa/tI.Kelvins/zI*StandardConditions.T.Kelvins/StandardConditions.P.Mpa;
                result = qOp + qPr;
                ListingCalculation = string.Join("\n", new List<string>
                {
                    $"Qрем(стр) = {qOp} + {qPr} = {result} тыс.м³ - расход газа при проведении ремонтов на участке МГ с опорожнением газа из участка МГ",
                    $"",
                    $"Qоп = 0,001 * {Volume} * {pI.Mpa} / {tI.Kelvins} / {zI} * {StandardConditions.T.Kelvins} / {StandardConditions.P.Mpa} = {qOp} тыс.м³ - объем газа, стравливаемого при опорожнении участка МГ",
                    $"Qпр = 0,001 * 3 * {Volume} * {PurgeCount} = {qPr} тыс.м³ - объем газа, расходуемого на удаление газовоздушной смеси из участка МГ после проведения огневых работ",
                    $"Vуч = {Volume} м³ - геометрический объем участка газопровода",
                    $"P1ср = {listingPi} МПа - среднее давление газа на участке перед стравливанием",
                    $"P1н = {pInitialIn.Mpa} + {PressureAir.Mpa} = {pInitialInAbs.ToString(PressureUnit.Mpa)} - абсолютное давление газа в начале участка перед стравливанием",
                    $"P1к = {pInitialOut.Mpa} + {PressureAir.Mpa} = {pInitialOutAbs.ToString(PressureUnit.Mpa)} - абсолютное давление газа в конце участка перед стравливанием",
                    $"PAir = {PressureAir.Mpa} - атмосферное давление",
                    $"T1ср = ({tInitialIn.Kelvins} + {tInitialOut.Kelvins}) / 2 = {tI.ToString(TemperatureUnit.Kelvin)} - средняя температура газа на участке перед стравливанием",
                    $"T1н = {tInitialIn.ToString(TemperatureUnit.Kelvin)} - температура газа в начале участка перед стравливанием",
                    $"T1к = {tInitialOut.ToString(TemperatureUnit.Kelvin)} - температура газа в конце участка перед стравливанием",
                    $"z1ср = {zI} - коэффициент сжимаемости при Т1ср, P1ср (по ГОСТ 30319.3)",
                    $"k = {PurgeCount} - количество продувок",
                    $"TSt = {StandardConditions.T.ToString(TemperatureUnit.Kelvin)} - температура газа при стандартных условиях",
                    $"PSt = {StandardConditions.P.ToString(PressureUnit.Mpa)} - абсолютное давление газа при стандартных условиях",
                });
            }
            else // частичное стравливание
            {
                result = 0.001*Volume*(pI.Mpa/tI.Kelvins/zI - pF.Mpa/tF.Kelvins/zF)*StandardConditions.T.Kelvins/StandardConditions.P.Mpa;
                ListingCalculation = string.Join("\n", new List<string>
                {
                     $"Qсниж_ЛЧ = 0,001 * {Volume} * ({pI.Mpa} / {tI.Kelvins} / {zI} - {pF.Mpa} / {tF.Kelvins} / {zF}) * {StandardConditions.T.Kelvins} / {StandardConditions.P.Mpa} = {result} тыс.м³ - объем газа, стравливаемого при снижении давления газа на участке МГ",
                     $"Vуч = {Volume} м³ - геометрический объем участка газопровода",
                     $"P1ср = {listingPi} МПа - среднее давление газа на участке перед стравливанием",
                     $"P1н = {pInitialIn.Mpa} + {PressureAir.Mpa} = {pInitialInAbs.ToString(PressureUnit.Mpa)}   - абсолютное давление газа в начале участка перед стравливанием",
                     $"P1к = {pInitialOut.Mpa} + {PressureAir.Mpa} = {pInitialOutAbs.ToString(PressureUnit.Mpa)} - абсолютное давление газа в конце участка перед стравливанием",
                     $"P2ср = {listingPf} МПа - среднее абсолютное давление газа на участке после стравливания",
                     $"P2н = {PressureFinalIn.Mpa} + {PressureAir.Mpa} = {pFinalIn.ToString(PressureUnit.Mpa)}   - абсолютное давление газа в начале участка после стравливания",
                     $"P2к = {PressureFinalOut.Mpa} + {PressureAir.Mpa} = {pFinalOut.ToString(PressureUnit.Mpa)} - абсолютное давление газа в конце участка после стравливания",
                     $"PAir = {PressureAir.Mpa} - атмосферное давление",
                     $"T1ср = ({tInitialIn.Kelvins} + {tInitialOut.Kelvins}) / 2 = {tI.ToString(TemperatureUnit.Kelvin)} - средняя температура газа на участке перед стравливанием",
                     $"T1н = {tInitialIn.ToString(TemperatureUnit.Kelvin)}  - температура газа в начале участка перед стравливанием",
                     $"T1к = {tInitialOut.ToString(TemperatureUnit.Kelvin)} - температура газа в конце участка перед стравливанием",
                     $"Т2ср =  ({TemperatureFinalIn.Kelvins} + {TemperatureFinalOut.Kelvins}) / 2 = {tF.ToString(TemperatureUnit.Kelvin)} - средняя температура газа на участке после стравливания",
                     $"T2н = {TemperatureFinalIn.ToString(TemperatureUnit.Kelvin)}  - температура газа в начале участка после стравливания",
                     $"T2к = {TemperatureFinalOut.ToString(TemperatureUnit.Kelvin)} - температура газа в конце участка после стравливания",
                     $"z1ср = {zI} - коэффициент сжимаемости при Т1ср, P1ср (по ГОСТ 30319.3)",
                     $"z2ср = {zF} - коэффициент сжимаемости при Т2ср, P2ср (по ГОСТ 30319.3)",
                     $"TSt = {StandardConditions.T.ToString(TemperatureUnit.Kelvin)} - температура газа при стандартных условиях",
                     $"PSt = {StandardConditions.P.ToString(PressureUnit.Mpa)} - абсолютное давление газа при стандартных условиях",
                });
            }

            return result;
        }
#region property
        /// <summary> Объем выработки газа (традиционными методами) </summary>
        public double RecoveryVolume
        {
            get
            {
                if (HasRecovery)
                {
                    var pAir = PressureAir;
                    var pInitialIn = PressureInitialIn + pAir;
                    var pInitialOut = PressureInitialOut + pAir;
                    var pRecoveryIn = PressureRecoveryIn + pAir;
                    var pRecoveryOut = PressureRecoveryOut + pAir;

                    var pI = Pressure.FromMpa(2.0 / 3.0 *
                                 (pInitialIn.Mpa + Math.Pow(pInitialOut.Mpa, 2.0) / (pInitialIn.Mpa + pInitialOut.Mpa)));// ((PressureInitialIn + PressureInitialOut) / 2) + pAir;
                    var tI = (TemperatureInitialIn + TemperatureInitialOut)/2;
                //    var zI = SupportCalculations.GasCompressibilityFactorApproximate(pI, tI, Density);
                    var gasContent = new List<ComponentContent>
                    {
                        new ComponentContent
                        {
                            Component = PropertyType.ContentMethane,
                            Concentration = 1 - (NitrogenContent/100.0 + CarbonDioxideContent/100.0)
                        },
                        new ComponentContent
                        {
                            Component = PropertyType.ContentNitrogen,
                            Concentration = NitrogenContent/100.0
                        },
                        new ComponentContent
                        {
                            Component = PropertyType.ContentCarbonDioxid,
                            Concentration = CarbonDioxideContent/100.0
                        },
                    };
                    var zI = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, pI.Mpa, tI.Kelvins);

                    var pR = Pressure.FromMpa(2.0 / 3.0 *
                                      (pRecoveryIn.Mpa +
                                       Math.Pow(pRecoveryOut.Mpa, 2.0) / (pRecoveryIn.Mpa + pRecoveryOut.Mpa)));// ((PressureRecoveryIn + PressureRecoveryOut) / 2) + pAir;
                    var tR = (TemperatureRecoveryIn + TemperatureRecoveryOut)/2;
                 //   var zR = SupportCalculations.GasCompressibilityFactorApproximate(pR, tR, Density);
                    var zR = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, pR.Mpa, tR.Kelvins);

                    return 0.001* Volume * (pI.Mpa/tI.Kelvins/zI - pR.Mpa/tR.Kelvins/zR)*StandardConditions.T.Kelvins/StandardConditions.P.Mpa;
                }

                return 0;
            }
        }
        /// <summary>
        /// Объем выработки газа с помощью мобильной КС
        /// </summary>
        public double MobilePumpRecoveryVolume
        {
            get
            {
                var pAir = PressureAir;
                var pInitialIn = PressureInitialIn + pAir;
                var pInitialOut = PressureInitialOut + pAir;
                var pMobilePumpRecoveryIn = PressureMobilePumpRecoveryIn + pAir;
                var pMobilePumpRecoveryOut = PressureMobilePumpRecoveryOut + pAir;
                var gasContent = new List<ComponentContent>
                    {
                        new ComponentContent
                        {
                            Component = PropertyType.ContentMethane,
                            Concentration = 1 - (NitrogenContent/100.0 + CarbonDioxideContent/100.0)
                        },
                        new ComponentContent
                        {
                            Component = PropertyType.ContentNitrogen,
                            Concentration = NitrogenContent/100.0
                        },
                        new ComponentContent
                        {
                            Component = PropertyType.ContentCarbonDioxid,
                            Concentration = CarbonDioxideContent/100.0
                        },
                    };

                if (HasMobilePumpRecovery)
                {
                    var pI = Pressure.FromMpa(2.0 / 3.0 *
                                 (pInitialIn.Mpa + Math.Pow(pInitialOut.Mpa, 2.0) / (pInitialIn.Mpa + pInitialOut.Mpa))); // ((PressureInitialIn + PressureInitialOut) / 2) + PressureAir;
                    var tI = (TemperatureInitialIn+ TemperatureInitialOut)/2;
              //      var zI = SupportCalculations.GasCompressibilityFactorApproximate(pI, tI, Density);
                    var zI = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, pI.Mpa, tI.Kelvins);

                    if (HasRecovery)
                    {
                        pI = ((PressureRecoveryIn + PressureRecoveryOut) / 2) + PressureAir;
                        tI = (TemperatureRecoveryIn + TemperatureRecoveryOut)/2;
                 //       zI = SupportCalculations.GasCompressibilityFactorApproximate(pI, tI, Density);
                        zI = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, pI.Mpa, tI.Kelvins);
                    }

                    var pMpr = Pressure.FromMpa(2.0 / 3.0 *
                                     (pMobilePumpRecoveryIn.Mpa +
                                      Math.Pow(pMobilePumpRecoveryOut.Mpa, 2.0) /
                                      (pMobilePumpRecoveryIn.Mpa + pMobilePumpRecoveryOut.Mpa))); // ((PressureMobilePumpRecoveryIn + PressureMobilePumpRecoveryOut) / 2) + PressureAir;
                    var tMpr = (TemperatureMobilePumpRecoveryIn+ TemperatureMobilePumpRecoveryOut)/2;
                    //   var zMpr = SupportCalculations.GasCompressibilityFactorApproximate(pMpr, tMpr, Density);
                    var zMpr = GasCompressibilityCalculator.GasCompressibilityFactor(gasContent, pMpr.Mpa, tMpr.Kelvins);

                    return 0.001* Volume * (pI.Mpa/tI.Kelvins/zI - pMpr.Mpa/tMpr.Kelvins/zMpr)*StandardConditions.T.Kelvins/StandardConditions.P.Mpa;
                }
                return 0;
            }
        }
        public double Volume
        {
            get
            {
                return PipingVolume == 0 ? SectionVolume : PipingVolume;
            } 
        }
        [Display(Name = "Давление газа в начале участка до отключения, кг/см²")] 
        public Pressure PressureInitialIn { get; set; }
        [Display(Name = "Давление газа в конце участка до отключения, кг/см²")]
        public Pressure PressureInitialOut { get; set; }
        [Display(Name = "Температура газа в начале участка до отключения, Гр.С")]
        public Temperature TemperatureInitialIn { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Температура газа в конце участка до отключения, Гр.С")]
        public Temperature TemperatureInitialOut { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Выработка газа традиционными методами")]
        public bool HasRecovery { get; set; }
        [Display(Name = "Давление газа в начале участка после выработки, кг/см²")]
        public Pressure PressureRecoveryIn { get; set; }
        [Display(Name = "Давление газа в конце участка после выработки, кг/см²")]
        public Pressure PressureRecoveryOut { get; set; }
        [Display(Name = "Температура газа в начале участка после выработки, Гр.С")]
        public Temperature TemperatureRecoveryIn { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Температура газа в конце участка после выработки, Гр.С")]
        public Temperature TemperatureRecoveryOut { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Выработка газа мобильной КС")]
        public bool HasMobilePumpRecovery { get; set; }
        [Display(Name = "Давление газа в начале участка после выработки мобильной КС, кг/см²")]
        public Pressure PressureMobilePumpRecoveryIn { get; set; }
        [Display(Name = "Давление газа в конце участка после выработки мобильной КС, кг/см²")]
        public Pressure PressureMobilePumpRecoveryOut { get; set; }
        [Display(Name = "Температура газа в начале участка после выработки мобильной КС, Гр.С")]
        public Temperature TemperatureMobilePumpRecoveryIn { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Температура газа в конце участка после выработки мобильной КС, Гр.С")]
        public Temperature TemperatureMobilePumpRecoveryOut { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Давление газа в начале участка после стравливания, кг/см²")]
        public Pressure PressureFinalIn { get; set; }
        [Display(Name = "Давление газа в конце участка после стравливания, кг/см²")]
        public Pressure PressureFinalOut { get; set; }
        [Display(Name = "Температура газа в начале участка после стравливания, Гр.С")]
        public Temperature TemperatureFinalIn { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Температура газа в конце участка после стравливания, Гр.С")]
        public Temperature TemperatureFinalOut { get; set; } = Temperature.FromCelsius(0);
        [Display(Name = "Плотность газа, кг/м³")]
        public Density Density { get; set; }
        [Display(Name = "Давление атмосферное, мм рт.ст.")]
        public Pressure PressureAir { get; set; }
        [Display(Name = "Содержание азота в газе, мол.доля %")]
        public double NitrogenContent { get; set; }
        [Display(Name = "Содержание двуокиси углерода СО2 в газе, мол.доля %")]
        public double CarbonDioxideContent { get; set; }
        [Display(Name = "Количество продувок")]
        public int PurgeCount { get; set; }
        // Эта часть используется если выбран тип объекта отличный от газопровода.
        // Тогда геометрический объем расчитывается с помощью калькулятора
        [Display(Name = "Перечень газопроводов")]
        public List<PipeSection> Piping { get; set; }
        public double PipingVolume
        {
            get { return Piping.Sum(p => p.Volume); }
        }
        // Эта часть для объектов типа газопровод, 
        // объем расчитывается в зависимости от выбранного участка на газопроводе.
        [Display(Name = "Километр начала участка на газопроводе")]
        public double KmStart { get; set; }
        [Display(Name = "Километр конца участка на газопроводе")]
        public double KmEnd { get; set; }
        [Display (Name = "Геометрический объем выбранного участка газопровода")]
        public double SectionVolume { get; set; }
#endregion
    }
}