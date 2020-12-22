using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JpegCompression
{
    public class Encoder
    {
        private HashSet<byte> Alphabet { get; set; }
        public IList<byte> Bytes { get; }

        public Encoder(IList<byte> bytes)
        {
            this.Bytes = bytes;
            this.Alphabet = CreateAlphabet();
        }

        public static HashSet<byte> CreateAlphabet()
        {
            byte[] alphabet = new byte[256];
            for (int i = 0; i < alphabet.Length; i++)
            {
                alphabet[i] = (byte)i;
            }
            var result = new HashSet<byte>(alphabet);

            return result;
        }

        public byte[] Encode()
        {
            ArithmeticCoder arithmeticCoder = new ArithmeticCoder(this.Alphabet);
            BitArray encodedBits = arithmeticCoder.Encode(this.Bytes);

            int bytesCount = (int)Math.Ceiling((double)encodedBits.Count / 8);
            byte[] forBits = new byte[bytesCount];
            encodedBits.CopyTo(forBits, 0);

            var result = new List<byte>(bytesCount);
            result.AddRange(forBits);

            return result.ToArray();
        }
    }
}
