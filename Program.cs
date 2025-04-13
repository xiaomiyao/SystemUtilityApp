// Program.cs
using SystemUtilityApp.Helpers;
using SystemUtilityApp.Modules;

while (true)
{
    ConsoleUI.ShowMainMenu();
    Console.Write("\nSelect an option: ");
    string input = Console.ReadLine()?.Trim().ToLower();

    switch (input)
    {
        case "1": CpuMonitor.Run(); break;
        case "2": MemoryMonitor.Run(); break;
        case "3": DiscMonitor.Run(); break;
        case "4": ProcessManager.Run(); break;
        case "5": FileManager.Run(); break;
        case "6": FileSearch.Run(); break;
        case "7": RegistryEditor.Run(); break;
        case "8": ServiceManager.Run(); break;
        case "9": EventViewer.Run(); break;
        case "0":
            Console.WriteLine("Exiting...");
            return;
        default:
            Console.WriteLine("Invalid option. Try again.");
            break;
    }

    Console.WriteLine("\nPress any key to return to the main menu...");
    Console.ReadKey();
    Console.Clear();
}

