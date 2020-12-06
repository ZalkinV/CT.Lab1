using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Archiver
{
    public class EntropyCalculator
    {
        public IList<byte> Bytes { get; }

        public Dictionary<byte, double> ZeroOrderProbabilities { get; }
        public Dictionary<Comb2, double> FirstOrderProbabilities { get; }
        public Dictionary<Comb3, double> SecondOrderProbabilities { get; }

        public double ZeroOrderEntropy { get; }
        public double FirstOrderEntropy { get; }
        public double SecondOrderEntropy { get; }

        public EntropyCalculator(IList<byte> bytes)
        {
            this.Bytes = bytes;
            this.ZeroOrderProbabilities = CalculateZeroOrderProbabilities();
            this.FirstOrderProbabilities = CalculateFirstOrderProbabilities();
            this.SecondOrderProbabilities = CalculateSecondOrderProbabilities();
            this.ZeroOrderEntropy = CalculateZeroOrderEntropy();
            this.FirstOrderEntropy = CalculateFirstOrderEntropy();
            this.SecondOrderEntropy = CalculateSecondOrderEntropy();
        }

        private Dictionary<byte, double> CalculateZeroOrderProbabilities()
        {
            int bytesCount = this.Bytes.Count;
            Dictionary<byte, int> bytesCounts = new Dictionary<byte, int>();
            for (int i = 0; i < bytesCount; i++)
            {
                byte byteValue = this.Bytes[i];
                if (!bytesCounts.ContainsKey(byteValue))
                    bytesCounts[byteValue] = 0;
                bytesCounts[byteValue]++;
            }

            Dictionary<byte, double> probabilities = new Dictionary<byte, double>();
            foreach (var byteCount in bytesCounts)
            {
                probabilities[byteCount.Key] = (double)byteCount.Value / bytesCount;
            }

            return probabilities;
        }

        private Dictionary<Comb2, double> CalculateFirstOrderProbabilities()
        {
            int bytesCount = this.Bytes.Count;
            Dictionary<Comb2, int> bytesCounts = new Dictionary<Comb2, int>();
            for (int i = 1; i < bytesCount; i++)
            {
                Comb2 comb = new Comb2(this.Bytes[i], this.Bytes[i - 1]);
                if (!bytesCounts.ContainsKey(comb))
                    bytesCounts[comb] = 0;
                bytesCounts[comb]++;
            }


            Dictionary<Comb2, double> probabilities = new Dictionary<Comb2, double>();
            Dictionary<byte, int> prevByteCombinationsCount = new Dictionary<byte, int>();
            foreach (var byteCount in bytesCounts)
            {
                byte prevByte = byteCount.Key.Previous;
                int combinationsCount = 0;
                if (!prevByteCombinationsCount.ContainsKey(prevByte))
                {
                    combinationsCount = bytesCounts.Where(x => x.Key.Previous == prevByte).Sum(x => x.Value);
                    prevByteCombinationsCount[prevByte] = combinationsCount;
                }
                else
                {
                    combinationsCount = prevByteCombinationsCount[prevByte];
                }

                probabilities[byteCount.Key] = (double)byteCount.Value / combinationsCount;
            }

            return probabilities;
        }

        private Dictionary<Comb3, double> CalculateSecondOrderProbabilities()
        {
            int bytesCount = this.Bytes.Count;
            Dictionary<Comb3, int> bytesCounts = new Dictionary<Comb3, int>();
            for (int i = 2; i < bytesCount; i++)
            {
                Comb3 comb = new Comb3(this.Bytes[i], this.Bytes[i - 1], this.Bytes[i - 2]);
                if (!bytesCounts.ContainsKey(comb))
                    bytesCounts[comb] = 0;
                bytesCounts[comb]++;
            }


            Dictionary<Comb3, double> probabilities = new Dictionary<Comb3, double>();
            Dictionary<Comb2, int> prevByteCombinationsCount = new Dictionary<Comb2, int>();
            foreach (var byteCount in bytesCounts)
            {
                Comb2 prevBytes = new Comb2(byteCount.Key.Previous, byteCount.Key.PPrevious);
                int combinationsCount = 0;
                if (!prevByteCombinationsCount.ContainsKey(prevBytes))
                {
                    combinationsCount = bytesCounts.Where(x => new Comb2(x.Key.Previous, x.Key.PPrevious).Equals(prevBytes)).Sum(x => x.Value);
                    prevByteCombinationsCount[prevBytes] = combinationsCount;
                }
                else
                {
                    combinationsCount = prevByteCombinationsCount[prevBytes];
                }

                probabilities[byteCount.Key] = (double)byteCount.Value / combinationsCount;
            }

            return probabilities;
        }

        private double CalculateZeroOrderEntropy()
        {
            double entropy = 0;
            foreach (var probability in this.ZeroOrderProbabilities)
            {
                entropy += probability.Value * Math.Log2(probability.Value);
            }
            entropy *= -1;

            return entropy;
        }

        private double CalculateFirstOrderEntropy()
        {
            double entropy = 0;
            foreach (var byteProb in this.FirstOrderProbabilities)
            {
                double prevByteProb = this.ZeroOrderProbabilities[byteProb.Key.Previous];
                entropy += prevByteProb * byteProb.Value * Math.Log2(byteProb.Value);
            }
            
            entropy *= -1;

            return entropy;
        }

        private double CalculateSecondOrderEntropy()
        {
            double entropy = 0;
            foreach (var byteProb in this.SecondOrderProbabilities)
            {
                Comb2 prevBytes = new Comb2(byteProb.Key.Previous, byteProb.Key.PPrevious);
                double pprevByteProb = this.ZeroOrderProbabilities[prevBytes.Previous];
                double prevBytesProb = this.FirstOrderProbabilities[prevBytes];
                entropy += pprevByteProb * prevBytesProb * byteProb.Value * Math.Log2(byteProb.Value);
            }

            entropy *= -1;

            return entropy;
        }
    }
}
