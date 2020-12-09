﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archiver.Compression
{
    public class Huffman
    {
        public Dictionary<byte, int> Counts { get; set; }
        public Dictionary<byte, List<bool>> Codes { get; set; }

        public Huffman()
        {
            this.Counts = new Dictionary<byte, int>();
            this.Codes = new Dictionary<byte, List<bool>>();
        }

        public Huffman(Dictionary<byte, int> counts)
        {
            this.Counts = counts;
            this.Codes = new Dictionary<byte, List<bool>>();
        }

        public void BuildCodes(IList<byte> bytes)
        {
            this.Counts = CountBytes(bytes);

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

        public static Dictionary<byte, int> CountBytes(IList<byte> bytes)
        {
            int bytesCount = bytes.Count;

            Dictionary<byte, int> counts = new Dictionary<byte, int>();
            for (int i = 0; i < bytesCount; i++)
            {
                byte curByte = bytes[i];
                if (counts.ContainsKey(curByte))
                    counts[curByte]++;
                else
                    counts[curByte] = 1;
            }

            return counts;
        }

        private void AddBitToCode(HuffmanNode node, bool isLeft)
        {
            bool bitToAdd = !isLeft;
            foreach (byte symbol in node.Symbols)
            {
                if (this.Codes.ContainsKey(symbol))
                    this.Codes[symbol].Insert(0, bitToAdd);
                else
                    this.Codes[symbol] = new List<bool> { bitToAdd };
            }
        }

        public static int CompareNodes(HuffmanNode left, HuffmanNode right)
        {
            return left.Count.CompareTo(right.Count);
        }

        public BitArray Encode(IList<byte> bytes)
        {
            List<bool> bits = new List<bool>();
            for (int i = 0; i < bytes.Count; i++)
            {
                byte curByte = bytes[i];
                bits.AddRange(this.Codes[curByte]);
            }

            BitArray result = new BitArray(bits.ToArray());

            return result;
        }

        public IList<byte> Decode(BitArray bitArray)
        {
            
            return new List<byte>();
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
