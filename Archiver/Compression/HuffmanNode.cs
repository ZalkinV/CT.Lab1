using System;
using System.Collections.Generic;
using System.Text;

namespace Archiver.Compression
{
    public class HuffmanNode
    {
        public int Count { get; set; }
        public byte[] Symbols { get; set; }

        public HuffmanNode(int count, byte[] symbols)
        {
            this.Count = count;
            this.Symbols = symbols;
        }

        public HuffmanNode(int count, byte symbol)
            : this(
                  count: count,
                  symbols: new byte[] { symbol })
        {

        }

        public static HuffmanNode Merge(HuffmanNode left, HuffmanNode right)
        {
            int sumCount = left.Count + right.Count;
            byte[] bytes = new byte[left.Symbols.Length + right.Symbols.Length];
            Array.Copy(left.Symbols, bytes, left.Symbols.Length);
            Array.Copy(right.Symbols, 0, bytes, left.Symbols.Length, right.Symbols.Length);

            HuffmanNode node = new HuffmanNode(sumCount, bytes);
            return node;
        }

        public override string ToString()
        {
            return $"{this.Count} {string.Join(' ', this.Symbols)}";
        }
    }
}
