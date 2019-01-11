using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazRouter.Service.Exchange.Lib.AsduEsg.ExchangeFormat
{
    public static class GuidExtensions
    {
        public static string ToOracle(this Guid guid)
        {
            return BitConverter.ToString(guid.ToByteArray()).Replace("-", "");
        }

        public static Guid ConvertOracle(Guid guid)
        {
            var b = guid.ToByteArray();
            var c = new byte[16];

            c[0] = b[3];
            c[1] = b[2];
            c[2] = b[1];
            c[3] = b[0];

            c[4] = b[5];
            c[5] = b[4];
            c[6] = b[7];
            c[7] = b[6];

            c[8] = b[8];
            c[9] = b[9];
            c[10] = b[10];
            c[11] = b[11];

            c[12] = b[12];
            c[13] = b[13];
            c[14] = b[14];
            c[15] = b[15];

            return new Guid(c);
        }

        public static Guid? ParseOracle(string guidString)
        {
            if (string.IsNullOrEmpty(guidString)) return null;
            return ConvertOracle(Guid.Parse(guidString));
        }
        //public static string ConvertGuid(this Guid guid)
        //{
        //    return guid.Convert().ToString().Replace("-", "").ToUpper();
        //}

        //public static string ToStringParameter(this Guid guid)
        //{
        //    return guid.ToString().Replace("{", "").Replace("}", "").Replace("-", "").ToUpper();
        //}



    }
}
