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
            //this.FirstOrderProbabilities = CalculateFirstOrderProbabilities();
            var a = CalculateFirstOrderProbabilities();
            //this.SecondOrderProbabilities = CalculateSecondOrderProbabilities();
            this.ZeroOrderEntropy = CalculateZeroOrderEntropy();
            this.FirstOrderEntropy = CalculateFirstOrderEntropy();
            //this.SecondOrderEntropy = CalculateSecondOrderEntropy();
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
            Dictionary<Comb2, int> bytesCounts = new Dictionary<Comb2, int>(256 * 256);
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

        //private double[,] CalculateFirstOrderProbabilities()
        //{
        //    int bytesCount = this.Bytes.Count;
        //    int[,] bytesCounts = new int[BYTES_COUNT, BYTES_COUNT];
        //    for (int i = 1; i < bytesCount; i++)
        //    {
        //        int byteValue = this.Bytes[i];
        //        int prevByteValue = this.Bytes[i - 1];
        //        bytesCounts[byteValue, prevByteValue]++;
        //    }


        //    double[,] probabilities = new double[BYTES_COUNT, BYTES_COUNT];
        //    for (int i = 0; i < BYTES_COUNT; i++)
        //    {
        //        int combinationsCount = 0;
        //        for (int ic = 0; ic < BYTES_COUNT; ic++)
        //            combinationsCount += bytesCounts[i, ic];

        //        for (int j = 0; j < BYTES_COUNT; j++)
        //        {
        //            probabilities[i, j] = (double)bytesCounts[i, j] / combinationsCount;
        //        }
        //    }

        //    return probabilities;
        //}

        //private double[,,] CalculateSecondOrderProbabilities()
        //{
        //    int bytesCount = this.Bytes.Count;
        //    int[,,] bytesCounts = new int[BYTES_COUNT, BYTES_COUNT, BYTES_COUNT];
        //    for (int i = 2; i < bytesCount; i++)
        //    {
        //        int byteValue = this.Bytes[i];
        //        int prevByteValue = this.Bytes[i - 1];
        //        int pprevByteValue = this.Bytes[i - 2];
        //        bytesCounts[byteValue, prevByteValue, pprevByteValue]++;
        //    }

        //    double[,,] probabilities = new double[BYTES_COUNT, BYTES_COUNT, BYTES_COUNT];
        //    for (int i = 0; i < BYTES_COUNT; i++)
        //    {
        //        for (int j = 0; j < BYTES_COUNT; j++)
        //        {
        //            int combinationsCount = 0;
        //            for (int ijc = 0; ijc < BYTES_COUNT; ijc++)
        //                combinationsCount += bytesCounts[i, j, ijc];

        //            for (int k = 0; k < BYTES_COUNT; k++)
        //            {
        //                probabilities[i, j, k] = (double)bytesCounts[i, j, k] / combinationsCount;
        //            }
        //        }
        //    }

        //    return probabilities;
        //}

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
                entropy += prevByteProb * byteProb.Value * Math.Log2(prevByteProb);
            }
            //for (int i = 0; i < this.FirstOrderProbabilities.Length; i++)
            //{
            //    for (int j = 0; j < this.FirstOrderProbabilities.Length; j++)
            //    {
            //        double bytesProb = this.FirstOrderProbabilities[i, j];
            //        if (bytesProb != 0)
            //        {
            //            double prevProb = this.ZeroOrderProbabilities[j];
            //            entropy += prevProb * bytesProb * Math.Log2(bytesProb);
            //        }
            //    }

            //}
            entropy *= -1;

            return entropy;
        }

        //private double CalculateSecondOrderEntropy()
        //{
        //    return 0;
        //}
    }
}
