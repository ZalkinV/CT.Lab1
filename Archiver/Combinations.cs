using System;
using System.Collections.Generic;
using System.Text;

namespace Archiver
{
    public struct Comb2
    {
        public byte Current;
        public byte Previous;

        public Comb2(byte current, byte previous)
        {
            this.Current = current;
            this.Previous = previous;
        }

        public override bool Equals(object obj)
        {
            return obj is Comb2 comb &&
                   this.Current == comb.Current &&
                   this.Previous == comb.Previous;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Current, this.Previous);
        }

        public override string ToString()
        {
            return $"{Previous} {Current}";
        }
    }

    public struct Comb3
    {
        public byte Current;
        public byte Previous;
        public byte PPrevious;

        public Comb3(byte current, byte previous, byte pPrevious)
        {
            this.Current = current;
            this.Previous = previous;
            this.PPrevious = pPrevious;
        }

        public override string ToString()
        {
            return $"{PPrevious} {Previous} {Current}";
        }
    }
}
