using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archiver.Compression
{
    public class HuffmanEncoder
    {
        public IList<byte> Bytes { get; }

        public Dictionary<byte, int> Counts { get; }
        public Dictionary<byte, List<bool>> Codes { get; set; }

        public HuffmanEncoder(IList<byte> bytes)
        {
            this.Bytes = bytes;
            this.Counts = new Dictionary<byte, int>();
            this.Codes = new Dictionary<byte, List<bool>>();
        }

        public void Count()
        {
            int bytesCount = this.Bytes.Count;
            for (int i = 0; i < bytesCount; i++)
            {
                byte curByte = this.Bytes[i];
                if (this.Counts.ContainsKey(curByte))
                    this.Counts[curByte]++;
                else
                    this.Counts[curByte] = 1;
            }
        }

        public void BuildCodes()
        {
            List<HuffmanNode> nodes = new List<HuffmanNode>(this.Counts.Count);
            foreach (var count in this.Counts)
            {
                HuffmanNode node = new HuffmanNode(count.Value, count.Key);
                nodes.Add(node);
            }

            while (nodes.Count > 1)
            {
                nodes.Sort(CompareNodes);
                HuffmanNode left = nodes[0];
                HuffmanNode right = nodes[1];

                AddBitToCode(left, true);
                AddBitToCode(right, false);

                var mergedNode = HuffmanNode.Merge(left, right);

                nodes.RemoveAt(0);
                nodes[0] = mergedNode;
            }
        }

        private void AddBitToCode(HuffmanNode node, bool isLeft)
        {
            bool bitToAdd = !isLeft;
            foreach (byte symbol in node.Symbols)
            {
                if (this.Codes.ContainsKey(symbol))
                    this.Codes[symbol].Add(bitToAdd);
                else
                    this.Codes[symbol] = new List<bool> { bitToAdd };
            }
        }

        public static int CompareNodes(HuffmanNode left, HuffmanNode right)
        {
            return left.Count.CompareTo(right.Count);
        }

        public BitArray Encode()
        {
            List<bool> bits = new List<bool>();
            for (int i = 0; i < this.Bytes.Count; i++)
            {
                byte curByte = this.Bytes[i];
                bits.AddRange(this.Codes[curByte]);
            }

            BitArray result = new BitArray(bits.ToArray());

            return result;
        }

        public void PrintCodes()
        {
            var codes = this.Codes
                .ToDictionary(
                    c => c.Key,
                    c => string.Join("", c.Value.Select(b => b ? 1 : 0)))
                .OrderBy(x => x.Key);

            Console.WriteLine("Huffman tree codes:");
            foreach (var code in codes)
            {
                Console.WriteLine($"{code.Key} {code.Value}");
            }
        }
    }
}
