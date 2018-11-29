using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MassTransit;
using Edison.Core.Interfaces;
using Edison.Core.Common.Models;
using Edison.Common.Messages.Interfaces;
using Edison.Common.Messages;

namespace Edison.ResponseService.Consumers
{
    /// <summary>
    /// Masstransit base consumer for actions.
    /// </summary>
    public class ResponseActionBaseConsumer
    {
        protected readonly ILogger<ResponseActionBaseConsumer> _logger;
        private readonly IResponseRestService _responseRestService;

        public ResponseActionBaseConsumer(IResponseRestService responseRestService,
            ILogger<ResponseActionBaseConsumer> logger)
        {
            _responseRestService = responseRestService;
            _logger = logger;
        }

        protected async Task GenerateActionCallback<T>(ConsumeContext<T> context, ActionStatus status, DateTime startDate, string errorMessage = "") where T : class, IActionBaseEvent
        {
            DateTime endDate = DateTime.UtcNow;
            await context.Publish(new EventSagaReceiveResponseActionCallback()
            {
                IsCloseAction = context.Message.IsCloseAction,
                ResponseId = context.Message.ResponseId,
                ActionId = context.Message.ActionId,
                Status = status,
                StartDate = startDate,
                EndDate = endDate,
                ErrorMessage = errorMessage
            });

            bool result = await _responseRestService.CompleteAction(new ActionCompletionModel()
            {
                ResponseId = context.Message.ResponseId,
                ActionId = context.Message.ActionId,
                Status = status,
                StartDate = startDate,
                EndDate = endDate,
                ErrorMessage = errorMessage
            });

            if (!result)
                _logger.LogError($"CompleteAction for Action '{context.Message.ActionId}' could not be completed.");
        }
    }
}
