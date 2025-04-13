using System;
using System.IO;

namespace SystemUtilityApp.Modules
{
    public static class FileManager
    {
        public static void Run()
        {
            Console.Clear();
            Console.WriteLine("=== File Manager ===\n");

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Options:");
                Console.WriteLine("[D] Display File Tree");
                Console.WriteLine("[C] Copy file/folder");
                Console.WriteLine("[M] Move file/folder");
                Console.WriteLine("[R] Rename file/folder");
                Console.WriteLine("[X] Delete file/folder");
                Console.WriteLine("[N] Create new folder");
                Console.WriteLine("[Q] Quit to main menu");
                Console.Write("Select an option: ");
                string option = Console.ReadLine().Trim().ToUpper();

                switch (option)
                {
                    case "D":
                        DisplayFileTree();
                        break;
                    case "C":
                        CopyItem();
                        break;
                    case "M":
                        MoveItem();
                        break;
                    case "R":
                        RenameItem();
                        break;
                    case "X":
                        DeleteItem();
                        break;
                    case "N":
                        CreateFolder();
                        break;
                    case "Q":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                if (!exit)
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                    Console.WriteLine("=== File Manager ===\n");
                }
            }
        }

        // Option D: Display file tree.
        private static void DisplayFileTree()
        {
            Console.Write("Enter the starting directory (leave blank for current directory): ");
            string startDir = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(startDir))
            {
                startDir = Directory.GetCurrentDirectory();
            }

            if (!Directory.Exists(startDir))
            {
                Console.WriteLine("Directory does not exist.");
                return;
            }

            Console.WriteLine($"\nFile Tree for: {startDir}\n");
            try
            {
                DisplayDirectory(startDir, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error displaying file tree: " + ex.Message);
            }
        }

        // Recursively displays directory structure.
        private static void DisplayDirectory(string path, int indent)
        {
            string indentString = new string(' ', indent * 2);
            // Show directory name in brackets.
            Console.WriteLine($"{indentString}[{Path.GetFileName(path)}]");

            // List files in current directory.
            string[] files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                Console.WriteLine($"{indentString}  {Path.GetFileName(file)}");
            }

            // Recurse into subdirectories.
            string[] directories = Directory.GetDirectories(path);
            foreach (var dir in directories)
            {
                DisplayDirectory(dir, indent + 1);
            }
        }

        // Option C: Copy file/folder.
        private static void CopyItem()
        {
            Console.Write("Enter source path: ");
            string source = Console.ReadLine().Trim();
            Console.Write("Enter destination path: ");
            string destination = Console.ReadLine().Trim();

            if (File.Exists(source))
            {
                try
                {
                    File.Copy(source, destination, true);
                    Console.WriteLine("File copied successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error copying file: " + ex.Message);
                }
            }
            else if (Directory.Exists(source))
            {
                try
                {
                    CopyDirectory(source, destination);
                    Console.WriteLine("Directory copied successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error copying directory: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Source path does not exist.");
            }
        }

        // Helper: Recursively copy directories.
        private static void CopyDirectory(string sourceDir, string destDir)
        {
            // Ensure destination exists.
            Directory.CreateDirectory(destDir);
            // Copy files.
            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }
            // Recursively copy subdirectories.
            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destDir, Path.GetFileName(directory));
                CopyDirectory(directory, destSubDir);
            }
        }

        // Option M: Move file/folder.
        private static void MoveItem()
        {
            Console.Write("Enter source path: ");
            string source = Console.ReadLine().Trim();
            Console.Write("Enter destination path: ");
            string destination = Console.ReadLine().Trim();

            if (File.Exists(source))
            {
                try
                {
                    File.Move(source, destination);
                    Console.WriteLine("File moved successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error moving file: " + ex.Message);
                }
            }
            else if (Directory.Exists(source))
            {
                try
                {
                    Directory.Move(source, destination);
                    Console.WriteLine("Directory moved successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error moving directory: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Source path does not exist.");
            }
        }

        // Option R: Rename file/folder.
        private static void RenameItem()
        {
            Console.Write("Enter current path: ");
            string source = Console.ReadLine().Trim();
            Console.Write("Enter new name (not the full path): ");
            string newName = Console.ReadLine().Trim();

            if (File.Exists(source))
            {
                try
                {
                    string dir = Path.GetDirectoryName(source);
                    string destination = Path.Combine(dir, newName);
                    File.Move(source, destination);
                    Console.WriteLine("File renamed successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error renaming file: " + ex.Message);
                }
            }
            else if (Directory.Exists(source))
            {
                try
                {
                    string parentDir = Directory.GetParent(source).FullName;
                    string destination = Path.Combine(parentDir, newName);
                    Directory.Move(source, destination);
                    Console.WriteLine("Directory renamed successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error renaming directory: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Path does not exist.");
            }
        }

        // Option X: Delete file/folder.
        private static void DeleteItem()
        {
            Console.Write("Enter the path to delete: ");
            string path = Console.ReadLine().Trim();

            if (File.Exists(path))
            {
                try
                {
                    Console.Write("Are you sure you want to delete this file? (Y/N): ");
                    if (Console.ReadLine().Trim().ToUpper() == "Y")
                    {
                        File.Delete(path);
                        Console.WriteLine("File deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Deletion cancelled.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting file: " + ex.Message);
                }
            }
            else if (Directory.Exists(path))
            {
                try
                {
                    Console.Write("Are you sure you want to delete this directory and all its contents? (Y/N): ");
                    if (Console.ReadLine().Trim().ToUpper() == "Y")
                    {
                        Directory.Delete(path, true);
                        Console.WriteLine("Directory deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Deletion cancelled.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting directory: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("Path does not exist.");
            }
        }

        // Option N: Create new folder.
        private static void CreateFolder()
        {
            Console.Write("Enter the path to create a new folder: ");
            string path = Console.ReadLine().Trim();
            try
            {
                Directory.CreateDirectory(path);
                Console.WriteLine("Folder created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating folder: " + ex.Message);
            }
        }
    }
}
