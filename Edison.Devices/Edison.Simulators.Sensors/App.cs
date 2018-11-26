using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Edison.Simulators.Sensors.Helpers;
using CsvHelper;
using System.IO;
using Edison.Simulators.Sensors.Models;
using Edison.Simulators.Sensors.Models.Helpers;
using Edison.Common.Interfaces;
using Edison.Common.DAO;

namespace Edison.Simulators.Sensors
{
    public class Application
    {
        private readonly ILogger<Application> _logger;
        private ICosmosDBRepository<EventClusterDAO> _repoEventClusters;
        private ICosmosDBRepository<ResponseDAO> _repoResponses;
        private ICosmosDBRepository<Entity> _repoSagas;
        private ICosmosDBRepository<BotDAO> _repoBot;
        private ICosmosDBRepository<ChatReportDAO> _repoChatReports;
        private ICosmosDBRepository<DeviceDAO> _repoDevices;
        private ICosmosDBRepository<NotificationDAO> _repoNotifications;
        private ICosmosDBRepository<ActionPlanDAO> _repoActionPlans;
        private readonly IoTDeviceHelper _iotDeviceHelper;
        private const int SIMULATION_WINDOW_SIZE = 10;

        private readonly Random _rand = new Random();
        private bool _InterruptSimulation = false;

        public Application(ILogger<Application> logger, 
            ICosmosDBRepository<EventClusterDAO> repoEventClusters, ICosmosDBRepository<ResponseDAO> repoResponses, ICosmosDBRepository<Entity> repoSagas,
            ICosmosDBRepository<BotDAO> repoBot, ICosmosDBRepository<ChatReportDAO> repoChatReports, ICosmosDBRepository<DeviceDAO> repoDevices,
            ICosmosDBRepository<NotificationDAO> repoNotifications, ICosmosDBRepository<ActionPlanDAO> repoActionPlans, IoTDeviceHelper iotDeviceHelper)
        {
            _logger = logger;
            _repoEventClusters = repoEventClusters;
            _repoResponses = repoResponses;
            _repoSagas = repoSagas;
            _repoBot = repoBot;
            _repoChatReports = repoChatReports;
            _repoDevices = repoDevices;
            _repoNotifications = repoNotifications;
            _repoActionPlans = repoActionPlans;

            _iotDeviceHelper = iotDeviceHelper;
        }

        public async Task AddSensors(string path, bool overrideTags)
        {
            ConsoleHelper.WriteInfo($"Adding devices from file '{path}'...");
            using (TextReader fileReader = File.OpenText(path))
            {
                using (var csv = new CsvReader(fileReader))
                {
                    csv.Configuration.RegisterClassMap<IoTDevicetMap>();
                    var records = csv.GetRecords<IoTDevice>().ToList();
                    ConsoleHelper.WriteInfo($"{records.Count} devices found");

                    foreach(var iotDevice in records)
                    {
                        await _iotDeviceHelper.CreateDevice(iotDevice, overrideTags);
                    }
                    ConsoleHelper.WriteHighlight($"{records.Count} devices added.");
                }
            }

            await Task.Delay(5000);
        }

        public async Task RemoveSensors()
        {
            ConsoleHelper.WriteInfo($"Removing demo devices...");
            List<IoTDevice> iotDevices = await _iotDeviceHelper.GetDemoDevices();
            await _iotDeviceHelper.DeleteMultipleDevicesAsync(iotDevices);
            ConsoleHelper.WriteHighlight($"{iotDevices.Count} demo devices removed...");
            await Task.Delay(5000);
        }

        public async Task RemoveEventClusters()
        {
            ConsoleHelper.WriteInfo($"Removing eventclusters/responses/sagas...");
            await _repoEventClusters.DeleteItemsAsync(p => p.CreationDate != DateTime.MinValue);
            ConsoleHelper.WriteHighlight($"Eventclusters removed...");
            await _repoResponses.DeleteItemsAsync(p => p.CreationDate != DateTime.MinValue);
            ConsoleHelper.WriteHighlight($"Responses removed...");
            await _repoSagas.DeleteItemsAsync(p => p.Id != null);
            ConsoleHelper.WriteHighlight($"Sagas removed...");
            await Task.Delay(5000);
        }

        public async Task InitializeDB()
        {
            ConsoleHelper.WriteInfo($"Create Saga Collection");
            await _repoSagas.EnsureDatabaseAndCollectionExists();
            ConsoleHelper.WriteInfo($"Create Bot Collection");
            await _repoBot.EnsureDatabaseAndCollectionExists();
            ConsoleHelper.WriteInfo($"Create ChatReports Collection");
            await _repoChatReports.EnsureDatabaseAndCollectionExists();
            ConsoleHelper.WriteInfo($"Create Devices Collection");
            await _repoDevices.EnsureDatabaseAndCollectionExists();
            ConsoleHelper.WriteInfo($"Create Responses Collection");
            await _repoResponses.EnsureDatabaseAndCollectionExists();
            ConsoleHelper.WriteInfo($"Create EventClusters Collection");
            await _repoEventClusters.EnsureDatabaseAndCollectionExists();
            ConsoleHelper.WriteInfo($"Create Notifications Collection");
            await _repoNotifications.EnsureDatabaseAndCollectionExists();
            ConsoleHelper.WriteInfo($"Create ActionPlans Collection");
            await _repoActionPlans.EnsureDatabaseAndCollectionExists();

            foreach(var actionPlanDAO in ActionPlanDB.GetActionPlans())
                await _repoActionPlans.CreateItemAsync(actionPlanDAO);

            await Task.Delay(5000);
        }

        public async Task TestSensor()
        {
            List<IoTDevice> iotDevices = await _iotDeviceHelper.GetAllInputDevices();

            if (iotDevices == null || iotDevices.Count == 0)
            {
                ConsoleHelper.WriteWarning($"No devices found! Make sure to run '1. Setup list of devices' first.");
                return;
            }

            int i = 0;
            int valueParsed = 0;
            IoTDevice device = null;

            do
            {
                bool waitingForNumber = true;
                do
                {
                    ConsoleHelper.ClearConsole();
                    ConsoleHelper.WriteHighlight($"{i} messages sent from {(iotDevices.Count)} device(s).");
                    ConsoleHelper.WriteInfo("");
                    for (int n = 0; n < iotDevices.Count; n++)
                        ConsoleHelper.WriteInfo($"{n}. {iotDevices[n].DeviceType} {iotDevices[n].Name} [{iotDevices[n].Longitude}|{iotDevices[n].Latitude}]");
                    ConsoleHelper.WriteInfo("");
                    ConsoleHelper.WriteInfo("Type a number corresponding to the device to trigger. Or stop to terminate the simulation");
                    string value = Console.ReadLine();

                    if (int.TryParse(value, out valueParsed))
                        waitingForNumber = false;

                    if (value.ToLower() == "stop")
                        return;
                }
                while (waitingForNumber);

                device = iotDevices[valueParsed];

                string eventType = device.DeviceType == "SoundSensor" ? "Sound" : "Button";
                object messageObj = new ButtonSensorMessage();
                if (device.DeviceType == "SoundSensor")
                {
                    messageObj = new SoundSensorMessage()
                    {
                        Decibel = GetSoundMetadata()
                    };
                }
                await _iotDeviceHelper.SendMessage(device, eventType, messageObj);
                i++;

                await Task.Delay(1000);
            }
            while (true);
        }

        public async void TestSensors()
        {
            _InterruptSimulation = false;

            List<IoTDevice> iotDevices = await _iotDeviceHelper.GetAllInputDevices();

            if (iotDevices == null || iotDevices.Count == 0)
            {
                ConsoleHelper.WriteWarning($"No devices found! Make sure to run '1. Setup list of devices' first.");
                _InterruptSimulation = true;
                return;
            }

            int i = 0;
            IoTDevice device = null;

            do
            {
                device = iotDevices[_rand.Next(0, iotDevices.Count - 1)];

                string eventType = device.DeviceType == "SoundSensor" ? "Sound" : "Button";
                object messageObj = new ButtonSensorMessage();
                if(device.DeviceType == "SoundSensor")
                {
                    messageObj = new SoundSensorMessage()
                    {
                        Decibel = GetSoundMetadata()
                    };
                }
                await _iotDeviceHelper.SendMessage(device, eventType, messageObj);
                i++;

                ConsoleHelper.ClearConsole();
                ConsoleHelper.WriteInfo("Press Escape when you want to end the simulation.");
                ConsoleHelper.WriteInfo("");
                ConsoleHelper.WriteHighlight($"{i} messages sent from {(iotDevices.Count)} device(s).");
                ConsoleHelper.WriteInfo("");
                ConsoleHelper.WriteInfo($"Last message sent from {device.DeviceId}");
                ConsoleHelper.WriteInfo($"-Device Type: {device.DeviceType}");
                ConsoleHelper.WriteInfo($"-Event Type: {eventType}");
                ConsoleHelper.WriteInfo($"-Location Name: {device.Name}");
                ConsoleHelper.WriteInfo($"-Location 1: {device.Location1}");
                ConsoleHelper.WriteInfo($"-Location 2: {device.Location2}");
                ConsoleHelper.WriteInfo($"-Location 3: {device.Location3}");
                ConsoleHelper.WriteInfo($"-Latitude: {device.Latitude}");
                ConsoleHelper.WriteInfo($"-Longitude: {device.Longitude}");

                await Task.Delay(1000);
            }
            while (!_InterruptSimulation);
        }

        public async void MonitorLightBulbs()
        {
            _InterruptSimulation = false;

            List<IoTDevice> iotDevices = await _iotDeviceHelper.GetAllOutputDevices();

            if (iotDevices == null || iotDevices.Count == 0)
            {
                ConsoleHelper.WriteWarning($"No devices found! Make sure to run '1. Setup list of devices' first.");
                _InterruptSimulation = true;
                return;
            }

            do
            {
                ConsoleHelper.ClearConsole();
                Console.ResetColor();
                ConsoleHelper.WriteInfo("Press Escape when you want to end the simulation.");
                ConsoleHelper.WriteInfo("");
                for (int n = 0; n < iotDevices.Count; n++)
                {
                    var color = ConsoleColor.DarkGray;
                    if (iotDevices[n].Desired.ContainsKey("Color"))
                    {
                        switch (iotDevices[n].Desired["Color"])
                        {
                            case "red":
                                color = ConsoleColor.Red;
                                break;
                            case "green":
                                color = ConsoleColor.Green;
                                break;
                            case "blue":
                                color = ConsoleColor.Blue;
                                break;
                            case "purple":
                                color = ConsoleColor.Magenta;
                                break;
                            case "cyan":
                                color = ConsoleColor.Cyan;
                                break;
                            case "yellow":
                                color = ConsoleColor.Yellow;
                                break;
                            case "white":
                                color = ConsoleColor.White;
                                break;
                            case "off":
                            default:
                                color = ConsoleColor.DarkGray;
                                break;
                        }
                    }
                    Console.ForegroundColor = color;
                    ConsoleHelper.WriteInfo($"{iotDevices[n].DeviceType} {iotDevices[n].Name} [{iotDevices[n].Longitude}|{iotDevices[n].Latitude}] - {(iotDevices[n].Desired.ContainsKey("Color") ? iotDevices[n].Desired["Color"] : "NO DATA")}");
                }
                await Task.Delay(1000);
            }
            while (!_InterruptSimulation);

            Console.ResetColor();

        }

        public void InterruptSimulation()
        {
            _InterruptSimulation = true;
        }

        private List<StoryMessage> GetSimulationStory(string path)
        {
            ConsoleHelper.WriteInfo($"Getting simulation story from file '{path}'...");
            List<StoryMessage> storyMessages = null;
            using (TextReader fileReader = File.OpenText(path))
            {
                using (var csv = new CsvReader(fileReader))
                {
                    storyMessages = csv.GetRecords<StoryMessage>().ToList();
                    ConsoleHelper.WriteInfo($"{storyMessages.Count} story messages found.");
                }
            }

            return storyMessages;
        }

        public async void Simulate(string path)
        {
            _InterruptSimulation = false;

            //Get devices
            List<IoTDevice> iotDevices = await _iotDeviceHelper.GetAllInputDevices();
            if (iotDevices == null || iotDevices.Count == 0)
            {
                ConsoleHelper.WriteWarning($"No devices found! Make sure to run '1. Setup list of devices' first.");
                _InterruptSimulation = true;
                return;
            }

            //Get story messages and validation
            List<StoryMessage> storyMessages = GetSimulationStory(path);
            if(storyMessages == null || storyMessages.Count == 0)
            {
                ConsoleHelper.WriteWarning($"No story message found! Choose a valid csv file.");
                _InterruptSimulation = true;
                return;
            }

            List<string> deviceIds = storyMessages.GroupBy(p => p.DeviceId).Select(p => p.Key).ToList();
            if(deviceIds.Intersect(iotDevices.Select(p => p.DeviceId)).Count() != deviceIds.Count())
            {
                ConsoleHelper.WriteWarning($"Some devices in the story aren't properly set in IoT Hub. Please run the sensors csv file.");
                _InterruptSimulation = true;
                return;
            }

            int elapsedTime = 0;
            IoTDevice device = null;
            StoryMessage storyLog = null;

            for (int i = 0; i < storyMessages.Count; i++)
            {
                StoryMessage storyMessage = storyMessages[i];
                if (_InterruptSimulation)
                    return;

                if (storyMessage.Timestamp - elapsedTime > 0)
                {
                    await Task.Delay(storyMessage.Timestamp - elapsedTime);
                    elapsedTime += storyMessage.Timestamp - elapsedTime;
                }

                if (_InterruptSimulation)
                    return;

                device = iotDevices.Find(p => p.DeviceId == storyMessage.DeviceId);

                object messageObj = new ButtonSensorMessage();
                if (device.DeviceType == "SoundSensor")
                {
                    messageObj = new SoundSensorMessage()
                    {
                        Decibel = GetSoundMetadata()
                    };
                }
                await _iotDeviceHelper.SendMessage(device, storyMessage.EventType, messageObj);
                i++;

                int start = i > (SIMULATION_WINDOW_SIZE - 1) ? i - (SIMULATION_WINDOW_SIZE - 1) : 0;
                ConsoleHelper.ClearConsole();
                ConsoleHelper.WriteInfo("Press Escape when you want to end the simulation.");
                ConsoleHelper.WriteInfo("");
                ConsoleHelper.WriteHighlight($"{i} messages sent from {(deviceIds.Count)} demo device(s).");
                ConsoleHelper.WriteInfo("");
                ConsoleHelper.WriteInfo($"=================================");
                for(int j = start; j < start + SIMULATION_WINDOW_SIZE; j++)
                {
                    storyLog = storyMessages[j];
                    if ( j < i)
                        ConsoleHelper.WriteInfo($"--{storyLog.Timestamp}-{storyLog.DeviceId}");
                    else if (j > i)
                        ConsoleHelper.WriteInfo($"");
                    else
                        ConsoleHelper.WriteHighlight($"--{storyLog.Timestamp}-{storyLog.DeviceId}");
                }
                ConsoleHelper.WriteInfo($"=================================");
            }
            ConsoleHelper.WriteInfo("");
            ConsoleHelper.WriteHighlight("Done!");

            await Task.Delay(5000);
        }

        private double GetSoundMetadata()
        {
            return (_rand.NextDouble() * 20.00) + 120.00;
        }
    }
}
