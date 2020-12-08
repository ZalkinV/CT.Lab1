using Archiver.Compression;
using System;
using System.Collections.Generic;
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

        public IList<byte> Decode()
        {
            return this.Bytes;
        }

        public IList<byte> BWT(int initialStringIndex)
        {
            var bwt = new BurrowsWheelerTransform(this.Bytes);
            var reversedBytes = bwt.InverseTransform(initialStringIndex);

            return reversedBytes;
        }
    }
}
