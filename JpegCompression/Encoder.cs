using System;
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

        private static HashSet<byte> CreateAlphabet()
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
            byte[] result = new byte[0];

            return result;
        }
    }
}
