using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Constraints;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Views;
using Android.Widget;
using Edison.Mobile.Android.Common;
using Edison.Mobile.Android.Common.Controls;
using Edison.Mobile.User.Client.Core.Shared;
using Edison.Mobile.User.Client.Core.ViewModels;

namespace Edison.Mobile.User.Client.Droid.Activities
{
    [Activity(Theme = "@style/RemoteNotificationRelpyTheme", ExcludeFromRecents = true, ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait, TaskAffinity = "Edison.Mobile.RemoteReply")]
    public class SilentRemoteNotificationReplyActivity : BaseActivity<ChatViewModel>
    {
        private string _action;

        protected override void OnCreate(Bundle savedInstanceState)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("********************************************************************");
            System.Diagnostics.Debug.WriteLine("*********  SilentRemoteNotificationReplyActivity OnCreate  *********");
            System.Diagnostics.Debug.WriteLine("********************************************************************");
#endif

            base.OnCreate(savedInstanceState);

            if (Intent.Extras != null)
            {
                _action = Intent.GetStringExtra(Constants.IntentActionLabel);
#if DEBUG
            System.Diagnostics.Debug.WriteLine("********************************************************************");
            System.Diagnostics.Debug.WriteLine("*********  SilentRemoteNotificationReplyActivity Action = " + _action);
            System.Diagnostics.Debug.WriteLine("********************************************************************");
#endif
            }
        }

        protected async override void OnStart()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("*******************************************************************");
            System.Diagnostics.Debug.WriteLine("*********  SilentRemoteNotificationReplyActivity OnStart  *********");
            System.Diagnostics.Debug.WriteLine("*******************************************************************");
            if (ViewModel == null)
                System.Diagnostics.Debug.WriteLine("******************** ChatViewModel is NULL  ***********************");
            else
                System.Diagnostics.Debug.WriteLine("******************** ChatViewModel is Valid  ***********************");
#endif

            base.OnStart();

            switch (_action)
            {
                case Constants.ActionEmergency :
#if DEBUG
            System.Diagnostics.Debug.WriteLine("*********  Action Emergency  *********");
#endif
                    await ViewModel?.ActivateChatPromptAsync(ChatPromptType.Emergency);
                    break;

                case Constants.ActionSafe :
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("*********  Action Safe  *********");
#endif
                    await ViewModel?.ActivateChatPromptAsync(ChatPromptType.SafetyCheck);
                    var success = await ViewModel?.SendMessage(Resources.GetString(Resource.String.im_safe));
                    break;

                default:
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("*********  Invalid Action  *********");
#endif
                    LaunchRemoteNotificationRelpyActivity();
                    break;

            }
            Finish();
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
            StartActivity(launchIntent);
        }

    }
}