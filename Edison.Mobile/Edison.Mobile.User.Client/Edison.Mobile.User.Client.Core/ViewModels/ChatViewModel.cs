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
        public Uri ProfileImageUri => null;  // authService.ProfileImageUri or add from local file system- needs to be added
        public string Email => authService.UserInfo?.Email;

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

            if (actionPlans != null)
            {
                ActionPlans.Clear();
                ActionPlans.AddRange(actionPlans);
            }

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
            readMessagesCancellationTokenSource?.Cancel();
        }

        public async Task<bool> SendMessage(string message, bool isPromptedFromActionPlanButton = false)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("***************************************************");
            System.Diagnostics.Debug.WriteLine("***********  Entered Send Message Method  *********");
            System.Diagnostics.Debug.WriteLine("***************************************************");

            if (chatClientConfig == null)
                System.Diagnostics.Debug.WriteLine("***********  ChatClientConfig = NULL *********");
            else
                System.Diagnostics.Debug.WriteLine("***********  ChatClientConfig OK *********");
#endif
            var newActivity = new Activity
            {
                From = new ChannelAccount(chatClientConfig.UserId, chatClientConfig.Username),
                Type = ActivityTypes.Message,
                Text = message,
            };

#if DEBUG
            System.Diagnostics.Debug.WriteLine("***********  Chat 'Activity' created  *********");
#endif
            ActionPlanListModel messageActionPlan = null;
            if (isPromptedFromActionPlanButton)
            {
                messageActionPlan = CurrentActionPlan ?? GetEmergencyActionPlan();
                newActivity.Properties["reportType"] = CurrentActionPlan?.ActionPlanId ?? GetEmergencyActionPlan()?.ActionPlanId;
            }

            newActivity.Properties["deviceId"] = chatClientConfig?.DeviceId;

#if DEBUG
            System.Diagnostics.Debug.WriteLine("*************************************************");
            System.Diagnostics.Debug.WriteLine("*********  ChatViewModel - SendMessage  *********");
            System.Diagnostics.Debug.WriteLine("*********  Message = " + message);
            System.Diagnostics.Debug.WriteLine("*********  Chat UserId = " + chatClientConfig.UserId);
            System.Diagnostics.Debug.WriteLine("*********  Chat UserName = " + chatClientConfig.Username);
            System.Diagnostics.Debug.WriteLine("*********  Chat DeviceId = " + chatClientConfig.DeviceId.ToString());
            if (conversation == null)
                System.Diagnostics.Debug.WriteLine("*********  NO CONVERSATION OBJECT  *********");
            else
            {
                System.Diagnostics.Debug.WriteLine("*********  CONVERSATION OBJECT EXISTS  *********");
                if (conversation.ConversationId == null)
                    System.Diagnostics.Debug.WriteLine("*********  NO CONVERSATION ID  *********");
                else
                {
                    System.Diagnostics.Debug.WriteLine("*********  CONVERSATION ID EXISTS  *********");
                    System.Diagnostics.Debug.WriteLine("*********  ConversationId = " + conversation.ConversationId);
                }
            }
            if (ChatTokenContext == null)
                System.Diagnostics.Debug.WriteLine("*********  NO CHAT TOKEN CONTEXT  *********");
            else
            {
                System.Diagnostics.Debug.WriteLine("*********  CHAT TOKEN CONTEXT EXISTS  *********");
                if (ChatTokenContext.Token == null)
                    System.Diagnostics.Debug.WriteLine("*********  NO TOKEN  *********");
                else
                {
                    System.Diagnostics.Debug.WriteLine("*********  TOKEN EXSISTS  *********");
                    System.Diagnostics.Debug.WriteLine("*********  ChatTokenContext Token = " + ChatTokenContext.Token);
                }
                if (ChatTokenContext.ConversationId == null)
                    System.Diagnostics.Debug.WriteLine("*********  NO CONVERSATION ID  *********");
                else
                {
                    System.Diagnostics.Debug.WriteLine("*********  CONVERSATION ID EXISTS  *********");
                    System.Diagnostics.Debug.WriteLine("*********  ChatTokenContext ConversationId = " + ChatTokenContext.ConversationId);
                }
            }
            System.Diagnostics.Debug.WriteLine("*************************************************");
#endif

            // Mark and display message
            newActivity.Properties[Constants.ChatActivityMessageType] = Constants.ChatActivityClientTextMessage;
            var messageId = Guid.NewGuid().ToString();
            newActivity.Properties[Constants.ChatActivityMessageId] = messageId;
            var clientMessage = new ChatMessage
            {
                Text = newActivity.Text,
                UserModel = ChatTokenContext.UserContext,
                ActionPlan = messageActionPlan,
                IsNewActionPlan = messageActionPlan != CurrentActionPlan,
                Id = messageId
            };
            ChatMessages.Add(clientMessage);
            var index = ChatMessages.Count - 1;
            newActivity.Properties[Constants.ChatActivityMessageIndex] = index;

            ResourceResponse response = null;
            try
            {
                response = await client.Conversations.PostActivityAsync(conversation.ConversationId, newActivity);
            }
            catch (Exception ex)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("****************************************************");
                System.Diagnostics.Debug.WriteLine("*********  ChatViewModel - Send Exception  *********");
                System.Diagnostics.Debug.WriteLine("*********  Exception: " + ex.ToString());
                System.Diagnostics.Debug.WriteLine("****************************************************");
#endif
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine("***************************************************");
            System.Diagnostics.Debug.WriteLine("*********  ChatViewModel - Message Sent  *********");
            if (response == null)
                System.Diagnostics.Debug.WriteLine("*********  FAILED");
            else
                System.Diagnostics.Debug.WriteLine("*********  SUCCESS");
            System.Diagnostics.Debug.WriteLine("***************************************************");
#endif

            if (response == null && ChatMessages.Count > index)
                // remove message
                ChatMessages.RemoveAt(index);

            return response != null;
        }

        public async Task ActivateChatPrompt(ChatPromptType chatPromptType) 
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("********************************************************");
            System.Diagnostics.Debug.WriteLine("*********  ChatViewModel - ActivateChatPrompt  *********");
            System.Diagnostics.Debug.WriteLine("*********  ChatPromptType = " + chatPromptType.ToString());
            System.Diagnostics.Debug.WriteLine("********************************************************");
#endif

            OnChatPromptActivated?.Invoke(this, chatPromptType);
            switch (chatPromptType)
            {
                case ChatPromptType.Emergency:
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("********************************************************************");
                    System.Diagnostics.Debug.WriteLine("*********  ChatViewModel - Calling GetEmergencyActionPlan  *********");
                    if (ActionPlans == null)
                        System.Diagnostics.Debug.WriteLine("*********  ActionPlans = null");
                    else if (ActionPlans.Count == 0)
                        System.Diagnostics.Debug.WriteLine("*********  ActionPlans = 0");
                    else
                        System.Diagnostics.Debug.WriteLine("*********  ActionPlans exist");
                    System.Diagnostics.Debug.WriteLine("********************************************************************");
#endif
                    ActionPlanListModel actionPlan = null;
                    try
                    {
                        actionPlan = GetEmergencyActionPlan();
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("*************************************************************");
                        System.Diagnostics.Debug.WriteLine("********* Exception calling GetEmergencyActionPlan  *********");
                        System.Diagnostics.Debug.WriteLine("*************************************************************");
#endif
                    }

#if DEBUG
                    System.Diagnostics.Debug.WriteLine("***************************************");
                    if (actionPlan == null)
                        System.Diagnostics.Debug.WriteLine("*********  ActionPlan = null  *********");
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("*********   ActionPlan Exist  *********");
                        System.Diagnostics.Debug.WriteLine("*********   ActionPlan id= " + actionPlan.ActionPlanId.ToString());
                    }
                    System.Diagnostics.Debug.WriteLine("***************************************");
#endif
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("*****************************************************************************");
                    System.Diagnostics.Debug.WriteLine("*********  ChatViewModel - Calling BeginConversationWithActionPlan  *********");
                    System.Diagnostics.Debug.WriteLine("*****************************************************************************");
#endif
                    BeginConversationWithActionPlan(GetEmergencyActionPlan());
                    break;
                case ChatPromptType.SafetyCheck:
                    IsSafe = !IsSafe;
                    await responseRestService.SendIsSafe(IsSafe);
                    break;
                case ChatPromptType.ReportActivity:
                    break;
                default:
                    break;
            }
        }


        public async Task ActivateChatPromptAsync(ChatPromptType chatPromptType)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("********************************************************");
            System.Diagnostics.Debug.WriteLine("*********  ChatViewModel - ActivateChatPrompt  *********");
            System.Diagnostics.Debug.WriteLine("*********  ChatPromptType = " + chatPromptType.ToString());
            System.Diagnostics.Debug.WriteLine("********************************************************");
#endif

            OnChatPromptActivated?.Invoke(this, chatPromptType);
            switch (chatPromptType)
            {
                case ChatPromptType.Emergency:
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("********************************************************************");
                    System.Diagnostics.Debug.WriteLine("*********  ChatViewModel - Calling GetEmergencyActionPlan  *********");
                    if (ActionPlans == null)
                        System.Diagnostics.Debug.WriteLine("*********  ActionPlans = null");
                    else if (ActionPlans.Count == 0)
                        System.Diagnostics.Debug.WriteLine("*********  ActionPlans = 0");
                    else
                        System.Diagnostics.Debug.WriteLine("*********  ActionPlans exist");
                    System.Diagnostics.Debug.WriteLine("********************************************************************");
#endif

                    ActionPlanListModel actionPlan = null;
                    try
                    {
                        actionPlan = GetEmergencyActionPlan();
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("*************************************************************");
                        System.Diagnostics.Debug.WriteLine("********* Exception calling GetEmergencyActionPlan  *********");
                        System.Diagnostics.Debug.WriteLine("*************************************************************");
#endif
                    }

#if DEBUG
                    System.Diagnostics.Debug.WriteLine("***************************************");
                    if (actionPlan == null)
                        System.Diagnostics.Debug.WriteLine("*********  ActionPlan = null  *********");
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("*********   ActionPlan Exist  *********");
                        System.Diagnostics.Debug.WriteLine("*********   ActionPlan id= " + actionPlan.ActionPlanId.ToString());
                    }
                    System.Diagnostics.Debug.WriteLine("***************************************");
#endif
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("*****************************************************************************");
                    System.Diagnostics.Debug.WriteLine("*********  ChatViewModel - Calling BeginConversationWithActionPlan  *********");
                    System.Diagnostics.Debug.WriteLine("*****************************************************************************");
#endif

                    await BeginConversationWithActionPlanAsync(GetEmergencyActionPlan());
                    break;
                case ChatPromptType.SafetyCheck:
                    IsSafe = !IsSafe;
                    var success = await responseRestService.SendIsSafe(IsSafe);
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("*****************************************************************************");
                    if (success)
                        System.Diagnostics.Debug.WriteLine("*********  Safe Sent: SUCCESS  *********");
                    else
                        System.Diagnostics.Debug.WriteLine("*********  Safe Sent: FAILED  *********");
                    System.Diagnostics.Debug.WriteLine("*****************************************************************************");
#endif
                    break;
                case ChatPromptType.ReportActivity:
                    break;
                default:
                    break;
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
#if DEBUG
            System.Diagnostics.Debug.WriteLine("*********************************************************************");
            System.Diagnostics.Debug.WriteLine("*********  ChatViewModel - BeginConversationWithActionPlan  *********");
            System.Diagnostics.Debug.WriteLine("*********************************************************************");
#endif

            var actionPlan = actionPlanListModel ?? GetEmergencyActionPlan();
            CurrentActionPlan = actionPlan;

            Task.Run(async () => 
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("***************************************************");
                System.Diagnostics.Debug.WriteLine("***********  Calling UpdateDeviceLocation  *********");
                System.Diagnostics.Debug.WriteLine("***************************************************");
#endif

#if DEBUG
                System.Diagnostics.Debug.WriteLine("**************************************************************************");
                System.Diagnostics.Debug.WriteLine("*********  ChatViewModel - BeginConversationWithActionPlanAsync  *********");
                if (locationService?.LastKnownLocation == null)
                    System.Diagnostics.Debug.WriteLine("*********  LastKnowLocation = NULL");
                else
                    System.Diagnostics.Debug.WriteLine("*********  lastKnowLocation = Lat: " + locationService.LastKnownLocation.Latitude + "    Long: " + locationService.LastKnownLocation.Longitude);
                System.Diagnostics.Debug.WriteLine("**************************************************************************");
#endif
                var location = await locationService.GetLastKnownLocationAsync();
#if DEBUG
                System.Diagnostics.Debug.WriteLine("**************************************************************************");
                System.Diagnostics.Debug.WriteLine("*********  ChatViewModel - BeginConversationWithActionPlanAsync  *********");
                System.Diagnostics.Debug.WriteLine("*********  Location = Lat: " + location.Latitude + "    Long: " + location.Longitude);
                System.Diagnostics.Debug.WriteLine("**************************************************************************");
#endif

                await locationRestService.UpdateDeviceLocation(new Geolocation
                {
                    //                Latitude = locationService.LastKnownLocation.Latitude,
                    //                Longitude = locationService.LastKnownLocation.Longitude,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                });

#if DEBUG
                System.Diagnostics.Debug.WriteLine("***************************************************");
                System.Diagnostics.Debug.WriteLine("***********  Calling Send Message Method  *********");
                System.Diagnostics.Debug.WriteLine("***************************************************");
#endif
                await SendMessage(CurrentActionPlan.Name, true);
            });
        }

        public async Task BeginConversationWithActionPlanAsync(ActionPlanListModel actionPlanListModel = null)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("**************************************************************************");
            System.Diagnostics.Debug.WriteLine("*********  ChatViewModel - BeginConversationWithActionPlanAsync  *********");
            System.Diagnostics.Debug.WriteLine("**************************************************************************");
#endif
            var actionPlan = actionPlanListModel ?? GetEmergencyActionPlan();
            CurrentActionPlan = actionPlan;

            var location = await locationService.GetLastKnownLocationAsync();

#if DEBUG
            System.Diagnostics.Debug.WriteLine("**************************************************************************");
            System.Diagnostics.Debug.WriteLine("*********  ChatViewModel - BeginConversationWithActionPlanAsync  *********");
            System.Diagnostics.Debug.WriteLine("*********  Location = Lat: " + location.Latitude + "    Long: " + location.Longitude);
            System.Diagnostics.Debug.WriteLine("**************************************************************************");
#endif

            await locationRestService.UpdateDeviceLocation(new Geolocation 
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
            });

            var name = CurrentActionPlan == null ? "Emergency" : CurrentActionPlan.Name;
            await SendMessage(name, true);
        }



        async void HandleGeolocationTimer(object sender, EventArgs e)
        {
            await UpdateDeviceLocation();
        }

        async Task UpdateDeviceLocation()
        {
            if (locationService.LastKnownLocation != null)
            {
                await locationRestService.UpdateDeviceLocation(new Geolocation
                {
                    Latitude = locationService.LastKnownLocation.Latitude,
                    Longitude = locationService.LastKnownLocation.Longitude,
                });
            }
            else
            {
                var location = await locationService.GetLastKnownLocationAsync();
                await locationRestService.UpdateDeviceLocation(new Geolocation
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude
                });
            }
        }


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
#if DEBUG
            Console.WriteLine(response);
#endif
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
                                    CurrentActionPlan = actionPlan;

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

                            var message = new ChatMessage
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
                            };

                            // Check to see if the message was a client sent message that is already in ChatMessage collection
                            bool alreadyDisplayed = false;
                            int index = -1;
                            string id = null;
                            activity.Properties.TryGetValue(Constants.ChatActivityMessageType, out JToken token1);
                            activity.Properties.TryGetValue(Constants.ChatActivityMessageId, out JToken token2);
                            if (token1 != null && token1.ToObject<string>() == Constants.ChatActivityClientTextMessage && token2 != null)
                            {
                                // Is a message that was sent by the client and reflected back by chat bot service
                                alreadyDisplayed = true;
                                id = token2.ToObject<string>();
                                activity.Properties.TryGetValue(Constants.ChatActivityMessageIndex, out JToken token3);
                                if (token3 != null)
                                    index = token3.ToObject<int>();
                            }

                            if (alreadyDisplayed)
                            {
                                // try to locate message in ChatMessages and replace with the one reflected back
                                if (ChatMessages.Count > index)
                                {
                                    // iterate from index to end of chat message just in case messages have somehow been added - normally index will be last item
                                    for (int i = index; i < ChatMessages.Count; i++)
                                    {
                                        if (ChatMessages[i].Id == id)
                                        {
                                            // have found message, so replace
                                            ChatMessages[i] = message;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // count is last than index so just the last item
                                    if (ChatMessages.LastOrDefault()?.Id == id)
                                            ChatMessages[ChatMessages.Count-1] = message;
                                }


                            }
                            else
                                chatMessages.Add(message);
                        }

                        isEndingConversation = activity.Type == "endOfConversation";
                        previousActionPlan = CurrentActionPlan;
                    }

                    if (chatMessages.Count > 0)
                    {
                        if (!isInConversation)
                            ChatMessages.Clear();
 
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

        ActionPlanListModel GetEmergencyActionPlan()
        {
            if (ActionPlans == null || ActionPlans.Count == 0)
                return null;
            return ActionPlans.FirstOrDefault(a => a.Name == "Emergency"); // TODO: no magic strings
        }

    }
}
