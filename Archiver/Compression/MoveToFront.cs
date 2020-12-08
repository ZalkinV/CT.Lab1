using System;
using System.Collections.Generic;
using System.Text;

namespace Archiver.Compression
{
    public class MoveToFront
    {
        public IList<byte> Bytes { get; set; }

        public MoveToFront(IList<byte> bytes)
        {
            this.Bytes = bytes;
        }

        public IList<byte> Transform()
        {
            int bytesCount = this.Bytes.Count;
            int pileOfBookCount = 256;
            byte[] pileOfBook = new byte[pileOfBookCount];
            for (int i = 0; i < pileOfBookCount; i++)
                pileOfBook[i] = (byte)i;

            byte[] result = new byte[bytesCount];
            for (int i = 0; i < bytesCount; i++)
            {
                byte curByte = this.Bytes[i];
                byte indexOfByte = (byte)Array.IndexOf(pileOfBook, curByte);
                result[i] = indexOfByte;

                for (int j = indexOfByte; j > 0; j--)
                    pileOfBook[j] = pileOfBook[j - 1];
                pileOfBook[0] = curByte;
            }

            return result;
        }
    }
}
