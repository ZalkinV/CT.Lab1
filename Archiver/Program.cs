using System;
using System.IO;

namespace Archiver
{
    class Program
    {
        private const string INF_MODE = "i";
        private const string ENC_MODE = "e";
        private const string DEC_MODE = "d";
        private const string ARCHIVE_EXTENSION = ".zva";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                    args = new string[] { "e", "calgarycorpus/test.txt" };

                string mode = args[0];
                string filename = args[1];
                
                byte[] bytesFromFile = File.ReadAllBytes(filename);
                switch (mode)
                {
                    case INF_MODE:
                        CalculateEntropies(bytesFromFile);
                        break;
                    case ENC_MODE:
                        Encode(filename, bytesFromFile);
                        break;
                    case DEC_MODE:
                        break;
                    default:
                        Console.WriteLine(
                            $"Unknown mode '{mode}' was selected, try one of the following:\n" +
                            $"{INF_MODE} — info mode\n" +
                            $"{ENC_MODE} — encoding mode\n" +
                            $"{DEC_MODE} — decoding mode\n");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void CalculateEntropies(byte[] bytesFromFile)
        {
            EntropyCalculator entropyCalculator = new EntropyCalculator(bytesFromFile);
            Console.WriteLine($"0-th order entropy H(X) = {entropyCalculator.ZeroOrderEntropy}");
            Console.WriteLine($"1-th order entropy H(X|X) = {entropyCalculator.FirstOrderEntropy}");
            Console.WriteLine($"2-th order entropy H(X|XX) = {entropyCalculator.SecondOrderEntropy}");
        }

        public static void Encode(string filename, byte[] bytesFromFile)
        {
            Encoder encoder = new Encoder(bytesFromFile);
            byte[] encodedBytes = encoder.Encode();

            string newFilename = filename + ARCHIVE_EXTENSION;
            File.WriteAllBytes(newFilename, encodedBytes);

            Console.WriteLine($"File '{filename}' was encrypted as '{newFilename}'");
        }
    }
}
