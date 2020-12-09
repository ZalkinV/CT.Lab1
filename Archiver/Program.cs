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
                    args = new string[] { "i", "calgarycorpus/*" };

                string mode = args[0];
                string path = args[1];
                string dir = Path.GetDirectoryName(path);
                string pattern = Path.GetFileName(path);
                string[] filesNames = Directory.GetFiles(dir, pattern);

                foreach (string filename in filesNames)
                {
                    byte[] bytesFromFile = File.ReadAllBytes(filename);
                    switch (mode)
                    {
                        case INF_MODE:
                            CalculateEntropies(filename, bytesFromFile);
                            break;
                        case ENC_MODE:
                            Encode(path, bytesFromFile);
                            break;
                        case DEC_MODE:
                            try
                            {
                                Decode(path, bytesFromFile);
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
            Console.WriteLine($"Entropies for '{filename}':");
            EntropyCalculator entropyCalculator = new EntropyCalculator(bytesFromFile);
            Console.WriteLine($"H(X) = {entropyCalculator.ZeroOrderEntropy}");
            Console.WriteLine($"H(X|X) = {entropyCalculator.FirstOrderEntropy}");
            Console.WriteLine($"H(X|XX) = {entropyCalculator.SecondOrderEntropy}");
            Console.WriteLine();
        }

        public static void Encode(string filename, byte[] bytesFromFile)
        {
            Encoder encoder = new Encoder(bytesFromFile);
            byte[] encodedBytes = encoder.Encode();

            string newFilename = filename + ARCHIVE_EXTENSION;
            File.WriteAllBytes(newFilename, encodedBytes);

            Console.WriteLine($"File '{filename}' was encoded as '{newFilename}'");
        }

        public static void Decode(string filename, byte[] bytesFromFile)
        {
            if (!filename.EndsWith(ARCHIVE_EXTENSION))
                throw new ArgumentException($"Cannot decode files without '{ARCHIVE_EXTENSION}' extension!");

            Decoder decoder = new Decoder(bytesFromFile);
            byte[] decodedBytes = decoder.Decode();
            
            string newFilename = Path.GetFileNameWithoutExtension(filename);
            File.WriteAllBytes(newFilename, decodedBytes);

            Console.WriteLine($"File '{filename}' was decoded as '{newFilename}'");
        }
    }
}
