using System;

namespace Utils.Extensions
{
    // ReSharper disable once UnusedMember.Global
	public static class GuidExtensions
	{
	    // ReSharper disable once UnusedMember.Global
        public static Guid Convert(this Guid guid)
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
    }
}
