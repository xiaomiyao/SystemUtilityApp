using System;
using Microsoft.Win32;

namespace SystemUtilityApp.Modules
{
    public static class RegistryEditor
    {
        public static void Run()
        {
            Console.Clear();
            Console.WriteLine("=== Registry Editor (Read-Only Mode) ===\n");

            Console.WriteLine("Available Hives:");
            Console.WriteLine("1. HKEY_CURRENT_USER");
            Console.WriteLine("2. HKEY_LOCAL_MACHINE");
            Console.WriteLine("3. HKEY_CLASSES_ROOT");
            Console.WriteLine("4. HKEY_USERS");
            Console.WriteLine("5. HKEY_CURRENT_CONFIG");

            Console.Write("\nSelect a hive (1-5): ");
            string input = Console.ReadLine()?.Trim();
            RegistryKey root = input switch
            {
                "1" => Registry.CurrentUser,
                "2" => Registry.LocalMachine,
                "3" => Registry.ClassesRoot,
                "4" => Registry.Users,
                "5" => Registry.CurrentConfig,
                _ => null
            };

            if (root == null)
            {
                Console.WriteLine("Invalid selection.");
                Pause();
                return;
            }

            Console.Write("Enter the subkey path (or leave empty for root): ");
            string subkeyPath = Console.ReadLine()?.Trim();

            try
            {
                using RegistryKey key = string.IsNullOrEmpty(subkeyPath) ? root : root.OpenSubKey(subkeyPath);
                if (key == null)
                {
                    Console.WriteLine("Subkey not found or access denied.");
                }
                else
                {
                    Console.WriteLine($"\nKey: {key.Name}\n");

                    string[] valueNames = key.GetValueNames();
                    Console.WriteLine("Values:");
                    foreach (string name in valueNames)
                    {
                        object value = key.GetValue(name);
                        Console.WriteLine($"  {name} = {value}");
                    }

                    string[] subkeys = key.GetSubKeyNames();
                    Console.WriteLine("\nSubkeys:");
                    foreach (string name in subkeys)
                    {
                        Console.WriteLine($"  {name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error accessing registry: " + ex.Message);
            }

            Pause();
        }

        private static void Pause()
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}
