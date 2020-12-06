using System;
using System.Collections.Generic;
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

        public FirstLastPair[] BWT(byte[] bytes)
        {
            var unsortedPairs = BurrowsWheelerTransform.GetAllShifts(bytes);
            return unsortedPairs;
        }
    }
}
