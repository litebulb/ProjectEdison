using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Edison.ChatService
{
    public class EdisonBot : IBot
    {
        private readonly ILogger<EdisonBot> _logger;

        public EdisonBot(ILogger<EdisonBot> logger)
        {
            _logger = logger;
        }

        #pragma warning disable 1998
        public async Task OnTurnAsync(ITurnContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            //Activity activity = context.Activity;
            //Activity reply = activity.CreateReply("The message was not handled properly.");
            //await context.SendActivityAsync(reply);
        }
    }
}
