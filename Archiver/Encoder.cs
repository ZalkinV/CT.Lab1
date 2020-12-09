using Archiver.Compression;
using System;
using System.Collections;
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

        public byte[] Encode()
        {
            var rleResult1 = RLE(this.Bytes);
            var bwtResult = BWT(rleResult1);
            var mtfResult = MTF(bwtResult.Bytes);
            var rleResult2 = RLE(mtfResult);
            var hufResult = Huffman(rleResult2);

            CompressedData compressedData = new CompressedData(
                bwtInitialStringIndex: bwtResult.InitialStringIndex,
                hufSymbolsCount: hufResult.BytesCounts.Count,
                hufBytesCounts: hufResult.BytesCounts,
                hufBits: hufResult.Bits);
            byte[] result = compressedData.ToByteArray();
            
            return result;
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
            var huf = new Huffman();
            huf.CountBytes(bytes);
            huf.BuildCodes();
            var hufResult = huf.Encode(bytes);

            var result = new HuffmanResult(
                bytesCounts: huf.Counts,
                bits: hufResult);
            return result;
        }
    }
}
