using System;
using System.IO;

namespace SystemUtilityApp.Modules
{
    public static class DiscMonitor
    {
        // Define threshold (in percentage) for free space warning.
        private const double FreeSpaceWarningThreshold = 10.0; // Warn if free space is less than 10% of total.

        public static void Run()
        {
            Console.Clear();
            Console.WriteLine("=== Disk Monitor ===\n");

            // Get the list of available drives.
            DriveInfo[] drives = DriveInfo.GetDrives();

            if (drives.Length == 0)
            {
                Console.WriteLine("No drives found.");
                ReturnToMenu();
                return;
            }

            // Iterate through each drive.
            foreach (DriveInfo drive in drives)
            {
                if (!drive.IsReady)
                {
                    Console.WriteLine($"Drive {drive.Name} is not ready.");
                    continue;
                }

                long totalSize = drive.TotalSize;
                long freeSpace = drive.TotalFreeSpace;
                long usedSpace = totalSize - freeSpace;
                double usedPercent = (totalSize > 0) ? (usedSpace * 100.0 / totalSize) : 0;
                double freePercent = (totalSize > 0) ? (freeSpace * 100.0 / totalSize) : 0;

                // Display drive information.
                Console.WriteLine($"Drive: {drive.Name}");
                Console.WriteLine($"  Total Size:    {FormatBytes(totalSize)}");
                Console.WriteLine($"  Used Space:    {FormatBytes(usedSpace)} ({usedPercent:0.00}%)");
                Console.WriteLine($"  Free Space:    {FormatBytes(freeSpace)} ({freePercent:0.00}%)");

                // Draw disk usage bar.
                DrawDiskBar(usedPercent);

                // Show a warning if free space is critically low.
                if (freePercent < FreeSpaceWarningThreshold)
                {
                    Console.WriteLine("  WARNING: Free space is critically low!");
                }
                Console.WriteLine();
            }

            ReturnToMenu();
        }

        // Draws a progress bar for disk usage.
        private static void DrawDiskBar(double usedPercent)
        {
            int barWidth = 50;
            int filled = (int)(usedPercent / 100 * barWidth);
            string bar = new string('█', filled).PadRight(barWidth, '░');
            Console.WriteLine($"  Usage: [{bar}] {usedPercent:0.00}% used");
        }

        // Converts a byte count to a human-readable string.
        private static string FormatBytes(long bytes)
        {
            if (bytes < 1024)
                return $"{bytes} B";

            double size = bytes;
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            int unit = 0;
            while (size >= 1024 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }
            return $"{size:0.00} {units[unit]}";
        }

        // Waits for a key press before returning to the main menu.
        private static void ReturnToMenu()
        {
            Console.WriteLine("Press any key to return to the main menu...");
            Console.ReadKey();
        }
    }
}
