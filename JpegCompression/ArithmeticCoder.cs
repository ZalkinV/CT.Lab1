using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JpegCompression
{
    //http://bertolami.com/index.php?engine=blog&content=posts&detail=arithmetic-coding
    //https://www.youtube.com/watch?v=9vhbKiwjJo8&list=PLE125425EC837021F&index=53
    public class ArithmeticCoder
    {
        const int EOF = 256;

        List<int> Alphabet { get; set; }
        List<int> SymbolsCounts { get; set; }
        List<int> CumulativeCounts { get; set; }
        int TotalCount => this.CumulativeCounts.Last();

        int Left { get; set; }
        int Right { get; set; }
        int Range => Right - Left;

        const int Half = int.MaxValue / 2;
        const int Quarter = int.MaxValue / 4;
        const int ThirdQuarter = 3 * Quarter;

        int RemainsBitsCount { get; set; }

        public ArithmeticCoder(HashSet<byte> alphabet)
        {
            this.Alphabet = alphabet.Select(a => (int)a).OrderBy(a => a).ToList();
            this.Alphabet.Add(EOF);

            this.SymbolsCounts = CalculateCounts(this.Alphabet);
            this.CumulativeCounts = CalculateCumulativeCounts(this.SymbolsCounts);

            this.Left = 0;
            this.Right = int.MaxValue;

            this.RemainsBitsCount = 0;
        }

        public BitArray Encode(IList<byte> symbols)
        {
            List<bool> resultBits = new List<bool>();
            foreach (byte symbol in symbols)
            {
                List<bool> symbolBits = Encode(symbol);
                resultBits.AddRange(symbolBits);

                UpdateCount(symbol);
            }

            var eofBits = Encode(EOF);
            resultBits.AddRange(eofBits);

            BitArray result = new BitArray(resultBits.ToArray());

            return result;
        }

        public List<bool> Encode(int symbol)
        {
            this.Left = GetLeftBorder(symbol);
            this.Right = GetRightBorder(symbol);

            List<bool> result = new List<bool>();
            while (Half < this.Left|| this.Right < Half)
            {
                if (this.Right < Half) // E1
                {
                    AddBit(result, false);
                    this.Left *= 2;
                    this.Right *= 2;
                }
                else if (Half < this.Left) // E2
                {
                    AddBit(result, true);
                    this.Left = 2 * (this.Left - Half);
                    this.Right = 2 * (this.Right - Half);
                }    
            }

            while (Quarter < this.Left && this.Right < ThirdQuarter) // E3
            {
                this.RemainsBitsCount++;
                this.Left = 2 * (this.Left - Quarter);
                this.Right = 2 * (this.Right - Quarter);
            }

            return result;
        }

        private void AddBit(List<bool>bits, bool bit)
        {
            bits.Add(bit);
            while (this.RemainsBitsCount > 0)
            {
                bits.Add(!bit);
                this.RemainsBitsCount--;
            }
        }

        private static List<int> CalculateCounts(List<int> alphabet)
        {
            List<int> counts = new List<int>(alphabet.Count);
            foreach (var symbol in alphabet)
            {
                counts.Add(1);
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

        private void UpdateCount(int symbol)
        {
            this.SymbolsCounts[symbol]++;

            for (int i = symbol + 1; i < this.CumulativeCounts.Count; i++)
            {
                this.CumulativeCounts[i] = this.CumulativeCounts[i - 1] + this.CumulativeCounts[i - 1];
            }
        }

        private int GetLeftBorder(int symbol)
        {
            int leftBorder = this.Left + (int)(this.Range * (long)this.CumulativeCounts[symbol] / this.TotalCount);
            
            return leftBorder;
        }

        private int GetRightBorder(int symbol)
        {
            int rightBorder = this.Left + (int)(this.Range * (long)this.CumulativeCounts[symbol - 1] / this.TotalCount);

            return rightBorder;
        }
    }
}
