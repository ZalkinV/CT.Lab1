using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Archiver.Compression
{
    public class CompressedData
    {
        public int BwtInitialStringIndex { get; }
        public byte HufSymbolsCount { get; }
        public Dictionary<byte, int> HufBytesCounts { get; set; }
        public BitArray HufBits { get; set; }

        public CompressedData(
            int bwtInitialStringIndex,
            byte hufSymbolsCount,
            Dictionary<byte, int> hufBytesCounts,
            BitArray hufBits)
        {
            this.BwtInitialStringIndex = bwtInitialStringIndex;
            this.HufSymbolsCount = hufSymbolsCount;
            this.HufBytesCounts = hufBytesCounts;
            this.HufBits = hufBits;
        }

        public CompressedData(byte[] bytes)
        {
            int currentByteIndex = 0;
            this.BwtInitialStringIndex = BitConverter.ToInt32(bytes, currentByteIndex); currentByteIndex += 4;
            this.HufSymbolsCount = bytes[currentByteIndex]; currentByteIndex += 1;

            Dictionary<byte, int> hufBytesCount = new Dictionary<byte, int>();
            for (int i = 0; i < this.HufSymbolsCount; i++)
            {
                byte symbol = bytes[currentByteIndex]; currentByteIndex += 1;
                int symbolsCount = bytes[currentByteIndex]; currentByteIndex += 4;
                hufBytesCount[symbol] = symbolsCount;
            }
            this.HufBytesCounts = hufBytesCount;

            int bytesCountInBits = bytes.Length - currentByteIndex;
            byte[] forBits = new byte[bytesCountInBits];
            Array.Copy(bytes, currentByteIndex, forBits, 0, forBits.Length);
            this.HufBits = new BitArray(forBits);
        }

        public byte[] ToByteArray()
        {
            List<byte> result = new List<byte>();
            
            result.AddRange(BitConverter.GetBytes(this.BwtInitialStringIndex));
            result.Add(this.HufSymbolsCount);
            foreach (var byteCount in HufBytesCounts)
            {
                result.Add(byteCount.Key);
                result.AddRange(BitConverter.GetBytes(byteCount.Value));
            }

            byte[] forBits = new byte[this.HufBits.Count / 8 + 1];
            this.HufBits.CopyTo(forBits, 0);
            result.AddRange(forBits);

            return result.ToArray();
        }
    }
}
