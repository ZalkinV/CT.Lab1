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
            byte runLengthByte = this.Bytes[0];
            int byteCounter = 0;
            for (int i = 0; i < bytesCount; i++)
            {
                byte curByte = this.Bytes[i];
                if (curByte == runLengthByte)
                {
                    byteCounter++;
                }
                else
                {
                    if (byteCounter >= 2)
                        result.Add((byte)(byteCounter - 2));

                    byteCounter = 1;
                    runLengthByte = curByte;
                }

                if (byteCounter <= 2)
                {
                    result.Add(runLengthByte);
                }
                else if (byteCounter >= 5)
                {
                    result.Add((byte)(byteCounter - 2));
                    byteCounter = 0;
                }
            }
            if (byteCounter >= 2)
                result.Add((byte)(byteCounter - 2));

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
