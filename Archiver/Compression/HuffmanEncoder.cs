using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archiver.Compression
{
    public class HuffmanEncoder
    {
        public IList<byte> Bytes { get; set; }

        public IList<int> Counts { get; set; }
        public Dictionary<byte, int> Codes { get; set; }

        public HuffmanEncoder(IList<byte> bytes)
        {
            this.Bytes = bytes;
            this.Counts = new int[256];
            this.Codes = new Dictionary<byte, int>();
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
            nodes.Sort(CompareNodes);
        }

        public static int CompareNodes(HuffmanNode left, HuffmanNode right)
        {
            return -left.Count.CompareTo(right.Count);
        }

        public IList<byte> Encode()
        {
            return this.Bytes;
        }
    }
}
