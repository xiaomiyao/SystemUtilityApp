using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemUtilityApp.Helpers
{
    public static class ConsoleUI
    {
        public static void ShowMainMenu()
        {
            Console.Clear();
            Console.WriteLine("===== System Utility Console App =====\n");
            Console.WriteLine("1. CPU Monitor");
            Console.WriteLine("2. Memory Monitor");
            Console.WriteLine("3. Disk Monitor");
            Console.WriteLine("4. Process Manager");
            Console.WriteLine("5. File Manager");
            Console.WriteLine("6. File Search");
            Console.WriteLine("7. Registry Editor");
            Console.WriteLine("8. Service Manager");
            Console.WriteLine("9. System Event Viewer");
            Console.WriteLine("0. Exit");
        }
    }
}
