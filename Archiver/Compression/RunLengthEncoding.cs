using System;
using System.Collections.Generic;
using System.Text;

namespace Archiver.Compression
{
    public class RunLengthEncoding
    {
        public IList<byte> Bytes { get; set; }

        public RunLengthEncoding(IList<byte> bytes)
        {
            this.Bytes = bytes;
        }

        public IList<byte> Encode()
        {
            int bytesCount = this.Bytes.Count;

            List<byte> result = new List<byte>(bytesCount);
            byte prevByte = this.Bytes[0];
            byte byteCounter = 0;
            for (int i = 0; i < bytesCount; i++)
            {
                byte curByte = this.Bytes[i];
                if (curByte == prevByte && byteCounter < 255)
                {
                    byteCounter++;
                }
                else
                {
                    result.Add(prevByte);
                    result.Add(byteCounter);

                    byteCounter = 1;
                    prevByte = curByte;
                }
            }
            result.Add(prevByte);
            result.Add(byteCounter);

            return result;
        }

        public IList<byte> Decode()
        {
            int bytesCount = this.Bytes.Count;

            List<byte> result = new List<byte>(bytesCount);
            for (int i = 0; i < bytesCount; i += 2)
            {
                byte symbol = this.Bytes[i];
                byte symbolCount = this.Bytes[i + 1];
                for (int j = 0; j < symbolCount; j++)
                    result.Add(symbol);
            }

            return result;
        }
    }
}
