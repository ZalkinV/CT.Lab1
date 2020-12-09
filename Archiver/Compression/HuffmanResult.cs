using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Archiver.Compression
{
    public class HuffmanResult
    {
        public Dictionary<byte, int> BytesCounts { get; }
        public BitArray Bits { get; }

        public HuffmanResult(Dictionary<byte, int> bytesCounts, BitArray bits)
        {
            this.BytesCounts = bytesCounts;
            this.Bits = bits;
        }
    }
}
