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

        public IList<int> Counts { get; }
        public Dictionary<byte, List<bool>> Codes { get; set; }

        public HuffmanEncoder(IList<byte> bytes)
        {
            this.Bytes = bytes;
            this.Counts = new int[256];
            this.Codes = new Dictionary<byte, List<bool>>();
        }

        public void Count()
        {
            int bytesCount = this.Bytes.Count;
            for (int i = 0; i < bytesCount; i++)
            {
                byte curByte = this.Bytes[i];
                this.Counts[curByte]++;
            }
        }

        public void BuildCodes()
        {
            int countsLength = this.Counts.Count;
            List<HuffmanNode> nodes = new List<HuffmanNode>(countsLength);
            for (int i = 0; i < countsLength; i++)
            {
                byte curByte = (byte)i;
                int curCount = this.Counts[i];
                if (curCount != 0)
                {
                    HuffmanNode node = new HuffmanNode(curCount, curByte);
                    nodes.Add(node);
                }
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
    }
}
