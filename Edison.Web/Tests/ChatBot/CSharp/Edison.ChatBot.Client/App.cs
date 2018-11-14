using Edison.Core.Common.Models;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Edison.Core.Interfaces;
using Edison.Core;
using Microsoft.Extensions.Logging;
using Edison.ChatService.Helpers;

namespace Edison.ChatBot.Client
{
    public class Application
    {
        private readonly ClientConfig _config;
        private readonly IDirectLineRestService _directLineRestService;

        public Application(IOptions<ClientConfig> config, ILogger<RestServiceBase> logger)
        {
            _config = config.Value;
            _directLineRestService = new DirectLineRestService(_config.DirectLineServiceAPI, _config.BotSecret, logger);
        }

        /// <summary>
        /// Drives the user's conversation with the bot.
        /// </summary>
        /// <returns></returns>
        public async Task StartBotConversation()
        {
            //Get Token. Make sure that your b2c token is updated in appsettings!
            var tokenResults = await _directLineRestService.GenerateToken(new TokenConversationParameters()
            {
                User = new ChatUserModel()
                {
                    Id = _config.UserId,
                    Name = _config.Username,
                    Role = GetUserRoleFromString(_config.Role)
                }
            });

            // Create a new Direct Line client.
            DirectLineClient client = new DirectLineClient(tokenResults.Token);

            // Start the conversation.
            var conversation = await client.Conversations.StartConversationAsync();
            //keep conversationId for resume conversation + gethistory

            // Start the bot message reader in a separate thread.
            new System.Threading.Thread(async () => await ReadBotMessagesAsync(client, conversation.ConversationId)).Start();

            // Prompt the user to start talking to the bot.
            Console.Write("Type your message (or \"exit\" to end): ");

            Activity transcriptMessage = new Activity
            {
                From = new ChannelAccount(_config.UserId, _config.Username), // <-- mandatory
                Type = ActivityTypes.Message, // <-- mandatory
                ChannelData = new Command() // <--- use this to retrieve chat history 
                {
                    BaseCommand = Commands.GetTranscript,
                },
            };
            await client.Conversations.PostActivityAsync(conversation.ConversationId, transcriptMessage);

            // Loop until the user chooses to exit this loop.
            while (true)
            {
                // Accept the input from the user.
                string input = Console.ReadLine().Trim();

                // Check to see if the user wants to exit.
                if (input.ToLower() == "exit")
                {
                    // Exit the app if the user requests it.
                    break;
                }
                else
                {
                    if (input.Length > 0)
                    {
                        // Create a message activity with the text the user entered.
                        Activity userMessage = new Activity
                        {
                            From = new ChannelAccount(_config.UserId, _config.Username), // <-- mandatory
                            Text = input, // <-- not necessary to receive chat history
                            Type = ActivityTypes.Message, // <-- mandatory
                        };
                        userMessage.Properties["reportType"] = _config.ReportType;

                        // Send the message activity to the bot.
                        await client.Conversations.PostActivityAsync(conversation.ConversationId, userMessage);
                    }
                }
            }
        }


        /// <summary>
        /// Polls the bot continuously and retrieves messages sent by the bot to the client.
        /// </summary>
        /// <param name="client">The Direct Line client.</param>
        /// <param name="conversationId">The conversation ID.</param>
        /// <returns></returns>
        private async Task ReadBotMessagesAsync(DirectLineClient client, string conversationId)
        {
            string watermark = null;

            // Poll the bot for replies once per second.
            while (true)
            {
                // Retrieve the activity set from the bot.
                var activitySet = await client.Conversations.GetActivitiesAsync(conversationId, watermark);
                watermark = activitySet?.Watermark;

                // Extract the activies sent from our bot.
                var activities = from x in activitySet.Activities
                                 where x.From.Id == _config.BotId
                                 select x;

                // Analyze each activity in the activity set.
                foreach (Activity activity in activities)
                {
                    // Display the text of the activity.
                    Console.WriteLine(activity.Text);
                    // Redisplay the user prompt.
                    Console.Write("\nType your message (\"exit\" to end): ");
                }

                // Wait for one second before polling the bot again.
                await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
            }
        }

        private ChatUserRole GetUserRoleFromString(string roleStr)
        {
            Enum.TryParse(typeof(ChatUserRole), roleStr, out object role);
            if (role == null)
                role = ChatUserRole.Consumer;
            return (ChatUserRole)role;
        }
    }
}
