
namespace GazRouter.DTO
{

    public static class PhysicalQuantityConversions
    {
        public const double OneMegaPascal = 10.197162563367;

        public const double CelsiusZero = 273.15;// Кельвина
        public  const double OneMPa = 7500.6; //1 мПа в мм ртутного столба
        /// <summary>
        /// Перевод давления кг/см² -> МПа
        /// </summary>
        /// <param name="p">Давление в кг/см²</param>
        /// <returns>Давление в МПа</returns>
        public static double Kgh2Mpa(double p)
        {

            return p / OneMegaPascal;
        }

        /// <summary>
        /// Перевод давления МПа -> кг/см²
        /// </summary>
        /// <param name="p">Давление в МПа</param>
        /// <returns>Давление в кг/см²</returns>
        public static double Mpa2Kgh(double p)
        {
            return p * OneMegaPascal;
        }

        /// <summary>
        /// Перевод давления МПа -> мм рт.ст.
        /// </summary>
        /// <param name="p">Давление в МПа</param>
        /// <returns>Давление в мм рт.ст.</returns>
        public static double Mpa2mmHg(double p)
        {
       
            return p * OneMPa;
        }

        /// <summary>
        /// Перевод давления мм рт.ст. ->  МПа
        /// </summary>
        /// <param name="p">Давление в МПа</param>
        /// <returns>Давление в мм рт.ст.</returns>
        public static double mmHg2Mpa(double p)
        {
            return p / OneMPa;
        }

        /// <summary>
        /// Перевод температуры из °C -> K
        /// </summary>
        /// <param name="t">Температура в С</param>
        /// <returns>Температура в К</returns>
        public static double C2K(double t)
        {
            return t + CelsiusZero;
        }

        /// <summary>
        /// Перевод температуры K -> °C
        /// </summary>
        /// <param name="t">Температура в К</param>
        /// <returns>Температура в С</returns>
        public static double K2C(double t)
        {


            return t - CelsiusZero;
        }

        /// <summary>
        /// Перевод кол-ва энергии ккал -> кДж
        /// </summary>
        /// <param name="value">Кол-во энергии в ккал</param>
        /// <returns>Кол-во энергии в кДж</returns>
        public static double Kcal2Kilojoule(double value)
        {
            const double oneKcal = 4.1868;
            return oneKcal * value;
        }

        /// <summary>
        /// Перевод кол-ва энергии кДж -> ккал
        /// </summary>
        /// <param name="value">Кол-во энергии в кДж</param>
        /// <returns>Кол-во энергии в ккал</returns>
        public static double Kilojoule2Kcal(double value)
        {
            const double oneKj = 0.238846;
            return oneKj * value;
        }
    }
}