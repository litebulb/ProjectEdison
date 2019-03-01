using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Edison.Mobile.User.Client.Droid.Activities;
using Firebase.Messaging;
using Edison.Mobile.User.Client.Droid;
using Edison.Mobile.Android.Common.Notifications;
using Android.Util;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.User.Client.Core.ViewModels;
using Autofac;
using System.Threading.Tasks;
using Edison.Mobile.Android.Common;
using Edison.Mobile.User.Client.Core.CollectionItemViewModels;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Graphics.Drawables.Shapes;
using Android.Text;
using Android.Text.Style;
using Android.Support.V4.Content.Res;
using Edison.Mobile.User.Client.Droid.Services;

namespace Edison.Mobile.User.Client.Droid.Shared
{

    /*
     * This service is system service. It will run in the background even when the app is in the background or not running (though it can be killed by the user).
     */
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class EdisonFirebaseMessagingService : FirebaseMessagingService
    {
        private const string TAG = "FirebaseMessagingService";

        private ApplicationState _appState;
        private string _collapseKey;
        private string _messageId;
        private string _messageType;
        private int _timeToLive;
        private long _sentTime;


        private string _colorName = "blue";
        private string _responseTitle = "Event";
        private int _iconResId = Resource.Drawable.ic_edison_notification;
        private Color _color;
        private Edison.Core.Common.Models.Geolocation _location = null;
        private Color _smallIconColor;

        private Activity _currentActivity;


        /*
         * This method is called when a push notification message is received from Firebase Cloud Messaging (FCM), whether the app is in the foreground, background, or not running
         * FCM remote messages can have 'header data set by the remote service where they are created
         * FCM remote messages can have two types of main payload: Notification payloads and Data payloads
         * The creating service is responsible for populating the remote notification.
         * See: https://firebase.google.com/docs/reference/android/com/google/firebase/messaging/RemoteMessage
         * and  https://firebase.google.com/docs/cloud-messaging/concept-options#notifications_and_data_messages
         * 
         * The composition of push notifications should be designed to meet the business needs of the service the app is providing
         * 
         * This demo app has a very simple message structure created by the Edison Portal and sent via AzureNotificationHub and FCM.
         * The message payload is a text only message contained as a data payload, the format:
         * {"data":{"message":"The notification message text"}}
         *  
         * This method handles also provides a template to handle other common payload scenarios.
         * 
         * 
         * 
         */
        public async override void OnMessageReceived(RemoteMessage message)
        {

#if DEBUG
            System.Diagnostics.Debug.WriteLine("***************************************");
            System.Diagnostics.Debug.WriteLine("***************************************");
            System.Diagnostics.Debug.WriteLine("*********  Message Received  **********");
            System.Diagnostics.Debug.WriteLine("***************************************");
            System.Diagnostics.Debug.WriteLine("***************************************");
#endif

            //         if (message == null) return;

            // Need to resolve how notifications are structured, where the data is and how it is structured so we can retrieve and decode

            // Get the current Activity
            _currentActivity = BaseApplication.CurrentActivity;

            // Get the Apps current application state
            _appState = MainApplication.ApplicationState;

            // Extract 'header' data
            _collapseKey = message.CollapseKey;
            _messageId = message.MessageId;
            _messageType = message.MessageType;
            _timeToLive = message.Ttl;
            _sentTime = message.SentTime;

            // Check to see if the notification is a Silent notification. Silent notifications should be processed in the background and should not result 
            // in the generation of a local notification.
 //           await Task.Run(async () =>
 //           {
                if ((_messageType != null && _messageType.ToLowerInvariant().Contains(Constants.MessageTypeSilent)) || (message?.Data != null && message.Data.ContainsKey(Constants.MessageTypeSilent)))
                {
                    // Silent notification
                    // Extract the silent action to be performed
                    var action = message.Data[Constants.MessageDataAction];
                    Log.Debug(TAG, $"Notification Message Action: {action}");
                    await PerformSilentNotification(action);
                }
                else
                {
                    await PerformNonSilentNotification(message);
                }
 //           }); //.ConfigureAwait(false);
        }



        private async Task PerformSilentNotification(string action)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine($"[PNS] Perform action of type: {action}");
            Log.Debug(TAG, $"[PNS] Perform action of type: {action}");
            System.Diagnostics.Debug.WriteLine("******************************************");
            System.Diagnostics.Debug.WriteLine("*********  Silent Notification  **********");
            System.Diagnostics.Debug.WriteLine("******************************************");
#endif
            // Add supported actions here via switch statement


        }

        /// <summary>
        /// NOTE: You should provide all necessary information to display the notification in the data
        /// sent via FCM, so that the local notification can always be constructed with the appropriate data.
        /// This has NOT been done in this demo, so will try to infer as much information as possible
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task PerformNonSilentNotification(RemoteMessage message)
        {
            // Get and decode notification data

#if DEBUG
            System.Diagnostics.Debug.WriteLine("**********************************************");
            System.Diagnostics.Debug.WriteLine("*********  NON Silent Notification  **********");
            System.Diagnostics.Debug.WriteLine("**********************************************");
#endif

            // If the message type is used to differentiate the types of payload, use a switch statement here to process them

            var notification = message?.GetNotification();

            _smallIconColor = new Color(ResourcesCompat.GetColor(Resources, Resource.Color.app_blue, null));

            // Check if notification contains a notification payload
            if (notification != null)
            {
                // Notification contains a notification payload
                // This field will be non - null if a notification message is received while the application is in the foreground.

#if DEBUG
                System.Diagnostics.Debug.WriteLine("Template Notification:: Message body: " + notification.Body);
                Log.Debug(TAG, "Template Notification:: Message body: " + notification.Body);
#endif
                // Do not have to create and send a local notification for Notification payloads, as the FCM SDK framework does this
                // automatically in the background
                //SendLocalNotification(notification.Body);
            }


            string messageBody = null;
            // Check if notification contains a data payload
            if (message?.Data != null && message.Data.Count > 0)
            {
                // This should be modified to extract data from the data platform according to your data payload model
                // Data is usually serialized as json and can be associated with multiple keys or with a single key

                // If notification data is in Data, extract it

                // This demo  uses a simple key "messages" for the simple text data.
                if (message.Data.TryGetValue(Constants.MessageDataMessage, out messageBody) && !string.IsNullOrWhiteSpace(messageBody))
                {
                    // Data present so try to get from Data
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("Template Notification:: Message body: " + messageBody);
                    Log.Debug(TAG, "Template Notification:: Message body: " + messageBody);
#endif

                    // process further here as required.  for the demo app this is being done below


                }
            }


            // This demo app uses a simple text notification.
            // We will process it based up the current state of the application
            // a) Application is in foreground:
            //      i) Update the responses/events from the app service vis the ResponsesViewModel
            //      ii) If there is a new event/response: Create a Snackbar indicating a new event has been received
            //          If an event/response has been removed: Create a Snackbar indicating a new event has been closed
            //          If the event/responses are the same and the message body contains text: Create a Snackbar containing the text
            // b) Application is in background:
            //      i) Update the responses/events from the app service vis the ResponsesViewModel
            //      ii) If there is a new event/response: Create a local notification based upon the new response/event
            //          If an event/response has been removed: Create a local notification indicating a new event has been closed
            //          If the event/responses are the same and the message body contains text: Create a local notification with the message
            // c) Application is not running:
            //      i) Get responses/events from the app service via the ResponsesViewModel
            //      ii) If there is a new event/response: Create a local notification based upon the new response/event
            //          If an event/response has been removed: Create a local notification indicating a new event has been closed
            //          If the event/responses are the same and the message body contains text: Create a local notification with the message

#if DEBUG
                    System.Diagnostics.Debug.WriteLine("Application State: " + _appState.ToString());
                    Log.Debug(TAG, "Application State: " + _appState.ToString());
#endif

            // TODO:  THIS WILL CHANGE WHEN THE JSON THAT IS RETURNED IS UPDATED - WONT NEED TO GET DETAILS FROM VIEW MODEL


            int status = int.MinValue;
            string user = null;
            Guid responseId = Guid.Empty;
            string title = null;
            string messageText = null;
            List<string> tags = null;

            string bundleId = null;

            bool isResponseNotification = false;
            try
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<TempNotificationDTO>(messageBody);
                if (data != null)
                {
                    status = data.Status;
                    user = data.User;
                    responseId = data.ResponseId;
                    title = data.Title;
                    messageText = data.NotificationText;
                    tags = data.Tags;
                    isResponseNotification = true;
                    bundleId = responseId.ToString();
                }
                else
                {
                    messageText = messageBody;
                }
            }
            catch (Exception ex)
            {
                messageText = messageBody;
            }

            //            bool openApp = _appState == ApplicationState.Foreground ? false : true;  // may need to treat background differently
            if (isResponseNotification)
            {
                // get an instance of the responses view model
                var responsesViewModel = Container.Instance.Resolve<ResponsesViewModel>();
                // get the current response ids
                var currId = responsesViewModel?.CurrentResponseIds;
                // Try to get the details of this response if already existing (if doesn't exist will get null returned)
                var currentResponse = responsesViewModel?.GetResponseSummary(responseId);

                // set default for value then try to get from actual responses or infer from message
                _color = Constants.GetEventTypeColor(ApplicationContext, _colorName);

#if DEBUG
                System.Diagnostics.Debug.WriteLine("********************************************");
                System.Diagnostics.Debug.WriteLine("*********  Initial Msg Processed  **********");
                System.Diagnostics.Debug.WriteLine("********************************************");
#endif


                // Check if the app is in the foreground
                if (_appState == ApplicationState.Foreground)
                {
                    // in the foreground so will have already authenticated
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("****************************************");
                    System.Diagnostics.Debug.WriteLine("********* App in Foreground   **********");
                    System.Diagnostics.Debug.WriteLine("****************************************");

#endif

                    // get the latest responses
                    await UpdateResponses(responsesViewModel);

                    // try to get the response from the updated list
                    var newResponse = responsesViewModel?.GetResponseSummary(responseId);

                    PendingIntent pendingIntent = null;
                    List<NotificationCompat.Action> actions = null;


                    if (newResponse != null)
                    {
                        // Managed to get the response details from service, so populate values for local notification
                        _colorName = newResponse.Color;
                        _responseTitle = newResponse.Name;
                        var attributes = Constants.GetChatMessageButtonSettings(ApplicationContext, _responseTitle, newResponse.Icon);
                        var iconName = attributes.Item1;
                        _iconResId = GetIconResourceId(iconName);
                        _color = attributes.Item2;
                        _location = newResponse.Geolocation;

                        if (currentResponse == null)
                        {
                            // Must be a new response/event
#if DEBUG
                            System.Diagnostics.Debug.WriteLine("***********************************");
                            System.Diagnostics.Debug.WriteLine("********* New Response   **********");
                            System.Diagnostics.Debug.WriteLine("***********************************");
#endif
                        }
                        else
                        {
                            // Response details must gave changed
#if DEBUG
                            System.Diagnostics.Debug.WriteLine("***************************************");
                            System.Diagnostics.Debug.WriteLine("*********  Updated Response  **********");
                            System.Diagnostics.Debug.WriteLine("***************************************");
#endif
                            // Update Title depending on response type
                            // if(!newResponse.IsActive)
                            if (newResponse.EndDate < DateTime.UtcNow)
                                // The response/event is not active (its over)
                                _responseTitle = Resources.GetString(Resource.String.event_over) + ": " + _responseTitle;
                            else if (_location != currentResponse.Geolocation)
                                // The response/event location changed
                                _responseTitle = Resources.GetString(Resource.String.location_changed) + ": " + _responseTitle;
                            else
                                // The response/event has a different update
                                _responseTitle = Resources.GetString(Resource.String.event_update) + ": " + _responseTitle;
                        }

                        Edison.Mobile.Android.Common.Notifications.NotificationService.SendLocalNotification(this, Constants.EVENT_CHANNEL_ID,
                            title, Resource.Drawable.ic_edison_notification, _smallIconColor, NotificationCompat.CategoryMessage,
                            NotificationCompat.VisibilityPublic, Constants.NotificationId, Constants.NotificationTag, pendingIntent, true, _responseTitle, messageText, _iconResId, _color, bundleId, actions);
                    }
                    else if (currentResponse != null)
                    {
                        // Response must have been deleted
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("***************************************");
                        System.Diagnostics.Debug.WriteLine("*********  Response Removed  **********");
                        System.Diagnostics.Debug.WriteLine("***************************************");
#endif

                        // Note: in demo system should not reach here

                        _colorName = currentResponse.Color;
                        _responseTitle = currentResponse.Name;
                        var attributes = Constants.GetChatMessageButtonSettings(ApplicationContext, _responseTitle, currentResponse.Icon);
                        var iconName = attributes.Item1;
                        _iconResId = GetIconResourceId(iconName);
                        _color = attributes.Item2;
                        _location = currentResponse.Geolocation;

                        Edison.Mobile.Android.Common.Notifications.NotificationService.SendLocalNotification(this, Constants.EVENT_CHANNEL_ID,
                            title, Resource.Drawable.ic_edison_notification, _smallIconColor, NotificationCompat.CategoryMessage,
                            NotificationCompat.VisibilityPublic, Constants.NotificationId, Constants.NotificationTag, pendingIntent, true, _responseTitle, messageText, _iconResId, _color, bundleId, actions);
                    }
                    else // (currentResponse == null)
                    {
                        // Response never existed, so treat as text message.
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("****************************************************");
                        System.Diagnostics.Debug.WriteLine("*********  Not a Response - Text Message  **********");
                        System.Diagnostics.Debug.WriteLine("****************************************************");
#endif
                        // Try to infer info from message
                        var responseInferred = InferValuesFromMessage(ApplicationContext, messageText, false);

                        if (responseInferred)
                            SendLocalNotification(title, messageText, _appState == ApplicationState.Foreground ? false : true, _iconResId, _color);
                        else
                            SendLocalNotification(title, messageText, _appState == ApplicationState.Foreground ? false : true);
                    }
                }
                else
                {
                    // App in background or not running
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("*******************************************");
                    System.Diagnostics.Debug.WriteLine("*********  App NOT in Foreground  *********");
                    System.Diagnostics.Debug.WriteLine("*******************************************");
#endif

                    // Try to silently authenticate.  
                    // Note: If in Background should authenticate successfully, if not running may or may not authenticate
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("********* Trying to Authenticate Silently   **********");
#endif
                    // Not in background, so need check if we are authenticated
                    // get an instance of the login view model
                    var loginViewModel = Container.Instance.Resolve<LoginViewModel>();
                    var hasToken = await loginViewModel?.AuthService?.AcquireTokenSilently();

                    // If valid token, then Authentication service has valid access token and can request latest responses
                    if (hasToken)
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("****************************************");
                        System.Diagnostics.Debug.WriteLine("*********  App Authenticated  **********");
                        System.Diagnostics.Debug.WriteLine("****************************************");
#endif

                        string source = Constants.IntentSourceBackgroundNotification;
                        string authenticated = Constants.IntentAuthenticated;

                        // get the latest responses
                        await UpdateResponses(responsesViewModel);

                        // try to get the response from the updated list
                        var newResponse = responsesViewModel?.GetResponseSummary(responseId);

                        if (newResponse != null)
                        {
                            // Managed to get the response details from service, so populate values for local notification
                            _colorName = newResponse.Color;
                            _responseTitle = newResponse.Name;
                            var attributes = Constants.GetChatMessageButtonSettings(ApplicationContext, _responseTitle, newResponse.Icon);
                            var iconName = attributes.Item1;
                            _iconResId = GetIconResourceId(iconName);
                            _color = attributes.Item2;
                            _location = newResponse.Geolocation;

                            if (currentResponse == null)
                            { 
                                // Must be a new response/event
#if DEBUG
                                System.Diagnostics.Debug.WriteLine("***********************************");
                                System.Diagnostics.Debug.WriteLine("********* New Response   **********");
                                System.Diagnostics.Debug.WriteLine("***********************************");
#endif
                            }
                            else
                            {
                                // Response details must gave changed
#if DEBUG
                                System.Diagnostics.Debug.WriteLine("***************************************");
                                System.Diagnostics.Debug.WriteLine("*********  Updated Response  **********");
                                System.Diagnostics.Debug.WriteLine("***************************************");
#endif
                                // Update Title depending on response type
                                // if(!newResponse.IsActive)
                                if (newResponse.EndDate < DateTime.UtcNow)
                                    // The response/event is not active (its over)
                                    _responseTitle = Resources.GetString(Resource.String.event_over) + ": " + _responseTitle;
                                else if (_location != currentResponse.Geolocation)
                                    // The response/event location changed
                                    _responseTitle = Resources.GetString(Resource.String.location_changed) + ": " + _responseTitle;
                                else
                                    // The response/event has a different update
                                    _responseTitle = Resources.GetString(Resource.String.event_update) + ": " + _responseTitle;
                            }

                            Intent intent = new Intent(this, typeof(MainActivity));
                            CreateEdisonResponseNotification(title, messageText, intent, bundleId, source, authenticated);

                        }
                        else if (currentResponse != null)
                        {
                            // Response must have been deleted
#if DEBUG
                            System.Diagnostics.Debug.WriteLine("***************************************");
                            System.Diagnostics.Debug.WriteLine("*********  Response Removed  **********");
                            System.Diagnostics.Debug.WriteLine("***************************************");
#endif

                            // Note: in demo system should not reach here

                            _colorName = currentResponse.Color;
                            _responseTitle = currentResponse.Name;
                            var attributes = Constants.GetChatMessageButtonSettings(ApplicationContext, _responseTitle, currentResponse.Icon);
                            var iconName = attributes.Item1;
                            _iconResId = GetIconResourceId(iconName);
                            _color = attributes.Item2;
                            _location = currentResponse.Geolocation;

                            _responseTitle = Resources.GetString(Resource.String.event_over) + ": " + _responseTitle;

                            Intent intent = new Intent(this, typeof(MainActivity));
                            intent.AddFlags(ActivityFlags.ClearTop);
                            intent.PutExtra(Constants.IntentSourceLabel, source);
                            intent.PutExtra(Constants.NotificationIdLabel, Constants.NotificationId);
                            intent.PutExtra(Constants.NotificationTagLabel, Constants.NotificationTag);
                            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot | PendingIntentFlags.UpdateCurrent);

                            Edison.Mobile.Android.Common.Notifications.NotificationService.SendLocalNotification(this, Constants.EVENT_CHANNEL_ID,
                                title, Resource.Drawable.ic_edison_notification, _smallIconColor, NotificationCompat.CategoryMessage,
                                NotificationCompat.VisibilityPublic, Constants.NotificationId, Constants.NotificationTag, pendingIntent, true, _responseTitle, messageText, _iconResId, _color, bundleId, null);

                        }
                        else // (currentResponse == null)
                        {
                            // Response never existed, so treat as text message.
#if DEBUG
                            System.Diagnostics.Debug.WriteLine("****************************************************");
                            System.Diagnostics.Debug.WriteLine("*********  Not a Response - Text Message  **********");
                            System.Diagnostics.Debug.WriteLine("****************************************************");
#endif

                            // Try to infer info from message
                            var responseInferred = InferValuesFromMessage(ApplicationContext, messageText, false);

                            if (responseInferred)
                                SendLocalNotification(title, messageText, _appState == ApplicationState.Foreground ? false : true, _iconResId, _color);
                            else
                                SendLocalNotification(title, messageText, _appState == ApplicationState.Foreground ? false : true);
                        }
                    }
                    else
                    {
                        // app not authenticated so get what info we can on the referenced response
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("****************************************");
                        System.Diagnostics.Debug.WriteLine("*********  NOT Authenticated  **********");
                        System.Diagnostics.Debug.WriteLine("****************************************");
#endif

                        string source = Constants.IntentSourceNotRunningNotification;
                        string authenticated = Constants.IntentNotAuthenticated;

                        if (currentResponse == null)
                            // We have no info, so try to infer from the message and treat as a new response
                            InferValuesFromMessage(ApplicationContext, messageText);
                        else
                        {
                            // The response is currently in app so pull information from there
                            _colorName = currentResponse.Color;
                            _responseTitle = currentResponse.Name;
                            var attributes = Constants.GetChatMessageButtonSettings(ApplicationContext, _responseTitle, currentResponse.Icon);
                            var iconName = attributes.Item1;
                            _iconResId = GetIconResourceId(iconName);
                            _color = attributes.Item2;
                            _location = currentResponse.Geolocation;

                            // As its already there lets treat as an update
                            _responseTitle = Resources.GetString(Resource.String.event_update) + ": " + _responseTitle;
                        }


                        Intent intent = new Intent(this, typeof(LoginActivity));
                        CreateEdisonResponseNotification(title, messageText, intent, bundleId, source, authenticated);

                    }

                }

            }
            else
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("************************************************");
                System.Diagnostics.Debug.WriteLine("*********  Not Response Notification  **********");
                System.Diagnostics.Debug.WriteLine("************************************************");
#endif
                // Not a Response/event notification - treat as a text message
                SendLocalNotification(title, messageText, _appState == ApplicationState.Foreground ? false : true);
            }
        }



        private void SendLocalNotification(string messageText, bool openApp = true)
        {
            PendingIntent pendingIntent = null;
            if (openApp)
            {
                var intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                intent.PutExtra(Constants.NotificationIdLabel, Constants.NotificationId);
                intent.PutExtra(Constants.NotificationTagLabel, Constants.NotificationTag);
                pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);
            }
            /*
                        // use if in foreground and need to move to a new activity
                        // Create the TaskStackBuilder and add the intent, which inflates the back stack
                        var stackBuilder = global::Android.Support.V4.App.TaskStackBuilder.Create(this);
                        stackBuilder.AddNextIntentWithParentStack(intent);
                        // Get the PendingIntent containing the entire back stack
                        var pendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);
            */
            Color smallIconColor = new Color(ResourcesCompat.GetColor(Resources, Resource.Color.app_blue, null));
            Edison.Mobile.Android.Common.Notifications.NotificationService.SendLocalNotification(this, Constants.EVENT_CHANNEL_ID, 
                                                        "Edison Message", Resource.Drawable.ic_edison_notification, smallIconColor, NotificationCompat.CategoryMessage, 
                                                        NotificationCompat.VisibilityPublic, Constants.NotificationId, Constants.NotificationTag, pendingIntent, true, messageText, Resource.Drawable.edison_fox1);

        }



        private void SendLocalNotification(string title, string messageText, bool openApp = true, int resId = 0, int color=-1)
        {
            PendingIntent pendingIntent = null;
            if (openApp)
            {
                var intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                intent.PutExtra(Constants.NotificationIdLabel, Constants.NotificationId);
                intent.PutExtra(Constants.NotificationTagLabel, Constants.NotificationTag);
                pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);
            }
            /*
                        // Create the TaskStackBuilder and add the intent, which inflates the back stack
                        var stackBuilder = global::Android.Support.V4.App.TaskStackBuilder.Create(this);
                        stackBuilder.AddNextIntentWithParentStack(intent);
                        // Get the PendingIntent containing the entire back stack
                        var pendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);
            */
            Color smallIconColor = new Color(ResourcesCompat.GetColor(Resources, Resource.Color.app_blue, null));

            if (resId == 0)
                resId = Resource.Drawable.edison_fox1;

            Edison.Mobile.Android.Common.Notifications.NotificationService.SendLocalNotification(this, Constants.EVENT_CHANNEL_ID, 
                                                        title, Resource.Drawable.ic_edison_notification, smallIconColor, NotificationCompat.CategoryMessage, 
                                                        NotificationCompat.VisibilityPublic, Constants.NotificationId, Constants.NotificationTag, pendingIntent, true, messageText, resId, color);

        }


        private void SendLocaNotification(ResponseCollectionItemViewModel response, string messageText, int iocnResId, Color colorbool, bool openApp = true)
        {

            PendingIntent pendingIntent = null;
            if (openApp)
            {
                var intent = new Intent(this, typeof(MainActivity));
                intent.AddFlags(ActivityFlags.ClearTop);
                intent.PutExtra(Constants.NotificationIdLabel, Constants.NotificationId);
                intent.PutExtra(Constants.NotificationTagLabel, Constants.NotificationTag);
                pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);
            }

            // get info from response
            var colorName = response.Response.Color;
            var title = response.Response.Name;
            var attributes = Constants.GetChatMessageButtonSettings(ApplicationContext, title, response.Response.Icon);
            var iconName = attributes.Item1;
            var color = attributes.Item2;

            var message = response.Response.ActionPlan?.Description == null ? messageText : response.Response.ActionPlan.Description;
            var location = response.Response.Geolocation;

            Color smallIconColor = new Color(ResourcesCompat.GetColor(Resources, Resource.Color.app_blue, null));

            Edison.Mobile.Android.Common.Notifications.NotificationService.SendLocalNotification(this, Constants.EVENT_CHANNEL_ID,
                                                        title, Resource.Drawable.ic_edison_notification, smallIconColor, NotificationCompat.CategoryMessage,
                                                        NotificationCompat.VisibilityPublic, Constants.NotificationId, Constants.NotificationTag, pendingIntent, true, messageText, Resource.Drawable.edison_fox1);

        }




        private async Task UpdateResponses(ResponsesViewModel responsesViewModel)
        {
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            if (_currentActivity == null)
            {
                await responsesViewModel?.GetResponses();
                tcs.SetResult(true);
            }
            else
            {
                _currentActivity.RunOnUiThread(async () =>
                {
                    await responsesViewModel?.GetResponses();
                    tcs.SetResult(true);
                });
            }
            await tcs.Task;
        }

        private bool InferValuesFromMessage(Context ctx, string msg, bool createDefault = true)
        {
            bool ret = true;
            var message = msg.ToLowerInvariant();
            if (message.Contains(ctx.Resources.GetString(Resource.String.shooter)) || message.Contains(ctx.Resources.GetString(Resource.String.gun)))
            {
                _color = Constants.GetEventTypeColor(ApplicationContext, Core.Shared.Constants.ColorName.Red);
                _iconResId = Resource.Drawable.gun;
                _responseTitle = ctx.Resources.GetString(Resource.String.active_shooter);
            }
            else if (message.Contains(ctx.Resources.GetString(Resource.String.suspicious)) || message.Contains(ctx.Resources.GetString(Resource.String.pckg)) || message.Contains(ctx.Resources.GetString(Resource.String.bomb)))
            {
                _color = Constants.GetEventTypeColor(ApplicationContext, Core.Shared.Constants.ColorName.Red);
                _iconResId = Resource.Drawable.suspicious_package;
                _responseTitle = ctx.Resources.GetString(Resource.String.suspicious_package);
            }
            else if (message.Contains(ctx.Resources.GetString(Resource.String.fire_)))
            {
                _color = Constants.GetEventTypeColor(ApplicationContext, Core.Shared.Constants.ColorName.Red);
                _iconResId = Resource.Drawable.fire;
                _responseTitle = ctx.Resources.GetString(Resource.String.fire);
            }
            else if (message.Contains(ctx.Resources.GetString(Resource.String.protest)) || message.Contains(ctx.Resources.GetString(Resource.String.demonstration)) ||
                message.Contains(ctx.Resources.GetString(Resource.String.disturbance)) || message.Contains(ctx.Resources.GetString(Resource.String.riot)) ||
                message.Contains(ctx.Resources.GetString(Resource.String.violence)) || message.Contains(ctx.Resources.GetString(Resource.String.barricade)))
            {
                _color = Constants.GetEventTypeColor(ApplicationContext, Core.Shared.Constants.ColorName.Blue);
                _iconResId = Resource.Drawable.protest;
                _responseTitle = ctx.Resources.GetString(Resource.String.protest);
            }
            else if (message.Contains(ctx.Resources.GetString(Resource.String.health)) || message.Contains(ctx.Resources.GetString(Resource.String.virus)) ||
                message.Contains(ctx.Resources.GetString(Resource.String.infection)) || message.Contains(ctx.Resources.GetString(Resource.String.illness)))
            {
                _color = Constants.GetEventTypeColor(ApplicationContext, Core.Shared.Constants.ColorName.Blue);
                _iconResId = Resource.Drawable.health_check;
                _responseTitle = ctx.Resources.GetString(Resource.String.health_check);
            }
            else if (message.Contains(ctx.Resources.GetString(Resource.String.air)) || message.Contains(ctx.Resources.GetString(Resource.String.pollution)) || message.Contains(ctx.Resources.GetString(Resource.String.pollen)))
            {
                _color = Constants.GetEventTypeColor(ApplicationContext, Core.Shared.Constants.ColorName.Blue);
                _iconResId = Resource.Drawable.air_quality;
                _responseTitle = ctx.Resources.GetString(Resource.String.air_quality);
            }
            else if (message.Contains(ctx.Resources.GetString(Resource.String.tornado_)) || message.Contains(ctx.Resources.GetString(Resource.String.wind)))
            {
                _color = Constants.GetEventTypeColor(ApplicationContext, Core.Shared.Constants.ColorName.Yellow);
                _iconResId = Resource.Drawable.tornado;
                _responseTitle = ctx.Resources.GetString(Resource.String.tornado);
            }
            else if (message.Contains(ctx.Resources.GetString(Resource.String.vip_)) || message.Contains(ctx.Resources.GetString(Resource.String.celebrity)) || message.Contains(ctx.Resources.GetString(Resource.String.famous)))
            {
                _color = Constants.GetEventTypeColor(ApplicationContext, Core.Shared.Constants.ColorName.Blue);
                _iconResId = Resource.Drawable.vip;
                _responseTitle = ctx.Resources.GetString(Resource.String.vip);
            }
            else if (createDefault)
            {
                _color = Constants.GetEventTypeColor(ApplicationContext, Core.Shared.Constants.ColorName.Red);
                _iconResId = Resource.Drawable.emergency;
                if (message.Contains(ctx.Resources.GetString(Resource.String.emergency)))
                    _responseTitle = ctx.Resources.GetString(Resource.String.emergency);
                else
                    _responseTitle = ctx.Resources.GetString(Resource.String.announcement);
            }
            else
                ret = false;
            return ret;
        }



        private int GetIconResourceId(string iconName)
        {
            var id = ApplicationContext.GetDrawableId(iconName);
            return id == 0 ? Resource.Drawable.emergency : id;
        }


        private Drawable CreateCircleDrawable(Color color, int size = 100)
        {
            ShapeDrawable drw = new ShapeDrawable(new OvalShape());
            drw.SetIntrinsicHeight(size);
            drw.SetIntrinsicWidth(size);
            drw.Paint.Color = color;
            return drw;
        }



        private void CreateEdisonResponseNotification(string title, string messageText, Intent intent, string bundleId, string source, string authenticated)
        {
            var emergencyIntent = new Intent(this, typeof(RemoteNotificationService));
            emergencyIntent.PutExtra(Constants.IntentSourceLabel, source);
            emergencyIntent.PutExtra(Constants.IntentActionLabel, Constants.ActionEmergency);
            emergencyIntent.PutExtra(Constants.IntentAuthenticatedLabel, authenticated);
            emergencyIntent.PutExtra(Constants.NotificationIdLabel, Constants.NotificationId);
            emergencyIntent.PutExtra(Constants.NotificationTagLabel, Constants.NotificationTag);
            var emergencyPendingIntent = PendingIntent.GetService(this, 1, emergencyIntent, PendingIntentFlags.OneShot);

            //                        var activityIntent = new Intent(this, typeof(MainActivity));  // if we wanted to open the app
            //                       activityIntent.AddFlags(ActivityFlags.ClearTop);
            var activityIntent = new Intent(this, typeof(RemoteNotificationService));
            activityIntent.PutExtra(Constants.IntentSourceLabel, source);
            activityIntent.PutExtra(Constants.IntentActionLabel, Constants.ActionActivity);
            activityIntent.PutExtra(Constants.IntentAuthenticatedLabel, authenticated);
            activityIntent.PutExtra(Constants.NotificationIdLabel, Constants.NotificationId);
            activityIntent.PutExtra(Constants.NotificationTagLabel, Constants.NotificationTag);
            var activityPendingIntent = PendingIntent.GetService(this, 2, activityIntent, PendingIntentFlags.OneShot);

            //                       var safeIntent = new Intent(this, typeof(MainActivity));  // if we wanted to open the app
            //                      safeIntent.AddFlags(ActivityFlags.ClearTop);
            var safeIntent = new Intent(this, typeof(RemoteNotificationService));
            safeIntent.PutExtra(Constants.IntentSourceLabel, source);
            safeIntent.PutExtra(Constants.IntentActionLabel, Constants.ActionSafe);
            safeIntent.PutExtra(Constants.IntentAuthenticatedLabel, authenticated);
            safeIntent.PutExtra(Constants.NotificationIdLabel, Constants.NotificationId);
            safeIntent.PutExtra(Constants.NotificationTagLabel, Constants.NotificationTag);
            var safePendingIntent = PendingIntent.GetService(this, 3, safeIntent, PendingIntentFlags.OneShot);

            intent.AddFlags(ActivityFlags.ClearTop);
            intent.PutExtra(Constants.IntentSourceLabel, source);
            intent.PutExtra(Constants.NotificationIdLabel, Constants.NotificationId);
            intent.PutExtra(Constants.NotificationTagLabel, Constants.NotificationTag);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot | PendingIntentFlags.UpdateCurrent);

#if DEBUG
            System.Diagnostics.Debug.WriteLine("*******************************************************");
            System.Diagnostics.Debug.WriteLine("*********  Call SendLocalEdisonNotification  **********");
            System.Diagnostics.Debug.WriteLine("*******************************************************");
#endif
            Edison.Mobile.Android.Common.Notifications.NotificationService.SendLocalEdisonNotification(this, Constants.EVENT_CHANNEL_ID,
                        title, Resource.Drawable.ic_edison_notification, _smallIconColor, NotificationCompat.CategoryMessage,
                        NotificationCompat.VisibilityPublic, Constants.NotificationId, Constants.NotificationTag, pendingIntent, true, _responseTitle, messageText, _iconResId, _color,
                        Resource.Layout.collapsed_notification, Resource.Layout.extended_notification, Resource.Layout.headsup_notification,
                        Resource.Id.response_icon, Resource.Id.notification_title, Resource.Id.response_title, Resource.Id.message_text, Resource.Id.emergency_button,
                        Resource.Id.activity_button, Resource.Id.safe_button,
                        emergencyPendingIntent, activityPendingIntent, safePendingIntent, bundleId);
        }


    }


    public class TempNotificationDTO
    {
        public int Status;
        public string User;
        public Guid ResponseId;
        public string Title;
        public string NotificationText;
        public List<string> Tags;
    }

}