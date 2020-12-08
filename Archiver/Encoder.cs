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

            return mtfResult;
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
    }
}
