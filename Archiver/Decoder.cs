using Archiver.Compression;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archiver
{
    public class Decoder
    {
        public IList<byte> Bytes { get; set; }

        public Decoder(IList<byte> bytes)
        {
            this.Bytes = bytes;
        }

        public byte[] Decode()
        {
            var compressedData = new CompressedData(this.Bytes.ToArray());
            var huffmanResult = new HuffmanResult(
                bytesCounts: compressedData.HufBytesCounts,
                bits: compressedData.HufBits);
            
            var hufResult = Huffman(huffmanResult);
            var rleResult = RLE(hufResult);

            return this.Bytes.ToArray();
        }

        public static IList<byte> Huffman(HuffmanResult huffmanResult)
        {
            var huf = new Huffman();
            huf.BuildCodes(huffmanResult.BytesCounts);
            var hufResult = huf.Decode(huffmanResult.Bits);

            return hufResult;
        }

        public static IList<byte> RLE(IList<byte> bytes)
        {
            var rle = new RunLengthEncoding(bytes);
            var rleResult = rle.Decode();

            return rleResult;
        }

        public static IList<byte> BWT(int initialStringIndex, IList<byte> bytes)
        {
            var bwt = new BurrowsWheelerTransform(bytes);
            var reversedBytes = bwt.InverseTransform(initialStringIndex);

            return reversedBytes;
        }
    }
}
