using Archiver.Compression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archiver
{
    public class Encoder
    {
        public IList<byte> Bytes { get; }

        public Encoder(IList<byte> bytes)
        {
            this.Bytes = bytes;
        }

        public IList<byte> Encode()
        {
            var bwtResult = BWT(this.Bytes);
            var mtfResult = MTF(bwtResult.Bytes);
            var rleResult = RLE(mtfResult);

            return rleResult;
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

        public static HuffmanResult Huffman(IList<byte> bytes)
        {
            var huf = new HuffmanEncoder(bytes);
            huf.Count();
            huf.BuildCodes();
            var hufResult = huf.Encode();

            var result = new HuffmanResult(
                symbolsCount: (byte)huf.Codes.Count,
                bytesCounts: huf.Counts,
                bits: hufResult);
            return result;
        }
    }
}
