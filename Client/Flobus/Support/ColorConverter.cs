using System;
using System.Windows.Media;

namespace GazRouter.Flobus.Support
{
    public class ColorConverter
    {
        public static Color FromHex(uint argb)
        {
            return Color.FromArgb(
                (byte)((argb & -16777216) >> 0x18),
                (byte)((argb & 0xff0000) >> 16),
                (byte)((argb & 0xff00) >> 8),
                (byte)(argb & 0xff)
                );
        }

        public static uint ToHex(Color clr)
        {
            byte[] bytes = {clr.A, clr.R, clr.G, clr.B};
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}