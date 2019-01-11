using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace GazRouter.Service.Exchange.Lib.Import.Astra
{
    /// <summary>
    /// тип линейного участка АСТРы
    /// </summary>
    public class AstraPipeSectionType
    {
        public const int NotDefined = 0;
        public const int GasMainLineSection = 1;
        public const int OtherLineSection = 2;
    }

    /// <summary>
    /// линейный участок в АСТРе
    /// </summary>
    public class AstraPipeSection
    {
        public AstraPipeSection()
        {}

        public AstraPipeSection(string astraRow, CultureInfo culture, char valueSeparator)
        {
            var s = astraRow.Split(new[] { valueSeparator });
            if (s.Length > 2)
            {
                string dtStr = s[s.Length - 2];
                DateTime dt;
                if (DateTime.TryParseExact(dtStr, "yyyyMMddHH", culture, DateTimeStyles.None, out dt))
                    Timestamp = dt;
            }
            //парсинг строки запаса Астры, должно быть 8 элементов массива
            if (astraRow.Length >= 8)
            {
                try
                {
                    Description = s[0];
                    GasVolume = double.Parse(s[1], culture.NumberFormat);
                    PipeSectionAstraCode = int.Parse(s[2]);
                    GasMainAstraCode = int.Parse(s[3]);
                    KilometerStart = double.Parse(s[4], culture.NumberFormat);
                    KilometerEnd = double.Parse(s[5], culture.NumberFormat);
                }
                catch (SystemException)
                {
                    Description = "Parse error || String from file:" + astraRow;
                }
            }
            else
            {
                Description = "Bad supply record || String from file:" + astraRow;
            }
        }
        public DateTime Timestamp { get; set; }
        public int PipeSectionAstraCode { get; set; }
        public int GasMainAstraCode { get; set; }
        public double GasVolume { get; set; }
        public string Description { get; set; }
        public AstraPipeSectionType PipeSectionType { get; set; }
        public double KilometerStart { get; set; }
        public double KilometerEnd { get; set; }
    }
    /// <summary>
    /// линейный участок в АСТРе
    /// </summary>
    public class AstraPipeData
    {
        public AstraPipeData()
        {
            Items = new List<AstraPipeItem>();
        }

        [XmlArray("ExtItems")]
        [XmlArrayItem("ExtItem")]
        public List<AstraPipeItem> Items { get; set; }
    }

    public class AstraPipeItem
    {
        public string Raw { get; set; }
        public string ExtKey { get; set; }
        public int KodUch { get; set; }
        public int KodMg { get; set; }
        public double Value { get; set; }
        public string Description { get; set; }
        public double KmStart { get; set; }
        public double KmEnd { get; set; }
    }

    /// <summary>
    /// КЦ в АСтре
    /// </summary>
    public class AstraCompressorShop
    {
        public AstraCompressorShop(string astraRow, CultureInfo culture, char valueSeparator)
        {
            var s = astraRow.Split(new[] { valueSeparator });
            if (s.Length > 2)
            {
                string dtStr = s[s.Length - 2];
                DateTime dt;
                if (DateTime.TryParseExact(dtStr, "yyyyMMddHH", culture, DateTimeStyles.None, out dt))
                    Timestamp = dt;
            }
            if (astraRow.Length >= 6)
            {
                try
                {
                    Description = s[0];
                    AsduCode = s[1].Trim();
                    AstraCode = s[2].Trim();
                    Pumping = double.Parse(s[3].Trim(), culture.NumberFormat);
                }
                catch (SystemException)
                {
                }
            }
        }
        public double Pumping { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string AsduCode { get; set; }
        public string AstraCode { get; set; }

    }
}
