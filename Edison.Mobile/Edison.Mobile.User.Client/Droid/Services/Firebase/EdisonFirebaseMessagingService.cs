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
            Log.Debug(TAG, "***************************************");
            Log.Debug(TAG, "***************************************");
            Log.Debug(TAG, "***************************************");
            Log.Debug(TAG, "***************************************");
            Log.Debug(TAG, "From: " + message?.From);
            Log.Debug(TAG, "***************************************");
            Log.Debug(TAG, "***************************************");
            Log.Debug(TAG, "***************************************");
            Log.Debug(TAG, "***************************************");
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
#endif
            // Add supported actions here via switch statement



        }

        private async Task PerformNonSilentNotification(RemoteMessage message)
        {
            // Get and decode notification data

            // If the message type is used to differentiate the types of payload, use a switch statement here to process them

            var notification = message?.GetNotification();
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
            // we will process it based up the current state of the application
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
                bool test = true;
                messageText = messageBody;
            }

//            bool openApp = _appState == ApplicationState.Foreground ? false : true;  // may need to treat background differently
            if (isResponseNotification)
            {
                // get an instance of the 
                var responsesViewModel = Container.Instance.Resolve<ResponsesViewModel>();
                // get the current response ids
                var curr = responsesViewModel?.CurrentResponseIds;

                // We will assume that if the notified resource ids exists in the current list, then it may have been deleted so fetch it
                var response0 = responsesViewModel?.GetResponse(responseId);

                // now update the responses
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

                // try to get the response from the update list
                var response1 = responsesViewModel?.GetResponse(responseId);



                if (response0 == null && response1 != null)
                {
                    // ***** Response/event was new  ******
                    var colorName = response1.Color;
                    var responseTitle = response1.Name;
                    var attributes = Constants.GetChatMessageButtonSettings(ApplicationContext, responseTitle, response1.Icon);
                    var iconName = attributes.Item1;
                    var iconResId = GetIconResourceId(iconName);
                    var color = attributes.Item2;
                    var description = response1.ActionPlan?.Description;
                    var location = response1.Geolocation;

                    PendingIntent pendingIntent = null;
                    List<NotificationCompat.Action> actions = null;

                    Color smallIconColor = new Color(ResourcesCompat.GetColor(Resources, Resource.Color.app_blue, null));

                    if (_appState == ApplicationState.Foreground)
                    {
                        // ******* App in Foreground *******

                        Edison.Mobile.Android.Common.Notifications.NotificationService.SendLocalNotification(this, Constants.EVENT_CHANNEL_ID,
                            title, Resource.Drawable.ic_edison_notification, smallIconColor, NotificationCompat.CategoryMessage,
                            NotificationCompat.VisibilityPublic, Constants.NotificationId, Constants.NotificationTag, pendingIntent, true, responseTitle, messageText, iconResId, color, bundleId, actions);

                    }
                    else if (_appState == ApplicationState.Background )
                    {
                        // ******* App in Background *******

                        var emergencyIntent = new Intent(this, typeof(MainActivity));
                        emergencyIntent.AddFlags(ActivityFlags.ClearTop);
                        emergencyIntent.PutExtra(Constants.IntentSourceLabel, Constants.IntentSourceBackgroundNotification);
                        emergencyIntent.PutExtra(Constants.IntentActionLabel, Constants.ActionEmergency);
                        emergencyIntent.PutExtra(Constants.NotificationIdLabel, Constants.NotificationId);
                        emergencyIntent.PutExtra(Constants.NotificationTagLabel, Constants.NotificationTag);
                        var emergencyPendingIntent = PendingIntent.GetActivity(this, 1, emergencyIntent, PendingIntentFlags.OneShot);
 //                       var emergencyAction = new NotificationCompat.Action(Resource.Drawable.emergency, Resources.GetString(Resource.String.notification_emergency), emergencyPendingIntent);
//                        var emergencyAction = new NotificationCompat.Action(Resource.Drawable.emergency, (string)null, emergencyPendingIntent);


                        var activityIntent = new Intent(this, typeof(MainActivity));
                        activityIntent.AddFlags(ActivityFlags.ClearTop);
                        activityIntent.PutExtra(Constants.IntentSourceLabel, Constants.IntentSourceBackgroundNotification);
                        activityIntent.PutExtra(Constants.IntentActionLabel, Constants.ActionActivity);
                        activityIntent.PutExtra(Constants.NotificationIdLabel, Constants.NotificationId);
                        activityIntent.PutExtra(Constants.NotificationTagLabel, Constants.NotificationTag);
                        var activityPendingIntent = PendingIntent.GetActivity(this, 2, activityIntent, PendingIntentFlags.OneShot);
//                        var activityAction = new NotificationCompat.Action(Resource.Drawable.message, Resources.GetString(Resource.String.notification_activity), activityPendingIntent);
 //                       var activityAction = new NotificationCompat.Action(Resource.Drawable.message, (string)null, emergencyPendingIntent);

                        var safeIntent = new Intent(this, typeof(MainActivity));
                        safeIntent.AddFlags(ActivityFlags.ClearTop);
                        safeIntent.PutExtra(Constants.IntentSourceLabel, Constants.IntentSourceBackgroundNotification);
                        safeIntent.PutExtra(Constants.IntentActionLabel, Constants.ActionSafe);
                        safeIntent.PutExtra(Constants.NotificationIdLabel, Constants.NotificationId);
                        safeIntent.PutExtra(Constants.NotificationTagLabel, Constants.NotificationTag);
                        var safePendingIntent = PendingIntent.GetActivity(this, 3, safeIntent, PendingIntentFlags.OneShot);
 //                       var safeAction = new NotificationCompat.Action(Resource.Drawable.personal_check, Resources.GetString(Resource.String.im_safe), safePendingIntent);
//                        var safeAction = new NotificationCompat.Action(Resource.Drawable.personal_check, (string)null, safePendingIntent);

//                        var dismissAction = new NotificationCompat.Action(0, Resources.GetString(Resource.String.notification_dismiss), null);

  //                      actions = new List<NotificationCompat.Action> { emergencyAction, activityAction, safeAction};

                        var intent = new Intent(this, typeof(MainActivity));
                        intent.AddFlags(ActivityFlags.ClearTop);
                        intent.PutExtra(Constants.NotificationIdLabel, Constants.NotificationId);
                        intent.PutExtra(Constants.NotificationTagLabel, Constants.NotificationTag);
                        pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot | PendingIntentFlags.UpdateCurrent);


                        Edison.Mobile.Android.Common.Notifications.NotificationService.SendLocalEdisonNotification(this, Constants.EVENT_CHANNEL_ID,
                                    title, Resource.Drawable.ic_edison_notification, smallIconColor, NotificationCompat.CategoryMessage,
                                    NotificationCompat.VisibilityPublic, Constants.NotificationId, Constants.NotificationTag, pendingIntent, true, responseTitle, messageText, iconResId, color,
                                    Resource.Layout.collapsed_notification, Resource.Layout.extended_notification, Resource.Layout.headsup_notification, 
                                    Resource.Id.response_icon, Resource.Id.notification_title, Resource.Id.response_title, Resource.Id.message_text, Resource.Id.emergency_button, 
                                    Resource.Id.activity_button, Resource.Id.safe_button,
                                    emergencyPendingIntent, activityPendingIntent, safePendingIntent, bundleId);

                    }
                    else if (_appState == ApplicationState.NotRunning || _appState == ApplicationState.Unknown)
                    {
                        // ******* App not running *******

                        //                       RemoteViews notificationView = new RemoteViews(PackageName, Resource.Layout.push_notification);







                        var intent = new Intent(this, typeof(LoginActivity));
                        intent.AddFlags(ActivityFlags.ClearTop);
                        intent.PutExtra(Constants.NotificationIdLabel, Constants.NotificationId);
                        intent.PutExtra(Constants.NotificationTagLabel, Constants.NotificationTag);
                        pendingIntent = PendingIntent.GetActivity(this, 10, intent, PendingIntentFlags.OneShot | PendingIntentFlags.UpdateCurrent);


                        Edison.Mobile.Android.Common.Notifications.NotificationService.SendLocalNotification(this, Constants.EVENT_CHANNEL_ID,
                                                title, Resource.Drawable.ic_edison_notification, smallIconColor, NotificationCompat.CategoryMessage,
                                                NotificationCompat.VisibilityPublic, Constants.NotificationId, Constants.NotificationTag, pendingIntent, true, responseTitle, messageText, iconResId, color, bundleId, actions);

                    }
                }
                else if (response0 != null && response1 == null)
                {
                    // Response/event was removed








                }
                else if (response0 != null && response1 != null)
                {
                    // Response/event was modified








                }
                else if (response0 == null && response1 == null)
                {
                    // Response/event never existed, so there is an issue - treat as a text message
                    SendLocalNotification(title, messageText, _appState == ApplicationState.Foreground ? false : true);
                }
            }
            else
                    // Not a Response/event notification - treat as a text message
                SendLocalNotification(title, messageText, _appState == ApplicationState.Foreground ? false : true);

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
        private void SendLocalNotification(string title, string messageText, bool openApp = true)
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
            Edison.Mobile.Android.Common.Notifications.NotificationService.SendLocalNotification(this, Constants.EVENT_CHANNEL_ID, 
                                                        title, Resource.Drawable.ic_edison_notification, smallIconColor, NotificationCompat.CategoryMessage, 
                                                        NotificationCompat.VisibilityPublic, Constants.NotificationId, Constants.NotificationTag, pendingIntent, true, messageText, Resource.Drawable.edison_fox1);

        }


        private void SendLocaNotification(ResponseCollectionItemViewModel response, bool openApp = true, string messageText = null)
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