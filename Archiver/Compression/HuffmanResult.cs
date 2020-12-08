using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Archiver.Compression
{
    public class HuffmanResult
    {
        public byte SymbolsCount { get; }
        public Dictionary<byte, int> BytesCounts { get; }
        public BitArray Bits { get; }

        public HuffmanResult(byte symbolsCount, Dictionary<byte, int> bytesCounts, BitArray bits)
        {
            this.SymbolsCount = symbolsCount;
            this.BytesCounts = bytesCounts;
            this.Bits = bits;
        }
    }
}
