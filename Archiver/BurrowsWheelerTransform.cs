using System;
using System.Collections.Generic;
using System.Linq;
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

        public BurrowsWheelerResult DirectTransform()
        {
            var firstLastPairs = GetAllShifts(this.Bytes);
            this.Sort(firstLastPairs);

            int initialStringIndex = 0;
            byte[] lastSymbols = new byte[firstLastPairs.Count];
            for (int i = 0; i < firstLastPairs.Count; i++)
            {
                if (firstLastPairs[i].OriginIndex == 0)
                    initialStringIndex = i;
                lastSymbols[i] = firstLastPairs[i].Last;
            }

            var bwtResult = new BurrowsWheelerResult(lastSymbols, initialStringIndex);

            return bwtResult;
        }

        public static IList<FirstLastPair> GetAllShifts(IList<byte> bytes)
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

        public void Sort(IList<FirstLastPair> pairs)
        {
            int bytesCount = this.Bytes.Count;

            for (int i = 1; i < pairs.Count; i++)
            {
                int j = i - 1;

                FirstLastPair pairToPut = pairs[i];
                while (j >= 0 && pairs[j].First >= pairToPut.First)
                {
                    int leftIndex = pairs[j].OriginIndex;
                    int rightIndex = pairToPut.OriginIndex;
                    while (this.Bytes[leftIndex] == this.Bytes[rightIndex])
                    {
                        leftIndex = (leftIndex + 1) % bytesCount;
                        rightIndex = (rightIndex + 1) % bytesCount;

                        // If all substring chars are equal
                        if (leftIndex == pairToPut.OriginIndex || rightIndex == pairs[j].OriginIndex)
                            break;
                    }

                    if (this.Bytes[leftIndex] < this.Bytes[rightIndex]) break;

                    pairs[j + 1] = pairs[j];
                    j--;
                }

                pairs[j + 1] = pairToPut;
            }
        }
    }
}
