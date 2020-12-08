using System;
using System.Collections.Generic;
using System.Text;

namespace Archiver
{
    public class BurrowsWheelerTransform
    {
        public IList<byte> Bytes { get; }

        public BurrowsWheelerTransform(IList<byte> bytes)
        {
            this.Bytes = bytes;
        }

        public FirstLastPair[] DirectTransform()
        {
            var firstLastPairs = GetAllShifts(this.Bytes);
            this.Sort(firstLastPairs);

            return firstLastPairs;
        }

        public static FirstLastPair[] GetAllShifts(IList<byte> bytes)
        {
            int bytesCount = bytes.Count;
            FirstLastPair[] pairs = new FirstLastPair[bytesCount];
            for (int i = 0; i < bytesCount; i++)
            {
                byte curByte = bytes[i];
                int lastByteIndex = (i + bytesCount - 1) % bytesCount;
                byte lastByte = bytes[lastByteIndex];

                pairs[i] = new FirstLastPair(curByte, lastByte, i);
            }

            return pairs;
        }
    }
}
