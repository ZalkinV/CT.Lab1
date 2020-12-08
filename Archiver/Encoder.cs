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

        public BurrowsWheelerResult BWT()
        {
            var bwt = new BurrowsWheelerTransform(this.Bytes);
            var bwtResult = bwt.DirectTransform();

            return bwtResult;
        }
    }
}
