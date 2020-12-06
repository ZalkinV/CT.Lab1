using System;
using System.Collections.Generic;
using System.Text;

namespace Archiver
{
    public class Encoder
    {
        public IList<byte> Bytes { get; }

        public Encoder(IList<byte> bytes)
        {
            this.Bytes = bytes;
        }

        public FirstLastPair[] BWT(byte[] bytes)
        {
            int bytesCount = bytes.Length;
            FirstLastPair[] pairs = new FirstLastPair[bytes.Length];
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
