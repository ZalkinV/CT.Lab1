using System;
using System.Diagnostics;
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
#if DEBUG
                args = new string[] { "i", "calgarycorpus/" };
#endif

                if (args.Length == 0)
                {
                    args = new string[2];
                    Console.Write("Select mode: ");
                    args[0] = Console.ReadLine();
                    Console.Write("Select path: ");
                    args[1] = Console.ReadLine();
                    Console.WriteLine();
                }
                
                string mode = args[0];
                string path = args[1];
                string dir = Path.GetDirectoryName(path);
                string pattern = Path.GetFileName(path);
                string[] filesNames = Directory.GetFiles(dir, pattern);

                if (filesNames.Length == 0)
                    throw new ArgumentException($"No such files was found!");

                foreach (string filename in filesNames)
                {
                    byte[] bytesFromFile = File.ReadAllBytes(filename);
                    switch (mode)
                    {
                        case INF_MODE:
                            CalculateEntropies(filename, bytesFromFile);
                            break;
                        case ENC_MODE:
                            Encode(filename, bytesFromFile);
                            break;
                        case DEC_MODE:
                            try
                            {
                                Decode(filename, bytesFromFile);
                            }
                            catch (ArgumentException e)
                            {
                                Console.WriteLine(e.Message);
                            }
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
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void CalculateEntropies(string filename, byte[] bytesFromFile)
        {
            string PrepareDouble(double value) => Math.Round(value, 3).ToString("F3");

            Console.WriteLine($"Entropies for '{filename}':");
            EntropyCalculator entropyCalculator = new EntropyCalculator(bytesFromFile);
            Console.WriteLine($"H(X) = {PrepareDouble(entropyCalculator.ZeroOrderEntropy)}");
            Console.WriteLine($"H(X|X) = {PrepareDouble(entropyCalculator.FirstOrderEntropy)}");
            Console.WriteLine($"H(X|XX) = {PrepareDouble(entropyCalculator.SecondOrderEntropy)}");
            Console.WriteLine();
        }

        public static void Encode(string filename, byte[] bytesFromFile)
        {
            Console.Write($"{filename}->");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Encoder encoder = new Encoder(bytesFromFile);
            byte[] encodedBytes = encoder.Encode();

            string newFilename = filename + ARCHIVE_EXTENSION;
            File.WriteAllBytes(newFilename, encodedBytes);
            stopwatch.Stop();

            double compressionRate = 1 - (double)encodedBytes.Length / bytesFromFile.Length;

            Console.WriteLine(
                $"{newFilename} " +
                $"{bytesFromFile.Length}->{encodedBytes.Length} " +
                $"{compressionRate:P2} " +
                $"{stopwatch.Elapsed.TotalSeconds}s");
        }

        public static void Decode(string filename, byte[] bytesFromFile)
        {
            if (!filename.EndsWith(ARCHIVE_EXTENSION))
                throw new ArgumentException($"Cannot decode files without '{ARCHIVE_EXTENSION}' extension!");

            Console.Write($"{filename}->");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Decoder decoder = new Decoder(bytesFromFile);
            byte[] decodedBytes = decoder.Decode();
            
            string newFilename = Path.GetFileNameWithoutExtension(filename);
            File.WriteAllBytes(newFilename, decodedBytes);
            stopwatch.Stop();

            Console.WriteLine(
                $"{newFilename} " +
                $"{stopwatch.Elapsed.TotalSeconds}s");
        }
    }
}
