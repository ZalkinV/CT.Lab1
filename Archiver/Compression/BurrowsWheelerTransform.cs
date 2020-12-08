using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archiver.Compression
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

        //http://neerc.ifmo.ru/wiki/index.php?title=%D0%9F%D1%80%D0%B5%D0%BE%D0%B1%D1%80%D0%B0%D0%B7%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5_%D0%91%D0%B0%D1%80%D1%80%D0%BE%D1%83%D0%B7%D0%B0-%D0%A3%D0%B8%D0%BB%D0%B5%D1%80%D0%B0
        public IList<byte> InverseTransform(int initialStringIndex)
        {
            int countsLength = 256;
            int bytesCount = this.Bytes.Count;
            int[] counts = new int[countsLength];
            for (int i = 0; i < bytesCount; i++)
            {
                byte byteCode = this.Bytes[i];
                counts[byteCode]++;
            }

            int sum = 0;
            for (int i = 0; i < countsLength; i++)
            {
                sum += counts[i];
                counts[i] = sum - counts[i];
            }

            int[] forInvertTransform = new int[bytesCount];
            for (int i = 0; i < bytesCount; i++)
            {
                byte byteCode = this.Bytes[i];
                int countOfByte = counts[byteCode];
                forInvertTransform[countOfByte] = i;
                counts[byteCode]++;
            }

            byte[] result = new byte[bytesCount];
            int currentByteIndex = forInvertTransform[initialStringIndex];
            for (int i = 0; i < bytesCount; i++)
            {
                byte byteCode = this.Bytes[currentByteIndex];
                result[i] = byteCode;
                currentByteIndex = forInvertTransform[currentByteIndex];
            }

            return result;
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
