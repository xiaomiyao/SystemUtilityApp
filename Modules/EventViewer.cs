using System;
using System.Diagnostics;
using System.IO;

namespace SystemUtilityApp.Modules
{
    public static class EventViewer
    {
        public static void Run()
        {
            Console.Clear();
            Console.WriteLine("=== System Event Viewer ===\n");

            // Prompt for event log name (default "Application")
            Console.Write("Enter the Event Log name (default: Application): ");
            string logName = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(logName))
            {
                logName = "Application";
            }

            // Prompt for event type filter (Information/Warning/Error). Leave blank for no filter.
            Console.Write("Enter Event Type filter (Information, Warning, Error or leave blank): ");
            string typeFilterStr = Console.ReadLine()?.Trim();
            EventLogEntryType? typeFilter = null;
            if (!string.IsNullOrEmpty(typeFilterStr))
            {
                if (Enum.TryParse(typeFilterStr, true, out EventLogEntryType parsedType))
                {
                    typeFilter = parsedType;
                }
                else
                {
                    Console.WriteLine("Invalid event type entered. No type filter will be applied.");
                }
            }

            // Prompt for event ID filter (optional)
            Console.Write("Enter Event ID filter (or leave blank): ");
            string idFilterStr = Console.ReadLine()?.Trim();
            int? idFilter = null;
            if (!string.IsNullOrEmpty(idFilterStr) && int.TryParse(idFilterStr, out int parsedId))
            {
                idFilter = parsedId;
            }

            Console.WriteLine($"\nReading events from log: {logName}\n");
            int displayedCount = 0;

            try
            {
                using (EventLog eventLog = new EventLog(logName))
                {
                    // Display up to 20 recent events (in reverse order)
                    for (int i = eventLog.Entries.Count - 1; i >= 0 && displayedCount < 20; i--)
                    {
                        EventLogEntry entry = eventLog.Entries[i];

                        // Apply filters if specified.
                        if (typeFilter.HasValue && entry.EntryType != typeFilter.Value)
                            continue;
                        if (idFilter.HasValue && (int)entry.InstanceId != idFilter.Value)
                            continue;

                        Console.WriteLine("------------------------------------------------------------");
                        Console.WriteLine($"Time Generated: {entry.TimeGenerated}");
                        Console.WriteLine($"Type:           {entry.EntryType}");
                        Console.WriteLine($"Source:         {entry.Source}");
                        Console.WriteLine($"Event ID:       {entry.InstanceId}");
                        Console.WriteLine($"Message:        {entry.Message}");
                        displayedCount++;
                    }
                    Console.WriteLine($"\nDisplayed {displayedCount} event(s).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading event log: " + ex.Message);
            }

            // Ask if the user wants to export events.
            Console.Write("\nDo you want to export these events to a CSV file? (Y/N): ");
            string exportChoice = Console.ReadLine()?.Trim().ToUpper();
            if (exportChoice == "Y")
            {
                ExportEvents(logName, typeFilter, idFilter);
            }

            Pause();
        }

        // Exports matching events to a CSV file (up to 1000 events).
        private static void ExportEvents(string logName, EventLogEntryType? typeFilter, int? idFilter)
        {
            string fileName = $"EventLogExport_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
            int exportCount = 0;

            try
            {
                using (EventLog eventLog = new EventLog(logName))
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    // Write CSV header
                    writer.WriteLine("Time,Type,Source,EventID,Message");

                    // Export events (iterate all entries in reverse order for recency)
                    for (int i = eventLog.Entries.Count - 1; i >= 0 && exportCount < 1000; i--)
                    {
                        EventLogEntry entry = eventLog.Entries[i];

                        if (typeFilter.HasValue && entry.EntryType != typeFilter.Value)
                            continue;
                        if (idFilter.HasValue && (int)entry.InstanceId != idFilter.Value)
                            continue;

                        // Escape quotes in the message.
                        string safeMessage = entry.Message.Replace("\"", "\"\"");
                        string line = $"\"{entry.TimeGenerated}\",\"{entry.EntryType}\",\"{entry.Source}\",\"{entry.InstanceId}\",\"{safeMessage}\"";
                        writer.WriteLine(line);
                        exportCount++;
                    }
                }

                Console.WriteLine($"Exported {exportCount} event(s) to {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error exporting events: " + ex.Message);
            }
        }

        // Simple pause method.
        private static void Pause()
        {
            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
