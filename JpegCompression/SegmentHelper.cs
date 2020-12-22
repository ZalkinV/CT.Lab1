using System;
using System.Collections.Generic;
using System.Text;

namespace JpegCompression
{
    public static class SegmentHelper
    {
        const uint Half = uint.MaxValue / 2;
        const uint Quarter = uint.MaxValue / 4;
        const uint ThirdQuarter = 3 * Quarter;

        public static uint E1E2Scale(uint value, uint bit)
        {
            uint multipliedByTwo = value << 1;
            uint result = multipliedByTwo + bit;

            return result;
        }

        public static uint E3Scale(uint value, uint bit)
        {
            uint lastBit = value & ((uint)int.MaxValue + 1);
            uint shiftedValue = (value << 2) >> 1;
            uint result = lastBit + shiftedValue + bit;

            return result;
        }

        public static bool IsInSameHalf(uint left, uint right)
        {
            bool result = left >= Half || right <= Half;
            return result;
        }

        public static bool IsInFirstQuarter(uint value)
        {
            bool result = value <= Quarter;
            return result;
        }

        public static bool IsInSecondQuarter(uint value)
        {
            bool result = value >= Quarter;
            return result;
        }

        public static bool IsInThirdQuarter(uint value)
        {
            bool result = value <= ThirdQuarter;
            return result;
        }

        public static bool IsInForthQuarter(uint value)
        {
            bool result = value >= ThirdQuarter;
            return result;
        }
    }
}
