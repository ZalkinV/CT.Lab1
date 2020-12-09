﻿using Archiver.Compression;
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

            return this.Bytes.ToArray();
        }

        public static IList<byte> Huffman(HuffmanResult huffmanResult)
        {
            Huffman huf = new Huffman();
            huf.BuildCodes(huffmanResult.BytesCounts);
            var hufResult = huf.Decode(huffmanResult.Bits);


            return hufResult;
        }

        public static IList<byte> BWT(int initialStringIndex, IList<byte> bytes)
        {
            var bwt = new BurrowsWheelerTransform(bytes);
            var reversedBytes = bwt.InverseTransform(initialStringIndex);

            return reversedBytes;
        }
    }
}
