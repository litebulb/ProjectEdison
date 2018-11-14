using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.User.Client.Core.Network;
using Edison.Mobile.User.Client.Core.Chat;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json.Linq;
using Edison.Mobile.Common.Shared;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.Common.Notifications;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Network;
using Timer = System.Timers.Timer;

namespace Edison.Mobile.User.Client.Core.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        readonly ChatRestService chatRestService;
        readonly LocationRestService locationRestService;
        readonly ILocationService locationService;
        readonly INotificationService notificationService;
        readonly AuthService authService;
        readonly ChatClientConfig chatClientConfig;
        readonly Timer geolocationTimer;

        string chatWatermark;
        Task readMessagesTask;
        CancellationTokenSource readMessagesCancellationTokenSource;

        DirectLineClient client;
        Conversation conversation;

        public ChatUserTokenContext ChatTokenContext { get; set; }

        public ObservableRangeCollection<ChatMessage> ChatMessages { get; } = new ObservableRangeCollection<ChatMessage>();

        public string Initials => authService.Initials;

        public ChatViewModel(
            ChatRestService chatRestService,
            ChatClientConfig chatClientConfig,
            INotificationService notificationService,
            AuthService authService,
            LocationRestService locationRestService,
            ILocationService locationService
        )
        {
            this.chatRestService = chatRestService;
            this.locationRestService = locationRestService;
            this.chatClientConfig = chatClientConfig;
            this.notificationService = notificationService;
            this.authService = authService;
            this.locationService = locationService;

            geolocationTimer = new Timer(User.Client.Core.Shared.Constants.UpdateLocationTimerInterval);
            geolocationTimer.Elapsed += HandleGeolocationTimer;
        }

        public async override void ViewAppeared()
        {
            base.ViewAppeared();

            ChatTokenContext = await chatRestService.GetToken();

            if (ChatTokenContext != null)
            {
                chatClientConfig.UserId = ChatTokenContext.UserContext?.Id;
                chatClientConfig.Username = ChatTokenContext.UserContext?.Name;
                chatClientConfig.Role = ChatTokenContext.UserContext?.Role.ToString();

                client = new DirectLineClient(ChatTokenContext.Token);
                readMessagesCancellationTokenSource?.Cancel();
                readMessagesCancellationTokenSource = new CancellationTokenSource();
                conversation = await client.Conversations.StartConversationAsync();
                readMessagesTask = Task.Run(async () => await ReadBotMessagesAsync(client, conversation.ConversationId), readMessagesCancellationTokenSource.Token);
                await GetChatTranscript();
            }
        }

        public override void ViewDisappearing()
        {
            base.ViewDisappearing();
            readMessagesCancellationTokenSource.Cancel();
        }

        public async Task<bool> SendMessage(string message)
        {
            var userMessage = new Activity
            {
                From = new ChannelAccount(chatClientConfig.UserId, chatClientConfig.Username),
                Type = ActivityTypes.Message,
                Text = message,
            };

            userMessage.Properties["reportType"] = chatClientConfig.ReportType;
            userMessage.Properties["deviceId"] = chatClientConfig.DeviceId;

            var response = await client.Conversations.PostActivityAsync(conversation.ConversationId, userMessage);
            return response != null;
        }

        public void ChatSummoned()
        {
            Task.Run(async () => await UpdateDeviceLocation());
            geolocationTimer.Start();
        }

        public void ChatDismissed()
        {
            geolocationTimer.Stop();
        }

        async void HandleGeolocationTimer(object sender, EventArgs e)
        {
            await UpdateDeviceLocation();
        }

        async Task UpdateDeviceLocation() => await locationRestService.UpdateDeviceLocation(new Geolocation
        {
            Latitude = locationService.LastKnownLocation.Latitude,
            Longitude = locationService.LastKnownLocation.Longitude,
        });

        async Task GetChatTranscript()
        {
            var transcriptActivity = new Activity
            {
                From = new ChannelAccount(chatClientConfig.UserId, chatClientConfig.Username),
                Type = ActivityTypes.Message,
                ChannelData = new Command
                {
                    BaseCommand = Commands.GetTranscript,
                },
            };

            var response = await client.Conversations.PostActivityAsync(conversation.ConversationId, transcriptActivity);
            Console.WriteLine(response);
        }

        async Task ReadBotMessagesAsync(DirectLineClient directLineClient, string conversationId)
        {
            while (!readMessagesCancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var activitySet = await directLineClient.Conversations.GetActivitiesAsync(conversationId, chatWatermark);

                    chatWatermark = activitySet?.Watermark;

                    var chatMessages = new List<ChatMessage>();
                    var isEndingConversation = false;
                    foreach (var activity in activitySet.Activities)
                    {
                        if (activity.ChannelData is JObject channelData)
                        {
                            Enum.TryParse((string)channelData["baseCommand"], out Commands command);

                            if (command == Commands.SendMessage)
                            {
                                var sendMessageProperties = channelData["data"].ToObject<CommandSendMessageProperties>();
                                chatMessages.Add(new ChatMessage
                                {
                                    Text = activity.Text,
                                    UserModel = sendMessageProperties.From,
                                });
                            }
                            else if (command == Commands.EndConversation)
                            {
                                isEndingConversation = true;
                                break;
                            }
                        }
                        else if (IsMyChatId(activity.From.Id))
                        {
                            chatMessages.Add(new ChatMessage
                            {
                                Text = activity.Text,
                                UserModel = new ChatUserModel
                                {
                                    Id = activity.From.Id,
                                    Name = activity.From.Name,
                                    Role = ChatUserRole.Consumer,
                                },
                            });
                        }
                    }

                    if (isEndingConversation)
                    {
                        ChatMessages.Clear();
                    }
                    else if (chatMessages.Count > 0)
                    {
                        ChatMessages.AddRange(chatMessages);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                await Task.Delay(TimeSpan.FromSeconds(1), readMessagesCancellationTokenSource.Token).ConfigureAwait(false);
            }
        }

        bool IsMyChatId(string chatId) => chatId.Contains(authService.Email);
    }
}
