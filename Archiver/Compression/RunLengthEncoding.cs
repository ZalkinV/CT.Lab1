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

            return result;
        }
    }
}
