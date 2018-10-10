using Edison.Common.Messages;
using Edison.Common.Messages.Interfaces;
using Edison.Core.Common.Models;
using Edison.Core.Interfaces;
using MassTransit;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Edison.ResponseService.Consumers
{
    public class ResponseActionLightSensorEventConsumer : IConsumer<IActionLightSensorEvent>
    {
        private readonly ILogger<ResponseActionLightSensorEventConsumer> _logger;
        private readonly IDeviceRestService _deviceRestService;
        private readonly IIoTHubControllerRestService _iotHubControllerRestService;

        public ResponseActionLightSensorEventConsumer(IDeviceRestService deviceRestService,
            IIoTHubControllerRestService iotHubControllerRestService,
            ILogger<ResponseActionLightSensorEventConsumer> logger)
        {
            _deviceRestService = deviceRestService;
            _iotHubControllerRestService = iotHubControllerRestService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IActionLightSensorEvent> context)
        {
            try
            {
                if (context.Message != null && context.Message as IActionLightSensorEvent != null)
                {
                    IActionLightSensorEvent action = context.Message;
                    _logger.LogDebug($"ResponseActionLightSensorEventConsumer: ActionId: '{action.ActionId}'.");

                    IEnumerable<Guid> devicesInRadius = await _deviceRestService.GetDevicesInRadius(new DeviceGeolocationModel()
                    {
                        FetchSensors = false,
                        Radius = action.PrimaryRadius,
                        ResponseEpicenterLocation = action.Epicenter 
                    });

                    if(action.RadiusType == "secondary")
                    {
                        IEnumerable<Guid> devicesInSecondaryRadius = await _deviceRestService.GetDevicesInRadius(new DeviceGeolocationModel()
                        {
                            FetchSensors = false,
                            Radius = action.SecondaryRadius,
                            ResponseEpicenterLocation = action.Epicenter
                        });
                        devicesInRadius = devicesInSecondaryRadius.Except(devicesInRadius);
                    }

                    if(devicesInRadius.ToList().Count == 0)
                    {
                        _logger.LogDebug($"ResponseActionLightSensorEventConsumer: No light in radius.");
                        return;
                    }

                    //State
                    string state = string.Empty;
                    action.State = action.State.ToLower();
                    if (action.State != "off" && action.State != "on" && action.State != "flashing")
                        state = "off";
                    else
                        state = action.State;

                    //Color
                    string color = string.Empty;
                    action.Color = action.Color.ToLower();
                    if (action.Color != "off" && action.Color != "blue" && action.Color != "green" && action.Color != "red" &&
                        action.Color != "white" && action.Color != "yellow" && action.Color != "cyan" && action.Color != "purple")
                        color = "off";
                    else
                        color = action.Color;

                    //Flash Frequency
                    int frequency = 0;
                    if(action.State == "flashing")
                    {
                        frequency = action.FlashFrequency;
                    }

                    bool result = await _iotHubControllerRestService.UpdateDevicesDesired(new DevicesUpdateDesiredModel()
                    {
                        DeviceIds = devicesInRadius.ToList(),
                        Desired = new Dictionary<string, object>()
                        {
                            { "State", state },
                            { "Color", color },
                            { "FlashFrequency", frequency }
                        }
                    });
                    if (result)
                    {
                        _logger.LogDebug($"ResponseActionLightSensorEventConsumer: Desired properties applied properly.");
                        return;
                        //TODO: Publish callback event
                    }
                    else
                    {
                        _logger.LogError("ResponseActionLightSensorEventConsumer: Desired properties not applied properly.");
                        throw new Exception("Desired properties not applied properly.");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"ResponseActionLightSensorEventConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
