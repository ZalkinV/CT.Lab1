using System;
using System.Collections.Generic;
using System.Text;

namespace Archiver
{
    public struct FirstLastPair
    {
        public byte First;
        public byte Last;
        public int OriginIndex;

        public FirstLastPair(byte first, byte last, int originIndex)
        {
            this.First = first;
            this.Last = last;
            this.OriginIndex = originIndex;
        }

        public override bool Equals(object obj)
        {
            return obj is FirstLastPair pair &&
                   this.First == pair.First &&
                   this.Last == pair.Last &&
                   this.OriginIndex == pair.OriginIndex;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.First, this.Last, this.OriginIndex);
        }

        public override string ToString()
        {
            return $"{(char)First} {(char)Last} {OriginIndex}";
        }
    }
}
