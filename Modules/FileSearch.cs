using System;
using System.Collections.Generic;
using System.IO;

namespace SystemUtilityApp.Modules
{
    public static class FileSearch
    {
        public static void Run()
        {
            Console.Clear();
            Console.WriteLine("=== File Search ===\n");

            // Ask for the starting directory, defaulting to current directory.
            Console.Write("Enter starting directory (default: current directory): ");
            string startDir = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(startDir))
            {
                startDir = Directory.GetCurrentDirectory();
            }

            if (!Directory.Exists(startDir))
            {
                Console.WriteLine("Directory does not exist.");
                Pause();
                return;
            }

            // Ask for a file mask (e.g., *.txt) - default to "*" (all files).
            Console.Write("Enter file mask (e.g. *.txt, default: *): ");
            string mask = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(mask))
            {
                mask = "*";
            }

            // Optional filters can be added here as before...
            // For brevity, I'll leave out file size and date filters from this snippet.

            Console.WriteLine("\nSearching, please wait...\n");

            IEnumerable<string> files = SafeGetFiles(startDir, mask);

            int count = 0;
            Console.WriteLine("{0,-60} {1,15}", "Path", "Size");
            Console.WriteLine(new string('-', 80));
            foreach (var file in files)
            {
                try
                {
                    FileInfo info = new FileInfo(file);
                    Console.WriteLine("{0,-60} {1,15}", file, FormatBytes(info.Length));
                    count++;
                }
                catch
                {
                    // Skip any files that can't be accessed.
                }
            }

            Console.WriteLine($"\nTotal files found: {count}");
            Pause();
        }

        // Recursively retrieves files while catching unauthorized-access exceptions.
        private static IEnumerable<string> SafeGetFiles(string path, string searchPattern)
        {
            List<string> files = new List<string>();
            try
            {
                files.AddRange(Directory.GetFiles(path, searchPattern));
            }
            catch (UnauthorizedAccessException)
            {
                // Skip this directory if access is denied.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing files in {path}: {ex.Message}");
            }

            try
            {
                foreach (var directory in Directory.GetDirectories(path))
                {
                    files.AddRange(SafeGetFiles(directory, searchPattern));
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Skip this subdirectory if access is denied.
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing directories in {path}: {ex.Message}");
            }

            return files;
        }

        // Formats a byte count into a human-readable string.
        private static string FormatBytes(long bytes)
        {
            if (bytes < 1024)
                return $"{bytes} B";

            double size = bytes;
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            int unitIndex = 0;
            while (size >= 1024 && unitIndex < units.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }
            return $"{size:0.00} {units[unitIndex]}";
        }

        // Waits for user input before returning to the menu.
        private static void Pause()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
