using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;

namespace SystemUtilityApp.Modules
{
    public static class ProcessManager
    {
        public static void Run()
        {
            Console.Clear();
            Console.WriteLine("=== Process Manager ===\n");

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Options:");
                Console.WriteLine("[L]ist processes");
                Console.WriteLine("[K]ill process");
                Console.WriteLine("[F]ilter processes by name");
                Console.WriteLine("[Q]uit to main menu");
                Console.Write("Select an option: ");
                string option = Console.ReadLine().Trim().ToUpper();

                switch (option)
                {
                    case "L":
                        ListProcesses();
                        break;
                    case "K":
                        KillProcess();
                        break;
                    case "F":
                        FilterProcesses();
                        break;
                    case "Q":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        Pause();
                        break;
                }
            }
        }

        // List processes with an optional name filter.
        private static void ListProcesses(string filter = null)
        {
            Console.Clear();
            Console.WriteLine("Fetching processes... Please wait.");

            Process[] processes = Process.GetProcesses();

            // Capture initial CPU times for each process.
            var initialCpuTimes = new Dictionary<int, TimeSpan>();
            foreach (var proc in processes)
            {
                try
                {
                    initialCpuTimes[proc.Id] = proc.TotalProcessorTime;
                }
                catch
                {
                    // Skip processes we cannot access.
                }
            }

            // Wait 1 second to compute CPU usage.
            Thread.Sleep(1000);

            var processInfoList = new List<ProcessInfo>();
            foreach (var proc in processes)
            {
                try
                {
                    // Filter if a filter string is provided.
                    if (!string.IsNullOrEmpty(filter) &&
                        !proc.ProcessName.ToLower().Contains(filter.ToLower()))
                    {
                        continue;
                    }

                    // Get initial CPU time, if available.
                    TimeSpan initialTime = initialCpuTimes.ContainsKey(proc.Id)
                        ? initialCpuTimes[proc.Id]
                        : TimeSpan.Zero;
                    TimeSpan currentCpuTime = proc.TotalProcessorTime;

                    // Calculate CPU usage over the 1-second interval.
                    double cpuUsagePercent = ((currentCpuTime - initialTime).TotalMilliseconds / 1000.0)
                                             * 100.0 / Environment.ProcessorCount;

                    // Calculate memory usage in MB.
                    double memoryMB = proc.WorkingSet64 / (1024.0 * 1024);

                    processInfoList.Add(new ProcessInfo
                    {
                        Name = proc.ProcessName,
                        PID = proc.Id,
                        MemoryMB = memoryMB,
                        CPU = cpuUsagePercent
                    });
                }
                catch
                {
                    // Skip processes that can't be queried.
                }
            }

            // Display header.
            Console.WriteLine("{0,-30} {1,6} {2,12} {3,10}", "Process Name", "PID", "Memory (MB)", "CPU (%)");
            Console.WriteLine(new string('-', 60));
            foreach (var info in processInfoList.OrderBy(p => p.Name))
            {
                Console.WriteLine("{0,-30} {1,6} {2,12:0.00} {3,10:0.00}", info.Name, info.PID, info.MemoryMB, info.CPU);
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }

        // Allows filtering the list of processes by name.
        private static void FilterProcesses()
        {
            Console.Write("Enter filter string (process name): ");
            string filter = Console.ReadLine().Trim();
            ListProcesses(filter);
        }

        // Kill a process by either PID or name.
        private static void KillProcess()
        {
            Console.Write("Enter Process ID or Name to kill: ");
            string input = Console.ReadLine().Trim();

            Process target = null;
            int pid;
            if (int.TryParse(input, out pid))
            {
                try
                {
                    target = Process.GetProcessById(pid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Process not found: " + ex.Message);
                    Pause();
                    return;
                }
            }
            else
            {
                // If not a number, try to find by name (first match).
                Process[] processesByName = Process.GetProcessesByName(input);
                if (processesByName.Length > 0)
                {
                    target = processesByName[0];
                }
                else
                {
                    Console.WriteLine("Process with that name not found.");
                    Pause();
                    return;
                }
            }

            if (target != null)
            {
                try
                {
                    Console.Write("Are you sure you want to kill process {0} (PID {1})? (Y/N): ",
                        target.ProcessName, target.Id);
                    string confirmation = Console.ReadLine().Trim().ToUpper();
                    if (confirmation == "Y")
                    {
                        target.Kill();
                        Console.WriteLine("Process killed.");
                    }
                    else
                    {
                        Console.WriteLine("Operation cancelled.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to kill process: " + ex.Message);
                }
            }

            Pause();
        }

        // Pause execution and wait for a key press.
        private static void Pause()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }

        // Helper class to store process details.
        private class ProcessInfo
        {
            public string Name { get; set; }
            public int PID { get; set; }
            public double MemoryMB { get; set; }
            public double CPU { get; set; }
        }
    }
}
