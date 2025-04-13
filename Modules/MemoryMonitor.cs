using System;
using System.Management;
using System.Threading;

namespace SystemUtilityApp.Modules
{
    public static class MemoryMonitor
    {
        public static void Run()
        {
            Console.Clear();
            Console.WriteLine("=== Memory Monitor (Press Q to quit) ===\n");

            // Get total physical memory (won't change during runtime)
            ulong totalMemory = GetTotalMemory();
            if (totalMemory == 0)
            {
                Console.WriteLine("Error retrieving total memory.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            // Real-time loop
            while (true)
            {
                // Check if the user pressed Q to quit
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Q)
                    break;

                // Read free memory, calculate used memory, and usage percentage
                ulong freeMemory = GetFreeMemory();
                ulong usedMemory = totalMemory - freeMemory;
                double usagePercent = usedMemory * 100.0 / totalMemory;

                // Draw or refresh the memory usage bar and stats
                DrawMemoryBar(usagePercent, totalMemory, usedMemory, freeMemory);

                Thread.Sleep(1000);
            }

            Console.WriteLine("\nExiting Memory Monitor...");
            Thread.Sleep(1000);
        }

        // Returns total memory (in bytes) using WMI
        private static ulong GetTotalMemory()
        {
            try
            {
                ulong total = 0;
                using (var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        total = Convert.ToUInt64(obj["TotalVisibleMemorySize"]) * 1024; // Convert from KB to bytes
                    }
                }
                return total;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading total memory: " + ex.Message);
                return 0;
            }
        }

        // Returns free memory (in bytes) using WMI
        private static ulong GetFreeMemory()
        {
            try
            {
                ulong free = 0;
                using (var searcher = new ManagementObjectSearcher("SELECT FreePhysicalMemory FROM Win32_OperatingSystem"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        free = Convert.ToUInt64(obj["FreePhysicalMemory"]) * 1024; // Convert from KB to bytes
                    }
                }
                return free;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading free memory: " + ex.Message);
                return 0;
            }
        }

        // Draws the memory usage information and progress bar to the console.
        private static void DrawMemoryBar(double usagePercent, ulong total, ulong used, ulong free)
        {
            // Clear the console to refresh the display
            Console.Clear();
            Console.WriteLine("=== Memory Monitor (Press Q to quit) ===\n");

            int barWidth = 50;
            int filled = (int)(usagePercent / 100 * barWidth);
            // Filled part uses a solid block; empty part uses a light block
            string bar = new string('█', filled).PadRight(barWidth, '░');

            Console.WriteLine($"Memory Usage: {usagePercent,6:0.00}% [{bar}]");
            Console.WriteLine();
            Console.WriteLine($"Total Memory: {FormatBytes(total)}");
            Console.WriteLine($"Used Memory:  {FormatBytes(used)}");
            Console.WriteLine($"Free Memory:  {FormatBytes(free)}");
        }

        // Formats bytes into GB for display
        private static string FormatBytes(ulong bytes)
        {
            double gb = bytes / (1024.0 * 1024 * 1024);
            return $"{gb:0.00} GB";
        }
    }
}
