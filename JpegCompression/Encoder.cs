using Archiver.Compression;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace JpegCompression
{
    public class Encoder
    {
        public IList<byte> Bytes { get; }

        public Encoder(IList<byte> bytes)
        {
            this.Bytes = bytes;
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
            var bwtResult = BWT(this.Bytes);
            var mtfResult = MTF(bwtResult.Bytes);
            var rleResult = RLE(mtfResult);
            var arcResult = ARC(rleResult);

            int bytesCount = (int)Math.Ceiling((double)arcResult.Count / 8);
            byte[] forBits = new byte[bytesCount];
            arcResult.CopyTo(forBits, 0);

            var result = new List<byte>(bytesCount);
            result.AddRange(forBits);

            return result.ToArray();
        }

        public static BurrowsWheelerResult BWT(IList<byte> bytes)
        {
            var bwt = new BurrowsWheelerTransform(bytes);
            var bwtResult = bwt.DirectTransform();

            return bwtResult;
        }

        public static IList<byte> MTF(IList<byte> bytes)
        {
            var mtf = new MoveToFront(bytes);
            var mtfResult = mtf.Transform();

            return mtfResult;
        }

        public static IList<byte> RLE(IList<byte> bytes)
        {
            var rle = new RunLengthEncoding(bytes);
            var rleResult = rle.Encode();

            return rleResult;
        }

        public static BitArray ARC(IList<byte> bytes)
        {
            var alphabet = CreateAlphabet();
            var arc = new ArithmeticCoder(alphabet);
            BitArray encodedBits = arc.Encode(bytes);

            return encodedBits;
        }
    }
}
