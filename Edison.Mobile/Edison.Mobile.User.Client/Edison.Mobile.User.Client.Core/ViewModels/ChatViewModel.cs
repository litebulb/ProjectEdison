using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Edison.Core.Common.Models;
using Edison.Mobile.User.Client.Core.Network;
using Edison.Mobile.User.Client.Core.Chat;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json.Linq;
using Edison.Mobile.Common.Geo;
using Edison.Mobile.Common.Notifications;
using Edison.Mobile.Common.Auth;
using Edison.Mobile.Common.Network;
using Timer = System.Timers.Timer;
using System.Linq;
using Edison.Mobile.User.Client.Core.Shared;
using Edison.Mobile.Common.ViewModels;

namespace Edison.Mobile.User.Client.Core.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        readonly ChatRestService chatRestService;
        readonly LocationRestService locationRestService;
        readonly ILocationService locationService;
        readonly INotificationService notificationService;
        readonly ActionPlanRestService actionPlanRestService;
        readonly ResponseRestService responseRestService;
        readonly AuthService authService;
        readonly ChatClientConfig chatClientConfig;
        readonly Timer geolocationTimer;

        bool isInConversation;
        bool isSafe;
        string chatWatermark;
        Task readMessagesTask;
        CancellationTokenSource readMessagesCancellationTokenSource;
        ActionPlanListModel currentActionPlan;

        DirectLineClient client;
        Conversation conversation;

        public ChatUserTokenContext ChatTokenContext { get; set; }

        public bool IsSafe 
        {
            get => isSafe;
            set 
            {
                isSafe = value;
                OnIsSafeChanged?.Invoke(this, isSafe);
            }
        }

        public ObservableRangeCollection<ChatMessage> ChatMessages { get; } = new ObservableRangeCollection<ChatMessage>();
        public ObservableRangeCollection<ActionPlanListModel> ActionPlans { get; } = new ObservableRangeCollection<ActionPlanListModel>();

        public ObservableRangeCollection<ChatPromptType> ChatPromptTypes { get; } = new ObservableRangeCollection<ChatPromptType>
        {
            ChatPromptType.Emergency,
            ChatPromptType.ReportActivity,
        };

        public ActionPlanListModel CurrentActionPlan 
        {
            get => currentActionPlan;
            set 
            {
                currentActionPlan = value;
                OnCurrentActionPlanChanged?.Invoke(this, currentActionPlan);
            }
        }

        public event EventHandler<ActionPlanListModel> OnCurrentActionPlanChanged;
        public event EventHandler<ChatPromptType> OnChatPromptActivated;
        public event EventHandler<bool> OnIsSafeChanged;

        public string Initials => authService.Initials;

        public ChatViewModel(
            ChatRestService chatRestService,
            ChatClientConfig chatClientConfig,
            INotificationService notificationService,
            AuthService authService,
            LocationRestService locationRestService,
            ILocationService locationService,
            ActionPlanRestService actionPlanRestService,
            ResponseRestService responseRestService
        )
        {
            this.chatRestService = chatRestService;
            this.locationRestService = locationRestService;
            this.chatClientConfig = chatClientConfig;
            this.notificationService = notificationService;
            this.authService = authService;
            this.actionPlanRestService = actionPlanRestService;
            this.locationService = locationService;
            this.responseRestService = responseRestService;

            geolocationTimer = new Timer(Shared.Constants.UpdateLocationTimerInterval);
            geolocationTimer.Elapsed += HandleGeolocationTimer;
        }

        public async override void ViewAppeared()
        {
            base.ViewAppeared();

            var actionPlans = await actionPlanRestService.GetActionPlans();

            ActionPlans.AddRange(actionPlans);

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

        public async Task<bool> SendMessage(string message, bool isPromptedFromActionPlanButton = false)
        {
            var newActivity = new Activity
            {
                From = new ChannelAccount(chatClientConfig.UserId, chatClientConfig.Username),
                Type = ActivityTypes.Message,
                Text = message,
            };

            if (isPromptedFromActionPlanButton)
            {
                newActivity.Properties["reportType"] = CurrentActionPlan?.ActionPlanId ?? GetEmergencyActionPlan().ActionPlanId;
            }

            newActivity.Properties["deviceId"] = chatClientConfig.DeviceId;

            var response = await client.Conversations.PostActivityAsync(conversation.ConversationId, newActivity);
            return response != null;
        }

        public async Task ActivateChatPrompt(ChatPromptType chatPromptType) 
        {
            OnChatPromptActivated?.Invoke(this, chatPromptType);

            if (chatPromptType == ChatPromptType.Emergency) 
            {
                BeginConversationWithActionPlan(GetEmergencyActionPlan());
            }
            else if (chatPromptType == ChatPromptType.SafetyCheck) 
            {
                IsSafe = !IsSafe;
                await responseRestService.SendIsSafe(IsSafe);
            }
            else if (chatPromptType == ChatPromptType.ReportActivity) 
            {

            }
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

        public void BeginConversationWithActionPlan(ActionPlanListModel actionPlanListModel = null)
        {
            var actionPlan = actionPlanListModel ?? GetEmergencyActionPlan();
            CurrentActionPlan = actionPlan;

            Task.Run(async () => 
            {
                await locationRestService.UpdateDeviceLocation(new Geolocation 
                {
                    Latitude = locationService.LastKnownLocation.Latitude,
                    Longitude = locationService.LastKnownLocation.Longitude,
                });

                await SendMessage(CurrentActionPlan.Name, true);
            });
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
                    ActionPlanListModel previousActionPlan = null;
                    foreach (var activity in activitySet.Activities)
                    {
                        if (activity.ChannelData is JObject channelData)
                        {
                            Enum.TryParse((string)channelData["baseCommand"], out Commands command);

                            if (command == Commands.SendMessage)
                            {
                                var sendMessageProperties = channelData["data"].ToObject<CommandSendMessageProperties>();
                                var actionPlan = ActionPlans.FirstOrDefault(a => a.ActionPlanId.ToString() == sendMessageProperties.ReportType); // reportType is only populated by action plan button press
                                var isMyChatId = IsMyChatId(sendMessageProperties.From.Id);

                                if (isMyChatId && actionPlan != null)
                                {
                                    CurrentActionPlan = actionPlan;
                                }

                                chatMessages.Add(new ChatMessage
                                {
                                    Text = activity.Text,
                                    UserModel = sendMessageProperties.From,
                                    ActionPlan = actionPlan,
                                    IsNewActionPlan = isMyChatId && CurrentActionPlan != null && previousActionPlan != CurrentActionPlan,
                                });
                            }
                        }
                        else if (IsMyChatId(activity.From.Id))
                        {
                            var previouslySentMessage = ChatMessages.LastOrDefault(m => IsMyChatId(m.UserModel.Id));
                            var isNewActionPlan = previouslySentMessage == null || previouslySentMessage.ActionPlan != CurrentActionPlan;

                            chatMessages.Add(new ChatMessage
                            {
                                Text = activity.Text,
                                UserModel = new ChatUserModel
                                {
                                    Id = activity.From.Id,
                                    Name = activity.From.Name,
                                    Role = ChatUserRole.Consumer,
                                },
                                ActionPlan = CurrentActionPlan,
                                IsNewActionPlan = isNewActionPlan,
                            });
                        }

                        isEndingConversation = activity.Type == "endOfConversation";
                        previousActionPlan = CurrentActionPlan;
                    }

                    if (chatMessages.Count > 0)
                    {
                        if (!isInConversation)
                        {
                            ChatMessages.Clear();
                        }

                        isInConversation = true;
                        ChatMessages.AddRange(chatMessages);
                    }

                    if (isEndingConversation)
                    {
                        isInConversation = false;
                        CurrentActionPlan = null;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                await Task.Delay(TimeSpan.FromSeconds(1), readMessagesCancellationTokenSource.Token).ConfigureAwait(false);
            }
        }

        bool IsMyChatId(string chatId) => chatId.Contains(authService.UserInfo?.Email);

        ActionPlanListModel GetEmergencyActionPlan() => ActionPlans.First(a => a.Name == "Emergency"); // TODO: no magic strings
    }
}
