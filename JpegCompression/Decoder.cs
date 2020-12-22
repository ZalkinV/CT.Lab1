using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JpegCompression
{
    public class Decoder
    {
        private HashSet<byte> Alphabet { get; set; }
        public IList<byte> Bytes { get; }

        public Decoder(IList<byte> bytes)
        {
            this.Bytes = bytes;
            this.Alphabet = Encoder.CreateAlphabet();
        }

        public byte[] Decode()
        {
            ArithmeticCoder arithmeticCoder = new ArithmeticCoder(this.Alphabet);
            var bits = new BitArray(this.Bytes.ToArray());

            var decodedBytes = arithmeticCoder.Decode(bits);

            return decodedBytes.ToArray();
        }
    }
}
