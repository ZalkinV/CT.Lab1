using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archiver.Compression
{
    public class Huffman
    {
        private const int EOF_CODE = 257;

        public Dictionary<byte, int> Counts { get; set; }
        public Dictionary<int, List<bool>> Codes { get; set; }
        public Dictionary<string, int> InverseCodes { get; set; }

        public Huffman()
        {
            this.Counts = new Dictionary<byte, int>();
            this.Codes = new Dictionary<int, List<bool>>();
            this.InverseCodes = new Dictionary<string, int>();
        }

        public void BuildCodes(Dictionary<byte, int> counts)
        {
            this.Counts = counts;
            BuildCodes();

            foreach (var codeWord in this.Codes)
            {
                string code = string.Join("", codeWord.Value.Select(b => b ? 1 : 0));
                int value = codeWord.Key;
                this.InverseCodes.Add(code, value);
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
            HuffmanNode eofNode = new HuffmanNode(1, EOF_CODE);
            nodes.Add(eofNode);

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

        public void CountBytes(IList<byte> bytes)
        {
            int bytesCount = bytes.Count;
            for (int i = 0; i < bytesCount; i++)
            {
                byte curByte = bytes[i];
                if (this.Counts.ContainsKey(curByte))
                    this.Counts[curByte]++;
                else
                    this.Counts[curByte] = 1;
            }
        }

        private void AddBitToCode(HuffmanNode node, bool isLeft)
        {
            bool bitToAdd = !isLeft;
            foreach (int symbol in node.Symbols)
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
                List<bool> codeBits = this.Codes[curByte];
                bits.AddRange(codeBits);
            }
            bits.AddRange(this.Codes[EOF_CODE]);

            BitArray result = new BitArray(bits.ToArray());

            return result;
        }

        // TODO: Make more effective encoding with tree or something else
        public IList<byte> Decode(BitArray bitArray)
        {
            List<byte> result = new List<byte>();
            string currentCodeWord = string.Empty;
            for (int i = 0; i < bitArray.Count; i++)
            {
                bool currentBit = bitArray[i];
                currentCodeWord += currentBit ? "1" : "0";

                if (this.InverseCodes.TryGetValue(currentCodeWord, out int decodedValue))
                {
                    if (decodedValue == EOF_CODE)
                        break;

                    result.Add((byte)decodedValue);
                    currentCodeWord = string.Empty;
                }
            }
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
