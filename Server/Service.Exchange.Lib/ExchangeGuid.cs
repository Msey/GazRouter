using System;
using GazRouter.DTO.Infrastructure;

namespace GazRouter.Service.Exchange.Lib
{
    public class ExchangeGuid
    {
        public Guid? Value { get; set; }

        public ExchangeGuid()
        {
        }

        public ExchangeGuid(Guid? value)
        {
            Value = value;
        }

        public static implicit operator string(ExchangeGuid eg)
        {
            return eg.Value.HasValue ? ((Guid)eg.Value).Convert().ToString().Replace("-", "").ToUpper() : String.Empty;
        }

        public static implicit operator Guid?(ExchangeGuid eg)
        {
            return eg.Value;
        }
        
        public static implicit operator Guid(ExchangeGuid eg)
        {
            return (Guid) eg.Value;
        }

        public static implicit operator ExchangeGuid(string s)
        {
            var bytes = ParseHex(s);
            return new ExchangeGuid(new Guid(bytes));
        }

        public static byte[] ParseHex(string text)
        {
            var ret = new byte[text.Length / 2];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = Convert.ToByte(text.Substring(i * 2, 2), 16);
            }
            return ret;
        }

    }
}