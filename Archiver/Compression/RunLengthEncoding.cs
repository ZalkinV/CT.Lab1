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
                else if (byteCounter >= 257)
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
            byte? prevSymbol = null;
            int i = 0;
            while (i < bytesCount)
            {
                byte curSymbol = this.Bytes[i];
                if (curSymbol == prevSymbol)
                {
                    i++;
                    int symbolsCount = this.Bytes[i] + 1;
                    for (int j = 0; j < symbolsCount; j++)
                        result.Add(curSymbol);
                }
                else
                {
                    result.Add(curSymbol);
                    prevSymbol = curSymbol;
                }
                i++;
            }

            return result;
        }
    }
}
