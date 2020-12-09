using System;
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
        public Dictionary<string, byte> InverseCodes { get; set; }

        public Huffman()
        {
            this.Counts = new Dictionary<byte, int>();
            this.Codes = new Dictionary<byte, List<bool>>();
            this.InverseCodes = new Dictionary<string, byte>();
        }

        public void BuildCodes(Dictionary<byte, int> counts)
        {
            this.Counts = counts;
            BuildCodes();

            foreach (var codeWord in this.Codes)
            {
                string code = string.Join("", codeWord.Value.Select(b => b ? 1 : 0));
                byte value = codeWord.Key;
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
                List<bool> codeBits = this.Codes[curByte];
                bits.AddRange(codeBits);
            }

            int remainEmptyBitsCount = 8 - bits.Count % 8;
            if (remainEmptyBitsCount != 0)
            {
                IList<bool> notExistedCode = this.GetNotExistedCode(remainEmptyBitsCount);
                bits.AddRange(notExistedCode);
            }

            BitArray result = new BitArray(bits.ToArray());

            return result;
        }

        // TODO: Bad solution
        private IList<bool> GetNotExistedCode(int codeWordLength)
        {
            List<bool> shortestCode = null;
            List<bool> existedCode = null;
            foreach (var codeWord in this.Codes.Values)
            {
                if (shortestCode is null || codeWord.Count < shortestCode.Count)
                    shortestCode = codeWord;

                if (codeWord.Count == codeWordLength)
                {
                    existedCode = codeWord;
                    break;
                }
            }

            List<bool> notExistedCode;
            if (existedCode is null)
            {
                bool bitToSet = true;
                if (shortestCode.Count == 1)
                    bitToSet = !shortestCode[0];

                notExistedCode = new List<bool>(codeWordLength);
                for (int i = 0; i < codeWordLength; i++)
                    notExistedCode[i] = bitToSet;
            }
            else
            {
                notExistedCode = new List<bool>(existedCode);
                notExistedCode[notExistedCode.Count - 1] = !existedCode[notExistedCode.Count - 1];
            }

            return notExistedCode;
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

                if (this.InverseCodes.TryGetValue(currentCodeWord, out byte decodedValue))
                {
                    result.Add(decodedValue);
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
