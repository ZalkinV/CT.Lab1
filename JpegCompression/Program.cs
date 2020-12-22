using System;
using System.Diagnostics;
using System.IO;

namespace JpegCompression
{
    class Program
    {
        private const string ENC_MODE = "e";
        private const string DEC_MODE = "d";
        private const string ARCHIVE_EXTENSION = ".zvp";

        static void Main(string[] args)
        {
            try
            {
#if DEBUG
                args = new string[] { "e", "test.txt" };
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
                if (dir == string.Empty)
                    dir = "./";
                string[] filesNames = Directory.GetFiles(dir, pattern);

                if (filesNames.Length == 0)
                    throw new ArgumentException($"No such files was found!");

                foreach (string filename in filesNames)
                {
                    byte[] bytesFromFile = File.ReadAllBytes(filename);
                    switch (mode)
                    {
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
                                $"{ENC_MODE} — encoding mode\n" +
                                $"{DEC_MODE} — decoding mode\n");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                Console.WriteLine("Press any key to close the app...");
                Console.ReadLine();
            }
        }

        public static void Encode(string filename, byte[] bytesFromFile)
        {
            Console.Write($"{filename}->");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Encoder encoder = new Encoder(bytesFromFile);
            var encodedBytes = encoder.Encode();

            string newFilename = filename + ARCHIVE_EXTENSION;
            File.WriteAllBytes(newFilename, encodedBytes);
            
            stopwatch.Stop();

            double compressionRate = 1 - (double)encodedBytes.Length / bytesFromFile.Length;
            double averageBitsPerSymbol = ((double)encodedBytes.Length / bytesFromFile.Length) * 8;

            Console.WriteLine(
                $"{newFilename} " +
                $"{bytesFromFile.Length}->{encodedBytes.Length} " +
                $"{compressionRate:P2} " +
                $"{averageBitsPerSymbol:F3} bits " +
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

            string newFilename = filename.Remove(filename.LastIndexOf("."));
            File.WriteAllBytes(newFilename, decodedBytes);
            stopwatch.Stop();

            Console.WriteLine(
                $"{newFilename} " +
                $"{bytesFromFile.Length}->{decodedBytes.Length} " +
                $"{stopwatch.Elapsed.TotalSeconds}s");
        }
    }
}
