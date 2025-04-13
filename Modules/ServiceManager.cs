using System;
using System.Linq;
using System.ServiceProcess;
using System.Management;

namespace SystemUtilityApp.Modules
{
    public static class ServiceManager
    {
        public static void Run()
        {
            Console.Clear();
            Console.WriteLine("=== Service Manager ===\n");

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("Options:");
                Console.WriteLine("[L]ist Services");
                Console.WriteLine("[V]iew Service Properties");
                Console.WriteLine("[S]tart Service");
                Console.WriteLine("S[T]op Service");
                Console.WriteLine("[Q]uit to Main Menu");
                Console.Write("Select an option: ");
                string option = Console.ReadLine().Trim().ToUpper();

                switch (option)
                {
                    case "L":
                        ListServices();
                        break;
                    case "V":
                        ViewServiceProperties();
                        break;
                    case "S":
                        StartService();
                        break;
                    case "T":
                        StopService();
                        break;
                    case "Q":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }

        // Lists all services sorted by Display Name.
        private static void ListServices()
        {
            Console.Clear();
            Console.WriteLine("=== List of Services ===\n");

            ServiceController[] services = ServiceController.GetServices();
            Console.WriteLine("{0,-40} {1,-10} {2}", "Service Name", "Status", "Display Name");
            Console.WriteLine(new string('-', 70));

            foreach (ServiceController service in services.OrderBy(s => s.DisplayName))
            {
                Console.WriteLine("{0,-40} {1,-10} {2}", service.ServiceName, service.Status, service.DisplayName);
            }

            Pause();
        }

        // Displays properties for a given service.
        private static void ViewServiceProperties()
        {
            Console.Clear();
            Console.Write("Enter the Service Name to view details: ");
            string name = Console.ReadLine().Trim();

            try
            {
                ServiceController service = new ServiceController(name);
                Console.WriteLine($"\nService Name: {service.ServiceName}");
                Console.WriteLine($"Display Name: {service.DisplayName}");
                Console.WriteLine($"Status:       {service.Status}");
                Console.WriteLine($"Service Type: {service.ServiceType}");

                // Retrieve additional properties using WMI.
                try
                {
                    string query = $"SELECT * FROM Win32_Service WHERE Name = '{service.ServiceName}'";
                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                    {
                        foreach (ManagementObject obj in searcher.Get())
                        {
                            Console.WriteLine("Start Mode:   " + obj["StartMode"]);
                            Console.WriteLine("Path:         " + obj["PathName"]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Additional properties retrieval failed: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Service not found or access error: " + ex.Message);
            }

            Pause();
        }

        // Starts a service by its name.
        private static void StartService()
        {
            Console.Clear();
            Console.Write("Enter the Service Name to start: ");
            string name = Console.ReadLine().Trim();

            try
            {
                ServiceController service = new ServiceController(name);
                if (service.Status == ServiceControllerStatus.Running)
                {
                    Console.WriteLine("Service is already running.");
                }
                else
                {
                    Console.WriteLine("Starting service...");
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                    Console.WriteLine("Service started.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error starting service: " + ex.Message);
            }

            Pause();
        }

        // Stops a service by its name.
        private static void StopService()
        {
            Console.Clear();
            Console.Write("Enter the Service Name to stop: ");
            string name = Console.ReadLine().Trim();

            try
            {
                ServiceController service = new ServiceController(name);
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    Console.WriteLine("Service is already stopped.");
                }
                else
                {
                    Console.WriteLine("Stopping service...");
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                    Console.WriteLine("Service stopped.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error stopping service: " + ex.Message);
            }

            Pause();
        }

        // Pauses execution until the user presses a key.
        private static void Pause()
        {
            Console.WriteLine("\nPress any key to return to the service menu...");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("=== Service Manager ===\n");
        }
    }
}
