using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Edison.Simulators.Sensors.Config;
using Edison.Simulators.Sensors.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Edison.Common.Config;
using Edison.Common.DAO;
using Edison.Common.Interfaces;
using Edison.Common;
using Edison.Simulators.Sensors.Models;

namespace Edison.Simulators.Sensors
{
    class Program
    {
        private static bool _Interrupt = false;
        private static Application _App;

        static void Main(string[] args)
        {
            //DI
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            _App = serviceProvider.GetService<Application>();

            if (args.Length != 0)
            {
                //TODO
            }
            else
            {
                Task.Run(() => Menu()).Wait();
                while (!_Interrupt)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            ILoggerFactory loggerFactory = new LoggerFactory()
                .AddConsole()
                .AddDebug();

            services.AddSingleton(loggerFactory);
            services.AddLogging();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            
            IConfigurationRoot configuration = builder.Build();
            services.AddSingleton(configuration);

            // Support typed Options
            services.AddOptions();
            services.Configure<SimulatorConfig>(configuration.GetSection("Simulator"));
            services.Configure<CosmosDBOptions>(typeof(EventClusterDAO).FullName, configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(EventClusterDAO).FullName, opt => opt.Collection = opt.Collections.EventClusters);
            services.Configure<CosmosDBOptions>(typeof(ResponseDAO).FullName, configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(ResponseDAO).FullName, opt => opt.Collection = opt.Collections.Responses);
            services.Configure<CosmosDBOptions>(typeof(Entity).FullName, configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(Entity).FullName, opt => opt.Collection = "Sagas");
            services.Configure<CosmosDBOptions>(typeof(BotDAO).FullName, configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(BotDAO).FullName, opt => opt.Collection = opt.Collections.Bot);
            services.Configure<CosmosDBOptions>(typeof(DeviceDAO).FullName, configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(DeviceDAO).FullName, opt => opt.Collection = opt.Collections.Devices);
            services.Configure<CosmosDBOptions>(typeof(NotificationDAO).FullName, configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(NotificationDAO).FullName, opt => opt.Collection = opt.Collections.Notifications);
            services.Configure<CosmosDBOptions>(typeof(ActionPlanDAO).FullName, configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(ActionPlanDAO).FullName, opt => opt.Collection = opt.Collections.ActionPlans);
            services.Configure<CosmosDBOptions>(typeof(ChatReportDAO).FullName, configuration.GetSection("CosmosDb"));
            services.Configure<CosmosDBOptions>(typeof(ChatReportDAO).FullName, opt => opt.Collection = opt.Collections.ChatReports);
            services.AddSingleton<IoTDeviceHelper>();
            services.AddScoped(typeof(ICosmosDBRepository<>), typeof(CosmosDBRepository<>));
            services.AddSingleton<Application>();
        }

        private async static Task Menu()
        {
            ConsoleHelper.WriteInfo("1. Setup list of sensors");
            ConsoleHelper.WriteInfo("2. Setup list of sensors - override tags");
            ConsoleHelper.WriteInfo("3. Remove demo sensors");
            ConsoleHelper.WriteInfo("4. Remove all event clusters/responses/sagas");
            ConsoleHelper.WriteInfo("5. Start test simulation on one sensor");
            ConsoleHelper.WriteInfo("6. Start test simulation on all sensors");
            ConsoleHelper.WriteInfo("7. Start orchestrated simulation");
            //ConsoleHelper.WriteInfo("8. Monitor SmartBulbs"); //Removed because cause issues with running physical devices.
            ConsoleHelper.WriteInfo("A. Initialize Database");
            ConsoleHelper.WriteInfo("9. Exit");

            ConsoleKeyInfo key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.NumPad1:
                case ConsoleKey.D1:
                    await MenuAddSensors(false);
                    break;
                case ConsoleKey.NumPad2:
                case ConsoleKey.D2:
                    await MenuAddSensors(true);
                    break;
                case ConsoleKey.NumPad3:
                case ConsoleKey.D3:
                    await MenuRemoveSensors();
                    break;
                case ConsoleKey.NumPad4:
                case ConsoleKey.D4:
                    await MenuRemoveEventClusters();
                    break;
                case ConsoleKey.NumPad5:
                case ConsoleKey.D5:
                    await MenuTestSensor();
                    break;
                case ConsoleKey.NumPad6:
                case ConsoleKey.D6:
                    await MenuTestSensors();
                    break;
                case ConsoleKey.NumPad7:
                case ConsoleKey.D7:
                    await MenuSimulate();
                    break;
                //Removed because cause issues with running physical devices.
                //case ConsoleKey.NumPad8:
                //case ConsoleKey.D8:
                //    await MenuMonitorLights();
                //    break;
                case ConsoleKey.A:
                    await MenuInitializeDB();
                    break;
                case ConsoleKey.NumPad9:
                case ConsoleKey.D9:
                    _Interrupt = true;
                    break;
                default:
                    ConsoleHelper.ClearConsole();
                    await Menu();
                    break;
            }
        }

        private async static Task MenuInitializeDB()
        {
            ConsoleHelper.ClearConsole();
            ConsoleHelper.WriteInfo("Please enter 'Y' if you want to initialize the DB.");

            ConsoleKeyInfo key = Console.ReadKey();

            if (key.Key == ConsoleKey.Y)
            {
                await _App.InitializeDB();
            }
            ConsoleHelper.ClearConsole();
            await Menu();
        }

        private async static Task MenuAddSensors(bool overrideTags)
        {
            ConsoleHelper.ClearConsole();
            ConsoleHelper.WriteInfo("Please enter the path of the csv file. For example 'sensors.csv' or 'C://sensors.csv'.");

            string path = Console.ReadLine();

            if (File.Exists(path))
            {
                await _App.AddSensors(path, overrideTags);
                ConsoleHelper.ClearConsole();
            }
            else
            {
                ConsoleHelper.ClearConsole();
                ConsoleHelper.WriteError($"The file '{path}' does not exist.");
            }
            await Menu();
        }

        private async static Task MenuRemoveSensors()
        {
            ConsoleHelper.ClearConsole();
            ConsoleHelper.WriteInfo("Please enter 'Y' if you are sure to delete all demo sensors.");

            ConsoleKeyInfo key = Console.ReadKey();

            if (key.Key == ConsoleKey.Y)
            {
                await _App.RemoveSensors();
            }
            ConsoleHelper.ClearConsole();
            await Menu();
        }

        private async static Task MenuRemoveEventClusters()
        {
            ConsoleHelper.ClearConsole();
            ConsoleHelper.WriteInfo("Please enter 'Y' if you are sure to delete all event clusters.");

            ConsoleKeyInfo key = Console.ReadKey();

            if (key.Key == ConsoleKey.Y)
            {
                await _App.RemoveEventClusters();
            }
            ConsoleHelper.ClearConsole();
            await Menu();
        }

        private async static Task MenuTestSensor()
        {
            ConsoleHelper.ClearConsole();

            await _App.TestSensor();

            ConsoleHelper.ClearConsole();
            await Menu();
        }

        private async static Task MenuTestSensors()
        {
            ConsoleHelper.ClearConsole();
            ConsoleHelper.WriteInfo("Press Escape when you want to end the simulation.");

            _App.TestSensors();

            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey();
            }
            while (key.Key != ConsoleKey.Escape);

            _App.InterruptSimulation();

            ConsoleHelper.ClearConsole();

            await Task.Delay(2000);
            await Menu();
        }

        private async static Task MenuMonitorLights()
        {
            ConsoleHelper.ClearConsole();
            ConsoleHelper.WriteInfo("Press Escape when you want to end the monitoring.");

            _App.MonitorSmartBulbs();

            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey();
            }
            while (key.Key != ConsoleKey.Escape);

            _App.InterruptSimulation();

            ConsoleHelper.ClearConsole();

            await Task.Delay(2000);
            await Menu();
        }

        private async static Task MenuSimulate()
        {
            ConsoleHelper.ClearConsole();
            ConsoleHelper.WriteInfo("Please enter the path of the csv file. For example 'simulation.csv' or 'C://simulation.csv'.");

            string path = Console.ReadLine();

            if (File.Exists(path))
            {
                ConsoleHelper.WriteInfo("Press Escape when you want to end the simulation.");
                _App.Simulate(path);

                ConsoleKeyInfo key;
                do
                {
                    key = Console.ReadKey();
                }
                while (key.Key != ConsoleKey.Escape);

                _App.InterruptSimulation();

                ConsoleHelper.ClearConsole();
            }
            else
            {
                ConsoleHelper.ClearConsole();
                ConsoleHelper.WriteError($"The file '{path}' does not exist.");
            }
            await Menu();
        }

        private static void Usage()
        {
            Console.WriteLine("Help");
            Console.WriteLine("----------------");
            Console.WriteLine("-addsensors file.csv: Add devices from a csv files");
            Console.WriteLine("-removesensors: Remove demo devices");
            Console.WriteLine("-test: Send random events from all existing demo sensors");
            Console.WriteLine("-simulate file.csv: Start an orchestrated simulation");
        }
    }
}
