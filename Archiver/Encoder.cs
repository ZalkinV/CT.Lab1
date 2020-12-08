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

        public IList<byte> BWT()
        {
            var bwt = new BurrowsWheelerTransform(this.Bytes);
            var pairs = bwt.DirectTransform();
            var lastSymbols = pairs.Select(p => p.Last).ToList();
            return lastSymbols;
        }
    }
}
