using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Autofac;
using Edison.Mobile.Android.Common;
using Edison.Mobile.Common.Ioc;
using Edison.Mobile.User.Client.Core.Shared;
using Edison.Mobile.User.Client.Core.ViewModels;
using Edison.Mobile.User.Client.Droid.Activities;

namespace Edison.Mobile.User.Client.Droid.Services
{
    [Service(Exported=false)]
    public class RemoteNotificationService : IntentService
    {
        private const string TAG = "RemoteNotificationService";

        protected async override void OnHandleIntent(Intent intent)
        {
            // extract data from Intent
            if (intent.Extras != null)
            {
                var source = intent.GetStringExtra(Constants.IntentSourceLabel);
                var action = intent.GetStringExtra(Constants.IntentActionLabel);
                var authenticated = intent.GetStringExtra(Constants.IntentAuthenticatedLabel);
                var notificationId = intent.GetIntExtra(Constants.NotificationIdLabel, 0);
                var notificationTag = intent.GetStringExtra(Constants.NotificationTagLabel);

                // Cancel notification
                var notificationManager = NotificationManager.FromContext(ApplicationContext);
                if (string.IsNullOrWhiteSpace(notificationTag))
                    notificationManager.Cancel(notificationId);
                else
                    notificationManager.Cancel(notificationTag, notificationId);


#if DEBUG
                System.Diagnostics.Debug.WriteLine("**********************************************************");
                System.Diagnostics.Debug.WriteLine("*********  NotificationService - OnHandleIntent  *********");
                System.Diagnostics.Debug.WriteLine("*********  Source = " + source);
                System.Diagnostics.Debug.WriteLine("*********  Action = " + action);
                System.Diagnostics.Debug.WriteLine("*********  Authenticated = " + authenticated);
                System.Diagnostics.Debug.WriteLine("**********************************************************");
#endif



                if (source == Constants.IntentSourceBackgroundNotification)
                    await ProcessAction(source, action);
                else if (source == Constants.IntentSourceNotRunningNotification)
                {
                    if (authenticated !=  Constants.IntentAuthenticated)
                    {
                        // The user is not authenticated so make them login
                        await LaunchAppAsync(typeof(LoginActivity), source, action);
                        return;
                    }
                    // the user is authenticated, so handle
                    await ProcessAction(source, action);
                }
                else
                {
                    // Should never get here so launch app with login as a last resort
                    await LaunchAppAsync(typeof(LoginActivity), source, action);
                    return;
                }
            }
        }

        private void LaunchApp(Type type, string source, string action)
        {
            var currentActivity = MainApplication.CurrentActivity;
            var appState = MainApplication.ApplicationState;

            var launchIntent = new Intent(this, type);
            launchIntent.PutExtra(Constants.IntentSourceLabel, source);
            launchIntent.PutExtra(Constants.IntentActionLabel, action);
            launchIntent.PutExtra(Constants.NotificationIdLabel, Constants.NotificationId);
            launchIntent.PutExtra(Constants.NotificationTagLabel, Constants.NotificationTag);
            if (appState != ApplicationState.NotRunning && appState != ApplicationState.Unknown && currentActivity != null)
            {
                launchIntent.AddFlags(ActivityFlags.ClearTop);
                currentActivity.StartActivity(launchIntent);
            }
            else
            {
                launchIntent.AddFlags(ActivityFlags.NewTask);
                ApplicationContext.StartActivity(launchIntent);
            }
        }

        private async Task LaunchAppAsync(Type type, string source, string action)
        {
            await Task.Run(() =>
            {
                LaunchApp(type, source, action);
            });
        }

        private async Task ProcessAction(string source, string action)
        { 
            // get an instance of the chat view model
            var chatViewModel = Container.Instance.Resolve<ChatViewModel>();

#if DEBUG
            System.Diagnostics.Debug.WriteLine("**********************************************************");
            System.Diagnostics.Debug.WriteLine("*********  NotificationService - Process Action  *********");
            System.Diagnostics.Debug.WriteLine("**********************************************************");
#endif
            var currentActivity = MainApplication.CurrentActivity;
            var appState = MainApplication.ApplicationState;
            switch (action)
            {
                case Constants.ActionEmergency:
                    //                   await chatViewModel?.ActivateChatPrompt(ChatPromptType.Emergency);
                    if (appState != ApplicationState.NotRunning && appState != ApplicationState.Unknown && currentActivity != null)
                        await chatViewModel?.ActivateChatPromptAsync(ChatPromptType.Emergency);
                    else
                        LaunchSilentRemoteNotificationRelpyActivity(Constants.ActionEmergency);
                    return;

                case Constants.ActionActivity:
                    LaunchRemoteNotificationRelpyActivity();
                    return;

                case Constants.ActionSafe:
                    //                    await chatViewModel?.ActivateChatPrompt(ChatPromptType.SafetyCheck);
                    if (appState != ApplicationState.NotRunning && appState != ApplicationState.Unknown && currentActivity != null)
                    {
                        await chatViewModel?.ActivateChatPromptAsync(ChatPromptType.SafetyCheck);
                        var success = await chatViewModel?.SendMessage(Resources.GetString(Resource.String.im_safe));
                    }
                    else
                        LaunchSilentRemoteNotificationRelpyActivity(Constants.ActionSafe);
                    return;

                default:
                    // assume there is an issue so just create an intent to launch app
                    await LaunchAppAsync(typeof(MainActivity), source, action);
                    return;
            }
        }

        private void LaunchRemoteNotificationRelpyActivity()
        {
            // Close the notification drawer otherwise it will hide the activity
            Intent closeIntent = new Intent(Intent.ActionCloseSystemDialogs);
            ApplicationContext.SendBroadcast(closeIntent);

            // Create and launch an isolated MessageDialig Activity to handle text input
            // NOTE: we have to do it like this as there is no way to attach, trigger and handle a DirectReply to a 
            // custom ReomoteViews object used in an custom notification layout. DirectReply is only supported by Actions
            // attached to a notification and cant mix Actions with the custom RemoteViews. Also could to find a way to trigger 
            // an attached Action in code, otherwise might have been able to attach the action and trigger it from the pending intent
            var launchIntent = new Intent(this, typeof(RemoteNotificationRelpyActivity));
            launchIntent.AddFlags(ActivityFlags.MultipleTask | ActivityFlags.NewTask);

            var currentActivity = MainApplication.CurrentActivity;
            var appState = MainApplication.ApplicationState;
            if (appState != ApplicationState.NotRunning && appState != ApplicationState.Unknown && currentActivity != null)
                currentActivity.StartActivity(launchIntent);
            else
                ApplicationContext.StartActivity(launchIntent);
        }


        private void LaunchSilentRemoteNotificationRelpyActivity(string action)
        {
            // Close the notification drawer otherwise it will hide the activity
            Intent closeIntent = new Intent(Intent.ActionCloseSystemDialogs);
            ApplicationContext.SendBroadcast(closeIntent);

            // Create and launch an isolated MessageDialig Activity to handle text input
            // NOTE: we have to do it like this as there is no way to attach, trigger and handle a DirectReply to a 
            // custom ReomoteViews object used in an custom notification layout. DirectReply is only supported by Actions
            // attached to a notification and cant mix Actions with the custom RemoteViews. Also could to find a way to trigger 
            // an attached Action in code, otherwise might have been able to attach the action and trigger it from the pending intent
            var launchIntent = new Intent(this, typeof(SilentRemoteNotificationReplyActivity));
            launchIntent.AddFlags(ActivityFlags.MultipleTask | ActivityFlags.NewTask);
            launchIntent.PutExtra(Constants.IntentActionLabel, action);
            ApplicationContext.StartActivity(launchIntent);
        }



    }
}