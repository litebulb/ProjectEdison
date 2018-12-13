using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MassTransit;
using Edison.Core.Interfaces;
using Edison.Core.Common.Models;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;

namespace Edison.ResponseService.Consumers
{
    /// <summary>
    /// Masstransit consumer that handles the twilio action from a response
    /// </summary>
    public class ResponseActionTwilioEventConsumer : ResponseActionBaseConsumer, IConsumer<IActionTwilioEvent>
    {
        private readonly ITwilioRestService _twilioRestService;

        public ResponseActionTwilioEventConsumer(IResponseRestService responseRestService,
            ITwilioRestService twilioRestService,
            ILogger<ResponseActionTwilioEventConsumer> logger) : base(responseRestService, logger)
        {
            _twilioRestService = twilioRestService;
        }

        public async Task Consume(ConsumeContext<IActionTwilioEvent> context)
        {
            DateTime actionStartDate = DateTime.UtcNow;
            try
            {
                if (context.Message != null && context.Message as IActionTwilioEvent != null)
                {
                    IActionTwilioEvent action = context.Message;
                    _logger.LogDebug($"ResponseActionTwilioEventConsumer: ActionId: '{action.ActionId}'.");
                    _logger.LogDebug($"ResponseActionTwilioEventConsumer: Message: '{action.Message}'.");

                    TwilioModel result = await _twilioRestService.EmergencyCall(new TwilioModel()
                    {
                        Message = action.Message
                    });

                    //Success
                    if (result != null)
                    {
                        await GenerateActionCallback(context, ActionStatus.Success, actionStartDate);
                        _logger.LogDebug($"ResponseActionTwilioEventConsumer: SID: '{result.CallSID}' connected.");
                        return;
                    }

                    //Error
                    await GenerateActionCallback(context, ActionStatus.Error, actionStartDate, $"The twilio call could not be completed");
                    _logger.LogError("ResponseActionTwilioEventConsumer: The call could not be connected.");
                }
                _logger.LogError("ResponseActionTwilioEventConsumer: Invalid Null or Empty Action Notification");
                throw new Exception("Invalid or Null Action Twilio Call");
            }
            catch (Exception e)
            {
                await GenerateActionCallback(context, ActionStatus.Error, actionStartDate, $"There was an issue handling the action: {e.Message}.");
                _logger.LogError($"ResponseActionTwilioEventConsumer: {e.Message}");
                throw e;
            }
        }
    }
}
