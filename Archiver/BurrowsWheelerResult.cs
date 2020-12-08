using System;
using System.Collections.Generic;
using System.Text;

namespace Archiver
{
    public class BurrowsWheelerResult
    {
        public IList<byte> Bytes { get; set; }
        public int InitialStringIndex { get; set; }

        public BurrowsWheelerResult(IList<byte> bytes, int initialStringIndex)
        {
            this.Bytes = bytes;
            this.InitialStringIndex = initialStringIndex;
        }
    }
}
