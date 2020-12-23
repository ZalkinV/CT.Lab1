using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JpegCompression
{
    public class CompressedData
    {
        public int BwtInitialStringIndex { get; }
        public BitArray ArcBits { get; set; }

        public CompressedData(
            int bwtInitialStringIndex,
            BitArray arcBits)
        {
            this.BwtInitialStringIndex = bwtInitialStringIndex;
            this.ArcBits = arcBits;
        }

        public CompressedData(byte[] bytes)
        {
            int currentByteIndex = 0;
            this.BwtInitialStringIndex = BitConverter.ToInt32(bytes, currentByteIndex); currentByteIndex += 4;

            int bytesCountInBits = bytes.Length - currentByteIndex;
            byte[] forBits = new byte[bytesCountInBits];
            Array.Copy(bytes, currentByteIndex, forBits, 0, forBits.Length);
            this.ArcBits = new BitArray(forBits);
        }

        public byte[] ToByteArray()
        {
            List<byte> result = new List<byte>();

            result.AddRange(BitConverter.GetBytes(this.BwtInitialStringIndex));

            int bytesCount = (int)Math.Ceiling((double)this.ArcBits.Count / 8);
            byte[] forBits = new byte[bytesCount];
            this.ArcBits.CopyTo(forBits, 0);
            result.AddRange(forBits);

            return result.ToArray();
        }
    }
}
