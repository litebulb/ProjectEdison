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
        private List<string> _colors = new List<string>()
        {
            "off", "red", "green", "blue", "cyan", "yellow", "purple", "white"
        };
        private List<string> _states = new List<string>()
        {
            "off", "on", "flashing"
        };

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

                    if(action.Epicenter == null)
                    {
                        await context.Publish(new EventSagaReceiveResponseActionClosed(context.Message.IsCloseAction)
                        {
                            ResponseId = context.Message.ResponseId,
                            ActionId = context.Message.ActionId,
                            IsSuccessful = false,
                            IsSkipped = true,
                            ErrorMessage = "The response has no location. This action can not be executed."
                        });
                        _logger.LogDebug("ResponseActionLightSensorEventConsumer: The response has no location. This action can not be executed.");
                        return;
                    }

                    IEnumerable<Guid> devicesInRadius = await _deviceRestService.GetDevicesInRadius(new DeviceGeolocationModel()
                    {
                        DeviceType = "Lightbulb",
                        Radius = action.PrimaryRadius,
                        ResponseEpicenterLocation = action.Epicenter 
                    });

                    if(action.RadiusType == "secondary")
                    {
                        IEnumerable<Guid> devicesInSecondaryRadius = await _deviceRestService.GetDevicesInRadius(new DeviceGeolocationModel()
                        {
                            DeviceType = "Lightbulb",
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
                    if (!_states.Contains(action.State))
                        state = "off";
                    else
                        state = action.State;

                    //Color
                    string color = string.Empty;
                    action.Color = action.Color.ToLower();
                    if (!_colors.Contains(action.Color))
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
                        await context.Publish(new EventSagaReceiveResponseActionClosed(context.Message.IsCloseAction)
                        {
                            ResponseId = context.Message.ResponseId,
                            ActionId = context.Message.ActionId,
                            IsSuccessful = true
                        });
                        return;
                    }
                    else
                    {
                        await context.Publish(new EventSagaReceiveResponseActionClosed(context.Message.IsCloseAction)
                        {
                            ResponseId = context.Message.ResponseId,
                            ActionId = context.Message.ActionId,
                            IsSuccessful = false,
                            ErrorMessage = "Desired properties not applied properly."
                        });
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
