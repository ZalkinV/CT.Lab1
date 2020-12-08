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
            int bytesCount = this.Bytes.Count;

            int[] firstByteIndexes = new int[bytesCount];
            for (int i = 0; i < firstByteIndexes.Length; i++)
                firstByteIndexes[i] = i;

            Array.Sort(firstByteIndexes, CompareShifts);
            int initialStringIndex = Array.IndexOf(firstByteIndexes, 0);

            byte[] lastBytes = new byte[bytesCount];
            for (int i = 0; i < firstByteIndexes.Length; i++)
            {
                int firstByteIndex = firstByteIndexes[i];
                int lastByteIndex = (firstByteIndex + bytesCount - 1) % bytesCount;
                lastBytes[i] = this.Bytes[lastByteIndex];
            }

            var bwtResult = new BurrowsWheelerResult(lastBytes, initialStringIndex);

            return bwtResult;
        }

        private int CompareShifts(int indexOfLeft, int indexOfRight)
        {
            if (indexOfLeft == indexOfRight) return 0;

            int byteCompResult = this.Bytes[indexOfLeft].CompareTo(this.Bytes[indexOfRight]);
            
            int newIndexOfLeft = indexOfLeft;
            int newIndexOfRight = indexOfRight;
            while (byteCompResult == 0)
            {
                newIndexOfLeft = (newIndexOfLeft + 1) % this.Bytes.Count;
                newIndexOfRight = (newIndexOfRight + 1) % this.Bytes.Count;

                byteCompResult = this.Bytes[newIndexOfLeft].CompareTo(this.Bytes[newIndexOfRight]);
                // If all substring chars are equal and comparison doesn't matter
                if (newIndexOfLeft == indexOfLeft || newIndexOfRight == indexOfRight)
                    return 0;
            }

            return byteCompResult;
        }
    }
}
