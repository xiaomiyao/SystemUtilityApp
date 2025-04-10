using System;
using System.Diagnostics;
using System.Threading;

namespace SystemUtilityApp.Modules
{
    public static class CpuMonitor
    {
        public static void Run()
        {
            Console.Clear();
            Console.WriteLine("=== CPU Monitor (Press Q to quit) ===\n");

            try
            {
                using var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                _ = cpuCounter.NextValue(); // Warm-up call
                Thread.Sleep(1000);

                while (true)
                {
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                        break;

                    float usage = cpuCounter.NextValue();
                    DrawCpuBar(usage);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading CPU usage:");
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("\n\nExiting CPU Monitor...");
            Thread.Sleep(1000);
        }

        private static void DrawCpuBar(float usage)
        {
            int barWidth = 50;
            int filled = (int)(usage / 100 * barWidth);
            string bar = new string('█', filled).PadRight(barWidth, '░');

            Console.Write($"\rCPU Usage: {usage,5:0.0}% [{bar}]");
        }

    }
}
