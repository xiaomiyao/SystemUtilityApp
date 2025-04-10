using System;
using System.Management;

namespace SystemUtilityApp.Modules
{
    public static class MemoryMonitor
    {
        public static void Run()
        {
            Console.Clear();
            Console.WriteLine("=== Memory Monitor ===\n");

            try
            {
                ulong totalMemory = 0;
                ulong freeMemory = 0;

                var searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    totalMemory = Convert.ToUInt64(obj["TotalVisibleMemorySize"]);
                    freeMemory = Convert.ToUInt64(obj["FreePhysicalMemory"]);
                }

                ulong usedMemory = totalMemory - freeMemory;

                // Convert from KB to bytes
                totalMemory *= 1024;
                freeMemory *= 1024;
                usedMemory *= 1024;

                Console.WriteLine($"Total Memory:     {FormatBytes(totalMemory)}");
                Console.WriteLine($"Used Memory:      {FormatBytes(usedMemory)}");
                Console.WriteLine($"Available Memory: {FormatBytes(freeMemory)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to read memory info:");
                Console.WriteLine(ex.Message);
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        private static string FormatBytes(ulong bytes)
        {
            double gb = bytes / (1024.0 * 1024 * 1024);
            return $"{gb:0.00} GB";
        }
    }
}

