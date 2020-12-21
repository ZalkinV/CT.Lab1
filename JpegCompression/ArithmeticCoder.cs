using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JpegCompression
{
    //https://neerc.ifmo.ru/wiki/index.php?title=%D0%90%D1%80%D0%B8%D1%84%D0%BC%D0%B5%D1%82%D0%B8%D1%87%D0%B5%D1%81%D0%BA%D0%BE%D0%B5_%D0%BA%D0%BE%D0%B4%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5
    //http://bertolami.com/index.php?engine=blog&content=posts&detail=arithmetic-coding
    public class ArithmeticCoder
    {
        Dictionary<byte, int> Weights { get; set; }
        int WeightsSum { get; set; }
        Dictionary<byte, Segment> Segments { get; set; }

        public ArithmeticCoder(HashSet<byte> alphabet)
        {
            this.Weights = CreateWeights(alphabet);
            this.WeightsSum = this.Weights.Sum(w => w.Value);
            this.Segments = CreateSegments(alphabet);
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

        private static Dictionary<byte, int> CreateWeights(HashSet<byte> alphabet)
        {
            Dictionary<byte, int> weights = new Dictionary<byte, int>(alphabet.Count);
            foreach (var symbol in alphabet)
                weights[symbol] = 1;

            return weights;
        }

        private void ResizeSegments()
        {
            double left = 0;
            int weightsSum = this.WeightsSum;

            foreach ((byte symbol, Segment segment) in this.Segments)
            {
                double segmentLength = (double)this.Weights[symbol] / weightsSum;
                segment.Left = left;
                segment.Right = left + segmentLength;
                left = segment.Right;
            }
        }

        public double Encode(IList<byte> bytes)
        {
            double left = 0;
            double right = 1;
            for (int i = 0; i < bytes.Count; i++)
            {
                byte curByte = bytes[i];
                this.Weights[curByte]++;
                this.WeightsSum++;
                double newLeft = left + (right - left) * this.Segments[curByte].Left;
                double newRight = left + (right - left) * this.Segments[curByte].Right;
                left = newLeft;
                right = newRight;
                ResizeSegments();
            }

            double code = (left + right) / 2;
            return code;
        }
    }
}
