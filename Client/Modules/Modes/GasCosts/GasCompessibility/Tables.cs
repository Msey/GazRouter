using System;
using System.Collections.Generic;
using GazRouter.DTO.Dictionaries.PropertyTypes;

namespace GazRouter.Modes.GasCosts.GasCompessibility
{
    public class Tables
    {
        /// <summary>
        /// “‡·Î. ¿1 (√Œ—“ 30319.3-2015)
        /// </summary>
        /// <returns></returns>
        public static List<PureComponentsParameters> TableA1()
        {
            return new List<PureComponentsParameters>()
            {
                new PureComponentsParameters {Component = PropertyType.ContentMethane, Mi = 16.043, zci = 0.9981, Ei = 151.318300, Ki = 0.4619255, Gi = 0.0, Qi = 0.0, Fi = 0.0, Si = 0.0, Wi = 0.0},
                new PureComponentsParameters {Component = PropertyType.ContentEthane, Mi = 30.070, zci = 0.992, Ei = 244.166700, Ki = 0.5279209, Gi = 0.079300, Qi = 0.0, Fi = 0.0, Si = 0.0, Wi = 0.0},
                new PureComponentsParameters {Component = PropertyType.ContentPropane, Mi = 44.097, zci = 0.9834, Ei = 298.118300, Ki = 0.5837490, Gi = 0.141239, Qi = 0.0, Fi = 0.0, Si = 0.0, Wi = 0.0},
                new PureComponentsParameters {Component = PropertyType.ContentIsobutane, Mi = 58.123, zci = 0.971, Ei = 324.068900, Ki = 0.6406937, Gi = 0.256692, Qi = 0.0, Fi = 0.0, Si = 0.0, Wi = 0.0},
                new PureComponentsParameters {Component = PropertyType.ContentButane, Mi = 58.123, zci = 0.9682, Ei = 337.638900, Ki = 0.6341423, Gi = 0.281835, Qi = 0.0, Fi = 0.0, Si = 0.0, Wi = 0.0},
                new PureComponentsParameters {Component = PropertyType.ContentIsopentane, Mi = 72.150, zci = 0.953, Ei = 365.599900, Ki = 0.6738577, Gi = 0.332267, Qi = 0.0, Fi = 0.0, Si = 0.0, Wi = 0.0},
                new PureComponentsParameters {Component = PropertyType.ContentPentane, Mi = 72.150, zci = 0.945, Ei = 370.682300, Ki = 0.6798307, Gi = 0.366911, Qi = 0.0, Fi = 0.0, Si = 0.0, Wi = 0.0},
                new PureComponentsParameters {Component = PropertyType.ContentHexane, Mi = 86.177, zci = 0.919, Ei = 402.636293, Ki = 0.7175118, Gi = 0.289731, Qi = 0.0, Fi = 0.0, Si = 0.0, Wi = 0.0},
                new PureComponentsParameters {Component = PropertyType.ContentNitrogen, Mi = 28.0135, zci = 0.9997, Ei = 99.737780, Ki = 0.4479153, Gi = 0.027815, Qi = 0.0, Fi = 0.0, Si = 0.0, Wi = 0.0},
                new PureComponentsParameters {Component = PropertyType.ContentCarbonDioxid, Mi = 44.010, zci = 0.9947, Ei = 241.960600, Ki = 0.4557489, Gi = 0.189065, Qi = 0.690000, Fi = 0.0, Si = 0.0, Wi = 0.0},
                new PureComponentsParameters {Component = PropertyType.ContentHelium, Mi = 4.0026, zci = 1.0005, Ei = 2.610111, Ki = 0.3589888, Gi = 0.0, Qi = 0.0, Fi = 0.0, Si = 0.0, Wi = 0.0},
                new PureComponentsParameters {Component = PropertyType.ContentHydrogen, Mi = 2.0159, zci = 1.0006, Ei = 26.957940, Ki = 0.3514916, Gi = 0.034369, Qi = 0.0, Fi = 1.0, Si = 0.0, Wi = 0.0}
            };
        }
        
        /// <summary>
        /// “‡·Î. ¿2 (√Œ—“ 30319.3-2015)
        /// </summary>
        /// <returns></returns>
        public static List<ComponentsBinaryInteraction> TableA2()
        {
            return new List<ComponentsBinaryInteraction>
            {
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentMethane,
                    Component2 = PropertyType.ContentPropane,
                    Eij = 0.994635,
                    Vij = 0.990877,
                    Kij = 1.007619,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentMethane,
                    Component2 = PropertyType.ContentIsobutane,
                    Eij = 1.019530,
                    Vij = 1.0,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentMethane,
                    Component2 = PropertyType.ContentButane,
                    Eij = 0.989844,
                    Vij = 0.992291,
                    Kij = 0.997596,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentMethane,
                    Component2 = PropertyType.ContentIsopentane,
                    Eij = 1.002350,
                    Vij = 1.0,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentMethane,
                    Component2 = PropertyType.ContentPentane,
                    Eij = 0.999268,
                    Vij = 1.003670,
                    Kij = 1.002529,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentMethane,
                    Component2 = PropertyType.ContentHexane,
                    Eij = 1.107274,
                    Vij = 1.302576,
                    Kij = 0.982962,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentMethane,
                    Component2 = PropertyType.ContentNitrogen,
                    Eij = 0.971640,
                    Vij = 0.886106,
                    Kij = 1.003630,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentMethane,
                    Component2 = PropertyType.ContentCarbonDioxid,
                    Eij = 0.960644,
                    Vij = 0.963827,
                    Kij = 0.995933,
                    Gij = 0.807653
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentMethane,
                    Component2 = PropertyType.ContentHydrogen,
                    Eij = 1.170520,
                    Vij = 1.156390,
                    Kij = 1.023260,
                    Gij = 1.957310
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentEthane,
                    Component2 = PropertyType.ContentPropane,
                    Eij = 1.022560,
                    Vij = 1.065173,
                    Kij = 0.986893,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentEthane,
                    Component2 = PropertyType.ContentIsobutane,
                    Eij = 1.0,
                    Vij = 1.250000,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentEthane,
                    Component2 = PropertyType.ContentButane,
                    Eij = 1.013060,
                    Vij = 1.250000,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentEthane,
                    Component2 = PropertyType.ContentIsopentane,
                    Eij = 1.0,
                    Vij = 1.250000,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentEthane,
                    Component2 = PropertyType.ContentPentane,
                    Eij = 1.005320,
                    Vij = 1.250000,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentEthane,
                    Component2 = PropertyType.ContentNitrogen,
                    Eij = 0.970120,
                    Vij = 0.816431,
                    Kij = 1.007960,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentEthane,
                    Component2 = PropertyType.ContentCarbonDioxid,
                    Eij = 0.925053,
                    Vij = 0.969870,
                    Kij = 1.008510,
                    Gij = 0.370296
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentEthane,
                    Component2 = PropertyType.ContentHydrogen,
                    Eij = 1.164460,
                    Vij = 1.616660,
                    Kij = 1.020340,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentPropane,
                    Component2 = PropertyType.ContentButane,
                    Eij = 1.004900,
                    Vij = 1.0,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentPropane,
                    Component2 = PropertyType.ContentNitrogen,
                    Eij = 0.945939,
                    Vij = 0.915502,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentPropane,
                    Component2 = PropertyType.ContentCarbonDioxid,
                    Eij = 0.960237,
                    Vij = 1.0,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentPropane,
                    Component2 = PropertyType.ContentHydrogen,
                    Eij = 1.034787,
                    Vij = 1.0,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentIsobutane,
                    Component2 = PropertyType.ContentNitrogen,
                    Eij = 0.946914,
                    Vij = 1.0,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentIsobutane,
                    Component2 = PropertyType.ContentCarbonDioxid,
                    Eij = 0.906849,
                    Vij = 1.0,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentIsobutane,
                    Component2 = PropertyType.ContentHydrogen,
                    Eij = 1.300000,
                    Vij = 1.0,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentButane,
                    Component2 = PropertyType.ContentNitrogen,
                    Eij = 0.973384,
                    Vij = 0.993556,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentButane,
                    Component2 = PropertyType.ContentCarbonDioxid,
                    Eij = 0.897362,
                    Vij = 1.0,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentButane,
                    Component2 = PropertyType.ContentHydrogen,
                    Eij = 1.300000,
                    Vij = 1.0,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentIsopentane,
                    Component2 = PropertyType.ContentNitrogen,
                    Eij = 0.959340,
                    Vij = 1.0,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentIsopentane,
                    Component2 = PropertyType.ContentCarbonDioxid,
                    Eij = 0.726255,
                    Vij = 1.0,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentPentane,
                    Component2 = PropertyType.ContentNitrogen,
                    Eij = 0.945520,
                    Vij = 1.0,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentPentane,
                    Component2 = PropertyType.ContentCarbonDioxid,
                    Eij = 0.859764,
                    Vij = 1.0,
                    Kij = 1.0,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentHexane,
                    Component2 = PropertyType.ContentCarbonDioxid,
                    Eij = 0.855134,
                    Vij = 1.066638,
                    Kij = 0.910183,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentNitrogen,
                    Component2 = PropertyType.ContentCarbonDioxid,
                    Eij = 1.022740,
                    Vij = 0.835058,
                    Kij = 0.982361,
                    Gij = 0.982746
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentNitrogen,
                    Component2 = PropertyType.ContentHydrogen,
                    Eij = 1.086320,
                    Vij = 0.408838,
                    Kij = 1.032270,
                    Gij = 1.0
                },
                new ComponentsBinaryInteraction
                {
                    Component1 = PropertyType.ContentCarbonDioxid,
                    Component2 = PropertyType.ContentHydrogen,
                    Eij = 1.281790,
                    Vij = 1.0,
                    Kij = 1.0,
                    Gij = 1.0
                }
            };
        }

        /// <summary>
        /// “‡·Î. ¿3 (√Œ—“ 30319.3-2015)
        /// </summary>
        /// <returns></returns>
        public static List<DimensionlessCoefficients> TableA3()
        {
            return new List<DimensionlessCoefficients>
            {
                new DimensionlessCoefficients
                {
                    n = 1,
                    an = 0.153832600,
                    bn = 1,
                    cn = 0,
                    kn = 0,
                    un = 0.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 2,
                    an = 1.341953000,
                    bn = 1,
                    cn = 0,
                    kn = 0,
                    un = 0.5,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 3,
                    an = -2.998583000,
                    bn = 1,
                    cn = 0,
                    kn = 0,
                    un = 1.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 4,
                    an = -0.048312280,
                    bn = 1,
                    cn = 0,
                    kn = 0,
                    un = 3.5,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 5,
                    an = 0.375796500,
                    bn = 1,
                    cn = 0,
                    kn = 0,
                    un = -0.5,
                    gn = 1,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 6,
                    an = -1.589575000,
                    bn = 1,
                    cn = 0,
                    kn = 0,
                    un = 4.5,
                    gn = 1,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 7,
                    an = -0.053588470,
                    bn = 1,
                    cn = 0,
                    kn = 0,
                    un = 0.5,
                    gn = 0,
                    qn = 1,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 8,
                    an = 0.886594630,
                    bn = 1,
                    cn = 0,
                    kn = 0,
                    un = 7.5,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 1,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 9,
                    an = -0.710237040,
                    bn = 1,
                    cn = 0,
                    kn = 0,
                    un = 9.5,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 1,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 10,
                    an = -1.471722000,
                    bn = 1,
                    cn = 0,
                    kn = 0,
                    un = 6.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 1
                },
                new DimensionlessCoefficients
                {
                    n = 11,
                    an = 1.321850350,
                    bn = 1,
                    cn = 0,
                    kn = 0,
                    un = 12.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 1
                },
                new DimensionlessCoefficients
                {
                    n = 12,
                    an = -0.786659250,
                    bn = 1,
                    cn = 0,
                    kn = 0,
                    un = 12.5,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 1
                },
                new DimensionlessCoefficients
                {
                    n = 13,
                    an = 2.291290*Math.Pow(10.0, -9),
                    bn = 1,
                    cn = 1,
                    kn = 3,
                    un = -6.0,
                    gn = 0,
                    qn = 0,
                    fn = 1,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 14,
                    an = 0.157672400,
                    bn = 1,
                    cn = 1,
                    kn = 2,
                    un = 2.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 15,
                    an = -0.436386400,
                    bn = 1,
                    cn = 1,
                    kn = 2,
                    un = 3.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 16,
                    an = -0.044081590,
                    bn = 1,
                    cn = 1,
                    kn = 2,
                    un = 2.0,
                    gn = 0,
                    qn = 1,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 17,
                    an = -0.003433888,
                    bn = 1,
                    cn = 1,
                    kn = 4,
                    un = 2.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 18,
                    an = 0.032059050,
                    bn = 1,
                    cn = 1,
                    kn = 4,
                    un = 11.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 19,
                    an = 0.024873550,
                    bn = 2,
                    cn = 0,
                    kn = 0,
                    un = -0.5,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 20,
                    an = 0.073322790,
                    bn = 2,
                    cn = 0,
                    kn = 0,
                    un = 0.5,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 21,
                    an = -0.001600573,
                    bn = 2,
                    cn = 1,
                    kn = 2,
                    un = 0.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 22,
                    an = 0.642470600,
                    bn = 2,
                    cn = 1,
                    kn = 2,
                    un = 4.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0.0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 23,
                    an = -0.416260100,
                    bn = 2,
                    cn = 1,
                    kn = 2,
                    un = 6.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 24,
                    an = -0.066899570,
                    bn = 2,
                    cn = 1,
                    kn = 4,
                    un = 21.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 25,
                    an = 0.279179500,
                    bn = 2,
                    cn = 1,
                    kn = 4,
                    un = 23.0,
                    gn = 1,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 26,
                    an = -0.696605100,
                    bn = 2,
                    cn = 1,
                    kn = 4,
                    un = 22.0,
                    gn = 0,
                    qn = 1,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 27,
                    an = -0.002860589,
                    bn = 2,
                    cn = 1,
                    kn = 4,
                    un = -1.0,
                    gn = 0,
                    qn = 0,
                    fn = 1,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 28,
                    an = -0.008098836,
                    bn = 3,
                    cn = 0,
                    kn = 0,
                    un = -0.5,
                    gn = 0,
                    qn = 1,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 29,
                    an = 3.150547000,
                    bn = 3,
                    cn = 1,
                    kn = 1,
                    un = 7.0,
                    gn = 1,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 30,
                    an = 0.007224479,
                    bn = 3,
                    cn = 1,
                    kn = 1,
                    un = -1.0,
                    gn = 0,
                    qn = 0,
                    fn = 1,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 31,
                    an = -0.705752900,
                    bn = 3,
                    cn = 1,
                    kn = 2,
                    un = 6.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 32,
                    an = 0.534979200,
                    bn = 3,
                    cn = 1,
                    kn = 2,
                    un = 4.0,
                    gn = 1,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 33,
                    an = -0.079314910,
                    bn = 3,
                    cn = 1,
                    kn = 3,
                    un = 1.0,
                    gn = 1,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 34,
                    an = -1.418465000,
                    bn = 3,
                    cn = 1,
                    kn = 3,
                    un = 9.0,
                    gn = 1,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 35,
                    an = -5.99905*Math.Pow(10.0, -17),
                    bn = 3,
                    cn = 1,
                    kn = 4,
                    un = -13.0,
                    gn = 0,
                    qn = 0,
                    fn = 1,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 36,
                    an = 0.105840200,
                    bn = 3,
                    cn = 1,
                    kn = 4,
                    un = 21.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 37,
                    an = 0.034317290,
                    bn = 3,
                    cn = 1,
                    kn = 4,
                    un = 8.0,
                    gn = 0,
                    qn = 1,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 38,
                    an = -0.007022847,
                    bn = 4,
                    cn = 0,
                    kn = 0,
                    un = -0.5,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 39,
                    an = 0.024955870,
                    bn = 4,
                    cn = 0,
                    kn = 0,
                    un = 0.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 40,
                    an = 0.042968180,
                    bn = 4,
                    cn = 1,
                    kn = 2,
                    un = 2.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 41,
                    an = 0.746545300,
                    bn = 4,
                    cn = 1,
                    kn = 2,
                    un = 7.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 42,
                    an = -0.291961300,
                    bn = 4,
                    cn = 1,
                    kn = 2,
                    un = 9.0,
                    gn = 0,
                    qn = 1,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 43,
                    an = 7.294616000,
                    bn = 4,
                    cn = 1,
                    kn = 4,
                    un = 22.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 44,
                    an = -9.936757000,
                    bn = 4,
                    cn = 1,
                    kn = 4,
                    un = 23.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 45,
                    an = -0.005399808,
                    bn = 5,
                    cn = 0,
                    kn = 0,
                    un = 1.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 46,
                    an = -0.243256700,
                    bn = 5,
                    cn = 1,
                    kn = 2,
                    un = 9.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 47,
                    an = 0.049870160,
                    bn = 5,
                    cn = 1,
                    kn = 2,
                    un = 3.0,
                    gn = 0,
                    qn = 1,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 48,
                    an = 0.003733797,
                    bn = 5,
                    cn = 1,
                    kn = 4,
                    un = 8.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 49,
                    an = 1.874951000,
                    bn = 5,
                    cn = 1,
                    kn = 4,
                    un = 23.0,
                    gn = 0,
                    qn = 1,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 50,
                    an = 0.002168144,
                    bn = 6,
                    cn = 0,
                    kn = 0,
                    un = 1.5,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 51,
                    an = -0.658716400,
                    bn = 6,
                    cn = 1,
                    kn = 2,
                    un = 5.0,
                    gn = 1,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 52,
                    an = 0.000205518,
                    bn = 7,
                    cn = 0,
                    kn = 0,
                    un = -0.5,
                    gn = 0,
                    qn = 1,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 53,
                    an = 0.009776195,
                    bn = 7,
                    cn = 1,
                    kn = 2,
                    un = 4.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 54,
                    an = -0.020487080,
                    bn = 8,
                    cn = 1,
                    kn = 1,
                    un = 7.0,
                    gn = 1,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 55,
                    an = 0.015573220,
                    bn = 8,
                    cn = 1,
                    kn = 2,
                    un = 3.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 56,
                    an = 0.006862415,
                    bn = 8,
                    cn = 1,
                    kn = 2,
                    un = 0.0,
                    gn = 1,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 57,
                    an = -0.001226752,
                    bn = 9,
                    cn = 1,
                    kn = 2,
                    un = 1.0,
                    gn = 0,
                    qn = 0,
                    fn = 0,
                    sn = 0,
                    wn = 0
                },
                new DimensionlessCoefficients
                {
                    n = 58,
                    an = 0.002850908,
                    bn = 9,
                    cn = 1,
                    kn = 2,
                    un = 0.0,
                    gn = 0,
                    qn = 1,
                    fn = 0,
                    sn = 0,
                    wn = 0
                }
            };
        }
    }
}