using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JpegCompression
{
    //http://bertolami.com/index.php?engine=blog&content=posts&detail=arithmetic-coding
    //https://www.youtube.com/watch?v=9vhbKiwjJo8&list=PLE125425EC837021F&index=53
    public class ArithmeticCoder
    {
        const uint RIGHT = uint.MaxValue;
        const uint LEFT = uint.MinValue;
        const int EOF = 256;

        List<int> Alphabet { get; set; }
        List<int> SymbolsCounts { get; set; }
        List<int> CumulativeCounts { get; set; }

        public ArithmeticCoder(HashSet<byte> alphabet)
        {
            this.Alphabet = alphabet.Select(a => (int)a).OrderBy(a => a).ToList();
            this.Alphabet.Add(EOF);

            this.SymbolsCounts = CalculateCounts(this.Alphabet);
            this.CumulativeCounts = CalculateCumulativeCounts(this.SymbolsCounts);
        }

        private static List<int> CalculateCounts(List<int> alphabet)
        {
            List<int> counts = new List<int>(alphabet.Count);
            foreach (var symbol in alphabet)
            {
                counts[symbol] = 1;
            }

            return counts;
        }

        private static List<int> CalculateCumulativeCounts(List<int> symbolsCounts)
        {
            List<int> cumulativeCounts = new List<int>(symbolsCounts.Count);
            int cumulativeCount = 0;
            foreach (var symbolCount in symbolsCounts)
            {
                cumulativeCounts.Add(cumulativeCount);
                cumulativeCount += symbolCount;
            }
            cumulativeCounts.Add(cumulativeCount);

            return cumulativeCounts;
        }
    }
}
