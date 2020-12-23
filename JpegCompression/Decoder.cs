using Archiver.Compression;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JpegCompression
{
    public class Decoder
    {
        public IList<byte> Bytes { get; }

        public Decoder(IList<byte> bytes)
        {
            this.Bytes = bytes;
        }

        public byte[] Decode()
        {
            var compressedData = new CompressedData(this.Bytes.ToArray());

            IList<byte> arcResult = ARC(compressedData.ArcBits);
            IList<byte> rleResult = RLE(arcResult);
            IList<byte> mtfResult = MTF(rleResult);
            IList<byte> bwtResult = BWT(mtfResult, compressedData.BwtInitialStringIndex);

            return bwtResult.ToArray();
        }

        public static IList<byte> ARC(BitArray bits)
        {
            var arc = new ArithmeticCoder(Encoder.CreateAlphabet());
            var arcResult = arc.Decode(bits);

            return arcResult;
        }

        public static IList<byte> RLE(IList<byte> bytes)
        {
            var rle = new RunLengthEncoding(bytes);
            var rleResult = rle.Decode();

            return rleResult;
        }

        public static IList<byte> MTF(IList<byte> bytes)
        {
            var mtf = new MoveToFront(bytes);
            var mtfResult = mtf.InverseTransform();

            return mtfResult;
        }

        public static IList<byte> BWT(IList<byte> bytes, int initialStringIndex)
        {
            var bwt = new BurrowsWheelerTransform(bytes);
            var reversedBytes = bwt.InverseTransform(initialStringIndex);

            return reversedBytes;
        }
    }
}
