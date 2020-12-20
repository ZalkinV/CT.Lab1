using System;
using System.Collections.Generic;
using System.Text;

namespace JpegCompression
{
    public class ArithmeticCoder
    {
        Dictionary<byte, int> SymbolsWeights { get; set; }
        Dictionary<byte, Segment> Segments { get; set; }

        public ArithmeticCoder(HashSet<byte> alphabet)
        {
            this.Segments = CreateSegments(alphabet);
            this.SymbolsWeights = CreateSymbolsWeights(alphabet);
        }

        private static Dictionary<byte, Segment> CreateSegments(HashSet<byte> alphabet)
        {
            double prob = 1.0 / alphabet.Count;
            Segment prevSegment = new Segment(0, 0);

            Dictionary<byte, Segment> segments = new Dictionary<byte, Segment>(alphabet.Count);
            foreach (var symbol in alphabet)
            {
                double newRight = prevSegment.Right + prob;
                Segment curSegment = new Segment(prevSegment.Right, newRight);
                segments[symbol] = curSegment;
                prevSegment = curSegment;
            }

            return segments;
        }

        private static Dictionary<byte, int> CreateSymbolsWeights(HashSet<byte> alphabet)
        {
            Dictionary<byte, int> weights = new Dictionary<byte, int>(alphabet.Count);
            foreach (var symbol in alphabet)
                weights[symbol] = 1;

            return weights;
        }
    }
}
