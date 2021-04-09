using System;
using System.IO;

namespace ConsoleApp12
{
    class Program
    {     
        static void RunEncode()
        {
            System.Console.WriteLine("Input source file path");
            var src = System.Console.ReadLine();
            if (!File.Exists(src))
            {
                System.Console.WriteLine("Source file doesn't exist");
                return;
            }

            System.Console.WriteLine("Input target file path");
            var tar = System.Console.ReadLine();
            if (!File.Exists(tar))
            {
                System.Console.WriteLine("Target file doesn't exist");
                return;
            }

            int rangRate;
            try
            {
                System.Console.WriteLine("Input rang rate");
                rangRate = int.Parse(System.Console.ReadLine());
            }
            catch
            {
                System.Console.WriteLine("Rang rate must be integer");
                return;
            }

            try
            {
                var encoder = new Encoder(src, tar, rangRate);
                encoder.Encode();

                System.Console.WriteLine("Completed");
            }
            catch
            {
                System.Console.WriteLine("Encoding failed");
            }
        }

        static void RunDecode()
        {
            System.Console.WriteLine("Input source file path");
            var src = System.Console.ReadLine();
            if (!File.Exists(src))
            {
                System.Console.WriteLine("Source file doesn't exist");
                return;
            }

            System.Console.WriteLine("Input target dir path");
            var tar = System.Console.ReadLine();
            if (!Directory.Exists(tar))
            {
                System.Console.WriteLine("Target dir doesn't exist");
                return;
            }

            try
            {
                var decoder = new Decoder(src, tar);
                decoder.Decode();
                System.Console.WriteLine("Completed");
            }
            catch(Exception e)
            {
                System.Console.WriteLine("Decoding failed");
                throw e;
            }

        }

        static void PrintMenu()
        {
            System.Console.WriteLine("Input 1 to encode image");
            System.Console.WriteLine("Input 2 to decode image");
            System.Console.WriteLine("Input 3 to exit");
        }

        static void Main()
        {
        
            var encoder = new Encoder(
                "C:/Users/ivann/source/repos/ConsoleApp12/ConsoleApp12/sample.bmp",
                "C:/Users/ivann/source/repos/ConsoleApp12/ConsoleApp12/out.bin",
                    40);
            encoder.Encode();

            while (true)
            {
                PrintMenu();

                var inp = System.Console.ReadLine();

                switch(inp)
                {
                    case "1":
                        {
                            RunEncode();
                            break;
                        }
                    case "2":
                        {
                            RunDecode();
                            break;
                        }
                    case "3":
                        {
                            return;
                        }
                    default:
                        {
                            System.Console.WriteLine("Unknown command");
                            break;
                        }
                }
            }
         
        }
    }
}
