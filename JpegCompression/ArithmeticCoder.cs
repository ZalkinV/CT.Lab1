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

        uint Left { get; set; }
        uint Right { get; set; }
        ulong Range { get; set; }

        int RemainsBitsCount { get; set; }

        public ArithmeticCoder(HashSet<byte> alphabet)
        {
            this.Alphabet = alphabet.Select(a => (int)a).OrderBy(a => a).ToList();
            this.Alphabet.Add(EOF);

            this.SymbolsCounts = CalculateCounts(this.Alphabet);
            this.CumulativeCounts = CalculateCumulativeCounts(this.SymbolsCounts);

            this.Left = 0;
            this.Right = uint.MaxValue;

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

            var lastBits = GetLastEncodingBits();
            resultBits.AddRange(lastBits);

            BitArray result = new BitArray(resultBits.ToArray());

            return result;
        }

        public List<bool> Encode(int symbol)
        {
            this.Range = (ulong)(Right - Left) + 1;
            this.Right = GetRightBorder(symbol);
            this.Left = GetLeftBorder(symbol);

            List<bool> result = new List<bool>();
            while (SegmentHelper.IsInSameHalf(this.Left, this.Right))
            {
                var bit = this.Left >> 31 == 1 ? true : false;
                AddBit(result, bit);

                this.Left = SegmentHelper.E1E2Scale(this.Left, 0);
                this.Right = SegmentHelper.E1E2Scale(this.Right, 1);
            }
            while (SegmentHelper.IsInSecondQuarter(this.Left) && SegmentHelper.IsInThirdQuarter(this.Right))
            {
                this.Left = SegmentHelper.E3Scale(this.Left, 0);
                this.Right = SegmentHelper.E3Scale(this.Right, 1);
                this.RemainsBitsCount++;
            }

            return result;
        }

        private List<bool> GetLastEncodingBits()
        {
            List<bool> bitsToWrite = new List<bool>();

            var eofBits = Encode(EOF);
            bitsToWrite.AddRange(eofBits);

            bool bitToAdd = false;
            if (SegmentHelper.IsInFirstQuarter(this.Left) && SegmentHelper.IsInThirdQuarter(this.Right))
            {
                bitToAdd = false;
            }
            else if (SegmentHelper.IsInSecondQuarter(this.Left) && SegmentHelper.IsInForthQuarter(this.Right))
            {
                bitToAdd = true;
            }

            AddBit(bitsToWrite, bitToAdd);
            AddBit(bitsToWrite, !bitToAdd);

            return bitsToWrite;
        }

        uint Code { get; set; }
        int CurBit { get; set; }

        public byte[] Decode(BitArray bits)
        {
            InitializeCodeWithTheFirst32Bits(bits);

            List<byte> result = new List<byte>();
            while (true)
            {
                int decodedSymbol = DecodeSymbol(bits);

                if (decodedSymbol == EOF)
                    break;

                result.Add((byte)decodedSymbol);

                UpdateCount(decodedSymbol);
            }

            return result.ToArray();
        }

        private int DecodeSymbol(BitArray bits)
        {
            this.Range = (ulong)(this.Right - this.Left) + 1;
            
            ulong symbolCodePos = (ulong)(this.Code - this.Left) + 1;
            uint cumulativeCount = (uint)((symbolCodePos * (ulong)this.TotalCount - 1) / this.Range);
            var symbol = GetSymbolByCumulativeCount(cumulativeCount);

            this.Right = GetRightBorder(symbol);
            this.Left = GetLeftBorder(symbol);

            while (true)
            {
                if (SegmentHelper.IsInSameHalf(this.Left, this.Right))
                {
                    this.Left = SegmentHelper.E1E2Scale(this.Left, 0);
                    this.Right = SegmentHelper.E1E2Scale(this.Right, 1);
                }
                else if (SegmentHelper.IsInSecondQuarter(this.Left) && SegmentHelper.IsInThirdQuarter(this.Right))
                {
                    this.Left = SegmentHelper.E3Scale(this.Left, 0);
                    this.Right = SegmentHelper.E3Scale(this.Right, 1);

                    this.Code ^= 0x40000000;
                }
                else break;

                var nextBit = ReadNextBit(bits);
                this.Code = (this.Code << 1) + nextBit;
            }

            return symbol;
        }

        private int GetSymbolByCumulativeCount(uint cumulativeCount)
        {
            int lastIndex = this.CumulativeCounts.FindLastIndex(cc => cumulativeCount >= cc);

            return lastIndex;
        }

        private void InitializeCodeWithTheFirst32Bits(BitArray bits)
        {
            for (int i = 0; i < 32; i++)
            {
                this.Code = (this.Code << 1) + ReadNextBit(bits);
            }
        }

        private uint ReadNextBit(BitArray bits)
        {
            if (this.CurBit < bits.Count)
            {
                uint nextBit = (uint)(bits[this.CurBit] ? 1 : 0);
                this.CurBit++;
                return nextBit;
            }

            return 0;
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
                this.CumulativeCounts[i] = this.CumulativeCounts[i - 1] + this.SymbolsCounts[i - 1];
            }
        }

        private uint GetLeftBorder(int symbol)
        {
            uint leftBorder = (uint)(this.Left + this.Range * (ulong)this.CumulativeCounts[symbol] / (ulong)this.TotalCount);
            
            return leftBorder;
        }

        private uint GetRightBorder(int symbol)
        {
            uint rightBorder = (uint)(this.Left + this.Range * (ulong)this.CumulativeCounts[symbol + 1] / (ulong)this.TotalCount - 1);

            return rightBorder;
        }
    }
}
